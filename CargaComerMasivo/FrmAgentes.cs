using System;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    public partial class FrmAgentes : Form
    {
        private int  nIdAgenteActual = 0;
        private bool modoNuevo       = false;

        public FrmAgentes()
        {
            InitializeComponent();
            UITheme.AplicarIconoForm(this);
            CargarLista();
        }

        private void CargarLista()
        {
            cbAgente.Items.Clear();
            int total = SdkComercial.fObtenNumAgentes();
            for (int i = 0; i < total; i++)
            {
                string cod = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgenteNum(i, "cCodAgente",    sb, len));
                string nom = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgenteNum(i, "cNombreAgente", sb, len));
                int.TryParse(cod, out int codNum);
                int idReal = codNum > 0 ? SdkComercial.fBuscaIdAgente(codNum) : 0;
                if (idReal <= 0) idReal = codNum;
                cbAgente.Items.Add(new ItemAgente(idReal, cod, nom));
            }
            LimpiarCampos();
            SetEstado(false);
        }

        private void cbAgente_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cbAgente.SelectedItem as ItemAgente;
            if (item == null) return;
            CargarAgente(item.Id);
        }

        private void CargarAgente(int nId)
        {
            nIdAgenteActual = nId;
            modoNuevo       = false;

            txtCodigo.Text  = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cCodAgente",      sb, len));
            txtNombre.Text  = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cNombreAgente",   sb, len));
            txtRuta.Text    = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cRuta",           sb, len));
            txtZona.Text    = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cZona",           sb, len));
            txtComVenta.Text = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cComisionVenta", sb, len));
            txtComCobro.Text = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cComisionCobro", sb, len));

            string sTipo = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAgente(nId, "cTipoAgente", sb, len));
            int.TryParse(sTipo, out int tipo);
            switch (tipo)
            {
                case 1:  rbVentaCobro.Checked = true; break;
                case 2:  rbCobro.Checked      = true; break;
                default: rbVentas.Checked     = true; break;
            }

            SetEstado(true);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            modoNuevo       = true;
            nIdAgenteActual = 0;
            LimpiarCampos();
            cbAgente.SelectedIndex = -1;
            SetEstado(true);
            txtCodigo.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El codigo y el nombre son obligatorios.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tipo = rbVentaCobro.Checked ? 1 : (rbCobro.Checked ? 2 : 0);

            try
            {
                int res;
                if (modoNuevo)
                {
                    nIdAgenteActual = SdkComercial.fInsertaAgente();
                    if (nIdAgenteActual <= 0)
                        throw new Exception("fInsertaAgente error: " + nIdAgenteActual);
                }
                else
                {
                    res = SdkComercial.fEditaAgente(nIdAgenteActual);
                    if (res < 0)
                        throw new Exception("fEditaAgente error: " + res);
                }

                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cCodAgente",     txtCodigo.Text.Trim());
                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cNombreAgente",  txtNombre.Text.Trim());
                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cTipoAgente",    tipo.ToString());
                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cComisionVenta", txtComVenta.Text.Replace(",", "."));
                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cComisionCobro", txtComCobro.Text.Replace(",", "."));
                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cRuta",          txtRuta.Text.Trim());
                SdkComercial.fSetDatoAgente(nIdAgenteActual, "cZona",          txtZona.Text.Trim());

                res = SdkComercial.fGuardaAgente(nIdAgenteActual);
                if (res < 0)
                {
                    SdkComercial.fCancelarModificacionAgente();
                    throw new Exception("fGuardaAgente error: " + res);
                }

                SetEstado(lblEstado, "Agente guardado correctamente.", UITheme.Success);
                MessageBox.Show("Agente guardado exitosamente.", "Exito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarLista();
            }
            catch (Exception ex)
            {
                SetEstado(lblEstado, "Error al guardar agente.", UITheme.Danger);
                MessageBox.Show("Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (nIdAgenteActual <= 0 || modoNuevo) return;

            var confirm = MessageBox.Show(
                "Eliminar el agente '" + txtNombre.Text + "'?\nEsta accion no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            int res = SdkComercial.fBorraAgente(nIdAgenteActual);
            if (res >= 0)
            {
                SetEstado(lblEstado, "Agente eliminado.", UITheme.Success);
                CargarLista();
            }
            else
            {
                SetEstado(lblEstado, "Error al eliminar (codigo: " + res + ")", UITheme.Danger);
                MessageBox.Show("No se pudo eliminar el agente.\nCodigo: " + res, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e) => this.Close();

        private void LimpiarCampos()
        {
            txtCodigo.Clear();  txtNombre.Clear();
            txtComVenta.Text = "0.00"; txtComCobro.Text = "0.00";
            txtRuta.Clear();    txtZona.Clear();
            rbVentas.Checked = true;
            lblEstado.Text = "";
        }

        private void SetEstado(bool habilitado)
        {
            txtCodigo.Enabled    = habilitado;
            txtNombre.Enabled    = habilitado;
            txtComVenta.Enabled  = habilitado;
            txtComCobro.Enabled  = habilitado;
            txtRuta.Enabled      = habilitado;
            txtZona.Enabled      = habilitado;
            rbVentas.Enabled     = habilitado;
            rbVentaCobro.Enabled = habilitado;
            rbCobro.Enabled      = habilitado;
            btnGuardar.Enabled   = habilitado;
            btnEliminar.Enabled  = habilitado && !modoNuevo;
        }

        private static void SetEstado(System.Windows.Forms.Label lbl, string msg,
                                      System.Drawing.Color color)
        {
            lbl.Text      = msg;
            lbl.ForeColor = color;
        }

        private class ItemAgente
        {
            public int    Id     { get; }
            public string Codigo { get; }
            private string Nombre { get; }
            public ItemAgente(int id, string codigo, string nombre)
            { Id = id; Codigo = codigo; Nombre = nombre; }
            public override string ToString() => Codigo + " - " + Nombre;
        }
    }
}
