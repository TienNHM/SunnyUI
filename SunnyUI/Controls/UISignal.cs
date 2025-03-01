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
 * File Name: UISignal.cs
 * Description: Signal strength display
 * Current Version: V3.1
 * Creation Date: 2021-06-19
 *
 * 2021-06-19: V3.0.4 Added file description
 * 2021-06-20: V3.0.4 Adjusted default display height
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("Click")]
    [DefaultProperty("Level")]
    [ToolboxItem(true)]
    public class UISignal : UIControl
    {
        public UISignal()
        {
            SetStyleFlags(true, false);
            Width = Height = 35;
            ShowText = ShowRect = false;
        }

        private int lineWidth = 3;

        [DefaultValue(3)]
        [Description("Line Width"), Category("SunnyUI")]
        public int LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = value;
                Invalidate();
            }
        }

        private int lineInterval = 2;

        [DefaultValue(2)]
        [Description("Line Interval"), Category("SunnyUI")]
        public int LineInterval
        {
            get => lineInterval;
            set
            {
                lineInterval = value;
                Invalidate();
            }
        }

        private int lineHeight = 4;

        [DefaultValue(4)]
        [Description("Line Height"), Category("SunnyUI")]
        public int LineHeight
        {
            get => lineHeight;
            set
            {
                lineHeight = value;
                Invalidate();
            }
        }

        private int level = 5;

        [DefaultValue(5)]
        [Description("Level"), Category("SunnyUI")]
        public int Level
        {
            get => level;
            set
            {
                level = Math.Max(0, value);
                level = Math.Min(5, level);
                Invalidate();
            }
        }

        private Color onColor = Color.FromArgb(80, 160, 255);

        [DefaultValue(typeof(Color), "80, 160, 255")]
        [Description("On Signal Color"), Category("SunnyUI")]
        public Color OnColor
        {
            get => onColor;
            set
            {
                onColor = value;
                Invalidate();
            }
        }

        private Color offColor = Color.Silver;

        [DefaultValue(typeof(Color), "Silver")]
        [Description("Off Signal Color"), Category("SunnyUI")]
        public Color OffColor
        {
            get => offColor;
            set
            {
                offColor = value;
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
            //
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int left = (Width - lineWidth * 5 - lineInterval * 4) / 2;
            int top = (Height - lineHeight * 5) / 2;
            int bottom = top + lineHeight * 5;

            for (int i = 1; i <= 5; i++)
            {
                Color color = level >= i ? onColor : offColor;
                top = bottom - lineHeight * i;
                e.Graphics.FillRectangle(color, left, top, lineWidth, lineHeight * i);
                left += lineWidth + lineInterval;
            }
        }
    }
}
