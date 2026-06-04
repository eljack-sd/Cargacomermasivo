using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    public partial class FrmConexion : Form
    {
        private static readonly string RUTA_COMERCIAL = Program.RutaComercial;
        private static readonly string CAC_INI        = Path.Combine(RUTA_COMERCIAL, "CAC.ini");

        // Parámetros leídos de CAC.ini
        // Contraseña: si CAC.ini la trae vacía, usamos la default de CONTPAQi
        private string _srv  = "localhost";
        private string _inst = "COMPAC";
        private string _user = "sa";
        private string _pass = "Compac1234"; // default CONTPAQi

        private readonly List<ItemEmpresa> _empresas = new List<ItemEmpresa>();

        // Cadena de conexión exitosa a SQL Server (apuntando a master).
        // Se reutiliza para construir la conexión a cada BD de empresa.
        private string _connBase = "";

        public FrmConexion()
        {
            InitializeComponent();
            UITheme.AplicarIconoForm(this);
            this.Shown += (s, e) => Inicializar();
        }

        private void Inicializar()
        {
            try
            {
                // 1 — Verificar que CONTPAQi esté instalado
                if (!Directory.Exists(RUTA_COMERCIAL))
                {
                    MessageBox.Show($"No se encontró CONTPAQi en:\n{RUTA_COMERCIAL}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Directory.SetCurrentDirectory(RUTA_COMERCIAL);

                // 2 — Leer parámetros de CAC.ini
                LeerCacIni();

                // 3 — Arrancar servicio MGW
                SetEstado("Iniciando servicio MGW...", System.Drawing.Color.DarkOrange);
                IniciarServicioMGW();

                // 4 — Inicializar SDK
                SetEstado("Inicializando SDK de CONTPAQi...", System.Drawing.Color.DarkOrange);
                SdkComercial.fSetNombrePAQ("Comercial");
                int resSdk = SdkComercial.fInicializaSDK();
                if (resSdk != 0)
                {
                    MessageBox.Show($"Error al inicializar SDK:\n{SdkComercial.DescribirError(resSdk)}",
                        "Error SDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 5 — Cargar empresas usando el SDK (igual que CONTPAQi)
                SetEstado("Leyendo empresas desde SDK...", System.Drawing.Color.DarkOrange);
                CargarEmpresas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // LEER CAC.INI
        // ─────────────────────────────────────────────────────────────────────
        private void LeerCacIni()
        {
            if (!File.Exists(CAC_INI)) return;
            foreach (string linea in File.ReadAllLines(CAC_INI))
            {
                string l = linea.Trim();
                if (l.StartsWith("#") || !l.Contains("=")) continue;
                string[] p = l.Split(new[] { '=' }, 2);
                string k = p[0].Trim().ToLower();
                string v = p[1].Trim();
                if (k == "servidor")  _srv  = v;
                if (k == "instancia") _inst = v;
                if (k == "usuario")   _user = v;
                // Solo sobreescribir contraseña si CAC.ini trae un valor real
                if (k == "password" && !string.IsNullOrEmpty(v)) _pass = v;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CARGAR EMPRESAS USANDO EL SDK (fPosPrimerEmpresa / fPosSiguienteEmpresa)
        // El SDK devuelve exactamente las mismas empresas que muestra CONTPAQi.
        // ─────────────────────────────────────────────────────────────────────
        private void CargarEmpresas()
        {
            _empresas.Clear();
            cbEmpresa.Items.Clear();

            try
            {
                var sbNombre = new StringBuilder(3000);
                var sbRuta   = new StringBuilder(3000);
                int idEmp    = 0;

                // Primera empresa
                int res = SdkComercial.fPosPrimerEmpresa(ref idEmp, sbNombre, sbRuta);
                while (res == 0)
                {
                    string nombre = sbNombre.ToString().Trim();
                    string ruta   = sbRuta.ToString().Trim();

                    if (!string.IsNullOrEmpty(ruta))
                    {
                        // Intentar conectar por SQL para tener cadena de conexión disponible
                        string csEmpresa = IntentarConexionSQL(ruta);
                        _empresas.Add(new ItemEmpresa(nombre, ruta, "", csEmpresa));
                    }

                    sbNombre.Clear();
                    sbRuta.Clear();
                    idEmp = 0;
                    res = SdkComercial.fPosSiguienteEmpresa(ref idEmp, sbNombre, sbRuta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer empresas del SDK:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (_empresas.Count == 0)
            {
                SetEstado("✘ No se encontraron empresas en el SDK.", System.Drawing.Color.OrangeRed);
                MessageBox.Show(
                    "El SDK no devolvió ninguna empresa registrada.\n\n" +
                    "Escribe la ruta de tu empresa manualmente.",
                    "Sin empresas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MostrarManual();
                return;
            }

            foreach (var e in _empresas) cbEmpresa.Items.Add(e);
            cbEmpresa.SelectedIndex = 0;
            btnAbrir.Enabled        = true;
            SetEstado($"✔ {_empresas.Count} empresa(s) encontrada(s). Selecciona y haz clic en Abrir.",
                System.Drawing.Color.Green);
        }

        // Intenta construir una cadena SQL para la empresa dada su ruta de datos.
        // Necesaria para que FrmPrincipal pueda cargar catálogos vía SQL.
        private string IntentarConexionSQL(string rutaEmpresa)
        {
            // Derivar nombre de BD desde la ruta (último segmento del path)
            string db = Path.GetFileName(rutaEmpresa.TrimEnd('\\', '/'));
            if (string.IsNullOrEmpty(db)) return "";

            var candidatos = ConstruirCandidatos();
            foreach (var par in candidatos)
            {
                try
                {
                    string cs = par.Value.Replace("Database=master", "Database=" + db);
                    using (var c = new SqlConnection(cs))
                    {
                        c.Open();
                        if (string.IsNullOrEmpty(_connBase)) _connBase = par.Value;
                        return cs;
                    }
                }
                catch { }
            }
            return "";
        }

        // Genera candidatos de conexión: primero el de CAC.ini, luego instancias comunes de CONTPAQi
        private System.Collections.Generic.Dictionary<string, string> ConstruirCandidatos()
        {
            var dic = new System.Collections.Generic.Dictionary<string, string>();

            // Instancias comunes que usa CONTPAQi en distintas versiones
            string[] instancias = new[]
            {
                _inst,              // ← PRIMERO la que está en CAC.ini de este equipo
                "COMPACSQL2022",    // patrón más común en instalaciones recientes
                "COMPACSQL2019",
                "COMPACSQL2017",
                "COMPACSQL2016",
                "COMPACSQL2014",
                "COMPACSQL2012",
                "COMPAC",
                "COMPACDB",
                "SQLEXPRESS",
                "CONTPAQ",
                "ADMINPAQ",
                "MSSQLSERVER",      // instancia default (sin nombre)
            };

            foreach (string inst in instancias)
            {
                string srv = string.IsNullOrEmpty(inst) || inst == "MSSQLSERVER"
                    ? "localhost"
                    : $"localhost\\{inst}";

                string key = srv;
                if (!dic.ContainsKey(key))
                    dic[key] = $"Server={srv};Database=master;User Id={_user};Password={_pass};Connect Timeout=4;";
            }

            // También probar con Windows Auth como último recurso
            dic["localhost (Windows Auth)"] =
                $"Server=localhost;Database=master;Integrated Security=True;Connect Timeout=4;";

            return dic;
        }

        // Devuelve todas las bases de datos que tienen tabla admParametros con CNombreEmpresa
        // (identificador confiable de una empresa CONTPAQi, sin importar el nombre de la BD)
        private List<string> ObtenerBasesEmpresa(string connStr)
        {
            var lista = new List<string>();
            try
            {
                // 1) Obtener TODAS las bases de datos de usuario (excluye las del sistema)
                var todasLasBases = new List<string>();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT name FROM sys.databases " +
                        "WHERE name NOT IN ('master','tempdb','model','msdb') " +
                        "  AND state_desc = 'ONLINE' " +
                        "ORDER BY name",
                        conn))
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            todasLasBases.Add(r[0].ToString());
                }

                // 2) Filtrar solo las que tienen admParametros con CNombreEmpresa
                //    (estructura estándar de todas las empresas CONTPAQi Comercial)
                string baseConn = connStr; // apunta a master
                foreach (string db in todasLasBases)
                {
                    try
                    {
                        string cs = baseConn.Replace("Database=master", "Database=" + db);
                        using (var conn = new SqlConnection(cs))
                        {
                            conn.Open();
                            using (var cmd = new SqlCommand(
                                "SELECT COUNT(*) FROM admParametros " +
                                "WHERE cNomParametro = 'CNombreEmpresa'",
                                conn))
                            {
                                int cnt = (int)cmd.ExecuteScalar();
                                if (cnt > 0)
                                    lista.Add(db);
                            }
                        }
                    }
                    catch { } // BD no accesible o sin estructura CONTPAQi → ignorar
                }
            }
            catch { }
            return lista;
        }

        // ─────────────────────────────────────────────────────────────────────
        // FALLBACK: ENTRADA MANUAL
        // ─────────────────────────────────────────────────────────────────────
        private void MostrarManual()
        {
            cbEmpresa.Visible     = false;
            lblEmpresaLbl.Text    = "Nombre de la empresa (como aparece en CONTPAQi):";
            txtRutaManual.Visible = true;
            btnBuscar.Visible     = true;
            lblManual.Visible     = true;
            btnAbrir.Enabled      = true;
            SetEstado("Escribe el nombre exacto de tu empresa y haz clic en Abrir.",
                System.Drawing.Color.DarkOrange);
        }

        // ─────────────────────────────────────────────────────────────────────
        // ABRIR EMPRESA
        // ─────────────────────────────────────────────────────────────────────
        private void btnAbrir_Click(object sender, EventArgs e)
        {
            string ruta = "";

            if (cbEmpresa.Visible && cbEmpresa.SelectedItem is ItemEmpresa emp)
            {
                ruta = string.IsNullOrEmpty(emp.Ruta) ? emp.Nombre : emp.Ruta;
                // Guardar la conexión SQL a la empresa seleccionada para que
                // FrmPrincipal pueda cargar catálogos desde SQL Server.
                Program.ConnStrEmpresa = emp.ConnStr;
            }
            else
                ruta = txtRutaManual.Text.Trim();

            if (string.IsNullOrEmpty(ruta))
            {
                MessageBox.Show("Selecciona o escribe la empresa.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnAbrir.Enabled = false;
            SetEstado("Abriendo empresa...", System.Drawing.Color.DarkOrange);

            int res = SdkComercial.fAbreEmpresa(ruta);

            if (res == 0)
            {
                Program.DirEmpresa = ruta; // guardar para poder reabrir el SDK después del sync
                SetEstado("✔ Empresa abierta correctamente.", System.Drawing.Color.Green);
                new FrmControl().Show();
                this.Hide();
            }
            else
            {
                string desc = SdkComercial.DescribirError(res);
                btnAbrir.Enabled = true;
                SetEstado($"✘ {desc}", System.Drawing.Color.Red);
                MessageBox.Show(
                    $"No se pudo abrir la empresa.\n\n" +
                    $"Ruta/nombre usado: {ruta}\n" +
                    $"Error: {desc}\n\n" +
                    "Asegúrate de que CONTPAQi no tenga esa empresa abierta.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Carpeta de datos de la empresa";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtRutaManual.Text = dlg.SelectedPath;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            SdkComercial.fTerminaSDK();
            Application.Exit();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }

        // ─────────────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────────────
        private void IniciarServicioMGW()
        {
            if (Process.GetProcessesByName("MGWServiciosADD").Length > 0) return;
            string exe = Path.Combine(RUTA_COMERCIAL, "MGWServiciosADD.exe");
            if (!File.Exists(exe)) return;
            try
            {
                Process.Start(new ProcessStartInfo(exe)
                {
                    WorkingDirectory = RUTA_COMERCIAL,
                    UseShellExecute  = true
                });
                Thread.Sleep(5000);
            }
            catch { }
        }

        private void SetEstado(string texto, System.Drawing.Color color)
        {
            lblEstado.Text      = texto;
            lblEstado.ForeColor = color;
            Application.DoEvents();
        }

        private class ItemEmpresa
        {
            public string Nombre  { get; }
            public string Ruta    { get; }
            public string DbName  { get; }
            public string ConnStr { get; }
            public ItemEmpresa(string n, string r, string db = "", string cs = "")
            {
                Nombre  = n;
                Ruta    = r;
                DbName  = db;
                ConnStr = cs;
            }
            public override string ToString() => Nombre;
        }
    }
}
