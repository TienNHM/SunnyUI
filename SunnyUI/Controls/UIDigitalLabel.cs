﻿/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
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
 * File Name: UIDigitalLabel.cs
 * File Description: Cold LCD display label
 * Current Version: V3.6.1
 * Creation Date: 2023-12-01
 *
 * 2023-12-01: V3.6.1 Added file description
 * 2024-01-23: V3.6.3 Updated drawing
 * 2024-10-22: V3.7.2 Added DPI support
 ******************************************************************************/

/******************************************************************************
 * sa-digital-number.ttf
 * Digital Numbers Fonts is a fixed-width (web) font in a cold LCD display style.
 * Publicly available for free under the SIL Open Font License 1.1.
 * https://github.com/s-a/digital-numbers-font
 * Copyright (c) 2015, Stephan Ahlf (stephan.ahlf@googlemail.com)
 * This Font Software is licensed under the SIL Open Font License, Version 1.1.
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    [ToolboxItem(true)]
    public class UIDigitalLabel : UIControl
    {
        public UIDigitalLabel()
        {
            SetStyleFlags();
            Size = new Size(208, 42);
            TextAlign = HorizontalAlignment.Right;
            ShowText = ShowRect = ShowFill = false;
            ForeColor = Color.Lime;
            BackColor = Color.Black;
        }

        private double digitalValue;

        [Description("Floating point number"), Category("SunnyUI")]
        [DefaultValue(typeof(double), "0")]
        public double Value
        {
            get => digitalValue;
            set
            {
                digitalValue = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        public event EventHandler ValueChanged;

        private int digitalSize = 24;

        [Description("LCD font size"), Category("SunnyUI")]
        [DefaultValue(24)]
        public int DigitalSize
        {
            get => digitalSize;
            set
            {
                digitalSize = Math.Max(9, value);
                Invalidate();
            }
        }

        private int decimalPlaces = 2;

        [Description("Floating point number, number of decimal places to display"), Category("SunnyUI")]
        [DefaultValue(2)]
        public int DecimalPlaces
        {
            get => decimalPlaces;
            set
            {
                decimalPlaces = Math.Max(0, value);
                Invalidate();
            }
        }

        /// <summary>
        /// Override painting
        /// </summary>
        /// <param name="e">Painting parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            using Font font = DigitalFont.Instance.GetFont(UIStyles.DPIScale ? DigitalSize / UIDPIScale.SystemDPIScale : DigitalSize);
            using Brush br = new SolidBrush(ForeColor);

            string text = Value.ToString("F" + DecimalPlaces);
            SizeF sf = e.Graphics.MeasureString(text, font);
            float y = (Height - sf.Height) / 2.0f + 1 + Padding.Top;
            float x = Padding.Left;
            switch (TextAlign)
            {
                case HorizontalAlignment.Right:
                    x = Width - sf.Width - Padding.Right;
                    break;
                case HorizontalAlignment.Center:
                    x = (Width - sf.Width) / 2.0f;
                    break;
            }

            e.Graphics.DrawString(text, font, br, x, y);
        }

        private HorizontalAlignment textAlign = HorizontalAlignment.Right;

        /// <summary>
        /// Text alignment direction
        /// </summary>
        [Description("Text alignment direction"), Category("SunnyUI")]
        [DefaultValue(HorizontalAlignment.Right)]
        public new HorizontalAlignment TextAlign
        {
            get => textAlign;
            set
            {
                if (textAlign != value)
                {
                    textAlign = value;
                    Invalidate();
                }
            }
        }

        private Point textOffset = new Point(0, 0);

        [Description("Text offset"), Category("SunnyUI")]
        [DefaultValue(typeof(Point), "0, 0")]
        public Point TextOffset
        {
            get => textOffset;
            set
            {
                textOffset = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {

        }
    }
}
