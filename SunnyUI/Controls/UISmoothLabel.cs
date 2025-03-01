/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Class Library, Extension Class Library, Multi-page Development Framework.
 * CopyRight (C) 2012-2025 ShenYongHua.
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UISmoothLabel.cs
 * Description: Smooth text label with border
 * Current Version: V3.1
 * Creation Date: 2022-01-22
 *
 * 2022-01-22: V3.1.0 Added file description
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-11-16: V3.5.2 Refactored theme
******************************************************************************/


using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public sealed class UISmoothLabel : Label, IStyleInterface, IZoomScale, IFormTranslator
    {
        private PointF point;
        private SizeF drawSize;
        private Pen drawPen;
        private GraphicsPath drawPath;
        private SolidBrush forecolorBrush;

        public UISmoothLabel()
        {
            base.Font = UIStyles.Font();
            Version = UIGlobal.Version;

            foreColor = UIStyles.Blue.SmoothLabelForeColor;
            rectColor = UIStyles.Blue.SmoothLabelRectColor;

            drawPath = new GraphicsPath();
            drawPen = new Pen(rectColor, rectSize);
            forecolorBrush = new SolidBrush(ForeColor);
            Size = new Size(300, 60);
        }

        [Browsable(false)]
        [Description("Array of property names that need multi-language translation when the control is displayed"), Category("SunnyUI")]
        public string[] FormTranslatorProperties => ["Text"];

        [DefaultValue(true)]
        [Description("Multi-language support when the control is displayed"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                forecolorBrush?.Dispose();
                drawPath?.Dispose();
                drawPen?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Position of the control in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set the control scaling ratio
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public void SetZoomScale(float scale)
        {

        }

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        private Color foreColor;

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get => foreColor;
            set
            {
                foreColor = value;
                forecolorBrush.Color = foreColor;
                Invalidate();
            }
        }

        public string Version { get; }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="style">Theme style</param>
        private void SetStyle(UIStyle style)
        {
            if (!style.IsCustom())
            {
                SetStyleColor(style.Colors());
                Invalidate();
            }

            _style = style == UIStyle.Inherited ? UIStyle.Inherited : UIStyle.Custom;
        }

        public void SetInheritedStyle(UIStyle style)
        {
            SetStyle(style);
            _style = UIStyle.Inherited;
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        public void SetStyleColor(UIBaseStyle uiColor)
        {
            foreColor = uiColor.SmoothLabelForeColor;
            rectColor = uiColor.SmoothLabelRectColor;

            forecolorBrush.Color = foreColor;
            if (rectSize != 0)
                drawPen.Color = rectColor;
        }

        private UIStyle _style = UIStyle.Inherited;

        /// <summary>
        /// Theme style
        /// </summary>
        [DefaultValue(UIStyle.Inherited), Description("Theme style"), Category("SunnyUI")]
        public UIStyle Style
        {
            get => _style;
            set => SetStyle(value);
        }

        /// <summary>
        /// Override font change
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            Invalidate();
        }

        private Color rectColor;

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RectColor
        {
            get
            {
                return rectColor;
            }
            set
            {
                if (rectColor != value)
                {
                    rectColor = value;
                    if (rectSize != 0) drawPen.Color = rectColor;
                    RectColorChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
        }

        public event EventHandler RectColorChanged;

        private int rectSize = 2;

        /// <summary>
        /// Border width
        /// </summary>
        [Description("Border width"), Category("SunnyUI")]
        [DefaultValue(2)]
        public int RectSize
        {
            get => rectSize;
            set
            {
                value = Math.Max(0, value);
                if (rectSize != value)
                {
                    rectSize = value;

                    if (value == 0)
                    {
                        drawPen.Color = Color.Transparent;
                    }
                    else
                    {
                        drawPen.Color = RectColor;
                        drawPen.Width = value;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Override paint method
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Text.IsNullOrEmpty()) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            drawSize = TextRenderer.MeasureText(Text, Font);
            if (AutoSize)
            {
                point.X = Padding.Left;
                point.Y = Padding.Top;
            }
            else
            {
                if (TextAlign == ContentAlignment.TopLeft ||
                    TextAlign == ContentAlignment.MiddleLeft ||
                    TextAlign == ContentAlignment.BottomLeft)
                {
                    point.X = Padding.Left;
                }
                else if (TextAlign == ContentAlignment.TopCenter ||
                         TextAlign == ContentAlignment.MiddleCenter ||
                         TextAlign == ContentAlignment.BottomCenter)
                {
                    point.X = (Width - drawSize.Width) / 2;
                }
                else
                {
                    point.X = Width - (Padding.Right + drawSize.Width);
                }

                if (TextAlign == ContentAlignment.TopLeft ||
                    TextAlign == ContentAlignment.TopCenter ||
                    TextAlign == ContentAlignment.TopRight)
                {
                    point.Y = Padding.Top;
                }
                else if (TextAlign == ContentAlignment.MiddleLeft ||
                         TextAlign == ContentAlignment.MiddleCenter ||
                         TextAlign == ContentAlignment.MiddleRight)
                {
                    point.Y = (Height - drawSize.Height) / 2;
                }
                else
                {
                    point.Y = Height - (Padding.Bottom + drawSize.Height);
                }
            }

            float fontSize = e.Graphics.DpiY * Font.SizeInPoints / 72;

            drawPath.Reset();
            drawPath.AddString(Text, Font.FontFamily, (int)Font.Style, fontSize, point, StringFormat.GenericTypographic);

            e.Graphics.FillPath(forecolorBrush, drawPath);
            e.Graphics.DrawPath(drawPen, drawPath);
        }
    }
}
