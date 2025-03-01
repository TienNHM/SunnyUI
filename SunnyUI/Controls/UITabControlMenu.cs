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
 * If you use this code, please keep this note.
 ******************************************************************************
 * File name: UITabControlMenu.cs
 * File description: Tab menu control
 * Current version: V3.1
 * Creation date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2022-08-11: V3.0.2 Rewritten ItemSize, adjusted width and height for normal display
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2025-02-07: V3.8.1 Fixed the issue where TabPage background color was not set when switching theme colors, #IBKDR7
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public sealed class UITabControlMenu : TabControl, IStyleInterface, IZoomScale
    {
        public UITabControlMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            UpdateStyles();

            base.ItemSize = new Size(40, 200);
            DrawMode = TabDrawMode.OwnerDrawFixed;
            base.Font = UIStyles.Font();
            AfterSetFillColor(FillColor);
            Size = new Size(450, 270);
            Version = UIGlobal.Version;

            tabSelectedForeColor = UIStyles.Blue.TabControlTabSelectedColor;
            tabSelectedHighColor = UIStyles.Blue.TabControlTabSelectedColor;
            _fillColor = UIStyles.Blue.TabControlBackColor;
        }

        [DefaultValue(typeof(Size), "200, 40")]
        public new Size ItemSize
        {
            get => new Size(base.ItemSize.Height, base.ItemSize.Width);
            set
            {
                base.ItemSize = new Size(value.Height, value.Width);
                Invalidate();
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

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set custom theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        public string Version { get; }

        private HorizontalAlignment textAlignment = HorizontalAlignment.Center;

        [Description("Text display direction"), Category("SunnyUI")]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment TextAlignment
        {
            get => textAlignment;
            set
            {
                textAlignment = value;
                Invalidate();
            }
        }

        private Color _fillColor = UIColor.LightBlue;
        private Color tabBackColor = Color.FromArgb(56, 56, 56);

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
                if (_fillColor != value)
                {
                    _fillColor = value;
                    AfterSetFillColor(value);
                    Invalidate();
                }
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
        /// Border color
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

        private Color tabSelectedForeColor = UIColor.Blue;

        /// <summary>
        /// Border color
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

        private Color tabUnSelectedForeColor = Color.Silver;

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Unselected Tab page font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
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
        /// Border color
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

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = base.DisplayRectangle;
                return new Rectangle(rect.Left - 3, rect.Top - 4, rect.Width + 7, rect.Height + 8);
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
            TabBackColor = uiColor.BackColor;
            TabSelectedColor = uiColor.SelectedColor;
            TabUnSelectedForeColor = uiColor.UnSelectedForeColor;
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;
            Alignment = TabAlignment.Left;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is TabPage)
            {
                //e.Control.BackColor = FillColor;
                e.Control.Padding = new Padding(0);
            }
        }

        /// <summary>
        /// Override drawing
        /// </summary>
        /// <param name="e">Drawing parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw background color
            e.Graphics.Clear(TabBackColor);
            for (int index = 0; index <= TabCount - 1; index++)
            {
                Rectangle TabRect = new Rectangle(GetTabRect(index).Location.X - 2, GetTabRect(index).Location.Y - 2, base.ItemSize.Height + 4, base.ItemSize.Width);
                Size sf = TextRenderer.MeasureText(TabPages[index].Text, Font);
                int textLeft = 4 + 6 + 4 + (ImageList?.ImageSize.Width ?? 0);
                if (TextAlignment == HorizontalAlignment.Right)
                    textLeft = TabRect.Width - 4 - sf.Width;
                if (TextAlignment == HorizontalAlignment.Center)
                    textLeft = textLeft + (int)((TabRect.Width - textLeft - sf.Width) / 2.0f);

                if (index == SelectedIndex)
                {
                    // Draw selected Tab background color
                    e.Graphics.FillRectangle(TabSelectedColor, TabRect.X, TabRect.Y, TabRect.Width, TabRect.Height + 4);

                    // Draw selected Tab highlight
                    e.Graphics.FillRectangle(TabSelectedHighColor, TabRect.X, TabRect.Y, 4, TabRect.Height + 3);
                }

                // Draw title
                Color textColor = index == SelectedIndex ? tabSelectedForeColor : TabUnSelectedForeColor;
                e.Graphics.DrawString(TabPages[index].Text, Font, textColor, new Rectangle(textLeft, TabRect.Top, TabRect.Width, TabRect.Height), ContentAlignment.MiddleLeft);

                // Draw icon
                if (ImageList != null)
                {
                    int imageIndex = TabPages[index].ImageIndex;
                    if (imageIndex >= 0 && imageIndex < ImageList.Images.Count)
                    {
                        e.Graphics.DrawImage(ImageList.Images[imageIndex], TabRect.X + 4 + 6, TabRect.Y + 2 + (TabRect.Height - ImageList.ImageSize.Height) / 2.0f, ImageList.ImageSize.Width, ImageList.ImageSize.Height);
                    }
                }
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Init(SelectedIndex);
        }

        public void Init(int index = 0)
        {
            if (index < 0 || index >= TabPages.Count)
            {
                return;
            }

            if (SelectedIndex != index)
                SelectedIndex = index;

            List<UIPage> pages = TabPages[SelectedIndex].GetControls<UIPage>();
            foreach (var page in pages)
            {
                page.Init();
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
    }
}