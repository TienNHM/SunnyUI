/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua.
 * QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIRoundProcess.cs
 * Description: Circular progress bar
 * Current Version: V3.1
 * Creation Date: 2021-04-08
 *
 * 2021-04-08: V3.0.2 Added file description
 * 2021-10-18: V3.0.8 Added display of decimal places
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-07-12: V3.4.0 Updated colors when the inner circle size is small
 * 2023-07-15: V3.4.0 Added start angle and sweep angle
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Circular scrollbar
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    public class UIRoundProcess : UIControl
    {
        public UIRoundProcess()
        {
            SetStyleFlags(true, false);
            Size = new Size(120, 120);
            Inner = 30;
            Outer = 50;

            fillColor = UIStyles.Blue.ProcessBarForeColor;
            foreColor = UIStyles.Blue.ProcessBarForeColor;
            rectColor = UIStyles.Blue.ProcessBackColor;

            ShowText = false;
            ShowRect = false;
        }

        private int startAngle = 0;

        [Description("Start angle, 0 at due north, clockwise from 0 to 360°"), Category("SunnyUI")]
        [DefaultValue(0)]
        public int StartAngle
        {
            get => startAngle;
            set
            {
                startAngle = value;
                Invalidate();
            }
        }

        private int sweepAngle = 360;

        [Description("Sweep angle, range from 0 to 360°"), Category("SunnyUI")]
        [DefaultValue(360)]
        public int SweepAngle
        {
            get => sweepAngle;
            set
            {
                sweepAngle = Math.Max(1, value);
                sweepAngle = Math.Min(360, value);
                Invalidate();
            }
        }

        [Description("Display decimal places"), Category("SunnyUI")]
        [DefaultValue(1)]
        public int DecimalPlaces
        {
            get => decimalCount;
            set
            {
                decimalCount = Math.Max(value, 0);
                Text = (posValue * 100.0 / maximum).ToString("F" + decimalCount) + "%";
            }
        }

        private int decimalCount = 1;

        private int maximum = 100;

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

        private int inner = 30;
        private int outer = 50;

        [Description("Inner diameter")]
        [Category("SunnyUI")]
        [DefaultValue(30)]
        public int Inner
        {
            get => inner;
            set
            {
                inner = Math.Max(value, 0);
                Invalidate();
            }
        }

        [Description("Outer diameter")]
        [Category("SunnyUI")]
        [DefaultValue(50)]
        public int Outer
        {
            get => outer;
            set
            {
                outer = Math.Max(value, 5);
                Invalidate();
            }
        }

        /// <summary>
        /// Progress bar foreground color
        /// </summary>
        [Description("Progress bar foreground color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ProcessColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Progress bar background color
        /// </summary>
        [Description("Progress bar background color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "185, 217, 255")]
        public Color ProcessBackColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
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
                    Text = (posValue * 100.0 / maximum).ToString("F" + decimalCount) + "%";
                    ValueChanged?.Invoke(this, posValue);
                    Invalidate();
                }
            }
        }

        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            if (ShowText)
            {
                Size size = TextRenderer.MeasureText(Text, Font);
                if (Inner * 2 < size.Width - 4)
                    g.DrawString(Text, Font, ForeColor2, new Rectangle(0, 0, Width, Height), ContentAlignment.MiddleCenter);
                else
                    g.DrawString(Text, Font, ForeColor, new Rectangle(0, 0, Width, Height), ContentAlignment.MiddleCenter);
            }
        }

        private Color foreColor2 = Color.Black;
        public Color ForeColor2
        {
            get => foreColor2;
            set
            {
                foreColor2 = value;
                Invalidate();
            }
        }

        public delegate void OnValueChanged(object sender, int value);

        public event OnValueChanged ValueChanged;

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            int iin = Math.Min(inner, outer);
            int iou = Math.Max(inner, outer);
            if (iin == iou)
            {
                iou = iin + 1;
            }

            inner = iin;
            outer = iou;

            g.FillFan(ProcessBackColor, ClientRectangle.Center(), Inner, Outer, StartAngle - 90, SweepAngle);
            g.FillFan(ProcessColor, ClientRectangle.Center(), Inner, Outer, StartAngle - 90, Value * 1.0f / Maximum * SweepAngle);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            fillColor = uiColor.ProcessBarForeColor;
            foreColor = uiColor.ProcessBarForeColor;
            rectColor = uiColor.ProcessBackColor;
            foreColor2 = uiColor.RoundProcessForeColor2;
        }

        [DefaultValue(false)]
        public bool ShowProcess
        {
            get => ShowText;
            set => ShowText = value;
        }
    }
}
