namespace CargaComerMasivo
{
    partial class FrmConexion
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel    pnlHeader;
        private System.Windows.Forms.Label    lblTitulo;
        private System.Windows.Forms.Label    lblSubtitulo;
        private System.Windows.Forms.Panel    grpEmpresa;
        private System.Windows.Forms.Label    lblEmpresaLbl;
        private System.Windows.Forms.ComboBox cbEmpresa;
        private System.Windows.Forms.Label    lblManual;
        private System.Windows.Forms.TextBox  txtRutaManual;
        private System.Windows.Forms.Button   btnBuscar;
        private System.Windows.Forms.Button   btnAbrir;
        private System.Windows.Forms.Button   btnSalir;
        private System.Windows.Forms.Label    lblEstado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.BackColor = UITheme.BgForm;
            this.ForeColor = UITheme.TextMain;

            // ── Header ──
            pnlHeader    = new System.Windows.Forms.Panel();
            lblTitulo    = new System.Windows.Forms.Label();
            lblSubtitulo = new System.Windows.Forms.Label();

            var lblIcon   = new System.Windows.Forms.Label();
            var pnlLine   = new System.Windows.Forms.Panel();

            pnlHeader.BackColor = UITheme.BgHeader;
            pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height    = 72;

            lblIcon.Text      = "⬡";
            lblIcon.Font      = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            lblIcon.ForeColor = UITheme.Accent;
            lblIcon.AutoSize  = true;
            lblIcon.Location  = new System.Drawing.Point(18, 14);

            lblTitulo.Text      = "Comercial Masivo";
            lblTitulo.Font      = UITheme.FntTitle;
            lblTitulo.ForeColor = UITheme.TextMain;
            lblTitulo.AutoSize  = true;
            lblTitulo.Location  = new System.Drawing.Point(58, 12);

            var lblBadge = new System.Windows.Forms.Label();
            lblBadge.Text      = " v2.0 ";
            lblBadge.Font      = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            lblBadge.ForeColor = UITheme.BgHeader;
            lblBadge.BackColor = UITheme.Accent;
            lblBadge.AutoSize  = true;
            lblBadge.Location  = new System.Drawing.Point(232, 18);

            lblSubtitulo.Text      = "CONTPAQi Comercial Premium  ·  Carga Masiva de Documentos";
            lblSubtitulo.Font      = UITheme.FntSub;
            lblSubtitulo.ForeColor = UITheme.TextMuted;
            lblSubtitulo.AutoSize  = true;
            lblSubtitulo.Location  = new System.Drawing.Point(60, 42);

            pnlLine.BackColor = UITheme.Accent;
            pnlLine.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlLine.Height    = 2;

            pnlHeader.Controls.Add(lblIcon);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblBadge);
            pnlHeader.Controls.Add(lblSubtitulo);
            pnlHeader.Controls.Add(pnlLine);

            // ── Tarjeta empresa ──
            grpEmpresa    = new System.Windows.Forms.Panel();
            lblEmpresaLbl = new System.Windows.Forms.Label();
            cbEmpresa     = new System.Windows.Forms.ComboBox();
            lblManual     = new System.Windows.Forms.Label();
            txtRutaManual = new System.Windows.Forms.TextBox();
            btnBuscar     = new System.Windows.Forms.Button();

            var pnlStrip   = new System.Windows.Forms.Panel();
            var lblSection = new System.Windows.Forms.Label();

            grpEmpresa.BackColor = UITheme.BgCard;
            grpEmpresa.Location  = new System.Drawing.Point(22, 86);
            grpEmpresa.Size      = new System.Drawing.Size(456, 126);

            pnlStrip.BackColor = UITheme.Accent;
            pnlStrip.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlStrip.Height    = 2;

            lblSection.Text      = "EMPRESA";
            lblSection.Font      = UITheme.FntSection;
            lblSection.ForeColor = UITheme.TextDim;
            lblSection.BackColor = System.Drawing.Color.Transparent;
            lblSection.AutoSize  = true;
            lblSection.Location  = new System.Drawing.Point(12, 8);

            lblEmpresaLbl.Text      = "Selecciona la empresa registrada en CONTPAQi:";
            lblEmpresaLbl.Font      = UITheme.FntSm;
            lblEmpresaLbl.ForeColor = UITheme.TextDim;
            lblEmpresaLbl.AutoSize  = true;
            lblEmpresaLbl.Location  = new System.Drawing.Point(12, 28);

            UITheme.StyleCombo(cbEmpresa);
            cbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbEmpresa.Location      = new System.Drawing.Point(12, 46);
            cbEmpresa.Size          = new System.Drawing.Size(432, 26);

            lblManual.Text      = "Ruta de datos de la empresa:";
            lblManual.Font      = UITheme.FntSm;
            lblManual.ForeColor = UITheme.Danger;
            lblManual.AutoSize  = true;
            lblManual.Location  = new System.Drawing.Point(12, 82);
            lblManual.Visible   = false;

            UITheme.StyleInput(txtRutaManual);
            txtRutaManual.Location = new System.Drawing.Point(12, 98);
            txtRutaManual.Size     = new System.Drawing.Size(384, 26);
            txtRutaManual.Visible  = false;

            UITheme.StyleBtn(btnBuscar, UITheme.Neutral);
            btnBuscar.Text     = "···";
            btnBuscar.Location = new System.Drawing.Point(402, 96);
            btnBuscar.Size     = new System.Drawing.Size(42, 30);
            btnBuscar.Visible  = false;
            btnBuscar.Click   += new System.EventHandler(this.btnBuscar_Click);

            grpEmpresa.Controls.Add(lblSection);
            grpEmpresa.Controls.Add(lblEmpresaLbl);
            grpEmpresa.Controls.Add(cbEmpresa);
            grpEmpresa.Controls.Add(lblManual);
            grpEmpresa.Controls.Add(txtRutaManual);
            grpEmpresa.Controls.Add(btnBuscar);
            grpEmpresa.Controls.Add(pnlStrip);

            // ── Botones ──
            btnAbrir  = new System.Windows.Forms.Button();
            btnSalir  = new System.Windows.Forms.Button();

            UITheme.StyleBtn(btnAbrir, UITheme.Accent);
            btnAbrir.Text     = "▶   Abrir Empresa";
            btnAbrir.Location = new System.Drawing.Point(22, 228);
            btnAbrir.Size     = new System.Drawing.Size(220, 42);
            btnAbrir.Enabled  = false;
            btnAbrir.Click   += new System.EventHandler(this.btnAbrir_Click);

            UITheme.StyleBtn(btnSalir, UITheme.Neutral);
            btnSalir.ForeColor = UITheme.TextDim;
            btnSalir.Text      = "Salir";
            btnSalir.Location  = new System.Drawing.Point(258, 228);
            btnSalir.Size      = new System.Drawing.Size(220, 42);
            btnSalir.Click    += new System.EventHandler(this.btnSalir_Click);

            // ── Estado ──
            lblEstado          = new System.Windows.Forms.Label();
            lblEstado.AutoSize = false;
            lblEstado.Font     = UITheme.FntSm;
            lblEstado.ForeColor= UITheme.TextDim;
            lblEstado.Location = new System.Drawing.Point(22, 278);
            lblEstado.Size     = new System.Drawing.Size(456, 18);
            lblEstado.Text     = "Inicializando SDK...";

            // ── Form ──
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(500, 306);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Comercial Masivo — Conectar empresa";
            this.Controls.Add(pnlHeader);
            this.Controls.Add(grpEmpresa);
            this.Controls.Add(btnAbrir);
            this.Controls.Add(btnSalir);
            this.Controls.Add(lblEstado);
        }
    }
}
