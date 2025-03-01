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
 * File Name: UIRadioButton.cs
 * Description: Radio Button
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-16: V2.2.1 Added ReadOnly property
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2021-04-26: V3.0.3 Added default event CheckedChanged
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-12-21: V3.3.0 Fixed CheckedChanged event
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-11-07: V3.5.2 Added icon size modification
 * 2023-12-04: V3.6.1 Added property to modify icon size
 * 2024-08-26: V3.6.9 Fixed AutoSize not automatically displaying when text changes, #IAKYX4
 * 2024-08-30: V3.7.0 Modified AutoSize property to be saved in Design.cs file, #IAKYX4
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [ToolboxItem(true)]
    public sealed class UIRadioButton : UIControl
    {
        public delegate void OnValueChanged(object sender, bool value);

        public event OnValueChanged ValueChanged;

        public event EventHandler CheckedChanged;

        public UIRadioButton()
        {
            SetStyleFlags();
            Cursor = Cursors.Hand;
            ShowRect = false;
            Size = new Size(150, 29);

            foreColor = UIStyles.Blue.CheckBoxForeColor;
            fillColor = UIStyles.Blue.CheckBoxColor;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (AutoSize && Dock == DockStyle.None)
            {
                Size sf = TextRenderer.MeasureText(Text, Font);
                int w = sf.Width + RadioButtonSize + 7;
                int h = Math.Max(RadioButtonSize, sf.Height) + 5;
                if (Width != w) Width = w;
                if (Height != h) Height = h;
            }
        }

        /// <inheritdoc/>
        [Browsable(true)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.All)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                base.AutoSize = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        private int _imageSize = 16;
        private int _imageInterval = 3;

        [DefaultValue(16)]
        [Description("Icon size"), Category("SunnyUI")]
        [Browsable(true)]
        public int RadioButtonSize
        {
            get => _imageSize;
            set
            {
                _imageSize = Math.Max(value, 16);
                _imageSize = Math.Min(value, 128);
                Invalidate();
            }
        }

        [DefaultValue(3)]
        [Description("Button image text interval"), Category("SunnyUI")]
        public int ImageInterval
        {
            get => _imageInterval;
            set
            {
                _imageInterval = Math.Max(1, value);
                Invalidate();
            }
        }

        private bool _checked;

        [DefaultValue(false)]
        [Description("Checked"), Category("SunnyUI")]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;

                    if (value)
                    {
                        try
                        {
                            if (Parent == null) return;
                            List<UIRadioButton> buttons = Parent.GetControls<UIRadioButton>();
                            foreach (var box in buttons)
                            {
                                if (box == this) continue;
                                if (box.GroupIndex != GroupIndex) continue;
                                if (box.Checked) box.Checked = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(@"UIRadioBox click error." + ex.Message);
                        }
                    }

                    ValueChanged?.Invoke(this, _checked);
                    CheckedChanged?.Invoke(this, new EventArgs());
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            //填充文字
            Color color = foreColor;
            color = Enabled ? color : UIDisableColor.Fore;
            Rectangle rect = new Rectangle(_imageSize + _imageInterval * 2, 0, Width - _imageSize + _imageInterval * 2, Height);
            g.DrawString(Text, Font, color, rect, ContentAlignment.MiddleLeft);
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            int ImageSize = RadioButtonSize;
            //图标
            float top = (Height - ImageSize) / 2.0f;
            float left = Text.IsValid() ? ImageInterval : (Width - ImageSize) / 2.0f;

            Color color = Enabled ? fillColor : foreDisableColor;
            if (Checked)
            {
                g.FillEllipse(color, left, top, ImageSize, ImageSize);
                float pointSize = ImageSize - 4;
                g.FillEllipse(BackColor.IsValid() ? BackColor : Color.White,
                    left + ImageSize / 2.0f - pointSize / 2.0f,
                    top + ImageSize / 2.0f - pointSize / 2.0f,
                    pointSize, pointSize);

                pointSize = ImageSize - 8;
                g.FillEllipse(color,
                    left + ImageSize / 2.0f - pointSize / 2.0f,
                    top + ImageSize / 2.0f - pointSize / 2.0f,
                    pointSize, pointSize);
            }
            else
            {
                using Pen pn = new Pen(color, 2);
                g.SetHighQuality();
                g.DrawEllipse(pn, left + 1, top + 1, ImageSize - 2, ImageSize - 2);
                g.SetDefaultQuality();
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnClick(EventArgs e)
        {
            if (!ReadOnly && !Checked)
            {
                Checked = true;
            }

            base.OnClick(e);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            fillColor = uiColor.CheckBoxColor;
            foreColor = uiColor.CheckBoxForeColor;
        }

        [DefaultValue(0)]
        [Description("Group index"), Category("SunnyUI")]
        public int GroupIndex { get; set; }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RadioButtonColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }
    }
}