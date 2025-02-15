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
 * File Name: UITitlePanel.cs
 * File Description: Panel with title
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-07-30: V2.2.6 Added collapsible option
 * 2020-09-03: V3.0.6 Added title text color
 * 2022-05-30: V3.1.9 Fixed Padding setting
 * 2022-10-28: V3.2.6 Arrow icon color can be set
 * 2023-05-02: V3.3.6 Added a property for the close button, hides the control when clicked
 * 2023-05-12: V3.3.6 Changed title text alignment property from TextAlign to TextAlignment
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-07-12: V3.4.0 Removed Padding setting
 * 2023-08-07: V3.4.1 Added OnCollapsed event
 * 2024-12-14: V3.8.0 Added Padding setting #IBB1UF
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("Click"), DefaultProperty("Text")]
    public partial class UITitlePanel : UIPanel
    {
        private int _titleHeight = 35;

        [Description("Panel height"), Category("SunnyUI")]
        [DefaultValue(35)]
        public int TitleHeight
        {
            get => _titleHeight;
            set
            {
                if (_titleHeight != value)
                {
                    _titleHeight = Math.Max(19, value);
                    CalcSystemBoxPos();
                    Padding = new Padding(1, _titleHeight, 1, 1);
                    Invalidate();
                }
            }
        }

        public UITitlePanel()
        {
            InitializeComponent();
            SetStyleFlags(true, false);
            ShowText = false;
            CalcSystemBoxPos();

            titleColor = UIStyles.Blue.PanelTitleColor;
            titleForeColor = UIStyles.Blue.PanelTitleForeColor;

            Padding = new Padding(1, 35, 1, 1);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            symbolColor = uiColor.ButtonForeColor;
            titleColor = uiColor.PanelTitleColor;
            titleForeColor = uiColor.PanelTitleForeColor;
        }

        private HorizontalAlignment textAlign = HorizontalAlignment.Center;

        /// <summary>
        /// Text alignment direction
        /// </summary>
        [DefaultValue(HorizontalAlignment.Center)]
        [Description("Text alignment direction"), Category("SunnyUI"), Browsable(false)]
        public HorizontalAlignment TextAlign
        {
            get => textAlign;
            set
            {
                textAlign = value;
                Invalidate();
            }
        }

        private Color titleForeColor = Color.White;

        [DefaultValue(typeof(Color), "White")]
        [Description("Title text color"), Category("SunnyUI")]
        public Color TitleForeColor
        {
            get => titleForeColor;
            set
            {
                titleForeColor = value;
                Invalidate();
            }
        }

        private Color titleColor = UIColor.Blue;

        [DefaultValue(typeof(Color), "80, 160, 255")]
        [Description("Title color"), Category("SunnyUI")]
        public Color TitleColor
        {
            get => titleColor;
            set
            {
                titleColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            base.OnPaintFill(g, path);
            //IsRadius为True时，显示左上圆角
            bool RadiusLeftTop = RadiusSides.GetValue(UICornerRadiusSides.LeftTop);
            //IsRadius为True时，显示右上圆角
            bool RadiusRightTop = RadiusSides.GetValue(UICornerRadiusSides.RightTop);
            using var path1 = GetTitleFillPath(Radius, TitleHeight, RadiusLeftTop, RadiusRightTop);

            Color color = Enabled ? TitleColor : UIDisableColor.Fill;
            g.FillPath(color, path1);
            if (Height > TitleHeight)
                g.DrawLine(RectColor, 0, TitleHeight, Width, TitleHeight);

            color = Enabled ? TitleForeColor : UIFontColor.Regular;
            g.DrawString(Text, Font, color, new Rectangle(_titleInterval, 0, Width - _titleInterval * 2 - (ShowCollapse || ShowClose ? 24 : 0), TitleHeight), TextAlignment);

            if (ShowCollapse)
            {
                if (InControlBox)
                {
                    if (ShowRadius)
                        g.FillRoundRectangle(UIStyles.ActiveStyleColor.ButtonFillHoverColor, ControlBoxRect, 5);
                    else
                        g.FillRectangle(UIStyles.ActiveStyleColor.ButtonFillHoverColor, ControlBoxRect);
                }

                g.DrawFontImage(Collapsed ? 61703 : 61702, 24, SymbolColor,
                    new Rectangle(ControlBoxRect.Left + 2, ControlBoxRect.Top, ControlBoxRect.Width, ControlBoxRect.Height));
            }

            if (ShowClose)
            {
                if (InControlBox)
                {
                    if (ShowRadius)
                        g.FillRoundRectangle(UIStyles.ActiveStyleColor.ButtonFillHoverColor, ControlBoxRect, 5);
                    else
                        g.FillRectangle(UIStyles.ActiveStyleColor.ButtonFillHoverColor, ControlBoxRect);
                }

                g.DrawFontImage(361453, 24, SymbolColor,
                    new Rectangle(ControlBoxRect.Left + 2, ControlBoxRect.Top, ControlBoxRect.Width, ControlBoxRect.Height), 0, 2);
            }
        }

        private Color symbolColor = Color.White;

        /// <summary>
        /// Font icon color
        /// </summary>
        [Description("Icon color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color SymbolColor
        {
            get => symbolColor;
            set
            {
                if (symbolColor != value)
                {
                    symbolColor = value;
                    Invalidate();
                }
            }
        }

        private bool InControlBox;

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool inControlBox = e.Location.InRect(ControlBoxRect);
            if (inControlBox != InControlBox)
            {
                InControlBox = inControlBox;
                if (ShowCollapse || ShowClose) Invalidate();
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Override mouse leave event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            InControlBox = false;
            Invalidate();
        }

        private int _titleInterval = 10;

        [DefaultValue(10)]
        [Description("Distance from border when title text is left or right aligned"), Category("SunnyUI")]
        public int TitleInterval
        {
            get => _titleInterval;
            set
            {
                _titleInterval = value;
                Invalidate();
            }
        }

        protected GraphicsPath GetTitleFillPath(int radius, int height, bool cornerLeftTop = true, bool cornerRightTop = true)
        {
            Rectangle rect = ClientRectangle;
            GraphicsPath graphicsPath = new GraphicsPath();
            if (radius > 0 && ShowRadius)
            {
                if (cornerLeftTop)
                    graphicsPath.AddArc(0, 0, radius, radius, 180f, 90f);
                else
                    graphicsPath.AddLine(new Point(0, 1), new Point(0, 0));

                if (cornerRightTop)
                    graphicsPath.AddArc(rect.Width - radius - 1, 0, radius, radius, 270f, 90f);
                else
                    graphicsPath.AddLine(new Point(rect.Width - 1 - 1, 0), new Point(rect.Width - 1, 0));

                graphicsPath.AddLine(new Point(rect.Width - 1, radius), new Point(rect.Width - 1, height));
                graphicsPath.AddLine(new Point(radius, height), new Point(0, height));

                graphicsPath.CloseFigure();
            }
            else
            {
                Point[] points = new Point[] { new Point(0, 0), new Point(rect.Width - 1, 0), new Point(rect.Width - 1, height), new Point(0, height), new Point(0, 0), };
                graphicsPath = points.Path();
            }

            return graphicsPath;
        }

        private Rectangle ControlBoxRect;

        private void CalcSystemBoxPos()
        {
            ControlBoxRect = new Rectangle(Width - 6 - 28, TitleHeight / 2 - 14, 28, 28);
        }

        private bool showCollapse;

        [Description("Whether to open the collapse button"), Category("SunnyUI"), DefaultValue(false)]
        public bool ShowCollapse
        {
            get => showCollapse;
            set
            {
                showCollapse = value;
                showClose = false;
                Invalidate();
            }
        }

        private bool showClose;

        [Description("Whether to open the close button"), Category("SunnyUI"), DefaultValue(false)]
        public bool ShowClose
        {
            get => showClose;
            set
            {
                showClose = value;
                showCollapse = false;
                Invalidate();
            }
        }

        private bool collapsed;
        private int rowHeight = 180;
        private bool resizing;


        [Description("Whether to collapse"), Category("SunnyUI"), DefaultValue(false)]
        public bool Collapsed
        {
            get => collapsed;
            set
            {
                if (value)
                {
                    resizing = true;
                    Height = TitleHeight;
                }
                else
                {
                    resizing = false;
                    Height = rowHeight;
                }

                collapsed = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Override mouse click event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (ShowClose && e.Location.InRect(ControlBoxRect))
            {
                this.Hide();
            }

            if (ShowCollapse && e.Location.InRect(ControlBoxRect))
            {
                Collapsed = !Collapsed;
                OnCollapsed?.Invoke(this, e);
            }

            base.OnMouseClick(e);
        }

        public EventHandler OnCollapsed;

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CalcSystemBoxPos();
            if (!resizing)
            {
                rowHeight = Height;
            }
        }

        /// <summary>
        /// Override mouse double click event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (ShowCollapse && e.Location.Y <= TitleHeight)
            {
                Collapsed = !Collapsed;
                OnCollapsed?.Invoke(this, e);
            }

            base.OnMouseDoubleClick(e);
        }

        private void UITitlePanel_VisibleChanged(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control.Top < TitleHeight)
                {
                    control.Top = TitleHeight + 1;
                }
            }
        }
    }
}