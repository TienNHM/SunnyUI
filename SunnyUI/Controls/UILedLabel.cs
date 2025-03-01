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
 * File name: UILedLabel.cs
 * File description: LED label
 * Current version: V3.1
 * Creation date: 2021-04-11
 *
 * 2021-04-11: V3.0.2 Added file description
 * 2022-03-19: V3.1.1 Refactored theme colors
******************************************************************************/

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public class UILedLabel : UIControl
    {
        public UILedLabel()
        {
            SetStyleFlags(true, false);
            ShowText = ShowRect = ShowFill = false;
            foreColor = UIStyles.Blue.LedLabelForeColor;
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Painting parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int width = CharCount * IntervalOn * 5 +
                        CharCount * IntervalIn * 4 + (CharCount + 1) * IntervalOn + CharCount * IntervalIn;
            int height = IntervalOn * 7 + IntervalIn * 6;

            float left = 0;
            float top = 0;
            switch (TextAlign)
            {
                case ContentAlignment.TopLeft:
                    left = 0;
                    top = 0;
                    break;

                case ContentAlignment.TopCenter:
                    left = (Width - width) / 2.0f;
                    top = 0;
                    break;

                case ContentAlignment.TopRight:
                    left = Width - width;
                    top = 0;
                    break;

                case ContentAlignment.MiddleLeft:
                    left = 0;
                    top = (Height - height) / 2.0f;
                    break;

                case ContentAlignment.MiddleCenter:
                    left = (Width - width) / 2.0f;
                    top = (Height - height) / 2.0f;
                    break;

                case ContentAlignment.MiddleRight:
                    left = Width - width;
                    top = (Height - height) / 2.0f;
                    break;

                case ContentAlignment.BottomLeft:
                    left = 0;
                    top = Height - height;
                    break;

                case ContentAlignment.BottomCenter:
                    left = (Width - width) / 2.0f;
                    top = Height - height;
                    break;

                case ContentAlignment.BottomRight:
                    left = Width - width;
                    top = Height - height;
                    break;
            }

            int idx = 0;
            foreach (char c in Text)
            {
                float charStart = left + (IntervalOn + IntervalIn) * 6 * idx;
                byte[] bts = UILedChars.Chars.ContainsKey(c) ? UILedChars.Chars[c] : UILedChars.Chars[' '];
                for (int i = 0; i < 5; i++)
                {
                    byte bt = bts[i];
                    float btStart = charStart + (IntervalOn + IntervalIn) * i;
                    BitArray array = new BitArray(new[] { bt });
                    for (int j = 0; j < 7; j++)
                    {
                        bool bon = array[7 - j];
                        if (bon)
                        {
                            e.Graphics.FillRectangle(
                                ForeColor,
                                btStart,
                                 top + (IntervalOn + IntervalIn) * j,
                                IntervalOn,
                                IntervalOn);
                        }
                    }
                }

                idx++;
            }
        }

        private int intervalIn = 1;
        private int intervalOn = 2;

        private int CharCount => Text.Length;

        /// <summary>
        /// LED block spacing
        /// </summary>
        [DefaultValue(1), Description("LED block spacing"), Category("SunnyUI")]
        public int IntervalIn
        {
            get => intervalIn;
            set
            {
                if (intervalIn != value)
                {
                    intervalIn = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// LED block size
        /// </summary>
        [DefaultValue(2), Description("LED block size"), Category("SunnyUI")]
        public int IntervalOn
        {
            get => intervalOn;
            set
            {
                if (intervalOn != value)
                {
                    intervalOn = value;
                    Invalidate();
                }
            }
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

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            foreColor = uiColor.LedLabelForeColor;
        }
    }
}
