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
 * File Name: UIToolTip.cs
 * File Description: Tooltip
 * Current Version: V3.1
 * Creation Date: 2020-07-21
 *
 * 2020-07-21: V2.2.6 Added control
 * 2020-07-25: V2.2.6 Updated drawing
 * 2021-08-16: V3.0.6 Added ToolTip interface, solved the issue where composite controls like UITextBox could not display ToolTip
 * 2021-12-09: V3.0.9 Fixed default display
 * 2023-05-14: V3.3.6 Refactored DrawString function
 * 2023-10-26: V3.5.1 Added rotation angle parameter SymbolRotate for font icons
******************************************************************************/

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ProvideProperty("ToolTip", typeof(Control))]
    [DefaultEvent("Popup")]
    [ToolboxItemFilter("System.Windows.Forms")]
    public class UIToolTip : ToolTip
    {
        private readonly ConcurrentDictionary<Control, ToolTipControl> ToolTipControls =
            new ConcurrentDictionary<Control, ToolTipControl>();

        public UIToolTip()
        {
            InitOwnerDraw();
        }

        public UIToolTip(IContainer cont) : base(cont)
        {
            InitOwnerDraw();
        }

        [DefaultValue(typeof(Font), "Segoe UI, 9pt"), Description("Font"), Category("SunnyUI")]
        public Font Font { get; set; } = new Font("Segoe UI", 9);

        [DefaultValue(typeof(Font), "Segoe UI, 12pt"), Description("Title font"), Category("SunnyUI")]
        public Font TitleFont { get; set; } = new Font("Segoe UI", 12);

        [DefaultValue(typeof(Color), "239, 239, 239"), Description("Border color"), Category("SunnyUI")]
        public Color RectColor { get; set; } = UIChartStyles.Dark.ForeColor;

        [DefaultValue(true), Description("Auto size"), Category("SunnyUI")]
        public bool AutoSize { get; set; } = true;

        [DefaultValue(typeof(Size), "100, 70"), Description("Size when not auto-scaling"), Category("SunnyUI")]
        public Size Size { get; set; } = new Size(100, 70);

        public new void SetToolTip(Control control, string caption)
        {
            base.SetToolTip(control, caption);
            if (control is IToolTip toolTip)
            {
                base.SetToolTip(toolTip.ExToolTipControl(), caption);
            }
        }

        public void SetToolTip(Control control, string caption, string title, int symbol, int symbolSize,
            Color symbolColor, int symbolRotate = 0)
        {
            if (title == null) title = string.Empty;

            if (ToolTipControls.ContainsKey(control))
            {
                ToolTipControls[control].Title = title;
                ToolTipControls[control].ToolTipText = caption;
                ToolTipControls[control].Symbol = symbol;
                ToolTipControls[control].SymbolSize = symbolSize;
                ToolTipControls[control].SymbolColor = symbolColor;
                ToolTipControls[control].SymbolRotate = symbolRotate;
            }
            else
            {
                var ctrl = new ToolTipControl()
                {
                    Control = control,
                    Title = title,
                    ToolTipText = caption,
                    Symbol = symbol,
                    SymbolSize = symbolSize,
                    SymbolColor = symbolColor,
                    SymbolRotate = symbolRotate
                };

                ToolTipControls.TryAdd(control, ctrl);
            }

            if (control is IToolTip toolTip)
            {
                SetToolTip(toolTip.ExToolTipControl(), caption, title, symbol, symbolSize, symbolColor, symbolRotate);
            }

            base.SetToolTip(control, caption);
        }

        public void SetToolTip(Control control, string caption, string title)
        {
            if (title == null) title = string.Empty;

            if (ToolTipControls.ContainsKey(control))
            {
                ToolTipControls[control].Title = title;
                ToolTipControls[control].ToolTipText = caption;
            }
            else
            {
                var ctrl = new ToolTipControl()
                {
                    Control = control,
                    Title = title,
                    ToolTipText = caption
                };

                ToolTipControls.TryAdd(control, ctrl);
            }

            if (control is IToolTip toolTip)
            {
                SetToolTip(toolTip.ExToolTipControl(), caption, title);
            }

            base.SetToolTip(control, caption);
        }

        public void RemoveToolTip(Control control)
        {
            if (ToolTipControls.ContainsKey(control))
            {
                ToolTipControls.TryRemove(control, out _);
            }

            if (control is IToolTip toolTip)
            {
                RemoveToolTip(toolTip.ExToolTipControl());
            }
        }

        private void InitOwnerDraw()
        {
            OwnerDraw = true;
            Draw += ToolTipExDraw;
            Popup += UIToolTip_Popup;

            BackColor = UIChartStyles.Dark.BackColor;
            ForeColor = UIChartStyles.Dark.ForeColor;
            RectColor = UIChartStyles.Dark.ForeColor;
        }

        private void UIToolTip_Popup(object sender, PopupEventArgs e)
        {
            using var TempFont = Font.DPIScaleFont(Font.Size);
            using var TempTitleFont = TitleFont.DPIScaleFont(TitleFont.Size);

            if (ToolTipControls.ContainsKey(e.AssociatedControl))
            {
                var tooltip = ToolTipControls[e.AssociatedControl];

                if (tooltip.ToolTipText.IsValid())
                {
                    if (!AutoSize)
                    {
                        e.ToolTipSize = Size;
                    }
                    else
                    {
                        int symbolWidth = tooltip.Symbol > 0 ? tooltip.SymbolSize : 0;
                        Size titleSize = new Size(0, 0);
                        if (tooltip.Title.IsValid())
                        {
                            titleSize = TextRenderer.MeasureText(tooltip.Title, TempTitleFont);
                        }

                        Size textSize = TextRenderer.MeasureText(tooltip.ToolTipText, TempFont);
                        int allWidth = Math.Max(textSize.Width, titleSize.Width) + 10;
                        if (symbolWidth > 0) allWidth = allWidth + symbolWidth + 5;
                        int allHeight = titleSize.Height > 0 ? titleSize.Height + textSize.Height + 15 : textSize.Height + 10;
                        e.ToolTipSize = new Size(allWidth, allHeight);
                    }
                }
            }
            else
            {
                Size sf = TextRenderer.MeasureText(GetToolTip(e.AssociatedControl), TempFont);
                e.ToolTipSize = sf.Add(10, 10);
            }
        }

        private void ToolTipExDraw(object sender, DrawToolTipEventArgs e)
        {
            var bounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);
            e.Graphics.FillRectangle(BackColor, bounds);
            e.Graphics.DrawRectangle(RectColor, bounds);
            using var TempFont = Font.DPIScaleFont(Font.Size);
            using var TempTitleFont = TitleFont.DPIScaleFont(TitleFont.Size);

            if (ToolTipControls.ContainsKey(e.AssociatedControl))
            {
                var tooltip = ToolTipControls[e.AssociatedControl];
                if (tooltip.Symbol > 0)
                {
                    e.Graphics.DrawFontImage(tooltip.Symbol, tooltip.SymbolSize, tooltip.SymbolColor, new Rectangle(5, 5, tooltip.SymbolSize, tooltip.SymbolSize));
                }

                int symbolWidth = tooltip.Symbol > 0 ? tooltip.SymbolSize : 0;
                SizeF titleSize = new SizeF(0, 0);
                if (tooltip.Title.IsValid())
                {
                    if (tooltip.Title.IsValid())
                    {
                        titleSize = TextRenderer.MeasureText(tooltip.Title, TempTitleFont);
                    }

                    e.Graphics.DrawString(tooltip.Title, TempTitleFont, ForeColor, new Rectangle(tooltip.Symbol > 0 ? tooltip.SymbolSize + 5 : 5, 5, bounds.Width, bounds.Height), ContentAlignment.TopLeft);
                }

                if (titleSize.Height > 0)
                {
                    e.Graphics.DrawLine(ForeColor, symbolWidth == 0 ? 5 : symbolWidth + 5, 5 + titleSize.Height + 3,
                        e.Bounds.Width - 5, 5 + titleSize.Height + 3);
                }

                e.Graphics.DrawString(e.ToolTipText, TempFont, ForeColor, new Rectangle(tooltip.Symbol > 0 ? tooltip.SymbolSize + 5 : 5, titleSize.Height > 0 ? 10 + (int)titleSize.Height : 5, bounds.Width, bounds.Height), ContentAlignment.TopLeft);
            }
            else
            {
                e.Graphics.DrawString(e.ToolTipText, TempFont, ForeColor, new Rectangle(5, 5, bounds.Width, bounds.Height), ContentAlignment.TopLeft);
            }
        }

        public class ToolTipControl : ISymbol
        {
            public Control Control { get; set; }
            public string Title { get; set; }
            public string ToolTipText { get; set; }

            /// <summary>
            /// Font icon
            /// </summary>
            public int Symbol { get; set; }

            /// <summary>
            /// Font icon size
            /// </summary>
            public int SymbolSize { get; set; } = 32;

            /// <summary>
            /// Font icon offset position
            /// </summary>
            public Point SymbolOffset { get; set; } = new Point(0, 0);

            /// <summary>
            /// Font icon rotation angle
            /// </summary>
            public int SymbolRotate { get; set; } = 0;

            /// <summary>
            /// Font icon color
            /// </summary>
            public Color SymbolColor { get; set; } = UIChartStyles.Dark.ForeColor;
        }
    }
}