using System;
using System.Drawing;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    /// <summary>
    /// Paleta "Connector WinetPC" — negro profundo + plata elegante.
    /// Inspirada directamente en el logotipo: fondo negro, icono plata/gris claro.
    /// </summary>
    internal static class UITheme
    {
        // ─── Fondos (negros en escala) ────────────────────────────────────────
        public static readonly Color BgForm    = Color.FromArgb(  8,   8,   8);   // #080808 — base
        public static readonly Color BgCard    = Color.FromArgb( 13,  13,  13);   // #0D0D0D — tarjetas
        public static readonly Color BgCardAlt = Color.FromArgb( 17,  17,  17);   // #111111 — stat cards
        public static readonly Color BgInput   = Color.FromArgb( 22,  22,  22);   // #161616 — inputs
        public static readonly Color BgHeader  = Color.FromArgb(  4,   4,   4);   // #040404 — header/footer
        public static readonly Color BgBottom  = Color.FromArgb(  4,   4,   4);
        public static readonly Color BgGrid    = Color.FromArgb( 10,  10,  10);
        public static readonly Color BgGridAlt = Color.FromArgb( 15,  15,  15);

        // ─── Plata (acento principal — del logo) ──────────────────────────────
        public static readonly Color Accent    = Color.FromArgb(200, 200, 200);   // #C8C8C8 — plata logo
        public static readonly Color AccentDim = Color.FromArgb(130, 130, 130);   // #828282 — plata apagada
        public static readonly Color AccentHv  = Color.FromArgb(230, 230, 230);   // #E6E6E6 — hover

        // ─── Funcionales (muted — no rompen la paleta monocromática) ─────────
        public static readonly Color Success   = Color.FromArgb( 72, 168, 116);   // verde salvia
        public static readonly Color Danger    = Color.FromArgb(185,  68,  68);   // rojo coral muted
        public static readonly Color Warning   = Color.FromArgb(175, 135,  50);   // ámbar muted

        // ─── Neutrales ────────────────────────────────────────────────────────
        public static readonly Color Neutral   = Color.FromArgb( 24,  24,  24);   // botón neutro
        public static readonly Color NeutralHv = Color.FromArgb( 34,  34,  34);
        public static readonly Color Border    = Color.FromArgb( 28,  28,  28);   // bordes sutiles
        public static readonly Color Divider   = Color.FromArgb( 18,  18,  18);
        public static readonly Color BorderHi  = Color.FromArgb( 50,  50,  50);   // borde destacado

        // ─── Texto ────────────────────────────────────────────────────────────
        public static readonly Color TextMain  = Color.FromArgb(235, 235, 235);   // blanco suave
        public static readonly Color TextDim   = Color.FromArgb(140, 140, 140);   // gris medio
        public static readonly Color TextMuted = Color.FromArgb( 65,  65,  65);   // muy apagado

        // ─── Fuentes ─────────────────────────────────────────────────────────
        public static readonly Font FntDefault = new Font("Segoe UI",  9.5F);
        public static readonly Font FntBold    = new Font("Segoe UI",  9.5F, FontStyle.Bold);
        public static readonly Font FntSm      = new Font("Segoe UI",  8.5F);
        public static readonly Font FntSmBold  = new Font("Segoe UI",  8.5F, FontStyle.Bold);
        public static readonly Font FntTitle   = new Font("Segoe UI", 15F,  FontStyle.Bold);
        public static readonly Font FntSub     = new Font("Segoe UI",  9F);
        public static readonly Font FntBtn     = new Font("Segoe UI",  9.5F, FontStyle.Bold);
        public static readonly Font FntBtnLg   = new Font("Segoe UI", 10F,  FontStyle.Bold);
        public static readonly Font FntSection = new Font("Segoe UI",  7F,  FontStyle.Bold);
        public static readonly Font FntHuge    = new Font("Segoe UI", 24F,  FontStyle.Bold);
        public static readonly Font FntMed     = new Font("Segoe UI", 11F,  FontStyle.Bold);

        // ─── Icono de la aplicación ───────────────────────────────────────────
        private static System.Drawing.Icon _appIcon;
        public static System.Drawing.Icon AppIcon
        {
            get
            {
                if (_appIcon != null) return _appIcon;
                try
                {
                    var bmp     = Properties.Resources.icono;
                    var resized = new Bitmap(bmp, new Size(32, 32));
                    _appIcon    = System.Drawing.Icon.FromHandle(resized.GetHicon());
                }
                catch { }
                return _appIcon;
            }
        }

        public static void AplicarIconoForm(Form f)
        {
            try { if (AppIcon != null) f.Icon = AppIcon; }
            catch { }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────
        public static Color Brighten(Color c, int d = 20)
            => Color.FromArgb(
                Math.Min(255, c.R + d),
                Math.Min(255, c.G + d),
                Math.Min(255, c.B + d));

        /// <summary>
        /// Estiliza un botón. Si el fondo es claro (plata) usa texto oscuro automáticamente.
        /// </summary>
        public static void StyleBtn(Button b, Color bg, bool large = false)
        {
            b.BackColor = bg;
            // Fondo claro (plata) → texto negro elegante; fondo oscuro → texto claro
            b.ForeColor = bg.GetBrightness() > 0.45f
                ? Color.FromArgb(10, 10, 10)
                : TextMain;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize         = 0;
            b.FlatAppearance.MouseOverBackColor = Brighten(bg, 18);
            b.FlatAppearance.MouseDownBackColor = bg;
            b.Font   = large ? FntBtnLg : FntBtn;
            b.Cursor = Cursors.Hand;
        }

        public static void StyleRoundBtn(RoundedButton b, Color bg, bool large = false)
        {
            b.BackColor = bg;
            b.ForeColor = bg.GetBrightness() > 0.45f
                ? Color.FromArgb(10, 10, 10)
                : TextMain;
            b.Font   = large ? FntBtnLg : FntBtn;
            b.Cursor = Cursors.Hand;
        }

        public static void StyleInput(TextBox t)
        {
            t.BackColor   = BgInput;
            t.ForeColor   = TextMain;
            t.BorderStyle = BorderStyle.FixedSingle;
            t.Font        = FntDefault;
        }

        public static void StyleCombo(ComboBox c)
        {
            c.BackColor = BgInput;
            c.ForeColor = TextMain;
            c.FlatStyle = FlatStyle.Flat;
            c.Font      = FntDefault;
        }

        public static void StyleCheck(CheckBox c)
        {
            c.ForeColor = TextDim;
            c.BackColor = Color.Transparent;
            c.Font      = FntSm;
            c.Cursor    = Cursors.Hand;
        }

        public static void StyleRadio(RadioButton r)
        {
            r.ForeColor = TextDim;
            r.BackColor = Color.Transparent;
            r.Font      = FntSm;
            r.Cursor    = Cursors.Hand;
        }

        public static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor           = BgGrid;
            g.GridColor                 = Divider;
            g.BorderStyle               = BorderStyle.None;
            g.CellBorderStyle           = DataGridViewCellBorderStyle.SingleHorizontal;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle  = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeight       = 34;

            var ch = g.ColumnHeadersDefaultCellStyle;
            ch.BackColor          = BgHeader;
            ch.ForeColor          = TextDim;
            ch.Font               = FntSmBold;
            ch.SelectionBackColor = BgHeader;
            ch.Padding            = new Padding(8, 0, 0, 0);

            var dc = g.DefaultCellStyle;
            dc.BackColor          = BgGrid;
            dc.ForeColor          = TextMain;
            dc.SelectionBackColor = Color.FromArgb(38, 38, 38);
            dc.SelectionForeColor = TextMain;
            dc.Font               = FntDefault;
            dc.Padding            = new Padding(7, 0, 7, 0);

            var ac = g.AlternatingRowsDefaultCellStyle;
            ac.BackColor          = BgGridAlt;
            ac.ForeColor          = TextMain;
            ac.SelectionBackColor = Color.FromArgb(38, 38, 38);
            ac.SelectionForeColor = TextMain;

            g.RowHeadersVisible   = false;
            g.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AllowUserToAddRows  = false;
            g.ReadOnly            = true;
            g.RowTemplate.Height  = 28;
            g.Font                = FntDefault;
        }
    }
}
