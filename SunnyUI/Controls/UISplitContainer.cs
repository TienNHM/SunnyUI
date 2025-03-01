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
 * File Name: UISplitContainer.cs
 * Description: Split Container
 * Current Version: V3.1
 * Creation Date: 2021-10-30
 *
 * 2021-10-30: V3.0.8 Added file description
 * 2022-04-03: V3.1.3 Added theme style
 * 2022-04-20: V3.1.5 Fixed issue where expand/collapse operation failed after calling Collapse()
 * 2022-12-06: V3.3.0 Removed SplitterWidth restriction
 * 2022-12-06: V3.3.0 No arrow drawn when SplitterWidth is small
 * 2023-05-31: V3.6.6 Added read-only property SplitPanelState to get the state.
******************************************************************************/
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public sealed class UISplitContainer : SplitContainer, IStyleInterface, IZoomScale
    {
        private enum UIMouseType
        {
            None,
            Button,
            Split
        }

        public enum UISplitPanelState
        {
            Collapsed = 0,
            Expanded = 1,
        }

        private enum UIControlState
        {
            /// <summary>
            ///  Normal.
            /// </summary>
            Normal,
            /// <summary>
            /// Mouse entered.
            /// </summary>
            Hover,
        }

        public enum UICollapsePanel
        {
            None = 0,
            Panel1 = 1,
            Panel2 = 2,
        }

        private UICollapsePanel _collapsePanel = UICollapsePanel.Panel1;
        private UISplitPanelState _splitPanelState = UISplitPanelState.Expanded;
        private UIControlState _uiControlState;
        private int _lastDistance;
        private int _minSize;
        private UIMouseType _uiMouseType;
        private readonly object EventCollapseClick = new object();

        public UISplitContainer()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            _lastDistance = SplitterDistance;
            base.SplitterWidth = 11;
            MinimumSize = new Size(20, 20);
            Version = UIGlobal.Version;
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
        public void SetZoomScale(float scale)
        {

        }

        private Color barColor = Color.FromArgb(56, 56, 56);

        [DefaultValue(typeof(Color), "56, 56, 56"), Category("SunnyUI")]
        [Description("Background color of the split bar")]
        public Color BarColor
        {
            get => barColor;
            set
            {
                barColor = value;
                Invalidate();
            }
        }

        private Color handleColor = Color.FromArgb(106, 106, 106);

        [DefaultValue(typeof(Color), "106, 106, 106"), Category("SunnyUI")]
        [Description("Background color of the split bar button")]
        public Color HandleColor
        {
            get => handleColor;
            set
            {
                handleColor = value;
                Invalidate();
            }
        }

        private Color handleHoverColor = Color.FromArgb(186, 186, 186);

        [DefaultValue(typeof(Color), "186, 186, 186"), Category("SunnyUI")]
        [Description("Background color of the split bar button when mouse hovers over")]
        public Color HandleHoverColor
        {
            get => handleHoverColor;
            set
            {
                handleHoverColor = value;
                Invalidate();
            }
        }

        private Color arrowColor = Color.FromArgb(80, 160, 255);

        [DefaultValue(typeof(Color), "80, 160, 255"), Category("SunnyUI")]
        [Description("Background color of the split bar button arrow")]
        public Color ArrowColor
        {
            get => arrowColor;
            set
            {
                arrowColor = value;
                Invalidate();
            }
        }

        public event EventHandler CollapseClick
        {
            add => Events.AddHandler(EventCollapseClick, value);
            remove => Events.RemoveHandler(EventCollapseClick, value);
        }

        [DefaultValue(UICollapsePanel.Panel1), Category("SunnyUI")]
        [Description("Panel to collapse when clicked")]
        public UICollapsePanel CollapsePanel
        {
            get => _collapsePanel;
            set
            {
                if (_collapsePanel != value)
                {
                    Expand();
                    _collapsePanel = value;
                    Invalidate();
                }
            }
        }

        private int DefaultCollapseWidth => 80;

        private int DefaultArrowWidth => 24;

        private Rectangle CollapseRect
        {
            get
            {
                if (_collapsePanel == UICollapsePanel.None)
                {
                    return Rectangle.Empty;
                }

                Rectangle rect = SplitterRectangle;
                if (Orientation == Orientation.Horizontal)
                {
                    rect.X = (Width - DefaultCollapseWidth) / 2;
                    rect.Width = DefaultCollapseWidth;
                }
                else
                {
                    rect.Y = (Height - DefaultCollapseWidth) / 2;
                    rect.Height = DefaultCollapseWidth;
                }

                return rect;
            }
        }

        /// <summary>
        /// Collapse and expand state
        /// </summary>
        [Description("Collapse and expand state"), Category("SunnyUI")]
        public UISplitPanelState SplitPanelState
        {
            get => _splitPanelState;
            private set
            {
                if (_splitPanelState != value)
                {
                    switch (value)
                    {
                        case UISplitPanelState.Expanded:
                            Expand();
                            break;
                        case UISplitPanelState.Collapsed:
                            Collapse();
                            break;

                    }

                    _splitPanelState = value;
                }
            }
        }

        private UIControlState ControlState
        {
            set
            {
                if (_uiControlState != value)
                {
                    _uiControlState = value;
                    Invalidate(CollapseRect);
                }
            }
        }

        private UIStyle _style = UIStyle.Inherited;

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
        [Description("Get or set whether custom theme style is allowed"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public string Version
        {
            get;
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

        public void Collapse()
        {
            if (_collapsePanel != UICollapsePanel.None && SplitPanelState == UISplitPanelState.Expanded)
            {
                _lastDistance = SplitterDistance;
                if (_collapsePanel == UICollapsePanel.Panel1)
                {
                    _minSize = Panel1MinSize;
                    Panel1MinSize = 0;
                    SplitterDistance = 0;
                }
                else
                {
                    int width = Orientation == Orientation.Horizontal ?
                        Height : Width;
                    _minSize = Panel2MinSize;
                    Panel2MinSize = 0;
                    SplitterDistance = width - SplitterWidth - Padding.Vertical;
                }

                _splitPanelState = UISplitPanelState.Collapsed;
                Invalidate(SplitterRectangle);
            }
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
            Invalidate(SplitterRectangle);
        }

        public void Expand()
        {
            if (_collapsePanel != UICollapsePanel.None && SplitPanelState == UISplitPanelState.Collapsed)
            {
                if (_collapsePanel == UICollapsePanel.Panel1)
                {
                    Panel1MinSize = _minSize;
                }
                else
                {
                    Panel2MinSize = _minSize;
                }

                SplitterDistance = _lastDistance;
                _splitPanelState = UISplitPanelState.Expanded;
                Invalidate(SplitterRectangle);
            }
        }

        private void OnCollapseClick(EventArgs e)
        {
            SplitPanelState = SplitPanelState == UISplitPanelState.Collapsed ?
                UISplitPanelState.Expanded : UISplitPanelState.Collapsed;
            EventHandler handler = Events[EventCollapseClick] as EventHandler;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Panel1Collapsed || Panel2Collapsed)
            {
                return;
            }

            Rectangle rect = SplitterRectangle;
            bool bHorizontal = Orientation == Orientation.Horizontal;

            e.Graphics.FillRectangle(BarColor, rect);
            if (_collapsePanel == UICollapsePanel.None)
            {
                return;
            }

            //if (SplitterWidth < 11) SplitterWidth = 11;

            Rectangle arrowRect = CalcArrowRect(CollapseRect);
            Color handleRectColor = _uiControlState == UIControlState.Hover ? handleHoverColor : HandleColor;
            Point[] points = GetHandlePoints();
            using Brush br = new SolidBrush(handleRectColor);
            e.Graphics.SetHighQuality();
            e.Graphics.FillPolygon(br, points);
            e.Graphics.SetDefaultQuality();

            if (SplitterWidth >= 9)
            {
                switch (_collapsePanel)
                {
                    case UICollapsePanel.Panel1:
                        if (bHorizontal)
                        {
                            e.Graphics.DrawFontImage(SplitPanelState == UISplitPanelState.Collapsed ? 61703 : 61702,
                                22, arrowColor, arrowRect);
                        }
                        else
                        {
                            e.Graphics.DrawFontImage(SplitPanelState == UISplitPanelState.Collapsed ? 61701 : 61700,
                                22, arrowColor, arrowRect);
                        }
                        break;
                    case UICollapsePanel.Panel2:
                        if (bHorizontal)
                        {
                            e.Graphics.DrawFontImage(SplitPanelState == UISplitPanelState.Collapsed ? 61702 : 61703,
                                22, arrowColor, arrowRect);
                        }
                        else
                        {
                            e.Graphics.DrawFontImage(SplitPanelState == UISplitPanelState.Collapsed ? 61700 : 61701,
                                22, arrowColor, arrowRect);
                        }
                        break;
                }
            }
        }

        private Point[] GetHandlePoints()
        {
            bool bCollapsed = SplitPanelState == UISplitPanelState.Collapsed;

            if (Orientation == Orientation.Horizontal)
            {
                if ((CollapsePanel == UICollapsePanel.Panel1 && !bCollapsed) ||
                    (CollapsePanel == UICollapsePanel.Panel2 && bCollapsed))
                {
                    return new[]
                    {
                        new Point(CollapseRect.Left + 2, CollapseRect.Top),
                        new Point(CollapseRect.Right - 2, CollapseRect.Top),
                        new Point(CollapseRect.Right, CollapseRect.Bottom),
                        new Point(CollapseRect.Left, CollapseRect.Bottom),
                        new Point(CollapseRect.Left + 2, CollapseRect.Top)
                    };
                }

                if ((CollapsePanel == UICollapsePanel.Panel1 && bCollapsed) ||
                    (CollapsePanel == UICollapsePanel.Panel2 && !bCollapsed))
                {
                    return new[]
                    {
                        new Point(CollapseRect.Left, CollapseRect.Top),
                        new Point(CollapseRect.Right, CollapseRect.Top),
                        new Point(CollapseRect.Right - 2, CollapseRect.Bottom),
                        new Point(CollapseRect.Left + 2, CollapseRect.Bottom),
                        new Point(CollapseRect.Left, CollapseRect.Top)
                    };
                }
            }

            if (Orientation == Orientation.Vertical)
            {
                if ((CollapsePanel == UICollapsePanel.Panel1 && !bCollapsed) ||
                    (CollapsePanel == UICollapsePanel.Panel2 && bCollapsed))
                {
                    return new[]
                    {
                        new Point(CollapseRect.Left, CollapseRect.Top + 2),
                        new Point(CollapseRect.Right, CollapseRect.Top),
                        new Point(CollapseRect.Right, CollapseRect.Bottom),
                        new Point(CollapseRect.Left, CollapseRect.Bottom - 2),
                        new Point(CollapseRect.Left, CollapseRect.Top + 2)
                    };
                }

                if ((CollapsePanel == UICollapsePanel.Panel1 && bCollapsed) ||
                    (CollapsePanel == UICollapsePanel.Panel2 && !bCollapsed))
                {
                    return new[]
                    {
                        new Point(CollapseRect.Left,CollapseRect.Top),
                        new Point(CollapseRect.Right,CollapseRect.Top+2),
                        new Point(CollapseRect.Right,CollapseRect.Bottom-2),
                        new Point(CollapseRect.Left,CollapseRect.Bottom),
                        new Point(CollapseRect.Left,CollapseRect.Top)
                    };
                }
            }

            return new Point[0];
        }

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //If the left mouse button is not pressed, reset the mouse state
            if (e.Button != MouseButtons.Left)
            {
                _uiMouseType = UIMouseType.None;
            }

            Rectangle collapseRect = CollapseRect;
            Point mousePoint = e.Location;

            //Mouse is in the Button rectangle and not dragging
            if (collapseRect.Contains(mousePoint) && _uiMouseType != UIMouseType.Split)
            {
                Capture = false;
                SetCursor(Cursors.Hand);
                ControlState = UIControlState.Hover;
                return;
            }

            //Mouse is in the splitter rectangle
            if (SplitterRectangle.Contains(mousePoint))
            {
                ControlState = UIControlState.Normal;

                //If the button is already pressed or already collapsed, dragging is not allowed
                if (_uiMouseType == UIMouseType.Button ||
                    (_collapsePanel != UICollapsePanel.None && SplitPanelState == UISplitPanelState.Collapsed))
                {
                    Capture = false;
                    base.Cursor = Cursors.Default;
                    return;
                }

                //Mouse is not pressed, set Split cursor
                if (_uiMouseType == UIMouseType.None && !IsSplitterFixed)
                {
                    SetCursor(Orientation == Orientation.Horizontal ? Cursors.HSplit : Cursors.VSplit);
                    return;
                }
            }

            ControlState = UIControlState.Normal;

            //Dragging the splitter
            if (_uiMouseType == UIMouseType.Split && !IsSplitterFixed)
            {
                SetCursor(Orientation == Orientation.Horizontal ? Cursors.HSplit : Cursors.VSplit);
                base.OnMouseMove(e);
                return;
            }

            base.Cursor = Cursors.Default;
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Override mouse leave event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.Cursor = Cursors.Default;
            ControlState = UIControlState.Normal;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Rectangle collapseRect = CollapseRect;
            Point mousePoint = e.Location;

            if (collapseRect.Contains(mousePoint) ||
                (_collapsePanel != UICollapsePanel.None &&
                 SplitPanelState == UISplitPanelState.Collapsed))
            {
                _uiMouseType = UIMouseType.Button;
                return;
            }

            if (SplitterRectangle.Contains(mousePoint))
            {
                _uiMouseType = UIMouseType.Split;
            }

            base.OnMouseDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Invalidate(SplitterRectangle);
        }

        /// <summary>
        /// Override mouse up event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Invalidate(SplitterRectangle);

            Rectangle collapseRect = CollapseRect;
            Point mousePoint = e.Location;

            if (_uiMouseType == UIMouseType.Button && e.Button == MouseButtons.Left && collapseRect.Contains(mousePoint))
            {
                OnCollapseClick(EventArgs.Empty);
            }

            _uiMouseType = UIMouseType.None;
        }

        /// <summary>
        /// Set the cursor
        /// </summary>
        /// <param name="cursor">Cursor</param>
        private void SetCursor(Cursor cursor)
        {
            if (base.Cursor != cursor)
            {
                base.Cursor = cursor;
            }
        }

        /// <summary>
        /// Calculate arrow rectangle
        /// </summary>
        /// <param name="collapseRect">Collapse rectangle</param>
        /// <returns>Arrow rectangle</returns>
        private Rectangle CalcArrowRect(Rectangle collapseRect)
        {
            if (Orientation == Orientation.Horizontal)
            {
                int width = (collapseRect.Width - DefaultArrowWidth) / 2;
                return new Rectangle(
                    collapseRect.X + width,
                    collapseRect.Y - 1,
                    DefaultArrowWidth,
                    collapseRect.Height);
            }
            else
            {
                int width = (collapseRect.Height - DefaultArrowWidth) / 2;
                return new Rectangle(
                    collapseRect.X - 1,
                    collapseRect.Y + width,
                    collapseRect.Width,
                    DefaultArrowWidth);
            }
        }

        /// <summary>
        /// Set theme style color
        /// </summary>
        /// <param name="uiColor">UI color</param>
        public void SetStyleColor(UIBaseStyle uiColor)
        {
            arrowColor = uiColor.SplitContainerArrowColor;
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

        /// <summary>
        /// Set inherited style
        /// </summary>
        /// <param name="style">Theme style</param>
        public void SetInheritedStyle(UIStyle style)
        {
            SetStyle(style);
            _style = UIStyle.Inherited;
        }

        /// <summary>
        /// Set DPI scale
        /// </summary>
        public void SetDPIScale()
        {
            //
        }
    }
}
