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
 * File Name: UIMarkLabel.cs
 * Description: Label with color tag
 * Current Version: V3.1
 * Creation Date: 2021-03-07
 *
 * 2021-03-07: V3.0.2 Added file description
 * 2022-03-19: V3.1.1 Refactored theme colors
 ******************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public sealed class UIMarkLabel : UILabel
    {
        public UIMarkLabel()
        {
            Padding = new Padding(MarkSize + 2, 0, 0, 0);
            markColor = UIStyles.Blue.MarkLabelForeColor;
        }

        private bool autoSize;

        [Browsable(true)]
        [Description("Auto size"), Category("SunnyUI")]
        public override bool AutoSize
        {
            get => autoSize;
            set
            {
                autoSize = value;
                Invalidate();
            }
        }

        private int markSize = 3;

        [Description("Tag size"), Category("SunnyUI"), DefaultValue(3)]
        public int MarkSize
        {
            get => markSize;
            set
            {
                markSize = value;
                Invalidate();
            }
        }

        private UIMarkPos markPos = UIMarkPos.Left;

        [Description("Tag position"), Category("SunnyUI"), DefaultValue(UIMarkPos.Left)]
        public UIMarkPos MarkPos
        {
            get => markPos;
            set
            {
                markPos = value;

                switch (markPos)
                {
                    case UIMarkPos.Left: Padding = new Padding(MarkSize + 2, 0, 0, 0); break;
                    case UIMarkPos.Right: Padding = new Padding(0, 0, MarkSize + 2, 0); break;
                    case UIMarkPos.Top: Padding = new Padding(0, MarkSize + 2, 0, 0); break;
                    case UIMarkPos.Bottom: Padding = new Padding(0, 0, 0, MarkSize + 2); break;
                }

                Invalidate();
            }
        }

        public enum UIMarkPos
        {
            Left,
            Top,
            Right,
            Bottom
        }

        private Color markColor;

        [Description("Tag color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color MarkColor
        {
            get => markColor;
            set
            {
                markColor = value;
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
            markColor = uiColor.MarkLabelForeColor;
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Painting parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Size TextSize = TextRenderer.MeasureText(Text, Font);
            if (autoSize && Dock == DockStyle.None)
            {
                int width = (MarkPos == UIMarkPos.Left || MarkPos == UIMarkPos.Right) ?
                    TextSize.Width + MarkSize + 2 : TextSize.Width;
                int height = (MarkPos == UIMarkPos.Top || MarkPos == UIMarkPos.Bottom) ?
                    TextSize.Height + MarkSize + 2 : TextSize.Height;

                if (Width != width) Width = width;
                if (Height != height) Height = height;
            }

            switch (markPos)
            {
                case UIMarkPos.Left: e.Graphics.FillRectangle(MarkColor, 0, 0, MarkSize, Height); break;
                case UIMarkPos.Right: e.Graphics.FillRectangle(MarkColor, Width - MarkSize, 0, MarkSize, Height); break;
                case UIMarkPos.Top: e.Graphics.FillRectangle(MarkColor, 0, 0, Width, MarkSize); break;
                case UIMarkPos.Bottom: e.Graphics.FillRectangle(MarkColor, 0, Height - MarkSize, Width, MarkSize); break;
            }
        }
    }
}
