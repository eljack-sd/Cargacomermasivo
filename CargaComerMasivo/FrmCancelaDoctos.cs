using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    public partial class FrmCancelaDoctos : Form
    {
        // Código del concepto actualmente buscado (para el flujo fBuscarDocumento)
        private string _codConceptoActual = "";

        public FrmCancelaDoctos()
        {
            InitializeComponent();
            CargarConceptos();
            ActualizarBotones();
        }

        // ─────────────────────────────────────────────────────────────────────
        // CARGAR CONCEPTOS vía SQL (igual que FrmPrincipal)
        // ─────────────────────────────────────────────────────────────────────
        private void CargarConceptos()
        {
            cbConcepto.Items.Clear();

            if (string.IsNullOrEmpty(Program.ConnStrEmpresa)) return;

            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT CIDCONCEPTODOCUMENTO, CCODIGOCONCEPTO, CNOMBRECONCEPTO " +
                        "FROM admConceptos ORDER BY CCODIGOCONCEPTO",
                        conn))
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            int    id  = r.IsDBNull(0) ? 0  : Convert.ToInt32(r[0]);
                            string cod = r.IsDBNull(1) ? "" : r[1].ToString().Trim();
                            string nom = r.IsDBNull(2) ? "" : r[2].ToString().Trim();
                            cbConcepto.Items.Add(new ItemCombo(id, cod + " – " + nom, cod));
                        }
                }
            }
            catch { /* sin conexión SQL */ }

            if (cbConcepto.Items.Count > 0) cbConcepto.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        // BUSCAR — consulta SQL todos los documentos del concepto seleccionado
        // ─────────────────────────────────────────────────────────────────────
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            dgvDocumentos.DataSource = null;
            SetEstado("", false);
            ActualizarBotones();

            if (cbConcepto.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un concepto.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = (ItemCombo)cbConcepto.SelectedItem;
            _codConceptoActual = item.Code;

            if (string.IsNullOrEmpty(Program.ConnStrEmpresa))
            {
                SetEstado("Sin conexión a la base de datos.", true);
                return;
            }

            var dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    // CIDCTEPROVVENTA es el FK estándar de admDocumentos → admClientes
                    var sql =
                        "SELECT d.CIDDOCUMENTO AS ID, " +
                        "       d.CFOLIO       AS Folio, " +
                        "       d.CSERIE       AS Serie, " +
                        "       CONVERT(varchar(10), d.CFECHA, 103) AS Fecha, " +
                        "       ISNULL(cl.CRAZONSOCIAL, '') AS Cliente, " +
                        "       d.CTOTAL       AS Total " +
                        "FROM   admDocumentos d " +
                        "INNER JOIN admConceptos co " +
                        "       ON d.CIDCONCEPTODOCUMENTO = co.CIDCONCEPTODOCUMENTO " +
                        "LEFT  JOIN admClientes cl " +
                        "       ON d.CIDCTEPROVVENTA = cl.CIDCLIENTEPROVEEDOR " +
                        "WHERE  co.CCODIGOCONCEPTO = @cod " +
                        "ORDER  BY d.CFOLIO DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", _codConceptoActual);
                        using (var da = new SqlDataAdapter(cmd))
                            da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                SetEstado("Error SQL: " + ex.Message, true);
                return;
            }

            // Formatear columna Total a moneda
            foreach (DataRow row in dt.Rows)
            {
                if (row["Total"] != DBNull.Value)
                {
                    try
                    {
                        decimal tot = Convert.ToDecimal(row["Total"]);
                        row["Total"] = tot.ToString("N2");
                    }
                    catch { }
                }
            }

            dgvDocumentos.DataSource = dt;

            // Ocultar columna ID (se usa internamente para posicionar)
            if (dgvDocumentos.Columns.Contains("ID"))
                dgvDocumentos.Columns["ID"].Visible = false;

            // Anchos sugeridos
            AjustarColumnas();

            int n = dt.Rows.Count;
            lblConteo.Text = n == 0
                ? "No se encontraron documentos para este concepto."
                : n + " documento(s) encontrado(s). Selecciona los que deseas borrar.";

            SetEstado("", false);
            ActualizarBotones();
        }

        // ─────────────────────────────────────────────────────────────────────
        // BORRAR SELECCIONADOS
        // Flujo por cada fila: fBuscarDocumento(cod, serie, folio) → fBorraDocumento()
        // ─────────────────────────────────────────────────────────────────────
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (dgvDocumentos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona al menos un documento.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int total   = dgvDocumentos.SelectedRows.Count;
            string msg  = total == 1
                ? "¿Confirmas borrar físicamente 1 documento?\n\nEsta acción NO se puede deshacer."
                : $"¿Confirmas borrar físicamente {total} documentos?\n\nEsta acción NO se puede deshacer.";

            if (MessageBox.Show(msg, "Confirmar borrado",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            int ok = 0, errores = 0;
            var sbErr = new StringBuilder();

            // Iterar filas seleccionadas (copiamos a array para no mutar durante iteración)
            var filas = new DataGridViewRow[dgvDocumentos.SelectedRows.Count];
            dgvDocumentos.SelectedRows.CopyTo(filas, 0);

            foreach (var fila in filas)
            {
                string folio = fila.Cells["Folio"].Value?.ToString().Trim() ?? "";
                string serie = fila.Cells["Serie"].Value?.ToString().Trim() ?? "";

                // 1. Posicionar documento
                int resBuscar = SdkComercial.fBuscarDocumento(_codConceptoActual, serie, folio);
                if (resBuscar != 0)
                {
                    errores++;
                    sbErr.AppendLine($"Folio {folio}: no encontrado ({SdkComercial.DescribirError(resBuscar)})");
                    continue;
                }

                // 2. Borrar físicamente
                int resBorra = SdkComercial.fBorraDocumento();
                if (resBorra != 0)
                {
                    errores++;
                    sbErr.AppendLine($"Folio {folio}: error al borrar ({SdkComercial.DescribirError(resBorra)})");
                    continue;
                }

                ok++;
            }

            // Mostrar resultado
            string resMsg = $"Borrados: {ok}   Errores: {errores}";
            if (errores > 0)
                resMsg += "\n\nDetalle de errores:\n" + sbErr;

            if (errores == 0)
                MessageBox.Show($"✔ {ok} documento(s) borrado(s) correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(resMsg, "Resultado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            SetEstado(ok > 0
                ? $"✔ {ok} borrado(s) correctamente." + (errores > 0 ? $"  {errores} con error." : "")
                : $"✘ {errores} error(es) al borrar.", errores > 0 && ok == 0);

            // Refrescar grid automáticamente
            if (ok > 0) btnBuscar_Click(null, null);
        }

        // ─────────────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────────────
        private void dgvDocumentos_SelectionChanged(object sender, EventArgs e)
            => ActualizarBotones();

        private void ActualizarBotones()
        {
            btnBorrar.Enabled = dgvDocumentos.SelectedRows.Count > 0;
        }

        private void SetEstado(string msg, bool esError)
        {
            lblEstado.Text      = msg;
            lblEstado.ForeColor = esError ? UITheme.Danger : UITheme.Success;
        }

        private void AjustarColumnas()
        {
            // Anchos fijos razonables; Total y Cliente se expanden
            if (dgvDocumentos.Columns.Contains("Folio"))  dgvDocumentos.Columns["Folio"].Width  = 80;
            if (dgvDocumentos.Columns.Contains("Serie"))  dgvDocumentos.Columns["Serie"].Width  = 60;
            if (dgvDocumentos.Columns.Contains("Fecha"))  dgvDocumentos.Columns["Fecha"].Width  = 90;
            if (dgvDocumentos.Columns.Contains("Total"))  dgvDocumentos.Columns["Total"].Width  = 110;

            if (dgvDocumentos.Columns.Contains("Cliente"))
            {
                dgvDocumentos.Columns["Cliente"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e) => this.Close();

        // ─────────────────────────────────────────────────────────────────────
        // COMBO HELPER
        // ─────────────────────────────────────────────────────────────────────
        private class ItemCombo
        {
            public int    Id     { get; }
            public string Code   { get; }
            private readonly string _nombre;
            public ItemCombo(int id, string nombre, string code = "")
            {
                Id     = id;
                Code   = code ?? "";
                _nombre = nombre;
            }
            public override string ToString() => _nombre;
        }
    }
}
