using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CargaComerMasivo
{
    // ─── Modelos de datos ─────────────────────────────────────────────────────
    [DataContract]
    internal class ProductoItem
    {
        [DataMember(Name = "codigo")] public string Codigo { get; set; }
        [DataMember(Name = "nombre")] public string Nombre { get; set; }
        [DataMember(Name = "unidad")] public string Unidad { get; set; }
    }

    [DataContract]
    internal class ClienteItem
    {
        [DataMember(Name = "codigo")]                 public string Codigo         { get; set; }
        [DataMember(Name = "razon_social")]           public string RazonSocial    { get; set; }
        [DataMember(Name = "denominacion_comercial")] public string DenomComercial { get; set; }
        [DataMember(Name = "rfc")]                    public string Rfc            { get; set; }
        [DataMember(Name = "direccion")]              public string Direccion      { get; set; }
    }

    [DataContract]
    internal class SyncResponse
    {
        [DataMember(Name = "productos_faltantes")] public List<ProductoItem> ProductosFaltantes { get; set; }
        [DataMember(Name = "clientes_faltantes")]  public List<ClienteItem>  ClientesFaltantes  { get; set; }
    }

    // ─── Lógica de sincronización ─────────────────────────────────────────────
    internal static class SincronizadorCatalogo
    {
        private const string API_SYNC_URL = "https://cafeelgrande.hades.vraia.com/ceg/catalog/sync";
        private const string API_KEY      = "uiZ0KTBmaFHArbxF0WMIfCOY9Yc88m30czyfBCccIQQ9BNTKob8o4fU3LKe26bXOrmtD1G7MGnHXbTPltCUA26YNAW809cuBWiK6BL5h3CjSco2upvLRQBpKoofkzvKh";
        private const int    BUF          = 500;

        // ── Extrae productos del catálogo CONTPAQi vía SQL ───────────────────
        // Más fiable que SDK: no depende de funciones de enumeración del DLL.
        public static List<ProductoItem> ExtraerProductos()
        {
            var lista = new List<ProductoItem>();
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa)) return lista;

            using (var conn = new SqlConnection(Program.ConnStrEmpresa))
            {
                conn.Open();

                // Intentamos traer la unidad via JOIN con admUnidades
                // Detectar columnas disponibles en admProductos
                bool tieneNombre   = ColumnExists(conn, "admProductos", "CNOMBREPRODUCTO");
                bool tieneFKUnidad = ColumnExists(conn, "admProductos", "CIDUNIDADINVENTARIO");
                bool tieneUnidades = ColumnExists(conn, "admUnidades",  "CNOMBREUNIDAD");

                string colNombre = tieneNombre ? "ISNULL(p.CNOMBREPRODUCTO,'')" : "p.CCODIGOPRODUCTO";
                string sqlFinal;

                if (tieneFKUnidad && tieneUnidades)
                {
                    sqlFinal = $@"
                        SELECT p.CCODIGOPRODUCTO,
                               {colNombre} AS CNOMBREPRODUCTO,
                               ISNULL(u.CNOMBREUNIDAD, '') AS CUNIDAD
                        FROM   admProductos p
                        LEFT JOIN admUnidades u ON p.CIDUNIDADINVENTARIO = u.CIDUNIDAD
                        ORDER BY p.CCODIGOPRODUCTO";
                }
                else
                {
                    sqlFinal = $@"
                        SELECT CCODIGOPRODUCTO,
                               {colNombre.Replace("p.","")} AS CNOMBREPRODUCTO,
                               '' AS CUNIDAD
                        FROM   admProductos
                        ORDER BY CCODIGOPRODUCTO";
                }

                using (var cmd = new SqlCommand(sqlFinal, conn))
                using (var r   = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        string cod = r.IsDBNull(0) ? "" : r[0].ToString().Trim();
                        if (string.IsNullOrEmpty(cod)) continue;
                        if (EsCodigoPlaceholder(cod)) continue; // ej. "(Ninguno)"
                        lista.Add(new ProductoItem
                        {
                            Codigo = Sanitize(cod),
                            Nombre = Sanitize(r.IsDBNull(1) ? "" : r[1].ToString().Trim()),
                            Unidad = Sanitize(r.IsDBNull(2) ? "" : r[2].ToString().Trim())
                        });
                    }
            }
            return lista;
        }

        // ── Extrae clientes/proveedores del catálogo CONTPAQi vía SQL ─────────
        public static List<ClienteItem> ExtraerClientes()
        {
            var lista = new List<ClienteItem>();
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa)) return lista;

            using (var conn = new SqlConnection(Program.ConnStrEmpresa))
            {
                conn.Open();

                // Detectar qué columnas existen realmente en admClientes
                bool tieneRazon   = ColumnExists(conn, "admClientes", "CRAZONSOCIAL");
                bool tieneNomCom  = ColumnExists(conn, "admClientes", "CNOMBRECOMERCIAL");
                bool tieneRfc     = ColumnExists(conn, "admClientes", "CRFC");
                bool tieneDirec   = ColumnExists(conn, "admClientes", "CDIRECCION");

                // Construir SELECT con solo las columnas que existen
                string colRazon  = tieneRazon  ? "ISNULL(CRAZONSOCIAL,'')"     : "''";
                string colNomCom = tieneNomCom ? "ISNULL(CNOMBRECOMERCIAL,'')" : "''";
                string colRfc    = tieneRfc    ? "ISNULL(CRFC,'')"             : "''";
                string colDirec  = tieneDirec  ? "ISNULL(CDIRECCION,'')"       : "''";

                string sql = $@"
                    SELECT CCODIGOCLIENTE,
                           {colRazon}  AS CRAZONSOCIAL,
                           {colNomCom} AS CNOMBRECOMERCIAL,
                           {colRfc}    AS CRFC,
                           {colDirec}  AS CDIRECCION
                    FROM   admClientes
                    ORDER BY CCODIGOCLIENTE";

                using (var cmd = new SqlCommand(sql, conn))
                using (var r   = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        string cod = r.IsDBNull(0) ? "" : r[0].ToString().Trim();
                        if (string.IsNullOrEmpty(cod)) continue;
                        if (EsCodigoPlaceholder(cod)) continue;
                        lista.Add(new ClienteItem
                        {
                            Codigo         = Sanitize(cod),
                            RazonSocial    = Sanitize(r.IsDBNull(1) ? "" : r[1].ToString().Trim()),
                            DenomComercial = Sanitize(r.IsDBNull(2) ? "" : r[2].ToString().Trim()),
                            Rfc            = Sanitize(r.IsDBNull(3) ? "" : r[3].ToString().Trim()),
                            Direccion      = Sanitize(r.IsDBNull(4) ? "" : r[4].ToString().Trim())
                        });
                    }
            }
            return lista;
        }

        // ── Filtra códigos genéricos de CONTPAQi que no son registros reales ──
        private static bool EsCodigoPlaceholder(string cod)
        {
            // CONTPAQi crea un registro "(Ninguno)" como valor por defecto en varios catálogos
            return cod.StartsWith("(") && cod.EndsWith(")");
        }

        // ── Limpia caracteres que ciertos parsers JSON del servidor no toleran ─
        private static string Sanitize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            // Elimina bytes nulos y reemplaza caracteres de sustitución unicode
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c == '\0') continue;          // byte nulo — rompe parsers estrictos
                if (c == '�') continue;      // caracter de reemplazo unicode
                sb.Append(c);
            }
            return sb.ToString();
        }

        // ── Detecta si una columna existe en una tabla ────────────────────────
        private static bool ColumnExists(SqlConnection conn, string tabla, string columna)
        {
            using (var cmd = new SqlCommand(
                "SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_NAME = @t AND COLUMN_NAME = @c", conn))
            {
                cmd.Parameters.AddWithValue("@t", tabla);
                cmd.Parameters.AddWithValue("@c", columna);
                return cmd.ExecuteScalar() != null;
            }
        }

        // ── Envía catálogo a la API y recibe los faltantes ────────────────────
        // Llamar desde Task.Run — hace red, no SDK.
        public static SyncResponse EnviarCatalogo(List<ProductoItem> productos, List<ClienteItem> clientes,
                                                   Action<string> log = null)
        {
            string json = BuildJson(productos, clientes);
            byte[] data = Encoding.UTF8.GetBytes(json);

            // Guardar copia del JSON enviado para diagnóstico (sin BOM)
            try
            {
                string ruta = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads", "sync_payload_debug.json");
                System.IO.File.WriteAllText(ruta, json, new UTF8Encoding(false)); // false = sin BOM
                log?.Invoke($"  → JSON generado: {data.Length / 1024} KB  ({productos.Count} productos, {clientes.Count} clientes)  — guardado en {ruta}");
            }
            catch { }

            var req = (HttpWebRequest)WebRequest.Create(API_SYNC_URL);
            req.Method        = "POST";
            req.ContentType   = "application/json";
            req.ContentLength = data.Length;
            req.Timeout       = 120000; // 2 min — puede ser lento con catálogos grandes
            req.Headers.Add("X-CEG-API-Key", API_KEY);

            using (var s = req.GetRequestStream())
                s.Write(data, 0, data.Length);

            using (var resp = (HttpWebResponse)req.GetResponse())
            using (var sr   = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                string body = sr.ReadToEnd();
                return ParseResponse(body);
            }
        }

        // ── Crea un producto faltante en CONTPAQi vía SDK ─────────────────────
        // DEBE llamarse desde el UI thread (P/Invoke CONTPAQi).
        public static bool CrearProducto(ProductoItem p, out string error)
        {
            error = null;
            // fInsertaProducto() pone el SDK en modo inserción; retorna 0=ok, <0=error
            int res = SdkComercial.fInsertaProducto();
            if (res < 0)
            {
                error = $"fInsertaProducto: {SdkComercial.DescribirError(res)}";
                return false;
            }

            // fSetDatoProducto opera sobre el registro activo — NO lleva ID
            SdkComercial.fSetDatoProducto("CCODIGOPRODUCTO", p.Codigo);
            SdkComercial.fSetDatoProducto("CNOMBREPRODUCTO", string.IsNullOrEmpty(p.Nombre) ? p.Codigo : p.Nombre);
            // CESTATUS=0 → activo en CONTPAQi Comercial (0=activo, 1=inactivo)
            SdkComercial.fSetDatoProducto("CSTATUSPRODUCTO", "1");
            if (!string.IsNullOrEmpty(p.Unidad))
                SdkComercial.fSetDatoProducto("CUNIDADVENTA", p.Unidad);

            res = SdkComercial.fGuardaProducto();
            // fGuardaProducto: 0=éxito, cualquier otro valor (positivo o negativo) = error
            if (res != 0)
            {
                try { SdkComercial.fCancelarModificacionProducto(); } catch { }
                error = $"fGuardaProducto '{p.Codigo}': [{res}] {SdkComercial.DescribirError(res)}";
                return false;
            }
            return true;
        }

        // ── Crea un cliente faltante en CONTPAQi vía SDK ──────────────────────
        // DEBE llamarse desde el UI thread (P/Invoke CONTPAQi).
        public static bool CrearCliente(ClienteItem c, out string error)
        {
            error = null;
            int r1 = SdkComercial.fInsertaCteProv();
            if (r1 < 0) { error = $"fInsertaCteProv: {SdkComercial.DescribirError(r1)}"; return false; }

            SdkComercial.fSetDatoCteProv("CCODIGOCLIENTE",      c.Codigo);
            SdkComercial.fSetDatoCteProv("CRAZONSOCIAL",        c.RazonSocial ?? c.Codigo);
            // CNOMBRECOMERCIAL devuelve error 73 (campo inválido en SDK) — se omite
            SdkComercial.fSetDatoCteProv("CTIPOCLIENTE",        "1");
            // CESTATUS=1 → activo
            SdkComercial.fSetDatoCteProv("CESTATUS",            "1");
            // Lista de precios requerida por CONTPAQi — sin esto fGuarda devuelve 120513
            SdkComercial.fSetDatoCteProv("CLISTAPRECIOCLIENTE", "1");
            if (!string.IsNullOrEmpty(c.Rfc))
                SdkComercial.fSetDatoCteProv("CRFC", c.Rfc);

            int r7 = SdkComercial.fGuardaCteProv();
            // fGuardaCteProv: 0=éxito, cualquier otro valor (positivo o negativo) = error
            if (r7 != 0)
            {
                try { SdkComercial.fCancelarModificacionCteProv(); } catch { }
                error = $"fGuardaCteProv '{c.Codigo}': [{r7}] {SdkComercial.DescribirError(r7)}";
                return false;
            }
            return true;
        }

        // ── Diagnóstico binario — encuentra el registro que causa 400 ─────────
        // Devuelve una descripción del elemento problemático, o null si no lo encuentra.
        public static string DiagnosticarBinarySearch(List<ProductoItem> productos, List<ClienteItem> clientes,
                                                       Action<string> log = null)
        {
            // 1. Confirmar que con lista vacía responde 200
            if (!EsOk(new List<ProductoItem>(), new List<ClienteItem>()))
            {
                log?.Invoke("  ERROR: incluso con listas vacías el servidor devuelve 400.");
                return "Falla con catálogo vacío — problema en el servidor o API key.";
            }

            // 2. Buscar producto problemático
            string prodProblem = BinarySearchProducto(productos, clientes, log);
            // 3. Buscar cliente problemático (siempre, por si hay más de uno)
            string cliProblem  = BinarySearchCliente(productos, clientes, log);

            if (prodProblem == null && cliProblem == null)
                return "No se encontró ningún registro problemático de forma individual.";

            return string.Join("\n", new[] { prodProblem, cliProblem }.Where(x => x != null));
        }

        private static string BinarySearchProducto(List<ProductoItem> todos, List<ClienteItem> cliVacios,
                                                    Action<string> log)
        {
            int lo = 0, hi = todos.Count - 1;
            log?.Invoke($"  [PROD] Búsqueda binaria en {todos.Count} productos...");

            // Primero verificar que el bloque completo de productos falla
            if (EsOk(todos, cliVacios))
            {
                log?.Invoke("  [PROD] Todos los productos juntos pasan — sin problema en productos.");
                return null;
            }

            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                var mitad = todos.GetRange(lo, mid - lo + 1);
                log?.Invoke($"  [PROD] Probando índices {lo}–{mid} ({mitad.Count} items)...");
                if (!EsOk(mitad, new List<ClienteItem>()))
                    hi = mid;
                else
                    lo = mid + 1;
            }
            var malo = todos[lo];
            log?.Invoke($"  [PROD] *** PROBLEMA en índice {lo}: codigo=\"{malo.Codigo}\" nombre=\"{malo.Nombre}\" unidad=\"{malo.Unidad}\"");
            return $"PRODUCTO problemático [{lo}]: codigo=\"{malo.Codigo}\" | nombre=\"{malo.Nombre}\" | unidad=\"{malo.Unidad}\"";
        }

        private static string BinarySearchCliente(List<ProductoItem> prodVacios, List<ClienteItem> todos,
                                                   Action<string> log)
        {
            int lo = 0, hi = todos.Count - 1;
            log?.Invoke($"  [CLI] Búsqueda binaria en {todos.Count} clientes...");

            if (EsOk(prodVacios, todos))
            {
                log?.Invoke("  [CLI] Todos los clientes juntos pasan — sin problema en clientes.");
                return null;
            }

            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                var mitad = todos.GetRange(lo, mid - lo + 1);
                log?.Invoke($"  [CLI] Probando índices {lo}–{mid} ({mitad.Count} items)...");
                if (!EsOk(new List<ProductoItem>(), mitad))
                    hi = mid;
                else
                    lo = mid + 1;
            }
            var malo = todos[lo];
            log?.Invoke($"  [CLI] *** PROBLEMA en índice {lo}: codigo=\"{malo.Codigo}\" razon=\"{malo.RazonSocial}\" rfc=\"{malo.Rfc}\"");
            return $"CLIENTE problemático [{lo}]: codigo=\"{malo.Codigo}\" | razon_social=\"{malo.RazonSocial}\" | rfc=\"{malo.Rfc}\"";
        }

        private static bool EsOk(List<ProductoItem> prods, List<ClienteItem> clis)
        {
            try
            {
                string json = BuildJson(prods, clis);
                byte[] data = Encoding.UTF8.GetBytes(json);
                var req = (HttpWebRequest)WebRequest.Create(API_SYNC_URL);
                req.Method        = "POST";
                req.ContentType   = "application/json";
                req.ContentLength = data.Length;
                req.Timeout       = 30000;
                req.Headers.Add("X-CEG-API-Key", API_KEY);
                using (var s = req.GetRequestStream())
                    s.Write(data, 0, data.Length);
                using (var resp = (HttpWebResponse)req.GetResponse())
                    return resp.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse r && r.StatusCode == HttpStatusCode.BadRequest)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        // ── Helpers JSON ──────────────────────────────────────────────────────
        private static string BuildJson(List<ProductoItem> prods, List<ClienteItem> clis)
        {
            var sb = new StringBuilder(prods.Count * 60 + clis.Count * 80);
            sb.Append("{\"productos\":[");
            for (int i = 0; i < prods.Count; i++)
            {
                if (i > 0) sb.Append(',');
                var p = prods[i];
                // El servidor rechaza unidad vacía — usar "PZA" cuando no hay unidad registrada
                string unidad = string.IsNullOrEmpty(p.Unidad) ? "PZA" : p.Unidad;
                sb.Append("{\"codigo\":").Append(Js(p.Codigo))
                  .Append(",\"nombre\":").Append(Js(p.Nombre))
                  .Append(",\"unidad\":").Append(Js(unidad))
                  .Append('}');
            }
            sb.Append("],\"clientes\":[");
            for (int i = 0; i < clis.Count; i++)
            {
                if (i > 0) sb.Append(',');
                var c = clis[i];
                sb.Append("{\"codigo\":").Append(Js(c.Codigo))
                  .Append(",\"razon_social\":").Append(Js(c.RazonSocial))
                  .Append(",\"denominacion_comercial\":").Append(Js(c.DenomComercial))
                  .Append(",\"rfc\":").Append(Js(c.Rfc))
                  .Append(",\"direccion\":").Append(Js(c.Direccion))
                  .Append('}');
            }
            sb.Append("]}");
            return sb.ToString();
        }

        // Escapa un string para JSON — cubre todos los caracteres de control (0x00-0x1F)
        private static string Js(string s)
        {
            if (s == null) return "\"\"";
            var sb = new StringBuilder(s.Length + 4);
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:
                        if (c < 0x20)
                            sb.AppendFormat("\\u{0:x4}", (int)c); // otros chars de control
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static SyncResponse ParseResponse(string json)
        {
            var ser = new DataContractJsonSerializer(typeof(SyncResponse));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                return (SyncResponse)ser.ReadObject(ms);
        }
    }
}
