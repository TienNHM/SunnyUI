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
 * File Name: UIRadioButtonGroup.cs
 * Description: Radio button group
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-04-19: V2.2.3 Added unit
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-07-03: V2.2.6 Fixed bug where adjusting ItemSize was ineffective
 * 2020-07-04: V2.2.6 Can set initial selected value
 * 2022-11-21: V3.2.9 Fixed issue where switching node text was empty when not displayed
 * 2023-04-22: V3.3.5 Set selected item ForeColor
 * 2023-06-27: V3.3.9 Changed built-in item associated value from Tag to TagString
 * 2023-11-09: V3.5.2 Rewritten UIRadioButtonGroup
 * 2023-12-04: V3.6.1 Added property to modify icon size
 * 2024-09-09: V3.7.0 Changed method of calculating node position, solved issue: #IAPY94
 * 2024-11-29: V3.8.0 Fixed issue where items were misaligned when TitleTop was 0 #IB7STO
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultProperty("Items")]
    [DefaultEvent("ValueChanged")]
    public class UIRadioButtonGroup : UIGroupBox
    {
        public delegate void OnValueChanged(object sender, int index, string text);

        public event OnValueChanged ValueChanged;

        public UIRadioButtonGroup()
        {
            items.CountChange += Items_CountChange;
            ForeColor = UIStyles.Blue.CheckBoxForeColor;
            checkBoxColor = UIStyles.Blue.CheckBoxColor;
            hoverColor = UIStyles.Blue.ListItemHoverColor;
        }

        private Color checkBoxColor;
        private Color hoverColor;

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            checkBoxColor = uiColor.CheckBoxColor;
            ForeColor = uiColor.CheckBoxForeColor;
            hoverColor = uiColor.ListItemHoverColor;
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RadioButtonColor
        {
            get => checkBoxColor;
            set
            {
                checkBoxColor = value;
                Invalidate();
            }
        }

        private void Items_CountChange(object sender, EventArgs e)
        {
            InitRects();
            Invalidate();
        }

        public void Clear()
        {
            Items.Clear();
            SelectedIndex = -1;
            Invalidate();
            ValueChanged(this, -1, "");
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [MergableProperty(false)]
        [Description("List items"), Category("SunnyUI")]
        public UIObjectCollection Items => items;

        private readonly UIObjectCollection items = new UIObjectCollection();

        private void InitRects()
        {
            int startX = StartPos.X;
            int startY = TitleTop + StartPos.Y;
            for (int i = 0; i < Items.Count; i++)
            {
                string text = Items[i].ToString();
                int rowIndex = i / ColumnCount;
                int columnIndex = i % ColumnCount;
                int left = startX + ItemSize.Width * columnIndex + ColumnInterval * columnIndex;
                int top = startY + ItemSize.Height * rowIndex + RowInterval * rowIndex;
                Rectangle rect = new Rectangle(left, top, ItemSize.Width, ItemSize.Height);
                if (CheckBoxRects.NotContainsKey(i))
                    CheckBoxRects.Add(i, rect);
                else
                    CheckBoxRects[i] = rect;
            }
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Painting parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (TitleTop == 0 && Text.IsValid()) Text = "";
            if (Items.Count == 0) return;
            InitRects();

            if (activeIndex >= 0 && CheckBoxRects.TryGetValue(activeIndex, out Rectangle boxRect))
            {
                e.Graphics.FillRectangle(hoverColor, boxRect);
            }

            int startX = StartPos.X;
            int startY = TitleTop + StartPos.Y;

            for (int i = 0; i < Items.Count; i++)
            {
                string text = Items[i].ToString();
                int rowIndex = i / ColumnCount;
                int columnIndex = i % ColumnCount;
                int left = startX + ItemSize.Width * columnIndex + ColumnInterval * columnIndex;
                int top = startY + ItemSize.Height * rowIndex + RowInterval * rowIndex;
                Rectangle rect = new Rectangle(left, top, ItemSize.Width, ItemSize.Height);
                int ImageSize = RadioButtonSize;

                //Icon
                top = rect.Top + (rect.Height - ImageSize) / 2;
                left = rect.Left + 6;
                Color color = Enabled ? checkBoxColor : foreDisableColor;

                if (SelectedIndex == i)
                {
                    e.Graphics.FillEllipse(color, left, top, ImageSize, ImageSize);
                    float pointSize = ImageSize - 4;
                    e.Graphics.FillEllipse(BackColor.IsValid() ? BackColor : Color.White,
                        left + ImageSize / 2.0f - pointSize / 2.0f,
                        top + ImageSize / 2.0f - pointSize / 2.0f,
                        pointSize, pointSize);

                    pointSize = ImageSize - 8;
                    e.Graphics.FillEllipse(color,
                        left + ImageSize / 2.0f - pointSize / 2.0f,
                        top + ImageSize / 2.0f - pointSize / 2.0f,
                        pointSize, pointSize);
                }
                else
                {
                    using Pen pn = new Pen(color, 2);
                    e.Graphics.SetHighQuality();
                    e.Graphics.DrawEllipse(pn, left + 1, top + 1, ImageSize - 2, ImageSize - 2);
                    e.Graphics.SetDefaultQuality();
                }

                e.Graphics.DrawString(text, Font, ForeColor, rect, ContentAlignment.MiddleLeft, ImageSize + 10, 0);

            }
        }

        private Dictionary<int, bool> CheckStates = new Dictionary<int, bool>();
        private Dictionary<int, Rectangle> CheckBoxRects = new Dictionary<int, Rectangle>();

        int activeIndex = -1;
        private int _imageSize = 16;

        [DefaultValue(16)]
        [Description("Icon size"), Category("SunnyUI")]
        [Browsable(true)]
        public int RadioButtonSize
        {
            get => _imageSize;
            set
            {
                _imageSize = Math.Max(value, 16);
                _imageSize = Math.Min(value, 64);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int index = -1;
            foreach (var item in CheckBoxRects)
            {
                if (e.Location.InRect(item.Value))
                {
                    index = item.Key;
                    break;
                }
            }

            if (activeIndex != index)
            {
                activeIndex = index;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            activeIndex = -1;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            foreach (var pair in CheckBoxRects)
            {
                if (e.Location.InRect(pair.Value) && pair.Key >= 0 && pair.Key < items.Count)
                {
                    SelectedIndex = pair.Key;
                    Invalidate();
                }
            }
        }

        private int selectedIndex = -1;

        [Browsable(false)]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (Items.Count == 0)
                {
                    selectedIndex = -1;
                    return;
                }

                if (SelectedIndex != value)
                {
                    selectedIndex = value;
                    Invalidate();
                    ValueChanged?.Invoke(this, value, items.ContainsIndex(value) ? items[value].ToString() : "");
                }
            }
        }

        private int columnCount = 1;

        [DefaultValue(1)]
        [Description("Number of columns displayed"), Category("SunnyUI")]
        public int ColumnCount
        {
            get => columnCount;
            set
            {
                columnCount = value;
                InitRects();
                Invalidate();
            }
        }

        private Size itemSize = new Size(150, 29);

        [DefaultValue(typeof(Size), "150, 29")]
        [Description("Item size"), Category("SunnyUI")]
        public Size ItemSize
        {
            get => itemSize;
            set
            {
                itemSize = value;
                InitRects();
                Invalidate();
            }
        }

        private Point startPos = new Point(12, 12);

        [DefaultValue(typeof(Point), "12, 12")]
        [Description("Starting position of items"), Category("SunnyUI")]
        public Point StartPos
        {
            get => startPos;
            set
            {
                startPos = value;
                InitRects();
                Invalidate();
            }
        }

        public int columnInterval = 6;

        [DefaultValue(6)]
        [Description("Column interval"), Category("SunnyUI")]
        public int ColumnInterval
        {
            get => columnInterval;
            set
            {
                columnInterval = value;
                InitRects();
                Invalidate();
            }
        }

        private int rowInterval = 2;

        [DefaultValue(2)]
        [Description("Row interval"), Category("SunnyUI")]
        public int RowInterval
        {
            get => rowInterval;
            set
            {
                rowInterval = value;
                InitRects();
                Invalidate();
            }
        }
    }
}