namespace CargaComerMasivo
{
    partial class FrmAlmacenes
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel    pnlHeader;
        private System.Windows.Forms.Label    lblTitulo;
        private System.Windows.Forms.ComboBox cbAlmacen;
        private System.Windows.Forms.TextBox  txtCodigo;
        private System.Windows.Forms.TextBox  txtNombre;
        private System.Windows.Forms.TextBox  txtTextoExt1;
        private System.Windows.Forms.TextBox  txtTextoExt2;
        private System.Windows.Forms.Label    lblEstado;
        private System.Windows.Forms.Button   btnNuevo;
        private System.Windows.Forms.Button   btnGuardar;
        private System.Windows.Forms.Button   btnEliminar;
        private System.Windows.Forms.Button   btnSalir;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.BackColor = UITheme.BgForm;
            this.ForeColor = UITheme.TextMain;

            // ── HEADER ───────────────────────────────────────────────────
            pnlHeader    = new System.Windows.Forms.Panel();
            lblTitulo    = new System.Windows.Forms.Label();
            var lblIcon  = new System.Windows.Forms.Label();
            var pnlHLine = new System.Windows.Forms.Panel();

            pnlHeader.BackColor = UITheme.BgHeader;
            pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height    = 54;

            lblIcon.Text      = "⬡";
            lblIcon.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblIcon.ForeColor = UITheme.Accent;
            lblIcon.AutoSize  = true;
            lblIcon.Location  = new System.Drawing.Point(14, 10);

            lblTitulo.Text      = "Almacenes";
            lblTitulo.Font      = UITheme.FntHuge;
            lblTitulo.ForeColor = UITheme.TextMain;
            lblTitulo.AutoSize  = true;
            lblTitulo.Location  = new System.Drawing.Point(42, 10);

            var lblSub = new System.Windows.Forms.Label();
            lblSub.Text      = "Alta y edición de almacenes";
            lblSub.Font      = UITheme.FntSm;
            lblSub.ForeColor = UITheme.TextMuted;
            lblSub.AutoSize  = true;
            lblSub.Location  = new System.Drawing.Point(44, 32);

            pnlHLine.BackColor = UITheme.AccentDim;
            pnlHLine.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlHLine.Height    = 2;

            pnlHeader.Controls.Add(lblIcon);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblSub);
            pnlHeader.Controls.Add(pnlHLine);

            // ── CARD: Selección ──────────────────────────────────────────
            var grpSel      = new System.Windows.Forms.Panel();
            cbAlmacen       = new System.Windows.Forms.ComboBox();
            var pnlSelLine  = new System.Windows.Forms.Panel();
            var lblSelTitle = new System.Windows.Forms.Label();
            var lblSelLbl   = new System.Windows.Forms.Label();

            grpSel.BackColor = UITheme.BgCard;
            grpSel.Location  = new System.Drawing.Point(14, 62);
            grpSel.Size      = new System.Drawing.Size(532, 56);

            pnlSelLine.BackColor = UITheme.Accent;
            pnlSelLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlSelLine.Height    = 2;

            lblSelTitle.Text      = "SELECCIÓN DE ALMACÉN";
            lblSelTitle.Font      = UITheme.FntSection;
            lblSelTitle.ForeColor = UITheme.TextDim;
            lblSelTitle.BackColor = System.Drawing.Color.Transparent;
            lblSelTitle.AutoSize  = true;
            lblSelTitle.Location  = new System.Drawing.Point(10, 6);

            lblSelLbl.Text      = "Almacén:";
            lblSelLbl.Font      = UITheme.FntSm;
            lblSelLbl.ForeColor = UITheme.TextDim;
            lblSelLbl.BackColor = System.Drawing.Color.Transparent;
            lblSelLbl.AutoSize  = true;
            lblSelLbl.Location  = new System.Drawing.Point(10, 30);

            UITheme.StyleCombo(cbAlmacen);
            cbAlmacen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAlmacen.Location      = new System.Drawing.Point(72, 26);
            cbAlmacen.Size          = new System.Drawing.Size(450, 26);
            cbAlmacen.SelectedIndexChanged += new System.EventHandler(this.cbAlmacen_SelectedIndexChanged);

            grpSel.Controls.Add(lblSelTitle);
            grpSel.Controls.Add(lblSelLbl);
            grpSel.Controls.Add(cbAlmacen);
            grpSel.Controls.Add(pnlSelLine);

            // ── CARD: Datos del Almacén ──────────────────────────────────
            var grpDatos    = new System.Windows.Forms.Panel();
            txtCodigo       = new System.Windows.Forms.TextBox();
            txtNombre       = new System.Windows.Forms.TextBox();
            var pnlDatLine  = new System.Windows.Forms.Panel();
            var lblDatTitle = new System.Windows.Forms.Label();
            var lblCodigo   = new System.Windows.Forms.Label();
            var lblNombre   = new System.Windows.Forms.Label();

            grpDatos.BackColor = UITheme.BgCard;
            grpDatos.Location  = new System.Drawing.Point(14, 126);
            grpDatos.Size      = new System.Drawing.Size(532, 56);

            pnlDatLine.BackColor = UITheme.Accent;
            pnlDatLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlDatLine.Height    = 2;

            lblDatTitle.Text      = "DATOS DEL ALMACÉN";
            lblDatTitle.Font      = UITheme.FntSection;
            lblDatTitle.ForeColor = UITheme.TextDim;
            lblDatTitle.BackColor = System.Drawing.Color.Transparent;
            lblDatTitle.AutoSize  = true;
            lblDatTitle.Location  = new System.Drawing.Point(10, 6);

            lblCodigo.Text      = "Código:";
            lblCodigo.Font      = UITheme.FntSm;
            lblCodigo.ForeColor = UITheme.TextDim;
            lblCodigo.BackColor = System.Drawing.Color.Transparent;
            lblCodigo.AutoSize  = true;
            lblCodigo.Location  = new System.Drawing.Point(10, 30);

            UITheme.StyleInput(txtCodigo);
            txtCodigo.Location = new System.Drawing.Point(62, 26);
            txtCodigo.Size     = new System.Drawing.Size(100, 26);

            lblNombre.Text      = "Nombre:";
            lblNombre.Font      = UITheme.FntSm;
            lblNombre.ForeColor = UITheme.TextDim;
            lblNombre.BackColor = System.Drawing.Color.Transparent;
            lblNombre.AutoSize  = true;
            lblNombre.Location  = new System.Drawing.Point(178, 30);

            UITheme.StyleInput(txtNombre);
            txtNombre.Location = new System.Drawing.Point(232, 26);
            txtNombre.Size     = new System.Drawing.Size(290, 26);

            grpDatos.Controls.Add(lblDatTitle);
            grpDatos.Controls.Add(lblCodigo);  grpDatos.Controls.Add(txtCodigo);
            grpDatos.Controls.Add(lblNombre);  grpDatos.Controls.Add(txtNombre);
            grpDatos.Controls.Add(pnlDatLine);

            // ── CARD: Información Adicional ──────────────────────────────
            var grpExtra     = new System.Windows.Forms.Panel();
            txtTextoExt1     = new System.Windows.Forms.TextBox();
            txtTextoExt2     = new System.Windows.Forms.TextBox();
            var pnlExtLine   = new System.Windows.Forms.Panel();
            var lblExtTitle  = new System.Windows.Forms.Label();
            var lblExt1      = new System.Windows.Forms.Label();
            var lblExt2      = new System.Windows.Forms.Label();

            grpExtra.BackColor = UITheme.BgCard;
            grpExtra.Location  = new System.Drawing.Point(14, 190);
            grpExtra.Size      = new System.Drawing.Size(532, 90);

            pnlExtLine.BackColor = UITheme.Accent;
            pnlExtLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlExtLine.Height    = 2;

            lblExtTitle.Text      = "INFORMACIÓN ADICIONAL";
            lblExtTitle.Font      = UITheme.FntSection;
            lblExtTitle.ForeColor = UITheme.TextDim;
            lblExtTitle.BackColor = System.Drawing.Color.Transparent;
            lblExtTitle.AutoSize  = true;
            lblExtTitle.Location  = new System.Drawing.Point(10, 6);

            lblExt1.Text      = "Texto Extra 01:";
            lblExt1.Font      = UITheme.FntSm;
            lblExt1.ForeColor = UITheme.TextDim;
            lblExt1.BackColor = System.Drawing.Color.Transparent;
            lblExt1.AutoSize  = true;
            lblExt1.Location  = new System.Drawing.Point(10, 30);

            UITheme.StyleInput(txtTextoExt1);
            txtTextoExt1.Location = new System.Drawing.Point(110, 26);
            txtTextoExt1.Size     = new System.Drawing.Size(412, 26);

            lblExt2.Text      = "Texto Extra 02:";
            lblExt2.Font      = UITheme.FntSm;
            lblExt2.ForeColor = UITheme.TextDim;
            lblExt2.BackColor = System.Drawing.Color.Transparent;
            lblExt2.AutoSize  = true;
            lblExt2.Location  = new System.Drawing.Point(10, 64);

            UITheme.StyleInput(txtTextoExt2);
            txtTextoExt2.Location = new System.Drawing.Point(110, 60);
            txtTextoExt2.Size     = new System.Drawing.Size(412, 26);

            grpExtra.Controls.Add(lblExtTitle);
            grpExtra.Controls.Add(lblExt1);  grpExtra.Controls.Add(txtTextoExt1);
            grpExtra.Controls.Add(lblExt2);  grpExtra.Controls.Add(txtTextoExt2);
            grpExtra.Controls.Add(pnlExtLine);

            // ── BARRA INFERIOR (Dock=Bottom) ─────────────────────────────
            var pnlBot     = new System.Windows.Forms.Panel();
            var pnlBotLine = new System.Windows.Forms.Panel();
            lblEstado      = new System.Windows.Forms.Label();
            btnNuevo       = new System.Windows.Forms.Button();
            btnGuardar     = new System.Windows.Forms.Button();
            btnEliminar    = new System.Windows.Forms.Button();
            btnSalir       = new System.Windows.Forms.Button();

            pnlBot.BackColor = UITheme.BgBottom;
            pnlBot.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlBot.Height    = 70;

            pnlBotLine.BackColor = UITheme.AccentDim;
            pnlBotLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlBotLine.Height    = 2;

            lblEstado.AutoSize  = false;
            lblEstado.Font      = UITheme.FntSm;
            lblEstado.ForeColor = UITheme.TextDim;
            lblEstado.Location  = new System.Drawing.Point(14, 10);
            lblEstado.Size      = new System.Drawing.Size(532, 16);
            lblEstado.Text      = "";

            UITheme.StyleBtn(btnNuevo, UITheme.Accent);
            btnNuevo.Text     = "+ Nuevo";
            btnNuevo.Location = new System.Drawing.Point(14, 30);
            btnNuevo.Size     = new System.Drawing.Size(110, 34);
            btnNuevo.Click   += new System.EventHandler(this.btnNuevo_Click);

            UITheme.StyleBtn(btnGuardar, UITheme.Success);
            btnGuardar.Text     = "✔  Guardar";
            btnGuardar.Location = new System.Drawing.Point(132, 30);
            btnGuardar.Size     = new System.Drawing.Size(120, 34);
            btnGuardar.Click   += new System.EventHandler(this.btnGuardar_Click);

            UITheme.StyleBtn(btnEliminar, UITheme.Danger);
            btnEliminar.Text     = "✕  Eliminar";
            btnEliminar.Location = new System.Drawing.Point(260, 30);
            btnEliminar.Size     = new System.Drawing.Size(120, 34);
            btnEliminar.Click   += new System.EventHandler(this.btnEliminar_Click);

            UITheme.StyleBtn(btnSalir, UITheme.Neutral);
            btnSalir.ForeColor = UITheme.TextMuted;
            btnSalir.Text      = "Cerrar";
            btnSalir.Location  = new System.Drawing.Point(406, 30);
            btnSalir.Size      = new System.Drawing.Size(140, 34);
            btnSalir.Click    += new System.EventHandler(this.btnSalir_Click);

            pnlBot.Controls.Add(pnlBotLine);
            pnlBot.Controls.Add(lblEstado);
            pnlBot.Controls.Add(btnNuevo);
            pnlBot.Controls.Add(btnGuardar);
            pnlBot.Controls.Add(btnEliminar);
            pnlBot.Controls.Add(btnSalir);

            // ── FORM ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(560, 360);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Comercial Masivo — Almacenes";

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBot);
            this.Controls.Add(grpSel);
            this.Controls.Add(grpDatos);
            this.Controls.Add(grpExtra);
        }
    }
}
