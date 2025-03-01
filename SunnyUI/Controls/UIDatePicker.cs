/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, Multi-page Development Framework.
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
 * File Name: UIDatePicker.cs
 * File Description: Date Picker
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-08-07: V2.2.7 Editable input, date range control to prevent errors
 * 2021-04-15: V3.0.3 Added ShowToday property to display today
 * 2021-08-14: V3.0.6 Added options to select year, year and month, year, month, and day
 * 2022-11-08: V3.2.8 Added MaxDate, MinDate
 * 2023-05-14: V3.3.6 Year, year and month, year, month, and day can be set separately with format masks
 * 2023-05-14: V3.3.6 Fixed text formatting display issue
 * 2024-06-09: V3.6.6 Dropdown can be selected with a magnification of 2
 * 2024-07-13: V3.6.7 Modified the display method of the selected date in the dropdown
 * 2024-08-28: V3.7.0 Fixed display error when the format string contains '/'
 * 2024-11-10: V3.7.2 Added StyleDropDown property, set this property to change the dropdown theme when manually modifying the Style
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
    public sealed partial class UIDatePicker : UIDropControl, IToolTip
    {
        public delegate void OnDateTimeChanged(object sender, DateTime value);

        public UIDatePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;
            MaxLength = 10;
            EditorLostFocus += UIDatePicker_LostFocus;
            TextChanged += UIDatePicker_TextChanged;

            CreateInstance();
        }

        private DateTime max = DateTime.MaxValue;
        private DateTime min = DateTime.MinValue;

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["DateYearFormat", "DateYearMonthFormat", "DateFormat"];

        internal static DateTime EffectiveMaxDate(DateTime maxDate)
        {
            DateTime maxSupportedDate = DateTimePicker.MaximumDateTime;
            if (maxDate > maxSupportedDate)
            {
                return maxSupportedDate;
            }
            return maxDate;
        }

        internal static DateTime EffectiveMinDate(DateTime minDate)
        {
            DateTime minSupportedDate = DateTimePicker.MinimumDateTime;
            if (minDate < minSupportedDate)
            {
                return minSupportedDate;
            }
            return minDate;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MaxDateTime = new DateTime(9998, 12, 31);

        [DefaultValue(typeof(DateTime), "9998/12/31")]
        [Description("Maximum date"), Category("SunnyUI")]
        public DateTime MaxDate
        {
            get
            {
                return EffectiveMaxDate(max);
            }
            set
            {
                if (value != max)
                {
                    if (value < EffectiveMinDate(min))
                    {
                        value = EffectiveMinDate(min);
                    }

                    // If trying to set the maximum greater than MaxDateTime, throw.
                    if (value > MaximumDateTime)
                    {
                        value = MaximumDateTime;
                    }

                    max = value;

                    //If Value (which was once valid) is suddenly greater than the max (since we just set it)
                    //then adjust this...
                    if (Value > max)
                    {
                        Value = max;
                    }
                }
            }
        }

        [DefaultValue(typeof(DateTime), "1753/1/1")]
        [Description("Minimum date"), Category("SunnyUI")]
        public DateTime MinDate
        {
            get
            {
                return EffectiveMinDate(min);
            }
            set
            {
                if (value != min)
                {
                    if (value > EffectiveMaxDate(max))
                    {
                        value = EffectiveMaxDate(max);
                    }

                    // If trying to set the minimum less than MinimumDateTime, throw.
                    if (value < MinimumDateTime)
                    {
                        value = MinimumDateTime;
                    }

                    min = value;

                    //If Value (which was once valid) is suddenly less than the min (since we just set it)
                    //then adjust this...
                    if (Value < min)
                    {
                        Value = min;
                    }
                }
            }
        }

        internal static DateTime MaximumDateTime
        {
            get
            {
                DateTime maxSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
                if (maxSupportedDateTime.Year > MaxDateTime.Year)
                {
                    return MaxDateTime;
                }
                return maxSupportedDateTime;
            }
        }

        internal static DateTime MinimumDateTime
        {
            get
            {
                DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
                if (minSupportedDateTime.Year < 1753)
                {
                    return new DateTime(1753, 1, 1);
                }
                return minSupportedDateTime;
            }
        }

        [DefaultValue(false)]
        [Description("Whether the date input can be empty"), Category("SunnyUI")]
        public bool CanEmpty { get; set; }

        [DefaultValue(false)]
        [Description("Display today button when entering date"), Category("SunnyUI")]
        public bool ShowToday { get; set; }

        private UIDateType showType = UIDateType.YearMonthDay;

        [DefaultValue(UIDateType.YearMonthDay)]
        [Description("Date display type"), Category("SunnyUI")]
        public UIDateType ShowType
        {
            get => showType;
            set
            {
                showType = value;
                switch (showType)
                {
                    case UIDateType.YearMonthDay:
                        MaxLength = dateFormat.Length;
                        Text = Value.ToString(dateFormat);
                        break;
                    case UIDateType.YearMonth:
                        MaxLength = dateYearMonthFormat.Length;
                        Text = Value.ToString(dateYearMonthFormat);
                        break;
                    case UIDateType.Year:
                        MaxLength = dateYearFormat.Length;
                        Text = Value.ToString(dateYearFormat);
                        break;
                    default:
                        MaxLength = dateFormat.Length;
                        Text = Value.ToString(dateFormat);
                        break;
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Control that needs additional ToolTip
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        public int Year => Value.Year;
        public int Month => Value.Month;
        public int Day => Value.Day;

        private void UIDatePicker_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length == MaxLength && !DropSetted)
            {
                try
                {
                    switch (ShowType)
                    {
                        case UIDateType.YearMonthDay:
                            DateTime dt1 = Text.ToDateTime(DateFormat);
                            if (Value != dt1) Value = dt1;
                            break;
                        case UIDateType.YearMonth:
                            DateTime dt2 = Text.ToDateTime(DateYearMonthFormat);
                            if (Value != dt2) Value = dt2;
                            break;
                        case UIDateType.Year:
                            DateTime dt3 = Text.ToDateTime(DateYearFormat);
                            if (Value != dt3) Value = dt3;
                            break;
                    }
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
                switch (ShowType)
                {
                    case UIDateType.YearMonthDay:
                        DateTime dt1 = Text.ToDateTime(DateFormat);
                        if (Value != dt1) Value = dt1;
                        break;
                    case UIDateType.YearMonth:
                        DateTime dt2 = Text.ToDateTime(DateYearMonthFormat);
                        if (Value != dt2) Value = dt2;
                        break;
                    case UIDateType.Year:
                        DateTime dt3 = Text.ToDateTime(DateYearFormat);
                        if (Value != dt3) Value = dt3;
                        break;
                }
            }
            catch
            {
                Value = DateTime.Now.Date;
            }
        }

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

        private readonly UIDateItem item = new UIDateItem();

        /// <summary>
        /// Create object
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        private bool DropSetted = false;
        [Description("Selected date"), Category("SunnyUI")]
        public DateTime Value
        {
            get => item.Date;
            set
            {
                if (value < new DateTime(1900, 1, 1))
                    value = new DateTime(1900, 1, 1);

                DropSetted = true;
                switch (ShowType)
                {
                    case UIDateType.YearMonthDay:
                        Text = value.ToString(dateFormat, CultureInfo.InvariantCulture);
                        break;
                    case UIDateType.YearMonth:
                        Text = value.ToString(dateYearMonthFormat, CultureInfo.InvariantCulture);
                        break;
                    case UIDateType.Year:
                        Text = value.ToString(dateYearFormat, CultureInfo.InvariantCulture);
                        break;
                }

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
            item.ShowType = ShowType;
            item.Date = Value;
            item.ShowToday = ShowToday;
            item.PrimaryColor = RectColor;
            item.Translate();
            item.SetDPIScale();
            item.SetStyleColor(UIStyles.ActiveStyleColor);
            item.max = MaxDate;
            item.min = MinDate;
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;

            Size size = SizeMultiple == 1 ? new Size(284, 200) : new Size(568, 400);
            ItemForm.Show(this, size);
        }

        private string dateFormat = "yyyy-MM-dd";

        [Description("Date format mask"), Category("SunnyUI")]
        [DefaultValue("yyyy-MM-dd")]
        public string DateFormat
        {
            get => dateFormat;
            set
            {
                dateFormat = value;

                if (ShowType == UIDateType.YearMonthDay)
                {
                    MaxLength = dateFormat.Length;
                    Text = Value.ToString(dateFormat);
                }
            }
        }

        private string dateYearMonthFormat = "yyyy-MM";

        [Description("Date format mask"), Category("SunnyUI")]
        [DefaultValue("yyyy-MM")]
        public string DateYearMonthFormat
        {
            get => dateYearMonthFormat;
            set
            {
                dateYearMonthFormat = value;

                if (ShowType == UIDateType.YearMonth)
                {
                    MaxLength = dateYearMonthFormat.Length;
                    Text = Value.ToString(dateYearMonthFormat);
                }
            }
        }

        private string dateYearFormat = "yyyy";

        [Description("Date format mask"), Category("SunnyUI")]
        [DefaultValue("yyyy")]
        public string DateYearFormat
        {
            get => dateYearFormat;
            set
            {
                dateYearFormat = value;

                if (ShowType == UIDateType.Year)
                {
                    MaxLength = dateYearFormat.Length;
                    Text = Value.ToString(dateYearFormat);
                }
            }
        }
    }
}