using System;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // ── Verificación silenciosa de licencia ───────────────────────────
            // Guarda el Machine ID en temp (solo el desarrollador sabe dónde mirarlo)
            LicenseCheck.GuardarIdParaDiagnostico();

            // Si no está licenciado: salir sin abrir ninguna ventana, sin mensajes
            if (!LicenseCheck.EsValida())
                return;
            // ─────────────────────────────────────────────────────────────────

            // Capturar cualquier excepción no manejada en el hilo UI y mostrarla
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show(
                    "Error no capturado:\n\n" + e.Exception.GetType().Name + ": " + e.Exception.Message +
                    "\n\n" + e.Exception.StackTrace,
                    "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmConexion());
        }

        internal static readonly string RutaComercial =
            @"C:\Program Files (x86)\Compac\COMERCIAL\";

        // Cadena de conexión a la base de datos SQL de la empresa abierta.
        // Se asigna en FrmConexion cuando el usuario selecciona y abre una empresa.
        internal static string ConnStrEmpresa = "";

        // Ruta/nombre de la empresa abierta (se pasa a fAbreEmpresa).
        // Necesario para poder cerrar y reabrir el SDK y limpiar su estado interno.
        internal static string DirEmpresa = "";
    }
}
