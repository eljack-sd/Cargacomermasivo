using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    /// <summary>
    /// Formulario de PRUEBA — extrae productos y clientes directamente del SDK
    /// y los muestra en dos grillas para verificar que fObtenNumProductos /
    /// fLeeDatoProductoNum / fObtenNumCtesProv / fLeeDatoCtesProvNum funcionan.
    /// </summary>
    internal class FrmPruebaCatalogo : Form
    {
        private TabControl     tabs;
        private TabPage        tabProductos;
        private TabPage        tabClientes;
        private DataGridView   gridProductos;
        private DataGridView   gridClientes;
        private Label          lblInfo;
        private Button         btnCargar;
        private Button         btnCerrar;
        private ProgressBar    pb;

        // ── Constructor ───────────────────────────────────────────────────────
        public FrmPruebaCatalogo()
        {
            ConstruirUI();
            UITheme.AplicarIconoForm(this);
        }

        // ── UI ────────────────────────────────────────────────────────────────
        private void ConstruirUI()
        {
            Text            = "Test — Catálogo SDK";
            Size            = new Size(780, 560);
            MinimumSize     = new Size(780, 560);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = UITheme.BgForm;
            ForeColor       = UITheme.TextMain;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            Font            = UITheme.FntDefault;

            // ── Info bar ──────────────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 46,
                BackColor = UITheme.BgHeader
            };
            pnlTop.Paint += (s, e) =>
            {
                using (var p = new System.Drawing.Pen(UITheme.BorderHi, 1))
                    e.Graphics.DrawLine(p, 0, pnlTop.Height - 1, pnlTop.Width, pnlTop.Height - 1);
            };

            lblInfo = new Label
            {
                Text      = "Presiona  Cargar  para extraer el catálogo del SDK.",
                ForeColor = UITheme.TextDim,
                Font      = UITheme.FntSm,
                AutoSize  = true,
                Location  = new Point(14, 14)
            };

            btnCargar = new Button { Text = "Cargar", Size = new Size(80, 26), Location = new Point(598, 10) };
            UITheme.StyleBtn(btnCargar, UITheme.Accent);
            btnCargar.Click += BtnCargar_Click;

            btnCerrar = new Button { Text = "Cerrar", Size = new Size(72, 26), Location = new Point(686, 10) };
            UITheme.StyleBtn(btnCerrar, UITheme.Neutral);
            btnCerrar.Click += (s, e) => Close();

            pnlTop.Controls.AddRange(new Control[] { lblInfo, btnCargar, btnCerrar });

            // ── Progress bar (delgada, marquee) ───────────────────────────────
            pb = new ProgressBar
            {
                Dock                  = DockStyle.Bottom,
                Height                = 3,
                Style                 = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 20,
                Visible               = false
            };

            // ── Tabs ──────────────────────────────────────────────────────────
            tabs = new TabControl
            {
                Dock      = DockStyle.Fill,
                BackColor = UITheme.BgCard,
                Font      = UITheme.FntSmBold
            };

            tabProductos = new TabPage("Productos  (0)")
            {
                BackColor = UITheme.BgCard,
                ForeColor = UITheme.TextDim
            };

            tabClientes = new TabPage("Clientes  (0)")
            {
                BackColor = UITheme.BgCard,
                ForeColor = UITheme.TextDim
            };

            // Grilla productos
            gridProductos = new DataGridView { Dock = DockStyle.Fill };
            UITheme.StyleGrid(gridProductos);
            gridProductos.Columns.Add("Codigo",      "Código");
            gridProductos.Columns.Add("Nombre",      "Nombre / Descripción");
            gridProductos.Columns.Add("Unidad",      "Unidad");
            gridProductos.Columns["Codigo"].Width = 120;
            gridProductos.Columns["Unidad"].Width = 90;
            tabProductos.Controls.Add(gridProductos);

            // Grilla clientes
            gridClientes = new DataGridView { Dock = DockStyle.Fill };
            UITheme.StyleGrid(gridClientes);
            gridClientes.Columns.Add("Codigo",  "Código");
            gridClientes.Columns.Add("Razon",   "Razón Social");
            gridClientes.Columns.Add("RFC",     "RFC");
            gridClientes.Columns["Codigo"].Width = 120;
            gridClientes.Columns["RFC"].Width    = 140;
            tabClientes.Controls.Add(gridClientes);

            tabs.TabPages.AddRange(new[] { tabProductos, tabClientes });

            Controls.AddRange(new Control[] { tabs, pnlTop, pb });
        }

        // ── CARGAR ────────────────────────────────────────────────────────────
        private void BtnCargar_Click(object sender, EventArgs e)
        {
            btnCargar.Enabled = false;
            pb.Visible        = true;
            lblInfo.Text      = "Extrayendo...";
            Application.DoEvents();

            try
            {
                CargarProductos();
                CargarClientes();
            }
            catch (Exception ex)
            {
                lblInfo.Text = "Error: " + ex.Message;
            }
            finally
            {
                pb.Visible        = false;
                btnCargar.Enabled = true;
            }
        }

        // ── PRODUCTOS ─────────────────────────────────────────────────────────
        private void CargarProductos()
        {
            gridProductos.Rows.Clear();

            int total = SdkComercial.fObtenNumProductos();
            if (total < 0)
            {
                // Puede que retorne -1 si el método no está disponible o la empresa no está abierta
                lblInfo.Text = $"fObtenNumProductos retornó {total}. Verifica que la empresa esté abierta.";
                tabProductos.Text = "Productos  (?)";
                return;
            }

            var sbCod  = new StringBuilder(500);
            var sbNom  = new StringBuilder(500);
            var sbUnd  = new StringBuilder(500);

            for (int i = 0; i < total; i++)
            {
                sbCod.Clear(); sbNom.Clear(); sbUnd.Clear();

                SdkComercial.fLeeDatoProductoNum(i, "CCODIGOPRODUCTO", sbCod, sbCod.Capacity);
                SdkComercial.fLeeDatoProductoNum(i, "CNOMBREPRODUCTO", sbNom, sbNom.Capacity);
                SdkComercial.fLeeDatoProductoNum(i, "CUNIDADVENTA",    sbUnd, sbUnd.Capacity);

                string cod = sbCod.ToString().Trim();
                string nom = sbNom.ToString().Trim();
                string und = sbUnd.ToString().Trim();

                // Si el nombre vino vacío intentar con otro campo
                if (string.IsNullOrEmpty(nom))
                {
                    sbNom.Clear();
                    SdkComercial.fLeeDatoProductoNum(i, "CDESCRIPCION", sbNom, sbNom.Capacity);
                    nom = sbNom.ToString().Trim();
                }

                gridProductos.Rows.Add(cod, nom, und);
            }

            tabProductos.Text = $"Productos  ({total})";
            ActualizarInfo();
        }

        // ── CLIENTES ──────────────────────────────────────────────────────────
        private void CargarClientes()
        {
            gridClientes.Rows.Clear();

            int total = SdkComercial.fObtenNumCtesProv();
            if (total < 0)
            {
                tabClientes.Text = "Clientes  (?)";
                ActualizarInfo();
                return;
            }

            var sbCod = new StringBuilder(500);
            var sbRaz = new StringBuilder(500);
            var sbRfc = new StringBuilder(500);

            for (int i = 0; i < total; i++)
            {
                sbCod.Clear(); sbRaz.Clear(); sbRfc.Clear();

                SdkComercial.fLeeDatoCtesProvNum(i, "CCODIGOCLIENTE",  sbCod, sbCod.Capacity);
                SdkComercial.fLeeDatoCtesProvNum(i, "CRAZONSOCIAL",    sbRaz, sbRaz.Capacity);
                SdkComercial.fLeeDatoCtesProvNum(i, "CRFC",            sbRfc, sbRfc.Capacity);

                gridClientes.Rows.Add(
                    sbCod.ToString().Trim(),
                    sbRaz.ToString().Trim(),
                    sbRfc.ToString().Trim());
            }

            tabClientes.Text = $"Clientes  ({total})";
            ActualizarInfo();
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private void ActualizarInfo()
        {
            int p = gridProductos.Rows.Count;
            int c = gridClientes.Rows.Count;
            lblInfo.Text = $"Productos: {p}   |   Clientes/Proveedores: {c}   —   {DateTime.Now:HH:mm:ss}";
        }
    }
}
