namespace CargaComerMasivo
{
    partial class FrmPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        // Header
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitulo;

        // Excel frame
        private System.Windows.Forms.GroupBox grpExcel;
        private System.Windows.Forms.TextBox  txtRutaExcel;
        private System.Windows.Forms.Button   btnExaminar;
        private System.Windows.Forms.Button   btnCargarExcel;

        // Encabezado del documento
        private System.Windows.Forms.GroupBox  grpEncabezado;
        private System.Windows.Forms.Label     lblConcepto;
        private System.Windows.Forms.ComboBox  cbConcepto;
        private System.Windows.Forms.Label     lblFecha;
        private System.Windows.Forms.TextBox   txtFecha;
        private System.Windows.Forms.Label     lblFolio;
        private System.Windows.Forms.TextBox   txtFolio;
        private System.Windows.Forms.TextBox   txtSerie;
        private System.Windows.Forms.Label     lblSerie;
        private System.Windows.Forms.Button    btnSiguienteFolio;
        private System.Windows.Forms.Label     lblCliente;
        private System.Windows.Forms.ComboBox  cbCliente;
        private System.Windows.Forms.Label     lblAgente;
        private System.Windows.Forms.ComboBox  cbAgente;
        private System.Windows.Forms.Label     lblMoneda;
        private System.Windows.Forms.ComboBox  cbMoneda;
        private System.Windows.Forms.Label     lblTipoCambio;
        private System.Windows.Forms.TextBox   txtTipoCambio;
        private System.Windows.Forms.Label     lblAlmacen;
        private System.Windows.Forms.ComboBox  cbAlmacen;
        private System.Windows.Forms.Label     lblReferencia;
        private System.Windows.Forms.TextBox   txtReferencia;
        private System.Windows.Forms.Label     lblObservaciones;
        private System.Windows.Forms.TextBox   txtObservaciones;

        // Grid
        private System.Windows.Forms.DataGridView  dgvMovimientos;
        private System.Windows.Forms.Label         lblTotalLineas;
        private System.Windows.Forms.ProgressBar   pbProgreso;

        // Botones accion
        private System.Windows.Forms.CheckBox chkUnaSolaFactura;
        private System.Windows.Forms.Button   btnCargaMasiva;
        private System.Windows.Forms.Button   btnGuardar;
        private System.Windows.Forms.Button   btnSalir;
        private System.Windows.Forms.Label    lblEstado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader         = new System.Windows.Forms.Panel();
            this.lblTitulo         = new System.Windows.Forms.Label();
            this.grpExcel          = new System.Windows.Forms.GroupBox();
            this.txtRutaExcel      = new System.Windows.Forms.TextBox();
            this.btnExaminar       = new System.Windows.Forms.Button();
            this.btnCargarExcel    = new System.Windows.Forms.Button();
            this.grpEncabezado     = new System.Windows.Forms.GroupBox();
            this.lblConcepto       = new System.Windows.Forms.Label();
            this.cbConcepto        = new System.Windows.Forms.ComboBox();
            this.lblFecha          = new System.Windows.Forms.Label();
            this.txtFecha          = new System.Windows.Forms.TextBox();
            this.lblFolio          = new System.Windows.Forms.Label();
            this.txtFolio          = new System.Windows.Forms.TextBox();
            this.lblSerie          = new System.Windows.Forms.Label();
            this.txtSerie          = new System.Windows.Forms.TextBox();
            this.btnSiguienteFolio = new System.Windows.Forms.Button();
            this.lblCliente        = new System.Windows.Forms.Label();
            this.cbCliente         = new System.Windows.Forms.ComboBox();
            this.lblAgente         = new System.Windows.Forms.Label();
            this.cbAgente          = new System.Windows.Forms.ComboBox();
            this.lblMoneda         = new System.Windows.Forms.Label();
            this.cbMoneda          = new System.Windows.Forms.ComboBox();
            this.lblTipoCambio     = new System.Windows.Forms.Label();
            this.txtTipoCambio     = new System.Windows.Forms.TextBox();
            this.lblAlmacen        = new System.Windows.Forms.Label();
            this.cbAlmacen         = new System.Windows.Forms.ComboBox();
            this.lblReferencia     = new System.Windows.Forms.Label();
            this.txtReferencia     = new System.Windows.Forms.TextBox();
            this.lblObservaciones  = new System.Windows.Forms.Label();
            this.txtObservaciones  = new System.Windows.Forms.TextBox();
            this.dgvMovimientos    = new System.Windows.Forms.DataGridView();
            this.lblTotalLineas    = new System.Windows.Forms.Label();
            this.pbProgreso        = new System.Windows.Forms.ProgressBar();
            this.chkUnaSolaFactura = new System.Windows.Forms.CheckBox();
            this.btnCargaMasiva    = new System.Windows.Forms.Button();
            this.btnGuardar        = new System.Windows.Forms.Button();
            this.btnSalir          = new System.Windows.Forms.Button();
            this.lblEstado         = new System.Windows.Forms.Label();

            var font9  = new System.Drawing.Font("Century Gothic", 9F);
            var font9b = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);

            // ── Header ──────────────────────────────────────────────────────────
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height    = 44;
            pnlHeader.Controls.Add(lblTitulo);

            lblTitulo.AutoSize  = true;
            lblTitulo.Font      = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            lblTitulo.ForeColor = System.Drawing.Color.White;
            lblTitulo.Location  = new System.Drawing.Point(10, 8);
            lblTitulo.Text      = "  Carga Masiva de Documentos — CONTPAQi Comercial";

            // ── Archivo Excel ────────────────────────────────────────────────────
            grpExcel.Font      = font9b;
            grpExcel.ForeColor = System.Drawing.Color.DarkBlue;
            grpExcel.Location  = new System.Drawing.Point(10, 54);
            grpExcel.Size      = new System.Drawing.Size(1150, 60);
            grpExcel.Text      = "Archivo Excel";
            grpExcel.Anchor    = System.Windows.Forms.AnchorStyles.Top
                               | System.Windows.Forms.AnchorStyles.Left
                               | System.Windows.Forms.AnchorStyles.Right;
            grpExcel.Controls.AddRange(new System.Windows.Forms.Control[] {
                txtRutaExcel, btnExaminar, btnCargarExcel
            });

            txtRutaExcel.Font     = font9;
            txtRutaExcel.Location = new System.Drawing.Point(10, 25);
            txtRutaExcel.Size     = new System.Drawing.Size(820, 25);
            txtRutaExcel.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                  | System.Windows.Forms.AnchorStyles.Left
                                  | System.Windows.Forms.AnchorStyles.Right;
            txtRutaExcel.Text     = "";

            btnExaminar.Font     = font9b;
            btnExaminar.Location = new System.Drawing.Point(840, 24);
            btnExaminar.Size     = new System.Drawing.Size(100, 27);
            btnExaminar.Text     = "Examinar...";
            btnExaminar.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                 | System.Windows.Forms.AnchorStyles.Right;
            btnExaminar.Click   += new System.EventHandler(this.btnExaminar_Click);

            btnCargarExcel.BackColor = System.Drawing.Color.SteelBlue;
            btnCargarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCargarExcel.Font      = font9b;
            btnCargarExcel.ForeColor = System.Drawing.Color.White;
            btnCargarExcel.Location  = new System.Drawing.Point(948, 24);
            btnCargarExcel.Size      = new System.Drawing.Size(192, 27);
            btnCargarExcel.Text      = "▶ Cargar Archivo Excel";
            btnCargarExcel.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                     | System.Windows.Forms.AnchorStyles.Right;
            btnCargarExcel.Click    += new System.EventHandler(this.btnCargarExcel_Click);

            // ── Encabezado del Documento ─────────────────────────────────────────
            grpEncabezado.Font      = font9b;
            grpEncabezado.ForeColor = System.Drawing.Color.DarkBlue;
            grpEncabezado.Location  = new System.Drawing.Point(10, 124);
            grpEncabezado.Size      = new System.Drawing.Size(1150, 145);
            grpEncabezado.Text      = "Encabezado del Documento";
            grpEncabezado.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;

            System.Action<System.Windows.Forms.Label, string, int, int> setLbl =
                (l, t, x, y) => {
                    l.AutoSize  = true;
                    l.Font      = font9b;
                    l.ForeColor = System.Drawing.Color.Black;
                    l.Location  = new System.Drawing.Point(x, y);
                    l.Text      = t;
                };

            // Fila 1
            setLbl(lblConcepto, "Concepto:", 10, 22);
            cbConcepto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbConcepto.Font          = font9;
            cbConcepto.Location      = new System.Drawing.Point(10, 40);
            cbConcepto.Size          = new System.Drawing.Size(260, 25);

            setLbl(lblFecha, "Fecha (aaaa/mm/dd):", 280, 22);
            txtFecha.Font     = font9;
            txtFecha.Location = new System.Drawing.Point(280, 40);
            txtFecha.Size     = new System.Drawing.Size(130, 25);
            txtFecha.Text     = System.DateTime.Now.ToString("yyyy/MM/dd");

            setLbl(lblSerie, "Serie:", 420, 22);
            txtSerie.Font     = font9;
            txtSerie.Location = new System.Drawing.Point(420, 40);
            txtSerie.Size     = new System.Drawing.Size(55, 25);
            txtSerie.Text     = "";

            setLbl(lblFolio, "Folio:", 485, 22);
            txtFolio.Font     = font9;
            txtFolio.Location = new System.Drawing.Point(485, 40);
            txtFolio.Size     = new System.Drawing.Size(80, 25);
            txtFolio.Text     = "0";

            btnSiguienteFolio.Font     = font9;
            btnSiguienteFolio.Location = new System.Drawing.Point(573, 38);
            btnSiguienteFolio.Size     = new System.Drawing.Size(120, 27);
            btnSiguienteFolio.Text     = "Sig. Folio";
            btnSiguienteFolio.Click   += new System.EventHandler(this.btnSiguienteFolio_Click);

            setLbl(lblReferencia, "Referencia:", 703, 22);
            txtReferencia.Font     = font9;
            txtReferencia.Location = new System.Drawing.Point(703, 40);
            txtReferencia.Size     = new System.Drawing.Size(150, 25);

            setLbl(lblObservaciones, "Observaciones:", 862, 22);
            txtObservaciones.Font     = font9;
            txtObservaciones.Location = new System.Drawing.Point(862, 40);
            txtObservaciones.Size     = new System.Drawing.Size(275, 25);

            // Fila 2
            setLbl(lblCliente, "Cliente / Proveedor:", 10, 78);
            cbCliente.Font     = font9;
            cbCliente.Location = new System.Drawing.Point(10, 96);
            cbCliente.Size     = new System.Drawing.Size(340, 25);
            cbCliente.SelectedIndexChanged += new System.EventHandler(this.cbCliente_SelectedIndexChanged);

            setLbl(lblAgente, "Agente:", 360, 78);
            cbAgente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAgente.Font          = font9;
            cbAgente.Location      = new System.Drawing.Point(360, 96);
            cbAgente.Size          = new System.Drawing.Size(240, 25);

            setLbl(lblMoneda, "Moneda:", 610, 78);
            cbMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbMoneda.Font          = font9;
            cbMoneda.Location      = new System.Drawing.Point(610, 96);
            cbMoneda.Size          = new System.Drawing.Size(160, 25);

            setLbl(lblTipoCambio, "Tipo de Cambio:", 780, 78);
            txtTipoCambio.Font     = font9;
            txtTipoCambio.Location = new System.Drawing.Point(780, 96);
            txtTipoCambio.Size     = new System.Drawing.Size(90, 25);
            txtTipoCambio.Text     = "1.0";

            setLbl(lblAlmacen, "Almacen:", 880, 78);
            cbAlmacen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAlmacen.Font          = font9;
            cbAlmacen.Location      = new System.Drawing.Point(880, 96);
            cbAlmacen.Size          = new System.Drawing.Size(257, 25);

            grpEncabezado.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblConcepto,   cbConcepto,
                lblFecha,      txtFecha,
                lblSerie,      txtSerie,
                lblFolio,      txtFolio,      btnSiguienteFolio,
                lblReferencia, txtReferencia,
                lblObservaciones, txtObservaciones,
                lblCliente,    cbCliente,
                lblAgente,     cbAgente,
                lblMoneda,     cbMoneda,
                lblTipoCambio, txtTipoCambio,
                lblAlmacen,    cbAlmacen
            });

            // ── Grid de movimientos ──────────────────────────────────────────────
            dgvMovimientos.Location                          = new System.Drawing.Point(10, 278);
            dgvMovimientos.Size                              = new System.Drawing.Size(1150, 334);
            dgvMovimientos.Anchor                            = System.Windows.Forms.AnchorStyles.Top
                                                             | System.Windows.Forms.AnchorStyles.Bottom
                                                             | System.Windows.Forms.AnchorStyles.Left
                                                             | System.Windows.Forms.AnchorStyles.Right;
            dgvMovimientos.ReadOnly                          = true;
            dgvMovimientos.AllowUserToAddRows                = false;
            dgvMovimientos.AutoSizeColumnsMode               = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvMovimientos.BackgroundColor                   = System.Drawing.Color.White;
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Font = font9b;
            dgvMovimientos.DefaultCellStyle.Font             = font9;
            dgvMovimientos.RowHeadersVisible                 = false;
            dgvMovimientos.SelectionMode                     = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvMovimientos.BorderStyle                       = System.Windows.Forms.BorderStyle.Fixed3D;

            // ── Barra de progreso (delgada, justo bajo el grid) ──────────────────
            pbProgreso.Location = new System.Drawing.Point(10, 618);
            pbProgreso.Size     = new System.Drawing.Size(1150, 6);
            pbProgreso.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                                | System.Windows.Forms.AnchorStyles.Left
                                | System.Windows.Forms.AnchorStyles.Right;
            pbProgreso.Minimum  = 0;
            pbProgreso.Value    = 0;
            pbProgreso.Style    = System.Windows.Forms.ProgressBarStyle.Continuous;

            // ── Info / controles inferiores ──────────────────────────────────────
            lblTotalLineas.AutoSize  = true;
            lblTotalLineas.Font      = font9;
            lblTotalLineas.ForeColor = System.Drawing.Color.DimGray;
            lblTotalLineas.Location  = new System.Drawing.Point(10, 630);
            lblTotalLineas.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                     | System.Windows.Forms.AnchorStyles.Left;
            lblTotalLineas.Text      = "Total lineas: 0";

            chkUnaSolaFactura.AutoSize  = true;
            chkUnaSolaFactura.Font      = font9b;
            chkUnaSolaFactura.Location  = new System.Drawing.Point(10, 652);
            chkUnaSolaFactura.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                        | System.Windows.Forms.AnchorStyles.Left;
            chkUnaSolaFactura.Text      = "Una Sola Factura (todos los movimientos en un documento)";

            btnCargaMasiva.BackColor = System.Drawing.Color.FromArgb(0, 150, 50);
            btnCargaMasiva.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCargaMasiva.Font      = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            btnCargaMasiva.ForeColor = System.Drawing.Color.White;
            btnCargaMasiva.Location  = new System.Drawing.Point(10, 682);
            btnCargaMasiva.Size      = new System.Drawing.Size(200, 42);
            btnCargaMasiva.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                     | System.Windows.Forms.AnchorStyles.Left;
            btnCargaMasiva.Text      = "▶ CARGA MASIVA";
            btnCargaMasiva.Click    += new System.EventHandler(this.btnCargaMasiva_Click);

            btnGuardar.BackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGuardar.Font      = font9b;
            btnGuardar.ForeColor = System.Drawing.Color.White;
            btnGuardar.Location  = new System.Drawing.Point(220, 682);
            btnGuardar.Size      = new System.Drawing.Size(170, 42);
            btnGuardar.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                 | System.Windows.Forms.AnchorStyles.Left;
            btnGuardar.Text      = "Guardar Comercial";
            btnGuardar.Click    += new System.EventHandler(this.btnGuardar_Click);

            btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSalir.Font      = font9;
            btnSalir.Location  = new System.Drawing.Point(960, 682);
            btnSalir.Size      = new System.Drawing.Size(200, 42);
            btnSalir.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                               | System.Windows.Forms.AnchorStyles.Right;
            btnSalir.Text      = "Salir / Cerrar Empresa";
            btnSalir.Click    += new System.EventHandler(this.btnSalir_Click);

            lblEstado.AutoSize  = false;
            lblEstado.Font      = font9b;
            lblEstado.Location  = new System.Drawing.Point(400, 695);
            lblEstado.Size      = new System.Drawing.Size(540, 20);
            lblEstado.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                | System.Windows.Forms.AnchorStyles.Left
                                | System.Windows.Forms.AnchorStyles.Right;
            lblEstado.Text      = "";

            // ── Form ─────────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1170, 735);
            this.MinimumSize         = new System.Drawing.Size(1186, 774);
            this.WindowState         = System.Windows.Forms.FormWindowState.Maximized;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name                = "FrmPrincipal";
            this.Text                = "Comercial Masivo 2.0 — Carga de Documentos";

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                pnlHeader,
                grpExcel,
                grpEncabezado,
                dgvMovimientos,
                pbProgreso,
                lblTotalLineas,
                chkUnaSolaFactura,
                btnCargaMasiva,
                btnGuardar,
                btnSalir,
                lblEstado
            });
        }
    }
}
