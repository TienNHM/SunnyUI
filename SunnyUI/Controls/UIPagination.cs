﻿/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
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
 * File Name: UIPagination.cs
 * Description: Pagination
 * Current Version: V3.1
 * Creation Date: 2020-07-26
 *
 * 2020-07-15: V2.2.6 Added pagination control
 * 2021-03-27: V3.0.2 Fixed issue where no refresh occurs if the number of queries is equal twice
 * 2021-07-10: V3.0.4 Set total count not to refresh if the page does not exceed the total number of pages
 * 2023-06-27: V3.3.9 Changed built-in button associated value from Tag to TagString
 * 2023-08-30: V3.4.2 Changed text of left and right jump buttons to font icons
 * 2024-02-19: V3.6.3 Optimized button color matching logic
 * 2024-05-29: V3.6.6 Optimized custom button color matching logic
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public class UIPagination : UIPanel, ITranslate
    {
        public delegate void OnPageChangeEventHandler(object sender, object pagingSource, int pageIndex, int count);

        private int activePage = 1;
        private UISymbolButton b0;
        private UISymbolButton b1;
        private UISymbolButton b10;
        private UISymbolButton b11;
        private UISymbolButton b12;
        private UISymbolButton b13;
        private UISymbolButton b14;
        private UISymbolButton b15;
        private UISymbolButton b16;
        private UISymbolButton b2;
        private UISymbolButton b3;
        private UISymbolButton b4;
        private UISymbolButton b5;
        private UISymbolButton b6;
        private UISymbolButton b7;
        private UISymbolButton b8;
        private UISymbolButton b9;
        private UISymbolButton btnSelect;

        private readonly ConcurrentDictionary<int, UISymbolButton> buttons = new ConcurrentDictionary<int, UISymbolButton>();
        private readonly ConcurrentDictionary<UISymbolButton, int> buttonTags = new ConcurrentDictionary<UISymbolButton, int>();
        private CurrencyManager dataManager;

        private object dataSource;
        private UITextBox edtPage;

        private bool inSetDataConnection;

        /// <summary>
        /// Total number of pages
        /// </summary>
        [Browsable(false)]
        [Description("Total number of pages"), Category("SunnyUI")]
        public int PageCount { get; private set; }

        private int pagerCount = 7;

        private int pageSize = 20;

        private int totalCount = 1000;
        private UILabel uiLabel1;
        private UILabel uiLabel2;

        public UIPagination()
        {
            InitializeComponent();

            SetStyleFlags(true, false);

            ShowText = false;
            buttons.TryAdd(0, b0);
            buttons.TryAdd(1, b1);
            buttons.TryAdd(2, b2);
            buttons.TryAdd(3, b3);
            buttons.TryAdd(4, b4);
            buttons.TryAdd(5, b5);
            buttons.TryAdd(6, b6);
            buttons.TryAdd(7, b7);
            buttons.TryAdd(8, b8);
            buttons.TryAdd(9, b9);
            buttons.TryAdd(10, b10);
            buttons.TryAdd(11, b11);
            buttons.TryAdd(12, b12);
            buttons.TryAdd(13, b13);
            buttons.TryAdd(14, b14);
            buttons.TryAdd(15, b15);
            buttons.TryAdd(16, b16);

            for (var i = 0; i < 17; i++)
            {
                buttons[i].MouseEnter += UIDataGridPage_MouseEnter;
                buttons[i].MouseLeave += UIDataGridPage_MouseLeave;
                buttons[i].Click += UIDataGridPage_Click;
            }

            buttonTags.TryAdd(b0, -1);
            buttonTags.TryAdd(b16, 1);
            for (var i = 0; i < 17; i++)
            {
                if (buttonTags.NotContainsKey(buttons[i]))
                    buttonTags.TryAdd(buttons[i], 0);
            }

            Translate();
        }

        /// <summary>
        /// 重载字体变更
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (DefaultFontSize < 0)
            {
                foreach (var item in this.GetControls<UISymbolButton>(true))
                    item.Font = Font;

                foreach (var item in this.GetControls<UITextBox>(true))
                    item.Font = Font;

                foreach (var item in this.GetControls<UIComboBox>(true))
                    item.Font = Font;

                foreach (var item in this.GetControls<UILabel>(true))
                    item.Font = Font;
            }

            Translate();
        }

        public override void SetDPIScale()
        {
            base.SetDPIScale();
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;

            foreach (var item in this.GetControls<UISymbolButton>(true)) item.SetDPIScale();
            foreach (var item in this.GetControls<UITextBox>(true)) item.SetDPIScale();
            foreach (var item in this.GetControls<UIComboBox>(true)) item.SetDPIScale();
            foreach (var item in this.GetControls<UILabel>(true)) item.SetDPIScale();
        }

        public void Translate()
        {
            if (b0 == null) return;

            try
            {
                b0.Text = UIStyles.CurrentResources.Prev;
                b16.Text = UIStyles.CurrentResources.Next;
                btnSelect.Text = UIStyles.CurrentResources.SelectTitle;

                Size sf = TextRenderer.MeasureText(b0.Text, b0.Font);
                b0.Width = b0.SymbolSize + sf.Width + 10;

                sf = TextRenderer.MeasureText(b16.Text, b0.Font);
                b16.Width = b16.SymbolSize + sf.Width + 10;

                btnSelect.Width = TextRenderer.MeasureText(btnSelect.Text, btnSelect.Font).Width + 16;

                uiLabel1.Text = UIStyles.CurrentResources.SelectPageLeft;
                uiLabel2.Text = UIStyles.CurrentResources.SelectPageRight;
                edtPage.Left = uiLabel1.Right + 3;
                uiLabel2.Left = edtPage.Right + 3;
                btnSelect.Left = uiLabel2.Right + 3;
                uiLabel2.Top = uiLabel1.Top = (Height - uiLabel1.Height) / 2;

                SetShowButtons();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private int buttonInterval = 4;

        /// <summary>
        /// Button interval
        /// </summary>
        [DefaultValue(4)]
        [Description("Button interval")]
        [Category("SunnyUI")]
        public int ButtonInterval
        {
            get => buttonInterval;
            set
            {
                buttonInterval = Math.Max(0, value);
                buttonInterval = Math.Min(5, value);
                SetShowButtons();
            }
        }

        /// <summary>
        /// Total number of items
        /// </summary>
        [DefaultValue(1000)]
        [Description("Total number of items")]
        [Category("SunnyUI")]
        public int TotalCount
        {
            get => totalCount;
            set
            {
                totalCount = Math.Max(0, value);
                SetShowButtons();
                if (ActivePage > PageCount)
                    ActivePage = 1;
            }
        }

        /// <summary>
        /// Number of items displayed per page
        /// </summary>
        [DefaultValue(20)]
        [Description("Number of items displayed per page")]
        [Category("SunnyUI")]
        public int PageSize
        {
            get => pageSize;
            set
            {
                if (pageSize != value)
                {
                    pageSize = Math.Max(1, value);
                    SetShowButtons();
                    DataBind();
                }
            }
        }

        /// <summary>
        /// Number of page buttons, will fold when the total number of pages exceeds this value
        /// Must be an odd number between 5 and 13 inclusive
        /// </summary>
        [DefaultValue(7)]
        [Description("Number of page buttons, will fold when the total number of pages exceeds this value, must be an odd number between 5 and 13 inclusive")]
        [Category("SunnyUI")]
        public int PagerCount
        {
            get => pagerCount;
            set
            {
                if (pagerCount != value)
                {
                    pagerCount = Math.Max(5, value);
                    pagerCount = Math.Min(13, pagerCount);
                    if (pagerCount.Mod(2) == 0)
                        pagerCount = pagerCount - 1;

                    SetShowButtons();
                    DataBind();
                }
            }
        }

        /// <summary>
        /// Selected page
        /// </summary>
        [DefaultValue(1)]
        [Description("Selected page")]
        [Category("SunnyUI")]
        public int ActivePage
        {
            get => activePage;
            set
            {
                activePage = Math.Max(1, value);
                edtPage.IntValue = activePage;
                SetShowButtons();
                DataBind();
            }
        }

        public event EventHandler DataSourceChanged;

        /// <summary>
        /// Data source
        /// </summary>
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        [Description("Data source"), Category("SunnyUI")]
        public object DataSource
        {
            get => dataSource;
            set
            {
                if (value != null)
                {
                    if (!(value is DataTable || value is IList))
                    {
                        throw new Exception(UIStyles.CurrentResources.GridDataSourceException);
                    }
                }

                SetDataConnection(value, new BindingMemberInfo(""));
                dataSource = value;
                activePage = 1;
                TotalCount = dataManager?.List.Count ?? 0;
                DataSourceChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public object PageDataSource { get; private set; }

        private void UIDataGridPage_Click(object sender, EventArgs e)
        {
            var btn = (UISymbolButton)sender;
            btn.BringToFront();
            if (btn.TagString.IsValid())
                ActivePage += buttonTags[btn];
            else
                ActivePage = buttonTags[btn];
        }

        private void UIDataGridPage_MouseLeave(object sender, EventArgs e)
        {
            var btn = (UISymbolButton)sender;
            if (btn.TagString == "<<" || btn.TagString == ">>")
            {
                btn.Symbol = 361761;
                btn.Text = "";
            }
        }

        private void UIDataGridPage_MouseEnter(object sender, EventArgs e)
        {
            var btn = (UISymbolButton)sender;
            if (btn.TagString == "<<")
            {
                btn.Symbol = 361696;
                btn.Text = "";
            }

            if (btn.TagString == ">>")
            {
                btn.Symbol = 361697;
                btn.Text = "";
            }
        }

        #region InitializeComponent

        private void InitializeComponent()
        {
            b0 = new UISymbolButton();
            b1 = new UISymbolButton();
            b3 = new UISymbolButton();
            b2 = new UISymbolButton();
            b7 = new UISymbolButton();
            b6 = new UISymbolButton();
            b5 = new UISymbolButton();
            b4 = new UISymbolButton();
            b15 = new UISymbolButton();
            b14 = new UISymbolButton();
            b13 = new UISymbolButton();
            b12 = new UISymbolButton();
            b11 = new UISymbolButton();
            b10 = new UISymbolButton();
            b9 = new UISymbolButton();
            b8 = new UISymbolButton();
            b16 = new UISymbolButton();
            edtPage = new UITextBox();
            btnSelect = new UISymbolButton();
            uiLabel2 = new UILabel();
            uiLabel1 = new UILabel();
            SuspendLayout();
            // 
            // b0
            // 
            b0.Cursor = Cursors.Hand;
            b0.Font = new Font("Segoe UI", 10.5F);
            b0.ImageAlign = ContentAlignment.MiddleLeft;
            b0.Location = new Point(3, 3);
            b0.MinimumSize = new Size(1, 1);
            b0.Name = "b0";
            b0.Padding = new Padding(5, 0, 5, 0);
            b0.RadiusSides = UICornerRadiusSides.LeftTop | UICornerRadiusSides.LeftBottom;
            b0.Size = new Size(75, 29);
            b0.Symbol = 61700;
            b0.TabIndex = 0;
            b0.TagString = "<";
            b0.Text = "上一页";
            b0.TextAlign = ContentAlignment.MiddleRight;
            b0.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b1
            // 
            b1.Cursor = Cursors.Hand;
            b1.Font = new Font("Segoe UI", 12F);
            b1.Location = new Point(81, 3);
            b1.MinimumSize = new Size(1, 1);
            b1.Name = "b1";
            b1.RadiusSides = UICornerRadiusSides.None;
            b1.Size = new Size(29, 29);
            b1.Symbol = 0;
            b1.TabIndex = 1;
            b1.Text = "0";
            b1.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b3
            // 
            b3.Cursor = Cursors.Hand;
            b3.Font = new Font("Segoe UI", 12F);
            b3.Location = new Point(145, 3);
            b3.MinimumSize = new Size(1, 1);
            b3.Name = "b3";
            b3.RadiusSides = UICornerRadiusSides.None;
            b3.Size = new Size(29, 29);
            b3.Symbol = 0;
            b3.TabIndex = 3;
            b3.Text = "0";
            b3.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b2
            // 
            b2.Cursor = Cursors.Hand;
            b2.Font = new Font("Segoe UI", 12F);
            b2.Location = new Point(113, 3);
            b2.MinimumSize = new Size(1, 1);
            b2.Name = "b2";
            b2.RadiusSides = UICornerRadiusSides.None;
            b2.Size = new Size(29, 29);
            b2.Symbol = 0;
            b2.TabIndex = 2;
            b2.Text = "0";
            b2.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b7
            // 
            b7.Cursor = Cursors.Hand;
            b7.Font = new Font("Segoe UI", 12F);
            b7.Location = new Point(273, 3);
            b7.MinimumSize = new Size(1, 1);
            b7.Name = "b7";
            b7.RadiusSides = UICornerRadiusSides.None;
            b7.Size = new Size(29, 29);
            b7.Symbol = 0;
            b7.TabIndex = 7;
            b7.Text = "0";
            b7.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b6
            // 
            b6.Cursor = Cursors.Hand;
            b6.Font = new Font("Segoe UI", 12F);
            b6.Location = new Point(241, 3);
            b6.MinimumSize = new Size(1, 1);
            b6.Name = "b6";
            b6.RadiusSides = UICornerRadiusSides.None;
            b6.Size = new Size(29, 29);
            b6.Symbol = 0;
            b6.TabIndex = 6;
            b6.Text = "0";
            b6.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b5
            // 
            b5.Cursor = Cursors.Hand;
            b5.Font = new Font("Segoe UI", 12F);
            b5.Location = new Point(209, 3);
            b5.MinimumSize = new Size(1, 1);
            b5.Name = "b5";
            b5.RadiusSides = UICornerRadiusSides.None;
            b5.Size = new Size(29, 29);
            b5.Symbol = 0;
            b5.TabIndex = 5;
            b5.Text = "0";
            b5.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b4
            // 
            b4.Cursor = Cursors.Hand;
            b4.Font = new Font("Segoe UI", 12F);
            b4.Location = new Point(177, 3);
            b4.MinimumSize = new Size(1, 1);
            b4.Name = "b4";
            b4.RadiusSides = UICornerRadiusSides.None;
            b4.Size = new Size(29, 29);
            b4.Symbol = 0;
            b4.TabIndex = 4;
            b4.Text = "0";
            b4.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b15
            // 
            b15.Cursor = Cursors.Hand;
            b15.Font = new Font("Segoe UI", 12F);
            b15.Location = new Point(529, 3);
            b15.MinimumSize = new Size(1, 1);
            b15.Name = "b15";
            b15.RadiusSides = UICornerRadiusSides.None;
            b15.Size = new Size(29, 29);
            b15.Symbol = 0;
            b15.TabIndex = 15;
            b15.Text = "0";
            b15.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b14
            // 
            b14.Cursor = Cursors.Hand;
            b14.Font = new Font("Segoe UI", 12F);
            b14.Location = new Point(497, 3);
            b14.MinimumSize = new Size(1, 1);
            b14.Name = "b14";
            b14.RadiusSides = UICornerRadiusSides.None;
            b14.Size = new Size(29, 29);
            b14.Symbol = 0;
            b14.TabIndex = 14;
            b14.Text = "0";
            b14.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b13
            // 
            b13.Cursor = Cursors.Hand;
            b13.Font = new Font("Segoe UI", 12F);
            b13.Location = new Point(465, 3);
            b13.MinimumSize = new Size(1, 1);
            b13.Name = "b13";
            b13.RadiusSides = UICornerRadiusSides.None;
            b13.Size = new Size(29, 29);
            b13.Symbol = 0;
            b13.TabIndex = 13;
            b13.Text = "0";
            b13.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b12
            // 
            b12.Cursor = Cursors.Hand;
            b12.Font = new Font("Segoe UI", 12F);
            b12.Location = new Point(433, 3);
            b12.MinimumSize = new Size(1, 1);
            b12.Name = "b12";
            b12.RadiusSides = UICornerRadiusSides.None;
            b12.Size = new Size(29, 29);
            b12.Symbol = 0;
            b12.TabIndex = 12;
            b12.Text = "0";
            b12.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b11
            // 
            b11.Cursor = Cursors.Hand;
            b11.Font = new Font("Segoe UI", 12F);
            b11.Location = new Point(401, 3);
            b11.MinimumSize = new Size(1, 1);
            b11.Name = "b11";
            b11.RadiusSides = UICornerRadiusSides.None;
            b11.Size = new Size(29, 29);
            b11.Symbol = 0;
            b11.TabIndex = 11;
            b11.Text = "0";
            b11.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b10
            // 
            b10.Cursor = Cursors.Hand;
            b10.Font = new Font("Segoe UI", 12F);
            b10.Location = new Point(369, 3);
            b10.MinimumSize = new Size(1, 1);
            b10.Name = "b10";
            b10.RadiusSides = UICornerRadiusSides.None;
            b10.Size = new Size(29, 29);
            b10.Symbol = 0;
            b10.TabIndex = 10;
            b10.Text = "0";
            b10.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b9
            // 
            b9.Cursor = Cursors.Hand;
            b9.Font = new Font("Segoe UI", 12F);
            b9.Location = new Point(337, 3);
            b9.MinimumSize = new Size(1, 1);
            b9.Name = "b9";
            b9.RadiusSides = UICornerRadiusSides.None;
            b9.Size = new Size(29, 29);
            b9.Symbol = 0;
            b9.TabIndex = 9;
            b9.Text = "0";
            b9.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b8
            // 
            b8.Cursor = Cursors.Hand;
            b8.Font = new Font("Segoe UI", 12F);
            b8.Location = new Point(305, 3);
            b8.MinimumSize = new Size(1, 1);
            b8.Name = "b8";
            b8.RadiusSides = UICornerRadiusSides.None;
            b8.Size = new Size(29, 29);
            b8.Symbol = 0;
            b8.TabIndex = 8;
            b8.Text = "0";
            b8.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // b16
            // 
            b16.Cursor = Cursors.Hand;
            b16.Font = new Font("Segoe UI", 10.5F);
            b16.ImageAlign = ContentAlignment.MiddleRight;
            b16.Location = new Point(561, 3);
            b16.MinimumSize = new Size(1, 1);
            b16.Name = "b16";
            b16.Padding = new Padding(6, 0, 5, 0);
            b16.RadiusSides = UICornerRadiusSides.RightTop | UICornerRadiusSides.RightBottom;
            b16.Size = new Size(75, 29);
            b16.Symbol = 61701;
            b16.TabIndex = 16;
            b16.TagString = ">";
            b16.Text = "下一页";
            b16.TextAlign = ContentAlignment.MiddleLeft;
            b16.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            b16.LocationChanged += b16_LocationChanged;
            // 
            // edtPage
            // 
            edtPage.Cursor = Cursors.IBeam;
            edtPage.DoubleValue = 10D;
            edtPage.Font = new Font("Segoe UI", 12F);
            edtPage.IntValue = 10;
            edtPage.Location = new Point(673, 3);
            edtPage.Margin = new Padding(4, 5, 4, 5);
            edtPage.Minimum = 1D;
            edtPage.MinimumSize = new Size(1, 1);
            edtPage.Name = "edtPage";
            edtPage.Padding = new Padding(5);
            edtPage.ShowText = false;
            edtPage.Size = new Size(53, 29);
            edtPage.TabIndex = 1;
            edtPage.Text = "10";
            edtPage.TextAlignment = ContentAlignment.BottomCenter;
            edtPage.Type = UITextBox.UIEditType.Integer;
            edtPage.Watermark = "";
            // 
            // btnSelect
            // 
            btnSelect.Cursor = Cursors.Hand;
            btnSelect.Font = new Font("Segoe UI", 10.5F);
            btnSelect.Location = new Point(756, 3);
            btnSelect.MinimumSize = new Size(1, 1);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(61, 29);
            btnSelect.Symbol = 0;
            btnSelect.TabIndex = 3;
            btnSelect.Text = "确定";
            btnSelect.TipsFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnSelect.Click += btnSelect_Click;
            // 
            // uiLabel2
            // 
            uiLabel2.AutoSize = true;
            uiLabel2.BackColor = Color.Transparent;
            uiLabel2.Font = new Font("Segoe UI", 10.5F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(726, 10);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(21, 14);
            uiLabel2.TabIndex = 2;
            uiLabel2.Text = "页";
            uiLabel2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // uiLabel1
            // 
            uiLabel1.AutoSize = true;
            uiLabel1.BackColor = Color.Transparent;
            uiLabel1.Font = new Font("Segoe UI", 10.5F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(650, 10);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(21, 14);
            uiLabel1.TabIndex = 0;
            uiLabel1.Text = "第";
            uiLabel1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // UIPagination
            // 
            Controls.Add(edtPage);
            Controls.Add(btnSelect);
            Controls.Add(uiLabel2);
            Controls.Add(b16);
            Controls.Add(uiLabel1);
            Controls.Add(b15);
            Controls.Add(b14);
            Controls.Add(b13);
            Controls.Add(b12);
            Controls.Add(b11);
            Controls.Add(b10);
            Controls.Add(b9);
            Controls.Add(b8);
            Controls.Add(b7);
            Controls.Add(b6);
            Controls.Add(b5);
            Controls.Add(b4);
            Controls.Add(b3);
            Controls.Add(b2);
            Controls.Add(b1);
            Controls.Add(b0);
            Name = "UIPagination";
            RectSides = ToolStripStatusLabelBorderSides.None;
            Size = new Size(1139, 35);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion InitializeComponent

        private void SetShowButton(int buttonIdx, int pageIdx, int activeIdx)
        {
            buttons[buttonIdx].Symbol = 0;
            buttonTags[buttons[buttonIdx]] = pageIdx;
            buttons[buttonIdx].Visible = true;
            buttons[buttonIdx].TagString = "";
            buttons[buttonIdx].Selected = activeIdx == pageIdx;
            SetButtonWidth(buttons[buttonIdx], pageIdx.ToString());
            buttons[buttonIdx].Left = buttons[buttonIdx - 1].Right + buttonInterval - 1;
            if (buttons[buttonIdx].Selected) buttons[buttonIdx].BringToFront();
        }

        private void SetShowButton(int buttonIdx, int addCount, string tagString)
        {
            buttons[buttonIdx].Symbol = 361761;
            buttons[buttonIdx].Text = "";
            buttons[buttonIdx].SymbolOffset = new Point(-1, 1);
            buttonTags[buttons[buttonIdx]] = addCount;
            buttons[buttonIdx].Visible = true;
            buttons[buttonIdx].TagString = tagString;
            buttons[buttonIdx].Selected = false;
            buttons[buttonIdx].Left = buttons[buttonIdx - 1].Right + buttonInterval - 1;
            if (buttons[buttonIdx].Width != 29) buttons[buttonIdx].Width = 29;
        }

        private void SetButtonWidth(UISymbolButton button, string text)
        {
            if (button.Text != text) button.Text = text;
            int len = 29;
            if (button.Text.Length >= 3) len = 36;
            if (button.Text.Length >= 4) len = 44;
            if (button.Text.Length >= 4) len = 52;
            if (button.Width != len) button.Width = len;
        }

        private void SetHideButton(int beginIdx)
        {
            for (var i = beginIdx; i < 16; i++) buttons[i].Visible = false;
        }

        private void SetShowButtons()
        {
            b0.Visible = true;
            b16.Visible = true;

            PageCount = TotalCount.Mod(PageSize) == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;
            edtPage.Maximum = PageCount;

            if (activePage >= PageCount)
            {
                activePage = PageCount;
                b16.Enabled = false;
            }
            else
            {
                b16.Enabled = true;
            }
            if (activePage <= 1)
            {
                activePage = 1;
                b0.Enabled = false;
            }
            else
            {
                b0.Enabled = true;
            }

            b1.Left = b0.Right + buttonInterval - 1;
            edtPage.IntValue = activePage;

            if (TotalCount == 0)
            {
                PageCount = 1;
                activePage = 1;
                SetShowButton(1, 1, 1);
                SetHideButton(2);
                b16.Left = b1.Right + buttonInterval - 1;
                return;
            }

            if (PageCount <= PagerCount + 2)
            {
                for (var i = 1; i <= PageCount; i++) SetShowButton(i, i, activePage);

                b16.Left = buttons[PageCount].Right + buttonInterval - 1;
                SetHideButton(PageCount + 1);
            }
            else
            {
                //左
                var leftShow = PagerCount / 2 + 1 + 2;
                if (activePage <= leftShow)
                {
                    for (var i = 1; i <= leftShow; i++) SetShowButton(i, i, activePage);

                    SetShowButton(leftShow + 1, PagerCount - 2, ">>");
                    SetShowButton(leftShow + 2, PageCount, activePage);
                    SetHideButton(leftShow + 3);
                    b16.Left = buttons[leftShow + 2].Right + buttonInterval - 1;
                    return;
                }

                //右
                var rightShow = PageCount - (PagerCount / 2 + 1) - 1;
                if (activePage >= rightShow)
                {
                    SetShowButton(1, 1, activePage);
                    SetShowButton(2, 2 - PagerCount, "<<");

                    var idx = 3;
                    for (var i = rightShow; i <= PageCount; i++)
                    {
                        SetShowButton(idx, i, activePage);
                        idx++;
                    }

                    b16.Left = buttons[idx - 1].Right + buttonInterval - 1;
                    SetHideButton(idx);
                    return;
                }

                //中
                SetShowButton(1, 1, activePage);
                SetShowButton(2, 2 - PagerCount, "<<");
                var cIdx = 3;
                var sIdx = (PagerCount - 2) / 2;
                for (var i = cIdx; i <= PagerCount; i++)
                {
                    SetShowButton(cIdx, activePage - sIdx + (i - 3), activePage);
                    cIdx++;
                }

                SetShowButton(cIdx, PagerCount - 2, ">>");
                SetShowButton(cIdx + 1, PageCount, activePage);
                b16.Left = buttons[cIdx + 1].Right + buttonInterval - 1;
                SetHideButton(cIdx + 2);
            }
        }

        protected override void AfterSetRectColor(Color color)
        {
            base.AfterSetRectColor(color);

        }

        private void b16_LocationChanged(object sender, EventArgs e)
        {
            uiLabel1.Left = b16.Right + 6;
            Translate();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            ActivePage = edtPage.IntValue;
        }

        private void SetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember)
        {
            var dataSourceChanged = dataSource != newDataSource;

            if (inSetDataConnection) return;

            try
            {
                if (dataSourceChanged)
                {
                    inSetDataConnection = true;
                    CurrencyManager newDataManager = null;
                    if (newDataSource != null && newDataSource != Convert.DBNull)
                        newDataManager = (CurrencyManager)BindingContext[newDataSource, newDisplayMember.BindingPath];

                    dataManager = newDataManager;
                }
            }
            finally
            {
                inSetDataConnection = false;
            }
        }

        public void DataBind()
        {
            if (dataSource == null)
            {
                PageChanged?.Invoke(this, dataSource, activePage, PageSize);
                return;
            }

            var objects = new List<object>();
            var iBegin = PageSize * (ActivePage - 1);
            for (var i = iBegin; i < iBegin + PageSize; i++)
                if (i < TotalCount)
                    objects.Add(dataManager.List[i]);

            PageDataSource = objects;
            PageChanged?.Invoke(this, objects, activePage, objects.Count);
        }

        public event OnPageChangeEventHandler PageChanged;

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            foreach (var button in buttons.Values)
            {
                button.SetStyleColor(uiColor);
                button.FillColor = button.RectColor = uiColor.ButtonFillColor;
                button.SymbolColor = button.ForeColor = uiColor.ButtonForeColor;
                b0.RectSelectedColor = button.FillSelectedColor = uiColor.ButtonFillSelectedColor;
            }

            btnSelect.SetStyleColor(uiColor);
            btnSelect.FillColor = btnSelect.RectColor = uiColor.ButtonFillColor;
            btnSelect.ForeColor = btnSelect.SymbolColor = uiColor.ButtonForeColor;
            edtPage.BackColor = b0.BackColor = b16.BackColor = btnSelect.BackColor = uiColor.PanelFillColor;

            edtPage.SetStyleColor(uiColor);
            edtPage.RectColor = uiColor.ButtonFillColor;
            edtPage.Invalidate();

            uiLabel1.SetStyleColor(uiColor);
            uiLabel2.SetStyleColor(uiColor);
            uiLabel1.ForeColor = uiLabel2.ForeColor = uiColor.PanelForeColor;
        }

        /// <summary>
        /// Fill color, does not fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Button fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonFillColor
        {
            get => b0.FillColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.RectColor = button.FillColor = value;
                    button.Style = UIStyle.Custom;
                }

                edtPage.RectColor = btnSelect.RectColor = btnSelect.FillColor = value;
                btnSelect.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Button font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonForeColor
        {
            get => b0.ForeColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.SymbolColor = button.ForeColor = value;
                    button.Style = UIStyle.Custom;
                }

                btnSelect.SymbolColor = btnSelect.ForeColor = value;
                btnSelect.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Button hover fill color
        /// </summary>
        [DefaultValue(typeof(Color), "115, 179, 255"), Category("SunnyUI")]
        [Description("Button hover fill color")]
        public Color ButtonFillHoverColor
        {
            get => b0.FillHoverColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.RectHoverColor = button.FillHoverColor = value;
                    button.Style = UIStyle.Custom;
                }

                btnSelect.RectHoverColor = btnSelect.FillHoverColor = value;
                btnSelect.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Button hover font color
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Button hover font color")]
        public Color ButtonForeHoverColor
        {
            get => b0.ForeHoverColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.SymbolHoverColor = button.ForeHoverColor = value;
                    button.Style = UIStyle.Custom;
                }

                btnSelect.SymbolHoverColor = btnSelect.ForeHoverColor = value;
                btnSelect.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Button press fill color
        /// </summary>
        [DefaultValue(typeof(Color), "64, 128, 204"), Category("SunnyUI")]
        [Description("Button press fill color")]
        public Color ButtonFillPressColor
        {
            get => b0.FillPressColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.RectPressColor = button.FillPressColor = value;
                    button.Style = UIStyle.Custom;
                }

                btnSelect.RectPressColor = btnSelect.FillPressColor = value;
                btnSelect.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Button press font color
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Button press font color")]
        public Color ButtonForePressColor
        {
            get => b0.ForePressColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.SymbolPressColor = button.ForePressColor = value;
                    button.Style = UIStyle.Custom;
                }

                btnSelect.SymbolPressColor = btnSelect.ForePressColor = value;
                btnSelect.Style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// Button selected fill color
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("Button selected fill color")]
        public Color ButtonFillSelectedColor
        {
            get => b0.FillSelectedColor;
            set
            {
                foreach (var button in buttons.Values)
                {
                    button.RectSelectedColor = button.FillSelectedColor = value;
                    button.Style = UIStyle.Custom;
                }
            }
        }

        /// <summary>
        /// Scroll bar theme style
        /// </summary>
        [DefaultValue(true), Description("Scroll bar theme style"), Category("SunnyUI")]
        public bool ButtonStyleInherited
        {
            get => b0 != null && b0.Style == UIStyle.Inherited;
            set
            {
                if (value && b0 != null)
                {
                    foreach (var button in buttons.Values)
                    {
                        button.Style = UIStyle.Inherited;
                    }

                    btnSelect.Style = UIStyle.Inherited;
                    edtPage.Style = UIStyle.Inherited;
                }
            }
        }
    }
}