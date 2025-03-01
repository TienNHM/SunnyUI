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
 * File Name: UIAvatar.cs
 * Description: Avatar
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-10-26: V3.5.1 Added rotation angle parameter SymbolRotate to font icons
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Avatar
    /// </summary>
    [DefaultEvent("Click")]
    [DefaultProperty("Symbol")]
    [ToolboxItem(true)]
    public sealed class UIAvatar : UIControl, ISymbol, IZoomScale
    {
        /// <summary>
        /// Avatar icon type
        /// </summary>
        public enum UIIcon
        {
            /// <summary>
            /// Image
            /// </summary>
            Image,

            /// <summary>
            /// Symbol
            /// </summary>
            Symbol,

            /// <summary>
            /// Text
            /// </summary>
            Text
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public UIAvatar()
        {
            SetStyleFlags(true, false);
            Width = Height = 60;
            ShowText = false;
            ShowRect = false;

            fillColor = UIStyles.Blue.AvatarFillColor;
            foreColor = UIStyles.Blue.AvatarForeColor;
        }

        private int avatarSize = 60;
        private int baseAvatorSize = 60;

        /// <summary>
        /// Avatar size
        /// </summary>
        [DefaultValue(60), Description("Avatar size"), Category("SunnyUI")]
        public int AvatarSize
        {
            get => avatarSize;
            set
            {
                baseAvatorSize = value;
                avatarSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            fillColor = uiColor.AvatarFillColor;
            foreColor = uiColor.AvatarForeColor;
        }

        private UIIcon icon = UIIcon.Symbol;

        /// <summary>
        /// Display mode: Image, Symbol, Text
        /// </summary>
        [DefaultValue(UIIcon.Symbol), Description("Display mode: Image, Symbol, Text"), Category("SunnyUI")]
        public UIIcon Icon
        {
            get => icon;
            set
            {
                if (icon != value)
                {
                    icon = value;
                    Invalidate();
                }
            }
        }

        private UIShape sharpType = UIShape.Circle;

        /// <summary>
        /// Display shape: Circle, Square
        /// </summary>
        [DefaultValue(UIShape.Circle), Description("Display shape: Circle, Square"), Category("SunnyUI")]
        public UIShape Shape
        {
            get => sharpType;
            set
            {
                if (sharpType != value)
                {
                    sharpType = value;
                    Invalidate();
                }
            }
        }

        private Image image;

        /// <summary>
        /// Image
        /// </summary>
        [DefaultValue(typeof(Image), "null"), Description("Image"), Category("SunnyUI")]
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

        private int symbolSize = 45;
        private int baseSymbolSize = 45;

        /// <summary>
        /// Font icon size
        /// </summary>
        [DefaultValue(45), Description("Font icon size"), Category("SunnyUI")]
        public int SymbolSize
        {
            get => symbolSize;
            set
            {
                if (symbolSize != value)
                {
                    symbolSize = Math.Max(value, 16);
                    symbolSize = Math.Min(value, 128);
                    baseSymbolSize = symbolSize;
                    Invalidate();
                }
            }
        }

        private int symbol = 61447;

        /// <summary>
        /// Font icon
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(61447), Description("Font icon"), Category("SunnyUI")]
        public int Symbol
        {
            get => symbol;
            set
            {
                if (symbol != value)
                {
                    symbol = value;
                    Invalidate();
                }
            }
        }

        private Point symbolOffset = new Point(0, 0);

        /// <summary>
        /// Font icon offset position
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Font icon offset position"), Category("SunnyUI")]
        public Point SymbolOffset
        {
            get => symbolOffset;
            set
            {
                symbolOffset = value;
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

        private Point textOffset = new Point(0, 0);

        /// <summary>
        /// Text offset position
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Text offset position"), Category("SunnyUI")]
        public Point TextOffset
        {
            get => textOffset;
            set
            {
                textOffset = value;
                Invalidate();
            }
        }

        private Point imageOffset = new Point(0, 0);

        /// <summary>
        /// Image offset position
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Image offset position"), Category("SunnyUI")]
        public Point ImageOffset
        {
            get => imageOffset;
            set
            {
                imageOffset = value;
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
            Rectangle rect = new Rectangle((Width - avatarSize) / 2, (Height - avatarSize) / 2, avatarSize, avatarSize);

            switch (Shape)
            {
                case UIShape.Circle:
                    g.FillEllipse(fillColor, rect);
                    break;

                case UIShape.Square:
                    g.FillRoundRectangle(fillColor, rect, 5);
                    break;
            }
        }

        /// <summary>
        /// Horizontal offset
        /// </summary>
        [Browsable(false), DefaultValue(0), Description("Horizontal offset"), Category("SunnyUI")]
        public int OffsetX { get; set; } = 0;

        /// <summary>
        /// Vertical offset
        /// </summary>
        [Browsable(false), DefaultValue(0), Description("Vertical offset"), Category("SunnyUI")]
        public int OffsetY { get; set; } = 0;

        /// <summary>
        /// Continue drawing
        /// </summary>
        public event PaintEventHandler PaintAgain;

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            if (Icon == UIIcon.Image)
            {
                int size = avatarSize;
                if (Image == null)
                {
                    return;
                }

                float sc1 = Image.Width * 1.0f / size;
                float sc2 = Image.Height * 1.0f / size;

                Bitmap scaleImage = ((Bitmap)Image).ResizeImage((int)(Image.Width * 1.0 / Math.Min(sc1, sc2) + 0.5),
                    (int)(Image.Height * 1.0 / Math.Min(sc1, sc2) + 0.5));

                Bitmap bmp = scaleImage.Split(size, Shape);
                e.Graphics.DrawImage(bmp, (Width - avatarSize) / 2 + 1 + ImageOffset.X, (Height - avatarSize) / 2 + 1 + ImageOffset.Y);
                bmp.Dispose();
                scaleImage.Dispose();
                e.Graphics.SetHighQuality();

                using Pen pn = new Pen(BackColor, 4);
                if (Shape == UIShape.Circle)
                {
                    e.Graphics.DrawEllipse(pn, (Width - avatarSize) / 2 + 1 + ImageOffset.X, (Height - avatarSize) / 2 + 1 + ImageOffset.Y, size, size);
                }

                if (Shape == UIShape.Square)
                {
                    e.Graphics.DrawRoundRectangle(pn, (Width - avatarSize) / 2 + 1 + ImageOffset.X, (Height - avatarSize) / 2 + 1 + ImageOffset.Y, size, size, 5);
                }

                e.Graphics.SetDefaultQuality();
            }

            if (Icon == UIIcon.Symbol)
            {
                e.Graphics.DrawFontImage(symbol, symbolSize, ForeColor, new Rectangle((Width - avatarSize) / 2 + 1 + SymbolOffset.X,
                    (Height - avatarSize) / 2 + 1 + SymbolOffset.Y, avatarSize, avatarSize), 0, 0, SymbolRotate);
            }

            if (Icon == UIIcon.Text)
            {
                e.Graphics.DrawString(Text, Font, foreColor, ClientRectangle, ContentAlignment.MiddleCenter, TextOffset.X, TextOffset.Y);
            }

            PaintAgain?.Invoke(this, e);
        }

        /// <summary>
        /// Set control zoom scale
        /// </summary>
        /// <param name="scale">Zoom scale</param>
        public override void SetZoomScale(float scale)
        {
            base.SetZoomScale(scale);
            avatarSize = UIZoomScale.Calc(baseAvatorSize, scale);
            symbolSize = UIZoomScale.Calc(baseSymbolSize, scale);
            Invalidate();
        }
    }
}