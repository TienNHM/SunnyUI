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
 ******************************************************************************
 * File Name: UISymbolLabel.cs
 * Description: Label with font icon
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-04-23: V2.2.4 Added UISymbolLabel
 * 2021-12-24: V3.0.9 Fixed bug with Dock and AutoSize set simultaneously
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-05-16: V3.3.6 Refactored DrawFontImage function
 * 2023-10-26: V3.5.1 Added SymbolRotate parameter for font icon rotation
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public sealed class UISymbolLabel : UIControl, ISymbol
    {
        private int _symbolSize = 24;
        private int _imageInterval = 0;

        private Color symbolColor;

        public UISymbolLabel()
        {
            SetStyleFlags(true, false);
            ShowRect = false;
            symbolColor = UIStyles.Blue.LabelForeColor;
            foreColor = UIStyles.Blue.LabelForeColor;
            Width = 170;
            Height = 35;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        private bool autoSize;

        [Browsable(true)]
        [Description("Auto size"), Category("SunnyUI")]
        public override bool AutoSize
        {
            get => autoSize;
            set
            {
                autoSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Icon color
        /// </summary>
        [Description("Icon color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public Color SymbolColor
        {
            get => symbolColor;
            set
            {
                symbolColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        /// <summary>
        /// Font icon size
        /// </summary>
        [DefaultValue(24)]
        [Description("Font icon size"), Category("SunnyUI")]
        public int SymbolSize
        {
            get => _symbolSize;
            set
            {
                _symbolSize = Math.Max(value, 16);
                _symbolSize = Math.Min(value, 128);
                Invalidate();
            }
        }

        private int _symbolRotate = 0;

        /// <summary>
        /// Font icon rotation angle
        /// </summary>
        [DefaultValue(0)]
        [Description("Font icon rotation angle"), Category("SunnyUI")]
        public int SymbolRotate
        {
            get => _symbolRotate;
            set
            {
                if (_symbolRotate != value)
                {
                    _symbolRotate = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(0)]
        [Description("Interval between icon and text"), Category("SunnyUI")]
        public int ImageInterval
        {
            get => _imageInterval;
            set
            {
                _imageInterval = Math.Max(0, value);
                Invalidate();
            }
        }

        private bool _isCircle;

        [DefaultValue(false)]
        [Description("Display as circle"), Category("SunnyUI")]
        public bool IsCircle
        {
            get => _isCircle;
            set
            {
                _isCircle = value;
                if (value)
                {
                    Text = "";
                }
                else
                {
                    Invalidate();
                }
            }
        }

        private int _symbol = 361452;

        /// <summary>
        /// Font icon
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(361452)]
        [Description("Font icon"), Category("SunnyUI")]
        public int Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                Invalidate();
            }
        }

        private Point symbolOffset = new Point(0, 0);

        /// <summary>
        /// Offset position of font icon
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Offset position of font icon"), Category("SunnyUI")]
        public Point SymbolOffset
        {
            get => symbolOffset;
            set
            {
                symbolOffset = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            g.FillRectangle(BackColor, Bounds);
        }

        private int circleRectWidth = 1;

        [DefaultValue(1)]
        [Description("Circle border size"), Category("SunnyUI")]
        public int CircleRectWidth
        {
            get => circleRectWidth;
            set
            {
                circleRectWidth = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw border color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintRect(Graphics g, GraphicsPath path)
        {
            if (IsCircle)
            {
                int size = Math.Min(Width, Height) - 2 - CircleRectWidth;
                using var pn = new Pen(GetRectColor(), CircleRectWidth);
                g.SetHighQuality();
                g.DrawEllipse(pn, (Width - size) / 2.0f, (Height - size) / 2.0f, size, size);
                g.SetDefaultQuality();
            }
            else
            {
                base.OnPaintRect(g, path);
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            symbolColor = uiColor.LabelForeColor;
            foreColor = uiColor.LabelForeColor;
        }

        /// <summary>
        /// Override paint
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //重绘父类
            base.OnPaint(e);
            Size TextSize = TextRenderer.MeasureText(Text, Font);

            int height = Math.Max(SymbolSize, TextSize.Height);
            int width = SymbolSize + ImageInterval + TextSize.Width;

            if (Dock == DockStyle.None && autoSize)
            {
                TextAlign = ContentAlignment.MiddleCenter;
                if (Width != width + 4) Width = width + 4;
                if (Height != height + 4) Height = height + 4;
            }

            Rectangle rect;
            switch (TextAlign)
            {
                case ContentAlignment.TopLeft:
                    rect = new Rectangle(Padding.Left, Padding.Top, width, height);
                    break;
                case ContentAlignment.TopCenter:
                    rect = new Rectangle((Width - width) / 2, Padding.Top, width, height);
                    break;
                case ContentAlignment.TopRight:
                    rect = new Rectangle(Width - width - Padding.Right, Padding.Top, width, height);
                    break;
                case ContentAlignment.MiddleLeft:
                    rect = new Rectangle(Padding.Left, (Height - height) / 2, width, height);
                    break;
                case ContentAlignment.MiddleCenter:
                    rect = new Rectangle((Width - width) / 2, (Height - height) / 2, width, height);
                    break;
                case ContentAlignment.MiddleRight:
                    rect = new Rectangle(Width - width - Padding.Right, (Height - height) / 2, width, height);
                    break;
                case ContentAlignment.BottomLeft:
                    rect = new Rectangle(Padding.Left, Height - Padding.Bottom - height, width, height);
                    break;
                case ContentAlignment.BottomCenter:
                    rect = new Rectangle((Width - width) / 2, Height - Padding.Bottom - height, width, height);
                    break;
                case ContentAlignment.BottomRight:
                    rect = new Rectangle(Width - width - Padding.Right, Height - Padding.Bottom - height, width, height);
                    break;
                default:
                    rect = new Rectangle((Width - width) / 2, (Height - height) / 2, width, height);
                    break;
            }

            if (Text.IsNullOrEmpty())
                e.Graphics.DrawFontImage(Symbol, SymbolSize, symbolColor, new RectangleF((Width - SymbolSize) / 2.0f, (Height - SymbolSize) / 2.0f, SymbolSize, SymbolSize), SymbolOffset.X, SymbolOffset.Y, SymbolRotate);
            else
                e.Graphics.DrawFontImage(Symbol, SymbolSize, symbolColor, new Rectangle(rect.Left, rect.Top, SymbolSize, rect.Height), SymbolOffset.X, SymbolOffset.Y, SymbolRotate);

            e.Graphics.DrawString(Text, Font, ForeColor, rect, ContentAlignment.MiddleRight);
        }

        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        { }
    }
}
