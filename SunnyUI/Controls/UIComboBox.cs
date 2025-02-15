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
 * File Name: UIComboBox.cs
 * File Description: ComboBox
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-06-11: V2.2.5 Added DataSource, supports data binding
 * 2021-05-06: V3.0.3 Fixed issue where mouse dropdown selection triggers SelectedIndexChanged twice
 * 2021-06-03: V3.0.4 Updated data binding related code
 * 2021-08-03: V3.0.5 Clear display after Items.Clear
 * 2021-08-15: V3.0.6 Rewrote watermark text drawing method and added watermark text color
 * 2022-01-16: V3.1.0 Added dropdown color settings
 * 2022-04-13: V3.1.3 Automatically select SelectIndex based on Text
 * 2022-04-15: V3.1.3 Added filtering
 * 2022-04-16: V3.1.3 Filtering dropdown follows theme colors
 * 2022-04-20: V3.1.5 Show all data list when filter text is empty
 * 2022-05-04: V3.1.8 Fixed display of ValueMember bound value during filtering
 * 2022-05-24: V3.1.9 Clear text when Selceted=-1
 * 2022-08-25: V3.2.3 Dropdown border color can be set
 * 2022-11-03: V3.2.6 Remove leading and trailing spaces when filtering
 * 2022-11-13: V3.2.8 Added auto-adjust dropdown width when filtering is not displayed
 * 2022-11-30: V3.3.0 Added Clear method
 * 2023-02-04: V3.3.1 Added clear button
 * 2023-03-15: V3.3.3 Automatically close filter dropdown when losing focus
 * 2023-06-28: V3.3.9 Ignore case when filtering
 * 2023-07-03: V3.3.9 Modified the release of several objects
 * 2023-08-11: V3.4.1 Do not clear Text when DropDownStyle is DropDown after Items.Clear
 * 2023-12-26: V3.6.2 Added scrollbar settings for dropdown interface
 * 2024-01-27: V3.6.3 Fixed error when setting SelectedIndex in form constructor
 * 2024-10-28: V3.7.2 Added SelectionChangeCommitted event, responds when dropdown shows mouse click item
 * 2024-11-10: V3.7.2 Added StyleDropDown property, set this property to change dropdown theme when manually modifying Style
 * 2024-11-10: V3.7.2 Removed ScrollBarColor, ScrollBarBackColor, ScrollBarStyleInherited properties
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
    /// ComboBox
    /// </summary>
    [DefaultProperty("Items")]
    [DefaultEvent("SelectedIndexChanged")]
    [ToolboxItem(true)]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
    public sealed partial class UIComboBox : UIDropControl, IToolTip, IHideDropDown
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UIComboBox()
        {
            InitializeComponent();
            ListBox.SelectedIndexChanged += Box_SelectedIndexChanged;
            ListBox.ValueMemberChanged += Box_ValueMemberChanged;
            ListBox.SelectedValueChanged += ListBox_SelectedValueChanged;
            ListBox.ItemsClear += ListBox_ItemsClear;
            ListBox.ItemsRemove += ListBox_ItemsRemove;
            ListBox.MouseClick += ListBox_MouseClick;

            filterForm.BeforeListClick += ListBox_Click;

            edit.TextChanged += Edit_TextChanged;
            edit.KeyDown += Edit_KeyDown;
            DropDownWidth = 150;
            fullControlSelect = true;

            CreateInstance();
        }

        private void ListBox_MouseClick(object sender, MouseEventArgs e)
        {
            //SelectionChangeCommitted
            UIListBox listBox = (UIListBox)sender;
            int index = listBox.IndexFromPoint(e.X, e.Y);
            if (index != -1)
            {
                SelectionChangeCommitted?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectionChangeCommitted;

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        [DefaultValue(0), Category("SunnyUI"), Description("Vertical scrollbar width, minimum is the native scrollbar width")]
        public int ScrollBarWidth
        {
            get => ListBox.ScrollBarWidth;
            set => ListBox.ScrollBarWidth = value;
        }

        [DefaultValue(6), Category("SunnyUI"), Description("Vertical scrollbar handle width, minimum is the native scrollbar width")]
        public int ScrollBarHandleWidth
        {
            get => ListBox.ScrollBarHandleWidth;
            set => ListBox.ScrollBarHandleWidth = value;
        }

        [DefaultValue(false)]
        [Description("Show clear button"), Category("SunnyUI")]
        public bool ShowClearButton
        {
            get => showClearButton;
            set => showClearButton = value;
        }

        public override void Clear()
        {
            base.Clear();
            if (DataSource != null)
            {
                DataSource = null;
            }
            else
            {
                ListBox.Items.Clear();
            }
        }

        private void ListBox_Click(object sender, EventArgs e)
        {
            SelectTextChange = true;
            filterSelectedItem = filterList[(int)sender];
            filterSelectedValue = GetItemValue(filterSelectedItem);
            Text = GetItemText(filterSelectedItem).ToString();
            edit.SelectionStart = Text.Length;
            SelectedValueChanged?.Invoke(this, EventArgs.Empty);
            SelectTextChange = false;
        }

        private void ShowDropDownFilter()
        {
            if (Text.IsNullOrEmpty() && ShowFilter)
                FillFilterTextEmpty();

            FilterItemForm.AutoClose = false;
            if (!FilterItemForm.Visible)
            {
                filterForm.Style = StyleDropDown;
                if (StyleDropDown != UIStyle.Inherited) filterForm.Style = StyleDropDown;
                FilterItemForm.Show(this, new Size(DropDownWidth < Width ? Width : DropDownWidth, CalcItemFormHeight()));
                edit.Focus();
            }
        }

        private void Edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (ShowFilter)
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    if (!FilterItemForm.Visible)
                        ShowDropDownFilter();
                    int cnt = filterForm.ListBox.Items.Count;
                    int idx = filterForm.ListBox.SelectedIndex;

                    if (cnt > 0)
                    {
                        if (e.KeyCode == Keys.Down)
                        {
                            if (idx < cnt - 1)
                                filterForm.ListBox.SelectedIndex++;
                        }

                        if (e.KeyCode == Keys.Up)
                        {
                            if (idx > 0)
                                filterForm.ListBox.SelectedIndex--;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    FilterItemForm.Close();
                }
                else if (e.KeyCode == Keys.Return)
                {
                    if (FilterItemForm.Visible)
                    {
                        int cnt = filterForm.ListBox.Items.Count;
                        int idx = filterForm.ListBox.SelectedIndex;

                        if (cnt > 0 && idx >= 0 && idx < cnt)
                        {
                            SelectTextChange = true;
                            filterSelectedItem = filterList[idx];
                            filterSelectedValue = GetItemValue(filterSelectedItem);
                            Text = GetItemText(filterSelectedItem).ToString();
                            edit.SelectionStart = Text.Length;
                            SelectedValueChanged?.Invoke(this, EventArgs.Empty);
                            SelectTextChange = false;
                        }

                        FilterItemForm.Close();
                    }
                    else
                    {
                        ShowDropDownFilter();
                    }
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ShowDropDown();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    ItemForm.Close();
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
        }

        private object filterSelectedItem;
        private object filterSelectedValue;
        private bool showFilter;

        /// <summary>
        /// Show filter
        /// </summary>
        [DefaultValue(false)]
        [Description("Show filter"), Category("SunnyUI")]
        public bool ShowFilter
        {
            get => showFilter;
            set
            {
                showFilter = value;
                if (value)
                {
                    DropDownStyle = UIDropDownStyle.DropDown;
                }
            }
        }

        /// <summary>
        /// Maximum number of items to display in the filter
        /// </summary>
        [DefaultValue(100)]
        [Description("Maximum number of items to display in the filter"), Category("SunnyUI")]
        public int FilterMaxCount { get; set; } = 100;

        /// <summary>
        /// Dropdown state change event
        /// </summary>
        protected override void DropDownStyleChanged()
        {
            if (DropDownStyle == UIDropDownStyle.DropDownList)
            {
                showFilter = false;
            }
        }

        CurrencyManager dataManager;

        private void SetDataConnection()
        {
            if (!ShowFilter) return;

            if (DropDownStyle == UIDropDownStyle.DropDown && DataSource != null && DisplayMember.IsValid())
            {
                dataManager = (CurrencyManager)BindingContext[DataSource, new BindingMemberInfo(DisplayMember).BindingPath];
            }
        }

        private object GetItemValue(object item)
        {
            if (dataManager == null)
                return item;

            if (ValueMember.IsNullOrWhiteSpace())
                return null;

            PropertyDescriptor descriptor = dataManager.GetItemProperties().Find(ValueMemberBindingMemberInfo.BindingField, true);
            if (descriptor != null)
            {
                return descriptor.GetValue(item);
            }

            return null;
        }

        /// <summary>
        /// Controls that need additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        [DefaultValue(false)]
        public bool Sorted
        {
            get => ListBox.Sorted;
            set => ListBox.Sorted = value;
        }

        public int FindString(string s)
        {
            return ListBox.FindString(s);
        }

        public int FindString(string s, int startIndex)
        {
            return ListBox.FindString(s, startIndex);
        }

        public int FindStringExact(string s)
        {
            return ListBox.FindStringExact(s);
        }

        public int FindStringExact(string s, int startIndex)
        {
            return ListBox.FindStringExact(s, startIndex);
        }

        private void ListBox_ItemsRemove(object sender, EventArgs e)
        {
            if (ListBox.Count == 0 && DropDownStyle == UIDropDownStyle.DropDownList)
            {
                Text = "";
                edit.Text = "";
            }
        }

        private void ListBox_ItemsClear(object sender, EventArgs e)
        {
            if (DropDownStyle == UIDropDownStyle.DropDownList)
            {
                Text = "";
                edit.Text = "";
            }
        }

        public new event EventHandler TextChanged;

        private void Edit_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(this, e);

            if (!ShowFilter)
            {
                if (SelectTextChange) return;
                if (Text.IsValid())
                {
                    ListBox.ListBox.Text = Text;
                }
                else
                {
                    SelectTextChange = true;
                    SelectedIndex = -1;
                    edit.Text = "";
                    SelectTextChange = false;
                }
            }
            else
            {
                if (DropDownStyle == UIDropDownStyle.DropDownList) return;
                if (edit.Focused && Text.IsValid())
                {
                    ShowDropDownFilter();
                }

                if (Text.IsValid())
                {
                    string filterText = Text;
                    if (TrimFilter)
                        filterText = filterText.Trim();

                    filterForm.ListBox.Items.Clear();
                    filterList.Clear();

                    if (DataSource == null)
                    {
                        foreach (var item in Items)
                        {
                            if (FilterIgnoreCase)
                            {
                                if (item.ToString().ToUpper().Contains(filterText.ToUpper()))
                                {
                                    filterList.Add(item.ToString());
                                    if (filterList.Count > FilterMaxCount) break;
                                }
                            }
                            else
                            {
                                if (item.ToString().Contains(filterText))
                                {
                                    filterList.Add(item.ToString());
                                    if (filterList.Count > FilterMaxCount) break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dataManager != null)
                        {
                            for (int i = 0; i < Items.Count; i++)
                            {
                                if (FilterIgnoreCase)
                                {
                                    if (GetItemText(dataManager.List[i]).ToString().ToUpper().Contains(filterText.ToUpper()))
                                    {
                                        filterList.Add(dataManager.List[i]);
                                        if (filterList.Count > FilterMaxCount) break;
                                    }
                                }
                                else
                                {
                                    if (GetItemText(dataManager.List[i]).ToString().Contains(filterText))
                                    {
                                        filterList.Add(dataManager.List[i]);
                                        if (filterList.Count > FilterMaxCount) break;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var item in filterList)
                    {
                        filterForm.ListBox.Items.Add(GetItemText(item));
                    }
                }
                else
                {
                    FillFilterTextEmpty();
                    filterSelectedItem = null;
                    filterSelectedValue = null;
                    SelectedValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false)]
        [Description("Remove leading and trailing spaces when filtering"), Category("SunnyUI")]
        public bool TrimFilter { get; set; }

        [DefaultValue(false)]
        [Description("Ignore case when filtering"), Category("SunnyUI")]
        public bool FilterIgnoreCase { get; set; }

        private void FillFilterTextEmpty()
        {
            filterForm.ListBox.Items.Clear();
            filterList.Clear();

            if (DataSource == null)
            {
                foreach (var item in Items)
                {
                    filterList.Add(item.ToString());
                }
            }
            else
            {
                if (dataManager != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        filterList.Add(dataManager.List[i]);
                    }
                }
            }

            foreach (var item in filterList)
            {
                filterForm.ListBox.Items.Add(GetItemText(item));
            }
        }

        List<object> filterList = new List<object>();

        private void ListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!ShowFilter)
                SelectedValueChanged?.Invoke(this, e);
        }

        private void Box_ValueMemberChanged(object sender, EventArgs e)
        {
            ValueMemberChanged?.Invoke(this, e);
        }

        private void Box_DisplayMemberChanged(object sender, EventArgs e)
        {
            DisplayMemberChanged?.Invoke(this, e);
            SetDataConnection();
        }

        private void Box_DataSourceChanged(object sender, EventArgs e)
        {
            DataSourceChanged?.Invoke(this, e);
            SetDataConnection();
        }

        private bool SelectTextChange;

        private void Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTextChange = true;
            if (!ShowFilter)
            {
                if (ListBox.SelectedItem != null)
                    Text = ListBox.GetItemText(ListBox.SelectedItem);
                else
                    Text = "";
            }

            SelectTextChange = false;

            if (!Wana_1)
                SelectedIndexChanged?.Invoke(this, e);
        }

        public event EventHandler SelectedIndexChanged;

        public event EventHandler DataSourceChanged;

        public event EventHandler DisplayMemberChanged;

        public event EventHandler ValueMemberChanged;

        public event EventHandler SelectedValueChanged;

        /// <summary>
        /// Value change event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Value</param>
        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            Invalidate();
        }

        private readonly UIComboBoxItem dropForm = new UIComboBoxItem();
        private readonly UIComboBoxItem filterForm = new UIComboBoxItem();

        private UIDropDown filterItemForm;

        private UIDropDown FilterItemForm
        {
            get
            {
                if (filterItemForm == null)
                {
                    filterItemForm = new UIDropDown(filterForm);

                    if (filterItemForm != null)
                    {
                        filterItemForm.VisibleChanged += FilterItemForm_VisibleChanged;
                    }
                }

                return filterItemForm;
            }
        }

        private void FilterItemForm_VisibleChanged(object sender, EventArgs e)
        {
            dropSymbol = SymbolNormal;
            if (filterItemForm.Visible)
            {
                dropSymbol = SymbolDropDown;
            }

            Invalidate();
        }

        /// <summary>
        /// Create instance
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(dropForm);
        }

        protected override int CalcItemFormHeight()
        {
            int interval = ItemForm.Height - ItemForm.ClientRectangle.Height;
            return 4 + Math.Min(ListBox.Items.Count, MaxDropDownItems) * ItemHeight + interval;
        }

        private UIListBox ListBox
        {
            get => dropForm.ListBox;
        }

        private UIListBox FilterListBox
        {
            get => filterForm.ListBox;
        }

        [DefaultValue(25)]
        [Description("Item height"), Category("SunnyUI")]
        public int ItemHeight
        {
            get => ListBox.ItemHeight;
            set => FilterListBox.ItemHeight = ListBox.ItemHeight = value;
        }

        [DefaultValue(8)]
        [Description("Maximum number of items in the dropdown list"), Category("SunnyUI")]
        public int MaxDropDownItems { get; set; } = 8;

        private void UIComboBox_FontChanged(object sender, EventArgs e)
        {
            if (ItemForm != null)
            {
                ListBox.Font = Font;
            }

            if (filterForm != null)
            {
                filterForm.ListBox.Font = Font;
            }
        }

        public void ShowDropDown()
        {
            UIComboBox_ButtonClick(this, EventArgs.Empty);
        }

        public void HideDropDown()
        {
            try
            {
                if (!ShowFilter)
                {
                    if (ItemForm != null && ItemForm.Visible)
                        ItemForm.Close();
                }
                else
                {
                    if (FilterItemForm != null && FilterItemForm.Visible)
                        FilterItemForm.Close();
                }
            }
            catch
            {
            }
        }

        [DefaultValue(false)]
        [Description("Auto-adjust dropdown width when filtering is not displayed"), Category("SunnyUI")]
        public bool DropDownAutoWidth { get; set; }

        private void UIComboBox_ButtonClick(object sender, EventArgs e)
        {
            if (NeedDrawClearButton)
            {
                NeedDrawClearButton = false;
                Text = "";

                if (!showFilter)
                {
                    while (dropForm.ListBox.SelectedIndex != -1)
                        dropForm.ListBox.SelectedIndex = -1;
                }
                else
                {
                    while (filterForm.ListBox.SelectedIndex != -1)
                        filterForm.ListBox.SelectedIndex = -1;
                }

                Invalidate();
                return;
            }

            if (!ShowFilter)
            {
                if (Items.Count > 0)
                {
                    int dropWidth = Width;

                    if (DropDownAutoWidth)
                    {
                        if (DataSource == null)
                        {
                            for (int i = 0; i < Items.Count; i++)
                            {
                                Size sf = TextRenderer.MeasureText(Items[i].ToString(), Font);
                                dropWidth = Math.Max((int)sf.Width + ScrollBarInfo.VerticalScrollBarWidth() + 6, dropWidth);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Items.Count; i++)
                            {
                                Size sf = TextRenderer.MeasureText(dropForm.ListBox.GetItemText(Items[i]), Font);
                                dropWidth = Math.Max((int)sf.Width + ScrollBarInfo.VerticalScrollBarWidth() + 6, dropWidth);
                            }
                        }
                    }
                    else
                    {
                        dropWidth = Math.Max(DropDownWidth, dropWidth);
                    }

                    if (StyleDropDown != UIStyle.Inherited) dropForm.Style = StyleDropDown;
                    ItemForm.Show(this, new Size(dropWidth, CalcItemFormHeight()));
                }
            }
            else
            {
                if (FilterItemForm.Visible)
                {
                    FilterItemForm.Close();
                }
                else
                {
                    ShowDropDownFilter();
                }
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            ListBox.SetStyleColor(uiColor.DropDownStyle);
            FilterListBox.SetStyleColor(uiColor.DropDownStyle);
        }

        public object DataSource
        {
            get => ListBox.DataSource;
            set
            {
                ListBox.DataSource = value;
                Box_DataSourceChanged(this, EventArgs.Empty);
            }
        }

        [DefaultValue(150)]
        [Description("Dropdown width"), Category("SunnyUI")]
        public int DropDownWidth { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [MergableProperty(false)]
        [Description("Items"), Category("SunnyUI")]
        public ListBox.ObjectCollection Items => ListBox.Items;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Selected index"), Category("SunnyUI")]
        public int SelectedIndex
        {
            get => ShowFilter ? -1 : ListBox.SelectedIndex;
            set
            {
                if (!ShowFilter)
                {
                    if (DataSource != null && value == -1 && SelectedIndex > 0)
                    {
                        Wana_1 = true;
                        SelectedIndex = 0;
                        Wana_1 = false;
                    }

                    ListBox.SelectedIndex = value;
                }
            }
        }

        private bool Wana_1;

        [Browsable(false), Bindable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Selected item"), Category("SunnyUI")]
        public object SelectedItem
        {
            get => ShowFilter ? filterSelectedItem : ListBox.SelectedItem;
            set
            {
                if (!ShowFilter)
                {
                    ListBox.SelectedItem = value;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Selected text"), Category("SunnyUI")]
        public string SelectedText
        {
            get
            {
                if (DropDownStyle == UIDropDownStyle.DropDown)
                {
                    return edit.SelectedText;
                }
                else
                {
                    return Text;
                }
            }
        }

        public override void ResetText()
        {
            Clear();
        }

        [Description("Gets or sets the property to display for this list box."), Category("SunnyUI")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayMember
        {
            get => ListBox.DisplayMember;
            set
            {
                ListBox.DisplayMember = value;
                Box_DisplayMemberChanged(this, EventArgs.Empty);
            }
        }

        [Description("Gets or sets the format-specifier characters that indicate how a value is to be displayed."), Category("SunnyUI")]
        [DefaultValue("")]
        [MergableProperty(false)]
        public string FormatString
        {
            get => ListBox.FormatString;
            set => FilterListBox.FormatString = ListBox.FormatString = value;
        }

        [Description("Gets or sets a value indicating whether formatting is applied to the display value."), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool FormattingEnabled
        {
            get => ListBox.FormattingEnabled;
            set => FilterListBox.FormattingEnabled = ListBox.FormattingEnabled = value;
        }

        [Description("Gets or sets the property to use as the actual value for the items in the list."), Category("SunnyUI")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get => ListBox.ValueMember;
            set
            {
                ListBox.ValueMember = value;
                ValueMemberBindingMemberInfo = new BindingMemberInfo(value);
            }
        }

        BindingMemberInfo ValueMemberBindingMemberInfo;

        [
            DefaultValue(null),
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Bindable(true)
        ]
        public object SelectedValue
        {
            get => ShowFilter ? filterSelectedValue : ListBox.SelectedValue;
            set
            {
                if (!ShowFilter)
                    ListBox.SelectedValue = value;
            }
        }

        public string GetItemText(object item)
        {
            return ListBox.GetItemText(item);
        }

        private void UIComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !ShowFilter)
            {
                ShowDropDown();
            }
        }

        private void edit_Leave(object sender, EventArgs e)
        {
            HideDropDown();
        }

        [DefaultValue(typeof(Color), "White")]
        public Color ItemFillColor
        {
            get => ListBox.FillColor;
            set => FilterListBox.FillColor = ListBox.FillColor = value;
        }

        [DefaultValue(typeof(Color), "48, 48, 48")]
        public Color ItemForeColor
        {
            get => ListBox.ForeColor;
            set => FilterListBox.ForeColor = ListBox.ForeColor = value;
        }

        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color ItemSelectForeColor
        {
            get => ListBox.ItemSelectForeColor;
            set => FilterListBox.ItemSelectForeColor = ListBox.ItemSelectForeColor = value;
        }

        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ItemSelectBackColor
        {
            get => ListBox.ItemSelectBackColor;
            set => FilterListBox.ItemSelectBackColor = ListBox.ItemSelectBackColor = value;
        }

        [DefaultValue(typeof(Color), "220, 236, 255")]
        public Color ItemHoverColor
        {
            get => ListBox.HoverColor;
            set => FilterListBox.HoverColor = ListBox.HoverColor = value;
        }

        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ItemRectColor
        {
            get => ListBox.RectColor;
            set => ListBox.RectColor = value;
        }
    }
}