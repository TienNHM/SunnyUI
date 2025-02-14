/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * If you use this code, please retain this note.
 ******************************************************************************
 * File Name: UIValve.cs
 * File Description: Valve
 * Current Version: V3.1
 * Creation Date: 2021-08-08
 *
 * 2021-08-07: V3.0.5 Added valve control
 * 2021-08-08: V3.0.5 Completed valves in four directions
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Active")]
    [DefaultEvent("ActiveChanged")]
    public sealed class UIValve : Control, IZoomScale
    {
        public UIValve()
        {
            SetStyleFlags();
            Width = Height = 60;
            rectColor = Color.Silver;
            fillColor = Color.White;
            valveColor = UIColor.Blue;
            Version = UIGlobal.Version;
            ZoomScaleDisabled = true;
        }

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Control position in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set control scaling ratio
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public void SetZoomScale(float scale)
        {

        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Active = !Active;
        }

        /// <summary>
        /// Version
        /// </summary>
        public string Version
        {
            get;
        }

        private bool active;

        [DefaultValue(false), Description("Is scrolling"), Category("SunnyUI")]
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    active = value;
                    ActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ActiveChanged;

        private void SetStyleFlags(bool supportTransparent = true, bool selectable = true, bool resizeRedraw = false)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            if (supportTransparent) SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            if (selectable) SetStyle(ControlStyles.Selectable, true);
            if (resizeRedraw) SetStyle(ControlStyles.ResizeRedraw, true);
            base.DoubleBuffered = true;
            UpdateStyles();
        }

        public enum UIValveDirection
        {
            Left,
            Top,
            Right,
            Bottom
        }

        private UIValveDirection direction = UIValveDirection.Left;
        [DefaultValue(UIValveDirection.Left), Description("Valve direction"), Category("SunnyUI")]
        public UIValveDirection Direction
        {
            get => direction;
            set
            {
                direction = value;
                Invalidate();
            }
        }

        private Color rectColor;
        private Color fillColor;
        private Color valveColor;

        /// <summary>
        /// Valve color
        /// </summary>
        [Description("Valve color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ValveColor
        {
            get => valveColor;
            set
            {
                valveColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color RectColor
        {
            get => rectColor;
            set
            {
                rectColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color FillColor
        {
            get => fillColor;
            set
            {
                fillColor = value;
                Invalidate();
            }
        }

        [Description("Pipe size"), Category("SunnyUI")]
        [DefaultValue(20)]
        public int PipeSize
        {
            get => pipeSize;
            set
            {
                pipeSize = value;
                Invalidate();
            }
        }

        int pipeSize = 20;

        /// <summary>
        /// Override paint
        /// </summary>
        /// <param name="e">Paint parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int w = pipeSize / 2;
            Rectangle rect;
            Color[] colors;
            Point pt1, pt2, pt3, pt4;

            switch (direction)
            {
                case UIValveDirection.Left:
                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(w, 0), rectColor, fillColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, w, Height * 2));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(Width - w * 2 - 8, -5, w, Height + 50), new Rectangle(0, 5, w, Height + 20), GraphicsUnit.Pixel);
                    }

                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(w, 0), fillColor, rectColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, w, Height * 2));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(Width - w - 8, -5, w, Height + 50), new Rectangle(0, 5, w, Height + 20), GraphicsUnit.Pixel);
                    }

                    e.Graphics.DrawRectangle(RectColor, new Rectangle(Width - pipeSize - 8, 0, pipeSize - 1, Height - 1));

                    rect = new Rectangle(Width - pipeSize - 8 - 2, 4, pipeSize + 4, 6);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width - pipeSize - 8 - 2, Height - 4 - 6, pipeSize + 4, 6);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width - pipeSize - 8 - 14, Height / 2 - 2, 14, 4);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width - pipeSize - 8 - 14 - 10, Height / 2 - 14, 10, 27);
                    e.Graphics.FillRectangle(valveColor, rect);

                    colors = Color.White.GradientColors(valveColor, 14);
                    rect = new Rectangle(Width - pipeSize - 8 - 14 - 10, Height / 2 - 14 + 4, 10, 4);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(Width - pipeSize - 8 - 14 - 10, Height / 2 - 14 + 12, 10, 4);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(Width - pipeSize - 8 - 14 - 10, Height / 2 - 14 + 20, 10, 4);
                    e.Graphics.FillRectangle(colors[4], rect);

                    rect = new Rectangle(Width - pipeSize - 8 - 14 - 10, Height / 2 - 14, 10, 27);
                    e.Graphics.DrawRectangle(valveColor, rect);

                    pt1 = new Point(Width - pipeSize - 8 - 7, Height / 2 - 5);
                    pt2 = new Point(Width - pipeSize - 8 + 2, Height / 2 - 5 - 5);
                    pt3 = new Point(Width - pipeSize - 8 + 2, Height / 2 + 4 + 5);
                    pt4 = new Point(Width - pipeSize - 8 - 7, Height / 2 + 4);
                    e.Graphics.FillPolygon(rectColor, new PointF[] { pt1, pt2, pt3, pt4, pt1 });

                    break;
                case UIValveDirection.Bottom:
                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, w), rectColor, fillColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, Width * 2, w));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(-5, 8, Width + 50, w), new Rectangle(5, 0, Width + 20, w), GraphicsUnit.Pixel);
                    }

                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, w), fillColor, rectColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, Width * 2, w));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(-5, w + 8, Width + 50, w), new Rectangle(5, 0, Width + 20, w), GraphicsUnit.Pixel);
                    }

                    e.Graphics.DrawRectangle(RectColor, new Rectangle(0, 8, Width - 1, pipeSize - 1));

                    rect = new Rectangle(4, 8 - 2, 6, pipeSize + 4);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width - 4 - 6, 8 - 2, 6, pipeSize + 4);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width / 2 - 2, pipeSize + 8, 4, 14);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width / 2 - 14, pipeSize + 8 + 10 + 4, 27, 10);
                    e.Graphics.FillRectangle(valveColor, rect);

                    colors = Color.White.GradientColors(valveColor, 14);
                    rect = new Rectangle(Width / 2 - 14 + 4, pipeSize + 8 + 10 + 4, 4, 10);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(Width / 2 - 14 + 12, pipeSize + 8 + 10 + 4, 4, 10);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(Width / 2 - 14 + 20, pipeSize + 8 + 10 + 4, 4, 10);
                    e.Graphics.FillRectangle(colors[4], rect);

                    rect = new Rectangle(Width / 2 - 14, pipeSize + 8 + 10 + 4, 27, 10);
                    e.Graphics.DrawRectangle(valveColor, rect);

                    pt1 = new Point(Width / 2 - 5, pipeSize + 8 + 7);
                    pt2 = new Point(Width / 2 - 5 - 5, pipeSize + 8 - 2);
                    pt3 = new Point(Width / 2 + 4 + 5, pipeSize + 8 - 2);
                    pt4 = new Point(Width / 2 + 4, pipeSize + 8 + 7);
                    e.Graphics.FillPolygon(rectColor, new PointF[] { pt1, pt2, pt3, pt4, pt1 });
                    break;
                case UIValveDirection.Right:
                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(w, 0), rectColor, fillColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, w, Height * 2));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(8, -5, w, Height + 50), new Rectangle(0, 5, w, Height + 20), GraphicsUnit.Pixel);
                    }

                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(w, 0), fillColor, rectColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, w, Height * 2));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(w + 8, -5, w, Height + 50), new Rectangle(0, 5, w, Height + 20), GraphicsUnit.Pixel);
                    }

                    e.Graphics.DrawRectangle(RectColor, new Rectangle(8, 0, pipeSize - 1, Height - 1));

                    rect = new Rectangle(8 - 2, 4, pipeSize + 4, 6);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(8 - 2, Height - 4 - 6, pipeSize + 4, 6);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(pipeSize + 8, Height / 2 - 2, 14, 4);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(pipeSize + 8 + 10 + 4, Height / 2 - 14, 10, 27);
                    e.Graphics.FillRectangle(valveColor, rect);

                    colors = Color.White.GradientColors(valveColor, 14);
                    rect = new Rectangle(pipeSize + 8 + 10 + 4, Height / 2 - 14 + 4, 10, 4);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(pipeSize + 8 + 10 + 4, Height / 2 - 14 + 12, 10, 4);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(pipeSize + 8 + 10 + 4, Height / 2 - 14 + 20, 10, 4);
                    e.Graphics.FillRectangle(colors[4], rect);

                    rect = new Rectangle(pipeSize + 8 + 10 + 4, Height / 2 - 14, 10, 27);
                    e.Graphics.DrawRectangle(valveColor, rect);

                    pt1 = new Point(pipeSize + 8 + 7, Height / 2 - 5);
                    pt2 = new Point(pipeSize + 8 - 2, Height / 2 - 5 - 5);
                    pt3 = new Point(pipeSize + 8 - 2, Height / 2 + 4 + 5);
                    pt4 = new Point(pipeSize + 8 + 7, Height / 2 + 4);
                    e.Graphics.FillPolygon(rectColor, new PointF[] { pt1, pt2, pt3, pt4, pt1 });
                    break;
                case UIValveDirection.Top:
                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, w), rectColor, fillColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, Width * 2, w));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(-5, Height - w * 2 - 8, Width + 50, w), new Rectangle(5, 0, Width + 20, w), GraphicsUnit.Pixel);
                    }

                    using (Bitmap bmp = new Bitmap(Width, Height))
                    using (Graphics g1 = bmp.Graphics())
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, w), fillColor, rectColor))
                    {
                        g1.SetHighQuality();
                        g1.FillRectangle(lgb, new Rectangle(0, 0, Width * 2, w));
                        g1.SetDefaultQuality();
                        e.Graphics.DrawImage(bmp, new Rectangle(-5, Height - w - 8, Width + 50, w), new Rectangle(5, 0, Width + 20, w), GraphicsUnit.Pixel);
                    }

                    e.Graphics.DrawRectangle(RectColor, new Rectangle(0, Height - pipeSize - 8, Width - 1, pipeSize - 1));

                    rect = new Rectangle(4, Height - pipeSize - 8 - 2, 6, pipeSize + 4);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width - 4 - 6, Height - pipeSize - 8 - 2, 6, pipeSize + 4);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width / 2 - 2, Height - pipeSize - 8 - 14, 4, 14);
                    e.Graphics.FillRectangle(rectColor, rect);

                    rect = new Rectangle(Width / 2 - 14, Height - pipeSize - 8 - 14 - 10, 27, 10);
                    e.Graphics.FillRectangle(valveColor, rect);

                    colors = Color.White.GradientColors(valveColor, 14);
                    rect = new Rectangle(Width / 2 - 14 + 4, Height - pipeSize - 8 - 14 - 10, 4, 10);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(Width / 2 - 14 + 12, Height - pipeSize - 8 - 14 - 10, 4, 10);
                    e.Graphics.FillRectangle(colors[4], rect);
                    rect = new Rectangle(Width / 2 - 14 + 20, Height - pipeSize - 8 - 14 - 10, 4, 10);
                    e.Graphics.FillRectangle(colors[4], rect);

                    rect = new Rectangle(Width / 2 - 14, Height - pipeSize - 8 - 14 - 10, 27, 10);
                    e.Graphics.DrawRectangle(valveColor, rect);

                    pt1 = new Point(Width / 2 - 5, Height - pipeSize - 8 - 7);
                    pt2 = new Point(Width / 2 - 5 - 5, Height - pipeSize - 8 + 2);
                    pt3 = new Point(Width / 2 + 4 + 5, Height - pipeSize - 8 + 2);
                    pt4 = new Point(Width / 2 + 4, Height - pipeSize - 8 - 7);
                    e.Graphics.FillPolygon(rectColor, new PointF[] { pt1, pt2, pt3, pt4, pt1 });
                    break;
            }
        }
    }
}
