using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ElevatorControl
{
    public class GlassPanel : Panel
    {
        public int CornerRadius { get; set; } = 8;
        public Color GlassColor { get; set; } = Color.FromArgb(90, 180, 220, 255);
        public Color BorderColor { get; set; } = Color.FromArgb(120, 180, 200);

        public GlassPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do not paint default background to allow true transparency
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (Width <= 0 || Height <= 0) return;
                
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1));
                using var path = GetRoundedRect(rect, CornerRadius);
                using var glassBrush = new SolidBrush(GlassColor);
                g.FillPath(glassBrush, path);

                // Subtle glass highlight
                var highlightRect = new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height / 3));
                if (highlightRect.Width > 0 && highlightRect.Height > 0)
                {
                    using var highlightBrush = new LinearGradientBrush(highlightRect,
                        Color.FromArgb(Math.Min(255, GlassColor.A + 30), Color.White),
                        Color.FromArgb(0, Color.White),
                        LinearGradientMode.Vertical);
                    g.FillPath(highlightBrush, GetRoundedRect(highlightRect, CornerRadius));
                }

                using var borderPen = new Pen(BorderColor, 1.5f);
                g.DrawPath(borderPen, path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GlassPanel paint error: {ex.Message}");
            }
        }

        private static GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            // Ensure radius doesn't exceed rectangle dimensions
            int maxRadius = Math.Min(rect.Width / 2, rect.Height / 2);
            radius = Math.Min(radius, maxRadius);
            
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
