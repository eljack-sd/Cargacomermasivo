using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;

namespace CargaComerMasivo
{
    public partial class FrmPrincipal : Form
    {
        // Columnas del Excel (base 0) — layout real confirmado por el usuario
        private const int COL_CLIENTE          = 0;   // A: Codigo Cliente/Proveedor
        private const int COL_CODIGO_PRODUCTO  = 1;   // B: Clave Producto
        private const int COL_UNIDADES         = 2;   // C: Cantidad
        private const int COL_OBS_MOVIMIENTO   = 3;   // D: Observaciones del Movimiento
        private const int COL_OBS_DOCUMENTO    = 4;   // E: Observaciones del Documento
        private const int COL_SUBTOTAL         = 5;   // F: Subtotal (precio unitario)
        private const int COL_SERIE            = 6;   // G: Serie

        private DataTable dtExcel  = new DataTable();
        private int       nIdCteProv = 0;   // Solo para validacion GUI

        public FrmPrincipal()
        {
            InitializeComponent();
            CargarCombos();
        }

        // ─────────────────────────────────────────────────────────────────────
        // CARGA DE COMBOS AL INICIAR — usa SQL Server directamente.
        // ─────────────────────────────────────────────────────────────────────
        private void CargarCombos()
        {
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa))
            {
                MessageBox.Show(
                    "No se tiene conexion SQL a la empresa.\n\n" +
                    "Selecciona la empresa desde el combo y vuelve a intentarlo.",
                    "Sin conexion SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try { CargarConceptos(); }          catch { }
            try { CargarMonedas(); }             catch { }
            try { CargarAlmacenes(); }           catch { }
            try { CargarClientesProveedores(); } catch { }
            try { CargarAgentes(); }             catch { }
        }

        private void CargarConceptos()
        {
            cbConcepto.Items.Clear();
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
                        // Code = CCODIGOCONCEPTO (el string que fAltaDocumento necesita)
                        cbConcepto.Items.Add(new ItemCombo(id, cod + " - " + nom, cod));
                    }
            }
            if (cbConcepto.Items.Count > 0) cbConcepto.SelectedIndex = 0;
        }

        private void CargarMonedas()
        {
            cbMoneda.Items.Clear();
            using (var conn = new SqlConnection(Program.ConnStrEmpresa))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT CIDMONEDA, CNOMBREMONEDA FROM admMonedas ORDER BY CIDMONEDA",
                    conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        int    id  = r.IsDBNull(0) ? 0  : Convert.ToInt32(r[0]);
                        string nom = r.IsDBNull(1) ? "" : r[1].ToString().Trim();
                        cbMoneda.Items.Add(new ItemCombo(id, nom));
                    }
            }
            if (cbMoneda.Items.Count > 0) cbMoneda.SelectedIndex = 0;
        }

        private void CargarAlmacenes()
        {
            cbAlmacen.Items.Clear();
            using (var conn = new SqlConnection(Program.ConnStrEmpresa))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT CIDALMACEN, CCODIGOALMACEN, CNOMBREALMACEN " +
                    "FROM admAlmacenes ORDER BY CCODIGOALMACEN",
                    conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        int    id  = r.IsDBNull(0) ? 0  : Convert.ToInt32(r[0]);
                        string cod = r.IsDBNull(1) ? "" : r[1].ToString().Trim();
                        string nom = r.IsDBNull(2) ? "" : r[2].ToString().Trim();
                        // Code = CCODIGOALMACEN (el string que tMovimiento necesita)
                        cbAlmacen.Items.Add(new ItemCombo(id, cod + " - " + nom, cod));
                    }
            }
            if (cbAlmacen.Items.Count > 0) cbAlmacen.SelectedIndex = 0;
        }

        private void CargarClientesProveedores()
        {
            cbCliente.Items.Clear();
            using (var conn = new SqlConnection(Program.ConnStrEmpresa))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT CIDCLIENTEPROVEEDOR, CCODIGOCLIENTE, CRAZONSOCIAL " +
                    "FROM admClientes " +
                    "WHERE CESTATUS = 0 " +
                    "ORDER BY CCODIGOCLIENTE",
                    conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        int    id  = r.IsDBNull(0) ? 0  : Convert.ToInt32(r[0]);
                        string cod = r.IsDBNull(1) ? "" : r[1].ToString().Trim();
                        string nom = r.IsDBNull(2) ? "" : r[2].ToString().Trim();
                        if (!string.IsNullOrEmpty(cod))
                            cbCliente.Items.Add(new ItemCteCombo(cod, nom, id));
                    }
            }
        }

        private void CargarAgentes()
        {
            cbAgente.Items.Clear();
            cbAgente.Items.Add(new ItemCombo(0, "(Sin agente)", ""));
            using (var conn = new SqlConnection(Program.ConnStrEmpresa))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT CIDAGENTE, CCODIGOAGENTE, CNOMBREAGENTE " +
                    "FROM admAgentes ORDER BY CCODIGOAGENTE",
                    conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                    {
                        int    id  = r.IsDBNull(0) ? 0  : Convert.ToInt32(r[0]);
                        string cod = r.IsDBNull(1) ? "" : r[1].ToString().Trim();
                        string nom = r.IsDBNull(2) ? "" : r[2].ToString().Trim();
                        // Code = CCODIGOAGENTE (el string que tDocumento necesita)
                        cbAgente.Items.Add(new ItemCombo(id, cod + " - " + nom, cod));
                    }
            }
            cbAgente.SelectedIndex = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        // EXCEL
        // ─────────────────────────────────────────────────────────────────────
        private void btnExaminar_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title  = "Selecciona el archivo Excel de carga";
                dlg.Filter = "Excel (*.xlsx;*.xls)|*.xlsx;*.xls";
                string initDir = @"C:\Vb6\CargaComercialMasivo\";
                dlg.InitialDirectory = Directory.Exists(initDir)
                    ? initDir
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtRutaExcel.Text = dlg.FileName;
            }
        }

        private void btnCargarExcel_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtRutaExcel.Text))
            {
                MessageBox.Show("El archivo no existe:\n" + txtRutaExcel.Text, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var pkg = new ExcelPackage(new FileInfo(txtRutaExcel.Text)))
                {
                    var ws = pkg.Workbook.Worksheets[1]; // EPPlus 4 es 1-based
                    dtExcel = new DataTable();

                    int cols = ws.Dimension.Columns;
                    int rows = ws.Dimension.Rows;

                    for (int c = 1; c <= cols; c++)
                        dtExcel.Columns.Add(ws.Cells[1, c].Text ?? ("Col" + c));

                    // Buscar la ultima fila con datos para no incluir filas vacias del final
                    int ultimaFilaDatos = 1;
                    for (int r = 2; r <= rows; r++)
                    {
                        for (int c = 1; c <= cols; c++)
                        {
                            if (!string.IsNullOrEmpty(ws.Cells[r, c].Text))
                            { ultimaFilaDatos = r; break; }
                        }
                    }

                    // Cargar hasta la ultima fila con datos, CONSERVANDO filas vacias
                    // intermedias (son los separadores de documentos)
                    for (int r = 2; r <= ultimaFilaDatos; r++)
                    {
                        var row = dtExcel.NewRow();
                        for (int c = 1; c <= cols; c++)
                            row[c - 1] = ws.Cells[r, c].Text ?? "";
                        dtExcel.Rows.Add(row);
                    }
                }

                dgvMovimientos.DataSource = dtExcel;
                int lineasConDatos = 0;
                foreach (System.Data.DataRow dr in dtExcel.Rows)
                    if (!string.IsNullOrEmpty(dr[0].ToString().Trim())) lineasConDatos++;
                lblTotalLineas.Text = "Total lineas: " + lineasConDatos;
                lblEstado.Text              = "Excel cargado correctamente (" + dtExcel.Rows.Count + " lineas)";
                lblEstado.ForeColor         = UITheme.Success;
                pbProgreso.Value            = 0;
                pbProgreso.Maximum          = dtExcel.Rows.Count > 0 ? dtExcel.Rows.Count : 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer el Excel:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblEstado.Text      = "Error al cargar Excel";
                lblEstado.ForeColor = UITheme.Danger;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // SIGUIENTE FOLIO
        // ─────────────────────────────────────────────────────────────────────
        private void btnSiguienteFolio_Click(object sender, EventArgs e)
        {
            if (cbConcepto.SelectedItem == null) return;

            // fSiguienteFolio(aCodigoConcepto:CADENA, aSerie:CADENA ref, aFolio:DOUBLE ref)
            // El SDK rellena aSerie y aFolio con los valores del siguiente folio disponible.
            string codConcepto = ((ItemCombo)cbConcepto.SelectedItem).Code;
            if (string.IsNullOrEmpty(codConcepto))
            {
                MessageBox.Show("El concepto seleccionado no tiene codigo valido.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var    sbSerie = new StringBuilder(64);
            sbSerie.Append(txtSerie.Text.Trim());   // pasar la serie actual como entrada
            double folio   = 0;

            int res = SdkComercial.fSiguienteFolio(codConcepto, sbSerie, ref folio);
            if (res == 0 && folio > 0)
            {
                txtFolio.Text = ((long)folio).ToString();
                string serieDevuelta = sbSerie.ToString().Trim();
                if (!string.IsNullOrEmpty(serieDevuelta))
                    txtSerie.Text = serieDevuelta;
            }
            else
            {
                MessageBox.Show(
                    "No se pudo obtener el siguiente folio.\n" +
                    "Codigo: " + (res != 0 ? SdkComercial.DescribirError(res) : "Folio=0"),
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // BUSCAR CLIENTE AL SELECCIONAR EN EL COMBO
        // ─────────────────────────────────────────────────────────────────────
        private void cbCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cbCliente.SelectedItem as ItemCteCombo;
            if (item == null) return;

            nIdCteProv = item.Id;

            if (nIdCteProv <= 0)
            {
                MessageBox.Show("No se encontro el cliente con clave: " + item.Codigo, "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nIdCteProv = 0;
                return;
            }

            // Leer moneda y agente del cliente desde admClientes
            if (!string.IsNullOrEmpty(Program.ConnStrEmpresa))
            {
                try
                {
                    using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand(
                            "SELECT CIDMONEDA, CIDAGENTEVENTA FROM admClientes " +
                            "WHERE CCODIGOCLIENTE = @cod",
                            conn))
                        {
                            cmd.Parameters.AddWithValue("@cod", item.Codigo);
                            using (var r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    int idMon = r.IsDBNull(0) ? 0 : Convert.ToInt32(r[0]);
                                    int idAgt = r.IsDBNull(1) ? 0 : Convert.ToInt32(r[1]);
                                    if (idMon > 0)
                                        for (int i = 0; i < cbMoneda.Items.Count; i++)
                                            if (((ItemCombo)cbMoneda.Items[i]).Id == idMon)
                                            { cbMoneda.SelectedIndex = i; break; }
                                    if (idAgt > 0)
                                        for (int i = 0; i < cbAgente.Items.Count; i++)
                                            if (((ItemCombo)cbAgente.Items[i]).Id == idAgt)
                                            { cbAgente.SelectedIndex = i; break; }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // GUARDAR UN SOLO DOCUMENTO
        // ─────────────────────────────────────────────────────────────────────
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarEncabezado()) return;
            if (dtExcel.Rows.Count == 0)
            {
                MessageBox.Show("Carga primero el archivo Excel.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int resultado = GuardarDocumento(dtExcel.Rows, 0, dtExcel.Rows.Count - 1);
                if (resultado > 0)
                {
                    lblEstado.Text      = "Documento guardado correctamente (ID: " + resultado + ")";
                    lblEstado.ForeColor = UITheme.Success;
                    MessageBox.Show("Documento guardado exitosamente.\nID: " + resultado, "Exito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text      = "Error al guardar";
                lblEstado.ForeColor = UITheme.Danger;
                MessageBox.Show("Error al guardar documento:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CARGA MASIVA
        // ─────────────────────────────────────────────────────────────────────
        private void btnCargaMasiva_Click(object sender, EventArgs e)
        {
            if (chkUnaSolaFactura.Checked)
            {
                if (!ValidarEncabezado()) return;
            }
            else
            {
                if (cbConcepto.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un concepto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!DateTime.TryParse(txtFecha.Text, out _))
                {
                    MessageBox.Show("Ingresa la fecha en formato AAAA/MM/DD.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cbAlmacen.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un almacen.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (dtExcel.Rows.Count == 0)
            {
                MessageBox.Show("Carga primero el archivo Excel.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string modoTexto = chkUnaSolaFactura.Checked
                ? "UN documento con todas las lineas. Cliente: " + cbCliente.Text
                : "Un documento por BLOQUE de filas consecutivas (fila vacia = nuevo documento). Cliente leido de columna A.";

            var confirm = MessageBox.Show(
                "Deseas impactar " + dtExcel.Rows.Count + " lineas a CONTPAQi Comercial?\n" +
                "Concepto: " + cbConcepto.Text + "\n" +
                "Fecha: " + txtFecha.Text + "\n" +
                "Modo: " + modoTexto,
                "Confirmar Carga Masiva",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            btnCargaMasiva.Enabled  = false;
            btnGuardar.Enabled      = false;
            pbProgreso.Value        = 0;
            pbProgreso.Maximum      = dtExcel.Rows.Count;
            lblEstado.Text          = "Procesando...";
            lblEstado.ForeColor     = UITheme.Warning;
            Application.DoEvents();

            try
            {
                int exitosos = 0, errores = 0;
                var log = new List<string>();

                if (chkUnaSolaFactura.Checked)
                {
                    int idDoc = GuardarDocumento(dtExcel.Rows, 0, dtExcel.Rows.Count - 1);
                    if (idDoc > 0)
                    {
                        exitosos         = dtExcel.Rows.Count;
                        pbProgreso.Value = dtExcel.Rows.Count;
                    }
                    else
                    {
                        errores = dtExcel.Rows.Count;
                        log.Add("Error al crear documento unico: codigo " + idDoc);
                    }
                }
                else
                {
                    // Filas consecutivas sin fila vacia entre ellas = UN documento
                    // Fila vacia (col A en blanco) = separador entre documentos
                    int i = 0;
                    while (i < dtExcel.Rows.Count)
                    {
                        // Saltar filas vacias
                        string codCte = dtExcel.Rows[i][COL_CLIENTE].ToString().Trim();
                        if (string.IsNullOrEmpty(codCte))
                        {
                            pbProgreso.Value = i + 1;
                            i++;
                            Application.DoEvents();
                            continue;
                        }

                        // Detectar fin del bloque (hasta la proxima fila vacia o fin del excel)
                        int desde = i;
                        int hasta = i;
                        while (hasta + 1 < dtExcel.Rows.Count &&
                               !string.IsNullOrEmpty(dtExcel.Rows[hasta + 1][COL_CLIENTE].ToString().Trim()))
                        {
                            hasta++;
                        }

                        int numLineas = hasta - desde + 1;
                        string rangoTxt = desde == hasta
                            ? "Fila " + (desde + 2)
                            : "Filas " + (desde + 2) + "-" + (hasta + 2);

                        lblEstado.Text = "Procesando " + rangoTxt + " (" + numLineas + " movimientos)...";
                        Application.DoEvents();

                        // Validar que el cliente del bloque existe
                        int idCteProvFila = BuscarIdClienteSQL(codCte);
                        if (idCteProvFila <= 0)
                        {
                            errores++;
                            log.Add(rangoTxt + ": Cliente '" + codCte + "' no encontrado - bloque omitido.");
                            for (int k = desde; k <= hasta; k++) pbProgreso.Value = k + 1;
                            i = hasta + 1;
                            Application.DoEvents();
                            continue;
                        }

                        try
                        {
                            // Guardar el bloque completo como UN documento
                            int idDoc = GuardarDocumento(dtExcel.Rows, desde, hasta, codCte);
                            if (idDoc > 0)
                            {
                                exitosos++;
                                log.Add(rangoTxt + ": OK — Documento ID " + idDoc +
                                        " (" + numLineas + " mov.) Cliente: " + codCte);
                            }
                            else
                            {
                                errores++;
                                log.Add(rangoTxt + ": Error al guardar (codigo SDK: " + idDoc + ")");
                            }
                        }
                        catch (Exception ex)
                        {
                            errores++;
                            log.Add(rangoTxt + ": " + ex.Message);
                        }

                        for (int k = desde; k <= hasta; k++) pbProgreso.Value = k + 1;
                        Application.DoEvents();
                        i = hasta + 1;
                    }
                }

                string resumen = "Carga masiva completada:\nExitosos: " + exitosos + "\nErrores: " + errores;
                if (log.Count > 0)
                    resumen += "\n\nDetalle de errores:\n" + string.Join("\n", log.GetRange(0, Math.Min(log.Count, 10)));

                lblEstado.Text      = "Carga masiva: " + exitosos + " ok, " + errores + " errores";
                lblEstado.ForeColor = errores == 0 ? UITheme.Success : UITheme.Warning;
                MessageBox.Show(resumen, "Resultado", MessageBoxButtons.OK,
                    errores == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblEstado.Text      = "Error en carga masiva";
                lblEstado.ForeColor = UITheme.Danger;
                MessageBox.Show("Error en carga masiva:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCargaMasiva.Enabled = true;
                btnGuardar.Enabled     = true;
            }
        }

        // Devuelve CIDCLIENTEPROVEEDOR de admClientes dado el codigo de cliente
        private int BuscarIdClienteSQL(string codigoCliente)
        {
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa)) return 0;
            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT CIDCLIENTEPROVEEDOR FROM admClientes WHERE CCODIGOCLIENTE = @cod",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", codigoCliente);
                        object val = cmd.ExecuteScalar();
                        return val == null || val == DBNull.Value ? 0 : Convert.ToInt32(val);
                    }
                }
            }
            catch { return 0; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // LOGICA CENTRAL: CREAR Y GUARDAR DOCUMENTO
        //
        // Usa la API "Alto nivel" del SDK:
        //   fAltaDocumento(ref idDoc, ref tDocumento)        → crea encabezado
        //   fAltaMovimiento(idDoc, ref idMov, ref tMovimiento) → agrega renglon
        //   fGuardaMovimiento()                              → guarda renglon
        //   fGuardaDocumento()                               → guarda todo
        //
        // codigoCteProvOverride = codigo STRING del cliente (columna A del Excel
        // en modo por fila). Null/vacio = usa el cliente seleccionado en el combo.
        // ─────────────────────────────────────────────────────────────────────
        private int GuardarDocumento(DataRowCollection rows, int desde, int hasta,
                                     string codigoCteProvOverride = null)
        {
            // ── Concepto ─────────────────────────────────────────────────────
            var itemConcepto = (ItemCombo)cbConcepto.SelectedItem;
            string codConceptoStr = itemConcepto.Code;   // e.g. "OCC", "FAC", "COC"
            if (string.IsNullOrEmpty(codConceptoStr))
                throw new Exception("El concepto seleccionado no tiene codigo valido.");

            // ── Fecha ─────────────────────────────────────────────────────────
            if (!DateTime.TryParse(txtFecha.Text, out DateTime fechaDt))
                throw new Exception("Fecha invalida. Usa el formato AAAA/MM/DD.");
            string fecha = fechaDt.ToString("MM/dd/yyyy");   // SDK espera MM/dd/yyyy

            // ── Serie ────────────────────────────────────────────────────────
            string serie = "";
            if (desde < rows.Count && dtExcel.Columns.Count > COL_SERIE)
                serie = rows[desde][COL_SERIE].ToString().Trim();
            if (string.IsNullOrEmpty(serie))
                serie = txtSerie.Text.Trim();

            // ── Folio ────────────────────────────────────────────────────────
            int.TryParse(txtFolio.Text, out int folioInt);
            double folio = folioInt;

            // ── Tipo de cambio ────────────────────────────────────────────────
            double.TryParse(
                txtTipoCambio.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double tipoCambio);
            if (tipoCambio <= 0) tipoCambio = 1.0;

            // ── Moneda (ID numerico para el struct) ───────────────────────────
            int idMoneda = cbMoneda.SelectedItem != null ? ((ItemCombo)cbMoneda.SelectedItem).Id : 1;

            // ── Almacen (codigo STRING para tMovimiento) ──────────────────────
            string codAlmacen = cbAlmacen.SelectedItem != null
                ? ((ItemCombo)cbAlmacen.SelectedItem).Code : "";
            if (string.IsNullOrEmpty(codAlmacen))
                throw new Exception("El almacen seleccionado no tiene codigo valido.");

            // ── Agente (codigo STRING para tDocumento) ────────────────────────
            string codAgente = cbAgente.SelectedItem != null
                ? ((ItemCombo)cbAgente.SelectedItem).Code : "";

            // ── Cliente/Proveedor (codigo STRING para tDocumento) ─────────────
            string codCteProv;
            if (!string.IsNullOrEmpty(codigoCteProvOverride))
                codCteProv = codigoCteProvOverride;
            else
            {
                var itemCte = cbCliente.SelectedItem as ItemCteCombo;
                codCteProv = itemCte != null ? itemCte.Codigo : "";
            }
            if (string.IsNullOrEmpty(codCteProv))
                throw new Exception("No se especifico codigo de cliente/proveedor.");

            // ── Observaciones y referencia ────────────────────────────────────
            string obsDocumento = "";
            if (desde < rows.Count && dtExcel.Columns.Count > COL_OBS_DOCUMENTO)
                obsDocumento = rows[desde][COL_OBS_DOCUMENTO].ToString().Trim();
            if (string.IsNullOrEmpty(obsDocumento))
                obsDocumento = txtObservaciones.Text.Trim();

            string referencia = txtReferencia.Text.Trim();
            if (referencia.Length > 20) referencia = referencia.Substring(0, 20); // max 21 chars

            // ─────────────────────────────────────────────────────────────────
            // PASO 1 — Alta del documento (API Alto nivel)
            // ─────────────────────────────────────────────────────────────────
            var doc = new tDocumento
            {
                aFolio         = folio,
                aNumMoneda     = idMoneda,
                aTipoCambio    = tipoCambio,
                aImporte       = 0,      // el SDK lo calcula de los movimientos
                aDescuentoDoc1 = 0,
                aDescuentoDoc2 = 0,
                aSistemaOrigen = 205,    // 205 = CONTPAQi Comercial Premium
                aCodConcepto   = codConceptoStr,
                aSerie         = serie,
                aFecha         = fecha,
                aCodigoCteProv = codCteProv,
                aCodigoAgente  = codAgente,
                aReferencia    = referencia,
                aAfecta        = 1,      // 1=entradas (compras)
                aGasto1        = 0,
                aGasto2        = 0,
                aGasto3        = 0
            };

            int idDocumento = 0;
            string paso = "fAltaDocumento";
            try
            {
                int resAlta = SdkComercial.fAltaDocumento(ref idDocumento, ref doc);
                if (resAlta != 0)
                    throw new Exception("fAltaDocumento error: " + SdkComercial.DescribirError(resAlta));

                // ── PASO 2 — Observaciones del encabezado (bajo nivel) ────────
                if (!string.IsNullOrEmpty(obsDocumento))
                {
                    paso = "fSetDatoDocumento:COBSERVACIONES";
                    SdkComercial.fSetDatoDocumento("COBSERVACIONES", obsDocumento);
                }

                // ── PASO 3 — Movimientos ──────────────────────────────────────
                int consecutivo = 1;
                for (int i = desde; i <= hasta; i++)
                {
                    var row = rows[i];
                    string codProducto = row[COL_CODIGO_PRODUCTO].ToString().Trim();
                    if (string.IsNullOrEmpty(codProducto)) continue;

                    double.TryParse(
                        row[COL_UNIDADES].ToString().Replace(",", "."),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double unidades);
                    if (unidades <= 0) unidades = 1;

                    double.TryParse(
                        row[COL_SUBTOTAL].ToString().Replace(",", "."),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double precio);

                    string obsMov = dtExcel.Columns.Count > COL_OBS_MOVIMIENTO
                        ? row[COL_OBS_MOVIMIENTO].ToString().Trim()
                        : "";

                    paso = "fAltaMovimiento (prod=" + codProducto + ")";
                    var mov = new tMovimiento
                    {
                        aConsecutivo      = consecutivo++,
                        aUnidades         = unidades,
                        aPrecio           = precio,
                        aCosto            = precio,  // en compras, costo = precio
                        aCodProdSer       = codProducto,
                        aCodAlmacen       = codAlmacen,
                        aReferencia       = "",
                        aCodClasificacion = ""
                    };

                    int idMovimiento = 0;
                    int resMov = SdkComercial.fAltaMovimiento(idDocumento, ref idMovimiento, ref mov);
                    if (resMov != 0)
                    {
                        try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                        throw new Exception("fAltaMovimiento error en '" + codProducto + "': " +
                            SdkComercial.DescribirError(resMov));
                    }

                    // Observaciones del movimiento (campo bajo nivel)
                    if (!string.IsNullOrEmpty(obsMov))
                    {
                        paso = "fSetDatoMovimiento:COBSERVAMOV (prod=" + codProducto + ")";
                        SdkComercial.fSetDatoMovimiento("COBSERVAMOV", obsMov);
                    }

                    paso = "fGuardaMovimiento (prod=" + codProducto + ")";
                    int resGM = SdkComercial.fGuardaMovimiento();
                    if (resGM < 0)
                    {
                        try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                        throw new Exception("fGuardaMovimiento error en '" + codProducto + "': " +
                            SdkComercial.DescribirError(resGM));
                    }
                }

                // ── PASO 4 — Guardar documento ────────────────────────────────
                paso = "fGuardaDocumento";
                int resFinal = SdkComercial.fGuardaDocumento();
                if (resFinal < 0)
                {
                    try { SdkComercial.fCancelarModificacionDocumento(); } catch { }
                    throw new Exception("fGuardaDocumento error: " +
                        SdkComercial.DescribirError(resFinal));
                }

                // idDocumento fue asignado por fAltaDocumento; si es 0 intentar leerlo
                if (idDocumento <= 0)
                {
                    string sId = SdkComercial.LeerCampoDocumento("CIDDOCUMENTO");
                    int.TryParse(sId, out idDocumento);
                    if (idDocumento <= 0) idDocumento = 1; // al menos indico exito
                }

                return idDocumento;
            }
            catch (Exception ex) when (!ex.Message.StartsWith("CRASH@"))
            {
                throw new Exception("CRASH@[" + paso + "]: " + ex.Message, ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // VALIDACIONES
        // ─────────────────────────────────────────────────────────────────────
        private bool ValidarEncabezado()
        {
            if (cbConcepto.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un concepto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(((ItemCombo)cbConcepto.SelectedItem).Code))
            {
                MessageBox.Show("El concepto seleccionado no tiene codigo valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!DateTime.TryParse(txtFecha.Text, out _))
            {
                MessageBox.Show("Ingresa la fecha en formato AAAA/MM/DD.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbCliente.SelectedItem == null || nIdCteProv <= 0)
            {
                MessageBox.Show("Selecciona un cliente/proveedor valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbAlmacen.SelectedItem == null || string.IsNullOrEmpty(((ItemCombo)cbAlmacen.SelectedItem).Code))
            {
                MessageBox.Show("Selecciona un almacen valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseas cerrar la empresa y salir?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SdkComercial.fCierraEmpresa();
            base.OnFormClosed(e);
            Application.Exit();
        }

        // ─────────────────────────────────────────────────────────────────────
        // HELPER CLASSES
        // ─────────────────────────────────────────────────────────────────────
        private class ItemCombo
        {
            public int    Id   { get; }
            public string Code { get; }       // Codigo string (para SDK fAlta*)
            private string Nombre { get; }
            public ItemCombo(int id, string nombre, string code = "")
            {
                Id     = id;
                Code   = code ?? "";
                Nombre = nombre;
            }
            public override string ToString() => Nombre;
        }

        private class ItemCteCombo
        {
            public string Codigo { get; }
            public int    Id     { get; }
            private string Nombre { get; }
            public ItemCteCombo(string codigo, string nombre, int id = 0)
            {
                Codigo = codigo;
                Nombre = nombre;
                Id     = id;
            }
            public override string ToString() => Codigo + " - " + Nombre;
        }
    }
}
