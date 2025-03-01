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
 * File Name: UIDoubleUpDown.cs
 * File Description: Numeric up-down control
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-08-14: V2.2.7 Added font adjustment
 * 2020-12-10: V3.0.9 Added Readonly property
 * 2022-01-28: V3.1.0 Fixed issue where editing value was 0 when default value was not 0
 * 2022-02-07: V3.1.0 Added corner radius control
 * 2022-02-24: V3.1.1 Can set button size and color
 * 2022-05-05: V3.1.8 Added forbid input property
 * 2022-09-16: V3.2.4 Added double-click input property
 * 2022-11-12: V3.2.8 Changed floating-point size leave judgment to real-time input judgment
 * 2022-11-12: V3.2.8 Removed MaximumEnabled, MinimumEnabled, HasMaximum, HasMinimum properties
 * 2023-01-28: V3.3.1 Changed text box data input change event to MouseLeave
 * 2023-03-24: V3.3.3 Removed ForbidInput property, using Inputable property
 * 2023-12-28: V3.6.2 Fixed inconsistent button color when setting Style
 * 2024-08-27: V3.6.9 Changed edit box font to be consistent with display font
 * 2024-08-27: V3.7.0 Added offset position for add button font icon
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    public sealed partial class UIDoubleUpDown : UIPanel, IToolTip
    {
        public delegate void OnValueChanged(object sender, double value);

        public UIDoubleUpDown()
        {
            InitializeComponent();

            SetStyleFlags();

            ShowText = false;
            edit.Type = UITextBox.UIEditType.Double;
            edit.Parent = pnlValue;
            edit.Visible = false;
            edit.BorderStyle = BorderStyle.None;
            edit.MouseLeave += Edit_Leave;
            pnlValue.Paint += PnlValue_Paint;

            btnAdd.Style = UIStyle.Custom;
            btnDec.Style = UIStyle.Custom;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        /// <summary>
        /// Control that needs additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return pnlValue;
        }

        private void PnlValue_Paint(object sender, PaintEventArgs e)
        {
            if (Enabled)
            {
                e.Graphics.DrawLine(RectColor, 0, 0, pnlValue.Width, 0);
                e.Graphics.DrawLine(RectColor, 0, Height - 1, pnlValue.Width, Height - 1);
            }
            else
            {
                e.Graphics.DrawLine(RectDisableColor, 0, 0, pnlValue.Width, 0);
                e.Graphics.DrawLine(RectDisableColor, 0, Height - 1, pnlValue.Width, Height - 1);
            }
        }

        private void Edit_Leave(object sender, EventArgs e)
        {
            if (edit.Visible)
            {
                edit.Visible = false;
                pnlValue.FillColor = pnlColor;
                Value = edit.DoubleValue;
            }
        }

        /// <summary>
        /// Override font change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (DefaultFontSize < 0 && pnlValue != null) pnlValue.Font = this.Font;
            if (DefaultFontSize < 0 && edit != null) edit.Font = this.Font;
        }

        public event OnValueChanged ValueChanged;

        private double _value;

        [DefaultValue(0D)]
        [Description("Selected value"), Category("SunnyUI")]
        public double Value
        {
            get => _value;
            set
            {
                value = edit.CheckMaxMin(value);
                if (_value != value)
                {
                    _value = value;
                    pnlValue.Text = _value.ToString("F" + decLength);
                    ValueChanged?.Invoke(this, _value);
                }
            }
        }

        private int decLength = 1;

        [Description("Number of decimal places to display"), Category("SunnyUI")]
        [DefaultValue(1)]
        public int DecimalPlaces
        {
            get => decLength;
            set
            {
                decLength = Math.Max(value, 0);
                pnlValue.Text = _value.ToString("F" + decLength);
            }
        }

        private double step = 0.1;

        [DefaultValue(0.1)]
        [Description("Step value"), Category("SunnyUI")]
        public double Step
        {
            get => step;
            set => step = Math.Abs(value);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ReadOnly) return;

            Value += Step;
            if (edit.Visible)
            {
                edit.Visible = false;
                pnlValue.FillColor = pnlColor;
            }
        }

        private void btnDec_Click(object sender, EventArgs e)
        {
            if (ReadOnly) return;

            Value -= Step;
            if (edit.Visible)
            {
                edit.Visible = false;
                pnlValue.FillColor = pnlColor;
            }
        }

        [DefaultValue(typeof(double), "2147483647")]
        [Description("Maximum value"), Category("SunnyUI")]
        public double Maximum
        {
            get => edit.MaxValue;
            set => edit.MaxValue = value;
        }

        [DefaultValue(typeof(double), "-2147483648")]
        [Description("Minimum value"), Category("SunnyUI")]
        public double Minimum
        {
            get => edit.MinValue;
            set => edit.MinValue = value;
        }

        [DefaultValue(true)]
        [Description("Whether double-click input is allowed"), Category("SunnyUI")]
        public bool Inputable { get; set; } = true;

        private readonly UIEdit edit = new UIEdit();
        private Color pnlColor;
        private void pnlValue_DoubleClick(object sender, EventArgs e)
        {
            if (ReadOnly) return;
            if (!Inputable) return;

            edit.Left = 1;
            edit.Font = pnlValue.Font;
            edit.Top = (pnlValue.Height - edit.Height) / 2;
            edit.Width = pnlValue.Width - 2;
            pnlColor = pnlValue.FillColor;
            pnlValue.FillColor = Color.White;
            edit.TextAlign = HorizontalAlignment.Center;
            edit.DecLength = decLength;
            edit.DoubleValue = Value;
            edit.BringToFront();
            edit.Visible = true;
            edit.Focus();
            edit.SelectAll();
        }

        [DefaultValue(false)]
        [Description("Whether it is read-only"), Category("SunnyUI")]
        public bool ReadOnly { get; set; }

        protected override void OnRadiusSidesChange()
        {
            if (btnDec == null || btnAdd == null) return;

            btnDec.RadiusSides =
                 (RadiusSides.HasFlag(UICornerRadiusSides.LeftTop) ? UICornerRadiusSides.LeftTop : UICornerRadiusSides.None) |
                 (RadiusSides.HasFlag(UICornerRadiusSides.LeftBottom) ? UICornerRadiusSides.LeftBottom : UICornerRadiusSides.None);
            btnAdd.RadiusSides =
                (RadiusSides.HasFlag(UICornerRadiusSides.RightTop) ? UICornerRadiusSides.RightTop : UICornerRadiusSides.None) |
                (RadiusSides.HasFlag(UICornerRadiusSides.RightBottom) ? UICornerRadiusSides.RightBottom : UICornerRadiusSides.None);
        }

        protected override void OnRadiusChanged(int value)
        {
            if (btnDec == null || btnAdd == null) return;
            btnDec.Radius = btnAdd.Radius = value;
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Height < UIGlobal.EditorMinHeight) Height = UIGlobal.EditorMinHeight;
            if (Height > UIGlobal.EditorMaxHeight) Height = UIGlobal.EditorMaxHeight;
        }

        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            btnAdd.SetStyleColor(uiColor);
            btnDec.SetStyleColor(uiColor);
            pnlValue.SetStyleColor(uiColor);
            btnAdd.Invalidate();
            btnDec.Invalidate();
            pnlValue.Invalidate();
        }

        /// <summary>
        /// Fill color, do not fill if the value is background color or transparent color or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonFillColor
        {
            get => btnAdd.FillColor;
            set => btnDec.FillColor = btnAdd.FillColor = value;
        }

        /// <summary>
        /// Fill color when mouse hovers
        /// </summary>
        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Fill color when mouse hovers")]
        public Color ButtonFillHoverColor
        {
            get => btnAdd.FillHoverColor;
            set => btnDec.RectHoverColor = btnAdd.RectHoverColor = btnDec.FillHoverColor = btnAdd.FillHoverColor = value;
        }

        /// <summary>
        /// Fill color when mouse is pressed
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Fill color when mouse is pressed")]
        public Color ButtonFillPressColor
        {
            get => btnAdd.FillPressColor;
            set => btnDec.RectPressColor = btnAdd.RectPressColor = btnDec.FillPressColor = btnAdd.FillPressColor = value;
        }

        /// <summary>
        /// Font icon color
        /// </summary>
        [Description("Icon color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonSymbolColor
        {
            get => btnAdd.SymbolColor;
            set => btnDec.SymbolColor = btnAdd.SymbolColor = value;
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonRectColor
        {
            get => btnAdd.RectColor;
            set => pnlValue.RectColor = btnDec.RectColor = btnAdd.RectColor = value;
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            if (pnlValue == null) return;
            pnlValue.FillColor = color;
        }

        private int buttonWidth = 29;
        [DefaultValue(29)]
        public int ButtonWidth
        {
            get => buttonWidth;
            set
            {
                buttonWidth = Math.Max(value, 29);
                if (btnAdd == null || btnDec == null) return;
                btnAdd.Width = btnDec.Width = buttonWidth;
            }
        }

        public override Color ForeColor { get => pnlValue.ForeColor; set => pnlValue.ForeColor = value; }

        /// <summary>
        /// Offset position for add button font icon
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Offset position for add button font icon"), Category("SunnyUI")]
        public Point AddSymbolOffset
        {
            get => btnAdd.SymbolOffset;
            set => btnAdd.SymbolOffset = value;
        }

        /// <summary>
        /// Offset position for subtract button font icon
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Offset position for subtract button font icon"), Category("SunnyUI")]
        public Point DecSymbolOffset
        {
            get => btnDec.SymbolOffset;
            set => btnDec.SymbolOffset = value;
        }
    }
}