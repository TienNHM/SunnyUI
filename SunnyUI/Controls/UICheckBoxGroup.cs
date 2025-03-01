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
 * File Name: UICheckBoxGroup.cs
 * Description: Checkbox group
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-04-19: V2.2.3 Added unit
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-07-03: V2.2.6 Fixed bug where adjusting ItemSize was ineffective
 * 2020-07-04: V2.2.6 Can set initial selected value
 * 2022-06-30: V3.2.0 Check if created before setting item state
 * 2022-11-21: V3.2.9 Fixed issue where node text was empty when not displayed
 * 2023-04-19: V3.3.5 Set selected item ForeColor
 * 2023-06-27: V3.3.9 Internal item associated value changed from Tag to TagString
 * 2023-11-07: V3.5.2 Rewritten UICheckBoxGroup
 * 2023-12-04: V3.6.1 Added property to modify icon size
 * 2024-09-09: V3.7.0 Changed method of calculating node position, resolved issue: #IAPY94
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
    /// <summary>
    /// Checkbox group
    /// </summary>
    [DefaultProperty("Items")]
    [DefaultEvent("ValueChanged")]
    public class UICheckBoxGroup : UIGroupBox
    {
        /// <summary>
        /// Value change event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="index">Index</param>
        /// <param name="text">Text</param>
        /// <param name="isChecked">Is checked</param>
        public delegate void OnValueChanged(object sender, CheckBoxGroupEventArgs e);

        /// <summary>
        /// Value change event
        /// </summary>
        public event OnValueChanged ValueChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public UICheckBoxGroup()
        {
            items.CountChange += Items_CountChange;
            ForeColor = UIStyles.Blue.CheckBoxForeColor;
            checkBoxColor = UIStyles.Blue.CheckBoxColor;
            hoverColor = UIStyles.Blue.ListItemHoverColor;
        }

        private Color checkBoxColor;

        private void Items_CountChange(object sender, EventArgs e)
        {
            InitRects();
            Invalidate();
        }

        /// <summary>
        /// Get and set item value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool this[int index]
        {
            get => GetItemCheckState(index);
            set
            {
                SetItemCheckState(index, value);
                ValueChanged?.Invoke(this, new CheckBoxGroupEventArgs(index, Items[index].ToString(), this[index], SelectedIndexes.ToArray()));
                Invalidate();
            }
        }

        private Dictionary<int, bool> CheckStates = new Dictionary<int, bool>();
        private Dictionary<int, Rectangle> CheckBoxRects = new Dictionary<int, Rectangle>();
        private int _imageSize = 16;

        [DefaultValue(16)]
        [Description("Icon size"), Category("SunnyUI")]
        [Browsable(true)]
        public int CheckBoxSize
        {
            get => _imageSize;
            set
            {
                _imageSize = Math.Max(value, 16);
                _imageSize = Math.Min(value, 64);
                Invalidate();
            }
        }

        /// <summary>
        /// Clear all items
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            CheckStates.Clear();
            CheckBoxRects.Clear();
            Invalidate();
            ValueChanged?.Invoke(this, new CheckBoxGroupEventArgs(-1, "", false, SelectedIndexes.ToArray()));
        }

        /// <summary>
        /// Item list
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [MergableProperty(false)]
        [Description("Get the collection of items in this checkbox group"), Category("SunnyUI")]
        public UIObjectCollection Items => items;

        private readonly UIObjectCollection items = new UIObjectCollection();

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

        private Color hoverColor;

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Mouse hover fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "155, 200, 255")]
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color CheckBoxColor
        {
            get => checkBoxColor;
            set
            {
                checkBoxColor = value;
                Invalidate();
            }
        }

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
        /// Override paint
        /// </summary>
        /// <param name="e">Paint event args</param>
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
                int ImageSize = CheckBoxSize;

                //Icon
                top = rect.Top + (rect.Height - ImageSize) / 2;
                left = rect.Left + 6;
                Color color = Enabled ? checkBoxColor : foreDisableColor;

                if (this[i])
                {
                    e.Graphics.FillRoundRectangle(color, new Rectangle((int)left, (int)top, ImageSize, ImageSize), 1);
                    color = BackColor.IsValid() ? BackColor : Color.White;
                    Point pt2 = new Point((int)(left + ImageSize * 2 / 5.0f), (int)(top + ImageSize * 3 / 4.0f) - (ImageSize.Div(10)));
                    Point pt1 = new Point((int)left + 2 + ImageSize.Div(10), pt2.Y - (pt2.X - 2 - ImageSize.Div(10) - (int)left));
                    Point pt3 = new Point((int)left + ImageSize - 2 - ImageSize.Div(10), pt2.Y - (ImageSize - pt2.X - 2 - ImageSize.Div(10)) - (int)left);

                    PointF[] CheckMarkLine = { pt1, pt2, pt3 };
                    using Pen pn = new Pen(color, 2);
                    e.Graphics.SetHighQuality();
                    e.Graphics.DrawLines(pn, CheckMarkLine);
                    e.Graphics.SetDefaultQuality();
                }
                else
                {
                    using Pen pn = new Pen(color, 1);
                    e.Graphics.DrawRoundRectangle(pn, new Rectangle((int)left + 1, (int)top + 1, ImageSize - 2, ImageSize - 2), 1);
                    e.Graphics.DrawRectangle(pn, new Rectangle((int)left + 2, (int)top + 2, ImageSize - 4, ImageSize - 4));
                }

                e.Graphics.DrawString(text, Font, ForeColor, rect, ContentAlignment.MiddleLeft, ImageSize + 10, 0);

            }
        }

        int activeIndex = -1;

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
                    this[pair.Key] = !this[pair.Key];
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Selected state list
        /// </summary>
        [Browsable(false)]
        public List<int> SelectedIndexes
        {
            get
            {
                List<int> indexes = new List<int>();
                for (int i = 0; i < Items.Count; i++)
                {
                    if (this[i]) indexes.Add(i);
                }

                return indexes;
            }
            set
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    SetItemCheckState(i, false);
                }

                foreach (int i in value)
                {
                    if (i >= 0 && i < Items.Count)
                    {
                        SetItemCheckState(i, true);
                    }
                }

                ValueChanged?.Invoke(this, new CheckBoxGroupEventArgs(-1, "", false, SelectedIndexes.ToArray()));
                Invalidate();
            }
        }

        /// <summary>
        /// Set item state
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="isChecked">Is checked</param>
        private void SetItemCheckState(int index, bool isChecked)
        {
            if (index >= 0 && index < Items.Count && CheckStates.NotContainsKey(index))
            {
                CheckStates.Add(index, isChecked);
            }

            CheckStates[index] = isChecked;
        }

        /// <summary>
        /// Get item state
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Is checked</returns>
        private bool GetItemCheckState(int index)
        {
            if (index >= 0 && index < items.Count && CheckStates.ContainsKey(index))
                return CheckStates[index];

            return false;
        }

        /// <summary>
        /// All selected item list
        /// </summary>
        [Browsable(false)]
        public List<object> SelectedItems
        {
            get
            {
                List<object> objects = new List<object>();

                for (int i = 0; i < Items.Count; i++)
                {
                    if (this[i]) objects.Add(Items[i]);
                }

                return objects;
            }
        }

        private int columnCount = 1;

        /// <summary>
        /// Number of columns displayed
        /// </summary>
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

        private Size _itemSize = new Size(150, 29);

        /// <summary>
        /// Size of displayed items
        /// </summary>
        [DefaultValue(typeof(Size), "150, 29")]
        [Description("Size of displayed items"), Category("SunnyUI")]
        public Size ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                InitRects();
                Invalidate();
            }
        }

        private Point startPos = new Point(12, 12);

        /// <summary>
        /// Starting position of displayed items
        /// </summary>
        [DefaultValue(typeof(Point), "12, 12")]
        [Description("Starting position of displayed items"), Category("SunnyUI")]
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


        private int columnInterval = 6;

        /// <summary>
        /// Interval between columns of displayed items
        /// </summary>
        [DefaultValue(6)]
        [Description("Interval between columns of displayed items"), Category("SunnyUI")]
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

        /// <summary>
        /// Interval between rows of displayed items
        /// </summary>
        [DefaultValue(2)]
        [Description("Interval between rows of displayed items"), Category("SunnyUI")]
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

        /// <summary>
        /// Select all
        /// </summary>
        public void SelectAll()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                SetItemCheckState(i, true);
            }

            ValueChanged?.Invoke(this, new CheckBoxGroupEventArgs(-1, "", false, SelectedIndexes.ToArray()));
            Invalidate();
        }

        /// <summary>
        /// Unselect all
        /// </summary>
        public void UnSelectAll()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                SetItemCheckState(i, false);
            }

            ValueChanged?.Invoke(this, new CheckBoxGroupEventArgs(-1, "", false, SelectedIndexes.ToArray()));
            Invalidate();
        }

        /// <summary>
        /// Reverse selection
        /// </summary>
        public void ReverseSelected()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                SetItemCheckState(i, !this[i]);
            }

            ValueChanged?.Invoke(this, new CheckBoxGroupEventArgs(-1, "", false, SelectedIndexes.ToArray()));
            Invalidate();
        }
    }

    public class CheckBoxGroupEventArgs : EventArgs
    {
        public int Index { get; set; }

        public string Text { get; set; }
        public bool Checked { get; set; }

        public int[] SelectedIndexes { get; set; }

        public CheckBoxGroupEventArgs(int index, string text, bool isChecked, int[] indexes)
        {
            Index = index;
            Text = text;
            Checked = isChecked;
            SelectedIndexes = indexes;
        }
    }
}