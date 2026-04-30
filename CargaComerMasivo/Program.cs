using System;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Capturar cualquier excepciÃ³n no manejada en el hilo UI y mostrarla
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
    }
}
