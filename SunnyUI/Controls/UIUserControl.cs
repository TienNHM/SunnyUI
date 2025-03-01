/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, Multi-Page Development Framework.
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
 * File Name: UIUserControl.cs
 * File Description: User Control Base Class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2022-04-02: V3.1.1 Added user control base class
 * 2022-04-02: V3.1.2 Default set AutoScaleMode to None
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-07-02: V3.3.9 Added gradient direction selection
 * 2023-11-05: V3.5.2 Refactored theme
 * 2023-11-28: V3.6.0 Fixed color setting issue for controls inside Panel
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#pragma warning disable 1591
namespace Sunny.UI
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(System.ComponentModel.Design.IDesigner))]
    [DefaultEvent("Click"), DefaultProperty("Text")]
    public partial class UIUserControl : UserControl, IStyleInterface, IZoomScale, IFormTranslator
    {
        private int radius = 5;
        protected Color rectColor = UIStyles.Blue.PanelRectColor;
        protected Color fillColor = UIStyles.Blue.PanelFillColor;
        protected Color foreColor = UIStyles.Blue.PanelForeColor;
        protected Color fillColor2 = UIStyles.Blue.PanelFillColor2;
        protected bool InitializeComponentEnd;

        public UIUserControl()
        {
            InitializeComponent();
            Version = UIGlobal.Version;
            AutoScaleMode = AutoScaleMode.None;
            base.Font = UIStyles.Font();
            base.MinimumSize = new System.Drawing.Size(1, 1);
            SetStyleFlags(true, false);
        }

        [Browsable(false)]
        [Description("Array of property names that need multi-language translation when the control is displayed on the interface"), Category("SunnyUI")]
        public virtual string[] FormTranslatorProperties => null;

        [DefaultValue(true)]
        [Description("Need multi-language translation when the control is displayed on the interface"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.HideComboDropDown();
        }

        [Browsable(false)]
        public bool Disabled => !Enabled;

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Control's position in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set control scaling ratio
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public virtual void SetZoomScale(float scale)
        {

        }

        protected float DefaultFontSize = -1;

        public virtual void SetDPIScale()
        {
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        protected bool isReadOnly;

        protected void SetStyleFlags(bool supportTransparent = true, bool selectable = true, bool resizeRedraw = false)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            if (supportTransparent) SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            if (selectable) SetStyle(ControlStyles.Selectable, true);
            if (resizeRedraw) SetStyle(ControlStyles.ResizeRedraw, true);
            base.DoubleBuffered = true;
            UpdateStyles();
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
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

        private string text;

        [Category("SunnyUI")]
        [Description("Display text")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    Invalidate();
                }
            }
        }

        private ToolStripStatusLabelBorderSides _rectSides = ToolStripStatusLabelBorderSides.All;

        [DefaultValue(ToolStripStatusLabelBorderSides.All), Description("Border display position"), Category("SunnyUI")]
        public ToolStripStatusLabelBorderSides RectSides
        {
            get => _rectSides;
            set
            {
                _rectSides = value;
                OnRectSidesChange();
                Invalidate();
            }
        }

        protected virtual void OnRadiusSidesChange()
        {
        }

        protected virtual void OnRectSidesChange()
        {
        }

        private UICornerRadiusSides _radiusSides = UICornerRadiusSides.All;

        [DefaultValue(UICornerRadiusSides.All), Description("Corner radius display position"), Category("SunnyUI")]
        public UICornerRadiusSides RadiusSides
        {
            get => _radiusSides;
            set
            {
                _radiusSides = value;
                OnRadiusSidesChange();
                Invalidate();
            }
        }

        /// <summary>
        /// Whether to display rounded corners
        /// </summary>
        [Description("Whether to display rounded corners"), Category("SunnyUI")]
        protected bool ShowRadius => (int)RadiusSides > 0;

        //圆角角度
        [Description("Corner radius"), Category("SunnyUI")]
        [DefaultValue(5)]
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                if (radius != value)
                {
                    radius = Math.Max(0, value);
                    OnRadiusChanged(radius);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Whether to display the border
        /// </summary>
        [Description("Whether to display the border"), Category("SunnyUI")]
        [DefaultValue(true)]
        protected bool ShowRect => (int)RectSides > 0;

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RectColor
        {
            get
            {
                return rectColor;
            }
            set
            {
                if (rectColor != value)
                {
                    rectColor = value;
                    RectColorChanged?.Invoke(this, null);
                    Invalidate();
                }

                AfterSetRectColor(value);
            }
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color, no fill if the value is background color, transparent color, or null"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
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
                    FillColorChanged?.Invoke(this, null);
                    Invalidate();
                }

                AfterSetFillColor(value);
            }
        }

        private bool fillColorGradient;

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

        /// <summary>
        /// Set fill color
        /// </summary>
        /// <param name="value">Color</param>
        protected virtual void SetFillColor2(Color value)
        {
            if (fillColor2 != value)
            {
                fillColor2 = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor2
        {
            get => fillColor2;
            set => SetFillColor2(value);
        }

        protected virtual void SetFillDisableColor(Color color)
        {
            fillDisableColor = color;
            Invalidate();
        }

        protected virtual void SetRectDisableColor(Color color)
        {
            rectDisableColor = color;
            Invalidate();
        }

        protected virtual void SetForeDisableColor(Color color)
        {
            foreDisableColor = color;
            Invalidate();
        }

        protected bool showText = false;

        private bool showFill = true;

        /// <summary>
        /// Whether to display fill
        /// </summary>
        protected bool ShowFill
        {
            get => showFill;
            set
            {
                if (showFill != value)
                {
                    showFill = value;
                    Invalidate();
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (AutoScaleMode == AutoScaleMode.Font)
            {
                AutoScaleMode = AutoScaleMode.None;
            }
        }

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Visible || Width <= 0 || Height <= 0) return;
            if (IsDisposed) return;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = rect.CreateRoundedRectanglePath(radius, RadiusSides, RectSize);

            // Fill background color
            if (BackgroundImage == null && ShowFill && fillColor.IsValid())
            {
                OnPaintFill(e.Graphics, path);
            }

            // Fill border color
            if (ShowRect)
            {
                OnPaintRect(e.Graphics, path);
            }

            // Fill text
            rect = new Rectangle(1, 1, Width - 3, Height - 3);
            using var path1 = rect.GraphicsPath();
            OnPaintFore(e.Graphics, path1);
            base.OnPaint(e);
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected virtual void OnPaintFore(Graphics g, GraphicsPath path)
        {
            string text = Text;
            if (!showText && Text.IsValid()) text = "";
            Rectangle rect = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom);
            g.DrawString(text, Font, GetForeColor(), rect, TextAlignment);
        }

        /// <summary>
        /// Draw border color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected virtual void OnPaintRect(Graphics g, GraphicsPath path)
        {
            radius = Math.Min(radius, Math.Min(Width, Height));
            if (RectSides == ToolStripStatusLabelBorderSides.None)
            {
                return;
            }

            if (RadiusSides == UICornerRadiusSides.None || Radius == 0)
            {
                // Show left border line when IsRadius is False
                bool ShowRectLeft = RectSides.GetValue(ToolStripStatusLabelBorderSides.Left);
                // Show top border line when IsRadius is False
                bool ShowRectTop = RectSides.GetValue(ToolStripStatusLabelBorderSides.Top);
                // Show right border line when IsRadius is False
                bool ShowRectRight = RectSides.GetValue(ToolStripStatusLabelBorderSides.Right);
                // Show bottom border line when IsRadius is False
                bool ShowRectBottom = RectSides.GetValue(ToolStripStatusLabelBorderSides.Bottom);

                if (ShowRectLeft)
                    g.DrawLine(GetRectColor(), RectSize - 1, 0, RectSize - 1, Height, false, RectSize);
                if (ShowRectTop)
                    g.DrawLine(GetRectColor(), 0, RectSize - 1, Width, RectSize - 1, false, RectSize);
                if (ShowRectRight)
                    g.DrawLine(GetRectColor(), Width - 1, 0, Width - 1, Height, false, RectSize);
                if (ShowRectBottom)
                    g.DrawLine(GetRectColor(), 0, Height - 1, Width, Height - 1, false, RectSize);
            }
            else
            {
                g.DrawPath(GetRectColor(), path, true, RectSize);
                PaintRectDisableSides(g);
            }
        }

        private void PaintRectDisableSides(Graphics g)
        {
            // Show left border line when IsRadius is False
            bool ShowRectLeft = RectSides.GetValue(ToolStripStatusLabelBorderSides.Left);
            // Show top border line when IsRadius is False
            bool ShowRectTop = RectSides.GetValue(ToolStripStatusLabelBorderSides.Top);
            // Show right border line when IsRadius is False
            bool ShowRectRight = RectSides.GetValue(ToolStripStatusLabelBorderSides.Right);
            // Show bottom border line when IsRadius is False
            bool ShowRectBottom = RectSides.GetValue(ToolStripStatusLabelBorderSides.Bottom);

            // Show top-left corner radius when IsRadius is True
            bool RadiusLeftTop = RadiusSides.GetValue(UICornerRadiusSides.LeftTop);
            // Show bottom-left corner radius when IsRadius is True
            bool RadiusLeftBottom = RadiusSides.GetValue(UICornerRadiusSides.LeftBottom);
            // Show top-right corner radius when IsRadius is True
            bool RadiusRightTop = RadiusSides.GetValue(UICornerRadiusSides.RightTop);
            // Show bottom-right corner radius when IsRadius is True
            bool RadiusRightBottom = RadiusSides.GetValue(UICornerRadiusSides.RightBottom);

            var ShowRadius = RadiusSides > 0 && Radius > 0; // At least one corner shows radius
            if (!ShowRadius) return;

            if (!ShowRectLeft && !RadiusLeftBottom && !RadiusLeftTop)
            {
                g.DrawLine(GetFillColor(), RectSize - 1, 0, RectSize - 1, Height, false, RectSize);
            }

            if (!ShowRectTop && !RadiusRightTop && !RadiusLeftTop)
            {
                g.DrawLine(GetFillColor(), 0, RectSize - 1, Width, RectSize - 1, false, RectSize);
            }

            if (!ShowRectRight && !RadiusRightTop && !RadiusRightBottom)
            {
                g.DrawLine(GetFillColor(), Width - 1, 0, Width - 1, Height, false, RectSize);
            }

            if (!ShowRectBottom && !RadiusLeftBottom && !RadiusRightBottom)
            {
                g.DrawLine(GetFillColor(), 0, Height - 1, Width, Height - 1, false, RectSize);
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

        protected virtual void OnPaintFill(Graphics g, GraphicsPath path)
        {
            Color color = GetFillColor();

            if (fillColorGradient)
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

                if (RadiusSides == UICornerRadiusSides.None)
                    g.FillRectangle(br, ClientRectangle);
                else
                    g.FillPath(br, path);

                br.Dispose();
            }
            else
            {
                if (RadiusSides == UICornerRadiusSides.None || Radius == 0)
                    g.Clear(color);
                else
                    g.FillPath(color, path);
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

        protected virtual void AfterSetFillReadOnlyColor(Color color)
        {
        }

        protected virtual void AfterSetRectReadOnlyColor(Color color)
        {
        }

        protected virtual void AfterSetForeReadOnlyColor(Color color)
        {
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }


        protected UIStyle _style = UIStyle.Inherited;

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
        /// Set theme style
        /// </summary>
        /// <param name="style">Theme style</param>
        private void SetStyle(UIStyle style)
        {
            if (!style.IsCustom())
            {
                SetStyleColor(style.Colors());
                Invalidate();
            }

            _style = style == UIStyle.Inherited ? UIStyle.Inherited : UIStyle.Custom;
        }

        public virtual void SetInheritedStyle(UIStyle style)
        {
            SetStyle(style);
            _style = UIStyle.Inherited;
        }

        public virtual void SetStyleColor(UIBaseStyle uiColor)
        {
            fillColor2 = uiColor.PanelFillColor2;
            fillColor = uiColor.PanelFillColor;
            rectColor = uiColor.PanelRectColor;
            foreColor = uiColor.PanelForeColor;

            fillDisableColor = uiColor.FillDisableColor;
            rectDisableColor = uiColor.RectDisableColor;
            foreDisableColor = uiColor.ForeDisableColor;

            fillReadOnlyColor = uiColor.FillDisableColor;
            rectReadOnlyColor = uiColor.RectDisableColor;
            foreReadOnlyColor = uiColor.ForeDisableColor;
        }

        /// <summary>
        /// Set fill read-only color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetFillReadOnlyColor(Color color)
        {
            fillReadOnlyColor = color;
            AfterSetFillReadOnlyColor(color);
            Invalidate();
        }

        /// <summary>
        /// Set border read-only color
        /// </summary>
        /// <param name="color">Color</param>
        protected virtual void SetRectReadOnlyColor(Color color)
        {
            rectReadOnlyColor = color;
            AfterSetRectReadOnlyColor(color);
            Invalidate();
        }

        [DefaultValue(typeof(Color), "244, 244, 244")]
        [Description("Fill color when disabled"), Category("SunnyUI")]
        public Color FillDisableColor
        {
            get => fillDisableColor;
            set => SetFillDisableColor(value);
        }

        [DefaultValue(typeof(Color), "173, 178, 181")]
        [Description("Border color when disabled"), Category("SunnyUI")]
        public Color RectDisableColor
        {
            get => rectDisableColor;
            set => SetRectDisableColor(value);
        }

        protected virtual void OnRadiusChanged(int value)
        {
        }

        protected Color foreDisableColor = UIStyles.Blue.ForeDisableColor;
        protected Color rectDisableColor = UIStyles.Blue.RectDisableColor;
        protected Color fillDisableColor = UIStyles.Blue.FillDisableColor;
        /// <summary>
        /// Font read-only color
        /// </summary>
        protected Color foreReadOnlyColor = UIStyles.Blue.ForeDisableColor;

        /// <summary>
        /// Border read-only color
        /// </summary>
        protected Color rectReadOnlyColor = UIStyles.Blue.RectDisableColor;


        /// <summary>
        /// Fill read-only color
        /// </summary>
        protected Color fillReadOnlyColor = UIStyles.Blue.FillDisableColor;

        protected Color GetRectColor()
        {
            return Enabled ? (isReadOnly ? rectReadOnlyColor : rectColor) : rectDisableColor;
        }

        protected Color GetForeColor()
        {
            return Enabled ? (isReadOnly ? foreReadOnlyColor : foreColor) : foreDisableColor;
        }

        protected Color GetFillColor()
        {
            return Enabled ? (isReadOnly ? fillReadOnlyColor : fillColor) : fillDisableColor;
        }

        /// <summary>
        /// Override original property, get or set a value indicating whether the form is displayed in the Windows taskbar.
        /// </summary>
        /// <value><c>true</c> if [show in taskbar]; otherwise, <c>false</c>.</value>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property is disabled!", true)]
        public new BorderStyle BorderStyle => BorderStyle.None;

        public event EventHandler FillColorChanged;

        public event EventHandler RectColorChanged;

        public string Version
        {
            get;
        }

        private ContentAlignment _textAlignment = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Text alignment direction
        /// </summary>
        [Description("Text alignment direction"), Category("SunnyUI")]
        public ContentAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                TextAlignmentChange?.Invoke(this, value);
                Invalidate();
            }
        }

        public delegate void OnTextAlignmentChange(object sender, ContentAlignment alignment);

        public event OnTextAlignmentChange TextAlignmentChange;

        private int rectSize = 1;

        /// <summary>
        /// Border width
        /// </summary>
        [Description("Border width"), Category("SunnyUI")]
        [DefaultValue(1)]
        public int RectSize
        {
            get => rectSize;
            set
            {
                int v = value;
                if (v > 2) v = 2;
                if (v < 1) v = 1;
                if (rectSize != v)
                {
                    rectSize = v;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public new bool AutoScroll { get; set; } = false;
    }
}
