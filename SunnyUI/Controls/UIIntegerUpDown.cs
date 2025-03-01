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
 * File Name: UIIntegerUpDown.cs
 * Description: Numeric up-down selection box
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-08-14: V2.2.7 Added font adjustment
 * 2020-12-10: V3.0.9 Added Readonly property
 * 2022-02-07: V3.1.0 Added corner radius control
 * 2022-02-24: V3.1.1 Can set button size and color
 * 2022-05-05: V3.1.8 Added disable input property
 * 2022-09-16: V3.2.4 Added double-click input property
 * 2022-11-12: V3.2.8 Changed integer leave check to real-time input check
 * 2022-11-12: V3.2.8 Removed MaximumEnabled, MinimumEnabled, HasMaximum, HasMinimum properties
 * 2023-01-28: V3.3.1 Changed text box data input change event to MouseLeave
 * 2023-03-24: V3.3.3 Removed ForbidInput property, using Inputable property
 * 2023-12-28: V3.6.2 Fixed inconsistent button color when setting Style
 * 2024-08-27: V3.6.9 Modified edit box font to be consistent with display font
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
    public sealed partial class UIIntegerUpDown : UIPanel, IToolTip
    {
        public delegate void OnValueChanged(object sender, int value);

        public UIIntegerUpDown()
        {
            InitializeComponent();
            SetStyleFlags();
            ShowText = false;
            edit.Type = UITextBox.UIEditType.Integer;
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
        /// Controls that need additional ToolTip settings
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
                Value = edit.IntValue;
            }
        }

        public event OnValueChanged ValueChanged;

        private int _value;

        [DefaultValue(0)]
        [Description("Selected value"), Category("SunnyUI")]
        public int Value
        {
            get => _value;
            set
            {
                value = (int)edit.CheckMaxMin(value);
                if (_value != value)
                {
                    _value = value;
                    pnlValue.Text = _value.ToString();
                    ValueChanged?.Invoke(this, _value);
                }
            }
        }

        /// <summary>
        /// Reload font change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (DefaultFontSize < 0 && pnlValue != null) pnlValue.Font = this.Font;
            if (DefaultFontSize < 0 && edit != null) edit.Font = this.Font;
        }

        private int step = 1;

        [DefaultValue(1)]
        [Description("Step value"), Category("SunnyUI")]
        public int Step
        {
            get => step;
            set => step = Math.Max(1, value);
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

        [Description("Maximum value"), Category("SunnyUI")]
        [DefaultValue(int.MaxValue)]
        public int Maximum
        {
            get => (int)edit.MaxValue;
            set => edit.MaxValue = value;
        }

        [Description("Minimum value"), Category("SunnyUI")]
        [DefaultValue(int.MinValue)]
        public int Minimum
        {
            get => (int)edit.MinValue;
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
            edit.IntValue = Value;
            edit.BringToFront();
            edit.Visible = true;
            edit.Focus();
            edit.SelectAll();
        }

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
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
        /// Reload control size change
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
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonFillColor
        {
            get => btnAdd.FillColor;
            set => btnDec.FillColor = btnAdd.FillColor = value;
        }

        /// <summary>
        /// Mouse hover fill color
        /// </summary>
        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Mouse hover fill color")]
        public Color ButtonFillHoverColor
        {
            get => btnAdd.FillHoverColor;
            set => btnDec.RectHoverColor = btnAdd.RectHoverColor = btnDec.FillHoverColor = btnAdd.FillHoverColor = value;
        }

        /// <summary>
        /// Mouse press fill color
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Mouse press fill color")]
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