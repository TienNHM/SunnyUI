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
 * File name: UIComboDataGridView.cs
 * File description: Table list box
 * Current version: V3.1
 * Creation date: 2021-09-01
 *
 * 2020-09-01: V3.0.6 Added file description
 * 2021-11-05: V3.0.8 Added filtering
 * 2022-03-22: V3.1.1 Added auto-filtering, cell double-click selection
 * 2022-04-16: V3.1.3 Added row multi-selection
 * 2022-06-16: V3.2.0 Added dropdown width and height
 * 2022-06-19: V3.2.0 Added FilterChanged, output filter text and record count
 * 2022-09-08: V3.2.3 Added filter text exception handling
 * 2022-11-03: V3.2.6 Trimmed spaces before and after the string during filtering
 * 2022-11-18: V3.2.9 Added Filter1by1 property for incremental filtering
 * 2022-11-18: V3.2.9 Added Enter key confirmation for filter input
 * 2022-11-30: V3.3.0 Added Clear method
 * 2023-07-25: V3.4.1 After filter input, press the down key to switch to DataGridView, press Enter to quickly select data
 * 2023-09-25: V3.5.0 Added ClearFilter to clear the search bar text in the popup
 * 2024-03-22: V3.6.5 Added ShowDropDown()
 * 2024-11-10: V3.7.2 Added StyleDropDown property to manually change the dropdown theme
 * 2024-11-12: V3.7.2 Added dropdown scrollbar width adjustment property
 * 2024-11-28: V3.8.0 Fixed issue where the filter edit box in the dropdown could not always be displayed #IB7AFB
 * 2024-12-04: V3.8.0 Fixed an error #IB8YK9
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static Sunny.UI.UIDataGridView;

namespace Sunny.UI
{
    [DefaultProperty("ValueChanged")]
    [ToolboxItem(true)]
    public class UIComboDataGridView : UIDropControl, IToolTip
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIComboDataGridView
            // 
            DropDownStyle = UIDropDownStyle.DropDownList;
            Name = "UIComboDataGridView";
            ButtonClick += UIComboDataGridView_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            item?.Dispose();
            base.Dispose(disposing);
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        [DefaultValue(true), Description("Incremental filtering for filter input"), Category("SunnyUI")]
        public bool Filter1by1 { get; set; } = true;

        [DefaultValue(false)]
        [Description("Trim spaces before and after the string during filtering"), Category("SunnyUI")]
        public bool TrimFilter { get; set; }

        public event OnComboDataGridViewFilterChanged FilterChanged;

        [DefaultValue(500)]
        [Description("Dropdown width"), Category("SunnyUI")]
        public int DropDownWidth { get; set; }

        [DefaultValue(300)]
        [Description("Dropdown height"), Category("SunnyUI")]
        public int DropDownHeight { get; set; }

        private void UIComboDataGridView_ButtonClick(object sender, EventArgs e)
        {
            item.TrimFilter = TrimFilter;
            item.FilterColumnName = FilterColumnName;
            item.ShowFilter = ShowFilter;
            ItemForm.Size = ItemSize;
            item.ShowButtons = true;
            item.SetDPIScale();
            item.Translate();
            item.Filter1by1 = Filter1by1;
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;
            //ItemForm.Show(this);
            ItemForm.Show(this, new Size(DropDownWidth < Width ? Width : DropDownWidth, DropDownHeight));
            item.ComboDataGridViewFilterChanged += Item_ComboDataGridViewFilterChanged;
        }

        [DefaultValue(0), Category("SunnyUI"), Description("Vertical scrollbar width, minimum is the native scrollbar width")]
        public int ScrollBarWidth
        {
            get => DataGridView.ScrollBarWidth;
            set => DataGridView.ScrollBarWidth = value;
        }

        [DefaultValue(6), Category("SunnyUI"), Description("Vertical scrollbar handle width, minimum is the native scrollbar width")]
        public int ScrollBarHandleWidth
        {
            get => DataGridView.ScrollBarHandleWidth;
            set => DataGridView.ScrollBarHandleWidth = value;
        }

        public void ShowDropDown()
        {
            UIComboDataGridView_ButtonClick(this, EventArgs.Empty);
        }

        public override void Clear()
        {
            base.Clear();
            DataGridView.DataSource = null;
        }

        public void ClearFilter()
        {
            item.ClearFilter();
        }

        private void Item_ComboDataGridViewFilterChanged(object sender, UIComboDataGridViewArgs e)
        {
            FilterChanged?.Invoke(this, e);
        }

        [DefaultValue(typeof(Size), "320, 240"), Description("Dropdown popup size"), Category("SunnyUI")]
        public Size ItemSize { get; set; } = new Size(320, 240);

        public UIComboDataGridView()
        {
            InitializeComponent();
            fullControlSelect = true;
            CreateInstance();
            DropDownWidth = 500;
            DropDownHeight = 300;
        }

        /// <summary>
        /// Controls that need additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        [DefaultValue(true), Description("Show filter edit box in dropdown"), Category("SunnyUI")]
        public bool ShowFilter { get; set; } = true;

        private readonly UIComboDataGridViewItem item = new UIComboDataGridViewItem();

        /// <summary>
        /// Create object
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        public UIDataGridView DataGridView => item.DataGridView;

        public event OnSelectIndexChange SelectIndexChange;

        public delegate void OnValueChanged(object sender, object value);
        public event OnValueChanged ValueChanged;

        /// <summary>
        /// Value changed event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Value</param>
        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            if (DataGridView.MultiSelect)
            {
                ValueChanged?.Invoke(this, value);
            }
            else
            {
                if (ShowFilter)
                    ValueChanged?.Invoke(this, value);
                else
                    SelectIndexChange?.Invoke(this, value.ToString().ToInt());
            }
        }

        [DefaultValue(null)]
        public string FilterColumnName { get; set; }
    }
}
