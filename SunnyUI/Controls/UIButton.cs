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
 * File Name: UIButton.cs
 * File Description: Button
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-07-26: V2.2.6 Added Selected and selected color configuration
 * 2020-08-22: V2.2.7 Space key press background effect, added double-click event, solved the problem of slow response due to rapid clicks
 * 2020-09-14: V2.2.7 Tips color can be set
 * 2021-07-18: V3.0.5 Added ShowFocusColor to display Focus state
 * 2021-12-11: V3.0.9 Added gradient color
 * 2022-02-26: V3.1.1 Added AutoSize property
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-03-31: V3.1.2 Option to display light background
 * 2022-08-25: V3.2.3 Added single selection for buttons with the same GroupIndex in the same container
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-07-02: V3.3.9 Added direction selection for gradient color
 * 2023-11-24: V3.6.2 Fixed text color for LightStyle
 * 2023-12-06: V3.6.2 Fixed background color for LightStyle
 * 2024-02-22: V3.6.3 Added Alt shortcut key functionality for the & character in the button
 * 2024-02-23: V3.6.3 Added property editor for Text
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Button
    /// </summary>
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [ToolboxItem(true)]
    public class UIButton : UIControl, IButtonControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UIButton()
        {
            SetStyleFlags();
            TabStop = true;
            Width = 100;
            Height = 35;
            base.Cursor = Cursors.Hand;

            plainColor = UIStyles.Blue.PlainColor;

            foreHoverColor = UIStyles.Blue.ButtonForeHoverColor;
            forePressColor = UIStyles.Blue.ButtonForePressColor;
            foreSelectedColor = UIStyles.Blue.ButtonForeSelectedColor;

            rectHoverColor = UIStyles.Blue.ButtonRectHoverColor;
            rectPressColor = UIStyles.Blue.ButtonRectPressColor;
            rectSelectedColor = UIStyles.Blue.ButtonRectSelectedColor;

            fillHoverColor = UIStyles.Blue.ButtonFillHoverColor;
            fillPressColor = UIStyles.Blue.ButtonFillPressColor;
            fillSelectedColor = UIStyles.Blue.ButtonFillSelectedColor;
            SetStyle(ControlStyles.StandardDoubleClick, UseDoubleClick);

            UseMnemonic = true;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        [DefaultValue(true)]
        [Description("If true, the first character after the & symbol will be used as the button's mnemonic key"), Category("SunnyUI")]
        public bool UseMnemonic { get; set; }

        protected override bool ProcessMnemonic(char charCode)
        {
            if (UseMnemonic && CanProcessMnemonic() && IsMnemonic(charCode, Text))
            {
                PerformClick();
                return true;
            }

            return base.ProcessMnemonic(charCode);
        }

        private bool CanProcessMnemonic()
        {
            return UseMnemonic && CanSelect && Enabled && Visible && Parent != null;
        }

        public string TextWithoutMnemonics(string text)
        {
            if (text == null)
            {
                return null;
            }

            int index = text.IndexOf('&');

            if (index == -1)
            {
                return text;
            }

            StringBuilder str = new StringBuilder(text.Substring(0, index));
            for (; index < text.Length; ++index)
            {
                if (text[index] == '&')
                {
                    index++;    // Skip this & and copy the next character instead
                }
                if (index < text.Length)
                {
                    str.Append(text[index]);
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Whether to display light background
        /// </summary>
        [DefaultValue(false)]
        [Description("Whether to display light background"), Category("SunnyUI")]
        public bool LightStyle
        {
            get => lightStyle;
            set
            {
                if (lightStyle != value)
                {
                    lightStyle = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "243, 249, 255")]
        [Description("Light background"), Category("SunnyUI")]
        public Color LightColor
        {
            get => plainColor;
            set => SetPlainColor(value);
        }

        private bool autoSize;

        /// <summary>
        /// Auto size
        /// </summary>
        [Browsable(true), DefaultValue(false)]
        [Description("Auto size"), Category("SunnyUI")]
        public override bool AutoSize
        {
            get => autoSize;
            set
            {
                autoSize = value;
                Invalidate();
            }
        }

        private bool isClick;

        /// <summary>
        /// Perform click event
        /// </summary>
        public void PerformClick()
        {
            if (isClick) return;
            if (CanSelect && Enabled)
            {
                isClick = true;
                OnClick(EventArgs.Empty);
                isClick = false;
            }
        }

        /// <summary>
        /// Override click event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnClick(EventArgs e)
        {
            Form form = FindFormInternal();
            if (form != null) form.DialogResult = DialogResult;

            Focus();
            base.OnClick(e);
        }

        internal Form FindFormInternal()
        {
            Control cur = this;
            while (cur != null && !(cur is Form))
            {
                cur = cur.Parent;
            }

            return (Form)cur;
        }

        private bool showTips = false;

        /// <summary>
        /// Whether to show tips
        /// </summary>
        [Description("Whether to show tips"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool ShowTips
        {
            get
            {
                return showTips;
            }
            set
            {
                if (showTips != value)
                {
                    showTips = value;
                    Invalidate();
                }
            }
        }

        private string tipsText = "";

        /// <summary>
        /// Tips text
        /// </summary>
        [Description("Tips text"), Category("SunnyUI")]
        [DefaultValue("")]
        public string TipsText
        {
            get { return tipsText; }
            set
            {
                if (tipsText != value)
                {
                    tipsText = value;
                    if (ShowTips)
                    {
                        Invalidate();
                    }
                }
            }
        }

        private Font tipsFont = UIStyles.SubFont();

        /// <summary>
        /// Tips text font
        /// </summary>
        [Description("Tips text font"), Category("SunnyUI")]
        [DefaultValue(typeof(Font), "Segoe UI, 9pt")]
        public Font TipsFont
        {
            get { return tipsFont; }
            set
            {
                if (!tipsFont.Equals(value))
                {
                    tipsFont = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            if (FillColorGradient)
            {
                if (IsHover || IsPress || Selected || Disabled)
                {
                    base.OnPaintFill(g, path);
                }
                else
                {
                    LinearGradientBrush br;
                    switch (fillColorGradientDirection)
                    {
                        case FlowDirection.LeftToRight:
                            br = new LinearGradientBrush(new Point(0, 0), new Point(Width, y: 0), FillColor, FillColor2);
                            break;
                        case FlowDirection.TopDown:
                            br = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), FillColor, FillColor2);
                            break;
                        case FlowDirection.RightToLeft:
                            br = new LinearGradientBrush(new Point(Width, 0), new Point(0, y: 0), FillColor, FillColor2);
                            break;
                        case FlowDirection.BottomUp:
                            br = new LinearGradientBrush(new Point(0, Height), new Point(0, 0), FillColor, FillColor2);
                            break;
                        default:
                            br = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), FillColor, FillColor2);
                            break;
                    }

                    br.GammaCorrection = true;
                    g.FillPath(br, path);
                    br?.Dispose();
                }
            }
            else
            {
                base.OnPaintFill(g, path);
            }
        }

        private Color tipsColor = Color.Red;

        /// <summary>
        /// Tips background color
        /// </summary>
        [Description("Tips background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Red")]
        public Color TipsColor
        {
            get => tipsColor;
            set
            {
                tipsColor = value;
                Invalidate();
            }
        }

        private Color tipsForeColor = Color.White;

        /// <summary>
        /// Tips text color
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI"), Description("Tips text color")]
        public Color TipsForeColor
        {
            get => tipsForeColor;
            set
            {
                tipsForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Override paint event
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (autoSize && Dock == DockStyle.None)
            {
                Size sf = TextRenderer.MeasureText(Text, Font);
                if (Width != sf.Width + 6) Width = sf.Width + 6;
                if (Height != sf.Height + 6) Height = sf.Height + 6;
            }

            if (Enabled && ShowTips && !string.IsNullOrEmpty(TipsText))
            {
                e.Graphics.SetHighQuality();
                using var TempFont = TipsFont.DPIScaleFont(TipsFont.Size);
                Size sf = TextRenderer.MeasureText(TipsText, TempFont);
                int sfMax = Math.Max(sf.Width, sf.Height);
                int x = Width - 1 - 2 - sfMax;
                int y = 1 + 1;
                e.Graphics.FillEllipse(TipsColor, x - 1, y, sfMax, sfMax);
                e.Graphics.DrawString(TipsText, TempFont, TipsForeColor, new Rectangle(x, y, sfMax, sfMax), ContentAlignment.MiddleCenter);
            }

            if (Focused && ShowFocusLine)
            {
                Rectangle rect = new Rectangle(2, 2, Width - 5, Height - 5);
                using var path = rect.CreateRoundedRectanglePath(Radius);
                using Pen pn = new Pen(ForeColor);
                pn.DashStyle = DashStyle.Dot;
                e.Graphics.DrawPath(pn, path);
            }
        }

        /// <summary>
        /// Whether selected
        /// </summary>
        [DefaultValue(false)]
        [Description("Whether selected"), Category("SunnyUI")]
        public bool Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    Invalidate();

                    if (value && Parent != null)
                    {
                        if (this is UISymbolButton)
                        {
                            List<UISymbolButton> buttons = Parent.GetControls<UISymbolButton>();

                            foreach (var box in buttons)
                            {
                                if (box == this) continue;
                                if (box.GroupIndex != GroupIndex) continue;
                                if (box.Selected) box.Selected = false;
                            }

                            return;
                        }

                        if (this is UIButton)
                        {
                            List<UIButton> buttons = Parent.GetControls<UIButton>();

                            foreach (var box in buttons)
                            {
                                if (box is UISymbolButton) continue;
                                if (box == this) continue;
                                if (box.GroupIndex != GroupIndex) continue;
                                if (box.Selected) box.Selected = false;
                            }

                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            plainColor = uiColor.PlainColor;

            fillHoverColor = uiColor.ButtonFillHoverColor;
            rectHoverColor = uiColor.ButtonRectHoverColor;
            foreHoverColor = uiColor.ButtonForeHoverColor;

            fillPressColor = uiColor.ButtonFillPressColor;
            rectPressColor = uiColor.ButtonRectPressColor;
            forePressColor = uiColor.ButtonForePressColor;

            fillSelectedColor = uiColor.ButtonFillSelectedColor;
            foreSelectedColor = uiColor.ButtonForeSelectedColor;
            rectSelectedColor = uiColor.ButtonRectSelectedColor;
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color FillColor2
        {
            get => fillColor2;
            set => SetFillColor2(value);
        }

        /// <summary>
        /// Fill color gradient
        /// </summary>
        [Description("Fill color gradient"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool FillColorGradient
        {
            get => fillColorGradient;
            set
            {
                if (fillColorGradient != value)
                {
                    fillColorGradient = value;
                    Invalidate();
                }
            }
        }

        private FlowDirection fillColorGradientDirection = FlowDirection.TopDown;

        [Description("Fill color gradient direction"), Category("SunnyUI")]
        [DefaultValue(FlowDirection.TopDown)]
        public FlowDirection FillColorGradientDirection
        {
            get => fillColorGradientDirection;
            set
            {
                if (fillColorGradientDirection != value)
                {
                    fillColorGradientDirection = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RectColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

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

        /// <summary>
        /// Fill color when disabled
        /// </summary>
        [DefaultValue(typeof(Color), "244, 244, 244"), Category("SunnyUI")]
        [Description("Fill color when disabled")]
        public Color FillDisableColor
        {
            get => fillDisableColor;
            set => SetFillDisableColor(value);
        }

        /// <summary>
        /// Border color when disabled
        /// </summary>
        [DefaultValue(typeof(Color), "173, 178, 181"), Category("SunnyUI")]
        [Description("Border color when disabled")]
        public Color RectDisableColor
        {
            get => rectDisableColor;
            set => SetRectDisableColor(value);
        }

        /// <summary>
        /// Font color when disabled
        /// </summary>
        [DefaultValue(typeof(Color), "109, 109, 103"), Category("SunnyUI")]
        [Description("Font color when disabled")]
        public Color ForeDisableColor
        {
            get => foreDisableColor;
            set => SetForeDisableColor(value);
        }

        /// <summary>
        /// Fill color when mouse hovers
        /// </summary>
        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Fill color when mouse hovers")]
        public Color FillHoverColor
        {
            get => fillHoverColor;
            set => SetFillHoverColor(value);
        }

        /// <summary>
        /// Fill color when mouse presses
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Fill color when mouse presses")]
        public Color FillPressColor
        {
            get => fillPressColor;
            set => SetFillPressColor(value);
        }

        /// <summary>
        /// Font color when mouse hovers
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Font color when mouse hovers")]
        public Color ForeHoverColor
        {
            get => foreHoverColor;
            set => SetForeHoverColor(value);
        }

        /// <summary>
        /// Font color when mouse presses
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Font color when mouse presses")]
        public Color ForePressColor
        {
            get => forePressColor;
            set => SetForePressColor(value);
        }

        /// <summary>
        /// Border color when mouse hovers
        /// </summary>
        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Border color when mouse hovers")]
        public Color RectHoverColor
        {
            get => rectHoverColor;
            set => SetRectHoverColor(value);
        }

        /// <summary>
        /// Border color when mouse presses
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Border color when mouse presses")]
        public Color RectPressColor
        {
            get => rectPressColor;
            set => SetRectPressColor(value);
        }

        /// <summary>
        /// Fill color when selected
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Fill color when selected")]
        public Color FillSelectedColor
        {
            get => fillSelectedColor;
            set => SetFillSelectedColor(value);
        }

        /// <summary>
        /// Font color when selected
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Font color when selected")]
        public Color ForeSelectedColor
        {
            get => foreSelectedColor;
            set => SetForeSelectedColor(value);
        }

        /// <summary>
        /// Border color when selected
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Border color when selected")]
        public Color RectSelectedColor
        {
            get => rectSelectedColor;
            set => SetRectSelectedColor(value);
        }

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            IsPress = true;
            Invalidate();
        }

        /// <summary>
        /// Override mouse up event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            IsPress = false;
            Invalidate();
        }

        /// <summary>
        /// Override mouse leave event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsPress = false;
            IsHover = false;
            Invalidate();
        }

        /// <summary>
        /// Override mouse enter event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsHover = true;
            Invalidate();
        }

        /// <summary>
        /// Notify control that it is the default button
        /// </summary>
        /// <param name="value"></param>
        public void NotifyDefault(bool value)
        {
        }

        /// <summary>
        /// Specify identifier to indicate the return value of the dialog
        /// </summary>
        [DefaultValue(DialogResult.None)]
        [Description("Specify identifier to indicate the return value of the dialog"), Category("SunnyUI")]
        public DialogResult DialogResult { get; set; } = DialogResult.None;

        /// <summary>
        /// Key down event
        /// </summary>
        /// <param name="e">Key event arguments</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Focused && e.KeyCode == Keys.Space)
            {
                IsPress = true;
                Invalidate();
                PerformClick();
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Key up event
        /// </summary>
        /// <param name="e">Key event arguments</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            IsPress = false;
            Invalidate();

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Show focus line when activated
        /// </summary>
        [DefaultValue(false)]
        [Description("Show focus line when activated"), Category("SunnyUI")]
        public bool ShowFocusLine { get; set; }

        [DefaultValue(0)]
        [Description("Group index"), Category("SunnyUI")]
        public int GroupIndex { get; set; }

        [Description("Text return value"), Category("SunnyUI")]
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }
    }
}