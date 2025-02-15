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
 * File Name: UICheckBox.cs
 * Description: Checkbox
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-16: V2.2.1 Added ReadOnly property
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2021-04-26: V3.0.3 Added default event CheckedChanged
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-11-07: V3.5.2 Added option to modify icon size
 * 2023-12-04: V3.6.1 Added property to modify icon size
 * 2024-08-26: V3.6.9 Fixed issue where AutoSize did not automatically display when text changed, #IAKYX4
 * 2024-08-30: V3.7.0 Modified AutoSize property to be saved in Design.cs file, #IAKYX4
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Checkbox
    /// </summary>
    [DefaultEvent("CheckedChanged")]
    [DefaultProperty("Checked")]
    [ToolboxItem(true)]
    public class UICheckBox : UIControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UICheckBox()
        {
            SetStyleFlags();
            base.Cursor = Cursors.Hand;
            ShowRect = false;
            Size = new Size(150, 29);
            SetStyle(ControlStyles.StandardDoubleClick, UseDoubleClick);

            ForeColor = UIStyles.Blue.CheckBoxForeColor;
            fillColor = UIStyles.Blue.CheckBoxColor;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        /// <summary>
        /// Override paint method
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (AutoSize && Dock == DockStyle.None)
            {
                Size sf = TextRenderer.MeasureText(Text, Font);
                int w = sf.Width + CheckBoxSize + 7;
                int h = Math.Max(CheckBoxSize, sf.Height) + 5;
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

        /// <summary>
        /// Value changed event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Value</param>
        public delegate void OnValueChanged(object sender, bool value);

        /// <summary>
        /// Value changed event
        /// </summary>
        public event OnValueChanged ValueChanged;

        private int _imageSize = 16;
        private int _imageInterval = 3;

        [DefaultValue(16)]
        [Description("Icon size"), Category("SunnyUI")]
        [Browsable(true)]
        public int CheckBoxSize
        {
            get => _imageSize;
            set
            {
                _imageSize = Math.Max(value, 16);
                _imageSize = Math.Min(value, 64);
                Invalidate();
            }
        }

        /// <summary>
        /// Read-only property
        /// </summary>
        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Interval between icon and text
        /// </summary>
        [DefaultValue(3)]
        [Description("Interval between icon and text"), Category("SunnyUI")]
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

        /// <summary>
        /// Checked property
        /// </summary>
        [Description("Checked"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    ValueChanged?.Invoke(this, _checked);
                    CheckedChanged?.Invoke(this, new EventArgs());
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Checked changed event
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            // Fill text
            Color color = ForeColor;
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
            int ImageSize = CheckBoxSize;
            // Icon
            float top = (Height - ImageSize) / 2.0f;
            float left = Text.IsValid() ? ImageInterval : (Width - ImageSize) / 2.0f;
            Color color = Enabled ? fillColor : foreDisableColor;
            if (Checked)
            {
                g.FillRoundRectangle(color, new Rectangle((int)left, (int)top, ImageSize, ImageSize), 1);
                color = BackColor.IsValid() ? BackColor : Color.White;
                Point pt2 = new Point((int)(left + ImageSize * 2 / 5.0f), (int)(top + ImageSize * 3 / 4.0f) - (ImageSize.Div(10)));
                Point pt1 = new Point((int)left + 2 + ImageSize.Div(10), pt2.Y - (pt2.X - 2 - ImageSize.Div(10) - (int)left));
                Point pt3 = new Point((int)left + ImageSize - 2 - ImageSize.Div(10), pt2.Y - (ImageSize - pt2.X - 2 - ImageSize.Div(10)) - (int)left);

                PointF[] CheckMarkLine = { pt1, pt2, pt3 };
                using Pen pn = new Pen(color, 2);
                g.SetHighQuality();
                g.DrawLines(pn, CheckMarkLine);
                g.SetDefaultQuality();
            }
            else
            {
                using Pen pn = new Pen(color, 1);
                g.DrawRoundRectangle(pn, new Rectangle((int)left + 1, (int)top + 1, ImageSize - 2, ImageSize - 2), 1);
                g.DrawRectangle(pn, new Rectangle((int)left + 2, (int)top + 2, ImageSize - 4, ImageSize - 4));
            }
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnClick(EventArgs e)
        {
            if (!ReadOnly)
            {
                Checked = !Checked;
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
            ForeColor = uiColor.CheckBoxForeColor;
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color CheckBoxColor
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