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
 * File Name: UIDatePicker.cs
 * File Description: Time picker
 * Current Version: V3.1
 * Creation Date: 2020-05-29
 *
 * 2020-05-29: V2.2.5 Added file
 * 2020-08-07: V2.2.7 Editable input
 * 2020-09-16: V2.2.7 Changed the direction of the scroll wheel to select time
 * 2024-06-09: V3.6.6 Dropdown box can be magnified by 2 times
 * 2024-11-10: V3.7.2 Added StyleDropDown property, set this property to change the dropdown theme when manually modifying Style
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    public sealed class UITimePicker : UIDropControl, IToolTip
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UITimePicker
            // 
            Name = "UITimePicker";
            SymbolDropDown = 61555;
            SymbolNormal = 61555;
            ButtonClick += UITimePicker_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["TimeFormat"];

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

        public UITimePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;

            EditorLostFocus += UIDatePicker_LostFocus;
            TextChanged += UIDatePicker_TextChanged;
            MaxLength = 8;

            CreateInstance();
        }

        [DefaultValue(false)]
        [Description("Whether the date input can be empty"), Category("SunnyUI")]
        public bool CanEmpty { get; set; }

        private void UIDatePicker_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length == MaxLength)
            {
                try
                {
                    DateTime dt = (DateTime.Now.DateString() + " " + Text).ToDateTime(DateTimeEx.DateFormat + " " + timeFormat);
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
                DateTime dt = (DateTime.Now.DateString() + " " + Text).ToDateTime(DateTimeEx.DateFormat + " " + timeFormat);
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

        private readonly UITimeItem item = new UITimeItem();

        [DefaultValue(1)]
        [Description("Popup magnification, can be 1 or 2"), Category("SunnyUI")]
        public int SizeMultiple { get => item.SizeMultiple; set => item.SizeMultiple = value; }

        /// <summary>
        /// Create object
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        [Description("Selected time"), Category("SunnyUI")]
        public DateTime Value
        {
            get => item.Time;
            set
            {
                Text = value.ToString(timeFormat);
                if (item.Time != value)
                {
                    item.Time = value;
                }

                ValueChanged?.Invoke(this, Value);
            }
        }

        private string timeFormat = "HH:mm:ss";

        [Description("Time format mask"), Category("SunnyUI")]
        [DefaultValue("HH:mm:ss")]
        public string TimeFormat
        {
            get => timeFormat;
            set
            {
                timeFormat = value;
                Text = Value.ToString(timeFormat);
                MaxLength = timeFormat.Length;
            }
        }

        private void UITimePicker_ButtonClick(object sender, EventArgs e)
        {
            item.Time = Value;
            item.Translate();
            item.SetDPIScale();
            item.SetStyleColor(UIStyles.ActiveStyleColor);
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;
            Size size = SizeMultiple == 1 ? new Size(168, 200) : new Size(336, 400);
            ItemForm.Show(this, size);
        }
    }
}