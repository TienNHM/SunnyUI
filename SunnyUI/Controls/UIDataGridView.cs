/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, Multi-page Development Framework.
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIDataGridView.cs
 * File Description: Data Grid
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Enhanced functionality and beautification
 * 2020-07-15: V2.2.6 Changed default configuration to native
 * 2020-07-18: V2.2.6 Redrawn horizontal scrollbar
 * 2020-08-22: V2.2.7 Updated horizontal and vertical scrollbar display, optimized scrolling effect
 * 2020-08-28: V2.2.7 Adjusted horizontal scrollbar
 * 2021-03-25: V3.0.2 Modified vertical scrollbar to be consistent with the original version and added page scrolling
 * 2021-04-01: V3.0.2 Use Enter key instead of Tab key to jump to the next cell when editing input
 * 2021-04-29: V3.0.3 Set data row header color
 * 2021-05-22: V3.0.4 Added RowHeight, default 23
 * 2021-06-27: V3.0.4 Custom cell color
 * 2022-01-21: V3.1.0 Updated SelectedIndex value when single selection is selected
 * 2022-04-16: V3.1.3 Added scrollbar color settings
 * 2022-04-26: V3.1.8 Fixed "Index -1 does not have a value" error when DataSource binds List and List is empty
 * 2022-06-10: V3.1.9 No longer judge if DataSource binds List is empty, "Index -1 does not have a value" error should be judged by the user
 * 2022-06-11: V3.1.9 Hide ShowRect, set native property:
 *                    BorderStyle = BorderStyle.FixedSingle;
 * 2022-06-11: V3.1.9 Hide ShowGridLine, set native property:
 *                    CellBorderStyle = DataGridViewCellBorderStyle.Single;
 * 2022-06-11: V3.1.9 Hide RowHeight, use SetRowHeight() instead, or set native property:
 *                    AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
 *                    RowTemplate.Height set to height
 * 2022-06-22: V3.2.0 Deleted ShowRect, ShowGridLine, RowHeight three properties
 * 2022-07-11: V3.2.1 Fixed a display position issue with the scrollbar
 * 2022-07-11: V3.2.1 Added scrollbar border line settings
 * 2022-07-28: V3.2.2 Fixed the issue where the scrollbar still shows when ScrollBars is None
 * 2022-07-28: V3.2.2 Fixed the issue where the vertical scrollbar error occurs when dragging to the bottom with a single row and low table height
 * 2022-10-14: V3.2.6 Added property to set the width of the vertical scrollbar
 * 2023-06-28: V3.3.9 Added property to set the width of the horizontal scrollbar, but it may cover the data of the bottom row, use as needed
 * 2023-07-12: V3.4.0 Fixed the issue where the vertical scrollbar error occurs when clicking with frozen rows
 * 2023-11-05: V3.5.2 Refactored theme
 * 2024-06-19: V3.6.7 Added AddDateTimeColumn to solve the issue of default time column not displaying seconds
 * 2024-08-27: V3.7.0 Added property AutoScrollToBottom, whether to automatically scroll to the last row when data is updated
 * 2024-09-04: V3.7.0 Fixed the issue where the scrollbar error occurs when scrolling with hidden rows
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public class UIDataGridView : DataGridView, IStyleInterface, IZoomScale
    {
        private readonly UIScrollBar VBar = new UIScrollBar();
        private readonly UIHorScrollBarEx HBar = new UIHorScrollBarEx();

        public UIDataGridView()
        {
            BackgroundColor = UIColor.White;
            GridColor = UIColor.Blue;
            base.Font = UIStyles.Font();
            base.DoubleBuffered = true;

            VBar.Parent = this;
            VBar.Visible = false;
            HBar.FillColor = VBar.FillColor = UIColor.LightBlue;
            VBar.ForeColor = UIColor.Blue;
            VBar.StyleCustomMode = true;
            VBar.ValueChanged += VBarValueChanged;
            VBar.ShowLeftLine = true;

            HBar.Parent = this;
            HBar.Visible = false;
            HBar.ForeColor = UIColor.Blue;
            HBar.StyleCustomMode = true;
            HBar.ValueChanged += HBar_ValueChanged;

            SetBarPosition();

            //支持自定义标题行风格
            EnableHeadersVisualStyles = false;

            //标题行风格
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnHeadersDefaultCellStyle.BackColor = UIColor.Blue;
            ColumnHeadersDefaultCellStyle.ForeColor = UIColor.White;
            ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            ColumnHeadersDefaultCellStyle.Font = UIStyles.Font();

            //行头部颜色
            RowHeadersDefaultCellStyle.BackColor = UIColor.LightBlue;
            RowHeadersDefaultCellStyle.ForeColor = UIFontColor.Primary;
            RowHeadersDefaultCellStyle.SelectionBackColor = UIColor.Blue;
            RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            RowHeadersDefaultCellStyle.Font = UIStyles.Font();

            RowsDefaultCellStyle.Font = UIStyles.Font();
            DefaultCellStyle.Font = UIStyles.Font();

            //标题行行高，与OnColumnAdded事件配合
            ColumnHeadersHeight = 32;

            //设置奇偶数行颜色
            StripeEvenColor = UIColor.White;
            StripeOddColor = UIColor.LightBlue;

            VerticalScrollBar.ValueChanged += VerticalScrollBar_ValueChanged;
            HorizontalScrollBar.ValueChanged += HorizontalScrollBar_ValueChanged;
            VerticalScrollBar.VisibleChanged += VerticalScrollBar_VisibleChanged;
            HorizontalScrollBar.VisibleChanged += HorizontalScrollBar_VisibleChanged;
        }

        private int scrollBarWidth = 0;

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

        [DefaultValue(6), Category("SunnyUI"), Description("Vertical scrollbar handle width, minimum is the native scrollbar width")]
        public int ScrollBarHandleWidth
        {
            get => scrollBarHandleWidth;
            set
            {
                scrollBarHandleWidth = value;
                if (VBar != null) VBar.FillWidth = value;
            }
        }

        private int scrollBarHeight = 0;

        [DefaultValue(0), Category("SunnyUI"), Description("Horizontal scrollbar height, minimum is the native scrollbar width")]
        public int ScrollBarHeight
        {
            get => scrollBarHeight;
            set
            {
                scrollBarHeight = value;
                SetScrollInfo();
            }
        }

        private int scrollBarHandleHeight = 6;

        [DefaultValue(6), Category("SunnyUI"), Description("Horizontal scrollbar handle height, minimum is the native scrollbar width")]
        public int ScrollBarHandleHeight
        {
            get => scrollBarHandleHeight;
            set
            {
                scrollBarHandleHeight = value;
                if (HBar != null) HBar.FillHeight = value;
            }
        }

        /// <summary>
        /// 禁止控件跟随窗体缩放
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// 控件缩放前在其容器里的位置
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        [Description("Position of the control in its container before scaling")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// 设置控件缩放比例
        /// </summary>
        /// <param name="scale">缩放比例</param>
        public virtual void SetZoomScale(float scale)
        {

        }

        float ColumnHeadersDefaultCellStyleFontSize = -1;
        float RowHeadersDefaultCellStyleFontSize = -1;
        float DefaultCellStyleFontSize = -1;
        float RowsDefaultCellStyleFontSize = -1;

        public void SetDPIScale()
        {
            if (ColumnHeadersDefaultCellStyle.Font != null)
            {
                if (ColumnHeadersDefaultCellStyleFontSize < 0) ColumnHeadersDefaultCellStyleFontSize = ColumnHeadersDefaultCellStyle.Font.Size;
                ColumnHeadersDefaultCellStyle.Font = ColumnHeadersDefaultCellStyle.Font.DPIScaleFont(ColumnHeadersDefaultCellStyleFontSize);
            }

            if (RowHeadersDefaultCellStyle.Font != null)
            {
                if (RowHeadersDefaultCellStyleFontSize < 0) RowHeadersDefaultCellStyleFontSize = RowHeadersDefaultCellStyle.Font.Size;
                RowHeadersDefaultCellStyle.Font = RowHeadersDefaultCellStyle.Font.DPIScaleFont(RowHeadersDefaultCellStyleFontSize);
            }

            if (DefaultCellStyle.Font != null)
            {
                if (DefaultCellStyleFontSize < 0) DefaultCellStyleFontSize = DefaultCellStyle.Font.Size;
                DefaultCellStyle.Font = DefaultCellStyle.Font.DPIScaleFont(DefaultCellStyleFontSize);
            }

            if (RowsDefaultCellStyle.Font != null)
            {
                if (RowsDefaultCellStyleFontSize < 0) RowsDefaultCellStyleFontSize = RowsDefaultCellStyle.Font.Size;
                RowsDefaultCellStyle.Font = RowsDefaultCellStyle.Font.DPIScaleFont(RowsDefaultCellStyleFontSize);
            }
        }

        private readonly Dictionary<string, CellStyle> CellStyles = new Dictionary<string, CellStyle>();

        public class CellStyle
        {
            public int Row { get; set; }

            public int Col { get; set; }

            public Color BackColor { get; set; }

            public Color ForeColor { get; set; }

            public CellStyle(int row, int col, Color backColor, Color foreColor)
            {
                Row = row;
                Col = col;
                BackColor = backColor;
                ForeColor = foreColor;
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (CellStyles.Count > 0 && e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                object obj = e.Value;
                if (obj == null) return;

                string key = e.RowIndex + "_" + e.ColumnIndex;
                if (CellStyles.ContainsKey(key))
                {
                    e.CellStyle.ForeColor = CellStyles[key].ForeColor;
                    e.CellStyle.BackColor = CellStyles[key].BackColor;
                    e.CellStyle.SelectionForeColor = CellStyles[key].ForeColor;
                    e.CellStyle.SelectionBackColor = CellStyles[key].BackColor;
                }
            }
        }

        public void SetCellStyle(int row, int col, Color backColor, Color foreColor)
        {
            SetCellStyle(new CellStyle(row, col, backColor, foreColor));
        }

        public void SetCellStyle(CellStyle style)
        {
            string key = style.Row + "_" + style.Col;
            if (CellStyles.ContainsKey(key))
                CellStyles[key] = style;
            else
                CellStyles.Add(key, style);
        }

        public void ClearCellStyles()
        {
            CellStyles.Clear();
        }

        public void ClearCellStyle(int row, int col)
        {
            string key = row + "_" + col;
            if (CellStyles.ContainsKey(key))
            {
                CellStyles.Remove(key);
            }
        }

        public void SetRowHeight(int height)
        {
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            RowTemplate.Height = height;
        }

        public void SetColumnHeadersHeight(int height)
        {
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            ColumnHeadersHeight = height;
        }

        private void HorizontalScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            SetScrollInfo();
        }

        private void VerticalScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            SetScrollInfo();
        }

        public void Init()
        {
            //自动生成行
            AutoGenerateColumns = false;

            //列占满行
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //禁止调整数据行行高
            AllowUserToResizeRows = false;

            //允许调整标题行行宽
            AllowUserToResizeColumns = true;

            //禁用最后一行空白，自动新增行
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            //不显示表格线
            CellBorderStyle = DataGridViewCellBorderStyle.None;

            //禁止行多选
            MultiSelect = false;

            //不显示数据行标题
            RowHeadersVisible = false;

            //禁止只读
            //ReadOnly = false;

            //行选
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private bool isLightMode = false;
        public void LightMode()
        {
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            BackgroundColor = System.Drawing.Color.White;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            StripeOddColor = System.Drawing.Color.White;
            ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            GridColor = Color.LightGray;
            isLightMode = true;
        }

        private void VerticalScrollBar_ValueChanged(object sender, EventArgs e)
        {
            VBar.Value = FirstDisplayedScrollingRowIndex;
            VerticalScrollBarChanged?.Invoke(this, e);
        }

        private void VBarValueChanged(object sender, EventArgs e)
        {
            if (RowCount == 0) return;
            int idx = VBar.Value;
            if (idx < 0) idx = 0;
            if (idx >= RowCount) idx = RowCount - 1;

            SetFirstDisplayedScrollingRowIndex(idx);
        }

        private void SetFirstDisplayedScrollingRowIndex(int idx)
        {
            int lastFrozen = GetFrozenBottomIndex();
            int showidx = idx;
            if (Rows[0].Frozen)
            {
                if (RowCount > lastFrozen + 1)
                {
                    lastFrozen += 1;
                    showidx = Math.Max(idx, lastFrozen);
                }
            }

            bool isSet = false;
            for (int i = showidx; i < RowCount; i++)
            {
                if (Rows[i].Visible)
                {
                    showidx = i;
                    isSet = true;
                    break;
                }
            }

            if (!isSet)
            {
                for (int i = showidx; i > 0; i--)
                {
                    if (Rows[i].Visible && i > lastFrozen)
                    {
                        showidx = i;
                        isSet = true;
                        break;
                    }
                }
            }

            if (isSet)
            {
                FirstDisplayedScrollingRowIndex = showidx;
            }
        }

        private int GetFrozenBottomIndex()
        {
            int lastFrozen = 0;
            if (Rows[0].Frozen)
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i].Frozen)
                    {
                        lastFrozen = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return lastFrozen;
        }

        private void HorizontalScrollBar_ValueChanged(object sender, EventArgs e)
        {
            HBar.Value = HorizontalScrollBar.Value;
        }

        private void HBar_ValueChanged(object sender, EventArgs e)
        {
            HorizontalScrollBar.Value = HBar.Value;
            HorizontalScrollingOffset = HBar.Value;
            HorizontalScrollBarChanged?.Invoke(this, e);
        }

        public event EventHandler HorizontalScrollBarChanged;

        public event EventHandler VerticalScrollBarChanged;

        public void SetScrollInfo()
        {
            if (VBar == null || HBar == null)
            {
                return;
            }

            VBar.ThreadSafeCall(() =>
            {
                if (RowCount > DisplayedRowCount(false))
                {
                    VBar.Maximum = RowCount - DisplayedRowCount(false);
                    VBar.Value = FirstDisplayedScrollingRowIndex;
                    VBar.Visible = ScrollBars == ScrollBars.Vertical || ScrollBars == ScrollBars.Both;
                }
                else
                {
                    VBar.Visible = false;
                }
            });

            HBar.ThreadSafeCall(() =>
            {
                if (HorizontalScrollBar.Visible)
                {
                    HBar.Maximum = HorizontalScrollBar.Maximum;
                    HBar.Value = HorizontalScrollBar.Value;
                    HBar.BoundsWidth = HorizontalScrollBar.LargeChange;
                    HBar.LargeChange = HorizontalScrollBar.LargeChange;//.Maximum / VisibleColumnCount();
                    HBar.Visible = ScrollBars == ScrollBars.Horizontal || ScrollBars == ScrollBars.Both;
                }
                else
                {
                    HBar.Visible = false;
                }
            });

            SetBarPosition();
        }

        /// <summary>
        /// 重载绘图
        /// </summary>
        /// <param name="e">绘图参数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (BorderStyle == BorderStyle.FixedSingle)
            {
                Color color = RectColor;
                color = Enabled ? color : UIDisableColor.Fill;
                e.Graphics.DrawRectangle(color, new Rectangle(0, 0, Width - 1, Height - 1));
            }

            if (isLightMode)
            {
                e.Graphics.DrawLine(GridColor, 0, ColumnHeadersHeight - 1, Width, ColumnHeadersHeight - 1);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (VBar.Visible && ScrollMode == UIDataGridViewScrollMode.Page)
            {
                if (e.Delta > 10)
                {
                    var lineCount = Rows.GetLastRow(DataGridViewElementStates.Displayed) - FirstDisplayedScrollingRowIndex;
                    VBar.SetValue(VBar.Value - lineCount + 3);
                }
                else if (e.Delta < -10)
                {
                    var lineCount = FirstDisplayedScrollingRowIndex - Rows.GetLastRow(DataGridViewElementStates.Displayed);
                    VBar.SetValue(VBar.Value - lineCount - 3);
                }
            }
        }

        [Description("Vertical scrollbar scroll mode"), Category("SunnyUI")]
        [DefaultValue(UIDataGridViewScrollMode.Normal)]
        public UIDataGridViewScrollMode ScrollMode { get; set; } = UIDataGridViewScrollMode.Normal;

        public enum UIDataGridViewScrollMode
        {
            /// <summary>
            /// Normal
            /// </summary>
            Normal,
            /// <summary>
            /// Page
            /// </summary>
            Page
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            base.OnRowsAdded(e);
            SetScrollInfo();
        }

        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            base.OnRowsRemoved(e);
            SetScrollInfo();
        }

        /// <summary>
        /// 重载控件尺寸变更
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetScrollInfo();
            SetBarPosition();
        }

        protected override void OnColumnStateChanged(DataGridViewColumnStateChangedEventArgs e)
        {
            base.OnColumnStateChanged(e);
            SetScrollInfo();
        }

        protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        {
            base.OnColumnRemoved(e);
            SetScrollInfo();
        }

        private void SetBarPosition()
        {
            if (VBar == null || HBar == null)
            {
                return;
            }

            int barWidth = Math.Max(ScrollBarInfo.VerticalScrollBarWidth(), ScrollBarWidth);
            int barHeight = Math.Max(ScrollBarInfo.HorizontalScrollBarHeight(), ScrollBarHeight);

            if (BorderStyle == BorderStyle.FixedSingle)
            {
                VBar.ThreadSafeCall(() =>
                {
                    VBar.Left = Width - barWidth - 2;
                    VBar.Top = 1;
                    VBar.Width = barWidth + 1;
                    VBar.Height = Height - 2;
                    VBar.BringToFront();
                });

                HBar.ThreadSafeCall(() =>
                {
                    HBar.Left = 1;
                    HBar.Height = barHeight + 1;
                    HBar.Width = Width - (VBar.Visible ? VBar.Width : 0) - 2;
                    HBar.Top = Height - HBar.Height - 1;
                    HBar.BringToFront();
                });
            }
            else
            {
                VBar.ThreadSafeCall(() =>
                {
                    VBar.Left = Width - barWidth - 1;
                    VBar.Top = 0;
                    VBar.Width = barWidth + 1;
                    VBar.Height = Height;
                    VBar.BringToFront();
                });

                HBar.ThreadSafeCall(() =>
                {
                    HBar.Left = 0;
                    HBar.Height = barHeight + 1;
                    HBar.Width = Width - (VBar.Visible ? VBar.Width : 0);
                    HBar.Top = Height - HBar.Height;
                    HBar.BringToFront();
                });
            }
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            //设置可调整标题行行高
            if (ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize)
            {
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            }

            SetScrollInfo();
        }

        private UIStyle _style = UIStyle.Inherited;

        /// <summary>
        /// 主题样式
        /// </summary>
        [DefaultValue(UIStyle.Inherited), Description("Theme style"), Category("SunnyUI")]
        public UIStyle Style
        {
            get => _style;
            set => SetStyle(value);
        }

        [DefaultValue(typeof(Color), "White")]
        [Description("Even row display color"), Category("SunnyUI")]
        public Color StripeEvenColor
        {
            get => RowsDefaultCellStyle.BackColor;
            set
            {
                RowsDefaultCellStyle.BackColor = value;
                Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "243, 249, 255")]
        [Description("Odd row display color"), Category("SunnyUI")]
        public Color StripeOddColor
        {
            get => AlternatingRowsDefaultCellStyle.BackColor;
            set
            {
                AlternatingRowsDefaultCellStyle.BackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 设置主题样式
        /// </summary>
        /// <param name="style">主题样式</param>
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
            BackgroundColor = uiColor.PlainColor;

            //列头部颜色
            ColumnHeadersDefaultCellStyle.BackColor = uiColor.GridTitleColor;
            ColumnHeadersDefaultCellStyle.ForeColor = uiColor.GridTitleForeColor;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = uiColor.GridTitleColor;

            //行头部颜色
            RowHeadersDefaultCellStyle.BackColor = uiColor.PlainColor;
            RowHeadersDefaultCellStyle.ForeColor = uiColor.GridForeColor;
            RowHeadersDefaultCellStyle.SelectionBackColor = uiColor.RectColor;
            RowHeadersDefaultCellStyle.SelectionForeColor = uiColor.GridForeColor;

            //数据单元格选中颜色
            DefaultCellStyle.SelectionBackColor = uiColor.GridSelectedColor;
            DefaultCellStyle.SelectionForeColor = uiColor.GridSelectedForeColor;
            DefaultCellStyle.BackColor = uiColor.GridStripeEvenColor;
            DefaultCellStyle.ForeColor = uiColor.GridForeColor;

            //数据行选中颜色            
            RowsDefaultCellStyle.SelectionBackColor = uiColor.GridSelectedColor;
            RowsDefaultCellStyle.SelectionForeColor = uiColor.GridSelectedForeColor;
            RowsDefaultCellStyle.ForeColor = uiColor.GridForeColor;

            GridColor = uiColor.GridLineColor;
            RectColor = uiColor.RectColor;
            RowsDefaultCellStyle.BackColor = uiColor.GridStripeEvenColor;
            AlternatingRowsDefaultCellStyle.BackColor = uiColor.GridStripeOddColor;

            StripeEvenColor = uiColor.GridStripeEvenColor;
            StripeOddColor = uiColor.GridStripeOddColor;

            if (HBar != null && HBar.Style == UIStyle.Inherited)
            {
                HBar.ForeColor = uiColor.GridBarForeColor;
                HBar.HoverColor = uiColor.ButtonFillHoverColor;
                HBar.PressColor = uiColor.ButtonFillPressColor;
                HBar.FillColor = uiColor.GridBarFillColor;
                //HBar.RectColor = uiColor.RectColor;
                scrollBarColor = uiColor.GridBarForeColor;
                scrollBarBackColor = uiColor.GridBarFillColor;
            }

            if (VBar != null && VBar.Style == UIStyle.Inherited)
            {
                VBar.ForeColor = uiColor.GridBarForeColor;
                VBar.HoverColor = uiColor.ButtonFillHoverColor;
                VBar.PressColor = uiColor.ButtonFillPressColor;
                VBar.FillColor = uiColor.GridBarFillColor;
                scrollBarRectColor = VBar.RectColor = uiColor.RectColor;
                scrollBarColor = uiColor.GridBarForeColor;
                scrollBarBackColor = uiColor.GridBarFillColor;
            }
        }

        /// <summary>
        /// 自定义主题风格
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set whether custom theme style is allowed"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        public string Version => UIGlobal.Version;

        /// <summary>
        /// Tag字符串
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        protected override void OnCellBorderStyleChanged(EventArgs e)
        {
            base.OnCellBorderStyleChanged(e);
            VBar.ShowLeftLine = CellBorderStyle == DataGridViewCellBorderStyle.Single;
        }

        private Color _rectColor = UIColor.Blue;

        [DefaultValue(typeof(Color), "80, 160, 255")]
        [Description("Border color"), Category("SunnyUI")]
        public Color RectColor
        {
            get => _rectColor;
            set
            {
                if (_rectColor != value)
                {
                    _rectColor = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get
            {
                return CurrentRow != null ? CurrentRow.Index : -1;
            }
            set
            {
                //BindingContext[DataSource].Position = value;
                if (Rows.Count == 0)
                {
                    return;
                }

                if (value >= 0 && value < Rows.Count)
                {
                    foreach (DataGridViewRow row in SelectedRows)
                    {
                        row.Selected = false;
                    }

                    Rows[value].Selected = true;

                    SetFirstDisplayedScrollingRowIndex(value);

                    if (selectedIndex >= 0 && selectedIndex <= Rows.Count)
                        jumpIndex = selectedIndex;

                    selectedIndex = value;
                    SelectIndexChange?.Invoke(this, value);
                }
            }
        }

        private int jumpIndex = -1;

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
            SetScrollInfo();
        }

        public delegate void OnSelectIndexChange(object sender, int index);

        public event OnSelectIndexChange SelectIndexChange;

        protected override void OnRowEnter(DataGridViewCellEventArgs e)
        {
            base.OnRowEnter(e);

            if (e.RowIndex == jumpIndex)
            {
                jumpIndex = -1;
                return;
            }

            if (selectedIndex != e.RowIndex)
            {
                selectedIndex = e.RowIndex;
                SelectIndexChange?.Invoke(this, e.RowIndex);
            }
        }

        private int selectedIndex = -1;

        public DataGridViewColumn AddColumn(string columnName, string dataPropertyName, int fillWeight = 100, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleCenter, bool readOnly = true)
        {
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = columnName;
            column.DataPropertyName = dataPropertyName;
            column.Name = columnName;
            column.ReadOnly = readOnly;
            column.FillWeight = fillWeight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.DefaultCellStyle.Alignment = alignment;
            Columns.Add(column);
            return column;
        }

        public DataGridViewColumn AddCheckBoxColumn(string columnName, string dataPropertyName, int fillWeight = 100, bool readOnly = true)
        {
            DataGridViewColumn column = new DataGridViewCheckBoxColumn();
            column.HeaderText = columnName;
            column.DataPropertyName = dataPropertyName;
            column.Name = columnName;
            column.ReadOnly = readOnly;
            column.FillWeight = fillWeight;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns.Add(column);
            return column;
        }

        public DataGridViewColumn AddButtonColumn(string columnName, string dataPropertyName, int fillWeight = 100, bool readOnly = true)
        {
            DataGridViewColumn column = new DataGridViewButtonColumn();
            column.HeaderText = columnName;
            column.DataPropertyName = dataPropertyName;
            column.Name = columnName;
            column.ReadOnly = readOnly;
            column.FillWeight = fillWeight;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Columns.Add(column);
            return column;
        }

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public DataGridViewColumn AddDateTimeColumn(string columnName, string dataPropertyName, string format = DateTimeFormat, int fillWeight = 100, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleCenter, bool readOnly = true)
        {
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = columnName;
            column.DataPropertyName = dataPropertyName;
            column.Name = columnName;
            column.ReadOnly = readOnly;
            column.FillWeight = fillWeight;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.DefaultCellStyle.Alignment = alignment;
            column.DefaultCellStyle.Format = format;
            Columns.Add(column);
            return column;
        }

        public virtual void ClearRows()
        {
            if (DataSource != null)
            {
                DataSource = null;
            }

            Rows.Clear();
        }

        public virtual void ClearColumns()
        {
            Columns.Clear();
        }

        public virtual void ClearAll()
        {
            ClearRows();
            ClearColumns();
        }

        public int AddRow(params object[] values)
        {
            return Rows.Add(values);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (EnterAsTab)
            {
                Keys key = (keyData & Keys.KeyCode);
                if (key == Keys.Enter)
                {
                    //交由自定义控件处理
                    return false;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (EnterAsTab)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    return this.ProcessTabKey(e.KeyData);
                }
            }

            return base.ProcessDataGridViewKey(e);
        }

        [DefaultValue(false)]
        [Description("Use Enter key instead of Tab key to jump to the next cell when editing input"), Category("SunnyUI")]
        public bool EnterAsTab { get; set; }

        private Color scrollBarColor = Color.FromArgb(80, 160, 255);

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("Scrollbar fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ScrollBarColor
        {
            get => scrollBarColor;
            set
            {
                scrollBarColor = value;
                HBar.HoverColor = HBar.PressColor = HBar.ForeColor = value;
                VBar.HoverColor = VBar.PressColor = VBar.ForeColor = value;
                HBar.Style = VBar.Style = UIStyle.Custom;
                Invalidate();
            }
        }

        private Color scrollBarRectColor = Color.FromArgb(80, 160, 255);

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("Scrollbar border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ScrollBarRectColor
        {
            get => scrollBarRectColor;
            set
            {
                scrollBarRectColor = value;
                VBar.RectColor = value;
                HBar.Style = VBar.Style = UIStyle.Custom;
                Invalidate();
            }
        }

        private Color scrollBarBackColor = Color.FromArgb(243, 249, 255);

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("Scrollbar background color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color ScrollBarBackColor
        {
            get => scrollBarBackColor;
            set
            {
                scrollBarBackColor = value;
                HBar.FillColor = value;
                VBar.FillColor = value;
                HBar.Style = VBar.Style = UIStyle.Custom;
                Invalidate();
            }
        }

        /// <summary>
        /// 滚动条主题样式
        /// </summary>
        [DefaultValue(true), Description("Scrollbar theme style"), Category("SunnyUI")]
        public bool ScrollBarStyleInherited
        {
            get => HBar != null && HBar.Style == UIStyle.Inherited;
            set
            {
                if (value)
                {
                    if (HBar != null) HBar.Style = UIStyle.Inherited;
                    if (VBar != null) VBar.Style = UIStyle.Inherited;

                    scrollBarColor = UIStyles.Blue.GridBarForeColor;
                    scrollBarBackColor = UIStyles.Blue.GridBarFillColor;
                    scrollBarRectColor = VBar.RectColor = UIStyles.Blue.RectColor;
                }

            }
        }

        [DefaultValue(false), Category("SunnyUI"), Description("Indicates whether to automatically scroll to the last row when new data is added to the DataGridView")]
        public bool AutoScrollToBottom { get; set; }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);
            //是否自动滚动到最后一行
            if (AutoScrollToBottom && this.RowCount > 0)
            {
                //不选中单元格或行
                this.CurrentCell = null;
                //选中最后一行
                this.Rows[this.RowCount - 1].Selected = true;
                //滚动到最后一行
                SetRowHeight(this.RowCount - 1);
                //如果需要滚动到底部（右侧），使用下面的代码
                //this.FirstDisplayedCell = this.Rows[this.RowCount - 1].Cells[this.Columns.Count - 1];
            }
        }
    }

    public static class UIDataGridViewHelper
    {
        public static DataGridViewColumn SetFixedMode(this DataGridViewColumn column, int width)
        {
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
            return column;
        }

        public static DataGridViewColumn SetSortMode(this DataGridViewColumn column, DataGridViewColumnSortMode sortMode = DataGridViewColumnSortMode.Automatic)
        {
            column.SortMode = sortMode;
            return column;
        }

        public static bool IsDBNull(this DataGridViewCell cell)
        {
            return cell.Value is DBNull;
        }
    }
}