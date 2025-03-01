/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Class Library, Extension Class Library, Multi-Page Development Framework.
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
 * File Name: UILine.cs
 * Description: Divider Line
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2022-01-05: V3.0.9 Added line styles, supports transparent background
 * 2022-01-10: V3.1.0 Fixed issue where text was not displayed when empty
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-11-26: V3.2.9 When horizontal text is not centered, gradient color for lines can be set
 * 2023-05-12: V3.3.6 Refactored DrawString function
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
    public sealed class UILine : UIControl
    {
        public UILine()
        {
            SetStyleFlags(true, false);
            Size = new Size(360, 29);
            MinimumSize = new Size(1, 1);
            ForeColor = UIStyles.Blue.LineForeColor;
            fillColor = UIColor.Transparent;
            fillColor2 = UIStyles.Blue.PanelFillColor2;
            BackColor = UIColor.Transparent;
            rectColor = UIStyles.Blue.LineRectColor;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        public enum LineDirection
        {
            /// <summary>
            /// Horizontal
            /// </summary>
            Horizontal,

            /// <summary>
            /// Vertical
            /// </summary>
            Vertical
        }

        private LineDirection direction = LineDirection.Horizontal;

        [DefaultValue(LineDirection.Horizontal)]
        [Description("Line direction"), Category("SunnyUI")]
        public LineDirection Direction
        {
            get => direction;
            set
            {
                direction = value;
                Invalidate();
            }
        }

        private int lineSize = 1;

        [Description("Line width"), Category("SunnyUI")]
        [DefaultValue(1)]
        public int LineSize
        {
            get => lineSize;
            set
            {
                lineSize = Math.Max(1, value);
                Invalidate();
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            rectColor = uiColor.LineRectColor;
            ForeColor = uiColor.LineForeColor;
            fillColor2 = uiColor.PanelFillColor2;
            fillColor = UIColor.Transparent;
            BackColor = UIColor.Transparent;
        }

        private UILineCap startCap = UILineCap.None;

        [DefaultValue(UILineCap.None), Category("SunnyUI")]
        public UILineCap StartCap
        {
            get => startCap;
            set
            {
                startCap = value;
                Invalidate();
            }
        }

        private UILineCap endCap = UILineCap.None;

        [DefaultValue(UILineCap.None), Category("SunnyUI")]
        public UILineCap EndCap
        {
            get => endCap;
            set
            {
                endCap = value;
                Invalidate();
            }
        }

        private int lineCapSize = 6;

        [DefaultValue(6), Category("SunnyUI")]
        public int LineCapSize
        {
            get => lineCapSize;
            set
            {
                lineCapSize = value;
                Invalidate();
            }
        }

        private int textInterval = 10;

        [DefaultValue(10)]
        [Description("Text margin interval"), Category("SunnyUI")]
        public int TextInterval
        {
            get => textInterval;
            set
            {
                textInterval = value;
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
            //
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            SizeF sf = new SizeF(0, 0);
            float x = 0;
            using Pen pen = new Pen(rectColor, lineSize);
            if (LineDashStyle != UILineDashStyle.None)
            {
                pen.DashStyle = (DashStyle)((int)LineDashStyle);
            }

            if (Direction == LineDirection.Horizontal)
            {
                if (Text.IsValid())
                {
                    sf = TextRenderer.MeasureText(Text, Font);
                    switch (TextAlign)
                    {
                        case ContentAlignment.TopLeft:
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, 0, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, (Height - lineSize) / 2), ContentAlignment.BottomLeft); break;
                        case ContentAlignment.TopCenter:
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, 0, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, (Height - lineSize) / 2), ContentAlignment.BottomCenter); break;
                        case ContentAlignment.TopRight:
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, 0, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, (Height - lineSize) / 2), ContentAlignment.BottomRight); break;
                        case ContentAlignment.MiddleLeft:
                            x = Padding.Left + TextInterval;
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, 0, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, Height), TextAlign); break;
                        case ContentAlignment.MiddleCenter:
                            x = (Width - sf.Width) / 2 - 2;
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, 0, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, Height), TextAlign); break;
                        case ContentAlignment.MiddleRight:
                            x = Width - sf.Width - TextInterval - 4 - Padding.Right;
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, 0, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, Height), TextAlign); break;
                        case ContentAlignment.BottomLeft:
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, (Height + lineSize) / 2, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, Height), ContentAlignment.TopLeft); break;
                        case ContentAlignment.BottomCenter:
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, (Height + lineSize) / 2, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, Height), ContentAlignment.TopCenter); break;
                        case ContentAlignment.BottomRight:
                            g.DrawString(Text, Font, ForeColor, new Rectangle(Padding.Left + TextInterval + 2, (Height + lineSize) / 2, Width - Padding.Left - textInterval - 2 - Padding.Right - textInterval - 2, Height), ContentAlignment.TopRight); break;
                    }
                }

                int top = Height / 2;
                if (Text.IsValid())
                {
                    switch (TextAlign)
                    {
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.MiddleRight:
                            g.DrawLine(pen, Padding.Left, top, x, top);
                            g.DrawLine(pen, x + sf.Width + 2, top, Width - 2 - Padding.Left - Padding.Right, top);
                            break;
                        default:
                            if (LineColorGradient)
                            {
                                top = (Height - lineSize) / 2;
                                using LinearGradientBrush br = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), LineColor, LineColor2);
                                g.FillRectangle(br, new Rectangle(Padding.Left, top, Width - 2 - Padding.Left - Padding.Right, LineSize));
                            }
                            else
                            {
                                g.DrawLine(pen, Padding.Left, top, Width - 2 - Padding.Left - Padding.Right, top);
                            }
                            break;
                    }
                }
                else
                {
                    if (LineColorGradient)
                    {
                        top = (Height - lineSize) / 2;
                        using LinearGradientBrush br = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), LineColor, LineColor2);
                        g.FillRectangle(br, new Rectangle(Padding.Left, top, Width - 2 - Padding.Left - Padding.Right, LineSize));
                    }
                    else
                    {
                        g.DrawLine(pen, Padding.Left, top, Width - 2 - Padding.Left - Padding.Right, top);
                    }
                }

                switch (startCap)
                {
                    case UILineCap.Square:
                        top = Height / 2 - LineCapSize - 1;
                        g.FillRectangle(rectColor, new RectangleF(0, top, LineCapSize * 2, LineCapSize * 2));
                        break;
                    case UILineCap.Diamond:
                        break;
                    case UILineCap.Triangle:
                        break;
                    case UILineCap.Circle:
                        top = Height / 2 - LineCapSize - 1;
                        g.FillEllipse(rectColor, new RectangleF(0, top, LineCapSize * 2, LineCapSize * 2));
                        break;
                }

                switch (endCap)
                {
                    case UILineCap.Square:
                        top = Height / 2 - LineCapSize;
                        if (lineSize.Mod(2) == 1) top -= 1;
                        g.FillRectangle(rectColor, new RectangleF(Width - lineCapSize * 2 - 1, top, LineCapSize * 2, LineCapSize * 2));
                        break;
                    case UILineCap.Diamond:
                        break;
                    case UILineCap.Triangle:
                        break;
                    case UILineCap.Circle:
                        top = Height / 2 - LineCapSize - 1;
                        g.FillEllipse(rectColor, new RectangleF(Width - lineCapSize * 2 - 1, top, LineCapSize * 2, LineCapSize * 2));
                        break;
                }
            }
            else
            {
                int left = (Width - lineSize) / 2;
                g.DrawLine(pen, left, Padding.Top, left, Height - Padding.Top - Padding.Bottom);
            }
        }

        UILineDashStyle lineDashStyle = UILineDashStyle.None;
        [Description("Line style"), Category("SunnyUI")]
        [DefaultValue(UILineDashStyle.None)]
        public UILineDashStyle LineDashStyle
        {
            get => lineDashStyle;
            set
            {
                if (lineDashStyle != value)
                {
                    lineDashStyle = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or null, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Line color
        /// </summary>
        [Description("Line color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color LineColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        /// <summary>
        /// Line color 2
        /// </summary>
        [Description("Line color 2"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color LineColor2
        {
            get => fillColor2;
            set => SetFillColor2(value);
        }

        [Description("Line color gradient"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool LineColorGradient
        {
            get => fillColorGradient;
            set
            {
                if (fillColorGradient != value)
                {
                    fillColorGradient = value;
                    Invalidate();
                }
            }
        }
    }
}