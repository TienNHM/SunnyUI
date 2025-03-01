/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
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
 * File Name: UISymbolButton.cs
 * File Description: Font Icon Button
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-07-26: V2.2.6 Added Image property, added image and text placement position
 * 2022-01-05: V3.0.9 Added color setting for font icons
 * 2022-03-19: V3.1.1 Refactored theme color matching
 * 2023-05-15: V3.3.6 Refactored DrawString function
 * 2023-05-16: V3.3.6 Refactored DrawFontImage function
 * 2023-11-24: V3.6.2 Fixed text and icon color for LightStyle
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public class UISymbolButton : UIButton, ISymbol
    {
        private int _symbolSize = 24;
        private int _imageInterval = 0;

        public UISymbolButton()
        {
            ShowText = false;
        }

        private bool autoSize;

        [Browsable(false)]
        [Description("Auto size"), Category("SunnyUI")]
        public override bool AutoSize
        {
            get => autoSize;
            set => autoSize = false;
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

        private Color symbolColor = Color.White;

        /// <summary>
        /// Font icon color
        /// </summary>
        [Description("Icon color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color SymbolColor
        {
            get => symbolColor;
            set
            {
                if (symbolColor != value)
                {
                    symbolColor = value;
                    Invalidate();
                }
            }
        }

        private Color symbolHoverColor = Color.White;
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Font color when mouse hovers over the icon")]
        public Color SymbolHoverColor
        {
            get => symbolHoverColor;
            set
            {
                if (symbolHoverColor != value)
                {
                    symbolHoverColor = value;
                    Invalidate();
                }
            }
        }

        private Color symbolPressColor = Color.White;
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Font color when mouse presses the icon")]
        public Color SymbolPressColor
        {
            get => symbolPressColor;
            set
            {
                if (symbolPressColor != value)
                {
                    symbolPressColor = value;
                    Invalidate();
                }
            }
        }

        private Color symbolDisableColor = Color.FromArgb(109, 109, 103);
        [DefaultValue(typeof(Color), "109, 109, 103"), Category("SunnyUI")]
        [Description("Font color when the icon is disabled")]
        public Color SymbolDisableColor
        {
            get => symbolDisableColor;
            set
            {
                if (symbolDisableColor != value)
                {
                    symbolDisableColor = value;
                    Invalidate();
                }
            }
        }

        private Color symbolSelectedColor = Color.White;
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Font color when the icon is selected")]
        public Color SymbolSelectedColor
        {
            get => symbolSelectedColor;
            set
            {
                if (symbolSelectedColor != value)
                {
                    symbolSelectedColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            symbolColor = uiColor.ButtonForeColor;
            symbolHoverColor = uiColor.ButtonForeHoverColor;
            symbolPressColor = uiColor.ButtonForePressColor;
            symbolSelectedColor = uiColor.ButtonForeSelectedColor;
            symbolDisableColor = uiColor.ForeDisableColor;
        }

        private Image image;

        [DefaultValue(null)]
        [Description("Image"), Category("SunnyUI")]
        public Image Image
        {
            get => image;
            set
            {
                if (image != value)
                {
                    image = value;
                    Invalidate();
                }
            }
        }

        private ContentAlignment imageAlign = ContentAlignment.MiddleCenter;

        [DefaultValue(ContentAlignment.MiddleCenter)]
        [Description("Image placement position"), Category("SunnyUI")]
        public ContentAlignment ImageAlign
        {
            get => imageAlign;
            set
            {
                imageAlign = value;
                Invalidate();
            }
        }

        [DefaultValue(0)]
        [Description("Interval between image and text"), Category("SunnyUI")]
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
        [Description("Is it circular"), Category("SunnyUI")]
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
        /// Offset position of the font icon
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Offset position of the font icon"), Category("SunnyUI")]
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
            if (IsCircle)
            {
                int size = Math.Min(Width, Height) - 2 - CircleRectWidth;
                g.FillEllipse(GetFillColor(), (Width - size) / 2.0f - 1, (Height - size) / 2.0f - 1, size, size);
            }
            else
            {
                base.OnPaintFill(g, path);
            }
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
                g.DrawEllipse(pn, (Width - size) / 2.0f - 1, (Height - size) / 2.0f - 1, size, size);
                g.SetDefaultQuality();
            }
            else
            {
                base.OnPaintRect(g, path);
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Get font color
        /// </summary>
        /// <returns>Color</returns>
        protected Color GetSymbolForeColor()
        {
            //文字
            Color color = lightStyle ? rectColor : symbolColor;
            if (IsHover)
                color = symbolHoverColor;
            if (IsPress)
                color = symbolPressColor;
            if (selected)
                color = symbolSelectedColor;
            if (ShowFocusColor && Focused)
                color = symbolPressColor;
            return Enabled ? color : symbolDisableColor;
        }

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //重绘父类
            base.OnPaint(e);

            Size ImageSize = new Size(0, 0);
            if (Symbol > 0)
                ImageSize = new Size(SymbolSize, SymbolSize);
            if (Image != null)
                ImageSize = Image.Size;

            //字体图标
            Color color = GetForeColor();
            Size TextSize = TextRenderer.MeasureText(Text, Font);

            if (ImageAlign == ContentAlignment.MiddleCenter && TextAlign == ContentAlignment.MiddleCenter)
            {
                if (ImageSize.Width == 0)
                {
                    e.Graphics.DrawString(Text, Font, color, ClientRectangle, ContentAlignment.MiddleCenter);
                }
                else if (Text.IsNullOrEmpty())
                {
                    if (ImageSize.Width > 0)
                    {
                        if (Symbol > 0 && Image == null)
                        {
                            e.Graphics.DrawFontImage(Symbol, SymbolSize, GetSymbolForeColor(), ClientRectangle, SymbolOffset.X, SymbolOffset.Y, SymbolRotate);
                        }

                        if (Image != null)
                        {
                            e.Graphics.DrawImage(Image, (Width - ImageSize.Width) / 2.0f,
                                Padding.Top + (Height - ImageSize.Height - Padding.Top - Padding.Bottom) / 2.0f, ImageSize.Width, ImageSize.Height);
                        }
                    }
                }
                else
                {
                    int allWidth = ImageSize.Width + ImageInterval + TextSize.Width;

                    if (Symbol > 0 && Image == null)
                    {
                        e.Graphics.DrawFontImage(Symbol, SymbolSize, GetSymbolForeColor(),
                            new RectangleF((Width - allWidth) / 2.0f, 0, ImageSize.Width, Height), SymbolOffset.X, SymbolOffset.Y, SymbolRotate);
                    }

                    if (Image != null)
                    {
                        e.Graphics.DrawImage(Image, (Width - allWidth) / 2.0f, (Height - ImageSize.Height) / 2.0f,
                            ImageSize.Width, ImageSize.Height);
                    }

                    e.Graphics.DrawString(Text, Font, color, new Rectangle((int)((Width - allWidth) / 2.0f + ImageSize.Width + ImageInterval), 0, Width, Height), ContentAlignment.MiddleLeft);
                }
            }
            else
            {
                float left = 0;
                float top = 0;

                if (ImageSize.Width > 0)
                {
                    switch (ImageAlign)
                    {
                        case ContentAlignment.TopLeft:
                            left = Padding.Left;
                            top = Padding.Top;
                            break;

                        case ContentAlignment.TopCenter:
                            left = (Width - ImageSize.Width) / 2.0f;
                            top = Padding.Top;
                            break;

                        case ContentAlignment.TopRight:
                            left = Width - Padding.Right - ImageSize.Width;
                            top = Padding.Top;
                            break;

                        case ContentAlignment.MiddleLeft:
                            left = Padding.Left;
                            top = (Height - ImageSize.Height) / 2.0f;
                            break;

                        case ContentAlignment.MiddleCenter:
                            left = (Width - ImageSize.Width) / 2.0f;
                            top = (Height - ImageSize.Height) / 2.0f;
                            break;

                        case ContentAlignment.MiddleRight:
                            left = Width - Padding.Right - ImageSize.Width;
                            top = (Height - ImageSize.Height) / 2.0f;
                            break;

                        case ContentAlignment.BottomLeft:
                            left = Padding.Left;
                            top = Height - Padding.Bottom - ImageSize.Height;
                            break;

                        case ContentAlignment.BottomCenter:
                            left = (Width - ImageSize.Width) / 2.0f;
                            top = Height - Padding.Bottom - ImageSize.Height;
                            break;

                        case ContentAlignment.BottomRight:
                            left = Width - Padding.Right - ImageSize.Width;
                            top = Height - Padding.Bottom - ImageSize.Height;
                            break;
                    }

                    if (Symbol > 0 && Image == null)
                    {
                        e.Graphics.DrawFontImage(Symbol, SymbolSize, GetSymbolForeColor(),
                            new Rectangle((int)left, (int)top, (int)ImageSize.Width, (int)ImageSize.Height), SymbolOffset.X, SymbolOffset.Y, SymbolRotate);
                    }

                    if (Image != null)
                    {
                        e.Graphics.DrawImage(Image, left, top, ImageSize.Width, ImageSize.Height);
                    }
                }

                e.Graphics.DrawString(Text, Font, color, new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom), TextAlign);
            }
        }
    }
}