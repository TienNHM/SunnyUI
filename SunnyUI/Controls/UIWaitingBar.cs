/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, Multi-Page Development Framework.
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
 * File Name: UIWaitingBar.cs
 * Description: Waiting Scroll Bar Control
 * Current Version: V3.1
 * Creation Date: 2020-07-20
 *
 * 2020-07-20: V2.2.6 Added waiting scroll bar control
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2024-02-23: V3.6.3 Modified to allow custom colors
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public sealed class UIWaitingBar : UIControl
    {
        private readonly Timer timer;

        public UIWaitingBar()
        {
            SetStyleFlags(true, false);
            MinimumSize = new Size(70, 23);
            Size = new Size(300, 29);
            ShowText = false;

            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();

            fillColor = UIStyles.Blue.ProcessBarFillColor;
            foreColor = UIStyles.Blue.ProcessBarForeColor;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            timer?.Stop();
            timer?.Dispose();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Painting parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRoundRectangle(foreColor, (int)dz + 1, 1, sliderWidth, Height - 3, Radius);
        }

        // d is degrees, not radians
        private double d;

        private double dz;

        private int blockCount = 20;

        [DefaultValue(20)]
        [Description("Number of display blocks, the larger the number, the faster the movement"), Category("SunnyUI")]
        public int BlockCount
        {
            get => blockCount;
            set => blockCount = Math.Max(10, value);
        }

        public event EventHandler Tick;

        private void Timer_Tick(object sender, EventArgs e)
        {
            // The moving distance needs to subtract the width of the slider itself
            double dMoveDistance = Width - SliderWidth - 3;
            // The number of changes required
            double dStep = dMoveDistance / blockCount;
            // The degree added each time it changes
            double dPer = 180.0 / dStep;

            d += dPer;
            if (d > 360)
            {
                // A cycle is 360 degrees
                d = 0;
            }

            // Convert degrees to radians required by Math.Sin() using the formula: radians = degrees * π / 180
            dz = dMoveDistance * (1 + Math.Sin((d - 90) * Math.PI / 180)) / 2;

            Invalidate();

            Tick?.Invoke(this, e);
        }

        [DefaultValue(200)]
        [Description("Movement display interval"), Category("SunnyUI")]
        public int Interval
        {
            get => timer.Interval;
            set
            {
                timer.Stop();
                timer.Interval = Math.Max(50, value);
                timer.Start();
            }
        }

        private int sliderWidth = 60;

        [DefaultValue(60)]
        [Description("Slider width"), Category("SunnyUI")]
        public int SliderWidth
        {
            get => sliderWidth;
            set
            {
                sliderWidth = value;
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
            fillColor = uiColor.ProcessBarFillColor;
            foreColor = uiColor.ProcessBarForeColor;
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

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Foreground color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }
    }
}