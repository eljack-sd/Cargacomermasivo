using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CargaComerMasivo
{
    // ─────────────────────────────────────────────────────────────────────────
    // Structs "Alto nivel" — deben coincidir con los declarados en MGWServicios.dll
    // Verificados contra el repo JavierOC-Edu/SDKs_CONTPAQi (SDK_C# / MGWServicios.cs)
    // Pack=4: alineacion maxima de 4 bytes (igual que el DLL x86)
    // ─────────────────────────────────────────────────────────────────────────
    // Longitudes de campo — iguales a constantes.kLong* del SDK oficial.
    // El curso oficial SDK CONTPAQi 21/03/2025 usa SizeConst = constantes.kLong*
    // SIN +1: kLong ya incluye el terminador nulo (total de bytes del campo en el DLL).
    internal static class Klong
    {
        public const int Codigo      = 31;   // kLongCodigo      = 31  (30 chars + '\0')
        public const int Serie       = 12;   // kLongSerie       = 12  (11 chars + '\0')
        public const int Fecha       = 24;   // kLongFecha       = 24  (23 chars + '\0')
        public const int Referencia  = 21;   // kLongReferencia  = 21  (20 chars + '\0')
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct tDocumento
    {
        public double aFolio;           // Numero de folio (Double)
        public int    aNumMoneda;       // ID de moneda (1=MXN, 2=USD, ...)
        public double aTipoCambio;      // Tipo de cambio (1.0 para MXN)
        public double aImporte;         // Importe total (0 = SDK lo calcula)
        public double aDescuentoDoc1;   // Descuento nivel documento 1
        public double aDescuentoDoc2;   // Descuento nivel documento 2
        public int    aSistemaOrigen;   // 205 = CONTPAQi Comercial Premium

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Codigo)]
        public string aCodConcepto;     // Codigo del concepto (ej. "OCC", "FAC")

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Serie)]
        public string aSerie;           // Serie del documento

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Fecha)]
        public string aFecha;           // Fecha "MM/dd/yyyy"

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Codigo)]
        public string aCodigoCteProv;   // Codigo del cliente/proveedor

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Codigo)]
        public string aCodigoAgente;    // Codigo del agente (vacio si no aplica)

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Referencia)]
        public string aReferencia;      // Referencia

        public int    aAfecta;          // 1=entradas, 2=salidas, 3=ninguno
        public int    aGasto1;          // int, no double (confirmado curso oficial 21/03/2025)
        public int    aGasto2;
        public int    aGasto3;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct tMovimiento
    {
        public int    aConsecutivo;     // Numero de renglon (1, 2, 3...)
        public double aUnidades;        // Cantidad
        public double aPrecio;          // Precio unitario
        public double aCosto;           // Costo unitario

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Codigo)]
        public string aCodProdSer;      // Codigo del producto/servicio

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Codigo)]
        public string aCodAlmacen;      // Codigo del almacen

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Referencia)]
        public string aReferencia;      // Referencia del movimiento

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Klong.Codigo)]
        public string aCodClasificacion; // Codigo de clasificacion (vacio si no aplica)
    }

    /// <summary>
    /// Wrapper P/Invoke para MGWServicios.dll — SDK de CONTPAQi Comercial Premium.
    /// Flujo correcto (API "Alto nivel"):
    ///   1. fAltaDocumento(ref idDoc, ref tDocumento)   → 0=ok / neg=error
    ///   2. fSetDatoDocumento("COBSERVACIONES", obs)    → opcional
    ///   3. fAltaMovimiento(idDoc, ref idMov, ref tMov) → 0=ok / neg=error
    ///   4. fSetDatoMovimiento("COBSERVAMOV", obs)      → opcional
    ///   5. fGuardaMovimiento()                         → 0=ok / neg=error
    ///   6. (repite 3-5 por cada linea)
    ///   7. fGuardaDocumento()                          → 0=ok / neg=error
    ///   En caso de error: fCancelarModificacionDocumento()
    /// </summary>
    public static class SdkComercial
    {
        private const string DLL = "MGWServicios.dll";
        private const int BUF = 3000;

        // ── Inicializacion ────────────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fSetNombrePAQ(string aNombrePAQ);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fInicializaSDK();

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern void fTerminaSDK();

        // ── Empresa ───────────────────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fAbreEmpresa(string aDirectorioEmpresa);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fCierraEmpresa();

        // Navega por la lista de empresas registradas en CONTPAQi.
        // Retorna 0=ok, -1=no hay / fin de lista.
        // aDirEmpresa = ruta de datos que se pasa a fAbreEmpresa.
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fPosPrimerEmpresa(ref int idEmpresa, StringBuilder aNombreEmpresa, StringBuilder aDirEmpresa);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fPosSiguienteEmpresa(ref int idEmpresa, StringBuilder aNombreEmpresa, StringBuilder aDirEmpresa);

        // ── Errores ───────────────────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern void fError(int aNumError, StringBuilder aMensaje, int aLen);

        public static string DescribirError(int codigo)
        {
            var sb = new StringBuilder(BUF);
            try { fError(codigo, sb, BUF); } catch { }
            string msg = sb.ToString().Trim();
            return string.IsNullOrEmpty(msg) ? "Codigo " + codigo : "[" + codigo + "] " + msg;
        }

        // ── Clientes / Proveedores ────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fBuscaCteProv(string sCodigo);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fBuscaIdCteProv(int nIdCteProv);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fLeeDatoCteProv(int nIdCteProv, string sCampo, StringBuilder sValor, int aLen);

        // ── Agentes ───────────────────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fBuscaIdAgente(int nCodigo);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fLeeDatoAgente(int nIdAgente, string sCampo, StringBuilder sValor, int aLen);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fInsertaAgente();

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fEditaAgente(int nIdAgente);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fSetDatoAgente(int nIdAgente, string sCampo, string sValor);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fGuardaAgente(int nIdAgente);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fCancelarModificacionAgente();

        public static int fBorraAgente(int nIdAgente)                                              { return -1; }
        public static int fObtenNumAgentes()                                                        { return 0; }
        public static int fLeeDatoAgenteNum(int nPos, string sCampo, StringBuilder sb, int len)    { return -1; }

        // ── Almacenes ─────────────────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fBuscaAlmacen(string sCodigo);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fLeeDatoAlmacen(int nIdAlmacen, string sCampo, StringBuilder sValor, int aLen);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fEditaAlmacen(int nIdAlmacen);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fSetDatoAlmacen(int nIdAlmacen, string sCampo, string sValor);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fGuardaAlmacen(int nIdAlmacen);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fCancelarModificacionAlmacen();

        public static int fInsertaAlmacen()                                                                 { return -1; }
        public static int fBorraAlmacen(int nIdAlmacen)                                                    { return -1; }
        public static int fObtenNumAlmacenes()                                                              { return 0; }
        public static int fLeeDatoAlmacenNum(int nPos, string sCampo, StringBuilder sb, int len)           { return -1; }

        // ── Conceptos ─────────────────────────────────────────────────────────
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fBuscaConceptoDocto(string sCodigo);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fBuscaIdConceptoDocto(int nIdConcepto);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fLeeDatoConceptoDocto(int nIdConcepto, string sCampo, StringBuilder sValor, int aLen);

        public static int fObtenNumConceptos()                                                              { return 0; }
        public static int fLeeDatoConceptoNum(int nPos, string sCampo, StringBuilder sb, int len)          { return -1; }

        // ── Folios ────────────────────────────────────────────────────────────
        // Firma oficial: fSiguienteFolio(aCodigoConcepto:CADENA, aSerie:CADENA ref, aFolio:DOUBLE ref)
        // Retorna 0=ok; aSerie y aFolio se rellenan con el siguiente folio disponible.
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fSiguienteFolio(string aCodigoConcepto, StringBuilder aSerie, ref double aFolio);

        // ── Documentos — API "Bajo nivel" (fInsertarDocumento) ────────────────
        // Disponible pero NO usada: preferir API Alto nivel (fAltaDocumento).
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fInsertarDocumento();

        // ── Documentos — API "Alto nivel" (fAltaDocumento) ───────────────────
        //
        // FLUJO OBLIGATORIO:
        //  1. fAltaDocumento(ref idDoc, ref tDocumento) → 0=ok / neg=error
        //  2. fSetDatoDocumento("COBSERVACIONES", obs)  → opcional
        //  3. fAltaMovimiento(idDoc, ref idMov, ref tMovimiento) → 0=ok / neg=error
        //  4. fSetDatoMovimiento("COBSERVAMOV", obs)    → opcional
        //  5. fGuardaMovimiento()                       → 0=ok / neg=error
        //  6. (repite 3-5 por cada renglon)
        //  7. fGuardaDocumento()                        → 0=ok / neg=error
        //  En error: fCancelarModificacionDocumento()
        //
        // Firmas verificadas contra JavierOC-Edu/SDKs_CONTPAQi (MGWServicios.cs)
        // y analisis Ghidra del EXE original (PUSH EAX struct, PUSH addr idDoc, RET 8).

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fAltaDocumento(ref int aIdDocumento, ref tDocumento atDocumento);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fAltaMovimiento(int aIdDocumento, ref int aIdMovimiento, ref tMovimiento atMovimiento);

        // Establece un campo del documento actual (sin ID — usa contexto global)
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fSetDatoDocumento(string sCampo, string sValor);

        // Lee un campo del documento actual (sin ID — usa contexto global)
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fLeeDatoDocumento(string sCampo, StringBuilder sValor, int nLen);

        // Agrega un movimiento al documento actual (bajo nivel — no usado con fAltaMovimiento)
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fInsertarMovimiento();

        // Establece un campo del movimiento actual
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fSetDatoMovimiento(string sCampo, string sValor);

        // Lee un campo del movimiento actual
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fLeeDatoMovimiento(string sCampo, StringBuilder sValor, int nLen);

        // Guarda el movimiento actual
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fGuardaMovimiento();

        // Guarda el documento actual; retorna 0=ok o negativo=error
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fGuardaDocumento();

        // Cancela el documento en edicion sin guardarlo
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fCancelarModificacionDocumento();

        // Cancela cambios al movimiento activo
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fCancelaCambiosMovimiento();

        // Borra el documento actual (debe estar posicionado primero)
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fBorraDocumento();

        // Buscar documento existente por concepto+serie+folio.
        // Firma oficial: fBuscarDocumento(aCodConcepto:CADENA, aSerie:CADENA, aFolio:CADENA): ENTERO
        // Retorna 0=ok (documento posicionado en contexto global); negativo=no encontrado/error.
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int fBuscarDocumento(string aCodConcepto, string aSerie, string aFolio);

        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fBuscarIdDocumento(int nIdDocumento);

        // Pone el documento posicionado en modo edicion.
        // Firma oficial: fEditarDocumento(): ENTERO  (sin parametros)
        // Retorna 0=ok; negativo=error.
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fEditarDocumento();

        // Cancela (da de baja logica) el documento en edicion.
        // Firma oficial: fCancelaDocumento(): ENTERO  (sin parametros)
        // Flujo correcto: fBuscarDocumento → fEditarDocumento → fCancelaDocumento → fGuardaDocumento
        [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
        public static extern int fCancelaDocumento();

        // ── Helper ────────────────────────────────────────────────────────────
        public static string LeerCampo(Func<StringBuilder, int, int> accion)
        {
            var sb = new StringBuilder(BUF);
            try { accion(sb, BUF); } catch { }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Lee un campo del documento actualmente en contexto.
        /// </summary>
        public static string LeerCampoDocumento(string campo)
        {
            var sb = new StringBuilder(BUF);
            try { fLeeDatoDocumento(campo, sb, BUF); } catch { }
            return sb.ToString().Trim();
        }
    }
}
