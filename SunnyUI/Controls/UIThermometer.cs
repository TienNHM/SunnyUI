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
 * File Name: UIThermometer.cs
 * Description: Thermometer
 * Current Version: V3.6.1
 * Creation Date: 2023-11-30
 *
 * 2023-11-30: V3.6.1 Added file description
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sunny.UI
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Value")]
    [ToolboxItem(true)]
    public class UIThermometer : UIControl
    {
        public event EventHandler ValueChanged;

        public UIThermometer()
        {
            SetStyleFlags();
            Width = 32;
            Height = 150;

            ShowText = false;
            ShowRect = false;

            rectDisableColor = UIStyles.Blue.TrackDisableColor;
            fillColor = UIStyles.Blue.TrackBarFillColor;
            rectColor = UIStyles.Blue.TrackBarForeColor;
        }

        private int _maximum = 100;
        private int _minimum;
        private int thermometerValue;

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
            get => thermometerValue;
            set
            {
                int v = Math.Min(Math.Max(Minimum, value), Maximum);
                if (thermometerValue != v)
                {
                    thermometerValue = v;
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
            fillColor = uiColor.TrackBarFillColor;
            rectColor = uiColor.TrackBarForeColor;
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Drawing path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            g.Clear(fillColor);
            g.FillRoundRectangle(rectDisableColor, new Rectangle(Width / 2 - LineSize / 2, 5, LineSize, Height - 1 - 10), LineSize);

            int len = (int)((Value - Minimum) * 1.0 * (Height - 1 - 5 - BallSize) / (Maximum - Minimum));
            if (len > 0)
            {
                g.FillRoundRectangle(rectColor, new Rectangle(Width / 2 - LineSize / 2, Height - len - ballSize, LineSize, len), LineSize);
            }

            g.FillEllipse(rectColor, new Rectangle(Width / 2 - BallSize / 2, Height - BallSize - 1, BallSize, BallSize));
            g.FillRectangle(rectColor, new Rectangle(Width / 2 - LineSize / 2, Height - len - ballSize + LineSize / 2, LineSize, len + 2), true);

        }

        private int lineSize = 6;

        [DefaultValue(6)]
        [Description("Thermometer tube size"), Category("SunnyUI")]
        public int LineSize
        {
            get => lineSize;
            set
            {
                lineSize = Math.Max(6, value);
                Invalidate();
            }
        }

        private int ballSize = 20;

        [DefaultValue(20)]
        [Description("Thermometer bulb size"), Category("SunnyUI")]
        public int BallSize
        {
            get => ballSize;
            set
            {
                ballSize = Math.Max(16, value);
                Invalidate();
            }
        }

        /// <summary>/// Fill color, if the value is background color or transparent color or null, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Thermometer color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ThermometerColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        [DefaultValue(typeof(Color), "Silver")]
        [Description("Color when disabled"), Category("SunnyUI")]
        public Color DisableColor
        {
            get => rectDisableColor;
            set => SetRectDisableColor(value);
        }
    }
}
