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
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * File Name: UIPage.cs
 * File Description: Base class for pages, inherited from Form, can be placed in containers
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2021-05-21: V3.0.4 Changed the repeated execution of the Init event during page switching
 * 2021-06-20: V3.0.4 Added title row, replacing UITitlePage
 * 2021-07-18: V3.0.5 Fixed the issue of OnLoad being loaded twice during loading, added Final function, which is executed every time the page is switched or exited
 * 2021-08-17: V3.0.6 Added TitleFont property
 * 2021-08-24: V3.0.6 Fixed the issue of OnLoad being loaded twice during loading
 * 2021-12-01: V3.0.9 Added FeedBack and SetParam functions for multi-page value passing
 * 2021-12-30: V3.0.9 Added NeedReload, whether the page needs to reload Load when switching
 * 2022-04-02: V3.1.2 Default setting AutoScaleMode to None
 * 2022-04-26: V3.1.8 Shielded some properties
 * 2022-05-11: V3.1.8 Adjustable Padding when ShowTitle
 * 2022-06-11: V3.1.9 Default close semi-transparent mask for pop-up windows
 * 2022-08-25: V3.2.3 Refactored multi-page framework value passing, deleted SetParam, FeedbackToFrame
 * 2022-08-25: V3.2.3 Refactored multi-page framework value passing: page sends to framework SendParamToFrame function
 * 2022-08-25: V3.2.3 Refactored multi-page framework value passing: page sends to page SendParamToPage function
 * 2022-08-25: V3.2.3 Refactored multi-page framework value passing: receive framework, page value passing ReceiveParams event
 * 2022-10-28: V3.2.6 Added extension button to the title bar
 * 2023-02-24: V3.3.2 Added PageDeselecting, added judgment when canceling page selection
 * 2023-02-24: V3.3.2 Cancelled Dock.Fill at design time, changed to set at runtime
 * 2023-03-15: V3.3.3 Reorganized the page loading order
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-07-27: V3.4.1 Default prompt pop-up window TopMost is true
 * 2023-10-09: V3.5.0 Added an event that is executed after the form is displayed with a delay
 * 2023-10-26: V3.5.1 Added rotation angle parameter SymbolRotate for font icons
 * 2023-11-06: V3.5.2 Refactored theme
 * 2023-12-04: V3.6.1 Fixed the issue that BackColor was not saved after modifying Style
 * 2023-12-20: V3.6.2 Adjusted the position and logic of the AfterShow event
 * 2024-04-28: V3.6.5 Added WindowStateChanged event
 * 2024-10-30: V3.7.2 Added title bar image property IconImage, which takes precedence over Symbol
******************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("Initialize")]
    public partial class UIPage : Form, IStyleInterface, ISymbol, IZoomScale, ITranslate
    {
        public UIPage()
        {
            InitializeComponent();

            TopLevel = false;
            if (this.Register()) SetStyle(UIStyles.Style);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            UpdateStyles();

            //if (!IsDesignMode) base.Dock = DockStyle.Fill;

            Version = UIGlobal.Version;
            SetDPIScale();

            _rectColor = UIStyles.Blue.PageRectColor;
            ForeColor = UIStyles.Blue.PageForeColor;
            titleFillColor = UIStyles.Blue.PageTitleFillColor;
            titleForeColor = UIStyles.Blue.PageTitleForeColor;
            base.WindowState = FormWindowState.Normal;
            base.TopMost = false;
            base.FormBorderStyle = FormBorderStyle.None;
            base.AutoScroll = false;
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.Manual;
            base.SizeGripStyle = SizeGripStyle.Hide;
        }

        private Image iconImage = null;

        [Description("Title bar icon image, still use Icon property for status bar"), Category("SunnyUI")]
        [DefaultValue(null)]
        public Image IconImage
        {
            get => iconImage;
            set
            {
                iconImage = value;
                Invalidate();
            }
        }

        private int iconImageSize = 24;

        [Description("Title bar icon image size"), Category("SunnyUI")]
        [DefaultValue(24)]
        public int IconImageSize
        {
            get => iconImageSize;
            set
            {
                iconImageSize = Math.Max(16, value);
                iconImageSize = Math.Min(titleHeight - 2, iconImageSize);
                Invalidate();
            }
        }

        public readonly Guid Guid = Guid.NewGuid();
        private Color _rectColor = UIColor.Blue;

        private ToolStripStatusLabelBorderSides _rectSides = ToolStripStatusLabelBorderSides.None;

        protected UIStyle _style = UIStyle.Inherited;

        [Browsable(false)]
        public IFrame Frame
        {
            get; set;
        }

        private bool extendBox;

        [DefaultValue(false)]
        [Description("Show extension button"), Category("SunnyUI")]
        public bool ExtendBox
        {
            get => extendBox;
            set
            {
                extendBox = showTitle && value;
                CalcSystemBoxPos();
                Invalidate();
            }
        }

        public event OnWindowStateChanged WindowStateChanged;

        internal void DoWindowStateChanged(FormWindowState thisState, FormWindowState lastState)
        {
            WindowStateChanged?.Invoke(this, thisState, lastState);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.HideComboDropDown();
        }

        public event PageDeselectingEventHandler PageDeselecting;

        internal bool OnPageDeselecting()
        {
            PageDeselectingEventArgs e = new PageDeselectingEventArgs(false, string.Empty);
            PageDeselecting?.Invoke(this, e);
            return e.Cancel;
        }

        [Browsable(false)]
        public new IButtonControl AcceptButton
        {
            get => base.AcceptButton;
            set => base.AcceptButton = value;
        }

        [Browsable(false)]
        public new IButtonControl CancelButton
        {
            get => base.CancelButton;
            set => base.CancelButton = value;
        }

        [Browsable(false)]
        public new SizeGripStyle SizeGripStyle
        {
            get => base.SizeGripStyle;
            set => base.SizeGripStyle = SizeGripStyle.Hide;
        }

        [Browsable(false)]
        public new FormStartPosition StartPosition
        {
            get => base.StartPosition;
            set => base.StartPosition = FormStartPosition.Manual;
        }

        [Browsable(false)]
        public new bool AutoScroll
        {
            get => base.AutoScroll;
            set => base.AutoScroll = false;
        }

        [Browsable(false)]
        public new bool ShowIcon
        {
            get => base.ShowIcon;
            set => base.ShowIcon = false;
        }

        [Browsable(false)]
        public new bool ShowInTaskbar
        {
            get => base.ShowInTaskbar;
            set => base.ShowInTaskbar = false;
        }

        [Browsable(false)]
        public new bool IsMdiContainer
        {
            get => base.IsMdiContainer;
            set => base.IsMdiContainer = false;
        }

        // Do not show FormBorderStyle property
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {
                if (!Enum.IsDefined(typeof(FormBorderStyle), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormBorderStyle));
                base.FormBorderStyle = FormBorderStyle.None;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TopMost
        {
            get => base.TopMost;
            set => base.TopMost = false;
        }

        /// <summary>
        /// Do not show WindowState property
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FormWindowState WindowState
        {
            get
            {
                return base.WindowState;
            }
            set
            {
                base.WindowState = FormWindowState.Normal;
            }
        }

        public UIPage SetPageIndex(int pageIndex)
        {
            PageIndex = pageIndex;
            return this;
        }

        public UIPage SetPageGuid(Guid pageGuid)
        {
            PageGuid = pageGuid;
            return this;
        }

        public UIPage SetText(string text)
        {
            Text = text;
            return this;
        }

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Position of the control in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set the scaling ratio of the control
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public virtual void SetZoomScale(float scale)
        {

        }

        private float DefaultFontSize = -1;
        private float TitleFontSize = -1;

        public void SetDPIScale()
        {
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;

            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            if (TitleFontSize < 0) TitleFontSize = this.TitleFont.Size;

            this.SetDPIScaleFont(DefaultFontSize);
            TitleFont = TitleFont.DPIScaleFont(TitleFontSize);
            foreach (var control in this.GetAllDPIScaleControls())
            {
                control.SetDPIScale();
            }
        }

        public void Render()
        {
            if (DesignMode) return;

            if (UIStyles.Style.IsValid())
                SetInheritedStyle(UIStyles.Style);

            if (UIStyles.MultiLanguageSupport)
                Translate();
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
                _symbolSize = Math.Min(value, 128);
                SymbolChange();
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
                SymbolChange();
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
        /// Rotation angle of the font icon
        /// </summary>
        [DefaultValue(0)]
        [Description("Rotation angle of the font icon"), Category("SunnyUI")]
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

        [DefaultValue(false), Description("Not closed in the Frame framework"), Category("SunnyUI")]
        public bool AlwaysOpen
        {
            get; set;
        }

        protected virtual void SymbolChange()
        {
        }

        [DefaultValue(-1)]
        public int PageIndex { get; set; } = -1;

        [Browsable(false)]
        public Guid PageGuid { get; set; } = Guid.Empty;

        [Browsable(false), DefaultValue(null)]
        public TabPage TabPage { get; set; } = null;

        /// <summary>
        /// Border color
        /// </summary>
        /// <value>The color of the border style.</value>
        [Description("Border color"), Category("SunnyUI")]
        public Color RectColor
        {
            get => _rectColor;
            set
            {
                _rectColor = value;
                AfterSetRectColor(value);
                Invalidate();
            }
        }

        [DefaultValue(ToolStripStatusLabelBorderSides.None)]
        [Description("Border display position"), Category("SunnyUI")]
        public ToolStripStatusLabelBorderSides RectSides
        {
            get => _rectSides;
            set
            {
                _rectSides = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString
        {
            get; set;
        }

        public string Version
        {
            get;
        }

        /// <summary>
        /// Theme style
        /// </summary>
        [DefaultValue(UIStyle.Inherited), Description("Theme style"), Category("SunnyUI")]
        public UIStyle Style
        {
            get => _style;
            set => SetStyle(value);
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }


        public event EventHandler Initialize;

        public event EventHandler Finalize;

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (AllowShowTitle && !AllowAddControlOnTitle && e.Control.Top < TitleHeight)
            {
                e.Control.Top = Padding.Top;
            }
        }

        [DefaultValue(false)]
        [Description("Allow placing controls on the title bar"), Category("SunnyUI")]
        public bool AllowAddControlOnTitle
        {
            get; set;
        }

        public virtual void Init()
        {
            Initialize?.Invoke(this, new EventArgs());
            if (AfterShown != null)
            {
                AfterShownTimer = new System.Windows.Forms.Timer();
                AfterShownTimer.Tick += AfterShownTimer_Tick;
                AfterShownTimer.Start();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Init();
        }

        [Description("Background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Control")]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        private bool IsShown;
        private System.Windows.Forms.Timer AfterShownTimer;
        public event EventHandler AfterShown;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (AutoScaleMode == AutoScaleMode.Font) AutoScaleMode = AutoScaleMode.None;
            if (base.BackColor == SystemColors.Control) base.BackColor = UIStyles.Blue.PageBackColor;

            IsShown = true;
        }

        private void AfterShownTimer_Tick(object sender, EventArgs e)
        {
            AfterShownTimer.Stop();
            AfterShownTimer.Tick -= AfterShownTimer_Tick;
            AfterShownTimer?.Dispose();
            AfterShownTimer = null;

            AfterShown?.Invoke(this, EventArgs.Empty);
        }

        internal void ReLoad()
        {
            if (IsShown)
            {
                if (NeedReload)
                    OnLoad(EventArgs.Empty);
                else
                    Init();
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Whether the page needs to reload Load when switching"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool NeedReload { get; set; }

        public virtual void Final()
        {
            Finalize?.Invoke(this, new EventArgs());
        }

        public virtual void SetInheritedStyle(UIStyle style)
        {
            if (!DesignMode)
            {
                this.SuspendLayout();
                UIStyleHelper.SetChildUIStyle(this, style);

                if (_style == UIStyle.Inherited && style.IsValid())
                {
                    SetStyleColor(style.Colors());
                    Invalidate();
                    _style = UIStyle.Inherited;
                }

                UIStyleChanged?.Invoke(this, new EventArgs());
                this.ResumeLayout();
            }
        }

        protected virtual void SetStyle(UIStyle style)
        {
            this.SuspendLayout();

            if (!style.IsCustom())
            {
                SetStyleColor(style.Colors());
                Invalidate();
            }

            _style = style == UIStyle.Inherited ? UIStyle.Inherited : UIStyle.Custom;
            UIStyleChanged?.Invoke(this, new EventArgs());
            this.ResumeLayout();
        }

        public event EventHandler UIStyleChanged;

        public virtual void SetStyleColor(UIBaseStyle uiColor)
        {
            controlBoxForeColor = uiColor.FormControlBoxForeColor;
            controlBoxFillHoverColor = uiColor.FormControlBoxFillHoverColor;
            ControlBoxCloseFillHoverColor = uiColor.FormControlBoxCloseFillHoverColor;
            BackColor = uiColor.PageBackColor;
            _rectColor = uiColor.PageRectColor;
            ForeColor = uiColor.PageForeColor;
            titleFillColor = uiColor.PageTitleFillColor;
            titleForeColor = uiColor.PageTitleForeColor;
        }

        private Color controlBoxCloseFillHoverColor;
        /// <summary>
        /// Title bar color
        /// </summary>
        [Description("Title bar close button hover background color"), Category("SunnyUI"), DefaultValue(typeof(Color), "Red")]
        public Color ControlBoxCloseFillHoverColor
        {
            get => controlBoxCloseFillHoverColor;
            set
            {
                if (controlBoxCloseFillHoverColor != value)
                {
                    controlBoxCloseFillHoverColor = value;
                    Invalidate();
                }
            }
        }

        protected virtual void AfterSetFillColor(Color color)
        {
        }

        protected virtual void AfterSetRectColor(Color color)
        {
        }

        protected virtual void AfterSetForeColor(Color color)
        {
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 0 || Height <= 0) return;

            if (AllowShowTitle)
            {
                e.Graphics.FillRectangle(TitleFillColor, 0, 0, Width, TitleHeight);
            }

            if (RectSides != ToolStripStatusLabelBorderSides.None)
            {
                if (RectSides.GetValue(ToolStripStatusLabelBorderSides.Left))
                    e.Graphics.DrawLine(RectColor, 0, 0, 0, Height - 1);
                if (RectSides.GetValue(ToolStripStatusLabelBorderSides.Top))
                    e.Graphics.DrawLine(RectColor, 0, 0, Width - 1, 0);
                if (RectSides.GetValue(ToolStripStatusLabelBorderSides.Right))
                    e.Graphics.DrawLine(RectColor, Width - 1, 0, Width - 1, Height - 1);
                if (RectSides.GetValue(ToolStripStatusLabelBorderSides.Bottom))
                    e.Graphics.DrawLine(RectColor, 0, Height - 1, Width - 1, Height - 1);
            }

            if (!AllowShowTitle) return;

            int titleLeft = ImageInterval;
            if (IconImage != null)
            {
                e.Graphics.DrawImage(IconImage, new Rectangle(6, (TitleHeight - IconImageSize) / 2 + 1, IconImageSize, IconImageSize), new Rectangle(0, 0, IconImage.Width, IconImage.Height), GraphicsUnit.Pixel);
                titleLeft = ImageInterval + IconImageSize + 2;
            }
            else if (Symbol > 0)
            {
                e.Graphics.DrawFontImage(Symbol, SymbolSize, TitleForeColor, new Rectangle(ImageInterval, 0, SymbolSize, TitleHeight), SymbolOffset.X, SymbolOffset.Y, SymbolRotate);
                titleLeft = ImageInterval + SymbolSize + 2;
            }

            e.Graphics.DrawString(Text, TitleFont, TitleForeColor, new Rectangle(titleLeft, 0, Width, TitleHeight), ContentAlignment.MiddleLeft);

            e.Graphics.SetHighQuality();
            if (ControlBox)
            {
                if (InControlBox)
                {
                    e.Graphics.FillRectangle(ControlBoxCloseFillHoverColor, ControlBoxRect);
                }

                e.Graphics.DrawLine(controlBoxForeColor,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 - 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 - 5,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 + 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 + 5);
                e.Graphics.DrawLine(controlBoxForeColor,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 - 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 + 5,
                    ControlBoxRect.Left + ControlBoxRect.Width / 2 + 5,
                    ControlBoxRect.Top + ControlBoxRect.Height / 2 - 5);
            }

            if (ExtendBox)
            {
                if (InExtendBox)
                {
                    e.Graphics.FillRectangle(ControlBoxFillHoverColor, ExtendBoxRect);
                }

                if (ExtendSymbol == 0)
                {
                    e.Graphics.DrawLine(controlBoxForeColor,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 - 5 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 - 2,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 + 3);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 + 5 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 - 2,
                        ExtendBoxRect.Left + ExtendBoxRect.Width / 2 - 1,
                        ExtendBoxRect.Top + ExtendBoxRect.Height / 2 + 3);
                }
                else
                {
                    e.Graphics.DrawFontImage(extendSymbol, ExtendSymbolSize, controlBoxForeColor, ExtendBoxRect, ExtendSymbolOffset.X, ExtendSymbolOffset.Y);
                }
            }
        }

        private Color controlBoxForeColor = Color.White;
        /// <summary>
        /// Title bar color
        /// </summary>
        [Description("Title bar button color"), Category("SunnyUI"), DefaultValue(typeof(Color), "White")]
        public Color ControlBoxForeColor
        {
            get => controlBoxForeColor;
            set
            {
                if (controlBoxForeColor != value)
                {
                    controlBoxForeColor = value;
                    Invalidate();
                }
            }
        }

        private Color controlBoxFillHoverColor;
        /// <summary>
        /// Title bar color
        /// </summary>
        [Description("Title bar button hover background color"), Category("SunnyUI"), DefaultValue(typeof(Color), "115, 179, 255")]
        public Color ControlBoxFillHoverColor
        {
            get => controlBoxFillHoverColor;
            set
            {
                if (ControlBoxFillHoverColor != value)
                {
                    controlBoxFillHoverColor = value;
                    Invalidate();
                }
            }
        }

        private Point extendSymbolOffset = new Point(0, 0);

        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Extension button font icon offset"), Category("SunnyUI")]
        public Point ExtendSymbolOffset
        {
            get => extendSymbolOffset;
            set
            {
                extendSymbolOffset = value;
                Invalidate();
            }
        }

        private int _extendSymbolSize = 24;

        [DefaultValue(24)]
        [Description("Extension button font icon size"), Category("SunnyUI")]
        public int ExtendSymbolSize
        {
            get => _extendSymbolSize;
            set
            {
                _extendSymbolSize = Math.Max(value, 16);
                _extendSymbolSize = Math.Min(value, 128);
                Invalidate();
            }
        }

        private int extendSymbol;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(0)]
        [Description("Extension button font icon"), Category("SunnyUI")]
        public int ExtendSymbol
        {
            get => extendSymbol;
            set
            {
                extendSymbol = value;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (FormBorderStyle == FormBorderStyle.None && ShowTitle)
            {
                if (InControlBox)
                {
                    InControlBox = false;
                    Close();
                    AfterClose();
                }

                if (InExtendBox)
                {
                    InExtendBox = false;
                    if (ExtendMenu != null)
                    {
                        this.ShowContextMenuStrip(ExtendMenu, ExtendBoxRect.Left, TitleHeight - 1);
                    }
                    else
                    {
                        ExtendBoxClick?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        [DefaultValue(null)]
        [Description("Extension button menu"), Category("SunnyUI")]
        public UIContextMenuStrip ExtendMenu
        {
            get; set;
        }

        public event EventHandler ExtendBoxClick;

        private void AfterClose()
        {
            Console.WriteLine("Close");
        }

        private Color titleFillColor = Color.FromArgb(76, 76, 76);

        /// <summary>
        /// Fill color, if the value is background color or transparent color or null, it will not be filled
        /// </summary>
        [Description("Title color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "76, 76, 76")]
        public Color TitleFillColor
        {
            get => titleFillColor;
            set
            {
                titleFillColor = value;
                Invalidate();
            }
        }

        private Color titleForeColor = Color.White;

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color TitleForeColor
        {
            get => titleForeColor;
            set
            {
                titleForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Title font
        /// </summary>
        private Font titleFont = UIStyles.Font();

        /// <summary>
        /// Title font
        /// </summary>
        [Description("Title font"), Category("SunnyUI")]
        [DefaultValue(typeof(Font), "Segoe UI, 12pt")]
        public Font TitleFont
        {
            get => titleFont;
            set
            {
                titleFont = value;
                Invalidate();
            }
        }

        private int imageInterval = 6;

        public int ImageInterval
        {
            get => imageInterval;
            set
            {
                imageInterval = Math.Max(2, value);
                Invalidate();
            }
        }

        private int titleHeight = 35;

        [Description("Panel height"), Category("SunnyUI")]
        [DefaultValue(35)]
        public int TitleHeight
        {
            get => titleHeight;
            set
            {
                titleHeight = Math.Max(value, 19);
                Padding = new Padding(Padding.Left, ShowTitle ? Math.Max(titleHeight, Padding.Top) : 0, Padding.Right, Padding.Bottom);
                CalcSystemBoxPos();
                Invalidate();
            }
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CalcSystemBoxPos();
        }

        private bool InControlBox;
        private bool InExtendBox;

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ShowTitle)
            {
                if (ControlBox)
                {
                    bool inControlBox = e.Location.InRect(ControlBoxRect);
                    if (inControlBox != InControlBox)
                    {
                        InControlBox = inControlBox;
                        Invalidate();
                    }
                }

                if (ExtendBox)
                {
                    bool inExtendBox = e.Location.InRect(ExtendBoxRect);
                    if (inExtendBox != InExtendBox)
                    {
                        InExtendBox = inExtendBox;
                        Invalidate();
                    }
                }
            }
            else
            {
                InControlBox = InExtendBox = false;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            InExtendBox = InControlBox = false;
            Invalidate();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);

            if (AllowShowTitle)
            {
                Padding = new Padding(Padding.Left, Math.Max(titleHeight, Padding.Top), Padding.Right, Padding.Bottom);
            }
        }

        [Description("Allow showing title bar"), Category("SunnyUI"), DefaultValue(false)]
        public bool AllowShowTitle
        {
            get => ShowTitle;
            set => ShowTitle = value;
        }

        /// <summary>
        /// Whether to show the title bar of the form
        /// </summary>
        private bool showTitle;

        /// <summary>
        /// Whether to show the title bar of the form
        /// </summary>
        [Description("Whether to show the title bar of the form"), Category("WindowStyle"), DefaultValue(false)]
        public bool ShowTitle
        {
            get => showTitle;
            set
            {
                showTitle = value;
                Padding = new Padding(Padding.Left, value ? Math.Max(titleHeight, Padding.Top) : 0, Padding.Right, Padding.Bottom);
                Invalidate();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        private Rectangle ExtendBoxRect;

        private void CalcSystemBoxPos()
        {
            if (ControlBox)
            {
                ControlBoxRect = new Rectangle(Width - 6 - 28, titleHeight / 2 - 14, 28, 28);
            }
            else
            {
                ControlBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
            }

            if (ExtendBox)
            {
                if (ControlBox)
                {
                    ExtendBoxRect = new Rectangle(ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                }
                else
                {
                    ExtendBoxRect = new Rectangle(Width - 6 - 28, titleHeight / 2 - 14, 28, 28);
                }
            }
            else
            {
                ExtendBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
            }
        }

        private Rectangle ControlBoxRect;

        /// <summary>
        /// Whether to show the control buttons of the form
        /// </summary>
        private bool controlBox;

        /// <summary>
        /// Whether to show the control buttons of the form
        /// </summary>
        [Description("Whether to show the control buttons of the form"), Category("WindowStyle"), DefaultValue(false)]
        public new bool ControlBox
        {
            get => controlBox;
            set
            {
                controlBox = value;
                CalcSystemBoxPos();
                Invalidate();
            }
        }

        [Browsable(false)]
        public new bool MinimizeBox
        {
            get; set;
        }

        [Browsable(false)]
        public new bool MaximizeBox
        {
            get; set;
        }

        internal event OnReceiveParams OnFrameDealPageParams;

        public bool SendParamToFrame(object value)
        {
            var args = new UIPageParamsArgs(this, null, value);
            OnFrameDealPageParams?.Invoke(this, args);
            return args.Handled;
        }

        public bool SendParamToPage(int pageIndex, object value)
        {
            UIPage page = Frame.GetPage(pageIndex);
            if (page == null)
            {
                throw new NullReferenceException("Could not find the page with index: " + pageIndex);
            }

            var args = new UIPageParamsArgs(this, page, value);
            OnFrameDealPageParams?.Invoke(this, args);
            return args.Handled;
        }

        public bool SendParamToPage(Guid pageGuid, object value)
        {
            UIPage page = Frame.GetPage(pageGuid);
            if (page == null)
            {
                throw new NullReferenceException("Could not find the page with index: " + pageGuid);
            }

            var args = new UIPageParamsArgs(this, page, value);
            OnFrameDealPageParams?.Invoke(this, args);
            return args.Handled;
        }

        internal void DealReceiveParams(UIPageParamsArgs e)
        {
            ReceiveParams?.Invoke(this, e);
        }

        protected bool IsDesignMode
        {
            get
            {
                if (DesignMode) return true;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
                if (Process.GetCurrentProcess().ProcessName == "devenv") return true;
                return false;
            }
        }

        public event OnReceiveParams ReceiveParams;

        public virtual void Translate()
        {
            if (IsDesignMode) return;

            var controls = this.GetInterfaceControls<ITranslate>(true);
            foreach (var control in controls)
            {
                control.Translate();
            }

            this.TranslateOther();
        }

        [DefaultValue(true)]
        [Description("Controls need multi-language translation when displayed on the interface"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;
    }
}