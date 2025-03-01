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
 * File Name: UIGroupBox.cs
 * Description: Group Box
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2022-05-30: V3.1.9 Fixed Padding setting
 * 2023-05-13: V3.3.6 Refactored DrawString function
 * 2023-07-11: V3.4.0 Solved the issue where a horizontal line appears under the title when BackColor and FillColor are set to transparent
 * 2023-07-19: V3.4.1 Solved the issue where text overlaps with the border line when BackColor and FillColor are set to transparent
 * 2024-03-22: V3.6.5 Fixed the drawing color of the title line when Enabled is false
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultProperty("Text")]
    public partial class UIGroupBox : UIPanel
    {
        public UIGroupBox()
        {
            InitializeComponent();
            TextAlignment = ContentAlignment.MiddleLeft;
            TextAlignmentChange += UIGroupBox_TextAlignmentChange;
            SetStyleFlags(true, false);
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        private void UIGroupBox_TextAlignmentChange(object sender, ContentAlignment alignment)
        {
            Invalidate();
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            g.Clear(FillColor);
        }

        /// <summary>
        /// Draw border color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintRect(Graphics g, GraphicsPath path)
        {
            if (RectSides == ToolStripStatusLabelBorderSides.None)
            {
                return;
            }

            var rect = new Rectangle(0, TitleTop, Width - 1, Height - _titleTop - 1);
            if (Text.IsValid())
            {
                using var path1 = rect.CreateRoundedRectanglePathWithoutTop(Radius, RadiusSides, RectSize);
                g.DrawPath(GetRectColor(), path1, true, RectSize);
            }
            else
            {
                using var path1 = rect.CreateRoundedRectanglePath(Radius, RadiusSides, RectSize);
                g.DrawPath(GetRectColor(), path1, true, RectSize);
            }
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            Size size = TextRenderer.MeasureText(Text, Font);
            g.DrawString(Text, Font, ForeColor, FillColor, new Rectangle(TitleInterval, TitleTop - size.Height / 2, Width - TitleInterval * 2, size.Height), TextAlignment);

            int textLeft = TitleInterval;
            switch (TextAlignment)
            {
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    textLeft = (Width - size.Width) / 2 - 1;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    textLeft = (Width - TitleInterval - size.Width) - 2;
                    break;
            }

            if (RectSides.GetValue(ToolStripStatusLabelBorderSides.Top))
            {
                if (RadiusSides.GetValue(UICornerRadiusSides.LeftTop) && !UIStyles.GlobalRectangle)
                {
                    g.DrawLine(GetRectColor(), Radius / 2 * RectSize, TitleTop, textLeft, TitleTop, true, RectSize);
                }
                else
                {
                    g.DrawLine(GetRectColor(), 0, TitleTop, textLeft, TitleTop, true, RectSize);
                }

                if (RadiusSides.GetValue(UICornerRadiusSides.RightTop) && !UIStyles.GlobalRectangle)
                {
                    g.DrawLine(GetRectColor(), textLeft + size.Width, TitleTop, Width - Radius / 2 * RectSize, TitleTop, true, RectSize);
                }
                else
                {
                    g.DrawLine(GetRectColor(), textLeft + size.Width, TitleTop, Width, TitleTop, true, RectSize);
                }
            }
        }

        private int _titleTop = 16;

        [DefaultValue(16)]
        [Description("Title height"), Category("SunnyUI")]
        public int TitleTop
        {
            get => _titleTop;
            set
            {
                if (_titleTop != value)
                {
                    _titleTop = value;
                    Padding = new Padding(Padding.Left, Math.Max(value + 16, Padding.Top), Padding.Right, Padding.Bottom);
                    Invalidate();
                }
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            if (Padding.Top != Math.Max(TitleTop + 16, Padding.Top))
            {
                Padding = new Padding(Padding.Left, Math.Max(TitleTop + 16, Padding.Top), Padding.Right, Padding.Bottom);
            }
        }

        private int _titleInterval = 10;

        [DefaultValue(10)]
        [Description("Title display interval"), Category("SunnyUI")]
        public int TitleInterval
        {
            get => _titleInterval;
            set
            {
                if (_titleInterval != value)
                {
                    _titleInterval = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(HorizontalAlignment.Left)]
        [Description("Text display position"), Category("SunnyUI")]
        [Browsable(false)]
        public HorizontalAlignment TitleAlignment { get; set; }
    }
}