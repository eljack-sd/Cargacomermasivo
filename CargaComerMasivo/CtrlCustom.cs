using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace CargaComerMasivo
{
    // ─────────────────────────────────────────────────────────────────────────
    // RoundedButton
    // Botón owner-drawn con esquinas redondeadas suaves (anti-aliased),
    // hover state claro y press state oscuro — sin bordes cuadrados.
    // ─────────────────────────────────────────────────────────────────────────
    internal sealed class RoundedButton : Button
    {
        private int  _radius  = 7;
        private bool _hovered = false;
        private bool _pressed = false;

        public int Radius
        {
            get => _radius;
            set { _radius = Math.Max(0, value); Invalidate(); }
        }

        public RoundedButton()
        {
            SetStyle(
                ControlStyles.UserPaint             |
                ControlStyles.AllPaintingInWmPaint  |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor    = Cursors.Hand;
            Font      = UITheme.FntBtn;
            ForeColor = UITheme.TextMain;
            BackColor = UITheme.Neutral;
        }

        protected override void OnMouseEnter(EventArgs e)
        { base.OnMouseEnter(e); _hovered = true;  Invalidate(); }

        protected override void OnMouseLeave(EventArgs e)
        { base.OnMouseLeave(e); _hovered = false; _pressed = false; Invalidate(); }

        protected override void OnMouseDown(MouseEventArgs e)
        { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { _pressed = true; Invalidate(); } }

        protected override void OnMouseUp(MouseEventArgs e)
        { base.OnMouseUp(e); _pressed = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Pintar el fondo del padre para que las esquinas redondeadas
            // queden "transparentes" sobre cualquier fondo.
            Color parentBg = Parent?.BackColor ?? UITheme.BgForm;
            using (var bg = new SolidBrush(parentBg))
                g.FillRectangle(bg, 0, 0, Width, Height);

            // Elegir color de relleno según estado
            Color fill;
            if (!Enabled)      fill = Shift(BackColor, -38);
            else if (_pressed) fill = Shift(BackColor, -24);
            else if (_hovered) fill = Shift(BackColor, +24);
            else               fill = BackColor;

            // Dibujar rectángulo redondeado
            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (var path = BuildPath(rect, _radius))
            {
                using (var brush = new SolidBrush(fill))
                    g.FillPath(brush, path);

                // Borde sutil — da profundidad y distingue el botón del fondo
                Color borderColor = Color.FromArgb(
                    Math.Min(255, fill.R + 32),
                    Math.Min(255, fill.G + 32),
                    Math.Min(255, fill.B + 32));
                using (var pen = new System.Drawing.Pen(borderColor, 1f))
                    g.DrawPath(pen, path);
            }

            // Texto centrado
            Color fg = Enabled ? ForeColor : UITheme.TextMuted;
            using (var sf = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags   = StringFormatFlags.NoWrap
            })
            using (var tb = new SolidBrush(fg))
                g.DrawString(Text, Font, tb,
                    new RectangleF(2, 1, Width - 4, Height - 2), sf);
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        internal static GraphicsPath BuildPath(RectangleF r, int rad)
        {
            var p = new GraphicsPath();
            float d = rad * 2f;
            if (d <= 0 || r.Width < d || r.Height < d)
            { p.AddRectangle(r); return p; }
            p.AddArc(r.X,           r.Y,           d, d, 180, 90);
            p.AddArc(r.Right  - d,  r.Y,           d, d, 270, 90);
            p.AddArc(r.Right  - d,  r.Bottom - d,  d, d, 0,   90);
            p.AddArc(r.X,           r.Bottom - d,  d, d, 90,  90);
            p.CloseFigure();
            return p;
        }

        private static Color Shift(Color c, int d) => Color.FromArgb(
            Math.Max(0, Math.Min(255, c.R + d)),
            Math.Max(0, Math.Min(255, c.G + d)),
            Math.Max(0, Math.Min(255, c.B + d)));
    }
}
