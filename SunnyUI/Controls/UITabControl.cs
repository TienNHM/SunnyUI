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
 * File Name: UITabControl.cs
 * File Description: Tab control
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-06-27: V2.2.5 Redraw left and right selection buttons
 * 2020-08-12: V2.2.7 Title vertically centered
 * 2021-04-01: V3.0.2 Fixed the bug that the first TabPage could not be closed
 * 2021-06-08: V3.0.4 Added adjustable height for selected tab title highlight color
 * 2021-07-14: V3.0.5 Support for displaying tabs at the bottom
 * 2021-08-14: V3.0.6 Added DisposeTabPageAfterRemove flag to automatically destroy TabPage after removal
 * 2022-01-02: V3.0.9 Added badge
 * 2022-01-13: V3.1.0 Modified page navigation when deleting pages
 * 2022-04-18: V3.1.5 Added mouse hover effect to close button
 * 2022-04-20: V3.1.5 Disabled left and right keys when tabs are not visible
 * 2022-05-11: V3.1.8 Fixed issue where other controls could not use left and right keys after disabling left and right keys
 * 2022-05-17: V3.1.9 Fixed an issue where the home page could not be closed
 * 2022-06-19: V3.2.0 Execute UIPage's FormClosed event when closing pages in multi-page framework
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-11-06: V3.5.2 Refactored theme
 * 2023-12-13: V3.6.2 Optimized UIPage's Init and Final loading logic
 * 2024-11-29: V3.8.0 Fixed error when SelectedIndex=-1
 * 2024-12-12: V3.8.0 Fixed tab text overflow display #IB8571
 * 2024-12-12: V3.8.0 Added unselected tab color #IB7U69
 * 2025-02-07: V3.8.1 Fixed issue where TabPage background color was not set when switching theme colors, #IBKDR7
 * 2025-02-13: V3.8.1 Add the tab dividing line attribute ShowTabDivider, #IBLERL
******************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sunny.UI.Win32;

namespace Sunny.UI
{
    public sealed class UITabControl : TabControl, IStyleInterface, IZoomScale
    {
        private readonly UITabControlHelper Helper;
        private int DrawedIndex = -1;
        private readonly Timer timer;

        public UITabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            UpdateStyles();

            ItemSize = new Size(150, 40);
            DrawMode = TabDrawMode.OwnerDrawFixed;
            base.Font = UIStyles.Font();
            AfterSetFillColor(FillColor);
            Version = UIGlobal.Version;

            Helper = new UITabControlHelper(this);
            Helper.TabPageAndUIPageChanged += Helper_TabPageAndUIPageChanged;
            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += Timer_Tick;

            tabSelectedForeColor = UIStyles.Blue.TabControlTabSelectedColor;
            tabSelectedHighColor = UIStyles.Blue.TabControlTabSelectedColor;
            _fillColor = UIStyles.Blue.TabControlBackColor;
        }

        private void Helper_TabPageAndUIPageChanged(object sender, TabPageAndUIPageArgs e)
        {
            TabPageAndUIPageChanged?.Invoke(this, e);
        }

        public event EventHandler<TabPageAndUIPageArgs> TabPageAndUIPageChanged;

        [Browsable(false), DefaultValue(null)]
        public IFrame Frame { get; set; }

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

        private bool _showTabDivider = true;

        [Description("Show tab separators"), Category("SunnyUI")]
        [DefaultValue(true)]
        public bool ShowTabDivider
        {
            get => _showTabDivider;
            set
            {
                _showTabDivider = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set control scaling ratio
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public void SetZoomScale(float scale)
        {

        }

        private ConcurrentDictionary<TabPage, string> TipsTexts = new ConcurrentDictionary<TabPage, string>();

        public void SetTipsText(TabPage tabPage, string tipsText)
        {
            if (TipsTexts.ContainsKey(tabPage))
                TipsTexts[tabPage] = tipsText;
            else
                TipsTexts.TryAdd(tabPage, tipsText);

            Invalidate();
        }

        private string GetTipsText(TabPage tabPage)
        {
            return TipsTexts.ContainsKey(tabPage) ? TipsTexts[tabPage] : string.Empty;
        }

        private Color tipsColor = Color.Red;

        /// <summary>
        /// Badge background color
        /// </summary>
        [Description("Badge background color"), Category("SunnyUI")]
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
        /// Badge text color
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI"), Description("Badge text color")]
        public Color TipsForeColor
        {
            get => tipsForeColor;
            set
            {
                tipsForeColor = value;
                Invalidate();
            }
        }

        private Font tipsFont = UIStyles.SubFont();

        /// <summary>
        /// Badge text font
        /// </summary>
        [Description("Badge text font"), Category("SunnyUI")]
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

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            timer?.Stop();
            timer?.Dispose();
        }

        private string mainPage = "";

        /// <summary>
        /// Home page name, this page does not display the close button
        /// </summary>
        [DefaultValue(true)]
        [Description("Home page name, this page does not display the close button"), Category("SunnyUI")]
        public string MainPage
        {
            get => mainPage;
            set
            {
                mainPage = value;
                Invalidate();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            DrawedIndex = SelectedIndex;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ForbidCtrlTab)
            {
                switch (keyData)
                {
                    case (Keys.Tab | Keys.Control):
                        //组合键在调试时，不容易捕获；可以先按住Ctrl键（此时在断点处已经捕获），然后按下Tab键，都释放后，再进入断点处单步向下执行；
                        //此时会分两次进入断点，第一次（好像）是处理Ctrl键，第二次处理组合键
                        return true;
                }
            }

            if (Focused && !TabVisible)
            {
                switch (keyData)
                {
                    case Keys.Left:
                        //if (TabVisible)
                        return true;
                    case Keys.Right:
                        //if (TabVisible)
                        return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Disable Ctrl+Tab
        /// </summary>
        [DefaultValue(true)]
        [Description("Disable Ctrl+Tab"), Category("SunnyUI")]
        public bool ForbidCtrlTab { get; set; } = true;

        public bool SelectPage(int pageIndex) => Helper.SelectPage(pageIndex);

        public bool SelectPage(Guid pageGuid) => Helper.SelectPage(pageGuid);

        public bool RemovePage(int pageIndex) => Helper.RemovePage(pageIndex);

        public bool RemovePage(Guid pageGuid) => Helper.RemovePage(pageGuid);

        public void RemoveAllPages(bool keepMainPage = true) => Helper.RemoveAllPages(keepMainPage);

        public UIPage GetPage(int pageIndex) => Helper.GetPage(pageIndex);

        public UIPage GetPage(Guid pageGuid) => Helper.GetPage(pageGuid);

        public void SetTipsText(int pageIndex, string tipsText) => Helper.SetTipsText(pageIndex, tipsText);

        public void SetTipsText(Guid pageGuid, string tipsText) => Helper.SetTipsText(pageGuid, tipsText);

        public void AddPages(params UIPage[] pages)
        {
            foreach (var page in pages) AddPage(page);
        }

        public void AddPage(UIPage page)
        {
            Helper.AddPage(page);
            PageAdded?.Invoke(this, new UIPageEventArgs(page));
        }

        internal event OnUIPageChanged PageAdded;
        internal event OnUIPageChanged PageRemoved;

        public T GetPage<T>() where T : UIPage => Helper.GetPage<T>();

        public List<T> GetPages<T>() where T : UIPage => Helper.GetPages<T>();

        public string Version
        {
            get;
        }

        private Color _fillColor = UIColor.LightBlue;
        private Color tabBackColor = Color.FromArgb(56, 56, 56);

        /// <summary>
        /// Get or set the object string containing data about the control
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString
        {
            get; set;
        }

        /// <summary>
        /// Get or set the ability to customize the theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        private HorizontalAlignment textAlignment = HorizontalAlignment.Center;

        /// <summary>
        /// Text display direction
        /// </summary>
        [DefaultValue(HorizontalAlignment.Center)]
        [Description("Text display direction"), Category("SunnyUI")]
        public HorizontalAlignment TextAlignment
        {
            get => textAlignment;
            set
            {
                textAlignment = value;
                Invalidate();
            }
        }

        private bool tabVisible = true;

        /// <summary>
        /// Whether to display the tab page
        /// </summary>
        [DefaultValue(true)]
        [Description("Whether to display the tab page"), Category("SunnyUI")]
        public bool TabVisible
        {
            get => tabVisible;
            set
            {
                tabVisible = value;
                if (!tabVisible)
                {
                    ItemSize = new Size(0, 1);
                }
                else
                {
                    if (ItemSize == new Size(0, 1))
                    {
                        ItemSize = new Size(150, 40);
                    }
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Fill color when using borders, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color when using borders, no fill if the value is background color, transparent color, or null"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                AfterSetFillColor(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "56, 56, 56")]
        public Color TabBackColor
        {
            get => tabBackColor;
            set
            {
                if (tabBackColor != value)
                {
                    tabBackColor = value;
                    _menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private Color tabSelectedColor = Color.FromArgb(36, 36, 36);

        /// <summary>
        /// Selected Tab page background color
        /// </summary>
        [Description("Selected Tab page background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "36, 36, 36")]
        public Color TabSelectedColor
        {
            get => tabSelectedColor;
            set
            {
                if (tabSelectedColor != value)
                {
                    tabSelectedColor = value;
                    _menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private Color tabUnSelectedColor = Color.FromArgb(56, 56, 56);

        /// <summary>
        /// Unselected Tab page background color
        /// </summary>
        [Description("Unselected Tab page background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "56, 56, 56")]
        public Color TabUnSelectedColor
        {
            get => tabUnSelectedColor;
            set
            {
                if (tabUnSelectedColor != value)
                {
                    tabUnSelectedColor = value;
                    _menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private Color tabSelectedForeColor = UIColor.Blue;

        /// <summary>
        /// Selected Tab page font color
        /// </summary>
        [Description("Selected Tab page font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color TabSelectedForeColor
        {
            get => tabSelectedForeColor;
            set
            {
                if (tabSelectedForeColor != value)
                {
                    tabSelectedForeColor = value;
                    Invalidate();
                }
            }
        }

        private Color tabUnSelectedForeColor = Color.FromArgb(240, 240, 240);

        /// <summary>
        /// Unselected Tab page font color
        /// </summary>
        [Description("Unselected Tab page font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "240, 240, 240")]
        public Color TabUnSelectedForeColor
        {
            get => tabUnSelectedForeColor;
            set
            {
                if (tabUnSelectedForeColor != value)
                {
                    tabUnSelectedForeColor = value;
                    _menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private Color tabSelectedHighColor = UIColor.Blue;

        /// <summary>
        /// Selected Tab page highlight
        /// </summary>
        [Description("Selected Tab page highlight"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color TabSelectedHighColor

        {
            get => tabSelectedHighColor;
            set
            {
                if (tabSelectedHighColor != value)
                {
                    tabSelectedHighColor = value;
                    Invalidate();
                }
            }
        }

        private int tabSelectedHighColorSize = 4;

        /// <summary>
        /// Selected Tab page highlight height
        /// </summary>
        [Description("Selected Tab page highlight height"), Category("SunnyUI")]
        [DefaultValue(4)]
        public int TabSelectedHighColorSize

        {
            get => tabSelectedHighColorSize;
            set
            {
                value = Math.Max(value, 0);
                value = Math.Min(value, 8);
                tabSelectedHighColorSize = value;
                Invalidate();
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

        [Browsable(false)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = base.DisplayRectangle;
                if (tabVisible)
                {
                    return new Rectangle(rect.Left - 4, rect.Top - 4, rect.Width + 8, rect.Height + 8);
                }
                else
                {
                    return new Rectangle(rect.Left - 4, rect.Top - 5, rect.Width + 8, rect.Height + 9);
                }
            }
        }

        private void AfterSetFillColor(Color color)
        {
            foreach (TabPage page in TabPages)
            {
                page.BackColor = color;
            }
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

        public void SetStyleColor(UIBaseStyle uiColor)
        {
            tabSelectedForeColor = uiColor.TabControlTabSelectedColor;
            tabSelectedHighColor = uiColor.TabControlTabSelectedColor;
            _fillColor = uiColor.TabControlBackColor;

            foreach (TabPage page in TabPages)
            {
                page.BackColor = _fillColor;
            }
        }

        private UIMenuStyle _menuStyle = UIMenuStyle.Black;

        [DefaultValue(UIMenuStyle.Black)]
        [Description("Theme style"), Category("SunnyUI")]
        public UIMenuStyle MenuStyle
        {
            get => _menuStyle;
            set
            {
                if (value != UIMenuStyle.Custom)
                {
                    SetMenuStyle(UIStyles.MenuColors[value]);
                }

                _menuStyle = value;
            }
        }

        private void SetMenuStyle(UIMenuColor uiColor)
        {
            tabBackColor = uiColor.BackColor;
            tabSelectedColor = uiColor.SelectedColor;
            tabUnSelectedColor = uiColor.BackColor;
            tabUnSelectedForeColor = uiColor.UnSelectedForeColor;
            Invalidate();
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;
            //Alignment = TabAlignment.Top;
        }

        private bool showCloseButton;

        /// <summary>
        /// Show close button on all Tab page titles
        /// </summary>
        [DefaultValue(false), Description("Show close button on all Tab page titles"), Category("SunnyUI")]
        public bool ShowCloseButton
        {
            get => showCloseButton;
            set
            {
                if (showCloseButton != value)
                {
                    showCloseButton = value;
                    if (showActiveCloseButton) showActiveCloseButton = false;
                    Invalidate();
                }
            }
        }

        private bool showActiveCloseButton;

        /// <summary>
        /// Show close button on the currently active Tab page title
        /// </summary>
        [DefaultValue(false), Description("Show close button on the currently active Tab page title"), Category("SunnyUI")]
        public bool ShowActiveCloseButton
        {
            get => showActiveCloseButton;
            set
            {
                if (showActiveCloseButton != value)
                {
                    showActiveCloseButton = value;
                    if (showCloseButton) showCloseButton = false;
                    Invalidate();
                }
            }
        }

        private ConcurrentDictionary<int, bool> CloseRects = new ConcurrentDictionary<int, bool>();

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw background color
            e.Graphics.Clear(TabBackColor);

            if (!TabVisible)
            {
                return;
            }

            int tabIdx = -1;
            for (int i = 0; i < TabCount; i++)
            {
                if (SelectedTab == TabPages[i])
                {
                    tabIdx = i;
                    break;
                }
            }

            for (int index = 0; index <= TabCount - 1; index++)
            {
                Rectangle TabRect = new Rectangle(GetTabRect(index).Location.X - 2, GetTabRect(index).Location.Y - 2, ItemSize.Width, ItemSize.Height);
                if (Alignment == TabAlignment.Bottom)
                {
                    TabRect = new Rectangle(GetTabRect(index).Location.X - 2, GetTabRect(index).Location.Y + 2, ItemSize.Width, ItemSize.Height);
                }

                Rectangle textRect = new Rectangle(TabRect.Left + 4, TabRect.Top, TabRect.Width - 8, TabRect.Height);
                if (ImageList != null)
                {
                    textRect = new Rectangle(textRect.Left + ImageList.ImageSize.Width, textRect.Top,
                        textRect.Width - ImageList.ImageSize.Width, textRect.Height);
                }

                if (ShowCloseButton || ShowActiveCloseButton)
                {
                    textRect = new Rectangle(textRect.Left, textRect.Top, textRect.Width - 24, textRect.Height);
                }

                Size sf = TextRenderer.MeasureText(TabPages[index].Text, Font);
                // 绘制标题
                e.Graphics.FillRectangle(tabBackColor, TabRect);

                // 绘制背景
                e.Graphics.FillRectangle(index == SelectedIndex ? TabSelectedColor : TabUnSelectedColor, TabRect);
                if (TabSelectedHighColorSize > 0 && index == SelectedIndex)
                    e.Graphics.FillRectangle(TabSelectedHighColor, TabRect.Left, TabRect.Height - TabSelectedHighColorSize, TabRect.Width, TabSelectedHighColorSize);

                //e.Graphics.DrawString(TabPages[index].Text, Font, index == SelectedIndex ? tabSelectedForeColor : TabUnSelectedForeColor,
                //    new Rectangle(TabRect.Left + textLeft, TabRect.Top, TabRect.Width, TabRect.Height), ContentAlignment.MiddleLeft);

                e.Graphics.DrawTruncateString(TabPages[index].Text, Font, index == SelectedIndex ? tabSelectedForeColor : TabUnSelectedForeColor,
                    textRect, textRect.Width, TabPageTextAlignment);

                TabPage tabPage = TabPages[index];
                UIPage uiPage = Helper.GetPage(tabPage);
                bool show1 = tabPage.Text != MainPage;
                bool show2 = uiPage == null || !uiPage.AlwaysOpen;
                bool showButton = show1 && show2;

                if (showButton)
                {
                    if (ShowCloseButton || (ShowActiveCloseButton && index == SelectedIndex))
                    {
                        Color color = TabUnSelectedForeColor;
                        if (CloseRects.ContainsKey(index) && CloseRects[index])
                        {
                            color = tabSelectedForeColor;
                        }

                        e.Graphics.DrawFontImage(77, 28, color, new Rectangle(TabRect.Left + TabRect.Width - 28, 0, 24, TabRect.Height));
                    }
                }

                // 绘制图标
                if (ImageList != null)
                {
                    int imageIndex = TabPages[index].ImageIndex;
                    if (imageIndex >= 0 && imageIndex < ImageList.Images.Count)
                    {
                        e.Graphics.DrawImage(ImageList.Images[imageIndex], TabRect.Left + 4 + 6, TabRect.Y + (TabRect.Height - ImageList.ImageSize.Height) / 2.0f, ImageList.ImageSize.Width, ImageList.ImageSize.Height);
                    }
                }

                string TipsText = GetTipsText(TabPages[index]);
                if (Enabled && TipsText.IsValid())
                {
                    using var TempFont = TipsFont.DPIScaleFont(TipsFont.Size);
                    sf = TextRenderer.MeasureText(TipsText, TempFont);
                    int sfMax = Math.Max(sf.Width, sf.Height);
                    int x = TabRect.Width - 1 - 2 - sfMax;
                    if (showActiveCloseButton || ShowCloseButton) x -= 24;
                    int y = 1 + 1;
                    e.Graphics.FillEllipse(TipsColor, TabRect.Left + x - 1, y, sfMax, sfMax);
                    e.Graphics.DrawString(TipsText, TempFont, TipsForeColor, new Rectangle(TabRect.Left + x, y, sfMax, sfMax), ContentAlignment.MiddleCenter);
                }

                if (index <= TabCount - 2 && ShowTabDivider)
                {
                    if (index != tabIdx)
                        e.Graphics.DrawLine(tabUnSelectedForeColor.Alpha(100), TabRect.Right - 1, TabRect.Center().Y - TabRect.Height / 4.0f, TabRect.Right - 1, TabRect.Center().Y + TabRect.Height / 4.0f);
                }
            }
        }

        private HorizontalAlignment _tabPageTextAlignment = HorizontalAlignment.Left;
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TabPageTextAlignment
        {
            get => _tabPageTextAlignment;
            set
            {
                _tabPageTextAlignment = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ShowActiveCloseButton || ShowCloseButton)
            {
                for (int index = 0; index <= TabCount - 1; index++)
                {
                    Rectangle TabRect = new Rectangle(GetTabRect(index).Location.X - 2, GetTabRect(index).Location.Y - 2, ItemSize.Width, ItemSize.Height);
                    Rectangle closeRect = new Rectangle(TabRect.Right - 28, 0, 28, TabRect.Height);
                    bool inrect = e.Location.InRect(closeRect);
                    if (!CloseRects.ContainsKey(index))
                        CloseRects.TryAdd(index, false);

                    if (inrect != CloseRects[index])
                    {
                        CloseRects[index] = inrect;
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int removeIndex = -1;
            for (int index = 0; index <= TabCount - 1; index++)
            {
                Rectangle TabRect = new Rectangle(GetTabRect(index).Location.X - 2, GetTabRect(index).Location.Y - 2, ItemSize.Width, ItemSize.Height);
                Rectangle rect = new Rectangle(TabRect.Right - 28, TabRect.Top, 24, TabRect.Height);
                if (e.Location.InRect(rect))
                {
                    removeIndex = index;
                    break;
                }
            }

            if (removeIndex < 0 || removeIndex >= TabCount)
            {
                return;
            }

            TabPage tabPage = TabPages[removeIndex];
            UIPage uiPage = Helper.GetPage(tabPage);
            bool show1 = tabPage.Text != MainPage;
            bool show2 = uiPage == null || !uiPage.AlwaysOpen;
            bool showButton = show1 && show2;
            if (showButton)
            {
                if (ShowCloseButton)
                {
                    if (BeforeRemoveTabPage == null || BeforeRemoveTabPage.Invoke(this, removeIndex))
                    {
                        RemoveTabPage(removeIndex);
                    }
                }
                else if (ShowActiveCloseButton && removeIndex == SelectedIndex)
                {
                    if (DrawedIndex == removeIndex)
                    {
                        if (BeforeRemoveTabPage == null || BeforeRemoveTabPage.Invoke(this, removeIndex))
                        {
                            RemoveTabPage(removeIndex);
                        }
                    }
                }
            }
        }

        public delegate bool OnBeforeRemoveTabPage(object sender, int index);

        public delegate void OnAfterRemoveTabPage(object sender, int index);

        public event OnBeforeRemoveTabPage BeforeRemoveTabPage;

        public event OnAfterRemoveTabPage AfterRemoveTabPage;

        internal void RemoveTabPage(int index)
        {
            if (index < 0 || index >= TabCount)
            {
                return;
            }

            TabPage tabPage = TabPages[index];
            UIPage uiPage = Helper.GetPage(tabPage);
            if (uiPage != null)
            {
                PageRemoved?.Invoke(this, new UIPageEventArgs(uiPage));
                Helper.RemovePage(uiPage);
                AfterRemoveTabPage?.Invoke(this, index);
            }
            else
            {
                tabPage.Parent = null;
                tabPage.Dispose();
                tabPage = null;
            }

            if (TabCount > 1 && index > 0)
            {
                SelectedTab = TabPages[index - 1];
            }
        }

        public enum UITabPosition
        {
            Left,
            Right
        }

        /// <summary>
        /// Tab page display position
        /// </summary>
        [DefaultValue(UITabPosition.Left)]
        [Description("Tab page display position"), Category("SunnyUI")]
        public UITabPosition TabPosition
        {
            get => (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
                ? UITabPosition.Right
                : UITabPosition.Left;
            set
            {
                RightToLeftLayout = value == UITabPosition.Right;
                RightToLeft = (value == UITabPosition.Right) ? RightToLeft.Yes : RightToLeft.No;
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            CloseRects.Clear();
            Init();
            if ((ShowActiveCloseButton && !ShowCloseButton) || ShowTabDivider)
            {
                timer.Start();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            timer?.Start();
        }

        public void Init()
        {
            if (SelectedIndex < 0 || SelectedIndex >= TabPages.Count)
            {
                return;
            }

            if (SelectedIndex >= 0)
            {
                List<UIPage> pages = TabPages[SelectedIndex].GetControls<UIPage>();
                foreach (var page in pages)
                {
                    page.ReLoad();
                }
            }

            List<UITabControlMenu> leftTabControls = TabPages[SelectedIndex].GetControls<UITabControlMenu>();
            foreach (var tabControl in leftTabControls)
            {
                tabControl.Init();
            }

            List<UITabControl> topTabControls = TabPages[SelectedIndex].GetControls<UITabControl>();
            foreach (var tabControl in topTabControls)
            {
                tabControl.Init();
            }
        }

        internal IntPtr UpDownButtonHandle => FindUpDownButton();

        private IntPtr FindUpDownButton()
        {
            return User.FindWindowEx(Handle, IntPtr.Zero, UpDownButtonClassName, null).IntPtr();
        }

        public void OnPaintUpDownButton(UpDownButtonPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.ClipRectangle;
            Color upButtonArrowColor = tabUnSelectedForeColor;
            Color downButtonArrowColor = tabUnSelectedForeColor;

            Rectangle upButtonRect = rect;
            upButtonRect.X = 0;
            upButtonRect.Y = 0;
            upButtonRect.Width = rect.Width / 2 - 1;
            upButtonRect.Height -= 1;

            Rectangle downButtonRect = rect;
            downButtonRect.X = upButtonRect.Right + 1;
            downButtonRect.Y = 0;
            downButtonRect.Width = rect.Width / 2 - 1;
            downButtonRect.Height -= 1;
            g.Clear(tabBackColor);

            if (Enabled)
            {
                if (e.MouseOver)
                {
                    if (e.MousePress)
                    {
                        //鼠标按下
                        if (e.MouseInUpButton)
                            upButtonArrowColor = Color.FromArgb(200, TabSelectedHighColor);
                        else
                            downButtonArrowColor = Color.FromArgb(200, TabSelectedHighColor);
                    }
                    else
                    {
                        //鼠标移动
                        if (e.MouseInUpButton)
                            upButtonArrowColor = TabSelectedHighColor;
                        else
                            downButtonArrowColor = TabSelectedHighColor;
                    }
                }
            }
            else
            {
                upButtonArrowColor = SystemColors.ControlDark;
                downButtonArrowColor = SystemColors.ControlDark;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            RenderButton(g, upButtonRect, upButtonArrowColor, ArrowDirection.Left);
            RenderButton(g, downButtonRect, downButtonArrowColor, ArrowDirection.Right);
            UpDownButtonPaintEventHandler handler = Events[EventPaintUpDownButton] as UpDownButtonPaintEventHandler;
            handler?.Invoke(this, e);
        }

        private static void RenderButton(Graphics g, Rectangle rect, Color arrowColor, ArrowDirection direction)
        {
            switch (direction)
            {
                case ArrowDirection.Left:
                    g.DrawFontImage(61700, 24, arrowColor, rect);
                    break;

                case ArrowDirection.Right:
                    g.DrawFontImage(61701, 24, arrowColor, rect, 1);
                    break;
            }
        }

        private static readonly object EventPaintUpDownButton = new object();
        private const string UpDownButtonClassName = "msctls_updown32";
        private UpDownButtonNativeWindow _upDownButtonNativeWindow;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (UpDownButtonHandle != IntPtr.Zero)
            {
                if (_upDownButtonNativeWindow == null)
                {
                    _upDownButtonNativeWindow = new UpDownButtonNativeWindow(this);
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (UpDownButtonHandle != IntPtr.Zero)
            {
                if (_upDownButtonNativeWindow == null)
                {
                    _upDownButtonNativeWindow = new UpDownButtonNativeWindow(this);
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (_upDownButtonNativeWindow != null)
            {
                _upDownButtonNativeWindow.Dispose();
                _upDownButtonNativeWindow = null;
            }
        }

        /// <summary>
        /// Override control added event
        /// </summary>
        /// <param name="e">Event parameters</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (UpDownButtonHandle != IntPtr.Zero)
            {
                if (_upDownButtonNativeWindow == null)
                {
                    _upDownButtonNativeWindow = new UpDownButtonNativeWindow(this);
                }
            }

            if (e.Control is TabPage)
            {
                e.Control.Padding = new Padding(0);
                if (ShowActiveCloseButton && !ShowCloseButton)
                {
                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Override control size changed event
        /// </summary>
        /// <param name="e">Event parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (UpDownButtonHandle != IntPtr.Zero)
            {
                if (_upDownButtonNativeWindow == null)
                {
                    _upDownButtonNativeWindow = new UpDownButtonNativeWindow(this);
                }
            }
        }

        private class UpDownButtonNativeWindow : NativeWindow, IDisposable
        {
            private UITabControl _owner;
            private bool _bPainting;
            private Rectangle clipRect;

            public UpDownButtonNativeWindow(UITabControl owner)
            {
                _owner = owner;
                AssignHandle(owner.UpDownButtonHandle);
            }

            private static bool LeftKeyPressed()
            {
                if (SystemInformation.MouseButtonsSwapped)
                {
                    return (User.GetKeyState(User.VK_RBUTTON) < 0);
                }

                return (User.GetKeyState(User.VK_LBUTTON) < 0);
            }

            private void DrawUpDownButton()
            {
                RECT rect = new RECT();
                bool mousePress = LeftKeyPressed();
                Point cursorPoint = SystemEx.GetCursorPos();
                User.GetWindowRect(Handle, ref rect);
                var mouseOver = User.PtInRect(ref rect, cursorPoint);
                cursorPoint.X -= rect.Left;
                cursorPoint.Y -= rect.Top;
                var mouseInUpButton = cursorPoint.X < clipRect.Width / 2;
                using (Graphics g = Graphics.FromHwnd(Handle))
                {
                    UpDownButtonPaintEventArgs e = new UpDownButtonPaintEventArgs(g, clipRect, mouseOver, mousePress, mouseInUpButton);
                    _owner.OnPaintUpDownButton(e);
                }
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case User.WM_PAINT:
                        if (!_bPainting)
                        {
                            int itemTop = 0;
                            if (_owner.Alignment == TabAlignment.Top)
                            {
                                itemTop = 0;
                            }
                            else if (_owner.Alignment == TabAlignment.Bottom)
                            {
                                itemTop = _owner.Size.Height - _owner.ItemSize.Height;
                            }
                            Point UpDownButtonLocation = new Point(_owner.Size.Width - 52, itemTop);
                            Size UpDownButtonSize = new Size(52, _owner.ItemSize.Height);
                            clipRect = new Rectangle(UpDownButtonLocation, UpDownButtonSize);
                            User.MoveWindow(Handle, UpDownButtonLocation.X, UpDownButtonLocation.Y, clipRect.Width, clipRect.Height);

                            PAINTSTRUCT ps = new PAINTSTRUCT();
                            _bPainting = true;
                            User.BeginPaint(m.HWnd, ref ps);
                            DrawUpDownButton();
                            User.EndPaint(m.HWnd, ref ps);
                            _bPainting = false;
                            m.Result = Win32Helper.TRUE;
                        }
                        else
                        {
                            base.WndProc(ref m);
                        }
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            #region IDisposable 成员

            /// <summary>
            /// 析构函数
            /// </summary>
            public void Dispose()
            {
                _owner = null;
                base.ReleaseHandle();
            }

            #endregion IDisposable 成员
        }

        public delegate void UpDownButtonPaintEventHandler(object sender, UpDownButtonPaintEventArgs e);

        public class UpDownButtonPaintEventArgs : PaintEventArgs
        {
            public UpDownButtonPaintEventArgs(
                Graphics graphics,
                Rectangle clipRect,
                bool mouseOver,
                bool mousePress,
                bool mouseInUpButton)
                : base(graphics, clipRect)
            {
                MouseOver = mouseOver;
                MousePress = mousePress;
                MouseInUpButton = mouseInUpButton;
            }

            public bool MouseOver
            {
                get;
            }

            public bool MousePress
            {
                get;
            }

            public bool MouseInUpButton
            {
                get;
            }
        }
    }
}