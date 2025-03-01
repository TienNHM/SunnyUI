/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, Multi-page Development Framework.
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
 * File Name: UIBattery.cs
 * Description: Battery Power Icon
 * Current Version: V3.1
 * Creation Date: 2020-06-04
 *
 * 2020-06-04: V2.2.5 Added file
 * 2021-06-18: V3.0.4 Modified to allow custom background color
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-11-16: V3.5.2 Refactored theme
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sunny.UI
{
    /// <summary>
    /// Battery Power Icon
    /// </summary>
    [DefaultProperty("Power")]
    [ToolboxItem(true)]
    public sealed class UIBattery : UIControl
    {
        private Color colorDanger = UIColor.Orange;

        private Color colorEmpty = UIColor.Red;

        private Color colorSafe = UIColor.Green;

        private bool multiColor = true;

        private int power = 100;

        private int symbolSize = 36;

        /// <summary>
        /// Constructor
        /// </summary>
        public UIBattery()
        {
            SetStyleFlags(true, false);
            ShowRect = false;
            Width = 48;
            Height = 24;
            fillColor = UIStyles.Blue.BatteryFillColor;
        }

        /// <summary>
        /// Power
        /// </summary>
        [DefaultValue(100), Description("Power"), Category("SunnyUI")]
        public int Power
        {
            get => power;
            set
            {
                value = Math.Min(100, Math.Max(0, value));
                power = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Font Icon Size
        /// </summary>
        [DefaultValue(36), Description("Font Icon Size"), Category("SunnyUI")]
        public int SymbolSize
        {
            get => symbolSize;
            set
            {
                symbolSize = Math.Max(value, 16);
                symbolSize = Math.Min(value, 128);
                Invalidate();
            }
        }

        /// <summary>
        /// Multi-color display for power levels
        /// </summary>
        [DefaultValue(true), Description("Multi-color display for power levels"), Category("SunnyUI")]
        public bool MultiColor
        {
            get => multiColor;
            set
            {
                multiColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Color when power is empty
        /// </summary>
        [DefaultValue(typeof(Color), "230, 80, 80"), Description("Color when power is empty"), Category("SunnyUI")]
        public Color ColorEmpty
        {
            get => colorEmpty;
            set
            {
                colorEmpty = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Color when power is low
        /// </summary>
        [DefaultValue(typeof(Color), "220, 155, 40"), Description("Color when power is low"), Category("SunnyUI")]
        public Color ColorDanger
        {
            get => colorDanger;
            set
            {
                colorDanger = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Color when power is safe
        /// </summary>
        [DefaultValue(typeof(Color), "110, 190, 40"), Description("Color when power is safe"), Category("SunnyUI")]
        public Color ColorSafe
        {
            get => colorSafe;
            set
            {
                colorSafe = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Fill color, no fill if the value is background color, transparent color, or null
        /// </summary>
        [Description("Fill color, no fill if the value is background color, transparent color, or null")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            fillColor = uiColor.BatteryFillColor;
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            g.FillPath(fillColor, path);
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            var fa_battery_empty = 0xf244;
            var fa_battery_quarter = 0xf243;
            var fa_battery_half = 0xf242;
            var fa_battery_three_quarters = 0xf241;
            var fa_battery_full = 0xf240;

            int ShowSymbol;
            var color = GetForeColor();
            if (Power > 90)
            {
                ShowSymbol = fa_battery_full;
                if (multiColor) color = ColorSafe;
            }
            else if (Power > 62.5)
            {
                ShowSymbol = fa_battery_three_quarters;
                if (multiColor) color = ColorSafe;
            }
            else if (Power > 37.5)
            {
                ShowSymbol = fa_battery_half;
                if (multiColor) color = ColorSafe;
            }
            else if (Power > 10)
            {
                ShowSymbol = fa_battery_quarter;
                if (multiColor) color = ColorDanger;
            }
            else
            {
                ShowSymbol = fa_battery_empty;
                if (multiColor) color = ColorEmpty;
            }

            g.DrawFontImage(ShowSymbol, SymbolSize, color, new Rectangle(0, 0, Width, Height));
        }
    }
}