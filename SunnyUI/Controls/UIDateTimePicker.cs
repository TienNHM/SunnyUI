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
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    public sealed class UIDatetimePicker : UIDropControl, IToolTip
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIDatetimePicker
            // 
            Name = "UIDatetimePicker";
            SymbolDropDown = 61555;
            SymbolNormal = 61555;
            ButtonClick += UIDatetimePicker_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["DateFormat"];

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

        /// <summary>
        /// Control that needs additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        [DefaultValue(false)]
        [Description("Whether it can be empty when entering the date"), Category("SunnyUI")]
        public bool CanEmpty { get; set; }

        [DefaultValue(false)]
        [Description("Show today's button when entering the date"), Category("SunnyUI")]
        public bool ShowToday { get; set; }

        public UIDatetimePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;
            Text = Value.ToString(DateFormat);
            Width = 200;
            EditorLostFocus += UIDatePicker_LostFocus;
            TextChanged += UIDatePicker_TextChanged;
            MaxLength = 19;

            CreateInstance();
        }

        private void UIDatePicker_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length == MaxLength && !DropSetted)
            {
                try
                {
                    DateTime dt = Text.ToDateTime(DateFormat);
                    if (Value != dt) Value = dt;
                }
                catch
                {
                    Value = DateTime.Now.Date;
                }
            }
        }

        private void UIDatePicker_LostFocus(object sender, EventArgs e)
        {
            if (Text.IsNullOrEmpty())
            {
                if (CanEmpty) return;
            }

            try
            {
                DateTime dt = Text.ToDateTime(DateFormat);
                if (Value != dt) Value = dt;
            }
            catch
            {
                Value = DateTime.Now.Date;
            }
        }

        public delegate void OnDateTimeChanged(object sender, DateTime value);


        public event OnDateTimeChanged ValueChanged;

        /// <summary>
        /// Value changed event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Value</param>
        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            Value = (DateTime)value;
            Invalidate();
        }

        private readonly UIDateTimeItem item = new UIDateTimeItem();

        /// <summary>
        /// Create object
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        private bool DropSetted = false;
        [Description("Selected date and time"), Category("SunnyUI")]
        public DateTime Value
        {
            get => item.Date;
            set
            {
                if (value < new DateTime(1900, 1, 1))
                    value = new DateTime(1900, 1, 1);

                DropSetted = true;
                Text = value.ToString(dateFormat, CultureInfo.InvariantCulture);
                DropSetted = false;

                if (item.Date != value)
                {
                    item.Date = value;
                }

                ValueChanged?.Invoke(this, Value);
            }
        }

        [DefaultValue(1)]
        [Description("Popup magnification, can be 1 or 2"), Category("SunnyUI")]
        public int SizeMultiple { get => item.SizeMultiple; set => item.SizeMultiple = value; }

        private void UIDatetimePicker_ButtonClick(object sender, EventArgs e)
        {
            item.Date = Value;
            item.ShowToday = ShowToday;
            item.PrimaryColor = RectColor;
            item.Translate();
            item.SetDPIScale();
            item.SetStyleColor(UIStyles.ActiveStyleColor);
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;
            Size size = SizeMultiple == 1 ? new Size(452, 200) : new Size(904, 400);
            ItemForm.Show(this, size);
        }

        private string dateFormat = "yyyy-MM-dd HH:mm:ss";

        [Description("Date format mask"), Category("SunnyUI")]
        [DefaultValue("yyyy-MM-dd HH:mm:ss")]
        public string DateFormat
        {
            get => dateFormat;
            set
            {
                dateFormat = value;
                Text = Value.ToString(dateFormat);
                MaxLength = dateFormat.Length;
            }
        }
    }
}
