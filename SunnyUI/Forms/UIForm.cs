/******************************************************************************
 * SunnyUI Open-source control library, utility library, extension library, and multi-page development framework.
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
 * File Name: UIForm.cs
 * File Description: Base form class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-05-30: V2.2.5 Updated title movement, double-click to maximize/restore, maximize to the top, and normal dragging after maximization
 * 2020-07-01: V2.2.6 Redesigned title bar buttons similar to QQ
 * 2020-07-05: V2.2.6 Updated control buttons and rounded corners of the title bar to match form changes
 * 2020-09-17: V2.2.7 Rewrote WindowState-related code
 * 2020-09-17: V2.2.7 Added the ShowDragStretch property for resizable forms
 * 2021-02-04: V3.0.1 Added an extension button to the title bar
 * 2021-05-06: V3.0.3 Added support for placing controls on the title bar
 * 2021-08-17: V3.0.6 Added TitleFont property
 * 2021-08-17: V3.0.6 Adapted to taskbars in various screen directions
 * 2021-08-17: V3.0.8 Added IFrame interface
 * 2022-01-03: V3.0.9 Title bar buttons can now be customized with colors
 * 2022-02-09: V3.1.0 Added SetParamToPage for passing values between pages
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-03-28: V3.1.1 Added method to find pages
 * 2022-04-02: V3.1.2 Default AutoScaleMode set to None
 * 2022-04-26: V3.1.8 Hidden some properties
 * 2022-05-06: V3.1.8 Adjustable size when resizable with Padding
 * 2022-06-11: V3.1.9 Disabled semi-transparent mask for popups by default
 * 2022-07-05: V3.2.1 Added PageAdded, PageSelected, and PageRemoved events for multi-page framework
 * 2022-07-14: V3.2.1 Added UnRegisterHotKey for unloading global hotkeys
 * 2022-07-25: V3.2.2 Multi-page framework now calls UIPage’s Final and FormClosed events upon program close
 * 2022-08-25: V3.2.3 Refactored multi-page framework value passing by removing SetParamToPage
 * 2022-08-25: V3.2.3 Refactored value passing: SendParamToPage for framework-to-page communication
 * 2022-08-25: V3.2.3 Refactored value passing: ReceiveParams event for receiving values from pages
 * 2022-09-11: V3.2.3 Fixed WM_HOTKEY message handling in inherited pages
 * 2022-11-30: V3.3.0 Added RemoveAllPages function
 * 2023-01-25: V3.3.1 Close button area expanded when maximized
 * 2023-02-24: V3.3.2 Fixed PageSelected issue with the first UIPage not showing as selected
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-07-24: V3.4.1 Fixed issue with UIPage Final event not triggering during page switch
 * 2023-07-27: V3.4.1 Set TopMost to true for default pop-up messages
 * 2023-10-09: V3.5.0 Added delayed execution event after form display
 * 2023-11-05: V3.5.2 Refactored theme
 * 2023-11-19: V3.5.2 Changed default ShowShadow to enabled and ShowRadius to disabled
 * 2023-12-04: V3.6.1 Fixed BackColor not saving after modifying Style
 * 2023-12-13: V3.6.2 Optimized UIPage Init and Final load logic
 * 2023-02-19: V3.6.3 Fixed overlapping of title text and control buttons
 * 2024-02-22: V3.6.3 Restored Normal display when dragging title beyond a certain range while maximized
 * 2024-04-28: V3.6.5 Added WindowStateChanged event
 * 2024-05-16: V3.6.6 Resizable replaced ShowDragStretch for adjustable window size with border dragging
 * 2024-06-08: V3.6.6 Prevented icon conversion errors
 * 2024-07-20: V3.6.8 Fixed form size issue when restoring from maximized state
 * 2024-07-26: V3.6.8 Fixed mouse click events
 * 2024-07-28: V3.6.8 Prevented form size restoration when clicking the topmost area of the title bar after maximizing
 * 2025-01-09: V3.8.1 Fixed incomplete display of form borders #IBGJBS
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public partial class UIForm : UIBaseForm
    {
        public UIForm()
        {
            base.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;//Set the maximum size
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            FormBorderStyle = FormBorderStyle.None;
            m_aeroEnabled = false;
        }

        /// <summary>
        /// Disable control scaling with the form.
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form.")]
        public bool ZoomScaleDisabled { get; set; }

        private void SetZoomScaleRect()
        {
            if (ZoomScaleRect.Width == 0 && ZoomScaleRect.Height == 0)
            {
                ZoomScaleRect = new Rectangle(ZoomScaleSize.Width, ZoomScaleSize.Height, 0, 0);
            }

            if (ZoomScaleRect.Width == 0 && ZoomScaleRect.Height == 0)
            {
                ZoomScaleRect = new Rectangle(Left, Top, Width, Height);
            }

            ZoomScaleRectChanged?.Invoke(this, ZoomScaleRect);
        }

        public event OnZoomScaleRectChanged ZoomScaleRectChanged;

        [DefaultValue(typeof(Size), "0, 0")]
        [Description("Design the interface size"), Category("SunnyUI")]
        public Size ZoomScaleSize
        {
            get;
            set;
        }

        /// <summary>
        /// The position of the control in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set control scaling
        /// </summary>
        /// <param name="scale">Zoom ratio</param>
        private void SetZoomScale()
        {
            if (ZoomScaleDisabled) return;
            if (!UIStyles.DPIScale || !UIStyles.ZoomScale) return;
            if (ZoomScaleRect.Width == 0 || ZoomScaleRect.Height == 0) return;
            if (Width == 0 || Height == 0) return;
            float scale = Math.Min(Width * 1.0f / ZoomScaleRect.Width, Height * 1.0f / ZoomScaleRect.Height);
            if (scale.EqualsFloat(0)) return;
            foreach (Control control in this.GetAllZoomScaleControls())
            {
                if (control is IZoomScale ctrl)
                {
                    UIZoomScale.SetZoomScale(control, scale);
                }
            }

            ZoomScaleChanged?.Invoke(this, scale);
        }

        public event OnZoomScaleChanged ZoomScaleChanged;

        //FormBorderStyle property is not displayed
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

        protected override void CalcSystemBoxPos()
        {
            ControlBoxLeft = Width;

            if (ControlBox)
            {
                ControlBoxRect = new Rectangle(Width - 6 - 28, titleHeight / 2 - 14, 28, 28);
                ControlBoxLeft = ControlBoxRect.Left - 2;

                if (MaximizeBox)
                {
                    MaximizeBoxRect = new Rectangle(ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    ControlBoxLeft = MaximizeBoxRect.Left - 2;
                }
                else
                {
                    MaximizeBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
                }

                if (MinimizeBox)
                {
                    MinimizeBoxRect = new Rectangle(MaximizeBox ? MaximizeBoxRect.Left - 28 - 2 : ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    ControlBoxLeft = MinimizeBoxRect.Left - 2;
                }
                else
                {
                    MinimizeBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
                }

                if (ExtendBox)
                {
                    if (MinimizeBox)
                    {
                        ExtendBoxRect = new Rectangle(MinimizeBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    }
                    else
                    {
                        ExtendBoxRect = new Rectangle(ControlBoxRect.Left - 28 - 2, ControlBoxRect.Top, 28, 28);
                    }

                    ControlBoxLeft = ExtendBoxRect.Left - 2;
                }

                if (ControlBoxLeft != Width) ControlBoxLeft -= 6;
            }
            else
            {
                ExtendBoxRect = MaximizeBoxRect = MinimizeBoxRect = ControlBoxRect = new Rectangle(Width + 1, Height + 1, 1, 1);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (FormBorderStyle == FormBorderStyle.None && ShowTitle)
            {
                if (InControlBox)
                {
                    InControlBox = false;
                    Close();
                }

                if (InMinBox)
                {
                    InMinBox = false;
                    DoWindowStateChanged(FormWindowState.Minimized);
                    WindowState = FormWindowState.Minimized;
                }

                if (InMaxBox)
                {
                    InMaxBox = false;
                    ShowMaximize();
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

            base.OnMouseClick(e);
        }

        public event EventHandler ExtendBoxClick;

        private void ShowMaximize()
        {
            Screen screen = Screen.FromPoint(MousePosition);
            base.MaximumSize = ShowFullScreen ? screen.Bounds.Size : screen.WorkingArea.Size;
            if (screen.Primary)
                MaximizedBounds = ShowFullScreen ? screen.Bounds : screen.WorkingArea;
            else
                MaximizedBounds = new Rectangle(0, 0, 0, 0);

            if (WindowState == FormWindowState.Normal)
            {
                FormEx.SetFormRoundRectRegion(this, 0);
                DoWindowStateChanged(FormWindowState.Maximized);
                WindowState = FormWindowState.Maximized;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                FormEx.SetFormRoundRectRegion(this, ShowRadius ? 5 : 0);
                DoWindowStateChanged(FormWindowState.Normal);
                WindowState = FormWindowState.Normal;
            }

            Invalidate();
        }

        private bool FormMoveMouseDown;

        /// <summary>
        /// The position of the form when the left mouse button is pressed
        /// </summary>
        private Point FormLocation;

        /// <summary>
        /// The position of the mouse when the left mouse button is pressed
        /// </summary>
        private Point mouseOffset;

        /// <summary>
        /// Overload mouse press event
        /// </summary>
        /// <param name="e">Mouse event parameters</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (InControlBox || InMaxBox || InMinBox || InExtendBox) return;
            if (!ShowTitle) return;
            if (e.Y > Padding.Top) return;

            if (e.Button == MouseButtons.Left && Movable)
            {
                FormMoveMouseDown = true;
                FormLocation = Location;
                mouseOffset = MousePosition;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!MaximizeBox) return;
            if (InControlBox || InMaxBox || InMinBox || InExtendBox) return;
            if (!ShowTitle) return;
            if (e.Y > Padding.Top) return;
            if (e.Y == 0) return;

            ShowMaximize();
        }

        private long stickyBorderTime = 5000000;

        /// <summary>
        /// Set or get the maximum time for staying at the edge of the display (ms), default 500ms
        /// </summary>
        [Description("Set or get the maximum time to stay at the edge of the display (ms)"), Category("SunnyUI")]
        [DefaultValue(500)]
        public long StickyBorderTime
        {
            get => stickyBorderTime / 10000;
            set => stickyBorderTime = value * 10000;
        }

        /// <summary>
        /// Whether the mouse is staying at the top border of the display.
        /// </summary>
        private bool IsStayAtTopBorder;

        /// <summary>
        /// The time when the mouse stays at the top border of the display
        /// </summary>
        private long TopBorderStayTicks;

        /// <summary>
        /// Overload mouse up event
        /// </summary>
        /// <param name="e">Mouse up event</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!IsDisposed && FormMoveMouseDown)
            {
                //int screenIndex = GetMouseInScreen(PointToScreen(e.Location));
                Screen screen = Screen.FromPoint(MousePosition);
                if (MousePosition.Y == screen.WorkingArea.Top && MaximizeBox && WindowState == FormWindowState.Normal)
                {
                    ShowMaximize();
                }

                // Prevent the title bar from exceeding the container when the form is moved up, resulting in the inability to move later.
                if (Top < screen.WorkingArea.Top)
                {
                    Top = screen.WorkingArea.Top;
                }

                // Prevent the title bar from exceeding the container when the form is moved down, resulting in the inability to move later.
                if (Top > screen.WorkingArea.Bottom - TitleHeight)
                {
                    Top = screen.WorkingArea.Bottom - TitleHeight;
                }
            }

            // After the mouse is lifted, sticky is forcibly turned off and the mouse movement area is restored.
            IsStayAtTopBorder = false;
            Cursor.Clip = new Rectangle();
            FormMoveMouseDown = false;
        }

        /// <summary>
        /// Overload mouse move event
        /// </summary>
        /// <param name="e">Mpve move event</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (FormMoveMouseDown && !MousePosition.Equals(mouseOffset))
            {
                if (WindowState == FormWindowState.Maximized && (Math.Abs(MousePosition.X - mouseOffset.X) >= 6 || Math.Abs(MousePosition.Y - mouseOffset.Y) >= 6))
                {
                    int MaximizedWidth = Width;
                    int LocationX = Left;
                    ShowMaximize();
                    // After calculating the proportional scaling, the relative displacement of the mouse and the original position
                    float offsetXRatio = 1 - (float)Width / MaximizedWidth;
                    mouseOffset.X -= (int)((mouseOffset.X - LocationX) * offsetXRatio);
                }

                int offsetX = mouseOffset.X - MousePosition.X;
                int offsetY = mouseOffset.Y - MousePosition.Y;
                Rectangle WorkingArea = Screen.GetWorkingArea(this);

                // If the current mouse stays on the upper edge of the container, an edge wait with a time of MaximumBorderInterval(ms) will be triggered.
                // If the movement ends at this time, the window will be automatically maximized. This function is provided for multiple monitors arranged one above the other.
                //The advantage of setting the judgment here to a specific value is that if the form is quickly moved across the monitor, it is difficult to trigger the stay event.
                if (MousePosition.Y - WorkingArea.Top == 0)
                {
                    if (!IsStayAtTopBorder)
                    {
                        Cursor.Clip = WorkingArea;
                        TopBorderStayTicks = DateTime.Now.Ticks;
                        IsStayAtTopBorder = true;
                    }
                    else if (DateTime.Now.Ticks - TopBorderStayTicks > stickyBorderTime)
                    {
                        Cursor.Clip = new Rectangle();
                    }
                }

                Location = new Point(FormLocation.X - offsetX, FormLocation.Y - offsetY);
            }
            else
            {
                if (FormBorderStyle == FormBorderStyle.None)
                {
                    bool inControlBox = e.Location.InRect(ControlBoxRect);
                    if (WindowState == FormWindowState.Maximized && ControlBox)
                    {
                        if (e.Location.X > ControlBoxRect.Left && e.Location.Y < TitleHeight)
                            inControlBox = true;
                    }

                    bool inMaxBox = e.Location.InRect(MaximizeBoxRect);
                    bool inMinBox = e.Location.InRect(MinimizeBoxRect);
                    bool inExtendBox = e.Location.InRect(ExtendBoxRect);
                    bool isChange = false;

                    if (inControlBox != InControlBox)
                    {
                        InControlBox = inControlBox;
                        isChange = true;
                    }

                    if (inMaxBox != InMaxBox)
                    {
                        InMaxBox = inMaxBox;
                        isChange = true;
                    }

                    if (inMinBox != InMinBox)
                    {
                        InMinBox = inMinBox;
                        isChange = true;
                    }

                    if (inExtendBox != InExtendBox)
                    {
                        InExtendBox = inExtendBox;
                        isChange = true;
                    }

                    if (isChange)
                    {
                        Invalidate();
                    }
                }
                else
                {
                    InExtendBox = InControlBox = InMaxBox = InMinBox = false;
                }
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Overload Paint method
        /// </summary>
        /// <param name="e">Paint parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            if (FormBorderStyle != FormBorderStyle.None)
            {
                return;
            }

            if (ShowTitle)
            {
                e.Graphics.FillRectangle(titleColor, 0, 0, Width, TitleHeight);
                e.Graphics.DrawLine(RectColor, 0, titleHeight, Width, titleHeight);
            }

            if (ShowRect)
            {
                Point[] points;
                bool unShowRadius = !ShowRadius || WindowState == FormWindowState.Maximized ||
                                    (Width == Screen.PrimaryScreen.WorkingArea.Width &&
                                     Height == Screen.PrimaryScreen.WorkingArea.Height);
                if (unShowRadius)
                {
                    points = new[]
                    {
                        new Point(0, 0),
                        new Point(Width - 1, 0),
                        new Point(Width - 1, Height - 1),
                        new Point(0, Height - 1),
                        new Point(0, 0)
                    };
                }
                else
                {
                    points = new[]
                    {
                            new Point(0, 2),
                            new Point(2, 0),
                            new Point(Width - 1 - 2, 0),
                            new Point(Width - 1, 2),
                            new Point(Width - 1, Height - 1 - 2),
                            new Point(Width - 1 - 2, Height - 1),
                            new Point(2, Height - 1),
                            new Point(0, Height - 1 - 2),
                            new Point(0, 2)
                        };
                }

                e.Graphics.DrawLines(rectColor, points);

                if (!unShowRadius)
                {
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(2, 1), new Point(1, 2));
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(2, Height - 1 - 1), new Point(1, Height - 1 - 2));
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(Width - 1 - 2, 1), new Point(Width - 1 - 1, 2));
                    e.Graphics.DrawLine(Color.FromArgb(120, rectColor), new Point(Width - 1 - 2, Height - 1 - 1), new Point(Width - 1 - 1, Height - 1 - 2));
                }
            }

            if (!ShowTitle)
            {
                return;
            }

            int titleLeft = 6;
            if (ShowIcon && Icon != null)
            {
                try
                {
                    if (IconImage != null)
                    {
                        e.Graphics.DrawImage(IconImage, new Rectangle(6, (TitleHeight - IconImageSize) / 2 + 1, IconImageSize, IconImageSize), new Rectangle(0, 0, IconImage.Width, IconImage.Height), GraphicsUnit.Pixel);
                        titleLeft = 6 + IconImageSize + 2;
                    }
                    else
                    {
                        using (Image image = IconToImage(Icon))
                        {
                            e.Graphics.DrawImage(image, 6, (TitleHeight - 24) / 2 + 1, 24, 24);
                        }

                        titleLeft = 6 + 24 + 2;
                    }
                }
                catch
                {
                    Console.WriteLine("Icon conversion error");
                }
            }

            if (TextAlignment == StringAlignment.Center)
            {
                e.Graphics.DrawString(Text, TitleFont, titleForeColor, new Rectangle(0, 0, Width, TitleHeight), ContentAlignment.MiddleCenter);
            }
            else
            {
                e.Graphics.DrawString(Text, TitleFont, titleForeColor, new Rectangle(titleLeft, 0, Width, TitleHeight), ContentAlignment.MiddleLeft);
            }

            if (ControlBoxLeft != Width)
            {
                e.Graphics.FillRectangle(TitleColor, new Rectangle(ControlBoxLeft, 1, Width - ControlBoxLeft - 1, TitleHeight - 2));
            }

            e.Graphics.SetHighQuality();
            if (ControlBox)
            {
                if (InControlBox)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        e.Graphics.FillRectangle(ControlBoxCloseFillHoverColor, new Rectangle(ControlBoxRect.Left, 0, Width - ControlBoxRect.Left, TitleHeight));
                    }
                    else
                    {
                        if (ShowRadius)
                            e.Graphics.FillRoundRectangle(ControlBoxCloseFillHoverColor, ControlBoxRect, 5);
                        else
                            e.Graphics.FillRectangle(ControlBoxCloseFillHoverColor, ControlBoxRect);
                    }
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

            if (MaximizeBox)
            {
                if (InMaxBox)
                {
                    if (ShowRadius)
                        e.Graphics.FillRoundRectangle(ControlBoxFillHoverColor, MaximizeBoxRect, 5);
                    else
                        e.Graphics.FillRectangle(ControlBoxFillHoverColor, MaximizeBoxRect);
                }

                if (WindowState == FormWindowState.Maximized)
                {
                    e.Graphics.DrawRectangle(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 1,
                        7, 7);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 2,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 1,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 2,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 2,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 + 3);

                    e.Graphics.DrawLine(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 + 3,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 + 3,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 + 3);
                }

                if (WindowState == FormWindowState.Normal)
                {
                    e.Graphics.DrawRectangle(controlBoxForeColor,
                        MaximizeBoxRect.Left + MaximizeBoxRect.Width / 2 - 5,
                        MaximizeBoxRect.Top + MaximizeBoxRect.Height / 2 - 4,
                        10, 9);
                }
            }

            if (MinimizeBox)
            {
                if (InMinBox)
                {
                    if (ShowRadius)
                        e.Graphics.FillRoundRectangle(ControlBoxFillHoverColor, MinimizeBoxRect, 5);
                    else
                        e.Graphics.FillRectangle(ControlBoxFillHoverColor, MinimizeBoxRect);
                }

                e.Graphics.DrawLine(controlBoxForeColor,
                    MinimizeBoxRect.Left + MinimizeBoxRect.Width / 2 - 6,
                    MinimizeBoxRect.Top + MinimizeBoxRect.Height / 2,
                    MinimizeBoxRect.Left + MinimizeBoxRect.Width / 2 + 5,
                    MinimizeBoxRect.Top + MinimizeBoxRect.Height / 2);
            }

            if (ExtendBox)
            {
                if (InExtendBox)
                {
                    if (ShowRadius)
                        e.Graphics.FillRoundRectangle(ControlBoxFillHoverColor, ExtendBoxRect, 5);
                    else
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
                    e.Graphics.DrawFontImage(ExtendSymbol, ExtendSymbolSize, controlBoxForeColor, ExtendBoxRect, ExtendSymbolOffset.X, ExtendSymbolOffset.Y);
                }
            }

            e.Graphics.SetDefaultQuality();
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set a customizable theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        /// <summary>
        /// Overload control size change
        /// </summary>
        /// <param name="e">Parameter</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetZoomScale();
            CalcSystemBoxPos();

            if (IsShown)
            {
                SetRadius();
            }
        }

        protected virtual void AfterSetBackColor(Color color)
        {
        }

        protected virtual void AfterSetForeColor(Color color)
        {
        }

        private bool IsShown;

        [Description("Background Color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Control")]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SetRadius();
            IsShown = true;
            SetZoomScaleRect();
        }

        /// <summary>
        /// Whether to display rounded corners
        /// </summary>
        private bool _showRadius = false;

        /// <summary>
        /// Whether to display rounded corners
        /// </summary>
        [Description("Whether to display rounded corners"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool ShowRadius
        {
            get
            {
                return (_showRadius && !_showShadow && !UIStyles.GlobalRectangle);
            }
            set
            {
                _showRadius = value;
                SetRadius();
                Invalidate();
            }
        }

        /// <summary>
        /// Whether to display the shadow of the border
        /// </summary>
        private bool _showShadow = true;

        #region Border shadow

        /// <summary>
        /// Whether to display the shadow of the border
        /// </summary>
        [Description("Whether to display the shadow of the border"), Category("SunnyUI")]
        [DefaultValue(true)]
        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                _showShadow = value;
                Invalidate();
            }
        }

        private bool m_aeroEnabled;

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                Win32.Dwm.DwmIsCompositionEnabled(ref enabled);
                return enabled == 1;
            }

            return false;
        }

        #endregion Border shadow

        /// <summary>
        /// Whether to display the border
        /// </summary>
        private bool _showRect = true;

        /// <summary>
        /// Whether to display the border
        /// </summary>
        [Description("Whether to display the border"), Category("SunnyUI")]
        [DefaultValue(true)]
        public bool ShowRect
        {
            get => _showRect;
            set
            {
                _showRect = value;
                Invalidate();
            }
        }

        private void SetRadius()
        {
            if (DesignMode)
            {
                return;
            }

            if (WindowState == FormWindowState.Maximized || UIStyles.GlobalRectangle)
            {
                FormEx.SetFormRoundRectRegion(this, 0);
            }
            else
            {
                FormEx.SetFormRoundRectRegion(this, ShowRadius ? 5 : 0);
            }

            Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                {
                    cp.ClassStyle |= Win32.User.CS_DROPSHADOW;
                }

                if (FormBorderStyle == FormBorderStyle.None)
                {
                    // When the border style is FormBorderStyle.None
                    // Click the form taskbar icon to minimize it
                    cp.Style = cp.Style | Win32.User.WS_MINIMIZEBOX;
                    return cp;
                }

                return base.CreateParams;
            }
        }

        [Description("Display borders that can be dragged to resize the form"), Category("SunnyUI"), DefaultValue(false)]
        public bool Resizable
        {
            get => showDragStretch;
            set => showDragStretch = value;
        }

        [Browsable(false)]
        [Description("Display borders that can be dragged to resize the form"), Category("SunnyUI"), DefaultValue(false)]
        public bool ShowDragStretch
        {
            get => showDragStretch;
            set
            {
                showDragStretch = value;
                ShowRect = value;
                if (value) ShowRadius = false;
                SetPadding();
            }
        }

        #region Drag to resize form

        public event HotKeyEventHandler HotKeyEventHandler;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.User.WM_ERASEBKGND)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == Win32.User.WM_HOTKEY)
            {
                int hotKeyId = (int)(m.WParam);
                if (hotKeys != null && hotKeys.ContainsKey(hotKeyId))
                {
                    HotKeyEventHandler?.Invoke(this, new HotKeyEventArgs(hotKeys[hotKeyId], DateTime.Now));
                }
            }

            if (m.Msg == Win32.User.WM_ACTIVATE)
            {
                if (WindowState != FormWindowState.Minimized && lastWindowState == FormWindowState.Minimized)
                {
                    DoWindowStateChanged(WindowState, lastWindowState);
                    lastWindowState = WindowState;
                }
            }

            if (m.Msg == Win32.User.WM_ACTIVATEAPP)
            {
                if (WindowState == FormWindowState.Minimized && lastWindowState != FormWindowState.Minimized)
                {
                    DoWindowStateChanged(WindowState, lastWindowState);
                    lastWindowState = FormWindowState.Minimized;
                }
            }

            base.WndProc(ref m);

            if (m.Msg == Win32.User.WM_NCHITTEST && ShowDragStretch && WindowState == FormWindowState.Normal)
            {
                //Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                Point vPoint = new Point(MousePosition.X, MousePosition.Y);//Fixed the issue where the mouse displays left and right arrows when resizing the form after split screen
                vPoint = PointToClient(vPoint);
                int dragSize = 5;
                if (vPoint.X <= dragSize)
                {
                    if (vPoint.Y <= dragSize)
                        m.Result = (IntPtr)Win32.User.HTTOPLEFT;
                    else if (vPoint.Y >= ClientSize.Height - dragSize)
                        m.Result = (IntPtr)Win32.User.HTBOTTOMLEFT;
                    else
                        m.Result = (IntPtr)Win32.User.HTLEFT;
                }
                else if (vPoint.X >= ClientSize.Width - dragSize)
                {
                    if (vPoint.Y <= dragSize)
                        m.Result = (IntPtr)Win32.User.HTTOPRIGHT;
                    else if (vPoint.Y >= ClientSize.Height - dragSize)
                        m.Result = (IntPtr)Win32.User.HTBOTTOMRIGHT;
                    else
                        m.Result = (IntPtr)Win32.User.HTRIGHT;
                }
                else if (vPoint.Y <= dragSize)
                {
                    m.Result = (IntPtr)Win32.User.HTTOP;
                }
                else if (vPoint.Y >= ClientSize.Height - dragSize)
                {
                    m.Result = (IntPtr)Win32.User.HTBOTTOM;
                }
            }

            if (m.Msg == Win32.User.WM_NCPAINT && ShowShadow && m_aeroEnabled)
            {
                var v = 2;
                Win32.Dwm.DwmSetWindowAttribute(Handle, 2, ref v, 4);
                Win32.Dwm.MARGINS margins = new Win32.Dwm.MARGINS()
                {
                    bottomHeight = 0,
                    leftWidth = 0,
                    rightWidth = 0,
                    topHeight = 1
                };

                Win32.Dwm.DwmExtendFrameIntoClientArea(Handle, ref margins);
            }
        }

        #endregion Drag to resize form
    }
}