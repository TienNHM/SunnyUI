/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Class Library, Extension Class Library, Multi-Page Development Framework.
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
 * File Name: UIControl.cs
 * File Description: Base class for controls
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2021-12-13: V3.0.9 Border width can be set to 1 or 2
 * 2022-01-10: V3.1.0 Adjusted border and corner drawing
 * 2022-02-16: V3.1.1 Added read-only color setting to the base class
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-02-03: V3.3.1 Added touch screen press and release events for WIN10 system
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-11-05: V3.5.2 Refactored theme
******************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Base class for controls
    /// </summary>
    [ToolboxItem(false)]
    public class UIControl : Control, IStyleInterface, IZoomScale, IFormTranslator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UIControl()
        {
            Version = UIGlobal.Version;
            base.Font = UIStyles.Font();
            Size = new Size(100, 35);
            base.MinimumSize = new Size(1, 1);
        }

        [Browsable(false)]
        [Description("Array of property names that need multilingual translation when the control is displayed"), Category("SunnyUI")]
        public virtual string[] FormTranslatorProperties => null;

        [DefaultValue(true)]
        [Description("Need multilingual translation when the control is displayed"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;

        [Browsable(false)]
        public bool Disabled => !Enabled;

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Control position in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set control scaling ratio
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public virtual void SetZoomScale(float scale)
        {
            radius = UIZoomScale.Calc(baseRadius, scale);
        }

        protected bool selected;

        private float DefaultFontSize = -1;

        public virtual void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

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

        /// <summary>
        /// Is in design mode
        /// </summary>
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

        private ToolStripStatusLabelBorderSides _rectSides = ToolStripStatusLabelBorderSides.All;

        /// <summary>
        /// Border display position
        /// </summary>
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

        /// <summary>
        /// Corner radius display position
        /// </summary>
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

        private int radius = 5;
        private int baseRadius = 5;

        /// <summary>
        /// Corner radius
        /// </summary>
        [Description("Corner radius"), Category("SunnyUI")]
        [DefaultValue(5)]
        public int Radius
        {
            get => radius;
            set
            {
                if (radius != value)
                {
                    baseRadius = radius = Math.Max(0, value);
                    Invalidate();
                }
            }
        }

        private bool showText = true;

        /// <summary>
        /// Show text
        /// </summary>
        [Description("Show text"), Category("SunnyUI")]
        [DefaultValue(true)]
        protected bool ShowText
        {
            get => showText;
            set
            {
                if (showText != value)
                {
                    showText = value;
                    Invalidate();
                }
            }
        }

        private bool showRect = true;

        /// <summary>
        /// Show border
        /// </summary>
        protected bool ShowRect
        {
            get => showRect;
            set
            {
                if (showRect != value)
                {
                    showRect = value;
                    Invalidate();
                }
            }
        }

        private bool showFill = true;

        /// <summary>
        /// Show fill
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

        /// <summary>
        /// Version
        /// </summary>
        public string Version
        {
            get;
        }

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

        public void SetInheritedStyle(UIStyle style)
        {
            SetStyle(style);
            _style = UIStyle.Inherited;
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        /// <summary>
        /// Set theme style color
        /// </summary>
        /// <param name="uiColor"></param>
        public virtual void SetStyleColor(UIBaseStyle uiColor)
        {
            fillColor = uiColor.ButtonFillColor;
            fillColor2 = uiColor.ButtonFillColor2;
            foreColor = uiColor.ButtonForeColor;
            rectColor = uiColor.ButtonRectColor;

            fillDisableColor = uiColor.FillDisableColor;
            foreDisableColor = uiColor.ForeDisableColor;
            rectDisableColor = uiColor.RectDisableColor;

            fillReadOnlyColor = uiColor.FillDisableColor;
            rectReadOnlyColor = uiColor.RectDisableColor;
            foreReadOnlyColor = uiColor.ForeDisableColor;

            fillHoverColor = fillColor;
            foreHoverColor = foreColor;
            rectHoverColor = rectColor;

            fillPressColor = fillColor;
            forePressColor = foreColor;
            rectPressColor = rectColor;

            fillSelectedColor = fillColor;
            foreSelectedColor = foreColor;
            rectSelectedColor = rectColor;
        }

        /// <summary>
        /// Is mouse hover
        /// </summary>
        [Browsable(false)]
        public bool IsHover;

        /// <summary>
        /// Is mouse press
        /// </summary>
        [Browsable(false)]
        public bool IsPress;

        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Text alignment
        /// </summary>
        [Description("Text alignment"), Category("SunnyUI")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get => textAlign;
            set
            {
                if (textAlign != value)
                {
                    textAlign = value;
                    Invalidate();
                }
            }
        }

        private bool useDoubleClick;

        [Description("Enable double-click event"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool UseDoubleClick
        {
            get
            {
                return useDoubleClick;
            }
            set
            {
                if (useDoubleClick != value)
                {
                    useDoubleClick = value;
                    SetStyle(ControlStyles.StandardDoubleClick, useDoubleClick);
                    //Invalidate();
                }
            }
        }

        protected bool lightStyle;

        /// <summary>
        /// Override paint
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Visible || Width <= 0 || Height <= 0) return;
            if (IsDisposed) return;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = rect.CreateRoundedRectanglePath(radius, RadiusSides, RectSize);

            // Fill background color
            if (ShowFill && fillColor.IsValid())
            {
                OnPaintFill(e.Graphics, path);
            }

            // Fill border color
            if (ShowRect)
            {
                OnPaintRect(e.Graphics, path);
            }

            // Fill text
            if (ShowText)
            {
                OnPaintFore(e.Graphics, path);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// Get border color
        /// </summary>
        /// <returns>Color</returns>
        protected Color GetRectColor()
        {
            // Border
            Color color = rectColor;
            if (IsHover)
                color = rectHoverColor;
            if (IsPress)
                color = rectPressColor;
            if (selected)
                color = rectSelectedColor;
            if (ShowFocusColor && Focused)
                color = rectHoverColor;
            if (isReadOnly)
                color = rectReadOnlyColor;
            return Enabled ? color : rectDisableColor;
        }

        [Description("Show focus state color"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool ShowFocusColor
        {
            get;
            set;
        }

        protected bool isReadOnly;

        /// <summary>
        /// Get font color
        /// </summary>
        /// <returns>Color</returns>
        protected Color GetForeColor()
        {
            // Text
            Color color = lightStyle ? rectColor : foreColor;
            if (IsHover)
                color = foreHoverColor;
            if (IsPress)
                color = forePressColor;
            if (selected)
                color = foreSelectedColor;
            if (ShowFocusColor && Focused)
                color = foreHoverColor;
            if (isReadOnly)
                color = foreReadOnlyColor;
            return Enabled ? color : foreDisableColor;
        }

        /// <summary>
        /// Get fill color
        /// </summary>
        /// <returns>Color</returns>
        protected Color GetFillColor()
        {
            // Fill
            Color color = lightStyle ? plainColor : fillColor;
            if (IsHover)
                color = fillHoverColor;
            if (IsPress)
                color = fillPressColor;
            if (selected)
                color = fillSelectedColor;
            if (ShowFocusColor && Focused)
                color = fillHoverColor;
            if (isReadOnly)
                color = fillReadOnlyColor;
            return Enabled ? color : fillDisableColor;
        }

        /// <summary>
        /// Paint fill
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Path</param>
        protected virtual void OnPaintFill(Graphics g, GraphicsPath path)
        {
            Color color = GetFillColor();
            g.FillPath(color, path);
        }

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

        private void PaintRectDisableSides(Graphics g)
        {
            // When IsRadius is False, show left border line
            bool ShowRectLeft = RectSides.GetValue(ToolStripStatusLabelBorderSides.Left);
            // When IsRadius is False, show top border line
            bool ShowRectTop = RectSides.GetValue(ToolStripStatusLabelBorderSides.Top);
            // When IsRadius is False, show right border line
            bool ShowRectRight = RectSides.GetValue(ToolStripStatusLabelBorderSides.Right);
            // When IsRadius is False, show bottom border line
            bool ShowRectBottom = RectSides.GetValue(ToolStripStatusLabelBorderSides.Bottom);

            // When IsRadius is True, show top-left corner
            bool RadiusLeftTop = RadiusSides.GetValue(UICornerRadiusSides.LeftTop);
            // When IsRadius is True, show bottom-left corner
            bool RadiusLeftBottom = RadiusSides.GetValue(UICornerRadiusSides.LeftBottom);
            // When IsRadius is True, show top-right corner
            bool RadiusRightTop = RadiusSides.GetValue(UICornerRadiusSides.RightTop);
            // When IsRadius is True, show bottom-right corner
            bool RadiusRightBottom = RadiusSides.GetValue(UICornerRadiusSides.RightBottom);

            var ShowRadius = RadiusSides > 0 && Radius > 0; // At least one corner shows rounded corners
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

        /// <summary>
        /// Paint border color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Path</param>
        protected virtual void OnPaintRect(Graphics g, GraphicsPath path)
        {
            radius = Math.Min(radius, Math.Min(Width, Height));
            if (RadiusSides == UICornerRadiusSides.None || Radius == 0)
            {
                // When IsRadius is False, show left border line
                bool ShowRectLeft = RectSides.GetValue(ToolStripStatusLabelBorderSides.Left);
                // When IsRadius is False, show top border line
                bool ShowRectTop = RectSides.GetValue(ToolStripStatusLabelBorderSides.Top);
                // When IsRadius is False, show right border line
                bool ShowRectRight = RectSides.GetValue(ToolStripStatusLabelBorderSides.Right);
                // When IsRadius is False, show bottom border line
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

        /// <summary>
        /// Paint font
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Path</param>
        protected virtual void OnPaintFore(Graphics g, GraphicsPath path)
        {
            Rectangle rect = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom);
            g.DrawString(Text, Font, GetForeColor(), rect, TextAlign);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Fill light color
        /// </summary>
        protected Color plainColor = UIStyles.Blue.PlainColor;

        /// <summary>
        /// Fill color
        /// </summary>
        protected Color fillColor = UIStyles.Blue.ButtonFillColor;

        /// <summary>
        /// Fill mouse hover color
        /// </summary>
        protected Color fillHoverColor = UIStyles.Blue.ButtonFillColor;

        /// <summary>
        /// Fill mouse press color
        /// </summary>
        protected Color fillPressColor = UIStyles.Blue.ButtonFillColor;

        /// <summary>
        /// Selected color
        /// </summary>
        protected Color fillSelectedColor = UIStyles.Blue.ButtonFillColor;

        /// <summary>
        /// Fill disabled color
        /// </summary>
        protected Color fillDisableColor = UIStyles.Blue.FillDisableColor;

        /// <summary>
        /// Fill read-only color
        /// </summary>
        protected Color fillReadOnlyColor = UIStyles.Blue.FillDisableColor;

        /// <summary>
        /// Fill color
        /// </summary>
        protected Color fillColor2 = UIStyles.Blue.ButtonFillColor2;

        protected bool fillColorGradient = false;

        /// <summary>
        /// Border color
        /// </summary>
        protected Color rectColor = UIStyles.Blue.ButtonRectColor;

        /// <summary>
        /// Border mouse hover color
        /// </summary>
        protected Color rectHoverColor = UIStyles.Blue.ButtonRectColor;

        /// <summary>
        /// Border mouse press color
        /// </summary>
        protected Color rectPressColor = UIStyles.Blue.ButtonRectColor;

        /// <summary>
        /// Border selected color
        /// </summary>
        protected Color rectSelectedColor = UIStyles.Blue.ButtonRectColor;

        /// <summary>
        /// Border disabled color
        /// </summary>
        protected Color rectDisableColor = UIStyles.Blue.RectDisableColor;

        /// <summary>
        /// Border read-only color
        /// </summary>
        protected Color rectReadOnlyColor = UIStyles.Blue.RectDisableColor;

        /// <summary>
        /// Font color
        /// </summary>
        protected Color foreColor = UIStyles.Blue.ButtonForeColor;

        /// <summary>
        /// Font mouse hover color
        /// </summary>
        protected Color foreHoverColor = UIStyles.Blue.ButtonForeColor;

        /// <summary>
        /// Font mouse press color
        /// </summary>
        protected Color forePressColor = UIStyles.Blue.ButtonForeColor;

        /// <summary>
        /// Font selected color
        /// </summary>
        protected Color foreSelectedColor = UIStyles.Blue.ButtonForeColor;

        /// <summary>
        /// Font disabled color
        /// </summary>
        protected Color foreDisableColor = UIStyles.Blue.ForeDisableColor;

        /// <summary>
        /// Font read-only color
        /// </summary>
        protected Color foreReadOnlyColor = UIStyles.Blue.ForeDisableColor;

        /// <summary>
        /// Set selected color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetPlainColor(Color color)
        {
            if (plainColor != color)
            {
                plainColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set selected color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetFillSelectedColor(Color color)
        {
            if (fillSelectedColor != color)
            {
                fillSelectedColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set selected color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetForeSelectedColor(Color color)
        {
            if (foreSelectedColor != color)
            {
                foreSelectedColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set selected color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetRectSelectedColor(Color color)
        {
            if (rectSelectedColor != color)
            {
                rectSelectedColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set fill mouse hover color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetFillHoverColor(Color color)
        {
            if (fillHoverColor != color)
            {
                fillHoverColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set fill mouse press color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetFillPressColor(Color color)
        {
            if (fillPressColor != color)
            {
                fillPressColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set fill disabled color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetFillDisableColor(Color color)
        {
            if (fillDisableColor != color)
            {
                fillDisableColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set fill read-only color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetFillReadOnlyColor(Color color)
        {
            if (fillReadOnlyColor != color)
            {
                fillReadOnlyColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set border mouse hover color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetRectHoverColor(Color color)
        {
            if (rectHoverColor != color)
            {
                rectHoverColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set border mouse press color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetRectPressColor(Color color)
        {
            if (rectPressColor != color)
            {
                rectPressColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set border disabled color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetRectDisableColor(Color color)
        {
            if (rectDisableColor != color)
            {
                rectDisableColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set border read-only color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetRectReadOnlyColor(Color color)
        {
            if (rectReadOnlyColor != color)
            {
                rectReadOnlyColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set font mouse hover color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetForeHoverColor(Color color)
        {
            if (foreHoverColor != color)
            {
                foreHoverColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set font mouse press color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetForePressColor(Color color)
        {
            if (forePressColor != color)
            {
                forePressColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set font disabled color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetForeDisableColor(Color color)
        {
            if (foreDisableColor != color)
            {
                foreDisableColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set font read-only color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetForeReadonlyColor(Color color)
        {
            if (foreReadOnlyColor != color)
            {
                foreReadOnlyColor = color;
                Invalidate();
            }
        }

        /// <summary>
        /// Set border color
        /// </summary>
        /// <param name="value">Color</param>
        protected void SetRectColor(Color value)
        {
            if (rectColor != value)
            {
                rectColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set fill color
        /// </summary>
        /// <param name="value">Color</param>
        protected void SetFillColor(Color value)
        {
            if (fillColor != value)
            {
                fillColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set fill color
        /// </summary>
        /// <param name="value">Color</param>
        protected void SetFillColor2(Color value)
        {
            if (fillColor2 != value)
            {
                fillColor2 = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set font color
        /// </summary>
        /// <param name="value">Color</param>
        protected void SetForeColor(Color value)
        {
            if (foreColor != value)
            {
                foreColor = value;
                Invalidate();
            }
        }

        /// <summary>Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }

        /// <summary>Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnLostFocus(System.EventArgs)" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate();
        }

        [Description("Enable to respond to some touch screen click events"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool TouchPressClick { get; set; } = false;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////  WndProc window procedure:
        //////  When a WM_POINTERDOWN message is generated when the screen is pressed, we send a WM_LBUTTONDOWN message through the API function PostMessage
        //////  The WM_LBUTTONDOWN message generates a corresponding mouse left button down event, so we just need to write the processing process in the mouse_down event
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region WndProc window procedure

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        const int WM_POINTERDOWN = 0x0246;
        const int WM_POINTERUP = 0x0247;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        protected override void WndProc(ref Message m)
        {
            if (TouchPressClick)
            {
                switch (m.Msg)
                {
                    case WM_POINTERDOWN:
                        break;
                    case WM_POINTERUP:
                        break;
                    default:
                        base.WndProc(ref m);
                        return;
                }

                switch (m.Msg)
                {
                    case WM_POINTERDOWN:
                        PostMessage(m.HWnd, WM_LBUTTONDOWN, (int)m.WParam, (int)m.LParam);
                        break;
                    case WM_POINTERUP:
                        PostMessage(m.HWnd, WM_LBUTTONUP, (int)m.WParam, (int)m.LParam);
                        break;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        #endregion

    }
}