/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Class Library, Extension Class Library, Multi-page Development Framework.
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
 * File Name: UITextBox.cs
 * File Description: Input Box
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-06-03: V2.2.5 Added multiline, added scrollbar
 * 2020-09-03: V2.2.7 Added FocusedSelectAll property, select all when activated
 * 2021-04-15: V3.0.3 Modified text to be centered
 * 2021-04-17: V3.0.3 Removed height restriction based on font calculation, can be adjusted, solved the issue of not being able to enter a newline in multiline input
 * 2021-04-18: V3.0.3 Added ShowScrollBar property to control vertical scrollbar separately
 * 2021-06-01: V3.0.4 Added icon and font icon display
 * 2021-07-18: V3.0.5 Modified Focus to be usable
 * 2021-08-03: V3.0.5 Added GotFocus and LostFocus events
 * 2021-08-15: V3.0.6 Rewrote the watermark text drawing method and added watermark text color
 * 2021-09-07: V3.0.6 Added button
 * 2021-10-14: V3.0.8 Adjusted minimum height restriction
 * 2021-10-15: V3.0.8 Supported background color modification
 * 2022-01-07: V3.1.0 Button supports custom color
 * 2022-02-16: V3.1.1 Added read-only color setting
 * 2022-03-14: V3.1.1 Added scrollbar color setting
 * 2022-04-11: V3.1.3 Added ToolTip setting for button
 * 2022-06-10: V3.1.9 Redraw on size change
 * 2022-06-23: V3.2.0 Rewrote watermark text to solve the issue of whitening under different background colors
 * 2022-07-17: V3.2.1 Added SelectionChanged event
 * 2022-07-28: V3.2.2 Fixed the issue of not responding to Click and DoubleClick events when there is watermark text
 * 2022-09-05: V3.2.3 Fixed the issue of cursor sometimes not showing when there is no watermark text
 * 2022-09-16: V3.2.4 Supported custom right-click menu
 * 2022-09-16: V3.2.4 Fixed the issue of the right-side button not showing
 * 2022-11-03: V3.2.6 Added property to set vertical scrollbar width
 * 2022-11-12: V3.2.8 Changed integer and floating-point size validation from leaving to real-time input validation
 * 2022-11-12: V3.2.8 Removed MaximumEnabled, MinimumEnabled, HasMaximum, HasMinimum properties
 * 2022-11-26: V3.2.9 Added MouseClick, MouseDoubleClick events
 * 2023-02-07: V3.3.1 Added Tips red dot
 * 2023-02-10: V3.3.2 Added TouchPressClick property to respond to touch screen events when there is watermark, default is off
 * 2023-06-14: V3.3.9 Fixed button icon position
 * 2023-07-03: V3.3.9 Added support for changing text color when Enabled is false
 * 2023-07-16: V3.4.0 Fixed the issue of PasswordChar not working when Enabled is false
 * 2023-08-17: V3.4.1 Fixed the issue of text position changing after font size adjustment when Enabled is false
 * 2023-08-24: V3.4.2 Fixed the issue of text not showing when custom color is set and Enabled is false
 * 2023-10-25: V3.5.1 Fixed the issue of text not being vertically centered under high DPI
 * 2023-10-25: V3.5.1 Fixed the issue of underline not showing for some fonts
 * 2023-10-26: V3.5.1 Added SymbolRotate parameter for font icon rotation
 * 2023-11-16: V3.5.2 Refactored theme
 * 2023-12-18: V3.6.2 Fixed height not changing with font
 * 2023-12-18: V3.6.2 Modified Tips red dot position when button is shown
 * 2023-12-25: V3.6.2 Added property editor for Text
 * 2024-01-13: V3.6.3 Adjusted text box position when Radius is changed
 * 2024-06-11: V3.6.6 Made inheritable
 * 2024-08-12: V3.6.8 Solved the issue of incomplete display of native control font in Microsoft YaHei
 * 2024-08-26: V3.6.9 Fixed the issue of incomplete display of Microsoft YaHei font
 * 2024-08-27: V3.6.9 Auto adjust control height based on font when AutoSize is true
******************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    public partial class UITextBox : UIPanel, ISymbol, IToolTip
    {
        private readonly UIEdit edit = new UIEdit();
        private readonly UIScrollBar bar = new UIScrollBar();
        private readonly UISymbolButton btn = new UISymbolButton();

        public UITextBox()
        {
            InitializeComponent();
            InitializeComponentEnd = true;
            SetStyleFlags(true, true, true);

            ShowText = false;
            MinimumSize = new Size(1, 16);

            edit.AutoSize = false;
            edit.Top = (Height - edit.Height) / 2;
            edit.Left = 4;
            edit.Width = Width - 8;
            edit.Text = String.Empty;
            edit.BorderStyle = BorderStyle.None;
            edit.TextChanged += Edit_TextChanged;
            edit.KeyDown += Edit_OnKeyDown;
            edit.KeyUp += Edit_OnKeyUp;
            edit.KeyPress += Edit_OnKeyPress;
            edit.MouseEnter += Edit_MouseEnter;
            edit.Click += Edit_Click;
            edit.DoubleClick += Edit_DoubleClick;
            edit.Leave += Edit_Leave;
            edit.Validated += Edit_Validated;
            edit.Validating += Edit_Validating;
            edit.GotFocus += Edit_GotFocus;
            edit.LostFocus += Edit_LostFocus;
            edit.MouseLeave += Edit_MouseLeave;
            edit.MouseWheel += Edit_MouseWheel;
            edit.MouseDown += Edit_MouseDown;
            edit.MouseUp += Edit_MouseUp;
            edit.MouseMove += Edit_MouseMove;
            edit.SelectionChanged += Edit_SelectionChanged;
            edit.MouseClick += Edit_MouseClick;
            edit.MouseDoubleClick += Edit_MouseDoubleClick;
            edit.SizeChanged += Edit_SizeChanged;
            edit.FontChanged += Edit_FontChanged;

            btn.Parent = this;
            btn.Visible = false;
            btn.Text = "";
            btn.Symbol = 361761;
            btn.Top = 1;
            btn.Height = 25;
            btn.Width = 29;
            btn.BackColor = Color.Transparent;
            btn.Click += Btn_Click;
            btn.Radius = 3;
            btn.SymbolOffset = new Point(-1, 1);

            edit.Invalidate();
            Controls.Add(edit);
            fillColor = Color.White;

            bar.Parent = this;
            bar.Dock = DockStyle.None;
            bar.Visible = false;
            bar.ValueChanged += Bar_ValueChanged;
            bar.MouseEnter += Bar_MouseEnter;
            TextAlignment = ContentAlignment.MiddleLeft;

            lastEditHeight = edit.Height;
            Width = 150;
            Height = 29;

            editCursor = Cursor;
            TextAlignmentChange += UITextBox_TextAlignmentChange;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        private void Edit_FontChanged(object sender, EventArgs e)
        {
            if (!edit.Multiline)
            {
                int height = edit.Font.Height;
                edit.AutoSize = false;
                edit.Height = height + 2;
                SizeChange();
            }
        }

        int lastEditHeight = -1;
        private void Edit_SizeChanged(object sender, EventArgs e)
        {
            if (lastEditHeight != edit.Height)
            {
                lastEditHeight = edit.Height;
                SizeChange();
            }
        }

        public override void SetDPIScale()
        {
            base.SetDPIScale();
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;

            edit.SetDPIScale();
        }

        [Description("Enable to respond to some touch screen click events"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool TouchPressClick
        {
            get => edit.TouchPressClick;
            set => edit.TouchPressClick = value;
        }

        private bool _autoSize = false;
        public new bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                SizeChange();
            }
        }

        private UIButton tipsBtn;
        public void SetTipsText(ToolTip toolTip, string text)
        {
            if (tipsBtn == null)
            {
                tipsBtn = new UIButton();
                tipsBtn.Cursor = System.Windows.Forms.Cursors.Hand;
                tipsBtn.Size = new System.Drawing.Size(6, 6);
                tipsBtn.Style = Sunny.UI.UIStyle.Red;
                tipsBtn.StyleCustomMode = true;
                tipsBtn.Text = "";
                tipsBtn.Click += TipsBtn_Click;

                Controls.Add(tipsBtn);
                tipsBtn.Location = new System.Drawing.Point(Width - 8, 2);
                tipsBtn.BringToFront();
            }

            toolTip.SetToolTip(tipsBtn, text);
        }

        public event EventHandler TipsClick;
        private void TipsBtn_Click(object sender, EventArgs e)
        {
            TipsClick?.Invoke(this, EventArgs.Empty);
        }

        public void CloseTips()
        {
            if (tipsBtn != null)
            {
                tipsBtn.Click -= TipsBtn_Click;
                tipsBtn.Dispose();
                tipsBtn = null;
            }
        }

        public new event EventHandler MouseDoubleClick;
        public new event EventHandler MouseClick;

        private void Edit_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        private void Edit_MouseClick(object sender, MouseEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }

        private int scrollBarWidth = 0;

        [DefaultValue(0), Category("SunnyUI"), Description("Vertical scrollbar width, minimum is the native scrollbar width")]
        public int ScrollBarWidth
        {
            get => scrollBarWidth;
            set
            {
                scrollBarWidth = value;
                SetScrollInfo();
            }
        }

        private int scrollBarHandleWidth = 6;

        [DefaultValue(6), Category("SunnyUI"), Description("Vertical scrollbar handle width, minimum is the native scrollbar width")]
        public int ScrollBarHandleWidth
        {
            get => scrollBarHandleWidth;
            set
            {
                scrollBarHandleWidth = value;
                if (bar != null) bar.FillWidth = value;
            }
        }

        private void Edit_SelectionChanged(object sender, UITextBoxSelectionArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        public event OnSelectionChanged SelectionChanged;

        public void SetButtonToolTip(ToolTip toolTip, string tipText)
        {
            toolTip.SetToolTip(btn, tipText);
        }

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
            if (edit != null) edit.ContextMenuStrip = ContextMenuStrip;
        }

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or null value, it will not be filled
        /// </summary>
        [Description("Fill color, if the value is the background color or transparent color or null value, it will not be filled"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public new Color FillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    Invalidate();
                }

                AfterSetFillColor(value);
            }
        }

        /// <summary>
        /// Font read-only color
        /// </summary>
        [DefaultValue(typeof(Color), "109, 109, 103")]
        public Color ForeReadOnlyColor
        {
            get => foreReadOnlyColor;
            set => SetForeReadOnlyColor(value);
        }

        /// <summary>
        /// Border read-only color
        /// </summary>
        [DefaultValue(typeof(Color), "173, 178, 181")]
        public Color RectReadOnlyColor
        {
            get => rectReadOnlyColor;
            set => SetRectReadOnlyColor(value);
        }

        /// <summary>
        /// Fill read-only color
        /// </summary>
        [DefaultValue(typeof(Color), "244, 244, 244")]
        public Color FillReadOnlyColor
        {
            get => fillReadOnlyColor;
            set => SetFillReadOnlyColor(value);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }

        public event EventHandler ButtonClick;

        [DefaultValue(29), Category("SunnyUI"), Description("Button width")]
        public int ButtonWidth { get => btn.Width; set { btn.Width = Math.Max(20, value); SizeChange(); } }

        private bool showButton = false;
        [DefaultValue(false), Category("SunnyUI"), Description("Show button")]
        public bool ShowButton
        {
            get => showButton;
            set
            {
                showButton = !multiline && value;
                if (btn.IsValid()) btn.Visible = showButton;
                SizeChange();
            }
        }

        private void Edit_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        private void Edit_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        private void Edit_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        private void Edit_MouseLeave(object sender, EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        /// <summary>
        /// Controls that need additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        private void Edit_LostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(this, e);
        }

        private void Edit_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }

        private void Edit_Validating(object sender, CancelEventArgs e)
        {
            Validating?.Invoke(this, e);
        }

        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event MouseEventHandler MouseMove;
        public new event EventHandler GotFocus;
        public new event EventHandler LostFocus;
        public new event CancelEventHandler Validating;
        public new event EventHandler Validated;
        public new event EventHandler MouseLeave;
        public new event EventHandler DoubleClick;
        public new event EventHandler Click;
        [Browsable(true)]
        public new event EventHandler TextChanged;
        public new event KeyEventHandler KeyDown;
        public new event KeyEventHandler KeyUp;
        public new event KeyPressEventHandler KeyPress;
        public new event EventHandler Leave;

        private void Edit_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(this, e);
        }

        public new void Focus()
        {
            base.Focus();
            edit.Focus();
        }

        [Browsable(false)]
        public UIEdit TextBox => edit;

        private void Edit_Leave(object sender, EventArgs e)
        {
            Leave?.Invoke(this, e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            edit.BackColor = GetFillColor();

            edit.Visible = true;
            edit.Enabled = Enabled;
            if (!Enabled)
            {
                if (NeedDrawDisabledText) edit.Visible = false;
            }
        }

        private bool NeedDrawDisabledText => !Enabled && StyleCustomMode && (ForeDisableColor != Color.FromArgb(109, 109, 103) || FillDisableColor != Color.FromArgb(244, 244, 244));

        public override bool Focused => edit.Focused;

        [DefaultValue(false)]
        [Description("Select all text when activated"), Category("SunnyUI")]
        public bool FocusedSelectAll
        {
            get => edit.FocusedSelectAll;
            set => edit.FocusedSelectAll = value;
        }

        private void UITextBox_TextAlignmentChange(object sender, ContentAlignment alignment)
        {
            if (edit == null) return;
            if (alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.MiddleLeft ||
                alignment == ContentAlignment.BottomLeft)
                edit.TextAlign = HorizontalAlignment.Left;

            if (alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.MiddleCenter ||
                alignment == ContentAlignment.BottomCenter)
                edit.TextAlign = HorizontalAlignment.Center;

            if (alignment == ContentAlignment.TopRight || alignment == ContentAlignment.MiddleRight ||
                alignment == ContentAlignment.BottomRight)
                edit.TextAlign = HorizontalAlignment.Right;
        }

        private void Edit_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        protected override void OnCursorChanged(EventArgs e)
        {
            base.OnCursorChanged(e);
            edit.Cursor = Cursor;
        }

        private Cursor editCursor;

        private void Bar_MouseEnter(object sender, EventArgs e)
        {
            editCursor = Cursor;
            Cursor = Cursors.Default;
        }

        private void Edit_MouseEnter(object sender, EventArgs e)
        {
            Cursor = editCursor;
            if (FocusedSelectAll)
            {
                SelectAll();
            }
        }

        private void Edit_MouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
            if (bar != null && bar.Visible && edit != null)
            {
                var si = ScrollBarInfo.GetInfo(edit.Handle);
                if (e.Delta > 10)
                {
                    if (si.nPos > 0)
                    {
                        ScrollBarInfo.ScrollUp(edit.Handle);
                    }
                }
                else if (e.Delta < -10)
                {
                    if (si.nPos < si.ScrollMax)
                    {
                        ScrollBarInfo.ScrollDown(edit.Handle);
                    }
                }
            }

            SetScrollInfo();
        }

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            if (edit != null)
            {
                ScrollBarInfo.SetScrollValue(edit.Handle, bar.Value);
            }
        }

        private bool multiline = false;

        [DefaultValue(false)]
        public bool Multiline
        {
            get => multiline;
            set
            {
                multiline = value;
                edit.Multiline = value;
                // edit.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
                // bar.Visible = multiline;

                if (value && Type != UIEditType.String)
                {
                    Type = UIEditType.String;
                }

                SizeChange();
            }
        }

        private bool showScrollBar;

        [DefaultValue(false)]
        [Description("Show vertical scrollbar"), Category("SunnyUI")]
        public bool ShowScrollBar
        {
            get => showScrollBar;
            set
            {
                value = value && Multiline;
                showScrollBar = value;
                if (value)
                {
                    edit.ScrollBars = ScrollBars.Vertical;
                    bar.Visible = true;
                }
                else
                {
                    edit.ScrollBars = ScrollBars.None;
                    bar.Visible = false;
                }
            }
        }

        [DefaultValue(true)]
        public bool WordWarp
        {
            get => edit.WordWrap;
            set => edit.WordWrap = value;
        }

        public void Select(int start, int length)
        {
            edit.Focus();
            edit.Select(start, length);
        }

        public void ScrollToCaret()
        {
            edit.ScrollToCaret();
        }

        private void Edit_OnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        private void Edit_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoEnter?.Invoke(this, e);
            }

            KeyDown?.Invoke(this, e);
        }

        public event EventHandler DoEnter;

        private void Edit_OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        [DefaultValue(null)]
        [Description("Watermark text"), Category("SunnyUI")]
        public string Watermark
        {
            get => edit.Watermark;
            set => edit.Watermark = value;
        }

        [DefaultValue(typeof(Color), "Gray")]
        [Description("Watermark text color"), Category("SunnyUI")]
        public Color WatermarkColor
        {
            get => edit.WaterMarkColor;
            set => edit.WaterMarkColor = value;
        }

        [DefaultValue(typeof(Color), "Gray")]
        [Description("Watermark text active color"), Category("SunnyUI")]
        public Color WatermarkActiveColor
        {
            get => edit.WaterMarkActiveForeColor;
            set => edit.WaterMarkActiveForeColor = value;
        }

        public void SelectAll()
        {
            edit.Focus();
            edit.SelectAll();
        }

        internal void CheckMaxMin()
        {
            edit.CheckMaxMin();
        }

        private void Edit_TextChanged(object s, EventArgs e)
        {
            if (IsDisposed) return;
            TextChanged?.Invoke(this, e);
            if (Multiline) SetScrollInfo();
        }

        /// <summary>
        /// Override font change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            if (DefaultFontSize < 0 && edit != null)
            {
                edit.Font = this.Font;
            }

            Invalidate();
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SizeChange();
        }

        public void SetScrollInfo()
        {
            if (bar == null)
            {
                return;
            }

            var si = ScrollBarInfo.GetInfo(edit.Handle);
            bar.ThreadSafeCall(() =>
            {
                if (si.ScrollMax > 0)
                {
                    bar.Maximum = si.ScrollMax;
                    bar.Value = si.nPos;
                }
                else
                {
                    bar.Maximum = si.ScrollMax;
                }
            });
        }

        protected override void OnRadiusChanged(int value)
        {
            base.OnRadiusChanged(value);
            SizeChange();
        }

        private void SizeChange()
        {
            if (!InitializeComponentEnd) return;
            if (edit == null) return;
            if (btn == null) return;

            if (!multiline)
            {
                // Single line display

                // AutoSize automatically sets the height
                if (Dock == DockStyle.None && AutoSize)
                {
                    if (Height != edit.Height + 5)
                        Height = edit.Height + 5;
                }

                // Vertically center the edit box based on font size
                if (edit.Top != (Height - edit.Height) / 2 + 1)
                {
                    edit.Top = (Height - edit.Height) / 2 + 1;
                }

                int added = Radius <= 5 ? 0 : (Radius - 5) / 2;

                if (icon == null && Symbol == 0)
                {
                    edit.Left = 4;
                    edit.Width = Width - 8;
                    edit.Left = edit.Left + added;
                    edit.Width = edit.Width - added * 2;
                }
                else
                {
                    if (icon != null)
                    {
                        edit.Left = 4 + iconSize;
                        edit.Width = Width - 8 - iconSize - added;
                    }
                    else if (Symbol > 0)
                    {
                        edit.Left = 4 + SymbolSize;
                        edit.Width = Width - 8 - SymbolSize - added;
                    }
                }

                btn.Left = Width - 2 - ButtonWidth - added;
                btn.Top = 2;
                btn.Height = Height - 4;

                if (ShowButton)
                {
                    edit.Width = edit.Width - btn.Width - 3 - added;
                }

                if (tipsBtn != null)
                {
                    if (ShowButton)
                        tipsBtn.Location = new System.Drawing.Point(Width - btn.Width - 10 - added, 2);
                    else
                        tipsBtn.Location = new System.Drawing.Point(Width - 8 - added, 2);
                }
            }
            else
            {
                btn.Visible = false;
                edit.Top = 3;
                edit.Height = Height - 6;
                edit.Left = 4;
                edit.Width = Width - 8;

                int barWidth = Math.Max(ScrollBarInfo.VerticalScrollBarWidth() + 2, ScrollBarWidth);
                bar.Top = 2;
                bar.Width = barWidth + 1;
                bar.Left = Width - barWidth - 3;
                bar.Height = Height - 4;
                bar.BringToFront();

                SetScrollInfo();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            edit.Focus();
        }

        public void Clear()
        {
            edit.Clear();
        }

        [DefaultValue('\0')]
        [Description("Password mask"), Category("SunnyUI")]
        public char PasswordChar
        {
            get => edit.PasswordChar;
            set => edit.PasswordChar = value;
        }

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly
        {
            get => isReadOnly;
            set
            {
                isReadOnly = value;
                edit.ReadOnly = value;
                edit.BackColor = GetFillColor();
                Invalidate();
            }
        }

        [Description("Input type"), Category("SunnyUI")]
        [DefaultValue(UIEditType.String)]
        public UIEditType Type
        {
            get => edit.Type;
            set => edit.Type = value;
        }

        /// <summary>
        /// When InputType is a numeric type, the maximum value that can be entered
        /// </summary>
        [Description("When InputType is a numeric type, the maximum value that can be entered."), Category("SunnyUI")]
        [DefaultValue(2147483647D)]
        public double Maximum
        {
            get => edit.MaxValue;
            set => edit.MaxValue = value;
        }

        /// <summary>
        /// When InputType is a numeric type, the minimum value that can be entered
        /// </summary>
        [Description("When InputType is a numeric type, the minimum value that can be entered."), Category("SunnyUI")]
        [DefaultValue(-2147483648D)]
        public double Minimum
        {
            get => edit.MinValue;
            set => edit.MinValue = value;
        }

        [DefaultValue(0.00)]
        [Description("Floating-point return value"), Category("SunnyUI")]
        public double DoubleValue
        {
            get => edit.DoubleValue;
            set => edit.DoubleValue = value;
        }

        [DefaultValue(0)]
        [Description("Integer return value"), Category("SunnyUI")]
        public int IntValue
        {
            get => edit.IntValue;
            set => edit.IntValue = value;
        }

        [Description("Text return value"), Category("SunnyUI")]
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public override string Text
        {
            get => edit.Text;
            set => edit.Text = value;
        }

        [Description("Floating-point, number of decimal places to display"), Category("SunnyUI")]
        [DefaultValue(2)]
        public int DecimalPlaces
        {
            get => edit.DecLength;
            set => edit.DecLength = Math.Max(value, 0);
        }

        [DefaultValue(false)]
        [Description("Whether it can be empty when entering an integer or floating-point number"), Category("SunnyUI")]
        public bool CanEmpty
        {
            get => edit.CanEmpty;
            set => edit.CanEmpty = value;
        }

        public void Empty()
        {
            if (edit.CanEmpty)
                edit.Text = "";
        }

        public bool IsEmpty => edit.Text == "";

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            ActiveControl = edit;
        }

        [DefaultValue(32767)]
        public int MaxLength
        {
            get => edit.MaxLength;
            set => edit.MaxLength = Math.Max(value, 1);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            fillColor = uiColor.EditorBackColor;
            foreColor = UIFontColor.Primary;
            edit.BackColor = GetFillColor();
            edit.ForeColor = GetForeColor();
            edit.ForeDisableColor = uiColor.ForeDisableColor;

            if (bar != null && bar.Style == UIStyle.Inherited)
            {
                bar.ForeColor = uiColor.PrimaryColor;
                bar.HoverColor = uiColor.ButtonFillHoverColor;
                bar.PressColor = uiColor.ButtonFillPressColor;
                bar.FillColor = fillColor;
                scrollBarColor = uiColor.PrimaryColor;
                scrollBarBackColor = fillColor;
            }

            if (btn != null && btn.Style == UIStyle.Inherited)
            {
                btn.ForeColor = uiColor.ButtonForeColor;
                btn.FillColor = uiColor.ButtonFillColor;
                btn.RectColor = uiColor.RectColor;

                btn.FillHoverColor = uiColor.ButtonFillHoverColor;
                btn.RectHoverColor = uiColor.ButtonRectHoverColor;
                btn.ForeHoverColor = uiColor.ButtonForeHoverColor;

                btn.FillPressColor = uiColor.ButtonFillPressColor;
                btn.RectPressColor = uiColor.ButtonRectPressColor;
                btn.ForePressColor = uiColor.ButtonForePressColor;
            }
        }

        /// <summary>
        /// Scrollbar theme style
        /// </summary>
        [DefaultValue(true), Description("Scrollbar theme style"), Category("SunnyUI")]
        public bool ScrollBarStyleInherited
        {
            get => bar != null && bar.Style == UIStyle.Inherited;
            set
            {
                if (value)
                {
                    if (bar != null) bar.Style = UIStyle.Inherited;
                    scrollBarColor = UIStyles.Blue.PrimaryColor;
                    scrollBarBackColor = UIStyles.Blue.EditorBackColor;
                }

            }
        }

        protected override void SetForeDisableColor(Color color)
        {
            base.SetForeDisableColor(color);
            edit.ForeDisableColor = color;
        }

        private Color scrollBarColor = Color.FromArgb(80, 160, 255);

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or null value, it will not be filled
        /// </summary>
        [Description("Scrollbar fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ScrollBarColor
        {
            get => scrollBarColor;
            set
            {
                scrollBarColor = value;
                bar.HoverColor = bar.PressColor = bar.ForeColor = value;
                bar.Style = UIStyle.Custom;
                Invalidate();
            }
        }

        private Color scrollBarBackColor = Color.White;

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or null value, it will not be filled
        /// </summary>
        [Description("Scrollbar background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ScrollBarBackColor
        {
            get => scrollBarBackColor;
            set
            {
                scrollBarBackColor = value;
                bar.FillColor = value;
                bar.Style = UIStyle.Custom;
                Invalidate();
            }
        }

        protected override void AfterSetForeColor(Color color)
        {
            base.AfterSetForeColor(color);
            edit.ForeColor = GetForeColor();
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            edit.BackColor = GetFillColor();
            bar.FillColor = color;
        }

        protected override void AfterSetFillReadOnlyColor(Color color)
        {
            base.AfterSetFillReadOnlyColor(color);
            edit.BackColor = GetFillColor();
        }

        protected override void AfterSetForeReadOnlyColor(Color color)
        {
            base.AfterSetForeReadOnlyColor(color);
            edit.ForeColor = GetForeColor();
        }

        public enum UIEditType
        {
            /// <summary>
            /// String
            /// </summary>
            String,

            /// <summary>
            /// Integer
            /// </summary>
            Integer,

            /// <summary>
            /// Floating-point
            /// </summary>
            Double
        }

        [DefaultValue(false)]
        public bool AcceptsReturn
        {
            get => edit.AcceptsReturn;
            set => edit.AcceptsReturn = value;
        }

        [DefaultValue(AutoCompleteMode.None), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get => edit.AutoCompleteMode;
            set => edit.AutoCompleteMode = value;
        }

        [
            DefaultValue(AutoCompleteSource.None),
            TypeConverterAttribute(typeof(TextBoxAutoCompleteSourceConverter)),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource
        {
            get => edit.AutoCompleteSource;
            set => edit.AutoCompleteSource = value;
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            Localizable(true),
            Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => edit.AutoCompleteCustomSource;
            set => edit.AutoCompleteCustomSource = value;
        }

        [DefaultValue(CharacterCasing.Normal)]
        public CharacterCasing CharacterCasing
        {
            get => edit.CharacterCasing;
            set => edit.CharacterCasing = value;
        }

        public void Paste(string text)
        {
            edit.Paste(text);
        }

        internal class TextBoxAutoCompleteSourceConverter : EnumConverter
        {
            public TextBoxAutoCompleteSourceConverter(Type type) : base(type)
            {
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection values = base.GetStandardValues(context);
                ArrayList list = new ArrayList();
                int count = values.Count;
                for (int i = 0; i < count; i++)
                {
                    string currentItemText = values[i].ToString();
                    if (currentItemText != null && !currentItemText.Equals("ListItems"))
                    {
                        list.Add(values[i]);
                    }
                }

                return new StandardValuesCollection(list);
            }
        }

        [DefaultValue(false)]
        public bool AcceptsTab
        {
            get => edit.AcceptsTab;
            set => edit.AcceptsTab = value;
        }

        [DefaultValue(false)]
        public bool EnterAsTab
        {
            get => edit.EnterAsTab;
            set => edit.EnterAsTab = value;
        }

        [DefaultValue(true)]
        public bool ShortcutsEnabled
        {
            get => edit.ShortcutsEnabled;
            set => edit.ShortcutsEnabled = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanUndo
        {
            get => edit.CanUndo;
        }

        [DefaultValue(true)]
        public bool HideSelection
        {
            get => edit.HideSelection;
            set => edit.HideSelection = value;
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            MergableProperty(false),
            Localizable(true),
            Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))
        ]
        public string[] Lines
        {
            get => edit.Lines;
            set => edit.Lines = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get => edit.Modified;
            set => edit.Modified = value;
        }

        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int PreferredHeight
        {
            get => edit.PreferredHeight;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get => edit.SelectedText;
            set => edit.SelectedText = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get => edit.SelectionLength;
            set => edit.SelectionLength = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get => edit.SelectionStart;
            set => edit.SelectionStart = value;
        }

        [Browsable(false)]
        public int TextLength
        {
            get => edit.TextLength;
        }

        public void AppendText(string text)
        {
            edit.AppendText(text);
        }

        public void ClearUndo()
        {
            edit.ClearUndo();
        }

        public void Copy()
        {
            edit.Copy();
        }

        public void Cut()
        {
            edit.Cut();
        }

        public void Paste()
        {
            edit.Paste();
        }

        public char GetCharFromPosition(Point pt)
        {
            return edit.GetCharFromPosition(pt);
        }

        public int GetCharIndexFromPosition(Point pt)
        {
            return edit.GetCharIndexFromPosition(pt);
        }

        public int GetLineFromCharIndex(int index)
        {
            return edit.GetLineFromCharIndex(index);
        }

        public Point GetPositionFromCharIndex(int index)
        {
            return edit.GetPositionFromCharIndex(index);
        }

        public int GetFirstCharIndexFromLine(int lineNumber)
        {
            return edit.GetFirstCharIndexFromLine(lineNumber);
        }

        public int GetFirstCharIndexOfCurrentLine()
        {
            return edit.GetFirstCharIndexOfCurrentLine();
        }

        public void DeselectAll()
        {
            edit.DeselectAll();
        }

        public void Undo()
        {
            edit.Undo();
        }

        private Image icon;
        [Description("Icon"), Category("SunnyUI")]
        [DefaultValue(null)]
        public Image Icon
        {
            get => icon;
            set
            {
                icon = value;
                SizeChange();
                Invalidate();
            }
        }

        private int iconSize = 24;
        [Description("Icon size (square)"), Category("SunnyUI"), DefaultValue(24)]
        public int IconSize
        {
            get => iconSize;
            set
            {
                iconSize = Math.Min(UIGlobal.EditorMinHeight, value);
                SizeChange();
                Invalidate();
            }
        }

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (multiline) return;
            if (icon != null)
            {
                e.Graphics.DrawImage(icon, new Rectangle(4, (Height - iconSize) / 2, iconSize, iconSize), new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
            }
            else if (Symbol != 0)
            {
                e.Graphics.DrawFontImage(Symbol, SymbolSize, SymbolColor, new Rectangle(4 + symbolOffset.X, (Height - SymbolSize) / 2 + 1 + symbolOffset.Y, SymbolSize, SymbolSize), SymbolOffset.X, SymbolOffset.Y, SymbolRotate);
            }

            if (Text.IsValid() && NeedDrawDisabledText)
            {
                string text = Text;
                if (PasswordChar > 0)
                {
                    text = PasswordChar.ToString().Repeat(text.Length);
                }

                ContentAlignment textAlign = ContentAlignment.MiddleLeft;
                if (TextAlignment == ContentAlignment.TopCenter || TextAlignment == ContentAlignment.MiddleCenter || TextAlignment == ContentAlignment.BottomCenter)
                    textAlign = ContentAlignment.MiddleCenter;

                if (TextAlignment == ContentAlignment.TopRight || TextAlignment == ContentAlignment.MiddleRight || TextAlignment == ContentAlignment.BottomRight)
                    textAlign = ContentAlignment.MiddleRight;

                e.Graphics.DrawString(text, edit.Font, ForeDisableColor, edit.Bounds, textAlign);
            }
        }

        public Color _symbolColor = UIFontColor.Primary;

        /// <summary>
        /// Font icon color
        /// </summary>
        [DefaultValue(typeof(Color), "48, 48, 48")]
        [Description("Font icon color"), Category("SunnyUI")]
        public Color SymbolColor
        {
            get => _symbolColor;
            set
            {
                _symbolColor = value;
                Invalidate();
            }
        }

        private int _symbol;

        /// <summary>
        /// Font icon
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(0)]
        [Description("Font icon"), Category("SunnyUI")]
        public int Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                SizeChange();
                Invalidate();
            }
        }

        private int _symbolSize = 24;

        /// <summary>
        /// Font icon size
        /// </summary>
        [DefaultValue(24)]
        [Description("Font icon size"), Category("SunnyUI")]
        public int SymbolSize
        {
            get => _symbolSize;
            set
            {
                _symbolSize = Math.Max(value, 16);
                _symbolSize = Math.Min(value, UIGlobal.EditorMaxHeight);
                SizeChange();
                Invalidate();
            }
        }

        private Point symbolOffset = new Point(0, 0);

        /// <summary>
        /// Offset position of the font icon
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Offset position of the font icon"), Category("SunnyUI")]
        public Point SymbolOffset
        {
            get => symbolOffset;
            set
            {
                symbolOffset = value;
                Invalidate();
            }
        }

        private int _symbolRotate = 0;

        /// <summary>
        /// Font icon rotation angle
        /// </summary>
        [DefaultValue(0)]
        [Description("Font icon rotation angle"), Category("SunnyUI")]
        public int SymbolRotate
        {
            get => _symbolRotate;
            set
            {
                if (_symbolRotate != value)
                {
                    _symbolRotate = value;
                    Invalidate();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(361761)]
        [Description("Button font icon"), Category("SunnyUI")]
        public int ButtonSymbol
        {
            get => btn.Symbol;
            set => btn.Symbol = value;
        }

        [DefaultValue(24)]
        [Description("Button font icon size"), Category("SunnyUI")]
        public int ButtonSymbolSize
        {
            get => btn.SymbolSize;
            set => btn.SymbolSize = value;
        }

        [DefaultValue(typeof(Point), "-1, 1")]
        [Description("Offset position of the button font icon"), Category("SunnyUI")]
        public Point ButtonSymbolOffset
        {
            get => btn.SymbolOffset;
            set => btn.SymbolOffset = value;
        }

        /// <summary>
        /// Font icon rotation angle
        /// </summary>
        [DefaultValue(0)]
        [Description("Button font icon rotation angle"), Category("SunnyUI")]
        public int ButtonSymbolRotate
        {
            get => btn.SymbolRotate;
            set => btn.SymbolRotate = value;
        }

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or null value, it will not be filled
        /// </summary>
        [Description("Button fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonFillColor
        {
            get => btn.FillColor;
            set
            {
                btn.FillColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Button font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonForeColor
        {
            get => btn.ForeColor;
            set
            {
                btn.SymbolColor = btn.ForeColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Button border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonRectColor
        {
            get => btn.RectColor;
            set
            {
                btn.RectColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Button fill color when mouse hovers")]
        public Color ButtonFillHoverColor
        {
            get => btn.FillHoverColor;
            set
            {
                btn.FillHoverColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Button font color when mouse hovers")]
        public Color ButtonForeHoverColor
        {
            get => btn.ForeHoverColor;
            set
            {
                btn.SymbolHoverColor = btn.ForeHoverColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Button border color when mouse hovers")]
        public Color ButtonRectHoverColor
        {
            get => btn.RectHoverColor;
            set
            {
                btn.RectHoverColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Button fill color when mouse is pressed")]
        public Color ButtonFillPressColor
        {
            get => btn.FillPressColor;
            set
            {
                btn.FillPressColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Button font color when mouse is pressed")]
        public Color ButtonForePressColor
        {
            get => btn.ForePressColor;
            set
            {
                btn.SymbolPressColor = btn.ForePressColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Button border color when mouse is pressed")]
        public Color ButtonRectPressColor
        {
            get => btn.RectPressColor;
            set
            {
                btn.RectPressColor = value;
                btn.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Scrollbar theme style
        /// </summary>
        [DefaultValue(true), Description("Scrollbar theme style"), Category("SunnyUI")]
        public bool ButtonStyleInherited
        {
            get => btn != null && btn.Style == UIStyle.Inherited;
            set
            {
                if (value && btn != null)
                {
                    btn.Style = UIStyle.Inherited;
                }
            }
        }
    }
}