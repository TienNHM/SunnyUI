/******************************************************************************
* SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
* CopyRight (C) 2012-2023 ShenYongHua(沈永华).
* QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
*
* Blog:   https://www.cnblogs.com/yhuse
* Gitee:  https://gitee.com/yhuse/SunnyUI
* GitHub: https://github.com/yhuse/SunnyUI
*
* SunnyUI.dll can be used for free under the GPL-3.0 license.
* If you use this code, please keep this note.
* If you use this code, please keep this note.
******************************************************************************
* File Name: UINavMenu.cs
* File Description: Navigation Menu
* Current Version: V3.1
* Creation Date: 2020-01-01
*
* 2020-01-01: V2.2.0 Added file description
* 2020-07-01: V2.2.6 Fixed flickering caused by redrawing all nodes when events are triggered; fixed scroll wheel failure issue.
* 2020-03-12: V3.0.2 Added setting for secondary menu background color
* 2021-06-14: V3.0.4 Added right-side icon
* 2021-08-07: V3.0.5 Show/hide child node hint arrow
* 2021-08-27: V3.0.6 Added custom TipsText display color
* 2021-12-13: V3.0.9 Selected item can set background color gradient
* 2022-01-02: V3.0.9 Scroll bar color can be set
* 2022-03-19: V3.1.1 Refactored theme colors
* 2022-03-24: V3.1.1 Fixed TipsText display position
* 2022-04-14: V3.1.3 Refactored extension functions
* 2022-06-23: V3.2.0 Added SymbolOffset for drawing node font icons
* 2022-08-19: V3.2.3 Fixed selected node right-side icon foreground color
* 2022-11-03: V3.2.6 Added property to set vertical scroll bar width
* 2022-11-03: V3.2.6 Rewrote the drawing of the node right-side icon
* 2023-02-02: V3.3.1 Fixed mouse leave event
* 2023-02-10: V3.3.2 When there are child nodes, left-click on the parent node to expand/collapse, right-click to select
* 2023-05-12: V3.3.6 Refactored DrawString function
* 2023-05-16: V3.3.6 Refactored DrawFontImage function
* 2023-05-29: V3.3.7 Added PageGuid related extension methods
* 2023-11-16: V3.5.2 Refactored theme
* 2024-04-13: V3.6.5 Fixed issue where setting background color through code was ineffective
* 2024-05-17: V3.6.6 Prevented control flickering
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("MenuItemClick")]
    [DefaultProperty("Nodes")]
    public sealed class UINavMenu : TreeView, IStyleInterface, IZoomScale
    {
        public delegate void OnMenuItemClick(TreeNode node, NavMenuItem item, int pageIndex);

        public event OnMenuItemClick MenuItemClick;

        private readonly UIScrollBar Bar = new UIScrollBar();

        public UINavMenu()
        {
            base.SetStyle(
                        ControlStyles.DoubleBuffer |
                        ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.ResizeRedraw |
                        ControlStyles.SupportsTransparentBackColor, true);
            base.UpdateStyles();
            DoubleBuffered = true;
            this.HotTracking = true;
            this.CheckBoxes = false;
            this.ShowPlusMinus = false;
            this.ShowRootLines = false;

            BorderStyle = BorderStyle.None;
            //HideSelection = false;
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            FullRowSelect = true;
            ShowLines = false;

            base.Font = UIStyles.Font();
            ItemHeight = 50;
            BackColor = Color.FromArgb(56, 56, 56);

            Bar.ValueChanged += Bar_ValueChanged;
            Bar.Dock = DockStyle.Right;
            Bar.Visible = false;
            Bar.Style = UIStyle.Custom;
            Bar.StyleCustomMode = true;
            Bar.FillColor = fillColor;

            Bar.ForeColor = Color.Silver;
            Bar.HoverColor = Color.Silver;
            Bar.PressColor = Color.Silver;
            Bar.ZoomScaleDisabled = true;

            Controls.Add(Bar);
            Version = UIGlobal.Version;
            SetScrollInfo();

            selectedForeColor = UIStyles.Blue.NavMenuMenuSelectedColor;
            selectedHighColor = UIStyles.Blue.NavMenuMenuSelectedColor;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int Style = 0;
            if (DoubleBuffered) Style |= 0x0004;
            if (Style != 0) Win32.User.SendMessage(Handle, 0x112C, new IntPtr(0x0004), new IntPtr(Style));
        }

        private int scrollBarWidth = 0;

        [DefaultValue(0), Category("SunnyUI"), Description("Vertical scroll bar width, minimum is the native scroll bar width")]
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

        [DefaultValue(6), Category("SunnyUI"), Description("Vertical scroll bar handle width, minimum is the native scroll bar width")]
        public int ScrollBarHandleWidth
        {
            get => scrollBarHandleWidth;
            set
            {
                scrollBarHandleWidth = value;
                if (Bar != null) Bar.FillWidth = value;
            }
        }

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
        public void SetZoomScale(float scale)
        {

        }

        [Description("Scroll bar fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "56, 56, 56")]
        public Color ScrollFillColor
        {
            get => Bar.FillColor;
            set
            {
                menuStyle = UIMenuStyle.Custom;
                Bar.FillColor = value;
                Invalidate();
            }
        }

        [Description("Scroll bar color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color ScrollBarColor
        {
            get => Bar.ForeColor;
            set
            {
                menuStyle = UIMenuStyle.Custom;
                Bar.ForeColor = value;
                Invalidate();
            }
        }

        [Description("Scroll bar hover color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color ScrollBarHoverColor
        {
            get => Bar.HoverColor;
            set
            {
                menuStyle = UIMenuStyle.Custom;
                Bar.HoverColor = value;
                Invalidate();
            }
        }

        [Description("Scroll bar press color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color ScrollBarPressColor
        {
            get => Bar.PressColor;
            set
            {
                menuStyle = UIMenuStyle.Custom;
                Bar.PressColor = value;
                Invalidate();
            }
        }

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        [DefaultValue(false)]
        [Description("Only show one open node"), Category("SunnyUI")]
        public bool ShowOneNode { get; set; }

        private bool showItemsArrow = true;

        [DefaultValue(true)]
        [Description("Show child node hint arrow"), Category("SunnyUI")]
        public bool ShowItemsArrow
        {
            get => showItemsArrow;
            set
            {
                showItemsArrow = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        public void ClearAll()
        {
            Nodes.Clear();
            MenuHelper.Clear();
        }

        private Color backColor = Color.FromArgb(56, 56, 56);

        [DefaultValue(typeof(Color), "56, 56, 56")]
        [Description("Background color"), Category("SunnyUI")]
        public override Color BackColor
        {
            get => backColor;
            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    base.BackColor = value;
                    menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private Color fillColor = Color.FromArgb(56, 56, 56);

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "56, 56, 56")]
        public Color FillColor
        {
            get => fillColor;
            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private Color foreColor = Color.Silver;

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public override Color ForeColor
        {
            get => foreColor;
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            ScrollBarInfo.SetScrollValue(Handle, Bar.Value);
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        [DefaultValue(null)]
        [Description("Associated TabControl"), Category("SunnyUI")]
        public UITabControl TabControl { get; set; }

        private Color selectedColor = Color.FromArgb(36, 36, 36);

        private bool showTips;

        [Description("Show badge"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool ShowTips
        {
            get => showTips;
            set
            {
                if (showTips != value)
                {
                    showTips = value;
                    Invalidate();
                }
            }
        }

        private Font tipsFont = UIStyles.SubFont();

        [Description("Badge text font"), Category("SunnyUI")]
        [DefaultValue(typeof(Font), "Segoe UI, 9pt")]
        public Font TipsFont
        {
            get => tipsFont;
            set
            {
                if (!tipsFont.Equals(value))
                {
                    tipsFont = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "36, 36, 36")]
        [Description("Selected node color"), Category("SunnyUI")]
        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    menuStyle = UIMenuStyle.Custom;
                    Invalidate();
                }
            }
        }

        private bool fillColorGradient;

        [Description("Fill color gradient"), Category("SunnyUI")]
        [DefaultValue(false)]
        public bool SelectedColorGradient
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
        private void SetFillColor2(Color value)
        {
            if (selectedColor2 != value)
            {
                selectedColor2 = value;
                menuStyle = UIMenuStyle.Custom;
                Invalidate();
            }
        }

        /// <summary>
        /// Fill color
        /// </summary>
        private Color selectedColor2 = Color.FromArgb(36, 36, 36);

        /// <summary>
        /// Fill color, if the value is the background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "36, 36, 36")]
        public Color SelectedColor2
        {
            get => selectedColor2;
            set => SetFillColor2(value);
        }

        private Color selectedHighColor = UIColor.Blue;

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Selected node highlight color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color SelectedHighColor

        {
            get => selectedHighColor;
            set
            {
                selectedHighColor = value;
                Invalidate();
            }
        }

        private Color hoverColor = Color.FromArgb(76, 76, 76);

        [DefaultValue(typeof(Color), "76, 76, 76")]
        [Description("Mouse hover color"), Category("SunnyUI")]
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                if (hoverColor != value)
                {
                    hoverColor = value;
                    menuStyle = UIMenuStyle.Custom;
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
            selectedForeColor = uiColor.NavMenuMenuSelectedColor;
            selectedHighColor = uiColor.NavMenuMenuSelectedColor;
        }

        private UIMenuStyle menuStyle = UIMenuStyle.Black;

        [DefaultValue(UIMenuStyle.Black)]
        [Description("Navigation menu theme style"), Category("SunnyUI")]
        public UIMenuStyle MenuStyle
        {
            get => menuStyle;
            set
            {
                if (value != UIMenuStyle.Custom)
                {
                    SetMenuStyle(UIStyles.MenuColors[value]);
                }

                menuStyle = value;
            }
        }

        private void SetMenuStyle(UIMenuColor uiColor)
        {
            BackColor = uiColor.BackColor;
            fillColor = uiColor.BackColor;
            selectedColor = uiColor.SelectedColor;
            selectedColor2 = uiColor.SelectedColor2;
            foreColor = uiColor.UnSelectedForeColor;
            hoverColor = uiColor.HoverColor;
            secondBackColor = uiColor.SecondBackColor;

            if (Bar != null)
            {
                Bar.FillColor = uiColor.BackColor;
                Bar.ForeColor = uiColor.UnSelectedForeColor;
                Bar.HoverColor = uiColor.UnSelectedForeColor;
                Bar.PressColor = uiColor.UnSelectedForeColor;
            }

            Invalidate();
        }

        private Color selectedForeColor = UIColor.Blue;

        [DefaultValue(typeof(Color), "80, 160, 255")]
        [Description("Selected node font color"), Category("SunnyUI")]
        public Color SelectedForeColor
        {
            get => selectedForeColor;
            set
            {
                if (selectedForeColor != value)
                {
                    selectedForeColor = value;
                    Invalidate();
                }
            }
        }

        private bool ScrollBarVisible;

        private TreeNode CurrentNode;

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            TreeNode node = GetNodeAt(e.Location);
            if (node == null || CurrentNode == node)
            {
                return;
            }

            using Graphics g = CreateGraphics();
            if (CurrentNode != null && CurrentNode != SelectedNode)
            {
                ClearCurrentNode(g);
            }

            if (node != SelectedNode)
            {
                CurrentNode = node;
                OnDrawNode(new DrawTreeNodeEventArgs(g, CurrentNode, new Rectangle(0, CurrentNode.Bounds.Y, Width, CurrentNode.Bounds.Height), TreeNodeStates.Hot));
            }
        }

        private void ClearCurrentNode(Graphics g)
        {
            if (CurrentNode != null && CurrentNode != SelectedNode)
            {
                OnDrawNode(new DrawTreeNodeEventArgs(g, CurrentNode, new Rectangle(0, CurrentNode.Bounds.Y, Width, CurrentNode.Bounds.Height), TreeNodeStates.Default));
                CurrentNode = null;
            }
        }

        /// <summary>
        /// Override mouse leave event
        /// </summary>
        /// <param name="e">Mouse parameters</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            using Graphics g = CreateGraphics();
            ClearCurrentNode(g);
            base.OnMouseLeave(e);
        }

        private bool checkBoxes;

        [Browsable(false)]
        public new bool CheckBoxes
        {
            get => checkBoxes;
            set => checkBoxes = false;
        }

        private bool showSecondBackColor;

        [DefaultValue(false)]
        [Description("Show secondary node background color"), Category("SunnyUI")]
        public bool ShowSecondBackColor
        {
            get => showSecondBackColor;
            set
            {
                if (ShowSecondBackColor != value)
                {
                    showSecondBackColor = value;
                    Invalidate();
                }
            }
        }

        private Color secondBackColor = Color.FromArgb(66, 66, 66);

        [DefaultValue(typeof(Color), "66, 66, 66")]
        [Description("Secondary node background color"), Category("SunnyUI")]
        public Color SecondBackColor
        {
            get => secondBackColor;
            set
            {
                if (secondBackColor != value)
                {
                    secondBackColor = value;
                    Invalidate();
                }
            }
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.IsEmpty) return;
            if (BorderStyle != BorderStyle.None) BorderStyle = BorderStyle.None;
            if (checkBoxes != false) CheckBoxes = false;

            SetScrollInfo();

            if (e.Node == null || (e.Node.Bounds.Width <= 0 && e.Node.Bounds.Height <= 0 && e.Node.Bounds.X <= 0 && e.Node.Bounds.Y <= 0))
            {
                e.DrawDefault = true;
            }
            else
            {
                int drawLeft = e.Node.Level * 16 + 16 + 4;
                int imageLeft = drawLeft;
                bool haveImage = false;

                if (MenuHelper.GetSymbol(e.Node) > 0)
                {
                    haveImage = true;
                    drawLeft += MenuHelper.GetSymbolSize(e.Node) + 6;
                }
                else
                {
                    if (ImageList != null && ImageList.Images.Count > 0 && e.Node.ImageIndex >= 0 && e.Node.ImageIndex < ImageList.Images.Count)
                    {
                        haveImage = true;
                        drawLeft += ImageList.ImageSize.Width + 6;
                    }
                }

                if (e.Node == SelectedNode)
                {
                    if (SelectedColorGradient)
                    {
                        using LinearGradientBrush br = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Node.Bounds.Height), SelectedColor, SelectedColor2);
                        br.GammaCorrection = true;
                        e.Graphics.FillRectangle(br, new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(SelectedColor, new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));
                    }

                    e.Graphics.DrawString(e.Node.Text, Font, SelectedForeColor, new Rectangle(drawLeft, e.Bounds.Y, e.Bounds.Width - drawLeft, ItemHeight), ContentAlignment.MiddleLeft);
                    e.Graphics.FillRectangle(SelectedHighColor, new Rectangle(0, e.Bounds.Y, 4, e.Bounds.Height));
                }
                else if (e.Node == CurrentNode && (e.State & TreeNodeStates.Hot) != 0)
                {
                    e.Graphics.FillRectangle(HoverColor, new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));
                    e.Graphics.DrawString(e.Node.Text, Font, ForeColor, new Rectangle(drawLeft, e.Bounds.Y, e.Bounds.Width - drawLeft, ItemHeight), ContentAlignment.MiddleLeft);
                }
                else
                {
                    Color color = fillColor;
                    if (showSecondBackColor && e.Node.Level > 0)
                    {
                        color = SecondBackColor;
                    }

                    e.Graphics.FillRectangle(color, new Rectangle(new Point(0, e.Node.Bounds.Y), new Size(Width, e.Node.Bounds.Height)));
                    e.Graphics.DrawString(e.Node.Text, Font, ForeColor, new Rectangle(drawLeft, e.Bounds.Y, e.Bounds.Width - drawLeft, ItemHeight), ContentAlignment.MiddleLeft);
                }

                //画右侧图标
                Color rightSymbolColor = ForeColor;
                if (e.Node == SelectedNode) rightSymbolColor = SelectedForeColor;
                if (TreeNodeSymbols.ContainsKey(e.Node) && TreeNodeSymbols[e.Node].Count > 0)
                {
                    int size = e.Node.Nodes.Count > 0 ? 24 : 0;
                    int left = Width - size - 6;
                    if (Bar.Visible) left -= Bar.Width;

                    int firstLeft = left - TreeNodeSymbols[e.Node].Count * 30;
                    for (int i = 0; i < TreeNodeSymbols[e.Node].Count; i++)
                    {
                        e.Graphics.DrawFontImage(TreeNodeSymbols[e.Node][i], 24, rightSymbolColor, new Rectangle(firstLeft + i * 30, e.Bounds.Top, 30, e.Bounds.Height));
                    }
                }

                //画图片
                if (haveImage)
                {
                    if (MenuHelper.GetSymbol(e.Node) > 0)
                    {
                        Color color = e.Node == SelectedNode ? SelectedForeColor : ForeColor;
                        Point offset = MenuHelper.GetSymbolOffset(e.Node);
                        e.Graphics.DrawFontImage(MenuHelper.GetSymbol(e.Node), MenuHelper.GetSymbolSize(e.Node), color, new Rectangle(imageLeft, e.Bounds.Y, MenuHelper.GetSymbolSize(e.Node), e.Bounds.Height), offset.X, offset.Y, MenuHelper.GetSymbolRotate(e.Node));
                    }
                    else
                    {
                        if (e.Node == SelectedNode && e.Node.SelectedImageIndex >= 0 && e.Node.SelectedImageIndex < ImageList.Images.Count)
                            e.Graphics.DrawImage(ImageList.Images[e.Node.SelectedImageIndex], imageLeft, e.Bounds.Y + (e.Bounds.Height - ImageList.ImageSize.Height) / 2);
                        else
                            e.Graphics.DrawImage(ImageList.Images[e.Node.ImageIndex], imageLeft, e.Bounds.Y + (e.Bounds.Height - ImageList.ImageSize.Height) / 2);
                    }
                }

                //显示右侧下拉箭头
                if (ShowItemsArrow && e.Node.Nodes.Count > 0)
                {
                    int size = 24;
                    int left = Width - size - 6;
                    if (Bar.Visible) left -= Bar.Width;

                    SizeF sf = e.Graphics.GetFontImageSize(61702, 24);
                    Rectangle rect = new Rectangle((int)(left + sf.Width / 2) - 12, e.Bounds.Y, 24, e.Bounds.Height);
                    e.Graphics.DrawFontImage(e.Node.IsExpanded ? 61702 : 61703, 24, ForeColor, rect);
                }

                //显示Tips圆圈
                if (ShowTips && MenuHelper.GetTipsText(e.Node).IsValid())
                {
                    using var TempFont = TipsFont.DPIScaleFont(TipsFont.Size);
                    Size tipsSize = TextRenderer.MeasureText(MenuHelper.GetTipsText(e.Node), TempFont);
                    int sfMax = Math.Max(tipsSize.Width, tipsSize.Height);
                    int tipsLeft = Width - sfMax - 16;
                    if (e.Node.Nodes.Count > 0) tipsLeft -= 24;
                    if (Bar.Visible) tipsLeft -= Bar.Width;
                    if (TreeNodeSymbols.ContainsKey(e.Node)) tipsLeft -= TreeNodeSymbols[e.Node].Count * 30;
                    int tipsTop = e.Bounds.Y + (ItemHeight - sfMax) / 2;

                    if (MenuHelper[e.Node] != null)
                    {
                        if (MenuHelper[e.Node].TipsCustom)
                        {
                            e.Graphics.FillEllipse(MenuHelper[e.Node].TipsBackColor, tipsLeft - 1, tipsTop, sfMax, sfMax);
                            e.Graphics.DrawString(MenuHelper.GetTipsText(e.Node), TempFont, MenuHelper[e.Node].TipsForeColor, new Rectangle(tipsLeft, tipsTop, sfMax, sfMax), ContentAlignment.MiddleCenter);
                        }
                        else
                        {
                            e.Graphics.FillEllipse(TipsColor, tipsLeft - 1, tipsTop, sfMax, sfMax);
                            e.Graphics.DrawString(MenuHelper.GetTipsText(e.Node), TempFont, TipsForeColor, new Rectangle(tipsLeft, tipsTop, sfMax, sfMax), ContentAlignment.MiddleCenter);
                        }
                    }
                }
            }

            base.OnDrawNode(e);
        }

        private Color tipsColor = Color.Red;

        [DefaultValue(typeof(Color), "Red"), Category("SunnyUI"), Description("Node tip circle background color")]
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

        [DefaultValue(typeof(Color), "White"), Category("SunnyUI"), Description("Node tip circle text color")]
        public Color TipsForeColor
        {
            get => tipsForeColor;
            set
            {
                tipsForeColor = value;
                Invalidate();
            }
        }

        [Description("Select the first child node after expanding the node"), Category("SunnyUI"), DefaultValue(true)]
        public bool ExpandSelectFirst { get; set; } = true;

        public string Version { get; }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);
            if (e.Node == null) return;

            int size = e.Node.Nodes.Count > 0 ? 24 : 0;
            int left = Width - size - 6;
            if (Bar.Visible) left -= Bar.Width;

            int firstLeft = 0;
            if (TreeNodeSymbols.ContainsKey(e.Node))
                firstLeft = left - TreeNodeSymbols[e.Node].Count * 30;

            if (TreeNodeSymbols.ContainsKey(e.Node) && TreeNodeSymbols[e.Node].Count > 0 && e.X >= firstLeft && e.X < firstLeft + TreeNodeSymbols[e.Node].Count * 30)
            {
                int idx = (e.X - firstLeft) / 30;
                if (idx >= 0 && idx < TreeNodeSymbols[e.Node].Count)
                {
                    NodeRightSymbolClick?.Invoke(this, e.Node, idx, TreeNodeSymbols[e.Node][idx]);
                }
            }
            else
            {
                if (e.Node.Nodes.Count > 0)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (e.Node.IsExpanded)
                        {
                            e.Node.Collapse();
                        }
                        else
                        {
                            e.Node.Expand();
                        }

                        if (SelectedNode != null && SelectedNode == e.Node && e.Node.IsExpanded && ExpandSelectFirst && e.Node.Nodes.Count > 0)
                        {
                            SelectedNode = e.Node.Nodes[0];
                        }
                        else
                        {
                            SelectedNode = e.Node;
                        }
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        SelectedNode = e.Node;
                    }
                }
                else
                {
                    SelectedNode = e.Node;
                }

                ShowSelectedNode();
            }
        }

        public void SelectFirst()
        {
            if (Nodes.Count > 0)
            {
                if (Nodes[0].Nodes.Count > 0 && ExpandSelectFirst)
                {
                    Nodes[0].Expand();
                    SelectedNode = Nodes[0].Nodes[0];
                }
                else
                {
                    SelectedNode = Nodes[0];
                }

                Nodes[0].EnsureVisible();
            }

            ShowSelectedNode();
        }

        private void ShowSelectedNode()
        {
            NavMenuItem item = MenuHelper[SelectedNode];
            if (item != null)
            {
                if (item.PageGuid != Guid.Empty)
                {
                    TabControl?.SelectPage(item.PageGuid);
                }
                else
                {
                    if (item.PageIndex >= 0)
                    {
                        TabControl?.SelectPage(item.PageIndex);
                    }
                }
            }

            MenuItemClick?.Invoke(SelectedNode, MenuHelper[SelectedNode], GetPageIndex(SelectedNode));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 10)
            {
                ScrollBarInfo.ScrollUp(Handle);
            }
            else if (e.Delta < -10)
            {
                ScrollBarInfo.ScrollDown(Handle);
            }

            SetScrollInfo();
        }

        public void SetScrollInfo()
        {
            if (Nodes.Count == 0)
            {
                Bar.Visible = false;
                return;
            }

            int barWidth = Math.Max(ScrollBarInfo.VerticalScrollBarWidth(), ScrollBarWidth);
            var si = ScrollBarInfo.GetInfo(Handle);
            Bar.Maximum = si.ScrollMax;
            Bar.Visible = si.ScrollMax > 0 && si.nMax > 0 && si.nPage > 0;
            Bar.Value = si.nPos;
            Bar.Width = barWidth;
            Bar.BringToFront();

            if (ScrollBarVisible != Bar.Visible)
            {
                ScrollBarVisible = Bar.Visible;
                Invalidate();
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (ShowOneNode)
            {
                TreeNode node = e.Node.PrevNode;
                while (node != null)
                {
                    if (node.IsExpanded)
                    {
                        node.Collapse();
                    }
                    node = node.PrevNode;
                }

                node = e.Node.NextNode;
                while (node != null)
                {
                    if (node.IsExpanded)
                    {
                        node.Collapse();
                    }
                    node = node.NextNode;
                }
            }

            if (e.Node != null && ExpandSelectFirst && e.Node.Nodes.Count > 0)
            {
                e.Node.Expand();
                SelectedNode = e.Node.Nodes[0];
            }
            else
            {
                SelectedNode = e.Node;
            }
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            SetScrollInfo();
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);
            SetScrollInfo();
        }

        protected override void WndProc(ref Message m)
        {
            if (IsDisposed || Disposing) return;
            if (m.Msg == Win32.User.WM_ERASEBKGND)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
            Win32.User.ShowScrollBar(Handle, 3, false);
        }

        #region Extension functions
        private readonly NavMenuHelper MenuHelper = new NavMenuHelper();

        public void SelectPage(int pageIndex)
        {
            var node = MenuHelper.GetTreeNode(pageIndex);
            if (node != null)
            {
                SelectedNode = node;
                ShowSelectedNode();
            }
        }

        public void SelectPage(Guid pageGuid)
        {
            var node = MenuHelper.GetTreeNode(pageGuid);
            if (node != null)
            {
                SelectedNode = node;
                ShowSelectedNode();
            }
        }

        public int GetPageIndex(TreeNode node)
        {
            return MenuHelper.GetPageIndex(node);
        }

        public Guid GetPageGuid(TreeNode node)
        {
            return MenuHelper.GetGuid(node);
        }

        public TreeNode GetTreeNode(int pageIndex)
        {
            return MenuHelper.GetTreeNode(pageIndex);
        }

        public TreeNode GetTreeNode(Guid pageGuid)
        {
            return MenuHelper.GetTreeNode(pageGuid);
        }

        private void SetNodeItem(TreeNode node, NavMenuItem item)
        {
            MenuHelper.Add(node, item);
        }

        public UINavMenu SetNodePageIndex(TreeNode node, int pageIndex)
        {
            MenuHelper.SetPageIndex(node, pageIndex);
            return this;
        }

        public UINavMenu SetNodePageGuid(TreeNode node, Guid pageGuid)
        {
            MenuHelper.SetPageGuid(node, pageGuid);
            return this;
        }

        public UINavMenu SetNodeSymbol(TreeNode node, int symbol, int symbolSize = 24, int symbolRotate = 0)
        {
            MenuHelper.SetSymbol(node, symbol, symbolSize, symbolRotate);
            return this;
        }

        public UINavMenu SetNodeSymbol(TreeNode node, int symbol, Point symbolOffset, int symbolSize = 24, int symbolRotate = 0)
        {
            MenuHelper.SetSymbol(node, symbol, symbolOffset, symbolSize, symbolRotate);
            return this;
        }

        public UINavMenu SetNodeImageIndex(TreeNode node, int imageIndex)
        {
            node.ImageIndex = imageIndex;
            return this;
        }

        public void SetNodeTipsText(TreeNode node, string nodeTipsText)
        {
            MenuHelper.SetTipsText(node, nodeTipsText);
        }

        public void SetNodeTipsText(TreeNode node, string nodeTipsText, Color nodeTipsBackColor, Color nodeTipsForeColor)
        {
            MenuHelper.SetTipsText(node, nodeTipsText, nodeTipsBackColor, nodeTipsForeColor);
        }

        public TreeNode CreateNode(string text, int pageIndex)
        {
            return CreateNode(new NavMenuItem(text, pageIndex));
        }

        public TreeNode CreateNode(string text, Guid pageGuid)
        {
            return CreateNode(new NavMenuItem(text, pageGuid));
        }

        public TreeNode CreateNode(UIPage page)
        {
            return CreateNode(new NavMenuItem(page));
        }

        public TreeNode CreateNode(string text, int symbol, int symbolSize, int pageIndex, int symbolRotate = 0)
        {
            var node = CreateNode(text, pageIndex);
            SetNodeSymbol(node, symbol, symbolSize, symbolRotate);
            return node;
        }

        public TreeNode CreateNode(string text, int symbol, Point symbolOffset, int symbolSize, int pageIndex, int symbolRotate = 0)
        {
            var node = CreateNode(text, pageIndex);
            SetNodeSymbol(node, symbol, symbolOffset, symbolSize, symbolRotate);
            return node;
        }

        private TreeNode CreateNode(NavMenuItem item)
        {
            TreeNode node = new TreeNode(item.Text);
            Nodes.Add(node);
            SetNodeItem(node, item);
            return node;
        }

        public TreeNode CreateChildNode(TreeNode parent, string text, int pageIndex)
        {
            return CreateChildNode(parent, new NavMenuItem(text, pageIndex));
        }

        public TreeNode CreateChildNode(TreeNode parent, string text, Guid pageGuid)
        {
            return CreateChildNode(parent, new NavMenuItem(text, pageGuid));
        }

        public TreeNode CreateChildNode(TreeNode parent, UIPage page)
        {
            var childNode = CreateChildNode(parent, new NavMenuItem(page));
            if (page.Symbol > 0)
            {
                MenuHelper.SetSymbol(childNode, page.Symbol, page.SymbolOffset, page.SymbolSize, page.SymbolRotate);
            }

            return childNode;
        }

        public TreeNode CreateChildNode(TreeNode parent, string text, int symbol, int symbolSize, int pageIndex, int symbolRotate = 0)
        {
            var node = CreateChildNode(parent, text, pageIndex);
            SetNodeSymbol(node, symbol, symbolSize, symbolRotate);
            return node;
        }

        public TreeNode CreateChildNode(TreeNode parent, string text, int symbol, Point symbolOffset, int symbolSize, int pageIndex, int symbolRotate = 0)
        {
            var node = CreateChildNode(parent, text, pageIndex);
            SetNodeSymbol(node, symbol, symbolOffset, symbolSize, symbolRotate);
            return node;
        }

        private TreeNode CreateChildNode(TreeNode parent, NavMenuItem item)
        {
            TreeNode childNode = new TreeNode(item.Text);
            parent.Nodes.Add(childNode);
            SetNodeItem(childNode, item);
            return childNode;
        }

        private readonly Dictionary<TreeNode, List<int>> TreeNodeSymbols = new Dictionary<TreeNode, List<int>>();

        public void AddNodeRightSymbol(TreeNode node, int symbol)
        {
            if (!TreeNodeSymbols.ContainsKey(node))
                TreeNodeSymbols.Add(node, new List<int>());

            TreeNodeSymbols[node].Add(symbol);
            Invalidate();
        }

        public void RemoveNodeRightSymbol(TreeNode node, int symbol)
        {
            if (TreeNodeSymbols.ContainsKey(node))
            {
                int idx = TreeNodeSymbols[node].IndexOf(symbol);
                if (idx >= 0)
                {
                    TreeNodeSymbols[node].RemoveAt(idx);
                    Invalidate();
                }
            }
        }

        public void ClearNodeRightSymbol(TreeNode node)
        {
            if (TreeNodeSymbols.ContainsKey(node))
            {
                TreeNodeSymbols[node].Clear();
                Invalidate();
            }
        }

        #endregion Extension functions

        public delegate void OnNodeRightSymbolClick(object sender, TreeNode node, int index, int symbol);

        public event OnNodeRightSymbolClick NodeRightSymbolClick;
    }
}