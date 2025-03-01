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
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIRoundMeter.cs
 * File Description: Circular Chart
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
******************************************************************************/

using Sunny.UI.Properties;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// UIRoundMeter
    /// </summary>
    [ToolboxItem(true)]
    [DefaultProperty("Angle")]
    public sealed class UIRoundMeter : UIControl
    {
        /// <summary>
        /// Enum TMeterType
        /// </summary>
        public enum TMeterType
        {
            /// <summary>
            /// The custom
            /// </summary>
            Custom,

            /// <summary>
            /// The GPS
            /// </summary>
            Gps,

            /// <summary>
            /// The wind
            /// </summary>
            Wind
        }

        /// <summary>
        /// Enum TRunType
        /// </summary>
        public enum TRunType
        {
            /// <summary>
            /// The clock wise
            /// </summary>
            ClockWise,

            /// <summary>
            /// The anti clock wise
            /// </summary>
            AntiClockWise
        }

        private double _angle;
        private Image _angleImage;
        private Image _backImage;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private IContainer components;

        private TMeterType _meterType;
        private TRunType _runType;

        /// <summary>
        /// Constructor
        /// </summary>
        public UIRoundMeter()
        {
            InitializeComponent();
            SetStyleFlags(true, false);
            MeterType = TMeterType.Gps;
            _runType = TRunType.ClockWise;

            Width = 150;
            Height = 150;
        }

        /// <summary>
        /// Rotation Type
        /// </summary>
        [DefaultValue(TRunType.ClockWise)]
        [Description("Rotation Type"), Category("SunnyUI")]
        public TRunType RunType
        {
            get => _runType;
            set
            {
                if (_runType == value)
                {
                    return;
                }

                _runType = value;
                Invalidate();
            }
        }

        [DefaultValue(TMeterType.Gps)]
        [Description("Display Type"), Category("SunnyUI")]
        public TMeterType MeterType
        {
            get => _meterType;
            set
            {
                if (_meterType == value)
                {
                    return;
                }

                _meterType = value;

                if (value == TMeterType.Gps)
                {
                    BackgroundImage = Resources.gps1;
                    AngleImage = Resources.gps_postion;
                }

                if (value == TMeterType.Wind)
                {
                    BackgroundImage = Resources.wind;
                    AngleImage = Resources.wind_postion;
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Background Image
        /// </summary>
        [Description("Background Image"), Category("SunnyUI")]
        public new Image BackgroundImage
        {
            get => _backImage;
            set
            {
                if (value == null)
                {
                    _backImage = null;
                    Width = 150;
                    Height = 150;
                }
                else
                {
                    _backImage = new Bitmap(value);
                    Width = _backImage.Width;
                    Height = _backImage.Height;
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Arrow Image
        /// </summary>
        [Description("Arrow Image"), Category("SunnyUI")]
        public Image AngleImage
        {
            get => _angleImage;
            set
            {
                _angleImage = value == null ? null : new Bitmap(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Angle
        /// </summary>
        [DefaultValue(0D)]
        [Description("Angle"), Category("SunnyUI")]
        public double Angle
        {
            get => _angle;
            set
            {
                if (_angle.EqualsDouble(value))
                {
                    return;
                }

                _angle = value;
                Invalidate();
            }
        }

        /// <summary>
        /// BackgroundImageLayout
        /// </summary>
        [Browsable(false)]
        public new ImageLayout BackgroundImageLayout { get; set; }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Override Paint
        /// </summary>
        /// <param name="e">Paint Event Args</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_backImage != null)
            {
                e.Graphics.DrawImage(_backImage, Width / 2.0f - _backImage.Width / 2.0f, Height / 2.0f - _backImage.Height / 2.0f);
            }

            if (_angleImage == null)
            {
                return;
            }

            var rawImage = new Bitmap(_angleImage);
            if (_runType == TRunType.ClockWise)
            {
                var bmp = rawImage.Rotate((float)_angle, Color.Transparent);
                e.Graphics.DrawImage(bmp, Width / 2.0f - bmp.Width / 2.0f, Height / 2.0f - bmp.Height / 2.0f);
                bmp.Dispose();
            }

            if (_runType == TRunType.AntiClockWise)
            {
                var bmp = rawImage.Rotate((float)(360 - _angle), Color.Transparent);
                e.Graphics.DrawImage(bmp, Width / 2.0f - bmp.Width / 2.0f, Height / 2.0f - bmp.Height / 2.0f);
                bmp.Dispose();
            }

            rawImage.Dispose();
        }

        #region Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
        }

        #endregion Designer generated code
    }
}