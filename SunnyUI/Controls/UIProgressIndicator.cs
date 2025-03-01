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
 * File Name: UIProgressIndicator.cs
 * Description: Progress Indicator
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-12-18: V3.3.0 Added Active property for dynamic display activation
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public sealed class UIProgressIndicator : UIControl
    {
        private readonly Timer timer;

        public UIProgressIndicator()
        {
            SetStyleFlags(true, false);
            Width = Height = 100;

            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += timer_Tick;

            ShowText = false;
            ShowRect = false;

            foreColor = UIStyles.Blue.ProgressIndicatorColor;
        }

        [Description("Activate dynamic display"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool Active
        {
            get => timer.Enabled;
            set => timer.Enabled = value;
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
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            foreColor = uiColor.ProgressIndicatorColor;
            ClearImage();
        }

        private int Index;

        private Image image;

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            int circleSize = Math.Min(Width, Height).Div(6);

            if (image == null)
            {
                image = new Bitmap(Width, Height);
                using Graphics ig = image.Graphics();
                for (int i = 0; i < 8; i++)
                {
                    Point pt = GetPoint(i, circleSize);
                    ig.FillEllipse(Color.FromArgb(192, foreColor), pt.X, pt.Y, circleSize, circleSize);
                }
            }

            g.DrawImage(image, 0, 0);

            int idx = Index.Mod(8);
            g.FillEllipse(foreColor, GetPoint(idx, circleSize).X, GetPoint(idx, circleSize).Y, circleSize, circleSize);

            idx = (Index + 8 - 1).Mod(8);
            g.FillEllipse(Color.FromArgb(224, foreColor), GetPoint(idx, circleSize).X, GetPoint(idx, circleSize).Y, circleSize, circleSize);
        }

        private Point GetPoint(int index, int circleSize)
        {
            int len = Math.Min(Width, Height) / 2 - 2 - circleSize;
            int lenX = (int)(len * 0.707);
            int centerX = Width / 2 - circleSize / 2;
            int centerY = Height / 2 - circleSize / 2;

            switch (index)
            {
                case 0: return new Point(centerX, centerY - len);
                case 1: return new Point(centerX + lenX, centerY - lenX);
                case 2: return new Point(centerX + len, centerY);
                case 3: return new Point(centerX + lenX, centerY + lenX);
                case 4: return new Point(centerX, centerY + len);
                case 5: return new Point(centerX - lenX, centerY + lenX);
                case 6: return new Point(centerX - len, centerY);
                case 7: return new Point(centerX - lenX, centerY - lenX);
            }

            return new Point(centerX, centerY);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
            Index++;
            Tick?.Invoke(this, e);
        }

        public event EventHandler Tick;

        private void ClearImage()
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ClearImage();
            Invalidate();
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
    }
}