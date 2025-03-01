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
 * File Name: UIRichTextBox.cs
 * Description: Rich Text Input Box
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2021-05-25: V3.0.4 Support for changing background color
 * 2021-07-29: V3.0.5 Modified scrollbar to auto-hide when no text
 * 2022-02-23: V3.1.1 Added some native properties and events
 * 2022-03-14: V3.1.1 Added scrollbar color settings
 * 2022-11-03: V3.2.6 Added property to set vertical scrollbar width
 * 2023-11-13: V3.5.2 Refactored theme
 * 2023-12-25: V3.6.2 Added Text property editor
 * 2024-07-20: V3.6.8 Modified scrollbar color to follow theme
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    public sealed class UIRichTextBox : UIPanel, IToolTip
    {
        private UIScrollBar bar;
        private RichTextBox edit;

        public UIRichTextBox()
        {
            InitializeComponent();
            SetStyleFlags();
            ShowText = false;

            edit.MouseWheel += OnMouseWheel;
            edit.TextChanged += Edit_TextChanged;
            edit.KeyDown += EditOnKeyDown;
            edit.KeyUp += EditOnKeyUp;
            edit.KeyPress += EditOnKeyPress;
            edit.DoubleClick += Edit_DoubleClick;
            edit.Click += Edit_Click;

            bar.Parent = this;
            bar.Visible = false;
            bar.ValueChanged += Bar_ValueChanged;
            bar.MouseEnter += Bar_MouseEnter;
            SizeChange();

            edit.DragDrop += Edit_DragDrop;
            edit.DragEnter += Edit_DragEnter;
            edit.DragLeave += Edit_DragLeave;
            edit.DragOver += Edit_DragOver;
            edit.GiveFeedback += Edit_GiveFeedback;
            edit.QueryContinueDrag += Edit_QueryContinueDrag;
            edit.LinkClicked += Edit_LinkClicked;
            edit.Protected += Edit_Protected;
            edit.SelectionChanged += Edit_SelectionChanged;

            edit.ScrollBars = RichTextBoxScrollBars.Vertical;

            edit.Leave += Edit_Leave;
            edit.Validated += Edit_Validated;
            edit.Validating += Edit_Validating;
            edit.GotFocus += Edit_GotFocus;
            edit.LostFocus += Edit_LostFocus;
            edit.MouseLeave += Edit_MouseLeave;
            edit.MouseDown += Edit_MouseDown;
            edit.MouseUp += Edit_MouseUp;
            edit.MouseMove += Edit_MouseMove;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            edit?.Dispose();
            bar?.Dispose();
        }

        public new event EventHandler Leave;
        public new event EventHandler Validated;
        public new event CancelEventHandler Validating;
        public new event EventHandler GotFocus;
        public new event EventHandler LostFocus;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event MouseEventHandler MouseMove;
        public new event EventHandler MouseLeave;

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

        private void Edit_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(this, e);
        }

        private void Edit_Leave(object sender, EventArgs e)
        {
            Leave?.Invoke(this, e);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public new event EventHandler DoubleClick;
        public new event EventHandler Click;

        [
            DefaultValue(false),
            RefreshProperties(RefreshProperties.Repaint),
            Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public override bool AutoSize
        {
            get => edit.AutoSize;
            set => edit.AutoSize = value;
        }

        private void Edit_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        /// <summary>
        /// Control that needs additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        public void Clear()
        {
            edit.Clear();
        }

        public RichTextBox RichTextBox => edit;

        //public override Color BackColor { get => edit.BackColor; set { edit.BackColor = base.BackColor = value; } }

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
            if (edit != null) edit.ContextMenuStrip = ContextMenuStrip;
        }

        /// <summary>
        /// Override font change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (DefaultFontSize < 0 && edit != null) edit.Font = this.Font;
        }

        private bool showScrollBar = true;

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly
        {
            get => edit.ReadOnly;
            set => edit.ReadOnly = value;
        }

        private void Edit_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        private void Edit_Protected(object sender, EventArgs e)
        {
            Protected?.Invoke(this, e);
        }

        private void Edit_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            LinkClicked?.Invoke(this, e);
        }

        private void Edit_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            QueryContinueDrag?.Invoke(this, e);
        }

        private void Edit_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            GiveFeedback?.Invoke(this, e);
        }

        private void Edit_DragOver(object sender, DragEventArgs e)
        {
            DragOver?.Invoke(this, e);
        }

        private void Edit_DragLeave(object sender, EventArgs e)
        {
            DragLeave?.Invoke(this, e);
        }

        private void Edit_DragEnter(object sender, DragEventArgs e)
        {
            DragEnter?.Invoke(this, e);
        }

        private void Edit_DragDrop(object sender, DragEventArgs e)
        {
            DragDrop?.Invoke(this, e);
        }

        public new event KeyEventHandler KeyDown;

        public new event KeyEventHandler KeyUp;

        public new event KeyPressEventHandler KeyPress;

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            edit.Focus();
        }

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            ActiveControl = edit;
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

            if (bar != null && bar.Style == UIStyle.Inherited)
            {
                bar.ForeColor = uiColor.PrimaryColor;
                bar.HoverColor = uiColor.ButtonFillHoverColor;
                bar.PressColor = uiColor.ButtonFillPressColor;
                bar.FillColor = fillColor;
                scrollBarColor = uiColor.PrimaryColor;
                scrollBarBackColor = fillColor;
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

        private Color scrollBarColor = Color.FromArgb(80, 160, 255);

        /// <summary>
        /// Scrollbar fill color
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
        /// Scrollbar background color
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
            edit.ForeColor = color;
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            edit.BackColor = color;
            bar.FillColor = color;
        }

        private void EditOnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        private void EditOnKeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        private void EditOnKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        [Category("SunnyUI"), Browsable(true), DefaultValue(""), Description("Text")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]

        public override string Text
        {
            get => edit.Text;
            set => edit.Text = value;
        }

        [Browsable(true)]
        public new event EventHandler TextChanged;

        private void Edit_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(this, e);
            SetScrollInfo();
        }

        private void Bar_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            if (edit != null)
            {
                ScrollBarInfo.SetScrollValue(edit.Handle, bar.Value);
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            base.OnMouseWheel(e);
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

        public void SetScrollInfo()
        {
            int barWidth = Math.Max(ScrollBarInfo.VerticalScrollBarWidth() + 2, ScrollBarWidth);
            if (bar == null) return;
            bar.Width = barWidth + 1;
            bar.Left = Width - barWidth - 3;

            var si = ScrollBarInfo.GetInfo(edit.Handle);
            if (si.ScrollMax > 0)
            {
                bar.Maximum = si.ScrollMax;
                bar.Visible = showScrollBar && (si.ScrollMax > 0 && si.nMax > 0 && si.nPage > 0);
                bar.Visible = showScrollBar && ScrollBarInfo.IsVerticalScrollBarVisible(edit);
                bar.Value = si.nPos;
            }
            else
            {
                bar.Visible = false;
            }
        }

        private void SizeChange()
        {
            int barWidth = Math.Max(ScrollBarInfo.VerticalScrollBarWidth() + 2, ScrollBarWidth);
            bar.Top = 2;
            bar.Width = barWidth + 1;
            bar.Left = Width - barWidth - 3;
            bar.Height = Height - 4;
            bar.BringToFront();
            SetScrollInfo();
        }

        private void InitializeComponent()
        {
            edit = new RichTextBox();
            bar = new UIScrollBar();
            SuspendLayout();
            // 
            // edit
            // 
            edit.BorderStyle = BorderStyle.None;
            edit.Dock = DockStyle.Fill;
            edit.Location = new Point(2, 2);
            edit.Name = "edit";
            edit.Size = new Size(266, 176);
            edit.TabIndex = 0;
            edit.Text = "";
            // 
            // bar
            // 
            bar.Font = new Font("Segoe UI", 12F);
            bar.Location = new Point(247, 4);
            bar.MinimumSize = new Size(1, 1);
            bar.Name = "bar";
            bar.Size = new Size(19, 173);
            bar.TabIndex = 2;
            bar.Text = "uiScrollBar1";
            // 
            // UIRichTextBox
            // 
            Controls.Add(bar);
            Controls.Add(edit);
            FillColor = Color.White;
            Name = "UIRichTextBox";
            Padding = new Padding(2);
            FontChanged += UIRichTextBox_FontChanged;
            SizeChanged += UIRichTextBox_SizeChanged;
            ResumeLayout(false);
        }

        private void UIRichTextBox_SizeChanged(object sender, EventArgs e)
        {
            SizeChange();
        }

        private void UIRichTextBox_FontChanged(object sender, EventArgs e)
        {
            edit.Font = Font;
        }

        #region TextBoxBase

        [DefaultValue(false)]
        public bool AcceptsTab
        {
            get => edit.AcceptsTab;
            set => edit.AcceptsTab = value;
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

        #endregion TextBoxBase

        #region RichTextBox

        [Browsable(false)]
        public override bool AllowDrop
        {
            get => edit.AllowDrop;
            set => edit.AllowDrop = value;
        }

        [DefaultValue(true)]
        public bool AutoWordSelection
        {
            get => edit.AllowDrop;
            set => edit.AllowDrop = value;
        }

        [DefaultValue(0), Localizable(true)]
        public int BulletIndent
        {
            get => edit.BulletIndent;
            set => edit.BulletIndent = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanRedo
        {
            get => edit.CanRedo;
        }

        [DefaultValue(true)]
        public bool DetectUrls
        {
            get => edit.DetectUrls;
            set => edit.DetectUrls = value;
        }

        [DefaultValue(false)]
        public bool EnableAutoDragDrop
        {
            get => edit.EnableAutoDragDrop;
            set => edit.EnableAutoDragDrop = value;
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public RichTextBoxLanguageOptions LanguageOption
        {
            get => edit.LanguageOption;
            set => edit.LanguageOption = value;
        }

        [DefaultValue(int.MaxValue)]
        public int MaxLength
        {
            get => edit.MaxLength;
            set => edit.MaxLength = value;
        }

        [DefaultValue(true)]
        public bool Multiline
        {
            get => edit.Multiline;
            set => edit.Multiline = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string RedoActionName
        {
            get => edit.RedoActionName;
        }

        [DefaultValue(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool RichTextShortcutsEnabled
        {
            get => edit.RichTextShortcutsEnabled;
            set => edit.RichTextShortcutsEnabled = value;
        }

        [DefaultValue(0), Localizable(true)]
        public int RightMargin
        {
            get => edit.RightMargin;
            set => edit.RightMargin = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.All)]
        public string Rtf
        {
            get => edit.Rtf;
            set => edit.Rtf = value;
        }

        [DefaultValue(HorizontalAlignment.Left), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HorizontalAlignment SelectionAlignment
        {
            get => edit.SelectionAlignment;
            set => edit.SelectionAlignment = value;
        }

        [DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionBullet
        {
            get => edit.SelectionBullet;
            set => edit.SelectionBullet = value;
        }

        [DefaultValue(0), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionCharOffset
        {
            get => edit.SelectionCharOffset;
            set => edit.SelectionCharOffset = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectionColor
        {
            get => edit.SelectionColor;
            set => edit.SelectionColor = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectionBackColor
        {
            get => edit.SelectionBackColor;
            set => edit.SelectionBackColor = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font SelectionFont
        {
            get => edit.SelectionFont;
            set => edit.SelectionFont = value;
        }

        [DefaultValue(0), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionHangingIndent
        {
            get => edit.SelectionHangingIndent;
            set => edit.SelectionHangingIndent = value;
        }

        [DefaultValue(0), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionIndent
        {
            get => edit.SelectionIndent;
            set => edit.SelectionIndent = value;
        }

        [DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionProtected
        {
            get => edit.SelectionProtected;
            set => edit.SelectionProtected = value;
        }

        [DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedRtf
        {
            get => edit.SelectedRtf;
            set => edit.SelectedRtf = value;
        }

        [DefaultValue(0), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionRightIndent
        {
            get => edit.SelectionRightIndent;
            set => edit.SelectionRightIndent = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] SelectionTabs
        {
            get => edit.SelectionTabs;
            set => edit.SelectionTabs = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBoxSelectionTypes SelectionType
        {
            get => edit.SelectionType;
        }

        [DefaultValue(false)]
        public bool ShowSelectionMargin
        {
            get => edit.ShowSelectionMargin;
            set => edit.ShowSelectionMargin = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UndoActionName
        {
            get => edit.UndoActionName;
        }

        [DefaultValue(1.0f), Localizable(true)]
        public float ZoomFactor
        {
            get => edit.ZoomFactor;
            set => edit.ZoomFactor = value;
        }

        [DefaultValue(true)]
        public bool WordWrap
        {
            get => edit.WordWrap;
            set => edit.WordWrap = value;
        }

        public bool CanPaste(DataFormats.Format clipFormat)
        {
            return edit.CanPaste(clipFormat);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
            edit.DrawToBitmap(bitmap, targetBounds);
        }

        public int Find(string str)
        {
            return edit.Find(str);
        }

        public int Find(string str, RichTextBoxFinds options)
        {
            return edit.Find(str, options);
        }

        public int Find(string str, int start, RichTextBoxFinds options)
        {
            return edit.Find(str, start, options);
        }

        public int Find(string str, int start, int end, RichTextBoxFinds options)
        {
            return edit.Find(str, start, end, options);
        }

        public int Find(char[] characterSet)
        {
            return edit.Find(characterSet);
        }

        public int Find(char[] characterSet, int start)
        {
            return edit.Find(characterSet, start);
        }

        public int Find(char[] characterSet, int start, int end)
        {
            return edit.Find(characterSet, start, end);
        }

        public void LoadFile(string path)
        {
            edit.LoadFile(path);
        }

        public void LoadFile(string path, RichTextBoxStreamType fileType)
        {
            edit.LoadFile(path, fileType);
        }

        public void LoadFile(Stream data, RichTextBoxStreamType fileType)
        {
            edit.LoadFile(data, fileType);
        }

        public void Paste(DataFormats.Format clipFormat)
        {
            edit.Paste(clipFormat);
        }

        public void Redo()
        {
            edit.Redo();
        }

        public void SaveFile(string path)
        {
            edit.SaveFile(path);
        }

        public void SaveFile(string path, RichTextBoxStreamType fileType)
        {
            edit.SaveFile(path, fileType);
        }

        public void SaveFile(Stream data, RichTextBoxStreamType fileType)
        {
            edit.SaveFile(data, fileType);
        }

        [Browsable(false)]
        public new event DragEventHandler DragDrop;

        [Browsable(false)]
        public new event DragEventHandler DragEnter;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DragLeave;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event DragEventHandler DragOver;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event GiveFeedbackEventHandler GiveFeedback;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event QueryContinueDragEventHandler QueryContinueDrag;

        public event LinkClickedEventHandler LinkClicked;

        public event EventHandler Protected;

        public event EventHandler SelectionChanged;

        #endregion RichTextBox

        #region TextBoxBase
        public void ScrollToCaret()
        {
            edit.ScrollToCaret();
        }

        public void Select(int start, int length)
        {
            edit.Select(start, length);
        }

        public void SelectAll()
        {
            edit.SelectAll();
        }

        #endregion
    }
}