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
 * File Name: UIScrollingText.cs
 * File Description: Scrolling Text
 * Current Version: V3.1
 * Creation Date: 2020-06-29
 *
 * 2020-06-29: V2.2.6 Added control
 * 2021-07-16: V3.0.5 Added property to control scrolling
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-02-23: V3.3.2 Rewrote scrolling logic
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2024-12-02: V3.8.0 Can set default display position when scrolling stops
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public class UIScrollingText : UIControl
    {
        private readonly Timer timer;
        private int XPos = 0;
        private int XPos1 = 0;
        private int interval = 200;
        private int TextWidth = 0;

        public UIScrollingText()
        {
            SetStyleFlags(true, false);
            fillColor = UIStyles.Blue.ScrollingTextFillColor;
            foreColor = UIStyles.Blue.ScrollingTextForeColor;
            Reset();

            timer = new Timer();
            timer.Interval = interval;
            timer.Tick += Timer_Tick;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        [DefaultValue(false), Description("Whether to scroll"), Category("SunnyUI")]
        public bool Active
        {
            get => timer.Enabled;
            set
            {
                timer.Enabled = value;
                if (!value)
                {
                    Reset();
                }
            }
        }

        [Browsable(false), DefaultValue(false), Description("Click to pause scrolling"), Category("SunnyUI")]
        public bool ClickPause
        {
            get; set;
        }

        private void Reset()
        {
            XPos = 0;
            XPos1 = 0;
            Invalidate();
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

        [DefaultValue(200), Description("Refresh interval"), Category("SunnyUI")]
        public int Interval
        {
            get => interval;
            set
            {
                interval = Math.Max(value, 50);
                timer.Stop();
                timer.Interval = interval;
                timer.Start();
            }
        }

        private int offset = 10;

        [DefaultValue(10), Description("Offset"), Category("SunnyUI")]
        public int Offset
        {
            get => offset;
            set => offset = Math.Max(2, value);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ScrollingType == UIScrollingType.RightToLeft)
            {
                XPos -= Offset;
                if (XPos + TextWidth < 0)
                {
                    XPos = XPos1;
                    XPos -= Offset;
                }
            }

            if (ScrollingType == UIScrollingType.LeftToRight)
            {
                XPos += Offset;
                if (XPos > Width)
                {
                    XPos = XPos1;
                    XPos += Offset;
                }
            }

            Invalidate();
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            if (Active)
            {
                Size sf = TextRenderer.MeasureText(Text, Font);
                if (TextWidth != sf.Width)
                {
                    XPos = 0;
                    TextWidth = sf.Width;
                }

                if (ScrollingType == UIScrollingType.LeftToRight)
                {
                    if (XPos + TextWidth > Width && TextWidth < Width - offset)
                    {
                        XPos1 = XPos - Width + offset;
                        g.DrawString(Text, Font, ForeColor, new Rectangle(XPos1, 0, Width, Height), ContentAlignment.MiddleLeft);
                    }
                    else
                    {
                        XPos1 = -TextWidth + offset;
                    }

                    g.DrawString(Text, Font, ForeColor, new Rectangle(XPos, 0, Width, Height), ContentAlignment.MiddleLeft);
                }

                if (ScrollingType == UIScrollingType.RightToLeft)
                {
                    if (XPos < 0 && TextWidth < Width - offset)
                    {
                        XPos1 = Width + XPos - offset;
                        g.DrawString(Text, Font, ForeColor, new Rectangle(XPos1, 0, Width, Height), ContentAlignment.MiddleLeft);
                    }
                    else
                    {
                        XPos1 = Width - offset;
                    }

                    g.DrawString(Text, Font, ForeColor, new Rectangle(XPos, 0, Width, Height), ContentAlignment.MiddleLeft);
                }
            }
            else
            {
                base.OnPaintFore(g, path);
            }
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            Reset();
            base.OnSizeChanged(e);
        }

        private UIScrollingType scrollingType;

        [DefaultValue(UIScrollingType.RightToLeft), Description("Scrolling direction"), Category("SunnyUI")]
        public UIScrollingType ScrollingType
        {
            get => scrollingType;
            set
            {
                scrollingType = value;
                Reset();
                Invalidate();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Reset();
            base.OnTextChanged(e);
        }

        public enum UIScrollingType
        {
            RightToLeft,
            LeftToRight
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            fillColor = uiColor.ScrollingTextFillColor;
            foreColor = uiColor.ScrollingTextForeColor;
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or null, it will not be filled
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

        [DefaultValue(typeof(Color), "244, 244, 244")]
        [Description("Fill color when disabled"), Category("SunnyUI")]
        public Color FillDisableColor
        {
            get => fillDisableColor;
            set => SetFillDisableColor(value);
        }

        [DefaultValue(typeof(Color), "173, 178, 181")]
        [Description("Border color when disabled"), Category("SunnyUI")]
        public Color RectDisableColor
        {
            get => rectDisableColor;
            set => SetRectDisableColor(value);
        }

        [DefaultValue(typeof(Color), "109, 109, 103")]
        [Description("Font color when disabled"), Category("SunnyUI")]
        public Color ForeDisableColor
        {
            get => foreDisableColor;
            set => SetForeDisableColor(value);
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