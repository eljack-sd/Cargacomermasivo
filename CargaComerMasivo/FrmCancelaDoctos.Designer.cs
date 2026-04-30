namespace CargaComerMasivo
{
    partial class FrmCancelaDoctos
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel          pnlHeader;
        private System.Windows.Forms.Label          lblTitulo;
        private System.Windows.Forms.Panel          pnlFiltro;
        private System.Windows.Forms.Label          lblConcepto;
        private System.Windows.Forms.ComboBox       cbConcepto;
        private System.Windows.Forms.Button         btnBuscar;
        private System.Windows.Forms.DataGridView   dgvDocumentos;
        private System.Windows.Forms.Label          lblConteo;
        private System.Windows.Forms.Label          lblEstado;
        private System.Windows.Forms.Button         btnBorrar;
        private System.Windows.Forms.Button         btnSalir;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Instancias ────────────────────────────────────────────────────
            pnlHeader     = new System.Windows.Forms.Panel();
            lblTitulo     = new System.Windows.Forms.Label();
            pnlFiltro     = new System.Windows.Forms.Panel();
            lblConcepto   = new System.Windows.Forms.Label();
            cbConcepto    = new System.Windows.Forms.ComboBox();
            btnBuscar     = new System.Windows.Forms.Button();
            dgvDocumentos = new System.Windows.Forms.DataGridView();
            lblConteo     = new System.Windows.Forms.Label();
            lblEstado     = new System.Windows.Forms.Label();
            btnBorrar     = new System.Windows.Forms.Button();
            btnSalir      = new System.Windows.Forms.Button();

            var lblDot   = new System.Windows.Forms.Label();
            var pnlHLine = new System.Windows.Forms.Panel();
            var pnlAccent = new System.Windows.Forms.Panel();
            var lblFiltroTitle = new System.Windows.Forms.Label();

            // ── Form base ─────────────────────────────────────────────────────
            this.BackColor = UITheme.BgForm;
            this.ForeColor = UITheme.TextMain;

            // ── Header ────────────────────────────────────────────────────────
            pnlHeader.BackColor = UITheme.BgHeader;
            pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height    = 54;

            lblDot.Text      = "◆";
            lblDot.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblDot.ForeColor = UITheme.Danger;
            lblDot.AutoSize  = true;
            lblDot.Location  = new System.Drawing.Point(14, 16);

            lblTitulo.Text      = "Borrar Documentos";
            lblTitulo.Font      = UITheme.FntHuge;
            lblTitulo.ForeColor = UITheme.TextMain;
            lblTitulo.AutoSize  = true;
            lblTitulo.Location  = new System.Drawing.Point(44, 16);

            pnlHLine.BackColor = UITheme.Danger;
            pnlHLine.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlHLine.Height    = 2;

            pnlHeader.Controls.Add(lblDot);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(pnlHLine);

            // ── Panel Filtro ──────────────────────────────────────────────────
            pnlFiltro.BackColor = UITheme.BgCard;
            pnlFiltro.Location  = new System.Drawing.Point(14, 66);
            pnlFiltro.Size      = new System.Drawing.Size(956, 72);
            pnlFiltro.Anchor    = System.Windows.Forms.AnchorStyles.Top |
                                  System.Windows.Forms.AnchorStyles.Left |
                                  System.Windows.Forms.AnchorStyles.Right;

            pnlAccent.BackColor = UITheme.Accent;
            pnlAccent.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlAccent.Height    = 2;

            lblFiltroTitle.Text      = "FILTRAR POR CONCEPTO";
            lblFiltroTitle.Font      = UITheme.FntSection;
            lblFiltroTitle.ForeColor = UITheme.TextDim;
            lblFiltroTitle.BackColor = System.Drawing.Color.Transparent;
            lblFiltroTitle.AutoSize  = true;
            lblFiltroTitle.Location  = new System.Drawing.Point(10, 6);

            lblConcepto.Text      = "Concepto";
            lblConcepto.Font      = UITheme.FntSm;
            lblConcepto.ForeColor = UITheme.TextDim;
            lblConcepto.BackColor = System.Drawing.Color.Transparent;
            lblConcepto.AutoSize  = true;
            lblConcepto.Location  = new System.Drawing.Point(10, 20);

            cbConcepto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            UITheme.StyleCombo(cbConcepto);
            cbConcepto.Location = new System.Drawing.Point(10, 36);
            cbConcepto.Size     = new System.Drawing.Size(380, 26);
            cbConcepto.Anchor   = System.Windows.Forms.AnchorStyles.Top |
                                  System.Windows.Forms.AnchorStyles.Left;

            UITheme.StyleBtn(btnBuscar, UITheme.Accent);
            btnBuscar.Text     = "🔍  Buscar documentos";
            btnBuscar.Location = new System.Drawing.Point(400, 34);
            btnBuscar.Size     = new System.Drawing.Size(190, 30);
            btnBuscar.Click   += new System.EventHandler(this.btnBuscar_Click);

            pnlFiltro.Controls.Add(lblFiltroTitle);
            pnlFiltro.Controls.Add(lblConcepto);
            pnlFiltro.Controls.Add(cbConcepto);
            pnlFiltro.Controls.Add(btnBuscar);
            pnlFiltro.Controls.Add(pnlAccent);

            // ── Label conteo ──────────────────────────────────────────────────
            lblConteo.AutoSize = false;
            lblConteo.Font     = UITheme.FntSm;
            lblConteo.ForeColor= UITheme.TextDim;
            lblConteo.Location = new System.Drawing.Point(14, 146);
            lblConteo.Size     = new System.Drawing.Size(956, 18);
            lblConteo.Anchor   = System.Windows.Forms.AnchorStyles.Top |
                                  System.Windows.Forms.AnchorStyles.Left |
                                  System.Windows.Forms.AnchorStyles.Right;
            lblConteo.Text     = "Selecciona un concepto y presiona Buscar.";

            // ── Grid documentos ───────────────────────────────────────────────
            UITheme.StyleGrid(dgvDocumentos);
            dgvDocumentos.Location      = new System.Drawing.Point(14, 168);
            dgvDocumentos.Size          = new System.Drawing.Size(956, 390);
            dgvDocumentos.Anchor        = System.Windows.Forms.AnchorStyles.Top    |
                                          System.Windows.Forms.AnchorStyles.Bottom |
                                          System.Windows.Forms.AnchorStyles.Left   |
                                          System.Windows.Forms.AnchorStyles.Right;
            dgvDocumentos.MultiSelect   = true;
            dgvDocumentos.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            dgvDocumentos.SelectionChanged +=
                new System.EventHandler(this.dgvDocumentos_SelectionChanged);

            // ── Estado ────────────────────────────────────────────────────────
            lblEstado.AutoSize  = false;
            lblEstado.Font      = UITheme.FntSmBold;
            lblEstado.ForeColor = UITheme.Success;
            lblEstado.Location  = new System.Drawing.Point(14, 568);
            lblEstado.Size      = new System.Drawing.Size(620, 18);
            lblEstado.Anchor    = System.Windows.Forms.AnchorStyles.Bottom |
                                  System.Windows.Forms.AnchorStyles.Left   |
                                  System.Windows.Forms.AnchorStyles.Right;
            lblEstado.Text      = "";

            // ── Botones ───────────────────────────────────────────────────────
            UITheme.StyleBtn(btnBorrar, UITheme.Danger, large: true);
            btnBorrar.Text     = "🗑  Borrar Seleccionados";
            btnBorrar.Location = new System.Drawing.Point(14, 594);
            btnBorrar.Size     = new System.Drawing.Size(220, 42);
            btnBorrar.Enabled  = false;
            btnBorrar.Anchor   = System.Windows.Forms.AnchorStyles.Bottom |
                                  System.Windows.Forms.AnchorStyles.Left;
            btnBorrar.Click   += new System.EventHandler(this.btnBorrar_Click);

            UITheme.StyleBtn(btnSalir, UITheme.Neutral);
            btnSalir.Text     = "Cerrar";
            btnSalir.Location = new System.Drawing.Point(750, 594);
            btnSalir.Size     = new System.Drawing.Size(220, 42);
            btnSalir.Anchor   = System.Windows.Forms.AnchorStyles.Bottom |
                                 System.Windows.Forms.AnchorStyles.Right;
            btnSalir.Click   += new System.EventHandler(this.btnSalir_Click);

            // ── Form ─────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(984, 648);
            this.MinimumSize         = new System.Drawing.Size(800, 600);
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Borrar Documentos — CONTPAQi Comercial";

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFiltro);
            this.Controls.Add(lblConteo);
            this.Controls.Add(dgvDocumentos);
            this.Controls.Add(lblEstado);
            this.Controls.Add(btnBorrar);
            this.Controls.Add(btnSalir);
        }
    }
}
