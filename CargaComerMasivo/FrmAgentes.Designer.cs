namespace CargaComerMasivo
{
    partial class FrmAgentes
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel       pnlHeader;
        private System.Windows.Forms.Label       lblTitulo;
        private System.Windows.Forms.ComboBox    cbAgente;
        private System.Windows.Forms.TextBox     txtCodigo;
        private System.Windows.Forms.TextBox     txtNombre;
        private System.Windows.Forms.RadioButton rbVentas;
        private System.Windows.Forms.RadioButton rbVentaCobro;
        private System.Windows.Forms.RadioButton rbCobro;
        private System.Windows.Forms.TextBox     txtComVenta;
        private System.Windows.Forms.TextBox     txtComCobro;
        private System.Windows.Forms.TextBox     txtRuta;
        private System.Windows.Forms.TextBox     txtZona;
        private System.Windows.Forms.Label       lblEstado;
        private System.Windows.Forms.Button      btnNuevo;
        private System.Windows.Forms.Button      btnGuardar;
        private System.Windows.Forms.Button      btnEliminar;
        private System.Windows.Forms.Button      btnSalir;

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

            lblTitulo.Text      = "Agentes de Venta";
            lblTitulo.Font      = UITheme.FntHuge;
            lblTitulo.ForeColor = UITheme.TextMain;
            lblTitulo.AutoSize  = true;
            lblTitulo.Location  = new System.Drawing.Point(42, 10);

            var lblSub = new System.Windows.Forms.Label();
            lblSub.Text      = "Alta y edición de agentes";
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
            cbAgente        = new System.Windows.Forms.ComboBox();
            var pnlSelLine  = new System.Windows.Forms.Panel();
            var lblSelTitle = new System.Windows.Forms.Label();
            var lblSelLbl   = new System.Windows.Forms.Label();

            grpSel.BackColor = UITheme.BgCard;
            grpSel.Location  = new System.Drawing.Point(14, 62);
            grpSel.Size      = new System.Drawing.Size(532, 56);

            pnlSelLine.BackColor = UITheme.Accent;
            pnlSelLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlSelLine.Height    = 2;

            lblSelTitle.Text      = "SELECCIÓN DE AGENTE";
            lblSelTitle.Font      = UITheme.FntSection;
            lblSelTitle.ForeColor = UITheme.TextDim;
            lblSelTitle.BackColor = System.Drawing.Color.Transparent;
            lblSelTitle.AutoSize  = true;
            lblSelTitle.Location  = new System.Drawing.Point(10, 6);

            lblSelLbl.Text      = "Agente:";
            lblSelLbl.Font      = UITheme.FntSm;
            lblSelLbl.ForeColor = UITheme.TextDim;
            lblSelLbl.BackColor = System.Drawing.Color.Transparent;
            lblSelLbl.AutoSize  = true;
            lblSelLbl.Location  = new System.Drawing.Point(10, 30);

            UITheme.StyleCombo(cbAgente);
            cbAgente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAgente.Location      = new System.Drawing.Point(62, 26);
            cbAgente.Size          = new System.Drawing.Size(460, 26);
            cbAgente.SelectedIndexChanged += new System.EventHandler(this.cbAgente_SelectedIndexChanged);

            grpSel.Controls.Add(lblSelTitle);
            grpSel.Controls.Add(lblSelLbl);
            grpSel.Controls.Add(cbAgente);
            grpSel.Controls.Add(pnlSelLine);

            // ── CARD: Datos del Agente ───────────────────────────────────
            var grpDatos      = new System.Windows.Forms.Panel();
            txtCodigo         = new System.Windows.Forms.TextBox();
            txtNombre         = new System.Windows.Forms.TextBox();
            var pnlDatLine    = new System.Windows.Forms.Panel();
            var lblDatTitle   = new System.Windows.Forms.Label();
            var lblCodigo     = new System.Windows.Forms.Label();
            var lblNombre     = new System.Windows.Forms.Label();

            grpDatos.BackColor = UITheme.BgCard;
            grpDatos.Location  = new System.Drawing.Point(14, 126);
            grpDatos.Size      = new System.Drawing.Size(532, 90);

            pnlDatLine.BackColor = UITheme.Accent;
            pnlDatLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlDatLine.Height    = 2;

            lblDatTitle.Text      = "DATOS DEL AGENTE";
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
            lblNombre.Location  = new System.Drawing.Point(176, 30);

            UITheme.StyleInput(txtNombre);
            txtNombre.Location = new System.Drawing.Point(230, 26);
            txtNombre.Size     = new System.Drawing.Size(292, 26);

            lblCodigo.Location = new System.Drawing.Point(10, 62);
            lblNombre.Location = new System.Drawing.Point(176, 62);

            // reposicionar correctamente en fila
            lblCodigo.Location  = new System.Drawing.Point(10,  30);
            txtCodigo.Location  = new System.Drawing.Point(62,  26);
            lblNombre.Location  = new System.Drawing.Point(178, 30);
            txtNombre.Location  = new System.Drawing.Point(232, 26);

            // Fila 2: comisiones dentro del mismo card
            var lblComVentaLbl = new System.Windows.Forms.Label();
            var lblComCobroLbl = new System.Windows.Forms.Label();
            txtComVenta        = new System.Windows.Forms.TextBox();
            txtComCobro        = new System.Windows.Forms.TextBox();

            lblComVentaLbl.Text      = "Com. Venta %:";
            lblComVentaLbl.Font      = UITheme.FntSm;
            lblComVentaLbl.ForeColor = UITheme.TextDim;
            lblComVentaLbl.BackColor = System.Drawing.Color.Transparent;
            lblComVentaLbl.AutoSize  = true;
            lblComVentaLbl.Location  = new System.Drawing.Point(10, 64);

            UITheme.StyleInput(txtComVenta);
            txtComVenta.Location = new System.Drawing.Point(102, 60);
            txtComVenta.Size     = new System.Drawing.Size(80, 26);
            txtComVenta.Text     = "0.00";

            lblComCobroLbl.Text      = "Com. Cobro %:";
            lblComCobroLbl.Font      = UITheme.FntSm;
            lblComCobroLbl.ForeColor = UITheme.TextDim;
            lblComCobroLbl.BackColor = System.Drawing.Color.Transparent;
            lblComCobroLbl.AutoSize  = true;
            lblComCobroLbl.Location  = new System.Drawing.Point(200, 64);

            UITheme.StyleInput(txtComCobro);
            txtComCobro.Location = new System.Drawing.Point(296, 60);
            txtComCobro.Size     = new System.Drawing.Size(80, 26);
            txtComCobro.Text     = "0.00";

            grpDatos.Controls.Add(lblDatTitle);
            grpDatos.Controls.Add(lblCodigo);   grpDatos.Controls.Add(txtCodigo);
            grpDatos.Controls.Add(lblNombre);   grpDatos.Controls.Add(txtNombre);
            grpDatos.Controls.Add(lblComVentaLbl); grpDatos.Controls.Add(txtComVenta);
            grpDatos.Controls.Add(lblComCobroLbl); grpDatos.Controls.Add(txtComCobro);
            grpDatos.Controls.Add(pnlDatLine);

            // ── CARD: Tipo de Agente ─────────────────────────────────────
            var grpTipo      = new System.Windows.Forms.Panel();
            rbVentas         = new System.Windows.Forms.RadioButton();
            rbVentaCobro     = new System.Windows.Forms.RadioButton();
            rbCobro          = new System.Windows.Forms.RadioButton();
            var pnlTipoLine  = new System.Windows.Forms.Panel();
            var lblTipoTitle = new System.Windows.Forms.Label();

            grpTipo.BackColor = UITheme.BgCard;
            grpTipo.Location  = new System.Drawing.Point(14, 224);
            grpTipo.Size      = new System.Drawing.Size(532, 56);

            pnlTipoLine.BackColor = UITheme.Accent;
            pnlTipoLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlTipoLine.Height    = 2;

            lblTipoTitle.Text      = "TIPO DE AGENTE";
            lblTipoTitle.Font      = UITheme.FntSection;
            lblTipoTitle.ForeColor = UITheme.TextDim;
            lblTipoTitle.BackColor = System.Drawing.Color.Transparent;
            lblTipoTitle.AutoSize  = true;
            lblTipoTitle.Location  = new System.Drawing.Point(10, 6);

            UITheme.StyleRadio(rbVentas);
            rbVentas.Text     = "De Ventas";
            rbVentas.Checked  = true;
            rbVentas.Location = new System.Drawing.Point(14, 30);
            rbVentas.AutoSize = true;

            UITheme.StyleRadio(rbVentaCobro);
            rbVentaCobro.Text     = "De Venta / Cobro";
            rbVentaCobro.Location = new System.Drawing.Point(140, 30);
            rbVentaCobro.AutoSize = true;

            UITheme.StyleRadio(rbCobro);
            rbCobro.Text     = "De Cobro";
            rbCobro.Location = new System.Drawing.Point(300, 30);
            rbCobro.AutoSize = true;

            grpTipo.Controls.Add(lblTipoTitle);
            grpTipo.Controls.Add(rbVentas);
            grpTipo.Controls.Add(rbVentaCobro);
            grpTipo.Controls.Add(rbCobro);
            grpTipo.Controls.Add(pnlTipoLine);

            // ── CARD: Información Adicional ──────────────────────────────
            var grpExtra      = new System.Windows.Forms.Panel();
            txtRuta           = new System.Windows.Forms.TextBox();
            txtZona           = new System.Windows.Forms.TextBox();
            var pnlExtLine    = new System.Windows.Forms.Panel();
            var lblExtTitle   = new System.Windows.Forms.Label();
            var lblRuta       = new System.Windows.Forms.Label();
            var lblZona       = new System.Windows.Forms.Label();

            grpExtra.BackColor = UITheme.BgCard;
            grpExtra.Location  = new System.Drawing.Point(14, 288);
            grpExtra.Size      = new System.Drawing.Size(532, 56);

            pnlExtLine.BackColor = UITheme.Accent;
            pnlExtLine.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlExtLine.Height    = 2;

            lblExtTitle.Text      = "INFORMACIÓN ADICIONAL";
            lblExtTitle.Font      = UITheme.FntSection;
            lblExtTitle.ForeColor = UITheme.TextDim;
            lblExtTitle.BackColor = System.Drawing.Color.Transparent;
            lblExtTitle.AutoSize  = true;
            lblExtTitle.Location  = new System.Drawing.Point(10, 6);

            lblRuta.Text      = "Ruta:";
            lblRuta.Font      = UITheme.FntSm;
            lblRuta.ForeColor = UITheme.TextDim;
            lblRuta.BackColor = System.Drawing.Color.Transparent;
            lblRuta.AutoSize  = true;
            lblRuta.Location  = new System.Drawing.Point(10, 30);

            UITheme.StyleInput(txtRuta);
            txtRuta.Location = new System.Drawing.Point(46, 26);
            txtRuta.Size     = new System.Drawing.Size(190, 26);

            lblZona.Text      = "Zona:";
            lblZona.Font      = UITheme.FntSm;
            lblZona.ForeColor = UITheme.TextDim;
            lblZona.BackColor = System.Drawing.Color.Transparent;
            lblZona.AutoSize  = true;
            lblZona.Location  = new System.Drawing.Point(252, 30);

            UITheme.StyleInput(txtZona);
            txtZona.Location = new System.Drawing.Point(288, 26);
            txtZona.Size     = new System.Drawing.Size(234, 26);

            grpExtra.Controls.Add(lblExtTitle);
            grpExtra.Controls.Add(lblRuta); grpExtra.Controls.Add(txtRuta);
            grpExtra.Controls.Add(lblZona); grpExtra.Controls.Add(txtZona);
            grpExtra.Controls.Add(pnlExtLine);

            // ── BARRA INFERIOR (Dock=Bottom) ─────────────────────────────
            var pnlBot     = new System.Windows.Forms.Panel();
            var pnlBotLine = new System.Windows.Forms.Panel();
            var pnlBtns    = new System.Windows.Forms.Panel();
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
            lblEstado.Size      = new System.Drawing.Size(560, 16);
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
            this.ClientSize          = new System.Drawing.Size(560, 430);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Comercial Masivo — Agentes de Venta";

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBot);
            this.Controls.Add(grpSel);
            this.Controls.Add(grpDatos);
            this.Controls.Add(grpTipo);
            this.Controls.Add(grpExtra);
        }
    }
}
