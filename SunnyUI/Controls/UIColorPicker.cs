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
 * File Name: UIColorPicker.cs
 * File Description: Color picker
 * Current Version: V3.1
 * Creation Date: 2020-05-29
 *
 * 2020-05-31: V2.2.5 Added file
 * 2021-03-13: V3.0.2 Added click event to select color
 * 2022-03-10: V3.1.1 Fixed selected color not displaying
 * 2024-08-05: V3.6.8 Added ShowDropDown() function
 * 2024-11-10: V3.7.2 Added StyleDropDown property, set this property to modify the dropdown theme when manually changing Style
 ******************************************************************************
 * File Name: UIColorPicker.cs
 * File Description: Color picker with color wheel and eye dropper
 * File Author: jkristia
 * Open Source License: CPOL
 * Reference: https://www.codeproject.com/Articles/21965/Color-Picker-with-Color-Wheel-and-Eye-Dropper
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Color picker
    /// </summary>
    [DefaultProperty("ValueChanged")]
    [ToolboxItem(true)]
    public sealed class UIColorPicker : UIDropControl
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIColorPicker
            // 
            DropDownStyle = UIDropDownStyle.DropDownList;
            Name = "UIColorPicker";
            ButtonClick += UIColorPicker_ButtonClick;
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

        /// <summary>
        /// Color change event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Color</param>
        public delegate void OnColorChanged(object sender, Color value);

        /// <summary>
        /// Color change event
        /// </summary>
        public event OnColorChanged ValueChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public UIColorPicker()
        {
            InitializeComponent();
            Value = UIColor.Blue;
            CreateInstance();
        }

        public void ShowDropDown()
        {
            UIColorPicker_ButtonClick(this, EventArgs.Empty);
        }

        private void UIColorPicker_ButtonClick(object sender, EventArgs e)
        {
            item.SelectedColor = Value;
            item.Translate();
            item.SetDPIScale();
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;
            ItemForm.Show(this);
        }

        /// <summary>
        /// Click the entire control to select color
        /// </summary>
        [DefaultValue(false)]
        [Description("Click the entire control to select color"), Category("SunnyUI")]
        public bool FullControlSelect
        {
            get => fullControlSelect;
            set => fullControlSelect = value;
        }

        /// <summary>
        /// Value change event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Value</param>
        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            if (Value != (Color)value)
            {
                Value = (Color)value;
                Invalidate();
                ValueChanged?.Invoke(this, Value);
            }
        }

        private readonly UIColorItem item = new UIColorItem();

        /// <summary>
        /// Create instance
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        private Color selectColor;

        /// <summary>
        /// Selected color
        /// </summary>
        [DefaultValue(typeof(Color), "80, 160, 255")]
        [Description("Selected color"), Category("SunnyUI")]
        public Color Value
        {
            get => selectColor;
            set
            {
                selectColor = value;
                item.SelectedColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, System.Drawing.Drawing2D.GraphicsPath path)
        {
            base.OnPaintFore(g, path);
            if (Text.IsValid()) Text = "";
            using var colorPath = g.CreateRoundedRectanglePath(new Rectangle(3, 3, Width - 32, Height - 7), 3, UICornerRadiusSides.All);
            g.FillPath(Value, colorPath);
        }

        /// <summary>
        /// Override paint
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DropDownStyle != UIDropDownStyle.DropDownList)
                DropDownStyle = UIDropDownStyle.DropDownList;
        }
    }
}