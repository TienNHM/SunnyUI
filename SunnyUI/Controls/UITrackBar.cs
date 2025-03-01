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
 * File Name: UITrackBar.cs
 * File Description: Progress Indicator Bar
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2021-04-11: V3.0.2 Added vertical display mode
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-11-28: V3.6.1 Added a top-to-bottom progress display mode
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    [ToolboxItem(true)]
    public sealed class UITrackBar : UIControl
    {
        public event EventHandler ValueChanged;

        public UITrackBar()
        {
            SetStyleFlags();
            Width = 150;
            Height = 29;

            ShowText = false;
            ShowRect = false;

            rectDisableColor = UIStyles.Blue.TrackDisableColor;
            rectColor = UIStyles.Blue.TrackBarRectColor;
            fillColor = UIStyles.Blue.TrackBarFillColor;
            foreColor = UIStyles.Blue.TrackBarForeColor;
        }

        public enum BarDirection
        {
            /// <summary>
            /// Horizontal
            /// </summary>
            Horizontal,

            /// <summary>
            /// Vertical Up
            /// </summary>
            Vertical,

            /// <summary>
            /// Vertical Down
            /// </summary>
            VerticalDown
        }

        private BarDirection direction = BarDirection.Horizontal;

        [DefaultValue(BarDirection.Horizontal)]
        [Description("Line direction"), Category("SunnyUI")]
        public BarDirection Direction
        {
            get => direction;
            set
            {
                direction = value;
                Invalidate();
            }
        }

        private int _maximum = 100;
        private int _minimum;
        private int trackBarValue;

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly { get; set; }

        [DefaultValue(100)]
        [Description("Maximum value"), Category("SunnyUI")]
        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_maximum <= _minimum)
                    _minimum = _maximum - 1;

                Invalidate();
            }
        }

        [DefaultValue(0)]
        [Description("Minimum value"), Category("SunnyUI")]
        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_minimum >= _maximum)
                    _maximum = _minimum + 1;

                Invalidate();
            }
        }

        [DefaultValue(0)]
        [Description("Current value"), Category("SunnyUI")]
        public int Value
        {
            get => trackBarValue;
            set
            {
                int v = Math.Min(Math.Max(Minimum, value), Maximum);
                if (trackBarValue != v)
                {
                    trackBarValue = v;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            rectDisableColor = uiColor.TrackDisableColor;
            rectColor = uiColor.TrackBarRectColor;
            fillColor = uiColor.TrackBarFillColor;
            foreColor = uiColor.TrackBarForeColor;
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            g.Clear(fillColor);

            if (Direction == BarDirection.Horizontal)
            {
                g.FillRoundRectangle(rectDisableColor,
                    new Rectangle(5, Height / 2 - 3, Width - 1 - 10, 6), 6);

                int len = (int)((Value - Minimum) * 1.0 * (Width - 1 - 10) / (Maximum - Minimum));
                if (len > 0)
                {
                    g.FillRoundRectangle(foreColor, new Rectangle(5, Height / 2 - 3, len, 6), 6);
                }

                g.FillRoundRectangle(fillColor.IsValid() ? fillColor : Color.White,
                    new Rectangle(len, (Height - BarSize) / 2, 10, BarSize), 5);

                using Pen pen = new Pen(rectColor, 2);
                g.SetHighQuality();
                g.DrawRoundRectangle(pen, new Rectangle(len + 1, (Height - BarSize) / 2 + 1, 8, BarSize - 2), 5);
                g.SetDefaultQuality();
            }

            if (Direction == BarDirection.Vertical)
            {
                g.FillRoundRectangle(rectDisableColor, new Rectangle(Width / 2 - 3, 5, 6, Height - 1 - 10), 6);

                int len = (int)((Value - Minimum) * 1.0 * (Height - 1 - 10) / (Maximum - Minimum));
                if (len > 0)
                {
                    g.FillRoundRectangle(foreColor, new Rectangle(Width / 2 - 3, Height - len - 5, 6, len), 6);
                }

                g.FillRoundRectangle(fillColor.IsValid() ? fillColor : Color.White, new Rectangle((Width - BarSize) / 2, Height - len - 10 - 1, BarSize, 10), 5);

                using Pen pen = new Pen(rectColor, 2);
                g.SetHighQuality();
                g.DrawRoundRectangle(pen, new Rectangle((Width - BarSize) / 2 + 1, Height - len - 10, BarSize - 2, 8), 5);
                g.SetDefaultQuality();
            }

            if (Direction == BarDirection.VerticalDown)
            {
                g.FillRoundRectangle(rectDisableColor, new Rectangle(Width / 2 - 3, 5, 6, Height - 10), 6);

                int len = (int)((Value - Minimum) * 1.0 * (Height - 1 - 10) / (Maximum - Minimum));
                if (len > 0)
                {
                    g.FillRoundRectangle(foreColor, new Rectangle(Width / 2 - 3, 5, 6, len), 6);
                }

                g.FillRoundRectangle(fillColor.IsValid() ? fillColor : Color.White, new Rectangle((Width - BarSize) / 2, len, BarSize, 10), 5);

                using Pen pen = new Pen(rectColor, 2);
                g.SetHighQuality();
                g.DrawRoundRectangle(pen, new Rectangle((Width - BarSize) / 2 + 1, len + 1, BarSize - 2, 8), 5);
                g.SetDefaultQuality();
            }
        }

        private int trackBarSize = 20;

        [DefaultValue(20)]
        [Description("Button size"), Category("SunnyUI")]
        public int BarSize
        {
            get => trackBarSize;
            set
            {
                trackBarSize = Math.Max(12, value);
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!ReadOnly)
            {
                if (Direction == BarDirection.Horizontal)
                {
                    int len = e.X - 5;
                    int value = (len * 1.0 * (Maximum - Minimum) / (Width - 10)).RoundEx() + Minimum;
                    Value = Math.Min(Math.Max(Minimum, value), Maximum);
                }

                if (Direction == BarDirection.Vertical)
                {
                    int len = Height - 10 - e.Y;
                    int value = (len * 1.0 * (Maximum - Minimum) / (Height - 10)).RoundEx() + Minimum;
                    Value = Math.Min(Math.Max(Minimum, value), Maximum);
                }

                if (Direction == BarDirection.VerticalDown)
                {
                    int len = e.Y - 5;
                    int value = (len * 1.0 * (Maximum - Minimum) / (Height - 10)).RoundEx() + Minimum;
                    Value = Math.Min(Math.Max(Minimum, value), Maximum);
                }
            }
        }

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsPress && !ReadOnly)
            {
                if (Direction == BarDirection.Horizontal)
                {
                    int len = e.X - 5;
                    int value = (len * 1.0 * (Maximum - Minimum) / (Width - 10)).RoundEx() + Minimum;
                    Value = Math.Min(Math.Max(Minimum, value), Maximum);
                }

                if (Direction == BarDirection.Vertical)
                {
                    int len = Height - 10 - e.Y;
                    int value = (len * 1.0 * (Maximum - Minimum) / (Height - 10)).RoundEx() + Minimum;
                    Value = Math.Min(Math.Max(Minimum, value), Maximum);
                }

                if (Direction == BarDirection.VerticalDown)
                {
                    int len = e.Y - 5;
                    int value = (len * 1.0 * (Maximum - Minimum) / (Height - 10)).RoundEx() + Minimum;
                    Value = Math.Min(Math.Max(Minimum, value), Maximum);
                }
            }
        }

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            IsPress = true;
            Invalidate();
        }

        /// <summary>
        /// Override mouse up event
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            IsPress = false;
            Invalidate();
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or null, do not fill
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RectColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        [DefaultValue(typeof(Color), "Silver")]
        [Description("Disabled color"), Category("SunnyUI")]
        public Color DisableColor
        {
            get => rectDisableColor;
            set => SetRectDisableColor(value);
        }
    }
}