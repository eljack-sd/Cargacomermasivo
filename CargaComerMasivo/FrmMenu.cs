using System;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    public partial class FrmMenu : Form
    {
        public FrmMenu()
        {
            InitializeComponent();
        }

        private void btnDocumentos_Click(object sender, EventArgs e)
        {
            new FrmPrincipal().Show();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            new FrmCancelaDoctos().Show();
        }

        private void btnAgentes_Click(object sender, EventArgs e)
        {
            new FrmAgentes().Show();
        }

        private void btnAlmacenes_Click(object sender, EventArgs e)
        {
            new FrmAlmacenes().Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Â¿Deseas cerrar la empresa y salir?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SdkComercial.fCierraEmpresa();
                SdkComercial.fTerminaSDK();
                Application.Exit();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SdkComercial.fCierraEmpresa();
            SdkComercial.fTerminaSDK();
            base.OnFormClosed(e);
            Application.Exit();
        }
    }
}
