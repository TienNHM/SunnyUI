/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua(Shen Yonghua).
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIPanel.cs
 * Description: Panel
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2021-05-09: V3.0.3 Added double buffering to reduce flickering
 * 2021-09-03: V3.0.6 Support for background image display
 * 2021-12-11: V3.0.9 Added gradient color
 * 2021-12-13: V3.0.9 Border line width can be set to 1 or 2
 * 2022-01-10: V3.1.0 Adjusted border and corner drawing
 * 2022-01-27: V3.1.0 Disabled scrollbar display
 * 2022-02-16: V3.1.1 Base class added read-only color setting
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-06-10: V3.1.9 Redraw on size change
 * 2024-11-19: V3.7.2 Added opacity
******************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sunny.UI
{
    public partial class UIPanel : UIUserControl
    {
        public UIPanel()
        {
            InitializeComponent();
            base.Font = UIStyles.Font();
            base.MinimumSize = new Size(1, 1);
            showText = true;
            SetStyleFlags(true, false, true);
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["Text"];

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public override Color ForeColor
        {
            get => foreColor;
            set
            {
                foreColor = value;
                AfterSetForeColor(value);
                Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "109, 109, 103")]
        [Description("Font color when disabled"), Category("SunnyUI")]
        public Color ForeDisableColor
        {
            get => foreDisableColor;
            set => SetForeDisableColor(value);
        }

        [Description("Show text"), Category("SunnyUI")]
        [DefaultValue(true)]
        [Browsable(false)]
        public bool ShowText
        {
            get => showText;
            set
            {
                if (showText != value)
                {
                    showText = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Set read-only font color
        /// </summary>
        /// <param name="color">Color</param>
        protected void SetForeReadOnlyColor(Color color)
        {
            foreReadOnlyColor = color;
            AfterSetForeReadOnlyColor(color);
            Invalidate();
        }

        private byte _opacity = 255;
        [Description("Opacity"), Category("SunnyUI")]
        [DefaultValue(typeof(byte), "255")]
        public byte Opacity
        {
            get => _opacity;
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            if (_opacity == 255)
            {
                base.OnPaintFill(g, path);
            }
            else
            {
                var color = GetFillColor().Alpha(_opacity);
                if (RadiusSides == UICornerRadiusSides.None || Radius == 0)
                    g.Clear(color);
                else
                    g.FillPath(color, path);
            }
        }
    }
}