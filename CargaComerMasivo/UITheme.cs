using System;
using System.Drawing;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    /// <summary>
    /// Paleta ejecutiva dark-steel: azul-gris profundo, no negro puro.
    /// Inspirada en Bloomberg Terminal / Salesforce dark / enterprise dashboards.
    /// </summary>
    internal static class UITheme
    {
        // ─── Paleta ejecutiva ─────────────────────────────────────────────────
        // Fondo principal: azul-gris profundo (NO negro puro — tiene carácter)
        public static readonly Color BgForm    = Color.FromArgb( 22,  26,  44);  // #161A2C
        public static readonly Color BgCard    = Color.FromArgb( 30,  35,  58);  // #1E2339 — tarjetas
        public static readonly Color BgInput   = Color.FromArgb( 40,  46,  72);  // #282E48 — inputs
        public static readonly Color BgHeader  = Color.FromArgb( 14,  17,  30);  // #0E111E — header más oscuro
        public static readonly Color BgBottom  = Color.FromArgb( 14,  17,  30);  // barra inferior
        public static readonly Color BgGrid    = Color.FromArgb( 19,  23,  38);  // #131726 — grid
        public static readonly Color BgGridAlt = Color.FromArgb( 26,  31,  50);  // filas alternas

        // Acentos
        public static readonly Color Accent    = Color.FromArgb( 91, 143, 245);  // #5B8FF5 — azul suave
        public static readonly Color AccentDim = Color.FromArgb( 56, 106, 220);  // #386ADC — azul más oscuro
        public static readonly Color Success   = Color.FromArgb( 62, 196, 138);  // #3EC48A — verde muted
        public static readonly Color Danger    = Color.FromArgb(232,  96,  96);  // #E86060 — rojo coral
        public static readonly Color Warning   = Color.FromArgb(240, 167,  66);  // #F0A742 — ámbar

        // Neutros de UI
        public static readonly Color Neutral   = Color.FromArgb( 42,  50,  80);  // botón neutral
        public static readonly Color NeutralHv = Color.FromArgb( 52,  62,  98);  // neutral hover
        public static readonly Color Border    = Color.FromArgb( 52,  60,  96);  // #34406 — bordes
        public static readonly Color Divider   = Color.FromArgb( 36,  42,  68);  // divisores sutiles

        // Texto
        public static readonly Color TextMain  = Color.FromArgb(220, 228, 248);  // #DCE4F8 — casi blanco azulado
        public static readonly Color TextDim   = Color.FromArgb(122, 136, 176);  // #7A88B0 — gris-azul
        public static readonly Color TextMuted = Color.FromArgb( 72,  82, 120);  // #485278 — muy muted

        // ─── Fuentes ─────────────────────────────────────────────────────────
        public static readonly Font FntDefault = new Font("Segoe UI",  9.5F);
        public static readonly Font FntBold    = new Font("Segoe UI",  9.5F, FontStyle.Bold);
        public static readonly Font FntSm      = new Font("Segoe UI",  8.5F);
        public static readonly Font FntSmBold  = new Font("Segoe UI",  8.5F, FontStyle.Bold);
        public static readonly Font FntTitle   = new Font("Segoe UI", 15F,  FontStyle.Bold);
        public static readonly Font FntSub     = new Font("Segoe UI",  9F);
        public static readonly Font FntBtn     = new Font("Segoe UI",  9.5F, FontStyle.Bold);
        public static readonly Font FntBtnLg   = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        public static readonly Font FntSection = new Font("Segoe UI",  7.5F, FontStyle.Bold);
        public static readonly Font FntHuge    = new Font("Segoe UI", 12F,  FontStyle.Bold);

        // ─── Ayudantes ───────────────────────────────────────────────────────
        public static Color Brighten(Color c, int d = 24)
            => Color.FromArgb(Math.Min(255,c.R+d), Math.Min(255,c.G+d), Math.Min(255,c.B+d));

        public static void StyleBtn(Button b, Color bg, bool large = false)
        {
            b.BackColor = bg;
            b.ForeColor = TextMain;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize         = 0;
            b.FlatAppearance.MouseOverBackColor = Brighten(bg);
            b.FlatAppearance.MouseDownBackColor = bg;
            b.Font    = large ? FntBtnLg : FntBtn;
            b.Cursor  = Cursors.Hand;
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
            c.BackColor  = BgInput;
            c.ForeColor  = TextMain;
            c.FlatStyle  = FlatStyle.Flat;
            c.Font       = FntDefault;
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
            ch.Padding            = new Padding(7, 0, 0, 0);

            var dc = g.DefaultCellStyle;
            dc.BackColor          = BgGrid;
            dc.ForeColor          = TextMain;
            dc.SelectionBackColor = AccentDim;
            dc.SelectionForeColor = TextMain;
            dc.Font               = FntDefault;
            dc.Padding            = new Padding(6, 0, 6, 0);

            var ac = g.AlternatingRowsDefaultCellStyle;
            ac.BackColor          = BgGridAlt;
            ac.ForeColor          = TextMain;
            ac.SelectionBackColor = AccentDim;
            ac.SelectionForeColor = TextMain;

            g.RowHeadersVisible    = false;
            g.SelectionMode        = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill;
            g.AllowUserToAddRows   = false;
            g.ReadOnly             = true;
            g.RowTemplate.Height   = 27;
            g.Font                 = FntDefault;
        }
    }
}
