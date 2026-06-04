namespace CargaComerMasivo
{
    partial class FrmPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        // ── Header ───────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitulo;
        private RoundedButton btnHeaderCancelar;
        private RoundedButton btnHeaderAlmacenes;
        private RoundedButton btnHeaderAgentes;

        // ── Tarjeta Archivo Excel ────────────────────────────────────────────
        private System.Windows.Forms.Panel  pnlExcel;
        private System.Windows.Forms.TextBox  txtRutaExcel;
        private RoundedButton btnExaminar;
        private RoundedButton btnCargarExcel;

        // ── Tarjeta Encabezado del Documento ────────────────────────────────
        private System.Windows.Forms.Panel    pnlEncabezado;
        private System.Windows.Forms.Label    lblConcepto;
        private System.Windows.Forms.ComboBox cbConcepto;
        private System.Windows.Forms.Label    lblFecha;
        private System.Windows.Forms.TextBox  txtFecha;
        private System.Windows.Forms.Label    lblAlmacen;
        private System.Windows.Forms.ComboBox cbAlmacen;

        // ── Grid ─────────────────────────────────────────────────────────────
        private System.Windows.Forms.DataGridView dgvMovimientos;

        // ── Barra inferior (acción) ───────────────────────────────────────────
        private System.Windows.Forms.Panel  pnlBottomBar;
        private System.Windows.Forms.ProgressBar  pbProgreso;
        private System.Windows.Forms.Label        lblTotalLineas;
        private System.Windows.Forms.CheckBox     chkUnaSolaFactura;
        private RoundedButton btnCargaMasiva;
        private System.Windows.Forms.Label        lblEstado;
        private RoundedButton btnSalir;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader          = new System.Windows.Forms.Panel();
            this.lblTitulo          = new System.Windows.Forms.Label();
            this.btnHeaderCancelar  = new RoundedButton();
            this.btnHeaderAlmacenes = new RoundedButton();
            this.btnHeaderAgentes   = new RoundedButton();
            this.pnlExcel           = new System.Windows.Forms.Panel();
            this.txtRutaExcel       = new System.Windows.Forms.TextBox();
            this.btnExaminar        = new RoundedButton();
            this.btnCargarExcel     = new RoundedButton();
            this.pnlEncabezado      = new System.Windows.Forms.Panel();
            this.lblConcepto        = new System.Windows.Forms.Label();
            this.cbConcepto         = new System.Windows.Forms.ComboBox();
            this.lblFecha           = new System.Windows.Forms.Label();
            this.txtFecha           = new System.Windows.Forms.TextBox();
            this.lblAlmacen         = new System.Windows.Forms.Label();
            this.cbAlmacen          = new System.Windows.Forms.ComboBox();
            this.dgvMovimientos     = new System.Windows.Forms.DataGridView();
            this.pnlBottomBar       = new System.Windows.Forms.Panel();
            this.pbProgreso         = new System.Windows.Forms.ProgressBar();
            this.lblTotalLineas     = new System.Windows.Forms.Label();
            this.chkUnaSolaFactura  = new System.Windows.Forms.CheckBox();
            this.btnCargaMasiva     = new RoundedButton();
            this.lblEstado          = new System.Windows.Forms.Label();
            this.btnSalir           = new RoundedButton();

            // ── Form ─────────────────────────────────────────────────────────
            this.BackColor = UITheme.BgForm;
            this.ForeColor = UITheme.TextMain;

            // ════════════════════════════════════════════════════════════════
            // HEADER
            // ════════════════════════════════════════════════════════════════
            var lblDot   = new System.Windows.Forms.Label();
            var pnlHLine = new System.Windows.Forms.Panel();

            pnlHeader.BackColor = UITheme.BgHeader;
            pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height    = 54;

            // ◆ accent dot
            lblDot.Text      = "◆";
            lblDot.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            lblDot.ForeColor = UITheme.Accent;
            lblDot.AutoSize  = true;
            lblDot.Location  = new System.Drawing.Point(16, 17);

            lblTitulo.Text      = "Carga Masiva de Documentos — CONTPAQi Comercial";
            lblTitulo.Font      = UITheme.FntHuge;
            lblTitulo.ForeColor = UITheme.TextMain;
            lblTitulo.AutoSize  = true;
            lblTitulo.Location  = new System.Drawing.Point(44, 17);

            // 1px accent line at bottom of header
            pnlHLine.BackColor = UITheme.Border;
            pnlHLine.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlHLine.Height    = 1;

            // Nav buttons — derecha. Posición inicial arbitraria; se corrige
            // en AjustarBotonesHeader() al hacer Load y en cada Resize del panel.
            UITheme.StyleRoundBtn(btnHeaderCancelar, UITheme.Danger);
            btnHeaderCancelar.Text   = "✕  Cancelar Docs";
            btnHeaderCancelar.Size   = new System.Drawing.Size(148, 32);
            btnHeaderCancelar.Location = new System.Drawing.Point(0, 11);
            btnHeaderCancelar.Anchor = System.Windows.Forms.AnchorStyles.Top |
                                       System.Windows.Forms.AnchorStyles.Left;
            btnHeaderCancelar.Click += (s, e) => new FrmCancelaDoctos().ShowDialog();

            UITheme.StyleRoundBtn(btnHeaderAlmacenes, UITheme.Neutral);
            btnHeaderAlmacenes.Text   = "Almacenes";
            btnHeaderAlmacenes.Size   = new System.Drawing.Size(110, 32);
            btnHeaderAlmacenes.Location = new System.Drawing.Point(0, 11);
            btnHeaderAlmacenes.Anchor = System.Windows.Forms.AnchorStyles.Top |
                                        System.Windows.Forms.AnchorStyles.Left;
            btnHeaderAlmacenes.Click += (s, e) => new FrmAlmacenes().ShowDialog();

            UITheme.StyleRoundBtn(btnHeaderAgentes, UITheme.Neutral);
            btnHeaderAgentes.Text   = "Agentes";
            btnHeaderAgentes.Size   = new System.Drawing.Size(98, 32);
            btnHeaderAgentes.Location = new System.Drawing.Point(0, 11);
            btnHeaderAgentes.Anchor = System.Windows.Forms.AnchorStyles.Top |
                                      System.Windows.Forms.AnchorStyles.Left;
            btnHeaderAgentes.Click += (s, e) => new FrmAgentes().ShowDialog();

            // Reubicar en cada Resize del panel (DockStyle.Top cambia el ancho)
            pnlHeader.Resize += (s, pe) => AjustarBotonesHeader();

            pnlHeader.Controls.Add(lblDot);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(pnlHLine);
            pnlHeader.Controls.Add(btnHeaderCancelar);
            // btnHeaderAlmacenes y btnHeaderAgentes: funciones listas, botones ocultos por ahora
            // pnlHeader.Controls.Add(btnHeaderAlmacenes);
            // pnlHeader.Controls.Add(btnHeaderAgentes);

            // ════════════════════════════════════════════════════════════════
            // TARJETA — ARCHIVO EXCEL
            // ════════════════════════════════════════════════════════════════
            pnlExcel.BackColor = UITheme.BgCard;
            pnlExcel.Location  = new System.Drawing.Point(12, 64);
            pnlExcel.Size      = new System.Drawing.Size(1146, 64);
            pnlExcel.Anchor    = System.Windows.Forms.AnchorStyles.Top
                               | System.Windows.Forms.AnchorStyles.Left
                               | System.Windows.Forms.AnchorStyles.Right;
            // 1px border via Paint
            pnlExcel.Paint += (s, pe) =>
            {
                var c = (System.Windows.Forms.Control)s;
                using (var pen = new System.Drawing.Pen(UITheme.Border, 1))
                    pe.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            };

            var lblExcelSection = new System.Windows.Forms.Label();
            lblExcelSection.Text      = "ARCHIVO EXCEL  —  A:Cliente  B:Producto  C:Cant  D:ObsMov  E:ObsDoc  F:Precio  G:Serie  H:Referencia";
            lblExcelSection.Font      = UITheme.FntSection;
            lblExcelSection.ForeColor = UITheme.TextMuted;
            lblExcelSection.BackColor = System.Drawing.Color.Transparent;
            lblExcelSection.AutoSize  = true;
            lblExcelSection.Location  = new System.Drawing.Point(12, 7);

            UITheme.StyleInput(txtRutaExcel);
            txtRutaExcel.Location = new System.Drawing.Point(12, 30);
            txtRutaExcel.Size     = new System.Drawing.Size(758, 26);
            txtRutaExcel.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                  | System.Windows.Forms.AnchorStyles.Left
                                  | System.Windows.Forms.AnchorStyles.Right;
            txtRutaExcel.Text     = "";

            UITheme.StyleRoundBtn(btnExaminar, UITheme.Neutral);
            btnExaminar.Text     = "Examinar...";
            btnExaminar.Location = new System.Drawing.Point(778, 28);
            btnExaminar.Size     = new System.Drawing.Size(148, 30);
            btnExaminar.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                 | System.Windows.Forms.AnchorStyles.Right;
            btnExaminar.Click   += new System.EventHandler(this.btnExaminar_Click);

            UITheme.StyleRoundBtn(btnCargarExcel, UITheme.Accent);
            btnCargarExcel.Text     = "▶  Cargar Excel";
            btnCargarExcel.Location = new System.Drawing.Point(934, 28);
            btnCargarExcel.Size     = new System.Drawing.Size(200, 30);
            btnCargarExcel.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Right;
            btnCargarExcel.Click   += new System.EventHandler(this.btnCargarExcel_Click);

            pnlExcel.Controls.Add(lblExcelSection);
            pnlExcel.Controls.Add(txtRutaExcel);
            pnlExcel.Controls.Add(btnExaminar);
            pnlExcel.Controls.Add(btnCargarExcel);

            // ════════════════════════════════════════════════════════════════
            // TARJETA — ENCABEZADO DEL DOCUMENTO
            // ════════════════════════════════════════════════════════════════
            pnlEncabezado.BackColor = UITheme.BgCard;
            pnlEncabezado.Location  = new System.Drawing.Point(12, 138);
            pnlEncabezado.Size      = new System.Drawing.Size(1146, 72);
            pnlEncabezado.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;
            pnlEncabezado.Paint += (s, pe) =>
            {
                var c = (System.Windows.Forms.Control)s;
                using (var pen = new System.Drawing.Pen(UITheme.Border, 1))
                    pe.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            };

            var lblEncSection = new System.Windows.Forms.Label();
            lblEncSection.Text      = "ENCABEZADO DEL DOCUMENTO";
            lblEncSection.Font      = UITheme.FntSection;
            lblEncSection.ForeColor = UITheme.TextMuted;
            lblEncSection.BackColor = System.Drawing.Color.Transparent;
            lblEncSection.AutoSize  = true;
            lblEncSection.Location  = new System.Drawing.Point(12, 7);

            // Concepto
            lblConcepto.Text      = "Concepto";
            lblConcepto.Font      = UITheme.FntSm;
            lblConcepto.ForeColor = UITheme.TextDim;
            lblConcepto.BackColor = System.Drawing.Color.Transparent;
            lblConcepto.AutoSize  = true;
            lblConcepto.Location  = new System.Drawing.Point(12, 22);

            cbConcepto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            UITheme.StyleCombo(cbConcepto);
            cbConcepto.Location = new System.Drawing.Point(12, 38);
            cbConcepto.Size     = new System.Drawing.Size(340, 26);

            // Fecha
            lblFecha.Text      = "Fecha  (aaaa/mm/dd)";
            lblFecha.Font      = UITheme.FntSm;
            lblFecha.ForeColor = UITheme.TextDim;
            lblFecha.BackColor = System.Drawing.Color.Transparent;
            lblFecha.AutoSize  = true;
            lblFecha.Location  = new System.Drawing.Point(362, 22);

            UITheme.StyleInput(txtFecha);
            txtFecha.Location = new System.Drawing.Point(362, 38);
            txtFecha.Size     = new System.Drawing.Size(144, 26);
            txtFecha.Text     = System.DateTime.Now.ToString("yyyy/MM/dd");

            // Almacén
            lblAlmacen.Text      = "Almacén";
            lblAlmacen.Font      = UITheme.FntSm;
            lblAlmacen.ForeColor = UITheme.TextDim;
            lblAlmacen.BackColor = System.Drawing.Color.Transparent;
            lblAlmacen.AutoSize  = true;
            lblAlmacen.Location  = new System.Drawing.Point(516, 22);

            cbAlmacen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            UITheme.StyleCombo(cbAlmacen);
            cbAlmacen.Location = new System.Drawing.Point(516, 38);
            cbAlmacen.Size     = new System.Drawing.Size(618, 26);
            cbAlmacen.Anchor   = System.Windows.Forms.AnchorStyles.Top
                               | System.Windows.Forms.AnchorStyles.Left
                               | System.Windows.Forms.AnchorStyles.Right;

            pnlEncabezado.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblEncSection,
                lblConcepto, cbConcepto,
                lblFecha,    txtFecha,
                lblAlmacen,  cbAlmacen
            });

            // ════════════════════════════════════════════════════════════════
            // GRID DE MOVIMIENTOS
            // ════════════════════════════════════════════════════════════════
            UITheme.StyleGrid(dgvMovimientos);
            dgvMovimientos.Location = new System.Drawing.Point(12, 220);
            dgvMovimientos.Size     = new System.Drawing.Size(1146, 400);
            dgvMovimientos.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Bottom
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;

            // ════════════════════════════════════════════════════════════════
            // BARRA INFERIOR DE ACCIÓN
            // ════════════════════════════════════════════════════════════════
            pnlBottomBar.BackColor = UITheme.BgBottom;
            pnlBottomBar.Location  = new System.Drawing.Point(0, 626);
            pnlBottomBar.Size      = new System.Drawing.Size(1170, 118);
            pnlBottomBar.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                   | System.Windows.Forms.AnchorStyles.Left
                                   | System.Windows.Forms.AnchorStyles.Right;
            // 1px border en la parte superior
            pnlBottomBar.Paint += (s, pe) =>
            {
                var c = (System.Windows.Forms.Control)s;
                using (var pen = new System.Drawing.Pen(UITheme.Border, 1))
                    pe.Graphics.DrawLine(pen, 0, 0, c.Width, 0);
            };

            // Progress bar — full width, top of bar
            pbProgreso.Location  = new System.Drawing.Point(0, 0);
            pbProgreso.Size      = new System.Drawing.Size(1170, 3);
            pbProgreso.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                 | System.Windows.Forms.AnchorStyles.Left
                                 | System.Windows.Forms.AnchorStyles.Right;
            pbProgreso.Minimum   = 0;
            pbProgreso.Value     = 0;
            pbProgreso.Style     = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbProgreso.ForeColor = UITheme.Accent;

            // Total líneas
            lblTotalLineas.AutoSize  = true;
            lblTotalLineas.Font      = UITheme.FntSm;
            lblTotalLineas.ForeColor = UITheme.TextDim;
            lblTotalLineas.Location  = new System.Drawing.Point(16, 10);
            lblTotalLineas.BackColor = System.Drawing.Color.Transparent;
            lblTotalLineas.Text      = "Total líneas: 0";

            // CheckBox Una Sola Factura
            UITheme.StyleCheck(chkUnaSolaFactura);
            chkUnaSolaFactura.AutoSize  = true;
            chkUnaSolaFactura.Location  = new System.Drawing.Point(16, 30);
            chkUnaSolaFactura.BackColor = System.Drawing.Color.Transparent;
            chkUnaSolaFactura.Text      = "Una Sola Factura — todos los movimientos en un único documento";

            // Botón CARGA MASIVA — grande y prominente
            UITheme.StyleRoundBtn(btnCargaMasiva, UITheme.Success, large: true);
            btnCargaMasiva.Radius   = 8;
            btnCargaMasiva.Text     = "▶  CARGA MASIVA";
            btnCargaMasiva.Location = new System.Drawing.Point(16, 58);
            btnCargaMasiva.Size     = new System.Drawing.Size(234, 48);
            btnCargaMasiva.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                                    | System.Windows.Forms.AnchorStyles.Left;
            btnCargaMasiva.Click   += new System.EventHandler(this.btnCargaMasiva_Click);

            // Estado — texto de resultado
            lblEstado.AutoSize  = false;
            lblEstado.Font      = UITheme.FntSm;
            lblEstado.ForeColor = UITheme.TextDim;
            lblEstado.BackColor = System.Drawing.Color.Transparent;
            lblEstado.Location  = new System.Drawing.Point(264, 72);
            lblEstado.Size      = new System.Drawing.Size(680, 20);
            lblEstado.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                | System.Windows.Forms.AnchorStyles.Left
                                | System.Windows.Forms.AnchorStyles.Right;
            lblEstado.Text      = "";

            // Botón Salir — derecha
            UITheme.StyleRoundBtn(btnSalir, UITheme.Neutral);
            btnSalir.Radius   = 8;
            btnSalir.Text     = "Salir / Cerrar Empresa";
            btnSalir.Location = new System.Drawing.Point(910, 58);
            btnSalir.Size     = new System.Drawing.Size(220, 48);
            btnSalir.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                              | System.Windows.Forms.AnchorStyles.Right;
            btnSalir.Click   += new System.EventHandler(this.btnSalir_Click);

            pnlBottomBar.Controls.AddRange(new System.Windows.Forms.Control[] {
                pbProgreso,
                lblTotalLineas,
                chkUnaSolaFactura,
                btnCargaMasiva,
                lblEstado,
                btnSalir
            });

            // ════════════════════════════════════════════════════════════════
            // FORM
            // ════════════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1170, 744);
            this.MinimumSize         = new System.Drawing.Size(1000, 700);
            this.WindowState         = System.Windows.Forms.FormWindowState.Maximized;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name                = "FrmPrincipal";
            this.Text                = "Comercial Masivo — Carga de Documentos";

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                pnlHeader,
                pnlExcel,
                pnlEncabezado,
                dgvMovimientos,
                pnlBottomBar
            });
        }
    }
}
