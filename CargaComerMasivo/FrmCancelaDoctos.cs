using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    public partial class FrmCancelaDoctos : Form
    {
        private string _codConceptoActual = "";

        // Cache de nombres de columna detectados al primer Buscar
        private string _colSerie = null;   // CSERIE o CSERIEDOCUMENTO
        private string _joinCliente = null;  // LEFT JOIN ... detectado

        public FrmCancelaDoctos()
        {
            InitializeComponent();
            UITheme.AplicarIconoForm(this);
            CargarConceptos();
            ActualizarBotones();
        }

        // ─────────────────────────────────────────────────────────────────────
        // CARGAR CONCEPTOS
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
                        "FROM admConceptos ORDER BY CCODIGOCONCEPTO", conn))
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            int id = r.IsDBNull(0) ? 0 : Convert.ToInt32(r[0]);
                            string cod = r.IsDBNull(1) ? "" : r[1].ToString().Trim();
                            string nom = r.IsDBNull(2) ? "" : r[2].ToString().Trim();
                            cbConcepto.Items.Add(new ItemCombo(id, cod + " – " + nom, cod));
                        }
                }
            }
            catch { }
            if (cbConcepto.Items.Count > 0) cbConcepto.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        // DETECTAR COLUMNAS DINÁMICAMENTE (se corre una sola vez, se cachea)
        // ─────────────────────────────────────────────────────────────────────
        private void DetectarColumnas()
        {
            if (_colSerie != null && _joinCliente != null) return; // ya detectadas

            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();

                    // ── Detectar columna de Serie ────────────────────────────
                    string[] candidatosSerie = { "CSERIE", "CSERIEDOCUMENTO", "CSERIEORIGINAL" };
                    _colSerie = "";
                    foreach (string col in candidatosSerie)
                    {
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                            "WHERE TABLE_NAME='admDocumentos' AND COLUMN_NAME=@c", conn))
                        {
                            cmd.Parameters.AddWithValue("@c", col);
                            if ((int)cmd.ExecuteScalar() > 0) { _colSerie = col; break; }
                        }
                    }

                    // ── Detectar columna FK del Cliente ──────────────────────
                    string[] candidatosFk = { "CIDCTEPROVVENTA", "CIDCLIENTEPROVEEDOR", "CIDCLIENTEPROVVENTA" };
                    _joinCliente = "LEFT JOIN admClientes cl ON 1=0"; // fallback
                    foreach (string col in candidatosFk)
                    {
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                            "WHERE TABLE_NAME='admDocumentos' AND COLUMN_NAME=@c", conn))
                        {
                            cmd.Parameters.AddWithValue("@c", col);
                            if ((int)cmd.ExecuteScalar() > 0)
                            {
                                _joinCliente = $"LEFT JOIN admClientes cl ON d.{col} = cl.CIDCLIENTEPROVEEDOR";
                                break;
                            }
                        }
                    }

                    // Si no encontró FK directo, intentar por CCODIGOCLIENTE
                    if (_joinCliente.Contains("1=0"))
                    {
                        using (var cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                            "WHERE TABLE_NAME='admDocumentos' AND COLUMN_NAME='CCODIGOCLIENTE'", conn))
                        {
                            if ((int)cmd.ExecuteScalar() > 0)
                                _joinCliente =
                                    "LEFT JOIN admClientes cl ON cl.CIDCLIENTEPROVEEDOR = " +
                                    "(SELECT TOP 1 CIDCLIENTEPROVEEDOR FROM admClientes " +
                                    " WHERE CCODIGOCLIENTE = d.CCODIGOCLIENTE)";
                        }
                    }
                }
            }
            catch
            {
                // Si falla la detección, usar valores seguros vacíos
                if (_colSerie == null) _colSerie = "";
                if (_joinCliente == null) _joinCliente = "LEFT JOIN admClientes cl ON 1=0";
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // BUSCAR
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

            // Detectar columnas la primera vez
            DetectarColumnas();

            // Construir SELECT de Serie de forma segura
            string selectSerie = string.IsNullOrEmpty(_colSerie)
                ? "'' AS Serie"
                : $"ISNULL(d.{_colSerie}, '') AS Serie";

            var dt = new DataTable();
            try
            {
                var sql =
                    "SELECT d.CIDDOCUMENTO AS ID, " +
                    "       d.CFOLIO       AS Folio, " +
                    "       " + selectSerie + ", " +
                    "       CONVERT(varchar(10), d.CFECHA, 103) AS Fecha, " +
                    "       ISNULL(cl.CRAZONSOCIAL, '') AS Cliente, " +
                    "       d.CTOTAL       AS Total " +
                    "FROM   admDocumentos d " +
                    "INNER JOIN admConceptos co " +
                    "       ON d.CIDCONCEPTODOCUMENTO = co.CIDCONCEPTODOCUMENTO " +
                    _joinCliente + " " +
                    "WHERE  co.CCODIGOCONCEPTO = @cod " +
                    "ORDER  BY d.CFOLIO DESC";

                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
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
                // Si aún falla, resetear cache para forzar re-detección en el próximo intento
                _colSerie = null;
                _joinCliente = null;
                SetEstado("Error SQL: " + ex.Message, true);
                return;
            }

            // Formatear Total
            foreach (DataRow row in dt.Rows)
            {
                if (row["Total"] != DBNull.Value)
                {
                    try { row["Total"] = Convert.ToDecimal(row["Total"]).ToString("N2"); }
                    catch { }
                }
            }

            dgvDocumentos.DataSource = dt;
            if (dgvDocumentos.Columns.Contains("ID"))
                dgvDocumentos.Columns["ID"].Visible = false;
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
        // ─────────────────────────────────────────────────────────────────────
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (dgvDocumentos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona al menos un documento.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int total = dgvDocumentos.SelectedRows.Count;
            string msg = total == 1
                ? "¿Confirmas borrar físicamente 1 documento?\n\nEsta acción NO se puede deshacer."
                : $"¿Confirmas borrar físicamente {total} documentos?\n\nEsta acción NO se puede deshacer.";

            if (MessageBox.Show(msg, "Confirmar borrado",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            int ok = 0, errores = 0;
            var sbErr = new StringBuilder();

            var filas = new DataGridViewRow[dgvDocumentos.SelectedRows.Count];
            dgvDocumentos.SelectedRows.CopyTo(filas, 0);

            foreach (var fila in filas)
            {
                string folio = fila.Cells["Folio"].Value?.ToString().Trim() ?? "";
                string serie = dgvDocumentos.Columns.Contains("Serie")
                    ? fila.Cells["Serie"].Value?.ToString().Trim() ?? ""
                    : "";

                // Obtener el ID interno del documento (columna oculta "ID")
                int idDoc = 0;
                if (dgvDocumentos.Columns.Contains("ID") && fila.Cells["ID"].Value != null)
                    int.TryParse(fila.Cells["ID"].Value.ToString(), out idDoc);

                int resBuscar = SdkComercial.fBuscarDocumento(_codConceptoActual, serie, folio);
                if (resBuscar != 0)
                {
                    errores++;
                    sbErr.AppendLine($"Folio {folio}: no encontrado ({SdkComercial.DescribirError(resBuscar)})");
                    continue;
                }

                int resBorra = SdkComercial.fBorraDocumento();
                if (resBorra != 0)
                {
                    errores++;
                    sbErr.AppendLine($"Folio {folio}: error al borrar ({SdkComercial.DescribirError(resBorra)})");
                    continue;
                }

                // Borrado físico de movimientos y documento en la BD de CONTPAQi
                if (idDoc > 0)
                {
                    try
                    {
                        using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                        {
                            conn.Open();
                            using (var cmd = new SqlCommand(
                                "DELETE FROM admMovimientos WHERE CIDDOCUMENTO = @id; " +
                                "DELETE FROM admDocumentos  WHERE CIDDOCUMENTO = @id", conn))
                            {
                                cmd.Parameters.AddWithValue("@id", idDoc);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch { /* No interrumpir el flujo si falla el DELETE físico */ }

                    // Eliminar de nuestra base de control para que pueda volver a procesarse
                    RegistroProcesados.EliminarPorIdDoc(idDoc);
                }

                ok++;
            }

            string resMsg = $"Borrados: {ok}   Errores: {errores}";
            if (errores > 0) resMsg += "\n\nDetalle:\n" + sbErr;

            if (errores == 0)
                MessageBox.Show($"✔ {ok} documento(s) borrado(s) correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(resMsg, "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            SetEstado(ok > 0
                ? $"✔ {ok} borrado(s)." + (errores > 0 ? $"  {errores} con error." : "")
                : $"✘ {errores} error(es) al borrar.", errores > 0 && ok == 0);

            if (ok > 0) btnBuscar_Click(null, null);
        }

        // ─────────────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────────────
        private void dgvDocumentos_SelectionChanged(object sender, EventArgs e)
            => ActualizarBotones();

        private void ActualizarBotones()
            => btnBorrar.Enabled = dgvDocumentos.SelectedRows.Count > 0;

        private void SetEstado(string msg, bool esError)
        {
            lblEstado.Text = msg;
            lblEstado.ForeColor = esError ? UITheme.Danger : UITheme.Success;
        }

        private void AjustarColumnas()
        {
            if (dgvDocumentos.Columns.Contains("Folio")) dgvDocumentos.Columns["Folio"].Width = 80;
            if (dgvDocumentos.Columns.Contains("Serie")) dgvDocumentos.Columns["Serie"].Width = 60;
            if (dgvDocumentos.Columns.Contains("Fecha")) dgvDocumentos.Columns["Fecha"].Width = 90;
            if (dgvDocumentos.Columns.Contains("Total")) dgvDocumentos.Columns["Total"].Width = 110;
            if (dgvDocumentos.Columns.Contains("Cliente"))
                dgvDocumentos.Columns["Cliente"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnSalir_Click(object sender, EventArgs e) => this.Close();

        private class ItemCombo
        {
            public int Id { get; }
            public string Code { get; }
            private readonly string _nombre;
            public ItemCombo(int id, string nombre, string code = "")
            { Id = id; Code = code ?? ""; _nombre = nombre; }
            public override string ToString() => _nombre;
        }
    }
}