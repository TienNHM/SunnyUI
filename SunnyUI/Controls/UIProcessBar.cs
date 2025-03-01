/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIProcessBar.cs
 * Description: Progress Bar
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-19: V2.2.5 Added value change event
 * 2021-08-07: V3.0.5 Added vertical progress display
 * 2021-08-14: V3.0.6 Modified to display value when percentage is not shown
 * 2021-10-14: V3.0.8 Adjusted minimum height to 3
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-09-05: V3.2.3 Modified maximum value to be at least 1
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-09-05: V3.4.2 Fixed int overflow issue during value calculation
 * 2024-05-09: V3.6.6 Adjusted minimum width to accommodate vertical display
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    public sealed class UIProcessBar : UIControl
    {
        private int maximum = 100;

        public delegate void OnValueChanged(object sender, int value);

        public event OnValueChanged ValueChanged;

        public UIProcessBar()
        {
            SetStyleFlags(true, false);
            MinimumSize = new Size(3, 3);
            Size = new Size(300, 29);
            ShowText = false;

            fillColor = UIColor.LightBlue;
            foreColor = UIColor.Blue;
        }
        private UILine.LineDirection direction = UILine.LineDirection.Horizontal;

        [DefaultValue(UILine.LineDirection.Horizontal)]
        [Description("Line direction"), Category("SunnyUI")]
        public UILine.LineDirection Direction
        {
            get => direction;
            set
            {
                if (direction != value)
                {
                    direction = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(100)]
        [Description("Maximum value"), Category("SunnyUI")]
        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = Math.Max(1, value);
                Invalidate();
            }
        }

        private int posValue;

        [DefaultValue(0)]
        [Description("Current position"), Category("SunnyUI")]
        public int Value
        {
            get => posValue;
            set
            {
                value = Math.Max(value, 0);
                value = Math.Min(value, maximum);
                if (posValue != value)
                {
                    posValue = value;
                    ValueChanged?.Invoke(this, posValue);
                    Invalidate();
                }
            }
        }

        private bool showValue = true;

        [DefaultValue(true)]
        [Description("Show progress value"), Category("SunnyUI")]
        public bool ShowValue
        {
            get => showValue;
            set
            {
                showValue = value;
                Invalidate();
            }
        }

        [DefaultValue(1)]
        [Description("Step value"), Category("SunnyUI")]
        public int Step { get; set; } = 1;

        public void StepIt()
        {
            Value = Math.Min(Value + Step, Maximum);
        }

        private Bitmap image;
        private int imageRadius;

        /// <summary>
        /// Override paint method
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float processSize;
            string processText;

            if (Direction == UILine.LineDirection.Horizontal)
                processSize = posValue * 1.0f / Maximum * Width;
            else
                processSize = posValue * 1.0f / Maximum * Height;

            if (ShowPercent)
                processText = (posValue * 100.0 / maximum).ToString("F" + decimalCount) + "%";
            else
                processText = posValue.ToString();

            Size sf = TextRenderer.MeasureText("100%", Font);
            bool canShow;
            if (Direction == UILine.LineDirection.Horizontal)
                canShow = Height > sf.Height + 4;
            else
                canShow = Width > sf.Width + 4;

            if (ShowValue && canShow)
            {
                DrawString(e.Graphics, processText, Font, foreColor, Size, Padding, TextAlign);
            }

            if (image == null || image.Width != Width || image.Height != Height || imageRadius != Radius)
            {
                image?.Dispose();
                image = new Bitmap(Width, Height);
                imageRadius = Radius;
            }

            using Graphics g = image.Graphics();
            g.Clear(Color.Transparent);
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            g.SetHighQuality();
            g.FillRoundRectangle(rectColor, rect, Radius);
            g.DrawRoundRectangle(rectColor, rect, Radius);
            if (ShowValue && canShow)
            {
                DrawString(g, processText, Font, fillColor, Size, Padding, TextAlign);
            }

            if (Direction == UILine.LineDirection.Horizontal)
            {
                e.Graphics.DrawImage(image,
                    new RectangleF(0, 0, processSize, image.Height),
                    new RectangleF(0, 0, processSize, image.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                e.Graphics.DrawImage(image,
                    new RectangleF(0, image.Height - processSize, image.Width, processSize),
                    new RectangleF(0, image.Height - processSize, image.Width, processSize),
                    GraphicsUnit.Pixel);
            }
        }

        private void DrawString(Graphics g, string str, Font font, Color color, Size size, Padding padding, ContentAlignment align, int offsetX = 0, int offsetY = 0)
        {
            if (str.IsNullOrEmpty()) return;
            Size sf = TextRenderer.MeasureText(str, font);
            using Brush br = color.Brush();
            switch (align)
            {
                case ContentAlignment.MiddleCenter:
                    g.DrawString(str, font, br, padding.Left + (size.Width - sf.Width - padding.Left - padding.Right) / 2.0f + offsetX,
                        padding.Top + (size.Height - sf.Height - padding.Top - padding.Bottom) / 2.0f + offsetY);
                    break;

                case ContentAlignment.TopLeft:
                    g.DrawString(str, font, br, padding.Left + offsetX, padding.Top + offsetY);
                    break;

                case ContentAlignment.TopCenter:
                    g.DrawString(str, font, br, padding.Left + (size.Width - sf.Width - padding.Left - padding.Right) / 2.0f + offsetX, padding.Top + offsetY);
                    break;

                case ContentAlignment.TopRight:
                    g.DrawString(str, font, br, size.Width - sf.Width - padding.Right + offsetX, padding.Top + offsetY);
                    break;

                case ContentAlignment.MiddleLeft:
                    g.DrawString(str, font, br, padding.Left + offsetX, padding.Top + (size.Height - sf.Height - padding.Top - padding.Bottom) / 2.0f + offsetY);
                    break;

                case ContentAlignment.MiddleRight:
                    g.DrawString(str, font, br, size.Width - sf.Width - padding.Right + offsetX, padding.Top + (size.Height - sf.Height - padding.Top - padding.Bottom) / 2.0f + offsetY);
                    break;

                case ContentAlignment.BottomLeft:
                    g.DrawString(str, font, br, padding.Left + offsetX, size.Height - sf.Height - padding.Bottom + offsetY);
                    break;

                case ContentAlignment.BottomCenter:
                    g.DrawString(str, font, br, padding.Left + (size.Width - sf.Width - padding.Left - padding.Right) / 2.0f + offsetX, size.Height - sf.Height - padding.Bottom + offsetY);
                    break;

                case ContentAlignment.BottomRight:
                    g.DrawString(str, font, br, size.Width - sf.Width - padding.Right + offsetX, size.Height - sf.Height - padding.Bottom + offsetY);
                    break;
            }
        }

        /// <summary>
        /// Override size changed event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        private bool showPercent = true;

        [Description("Show percentage text"), Category("SunnyUI")]
        [DefaultValue(true)]
        public bool ShowPercent
        {
            get => showPercent;
            set
            {
                showPercent = value;
                Invalidate();
            }
        }

        [Description("Decimal places for text"), Category("SunnyUI")]
        [DefaultValue(1)]
        public int DecimalPlaces
        {
            get => decimalCount;
            set => decimalCount = Math.Max(value, 0);
        }

        private int decimalCount = 1;

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            fillColor = uiColor.ProcessBarFillColor;
            foreColor = uiColor.ProcessBarForeColor;
        }

        /// <summary>
        /// Fill color, if the value is background color, transparent color, or null, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
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
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RectColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }
    }
}