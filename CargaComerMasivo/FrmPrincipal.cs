using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;

namespace CargaComerMasivo
{
    public partial class FrmPrincipal : Form
    {
        // ─────────────────────────────────────────────────────────────────────
        // COLUMNAS DEL EXCEL (base 0)
        // A: Código cliente/proveedor
        // B: Clave producto
        // C: Cantidad
        // D: Observaciones del movimiento
        // E: Observaciones del documento
        // F: Subtotal (precio unitario)
        // G: Serie          (opcional — si vacía, se sube sin serie)
        // H: Referencia     (opcional — si vacía, se sube sin referencia)
        //
        // Regla de documentos:
        //   Filas contiguas sin espacio = un solo documento (varios movimientos)
        //   Fila vacía = separador → el siguiente bloque es un documento nuevo
        // ─────────────────────────────────────────────────────────────────────
        private const int COL_CLIENTE         = 0;
        private const int COL_CODIGO_PRODUCTO = 1;
        private const int COL_UNIDADES        = 2;
        private const int COL_OBS_MOVIMIENTO  = 3;
        private const int COL_OBS_DOCUMENTO   = 4;
        private const int COL_SUBTOTAL        = 5;
        private const int COL_SERIE           = 6;
        private const int COL_REFERENCIA      = 7;   // H: Referencia del documento

        private DataTable dtExcel = new DataTable();

        public FrmPrincipal()
        {
            InitializeComponent();
            CargarCombos();
            // Posicionar botones del header al cargar (DockStyle.Top no tiene ancho aún en InitializeComponent)
            this.Load += (s, e) => AjustarBotonesHeader();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Reposicionar botones del header pegados a la derecha.
        // Se llama en Load y en cada Resize del pnlHeader.
        // ─────────────────────────────────────────────────────────────────────
        private void AjustarBotonesHeader()
        {
            if (pnlHeader == null || pnlHeader.ClientSize.Width <= 0) return;
            int right = pnlHeader.ClientSize.Width - 14;
            // Solo btnHeaderCancelar visible; Almacenes y Agentes están ocultos por ahora
            btnHeaderCancelar.Left = right - btnHeaderCancelar.Width;
        }

        // ─────────────────────────────────────────────────────────────────────
        // CARGA DE COMBOS — solo Conceptos y Almacenes.
        // Moneda y Agente se resuelven por cliente vía SQL al crear cada documento.
        // ─────────────────────────────────────────────────────────────────────
        private void CargarCombos()
        {
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa))
            {
                MessageBox.Show(
                    "No se tiene conexión SQL a la empresa.\n\n" +
                    "Selecciona la empresa desde FrmConexion y vuelve a intentarlo.",
                    "Sin conexión SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try { CargarConceptos(); } catch { }
            try { CargarAlmacenes(); } catch { }
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
                        cbConcepto.Items.Add(new ItemCombo(id, cod + " - " + nom, cod));
                    }
            }
            if (cbConcepto.Items.Count > 0) cbConcepto.SelectedIndex = 0;
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
                        cbAlmacen.Items.Add(new ItemCombo(id, cod + " - " + nom, cod));
                    }
            }
            if (cbAlmacen.Items.Count > 0) cbAlmacen.SelectedIndex = 0;
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
                    var ws   = pkg.Workbook.Worksheets[1]; // EPPlus 4 — base 1
                    dtExcel  = new DataTable();
                    int cols = ws.Dimension.Columns;
                    int rows = ws.Dimension.Rows;

                    for (int c = 1; c <= cols; c++)
                        dtExcel.Columns.Add(ws.Cells[1, c].Text ?? ("Col" + c));

                    // Primero localizamos la última fila que tiene algún dato.
                    // Las filas en blanco INTERMEDIAS se conservan porque son
                    // separadores de documento (fila vacía = nuevo documento).
                    int ultimaFila = 1;
                    for (int r = 2; r <= rows; r++)
                        for (int c = 1; c <= cols; c++)
                            if (!string.IsNullOrEmpty(ws.Cells[r, c].Text))
                            { ultimaFila = r; break; }

                    // Cargar todas las filas hasta la última con dato,
                    // incluyendo las filas en blanco intermedias (separadores).
                    for (int r = 2; r <= ultimaFila; r++)
                    {
                        var row = dtExcel.NewRow();
                        for (int c = 1; c <= cols; c++)
                            row[c - 1] = ws.Cells[r, c].Text ?? "";
                        dtExcel.Rows.Add(row);
                    }
                }

                dgvMovimientos.DataSource = dtExcel;
                lblTotalLineas.Text        = "Total líneas: " + dtExcel.Rows.Count;
                lblEstado.Text             = "Excel cargado (" + dtExcel.Rows.Count + " líneas)";
                lblEstado.ForeColor        = UITheme.Success;
                pbProgreso.Value           = 0;
                pbProgreso.Maximum         = dtExcel.Rows.Count > 0 ? dtExcel.Rows.Count : 1;
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
        // CARGA MASIVA
        //
        // Modo "Una sola factura":
        //   Todos los movimientos del Excel → un único documento.
        //   Cliente: primera fila con dato en columna A.
        //
        // Modo normal (por bloque):
        //   Filas contiguas con cliente en col A = un bloque = un documento.
        //   Fila vacía en col A = separador entre bloques.
        // ─────────────────────────────────────────────────────────────────────
        private void btnCargaMasiva_Click(object sender, EventArgs e)
        {
            if (!ValidarEncabezado()) return;

            if (dtExcel.Rows.Count == 0)
            {
                MessageBox.Show("Carga primero el archivo Excel.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string modoTexto = chkUnaSolaFactura.Checked
                ? "UN documento con todos los movimientos. Cliente: primera fila del Excel."
                : "Un documento por bloque (filas contiguas = bloque). Cliente de columna A.";

            if (MessageBox.Show(
                    "¿Deseas impactar " + dtExcel.Rows.Count + " líneas a CONTPAQi Comercial?\n\n" +
                    "Concepto : " + cbConcepto.Text + "\n" +
                    "Fecha    : " + txtFecha.Text + "\n" +
                    "Almacén  : " + cbAlmacen.Text + "\n" +
                    "Modo     : " + modoTexto,
                    "Confirmar Carga Masiva",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            btnCargaMasiva.Enabled = false;
            pbProgreso.Value       = 0;
            pbProgreso.Maximum     = dtExcel.Rows.Count;
            lblEstado.Text         = "Procesando...";
            lblEstado.ForeColor    = UITheme.Warning;
            Application.DoEvents();

            try
            {
                int exitosos = 0, errores = 0;
                var log = new List<string>();

                if (chkUnaSolaFactura.Checked)
                {
                    // ── Modo: un solo documento con todos los movimientos ─────
                    string codCte = ObtenerClientePrimeraFila();
                    if (string.IsNullOrEmpty(codCte))
                    {
                        MessageBox.Show(
                            "La columna A del Excel no tiene código de cliente.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int idDoc = GuardarDocumento(dtExcel.Rows, 0, dtExcel.Rows.Count - 1, codCte);
                    if (idDoc > 0)
                    {
                        exitosos         = dtExcel.Rows.Count;
                        pbProgreso.Value = dtExcel.Rows.Count;
                        log.Add("Doc #" + idDoc + " — " + dtExcel.Rows.Count +
                                " movimiento(s). Cliente: " + codCte);
                    }
                    else
                    {
                        errores++;
                        log.Add("Error al crear documento único. Código SDK: " + idDoc);
                    }
                }
                else
                {
                    // ── Modo: un documento por bloque ────────────────────────
                    int i = 0;
                    while (i < dtExcel.Rows.Count)
                    {
                        string codCte = dtExcel.Rows[i][COL_CLIENTE].ToString().Trim();

                        // Fila vacía = separador entre bloques
                        if (string.IsNullOrEmpty(codCte))
                        {
                            pbProgreso.Value = i + 1;
                            i++;
                            Application.DoEvents();
                            continue;
                        }

                        // Detectar hasta dónde llega el bloque
                        int desde = i, hasta = i;
                        while (hasta + 1 < dtExcel.Rows.Count &&
                               !string.IsNullOrEmpty(dtExcel.Rows[hasta + 1][COL_CLIENTE].ToString().Trim()))
                            hasta++;

                        int    numLineas = hasta - desde + 1;
                        string rangoTxt  = desde == hasta
                            ? "Fila " + (desde + 2)
                            : "Filas " + (desde + 2) + "-" + (hasta + 2);

                        lblEstado.Text = "Procesando " + rangoTxt + " — cliente " + codCte + "...";
                        Application.DoEvents();

                        // Validar que el cliente exista en CONTPAQi
                        if (BuscarIdClienteSQL(codCte) <= 0)
                        {
                            errores++;
                            log.Add(rangoTxt + ": cliente '" + codCte + "' no encontrado — omitido.");
                            for (int k = desde; k <= hasta; k++) pbProgreso.Value = k + 1;
                            i = hasta + 1;
                            Application.DoEvents();
                            continue;
                        }

                        try
                        {
                            int idDoc = GuardarDocumento(dtExcel.Rows, desde, hasta, codCte);
                            if (idDoc > 0)
                            {
                                exitosos++;
                                log.Add(rangoTxt + ": OK — Doc #" + idDoc +
                                        " (" + numLineas + " mov.) Cliente: " + codCte);
                            }
                            else
                            {
                                errores++;
                                log.Add(rangoTxt + ": error SDK código " + idDoc);
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

                // ── Resumen final ─────────────────────────────────────────────
                string resumen = "Carga masiva completada:\nExitosos: " + exitosos + "\nErrores: " + errores;
                if (log.Count > 0)
                    resumen += "\n\nDetalle:\n" + string.Join("\n", log.GetRange(0, Math.Min(log.Count, 10)));

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
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // NÚCLEO: CREAR Y GUARDAR DOCUMENTO EN CONTPAQÍ
        //
        // Datos que ya NO vienen del UI — se deducen automáticamente:
        //   Folio        → 0  (SDK asigna el siguiente disponible)
        //   Tipo cambio  → 1.0
        //   Referencia   → ""
        //   Moneda       → leída de admClientes por cliente vía SQL
        //   Agente       → leído de admClientes por cliente vía SQL
        //   Serie        → columna G del Excel
        //   Obs.Doc.     → columna E del Excel
        //   Obs.Mov.     → columna D del Excel
        //
        // codCteProv: código STRING del cliente, siempre de columna A del Excel.
        // ─────────────────────────────────────────────────────────────────────
        private int GuardarDocumento(DataRowCollection rows, int desde, int hasta,
                                     string codCteProv)
        {
            if (string.IsNullOrEmpty(codCteProv))
                throw new Exception("No se especificó código de cliente/proveedor.");

            // ── Concepto ─────────────────────────────────────────────────────
            var    itemConcepto   = (ItemCombo)cbConcepto.SelectedItem;
            string codConceptoStr = itemConcepto.Code;
            if (string.IsNullOrEmpty(codConceptoStr))
                throw new Exception("El concepto seleccionado no tiene código válido.");

            // ── Fecha ─────────────────────────────────────────────────────────
            if (!DateTime.TryParse(txtFecha.Text, out DateTime fechaDt))
                throw new Exception("Fecha inválida. Usa el formato AAAA/MM/DD.");
            string fecha = fechaDt.ToString("MM/dd/yyyy"); // el SDK espera MM/dd/yyyy

            // ── Serie — columna G del Excel ───────────────────────────────────
            string serie = "";
            if (desde < rows.Count && dtExcel.Columns.Count > COL_SERIE)
                serie = rows[desde][COL_SERIE].ToString().Trim();

            // ── Almacén ──────────────────────────────────────────────────────
            string codAlmacen = cbAlmacen.SelectedItem != null
                ? ((ItemCombo)cbAlmacen.SelectedItem).Code : "";
            if (string.IsNullOrEmpty(codAlmacen))
                throw new Exception("El almacén seleccionado no tiene código válido.");

            // ── Moneda y Agente — auto-leídos de admClientes por cliente ──────
            int    idMoneda  = 1;
            string codAgente = "";
            ObtenerDatosCliente(codCteProv, out idMoneda, out codAgente);

            // ── Observaciones del documento — columna E del Excel ─────────────
            string obsDocumento = "";
            if (desde < rows.Count && dtExcel.Columns.Count > COL_OBS_DOCUMENTO)
                obsDocumento = rows[desde][COL_OBS_DOCUMENTO].ToString().Trim();

            // ── Referencia — columna H del Excel (opcional) ───────────────────
            string referencia = "";
            if (desde < rows.Count && dtExcel.Columns.Count > COL_REFERENCIA)
            {
                referencia = rows[desde][COL_REFERENCIA].ToString().Trim();
                if (referencia.Length > 20) referencia = referencia.Substring(0, 20);
            }

            // ─────────────────────────────────────────────────────────────────
            // PASO 1 — Alta del documento (API Alto nivel)
            // ─────────────────────────────────────────────────────────────────
            var doc = new tDocumento
            {
                aFolio         = 0,     // 0 = SDK asigna el siguiente folio disponible
                aNumMoneda     = idMoneda,
                aTipoCambio    = 1.0,
                aImporte       = 0,     // 0 = SDK lo calcula de los movimientos
                aDescuentoDoc1 = 0,
                aDescuentoDoc2 = 0,
                aSistemaOrigen = 205,   // 205 = CONTPAQi Comercial Premium
                aCodConcepto   = codConceptoStr,
                aSerie         = serie,
                aFecha         = fecha,
                aCodigoCteProv = codCteProv,
                aCodigoAgente  = codAgente,
                aReferencia    = referencia,
                aAfecta        = 1,     // 1 = entradas (compras)
                aGasto1        = 0,
                aGasto2        = 0,
                aGasto3        = 0
            };

            int    idDocumento = 0;
            string paso        = "fAltaDocumento";
            try
            {
                int resAlta = SdkComercial.fAltaDocumento(ref idDocumento, ref doc);
                if (resAlta != 0)
                    throw new Exception("fAltaDocumento error: " + SdkComercial.DescribirError(resAlta));

                // ── PASO 2 — Observaciones del encabezado ─────────────────────
                if (!string.IsNullOrEmpty(obsDocumento))
                {
                    paso = "fSetDatoDocumento:COBSERVACIONES";
                    SdkComercial.fSetDatoDocumento("COBSERVACIONES", obsDocumento);
                }

                // ── PASO 3 — Movimientos ──────────────────────────────────────
                int consecutivo = 1;
                for (int i = desde; i <= hasta; i++)
                {
                    var    row         = rows[i];
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
                        aCosto            = precio, // en compras, costo = precio
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

                // Si el SDK no devolvió el ID en idDocumento, intentar leerlo del contexto
                if (idDocumento <= 0)
                {
                    string sId = SdkComercial.LeerCampoDocumento("CIDDOCUMENTO");
                    int.TryParse(sId, out idDocumento);
                    if (idDocumento <= 0) idDocumento = 1;
                }

                return idDocumento;
            }
            catch (Exception ex) when (!ex.Message.StartsWith("CRASH@"))
            {
                throw new Exception("CRASH@[" + paso + "]: " + ex.Message, ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // HELPERS SQL
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lee CIDMONEDA y CCODIGOAGENTE de admClientes para el cliente dado.
        /// Si falla, devuelve los valores por defecto (moneda 1, sin agente).
        /// </summary>
        private void ObtenerDatosCliente(string codCteProv, out int idMoneda, out string codAgente)
        {
            idMoneda  = 1;
            codAgente = "";
            if (string.IsNullOrEmpty(Program.ConnStrEmpresa) ||
                string.IsNullOrEmpty(codCteProv)) return;
            try
            {
                using (var conn = new SqlConnection(Program.ConnStrEmpresa))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT c.CIDMONEDA, a.CCODIGOAGENTE " +
                        "FROM admClientes c " +
                        "LEFT JOIN admAgentes a ON a.CIDAGENTE = c.CIDAGENTEVENTA " +
                        "WHERE c.CCODIGOCLIENTE = @cod",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", codCteProv);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                if (!r.IsDBNull(0)) idMoneda  = Convert.ToInt32(r[0]);
                                if (!r.IsDBNull(1)) codAgente = r[1].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch { /* si falla la consulta, usar defaults — no interrumpir el proceso */ }
        }

        /// <summary>
        /// Devuelve CIDCLIENTEPROVEEDOR dado un código de cliente; 0 si no existe.
        /// Se usa para validar antes de crear el documento.
        /// </summary>
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

        /// <summary>
        /// Devuelve el código de cliente de la primera fila no vacía del Excel (columna A).
        /// </summary>
        private string ObtenerClientePrimeraFila()
        {
            foreach (DataRow row in dtExcel.Rows)
            {
                string cod = row[COL_CLIENTE].ToString().Trim();
                if (!string.IsNullOrEmpty(cod)) return cod;
            }
            return "";
        }

        // ─────────────────────────────────────────────────────────────────────
        // VALIDACIONES — solo Concepto, Fecha y Almacén.
        // El cliente y el resto ya no se validan aquí porque vienen del Excel.
        // ─────────────────────────────────────────────────────────────────────
        private bool ValidarEncabezado()
        {
            if (cbConcepto.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un concepto.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(((ItemCombo)cbConcepto.SelectedItem).Code))
            {
                MessageBox.Show("El concepto seleccionado no tiene código válido.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!DateTime.TryParse(txtFecha.Text, out _))
            {
                MessageBox.Show("Ingresa la fecha en formato AAAA/MM/DD.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbAlmacen.SelectedItem == null ||
                string.IsNullOrEmpty(((ItemCombo)cbAlmacen.SelectedItem).Code))
            {
                MessageBox.Show("Selecciona un almacén válido.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // SALIR
        // ─────────────────────────────────────────────────────────────────────
        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Deseas cerrar la empresa y salir?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
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
            public string Code { get; }
            private string Nombre { get; }

            public ItemCombo(int id, string nombre, string code = "")
            {
                Id     = id;
                Code   = code ?? "";
                Nombre = nombre;
            }

            public override string ToString() => Nombre;
        }
    }
}
