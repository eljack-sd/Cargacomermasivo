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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSub = new System.Windows.Forms.Label();
            this.pnlHLine = new System.Windows.Forms.Panel();
            this.grpSel = new System.Windows.Forms.Panel();
            this.lblSelTitle = new System.Windows.Forms.Label();
            this.lblSelLbl = new System.Windows.Forms.Label();
            this.cbAlmacen = new System.Windows.Forms.ComboBox();
            this.pnlSelLine = new System.Windows.Forms.Panel();
            this.grpDatos = new System.Windows.Forms.Panel();
            this.lblDatTitle = new System.Windows.Forms.Label();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.pnlDatLine = new System.Windows.Forms.Panel();
            this.grpExtra = new System.Windows.Forms.Panel();
            this.lblExtTitle = new System.Windows.Forms.Label();
            this.lblExt1 = new System.Windows.Forms.Label();
            this.txtTextoExt1 = new System.Windows.Forms.TextBox();
            this.lblExt2 = new System.Windows.Forms.Label();
            this.txtTextoExt2 = new System.Windows.Forms.TextBox();
            this.pnlExtLine = new System.Windows.Forms.Panel();
            this.pnlBot = new System.Windows.Forms.Panel();
            this.pnlBotLine = new System.Windows.Forms.Panel();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.grpSel.SuspendLayout();
            this.grpDatos.SuspendLayout();
            this.grpExtra.SuspendLayout();
            this.pnlBot.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(17)))), ((int)(((byte)(30)))));
            this.pnlHeader.Controls.Add(this.lblIcon);
            this.pnlHeader.Controls.Add(this.lblTitulo);
            this.pnlHeader.Controls.Add(this.lblSub);
            this.pnlHeader.Controls.Add(this.pnlHLine);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(560, 54);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblIcon
            // 
            this.lblIcon.AutoSize = true;
            this.lblIcon.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(143)))), ((int)(((byte)(245)))));
            this.lblIcon.Location = new System.Drawing.Point(14, 10);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(28, 25);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = "⬡";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            this.lblTitulo.Location = new System.Drawing.Point(42, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(93, 21);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Almacenes";
            // 
            // lblSub
            // 
            this.lblSub.AutoSize = true;
            this.lblSub.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(82)))), ((int)(((byte)(120)))));
            this.lblSub.Location = new System.Drawing.Point(44, 32);
            this.lblSub.Name = "lblSub";
            this.lblSub.Size = new System.Drawing.Size(154, 15);
            this.lblSub.TabIndex = 2;
            this.lblSub.Text = "Alta y edición de almacenes";
            // 
            // pnlHLine
            // 
            this.pnlHLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(106)))), ((int)(((byte)(220)))));
            this.pnlHLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHLine.Location = new System.Drawing.Point(0, 52);
            this.pnlHLine.Name = "pnlHLine";
            this.pnlHLine.Size = new System.Drawing.Size(560, 2);
            this.pnlHLine.TabIndex = 3;
            // 
            // grpSel
            // 
            this.grpSel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(58)))));
            this.grpSel.Controls.Add(this.lblSelTitle);
            this.grpSel.Controls.Add(this.lblSelLbl);
            this.grpSel.Controls.Add(this.cbAlmacen);
            this.grpSel.Controls.Add(this.pnlSelLine);
            this.grpSel.Location = new System.Drawing.Point(14, 62);
            this.grpSel.Name = "grpSel";
            this.grpSel.Size = new System.Drawing.Size(532, 56);
            this.grpSel.TabIndex = 2;
            // 
            // lblSelTitle
            // 
            this.lblSelTitle.AutoSize = true;
            this.lblSelTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSelTitle.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblSelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblSelTitle.Location = new System.Drawing.Point(10, 6);
            this.lblSelTitle.Name = "lblSelTitle";
            this.lblSelTitle.Size = new System.Drawing.Size(123, 12);
            this.lblSelTitle.TabIndex = 0;
            this.lblSelTitle.Text = "SELECCIÓN DE ALMACÉN";
            // 
            // lblSelLbl
            // 
            this.lblSelLbl.AutoSize = true;
            this.lblSelLbl.BackColor = System.Drawing.Color.Transparent;
            this.lblSelLbl.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSelLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblSelLbl.Location = new System.Drawing.Point(10, 30);
            this.lblSelLbl.Name = "lblSelLbl";
            this.lblSelLbl.Size = new System.Drawing.Size(57, 15);
            this.lblSelLbl.TabIndex = 1;
            this.lblSelLbl.Text = "Almacén:";
            // 
            // cbAlmacen
            // 
            this.cbAlmacen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAlmacen.Location = new System.Drawing.Point(72, 26);
            this.cbAlmacen.Name = "cbAlmacen";
            this.cbAlmacen.Size = new System.Drawing.Size(450, 21);
            this.cbAlmacen.TabIndex = 2;
            this.cbAlmacen.SelectedIndexChanged += new System.EventHandler(this.cbAlmacen_SelectedIndexChanged);
            // 
            // pnlSelLine
            // 
            this.pnlSelLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(143)))), ((int)(((byte)(245)))));
            this.pnlSelLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSelLine.Location = new System.Drawing.Point(0, 0);
            this.pnlSelLine.Name = "pnlSelLine";
            this.pnlSelLine.Size = new System.Drawing.Size(532, 2);
            this.pnlSelLine.TabIndex = 3;
            // 
            // grpDatos
            // 
            this.grpDatos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(58)))));
            this.grpDatos.Controls.Add(this.lblDatTitle);
            this.grpDatos.Controls.Add(this.lblCodigo);
            this.grpDatos.Controls.Add(this.txtCodigo);
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.pnlDatLine);
            this.grpDatos.Location = new System.Drawing.Point(14, 126);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(532, 56);
            this.grpDatos.TabIndex = 3;
            // 
            // lblDatTitle
            // 
            this.lblDatTitle.AutoSize = true;
            this.lblDatTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblDatTitle.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblDatTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblDatTitle.Location = new System.Drawing.Point(10, 6);
            this.lblDatTitle.Name = "lblDatTitle";
            this.lblDatTitle.Size = new System.Drawing.Size(109, 12);
            this.lblDatTitle.TabIndex = 0;
            this.lblDatTitle.Text = "DATOS DEL ALMACÉN";
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.BackColor = System.Drawing.Color.Transparent;
            this.lblCodigo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCodigo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblCodigo.Location = new System.Drawing.Point(10, 30);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(49, 15);
            this.lblCodigo.TabIndex = 1;
            this.lblCodigo.Text = "Código:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(62, 26);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(100, 20);
            this.txtCodigo.TabIndex = 2;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.BackColor = System.Drawing.Color.Transparent;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblNombre.Location = new System.Drawing.Point(178, 30);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(54, 15);
            this.lblNombre.TabIndex = 3;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(232, 26);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(290, 20);
            this.txtNombre.TabIndex = 4;
            // 
            // pnlDatLine
            // 
            this.pnlDatLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(143)))), ((int)(((byte)(245)))));
            this.pnlDatLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDatLine.Location = new System.Drawing.Point(0, 0);
            this.pnlDatLine.Name = "pnlDatLine";
            this.pnlDatLine.Size = new System.Drawing.Size(532, 2);
            this.pnlDatLine.TabIndex = 5;
            // 
            // grpExtra
            // 
            this.grpExtra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(58)))));
            this.grpExtra.Controls.Add(this.lblExtTitle);
            this.grpExtra.Controls.Add(this.lblExt1);
            this.grpExtra.Controls.Add(this.txtTextoExt1);
            this.grpExtra.Controls.Add(this.lblExt2);
            this.grpExtra.Controls.Add(this.txtTextoExt2);
            this.grpExtra.Controls.Add(this.pnlExtLine);
            this.grpExtra.Location = new System.Drawing.Point(14, 190);
            this.grpExtra.Name = "grpExtra";
            this.grpExtra.Size = new System.Drawing.Size(532, 90);
            this.grpExtra.TabIndex = 4;
            // 
            // lblExtTitle
            // 
            this.lblExtTitle.AutoSize = true;
            this.lblExtTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblExtTitle.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblExtTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblExtTitle.Location = new System.Drawing.Point(10, 6);
            this.lblExtTitle.Name = "lblExtTitle";
            this.lblExtTitle.Size = new System.Drawing.Size(135, 12);
            this.lblExtTitle.TabIndex = 0;
            this.lblExtTitle.Text = "INFORMACIÓN ADICIONAL";
            // 
            // lblExt1
            // 
            this.lblExt1.AutoSize = true;
            this.lblExt1.BackColor = System.Drawing.Color.Transparent;
            this.lblExt1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblExt1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblExt1.Location = new System.Drawing.Point(10, 30);
            this.lblExt1.Name = "lblExt1";
            this.lblExt1.Size = new System.Drawing.Size(81, 15);
            this.lblExt1.TabIndex = 1;
            this.lblExt1.Text = "Texto Extra 01:";
            // 
            // txtTextoExt1
            // 
            this.txtTextoExt1.Location = new System.Drawing.Point(110, 26);
            this.txtTextoExt1.Name = "txtTextoExt1";
            this.txtTextoExt1.Size = new System.Drawing.Size(412, 20);
            this.txtTextoExt1.TabIndex = 2;
            // 
            // lblExt2
            // 
            this.lblExt2.AutoSize = true;
            this.lblExt2.BackColor = System.Drawing.Color.Transparent;
            this.lblExt2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblExt2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblExt2.Location = new System.Drawing.Point(10, 64);
            this.lblExt2.Name = "lblExt2";
            this.lblExt2.Size = new System.Drawing.Size(81, 15);
            this.lblExt2.TabIndex = 3;
            this.lblExt2.Text = "Texto Extra 02:";
            // 
            // txtTextoExt2
            // 
            this.txtTextoExt2.Location = new System.Drawing.Point(110, 60);
            this.txtTextoExt2.Name = "txtTextoExt2";
            this.txtTextoExt2.Size = new System.Drawing.Size(412, 20);
            this.txtTextoExt2.TabIndex = 4;
            // 
            // pnlExtLine
            // 
            this.pnlExtLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(143)))), ((int)(((byte)(245)))));
            this.pnlExtLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlExtLine.Location = new System.Drawing.Point(0, 0);
            this.pnlExtLine.Name = "pnlExtLine";
            this.pnlExtLine.Size = new System.Drawing.Size(532, 2);
            this.pnlExtLine.TabIndex = 5;
            // 
            // pnlBot
            // 
            this.pnlBot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(17)))), ((int)(((byte)(30)))));
            this.pnlBot.Controls.Add(this.pnlBotLine);
            this.pnlBot.Controls.Add(this.lblEstado);
            this.pnlBot.Controls.Add(this.btnNuevo);
            this.pnlBot.Controls.Add(this.btnGuardar);
            this.pnlBot.Controls.Add(this.btnEliminar);
            this.pnlBot.Controls.Add(this.btnSalir);
            this.pnlBot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBot.Location = new System.Drawing.Point(0, 290);
            this.pnlBot.Name = "pnlBot";
            this.pnlBot.Size = new System.Drawing.Size(560, 70);
            this.pnlBot.TabIndex = 1;
            // 
            // pnlBotLine
            // 
            this.pnlBotLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(106)))), ((int)(((byte)(220)))));
            this.pnlBotLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBotLine.Location = new System.Drawing.Point(0, 0);
            this.pnlBotLine.Name = "pnlBotLine";
            this.pnlBotLine.Size = new System.Drawing.Size(560, 2);
            this.pnlBotLine.TabIndex = 0;
            // 
            // lblEstado
            // 
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(136)))), ((int)(((byte)(176)))));
            this.lblEstado.Location = new System.Drawing.Point(14, 10);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(532, 16);
            this.lblEstado.TabIndex = 1;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(14, 30);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(110, 34);
            this.btnNuevo.TabIndex = 2;
            this.btnNuevo.Text = "+ Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(132, 30);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(120, 34);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "✔  Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(260, 30);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(120, 34);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "✕  Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(82)))), ((int)(((byte)(120)))));
            this.btnSalir.Location = new System.Drawing.Point(406, 30);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(140, 34);
            this.btnSalir.TabIndex = 5;
            this.btnSalir.Text = "Cerrar";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FrmAlmacenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(26)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(560, 360);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlBot);
            this.Controls.Add(this.grpSel);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.grpExtra);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmAlmacenes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Comercial Masivo — Almacenes";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpSel.ResumeLayout(false);
            this.grpSel.PerformLayout();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.grpExtra.ResumeLayout(false);
            this.grpExtra.PerformLayout();
            this.pnlBot.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Panel pnlHLine;
        private System.Windows.Forms.Panel grpSel;
        private System.Windows.Forms.Label lblSelTitle;
        private System.Windows.Forms.Label lblSelLbl;
        private System.Windows.Forms.Panel pnlSelLine;
        private System.Windows.Forms.Panel grpDatos;
        private System.Windows.Forms.Label lblDatTitle;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Panel pnlDatLine;
        private System.Windows.Forms.Panel grpExtra;
        private System.Windows.Forms.Label lblExtTitle;
        private System.Windows.Forms.Label lblExt1;
        private System.Windows.Forms.Label lblExt2;
        private System.Windows.Forms.Panel pnlExtLine;
        private System.Windows.Forms.Panel pnlBot;
        private System.Windows.Forms.Panel pnlBotLine;
    }
}
