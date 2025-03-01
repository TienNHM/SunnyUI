﻿/******************************************************************************
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
 * File Name: UIImageListBox.cs
 * File Description: Image list box
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-05-21: V2.2.5 Added mouse hover highlight
 * 2021-08-07: V3.0.5 Load images from file and release occupation
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-08-30: V3.2.3 Added some events
 * 2022-09-05: V3.2.3 Fixed Click and DoubleClick events
 * 2022-11-03: V3.2.6 Added property to set vertical scrollbar width
 * 2023-05-13: V3.3.6 Refactored DrawString function
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("ItemClick")]
    public sealed partial class UIImageListBox : UIPanel, IToolTip
    {
        private readonly ImageListBox listbox = new ImageListBox();
        private readonly UIScrollBar bar = new UIScrollBar();

        public UIImageListBox()
        {
            SetStyleFlags(true, false);
            ShowText = false;
            Padding = new Padding(2);

            bar.ValueChanged += Bar_ValueChanged;
            bar.Width = SystemInformation.VerticalScrollBarWidth + 2;
            bar.Parent = this;
            bar.Dock = DockStyle.None;
            bar.Visible = false;

            listbox.Parent = this;
            listbox.Dock = DockStyle.Fill;
            listbox.Show();
            listbox.Bar = bar;

            listbox.SelectedIndexChanged += Listbox_SelectedIndexChanged;
            listbox.SelectedValueChanged += Listbox_SelectedValueChanged;
            listbox.Click += Listbox_Click;
            listbox.DoubleClick += Listbox_DoubleClick;
            listbox.BeforeDrawItem += Listbox_BeforeDrawItem;
            listbox.MouseDown += Listbox_MouseDown;
            listbox.MouseUp += Listbox_MouseUp;
            listbox.MouseMove += Listbox_MouseMove;

            listbox.MouseClick += Listbox_MouseClick;
            listbox.MouseDoubleClick += Listbox_MouseDoubleClick;
            listbox.KeyPress += Listbox_KeyPress;
            listbox.KeyDown += Listbox_KeyDown;
            listbox.KeyUp += Listbox_KeyUp;
            listbox.MouseEnter += Listbox_MouseEnter;
            listbox.MouseLeave += Listbox_MouseLeave;
            listbox.DrawItem += Listbox_DrawItem;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        public override void SetDPIScale()
        {
            base.SetDPIScale();
            listbox.SetDPIScale();
        }

        private void InitializeComponent()
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            bar?.Dispose();
            listbox?.Dispose();
        }

        private Color scrollBarColor = Color.FromArgb(80, 160, 255);

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
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

        private Color scrollBarBackColor = Color.FromArgb(243, 249, 255);

        /// <summary>
        /// Scrollbar background color
        /// </summary>
        [Description("Scrollbar background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
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

                    scrollBarColor = UIStyles.Blue.ListBarForeColor;
                    scrollBarBackColor = UIStyles.Blue.ListBarFillColor;
                }
            }
        }

        private int scrollBarWidth = 0;

        /// <summary>
        /// Vertical scrollbar width, minimum is the native scrollbar width
        /// </summary>
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

        /// <summary>
        /// Vertical scrollbar handle width, minimum is the native scrollbar width
        /// </summary>
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

        public event DrawItemEventHandler DrawItem;
        public new event EventHandler MouseLeave;
        public new event EventHandler MouseEnter;
        public new event KeyPressEventHandler KeyPress;
        public new event KeyEventHandler KeyDown;
        public new event KeyEventHandler KeyUp;
        public new event MouseEventHandler MouseClick;

        public new event MouseEventHandler MouseDoubleClick;

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
            if (listbox != null) listbox.ContextMenuStrip = ContextMenuStrip;
        }

        private void Listbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        private void Listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawItem?.Invoke(sender, e);
        }

        private void Listbox_MouseLeave(object sender, EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        private void Listbox_MouseEnter(object sender, EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        private void Listbox_MouseClick(object sender, MouseEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }

        private void Listbox_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        private void Listbox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        private void Listbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        /// <summary>
        /// Control that needs additional ToolTip
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return listbox;
        }

        public int IndexFromPoint(Point p)
        {
            return listbox.IndexFromPoint(p);
        }

        public int IndexFromPoint(int x, int y)
        {
            return listbox.IndexFromPoint(x, y);
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            if (listbox != null)
            {
                listbox.BackColor = color;
            }
        }

        private void Listbox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        private void Listbox_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        private void Listbox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event MouseEventHandler MouseMove;

        /// <summary>
        /// Override font change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (DefaultFontSize < 0 && listbox != null) listbox.Font = this.Font;
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetScrollInfo();
        }

        private void SetScrollInfo()
        {
            bar.Top = 2;
            bar.Height = Height - 4;
            int barWidth = Math.Max(ScrollBarInfo.VerticalScrollBarWidth() + Padding.Right, ScrollBarWidth);
            bar.Width = barWidth + 1;
            bar.Left = Width - barWidth - 3;
        }

        private void Listbox_BeforeDrawItem(object sender, ListBox.ObjectCollection items, DrawItemEventArgs e)
        {
            if (Items.Count != LastCount)
            {
                listbox.SetScrollInfo();
                LastCount = Items.Count;
                ItemsCountChange?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Listbox_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        private void Listbox_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        [Browsable(false)]
        public int Count => Items.Count;

        public new event EventHandler Click;

        public new event EventHandler DoubleClick;

        public event EventHandler ItemsCountChange;

        public event EventHandler SelectedIndexChanged;

        public event EventHandler SelectedValueChanged;

        private void Listbox_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectedValueChanged?.Invoke(this, e);
            Text = listbox.SelectedItem?.ToString();
        }

        private void Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        [DefaultValue(100)]
        [Description("Item height"), Category("SunnyUI")]
        public int ItemHeight
        {
            get => listbox.ItemHeight;
            set => listbox.ItemHeight = value;
        }

        [DefaultValue(4)]
        [Description("Image text interval"), Category("SunnyUI")]
        public int ImageInterval
        {
            get => listbox.ImageInterval;
            set => listbox.ImageInterval = value;
        }

        [DefaultValue(true)]
        [Description("Show description text"), Category("SunnyUI")]
        public bool ShowDescription
        {
            get => listbox.ShowDescription;
            set => listbox.ShowDescription = value;
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            if (bar != null && bar.Style == UIStyle.Inherited)
            {
                bar.ForeColor = uiColor.ListBarForeColor;
                bar.HoverColor = uiColor.ButtonFillHoverColor;
                bar.PressColor = uiColor.ButtonFillPressColor;
                bar.FillColor = uiColor.ListBarFillColor;

                scrollBarColor = uiColor.ListBarForeColor;
                scrollBarBackColor = uiColor.ListBarFillColor;
            }

            hoverColor = uiColor.ListItemHoverColor;
            if (listbox != null)
            {
                listbox.HoverColor = hoverColor;
                listbox.SetStyleColor(uiColor);
                listbox.BackColor = uiColor.ListBackColor;
                listbox.ForeColor = uiColor.ListForeColor;
            }

            fillColor = uiColor.ListBackColor;
        }

        private int LastCount;

        private int lastBarValue = -1;

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            if (listbox != null)
            {
                if (bar.Value != lastBarValue)
                {
                    ScrollBarInfo.SetScrollValue(listbox.Handle, bar.Value);
                    lastBarValue = bar.Value;
                }
            }
        }

        protected override void OnRadiusChanged(int value)
        {
            base.OnRadiusChanged(value);
            Padding = new Padding(Math.Max(2, value / 2));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [MergableProperty(false)]
        [Browsable(false)]
        [Description("List items"), Category("SunnyUI")]
        public ListBox.ObjectCollection Items => listbox.Items;

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="record">Image object</param>
        public void AddImage(ImageListItem record)
        {
            Items.Add(record);
        }

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="imagePath">Image path</param>
        /// <param name="description">Image description</param>
        public void AddImage(string imagePath, string description = "")
        {
            AddImage(new ImageListItem(imagePath, description));
        }

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="description">Image description</param>
        public void AddImage(Bitmap image, string description = "")
        {
            AddImage(new ImageListItem(image, description));
        }

        public void SelectedFirst()
        {
            listbox.SelectedFirst();
        }

        [DefaultValue(typeof(Color), "80, 160, 255")]
        [Description("Selected item background color"), Category("SunnyUI")]
        public Color ItemSelectBackColor
        {
            get => listbox.ItemSelectBackColor;
            set => listbox.ItemSelectBackColor = value;
        }

        [DefaultValue(typeof(Color), "243, 249, 255")]
        [Description("Selected item font color"), Category("SunnyUI")]
        public Color ItemSelectForeColor
        {
            get => listbox.ItemSelectForeColor;
            set => listbox.ItemSelectForeColor = value;
        }

        [Browsable(false)]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get => listbox.SelectedIndex;
            set => listbox.SelectedIndex = value;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public ImageListItem SelectedItem
        {
            get => listbox.SelectedItem as ImageListItem;
            set => listbox.SelectedItem = value;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public object SelectedValue
        {
            get => listbox.SelectedValue;
            set => listbox.SelectedValue = value;
        }

        private Color hoverColor = Color.FromArgb(155, 200, 255);

        [DefaultValue(typeof(Color), "220, 236, 255")]
        [Description("Mouse hover color"), Category("SunnyUI")]
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
                listbox.HoverColor = hoverColor;
            }
        }

        [ToolboxItem(false)]
        private sealed class ImageListBox : ListBox, IStyleInterface
        {
            private UIScrollBar bar;

            /// <summary>
            /// Tag string
            /// </summary>
            [DefaultValue(null)]
            [Description("Gets or sets the object string containing data about the control"), Category("SunnyUI")]
            public string TagString { get; set; }

            public UIScrollBar Bar
            {
                get => bar;
                set
                {
                    bar = value;
                    SetScrollInfo();
                }
            }

            private float DefaultFontSize = -1;

            public void SetDPIScale()
            {
                if (!UIDPIScale.NeedSetDPIFont()) return;
                if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
                this.SetDPIScaleFont(DefaultFontSize);
            }

            //protected override void WndProc(ref Message m)
            //{
            //    if (IsDisposed || Disposing) return;
            //    if (IsHandleCreated)
            //    {
            //        if (m.Msg == Win32.User.WM_ERASEBKGND)
            //        {
            //            m.Result = IntPtr.Zero;
            //            return;
            //        }
            //    }
            //
            //    base.WndProc(ref m);
            //}

            public ImageListBox()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);
                UpdateStyles();

                BorderStyle = BorderStyle.None;
                ForeColor = UIFontColor.Primary;
                IntegralHeight = false;
                ItemHeight = 100;
                DrawMode = DrawMode.OwnerDrawFixed;
                Version = UIGlobal.Version;
                SetScrollInfo();
            }

            /// <summary>
            /// Override control size change
            /// </summary>
            /// <param name="e">Parameters</param>
            protected override void OnSizeChanged(EventArgs e)
            {
                if (Bar != null && Bar.Visible)
                {
                    if (Bar.Value != 0)
                    {
                        ScrollBarInfo.SetScrollValue(Handle, Bar.Value);
                    }
                }
                //SetScrollInfo();
            }

            public void SetScrollInfo()
            {
                if (Bar == null)
                {
                    return;
                }

                var si = ScrollBarInfo.GetInfo(Handle);
                if (si.ScrollMax > 0)
                {
                    Bar.Maximum = si.ScrollMax;
                    Bar.Visible = si.ScrollMax > 0 && si.nMax > 0 && si.nPage > 0;
                    Bar.Value = si.nPos;
                }
                else
                {
                    Bar.Visible = false;
                }
            }

            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                e.ItemHeight += ItemHeight;
            }

            /// <summary>
            /// Custom theme style
            /// </summary>
            [DefaultValue(false), Browsable(false)]
            [Description("Gets or sets the ability to customize the theme style"), Category("SunnyUI")]
            public bool StyleCustomMode { get; set; }

            public string Version { get; }

            private UIStyle _style = UIStyle.Inherited;
            private Color _itemSelectBackColor = UIColor.Blue;
            private Color _itemSelectForeColor = Color.White;
            private int imageInterval = 4;
            private bool showDescription = true;

            [Browsable(false)]
            public int Count => Items.Count;

            public void SelectedFirst()
            {
                if (Count > 0 && SelectedIndex < 0)
                {
                    SelectedIndex = 0;
                }
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);

                if (Bar != null && Bar.Visible)
                {
                    var si = ScrollBarInfo.GetInfo(Handle);
                    int temp = Math.Abs(e.Delta / 120);
                    if (e.Delta > 10)
                    {
                        int nposnum = si.nPos - temp * SystemInformation.MouseWheelScrollLines;
                        ScrollBarInfo.SetScrollValue(Handle, nposnum >= si.nMin ? nposnum : 0);
                    }
                    else if (e.Delta < -10)
                    {
                        int nposnum = si.nPos + temp * SystemInformation.MouseWheelScrollLines;
                        ScrollBarInfo.SetScrollValue(Handle, nposnum <= si.ScrollMax ? nposnum : si.ScrollMax);
                    }
                }

                SetScrollInfo();
            }

            [DefaultValue(4)]
            public int ImageInterval
            {
                get => imageInterval;
                set
                {
                    if (imageInterval != value)
                    {
                        imageInterval = value;
                        Invalidate();
                    }
                }
            }

            [DefaultValue(true)]
            public bool ShowDescription
            {
                get => showDescription;
                set
                {
                    if (showDescription != value)
                    {
                        showDescription = value;
                        Invalidate();
                    }
                }
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
                ItemSelectBackColor = uiColor.ListItemSelectBackColor;
                ItemSelectForeColor = uiColor.ListItemSelectForeColor;
            }

            [Category("SunnyUI"), Description("The border color used to paint the control.")]
            public Color ItemSelectBackColor
            {
                get => _itemSelectBackColor;
                set
                {
                    if (_itemSelectBackColor != value)
                    {
                        _itemSelectBackColor = value;
                        Invalidate();
                    }
                }
            }

            [Category("SunnyUI"), Description("The border color used to paint the control.")]
            public Color ItemSelectForeColor
            {
                get => _itemSelectForeColor;
                set
                {
                    if (_itemSelectForeColor != value)
                    {
                        _itemSelectForeColor = value;
                        Invalidate();
                    }
                }
            }

            /// <summary>
            /// Add image
            /// </summary>
            /// <param name="record">Image object</param>
            public void AddImage(ImageListItem record)
            {
                Items.Add(record);
            }

            /// <summary>
            /// Add image
            /// </summary>
            /// <param name="imagePath">Image path</param>
            /// <param name="description">Image description</param>
            public void AddImage(string imagePath, string description = "")
            {
                AddImage(new ImageListItem(imagePath, description));
            }

            public void AddImage(Bitmap image, string description = "")
            {
                AddImage(new ImageListItem(image, description));
            }

            public delegate void OnBeforeDrawItem(object sender, ObjectCollection items, DrawItemEventArgs e);

            public event OnBeforeDrawItem BeforeDrawItem;

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                base.OnDrawItem(e);
                BeforeDrawItem?.Invoke(this, Items, e);
                if (Items.Count == 0)
                {
                    return;
                }

                bool otherState = e.State == DrawItemState.Grayed || e.State == DrawItemState.HotLight;
                if (!otherState)
                {
                    e.DrawBackground();
                }

                if (e.Index < 0 || e.Index >= Items.Count)
                {
                    return;
                }

                bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                Color backColor = isSelected ? ItemSelectBackColor : BackColor;
                Color foreColor = isSelected ? ItemSelectForeColor : ForeColor;
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

                if (!otherState)
                {
                    e.Graphics.FillRectangle(BackColor, e.Bounds);
                    e.Graphics.FillRectangle(backColor, rect);
                }
                else
                {
                    if (e.State == DrawItemState.Grayed)
                    {
                        backColor = BackColor;
                        foreColor = ForeColor;
                    }

                    if (e.State == DrawItemState.HotLight)
                    {
                        backColor = HoverColor;
                        foreColor = ForeColor;
                    }

                    e.Graphics.FillRectangle(BackColor, e.Bounds);
                    e.Graphics.FillRectangle(backColor, rect);
                }

                Graphics g = e.Graphics;
                Matrix oldTransform = g.Transform;
                Matrix newTransform = oldTransform.Clone();
                newTransform.Translate(e.Bounds.X, e.Bounds.Y);
                g.Transform = newTransform;
                ImageListItem item = (ImageListItem)Items[e.Index];
                Size sf = TextRenderer.MeasureText("ImageListBox", Font);
                int thumbnailSize = ShowDescription ? ((int)(ItemHeight - ImageInterval - sf.Height)) : (ItemHeight - ImageInterval * 2);

                if (item.Image != null)
                {
                    if (item.Image.Width <= thumbnailSize && item.Image.Height <= thumbnailSize)
                    {
                        g.DrawImage(item.Image, new Rectangle(ImageInterval, ImageInterval, item.Image.Width, item.Image.Height));
                    }
                    else
                    {
                        float scale = thumbnailSize * 1.0f / item.Image.Height;
                        g.DrawImage(item.Image, new Rectangle(ImageInterval, ImageInterval, (int)(item.Image.Width * scale), (int)(item.Image.Height * scale)));
                    }
                }

                if (ShowDescription && !string.IsNullOrEmpty(item.Description))
                {
                    g.DrawString(item.Description, e.Font, foreColor, new Rectangle(ImageInterval, e.Bounds.Y + thumbnailSize, e.Bounds.Width, e.Bounds.Height), ContentAlignment.TopLeft);
                }

                g.Transform = oldTransform;
            }

            private Color hoverColor = Color.FromArgb(155, 200, 255);

            [DefaultValue(typeof(Color), "155, 200, 255")]
            public Color HoverColor
            {
                get => hoverColor;
                set => hoverColor = value;
            }

            private int lastIndex = -1;
            private int mouseIndex = -1;

            [Browsable(false)]
            public int MouseIndex
            {
                get => mouseIndex;
                set
                {
                    if (mouseIndex != value)
                    {
                        if (lastIndex >= 0 && lastIndex != SelectedIndex)
                        {
                            using var g = CreateGraphics();
                            OnDrawItem(new DrawItemEventArgs(g, Font, GetItemRectangle(lastIndex), lastIndex, DrawItemState.Grayed));
                        }

                        mouseIndex = value;
                        if (mouseIndex >= 0 && mouseIndex != SelectedIndex)
                        {
                            using var g = CreateGraphics();
                            OnDrawItem(new DrawItemEventArgs(g, Font, GetItemRectangle(value), value, DrawItemState.HotLight));
                        }

                        lastIndex = mouseIndex;
                    }
                }
            }

            /// <summary>
            /// Override mouse move event
            /// </summary>
            /// <param name="e">Mouse parameters</param>
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                MouseIndex = IndexFromPoint(e.Location);
            }

            /// <summary>
            /// Override mouse leave event
            /// </summary>
            /// <param name="e">Mouse parameters</param>
            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                MouseIndex = -1;
            }
        }

        public class ImageListItem : IDisposable
        {
            public string ImagePath { get; private set; }

            public string Description { get; set; }

            public Image Image { get; private set; }

            public ImageListItem(string imagePath, string description = "")
            {
                if (File.Exists(imagePath))
                {
                    ImagePath = imagePath;
                    Image = ImageEx.FromFile(imagePath);
                }

                Description = description;
            }

            public ImageListItem(Bitmap image, string description = "")
            {
                ImagePath = "";
                Image = new Bitmap(image);
                Description = description;
            }

            public override string ToString()
            {
                return Description + ", " + ImagePath;
            }

            /// <summary>
            /// Destructor
            /// </summary>
            public void Dispose()
            {
                Image?.Dispose();
            }
        }
    }
}