using System;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    public partial class FrmAlmacenes : Form
    {
        private int  nIdAlmacenActual = 0;
        private bool modoNuevo        = false;

        public FrmAlmacenes()
        {
            InitializeComponent();
            UITheme.AplicarIconoForm(this);
            CargarLista();
        }

        private void CargarLista()
        {
            cbAlmacen.Items.Clear();
            int total = SdkComercial.fObtenNumAlmacenes();
            for (int i = 0; i < total; i++)
            {
                string cod = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAlmacenNum(i, "cCodAlmacen",    sb, len));
                string nom = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAlmacenNum(i, "cNombreAlmacen", sb, len));
                int idReal = !string.IsNullOrEmpty(cod) ? SdkComercial.fBuscaAlmacen(cod) : 0;
                cbAlmacen.Items.Add(new ItemAlmacen(idReal, cod, nom));
            }
            LimpiarCampos();
            SetEstado(false);
        }

        private void cbAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cbAlmacen.SelectedItem as ItemAlmacen;
            if (item == null) return;
            CargarAlmacen(item.Id);
        }

        private void CargarAlmacen(int nId)
        {
            nIdAlmacenActual = nId;
            modoNuevo        = false;

            txtCodigo.Text    = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAlmacen(nId, "cCodAlmacen",    sb, len));
            txtNombre.Text    = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAlmacen(nId, "cNombreAlmacen", sb, len));
            txtTextoExt1.Text = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAlmacen(nId, "cTxtExt01",      sb, len));
            txtTextoExt2.Text = SdkComercial.LeerCampo((sb, len) => SdkComercial.fLeeDatoAlmacen(nId, "cTxtExt02",      sb, len));

            SetEstado(true);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            modoNuevo        = true;
            nIdAlmacenActual = 0;
            LimpiarCampos();
            cbAlmacen.SelectedIndex = -1;
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

            try
            {
                int res;
                if (modoNuevo)
                {
                    nIdAlmacenActual = SdkComercial.fInsertaAlmacen();
                    if (nIdAlmacenActual <= 0)
                        throw new Exception("fInsertaAlmacen error: " + nIdAlmacenActual);
                }
                else
                {
                    res = SdkComercial.fEditaAlmacen(nIdAlmacenActual);
                    if (res < 0)
                        throw new Exception("fEditaAlmacen error: " + res);
                }

                SdkComercial.fSetDatoAlmacen(nIdAlmacenActual, "cCodAlmacen",    txtCodigo.Text.Trim());
                SdkComercial.fSetDatoAlmacen(nIdAlmacenActual, "cNombreAlmacen", txtNombre.Text.Trim());
                SdkComercial.fSetDatoAlmacen(nIdAlmacenActual, "cTxtExt01",      txtTextoExt1.Text.Trim());
                SdkComercial.fSetDatoAlmacen(nIdAlmacenActual, "cTxtExt02",      txtTextoExt2.Text.Trim());

                res = SdkComercial.fGuardaAlmacen(nIdAlmacenActual);
                if (res < 0)
                {
                    SdkComercial.fCancelarModificacionAlmacen();
                    throw new Exception("fGuardaAlmacen error: " + res);
                }

                Estatus(lblEstado, "Almacen guardado correctamente.", UITheme.Success);
                MessageBox.Show("Almacen guardado exitosamente.", "Exito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarLista();
            }
            catch (Exception ex)
            {
                Estatus(lblEstado, "Error al guardar almacen.", UITheme.Danger);
                MessageBox.Show("Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (nIdAlmacenActual <= 0 || modoNuevo) return;

            var confirm = MessageBox.Show(
                "Eliminar el almacen '" + txtNombre.Text + "'?\nEsta accion no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            int res = SdkComercial.fBorraAlmacen(nIdAlmacenActual);
            if (res >= 0)
            {
                Estatus(lblEstado, "Almacen eliminado.", UITheme.Success);
                CargarLista();
            }
            else
            {
                Estatus(lblEstado, "Error al eliminar (codigo: " + res + ")", UITheme.Danger);
                MessageBox.Show("No se pudo eliminar el almacen.\nCodigo: " + res, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e) => this.Close();

        private void LimpiarCampos()
        {
            txtCodigo.Clear(); txtNombre.Clear();
            txtTextoExt1.Clear(); txtTextoExt2.Clear();
            lblEstado.Text = "";
        }

        private void SetEstado(bool habilitado)
        {
            txtCodigo.Enabled    = habilitado;
            txtNombre.Enabled    = habilitado;
            txtTextoExt1.Enabled = habilitado;
            txtTextoExt2.Enabled = habilitado;
            btnGuardar.Enabled   = habilitado;
            btnEliminar.Enabled  = habilitado && !modoNuevo;
        }

        private static void Estatus(System.Windows.Forms.Label lbl, string msg,
                                    System.Drawing.Color color)
        {
            lbl.Text      = msg;
            lbl.ForeColor = color;
        }

        private class ItemAlmacen
        {
            public int    Id     { get; }
            public string Codigo { get; }
            private string Nombre { get; }
            public ItemAlmacen(int id, string codigo, string nombre)
            { Id = id; Codigo = codigo; Nombre = nombre; }
            public override string ToString() => Codigo + " - " + Nombre;
        }
    }
}
