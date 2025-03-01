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
 * File Name: UISwitch.cs
 * File Description: Switch
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2021-05-06: V3.0.3 Triggered ValueChanged event when Active state changes
 * 2021-09-14: V3.0.7 Added Disabled color
 * 2022-01-02: V3.0.9 Added ReadOnly property
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-09-26: V3.2.4 Fixed issue where value could still be changed by double-clicking when ReadOnly
 * 2022-04-23: V3.3.5 Added ActiveChanging event to allow judgment before state changes
 * 2023-05-13: V3.3.6 Refactored DrawString function
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sunny.UI
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Active")]
    [ToolboxItem(true)]
    public sealed class UISwitch : UIControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="value">Switch value</param>
        public delegate void OnValueChanged(object sender, bool value);

        public enum UISwitchShape
        {
            Round,
            Square
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["ActiveText", "InActiveText"];

        public UISwitch()
        {
            SetStyleFlags();
            Height = 29;
            Width = 75;
            ShowText = false;
            ShowRect = false;

            inActiveColor = Color.Gray;
            fillColor = Color.White;

            rectColor = UIStyles.Blue.SwitchActiveColor;
            fillColor = UIStyles.Blue.SwitchFillColor;
            inActiveColor = UIStyles.Blue.SwitchInActiveColor;
            rectDisableColor = UIStyles.Blue.SwitchRectDisableColor;
        }

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly { get; set; }

        private UISwitchShape switchShape = UISwitchShape.Round;

        [Description("Switch shape"), Category("SunnyUI")]
        [DefaultValue(UISwitchShape.Round)]
        public UISwitchShape SwitchShape
        {
            get => switchShape;
            set
            {
                switchShape = value;
                Invalidate();
            }
        }

        public event OnValueChanged ValueChanged;

        public event EventHandler ActiveChanged;

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        private bool activeValue;

        [DefaultValue(false)]
        [Description("Is active"), Category("SunnyUI")]
        public bool Active
        {
            get => activeValue;
            set
            {
                if (!ReadOnly && activeValue != value)
                {
                    activeValue = value;
                    ValueChanged?.Invoke(this, value);
                    ActiveChanged?.Invoke(this, new EventArgs());
                    Invalidate();
                }
            }
        }

        private string activeText = "On";

        [DefaultValue("On")]
        [Description("Active text"), Category("SunnyUI")]
        public string ActiveText
        {
            get => activeText;
            set
            {
                activeText = value;
                Invalidate();
            }
        }

        private string inActiveText = "Off";

        [DefaultValue("Off")]
        [Description("Inactive text"), Category("SunnyUI")]
        public string InActiveText
        {
            get => inActiveText;
            set
            {
                inActiveText = value;
                Invalidate();
            }
        }

        private Color inActiveColor;

        [DefaultValue(typeof(Color), "Gray")]
        [Description("Inactive color"), Category("SunnyUI")]
        public Color InActiveColor
        {
            get => inActiveColor;
            set
            {
                inActiveColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Button color, if the value is the background color or transparent color or null, it will not be filled
        /// </summary>
        [Description("Button color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Active color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ActiveColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnClick(EventArgs e)
        {
            ActiveChange();
            base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (!UseDoubleClick)
            {
                ActiveChange();
                base.OnClick(e);
            }
            else
            {
                base.OnDoubleClick(e);
            }
        }

        public event OnCancelEventArgs ActiveChanging;

        private void ActiveChange()
        {
            CancelEventArgs e = new CancelEventArgs();
            if (ActiveChanging != null)
            {
                ActiveChanging?.Invoke(this, e);
            }

            if (!e.Cancel)
            {
                Active = !Active;
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            rectColor = uiColor.SwitchActiveColor;
            fillColor = uiColor.SwitchFillColor;
            inActiveColor = uiColor.SwitchInActiveColor;
            rectDisableColor = uiColor.SwitchRectDisableColor;
        }

        [Description("Disabled color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "173, 178, 181")]
        public Color DisabledColor
        {
            get => rectDisableColor;
            set => SetRectDisableColor(value);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            Color color = Active ? ActiveColor : InActiveColor;
            if (!Enabled) color = rectDisableColor;

            if (SwitchShape == UISwitchShape.Round)
            {
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using GraphicsPath path1 = rect.CreateTrueRoundedRectanglePath(rect.Height);
                g.FillPath(color, path1, true);

                int width = Width - 3 - 1 - 3 - (rect.Height - 6);
                if (!Active)
                {
                    g.FillEllipse(fillColor.IsValid() ? fillColor : Color.White, 3, 3, rect.Height - 6, rect.Height - 6);
                    g.DrawString(InActiveText, Font, fillColor.IsValid() ? fillColor : Color.White, new Rectangle(3 + rect.Height - 6, 0, width, rect.Height), ContentAlignment.MiddleCenter);
                }
                else
                {
                    g.FillEllipse(fillColor.IsValid() ? fillColor : Color.White, Width - 3 - 1 - (rect.Height - 6), 3, rect.Height - 6, rect.Height - 6);
                    g.DrawString(ActiveText, Font, fillColor.IsValid() ? fillColor : Color.White, new Rectangle(3, 0, width, rect.Height), ContentAlignment.MiddleCenter);
                }
            }

            if (SwitchShape == UISwitchShape.Square)
            {
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.FillRoundRectangle(color, rect, Radius);

                int width = Width - 3 - 1 - 3 - (rect.Height - 6);
                if (!Active)
                {
                    g.FillRoundRectangle(fillColor.IsValid() ? fillColor : Color.White, 3, 3, rect.Height - 6, rect.Height - 6, Radius);
                    g.DrawString(InActiveText, Font, fillColor.IsValid() ? fillColor : Color.White, new Rectangle(3 + rect.Height - 6, 0, width, rect.Height), ContentAlignment.MiddleCenter);
                }
                else
                {
                    g.FillRoundRectangle(fillColor.IsValid() ? fillColor : Color.White, Width - 3 - 1 - (rect.Height - 6), 3, rect.Height - 6, rect.Height - 6, Radius);
                    g.DrawString(ActiveText, Font, fillColor.IsValid() ? fillColor : Color.White, new Rectangle(3, 0, width, rect.Height), ContentAlignment.MiddleCenter);
                }
            }
        }
    }
}