namespace CargaComerMasivo
{
    partial class FrmMenu
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel  pnlHeader;
        private System.Windows.Forms.Label  lblTitulo;
        private System.Windows.Forms.Label  lblSubtitulo;
        private System.Windows.Forms.Panel  grpDocumentos;
        private System.Windows.Forms.Button btnDocumentos;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Panel  grpCatalogos;
        private System.Windows.Forms.Button btnAgentes;
        private System.Windows.Forms.Button btnAlmacenes;
        private System.Windows.Forms.Button btnSalir;

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
            var lblIcon  = new System.Windows.Forms.Label();
            var lblBadge = new System.Windows.Forms.Label();
            var pnlLine  = new System.Windows.Forms.Panel();

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

            lblBadge.Text      = " v2.0 ";
            lblBadge.Font      = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            lblBadge.ForeColor = UITheme.BgHeader;
            lblBadge.BackColor = UITheme.Accent;
            lblBadge.AutoSize  = true;
            lblBadge.Location  = new System.Drawing.Point(232, 18);

            lblSubtitulo.Text      = "CONTPAQi Comercial Premium";
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

            // ── Bienvenida ──
            var lblWelcome = new System.Windows.Forms.Label();
            lblWelcome.Text      = "Selecciona una opción para continuar";
            lblWelcome.Font      = UITheme.FntSm;
            lblWelcome.ForeColor = UITheme.TextMuted;
            lblWelcome.AutoSize  = true;
            lblWelcome.Location  = new System.Drawing.Point(22, 84);

            // ── Sección Documentos ──
            grpDocumentos   = new System.Windows.Forms.Panel();
            btnDocumentos   = new System.Windows.Forms.Button();
            btnCancelar     = new System.Windows.Forms.Button();
            var lblDocTitle = new System.Windows.Forms.Label();
            var pnlDocLine  = new System.Windows.Forms.Panel();

            grpDocumentos.BackColor = UITheme.BgCard;
            grpDocumentos.Location  = new System.Drawing.Point(22, 104);
            grpDocumentos.Size      = new System.Drawing.Size(356, 156);

            pnlDocLine.BackColor = UITheme.Accent;
            pnlDocLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlDocLine.Height    = 2;

            lblDocTitle.Text      = "DOCUMENTOS";
            lblDocTitle.Font      = UITheme.FntSection;
            lblDocTitle.ForeColor = UITheme.TextDim;
            lblDocTitle.BackColor = System.Drawing.Color.Transparent;
            lblDocTitle.AutoSize  = true;
            lblDocTitle.Location  = new System.Drawing.Point(12, 8);

            UITheme.StyleBtn(btnDocumentos, UITheme.Success, large: true);
            btnDocumentos.Text     = "▶   Carga Masiva de Documentos";
            btnDocumentos.Location = new System.Drawing.Point(12, 28);
            btnDocumentos.Size     = new System.Drawing.Size(332, 50);
            btnDocumentos.Click   += new System.EventHandler(this.btnDocumentos_Click);

            UITheme.StyleBtn(btnCancelar, UITheme.Danger, large: true);
            btnCancelar.Text     = "✕   Cancelación de Documentos";
            btnCancelar.Location = new System.Drawing.Point(12, 88);
            btnCancelar.Size     = new System.Drawing.Size(332, 50);
            btnCancelar.Click   += new System.EventHandler(this.btnCancelar_Click);

            grpDocumentos.Controls.Add(lblDocTitle);
            grpDocumentos.Controls.Add(btnDocumentos);
            grpDocumentos.Controls.Add(btnCancelar);
            grpDocumentos.Controls.Add(pnlDocLine);

            // ── Sección Catálogos ──
            grpCatalogos    = new System.Windows.Forms.Panel();
            btnAgentes      = new System.Windows.Forms.Button();
            btnAlmacenes    = new System.Windows.Forms.Button();
            var lblCatTitle = new System.Windows.Forms.Label();
            var pnlCatLine  = new System.Windows.Forms.Panel();

            grpCatalogos.BackColor = UITheme.BgCard;
            grpCatalogos.Location  = new System.Drawing.Point(22, 272);
            grpCatalogos.Size      = new System.Drawing.Size(356, 156);

            pnlCatLine.BackColor = UITheme.AccentDim;
            pnlCatLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlCatLine.Height    = 2;

            lblCatTitle.Text      = "CATÁLOGOS";
            lblCatTitle.Font      = UITheme.FntSection;
            lblCatTitle.ForeColor = UITheme.TextDim;
            lblCatTitle.BackColor = System.Drawing.Color.Transparent;
            lblCatTitle.AutoSize  = true;
            lblCatTitle.Location  = new System.Drawing.Point(12, 8);

            UITheme.StyleBtn(btnAgentes, UITheme.Neutral, large: true);
            btnAgentes.ForeColor = UITheme.TextDim;
            btnAgentes.Text      = "    Agentes de Venta";
            btnAgentes.Location  = new System.Drawing.Point(12, 28);
            btnAgentes.Size      = new System.Drawing.Size(332, 50);
            btnAgentes.Click    += new System.EventHandler(this.btnAgentes_Click);

            UITheme.StyleBtn(btnAlmacenes, UITheme.Neutral, large: true);
            btnAlmacenes.ForeColor = UITheme.TextDim;
            btnAlmacenes.Text      = "    Almacenes";
            btnAlmacenes.Location  = new System.Drawing.Point(12, 88);
            btnAlmacenes.Size      = new System.Drawing.Size(332, 50);
            btnAlmacenes.Click    += new System.EventHandler(this.btnAlmacenes_Click);

            grpCatalogos.Controls.Add(lblCatTitle);
            grpCatalogos.Controls.Add(btnAgentes);
            grpCatalogos.Controls.Add(btnAlmacenes);
            grpCatalogos.Controls.Add(pnlCatLine);

            // ── Salir ──
            btnSalir = new System.Windows.Forms.Button();
            UITheme.StyleBtn(btnSalir, UITheme.Neutral);
            btnSalir.ForeColor = UITheme.TextMuted;
            btnSalir.FlatAppearance.BorderColor = UITheme.Border;
            btnSalir.FlatAppearance.BorderSize  = 1;
            btnSalir.Text       = "Cerrar empresa y salir";
            btnSalir.Location   = new System.Drawing.Point(22, 440);
            btnSalir.Size       = new System.Drawing.Size(356, 38);
            btnSalir.Click     += new System.EventHandler(this.btnSalir_Click);

            // ── Footer ──
            var lblFooter = new System.Windows.Forms.Label();
            lblFooter.Text      = "SDK CONTPAQi Comercial Premium  ·  2025";
            lblFooter.Font      = UITheme.FntSm;
            lblFooter.ForeColor = UITheme.TextMuted;
            lblFooter.AutoSize  = true;
            lblFooter.Location  = new System.Drawing.Point(22, 488);

            // ── Form ──
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(400, 510);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Comercial Masivo — Menú Principal";
            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(grpDocumentos);
            this.Controls.Add(grpCatalogos);
            this.Controls.Add(btnSalir);
            this.Controls.Add(lblFooter);
        }
    }
}
