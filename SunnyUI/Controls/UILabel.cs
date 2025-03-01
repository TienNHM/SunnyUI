/******************************************************************************
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
 * File Name: UILabel.cs
 * Description: Label
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-23: V2.2.4 Added UISymbolLabel
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2020-11-12: V3.0.8 Added text rotation angle
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-11-16: V3.5.2 Refactored theme
 * 2024-07-10: V3.6.7 Default text position TopLeft
******************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public class UILabel : Label, IStyleInterface, IZoomScale, IFormTranslator
    {
        public UILabel()
        {
            base.Font = UIStyles.Font();
            Version = UIGlobal.Version;
            ForeColor = UIStyles.Blue.LabelForeColor;
        }

        [Browsable(false)]
        [Description("Array of property names that need multilingual translation when the control is displayed"), Category("SunnyUI")]
        public string[] FormTranslatorProperties => ["Text"];

        [DefaultValue(true)]
        [Description("Whether the control needs multilingual translation when displayed"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Position of the control in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set the scaling ratio of the control
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public virtual void SetZoomScale(float scale)
        {

        }

        private int angle;

        [DefaultValue(0), Category("SunnyUI"), Description("Rotation angle when centered")]
        public int Angle
        {
            get => angle;
            set
            {
                angle = value;
                Invalidate();
            }
        }

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        public string Version { get; }

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

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set whether the theme style can be customized"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        public virtual void SetStyleColor(UIBaseStyle uiColor)
        {
            ForeColor = uiColor.LabelForeColor;
        }

        private UIStyle _style = UIStyle.Inherited;

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
        /// Override painting
        /// </summary>
        /// <param name="e">Painting parameters</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (TextAlign == ContentAlignment.MiddleCenter && Angle != 0 && !AutoSize)
            {
                e.Graphics.DrawRotateString(Text, Font, ForeColor, this.ClientRectangle.Center(), Angle);
            }
            else
            {
                base.OnPaint(e);
            }
        }
    }

    [ToolboxItem(true)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public sealed class UILinkLabel : LinkLabel, IStyleInterface, IZoomScale, IFormTranslator
    {
        public UILinkLabel()
        {
            base.Font = UIStyles.Font();
            LinkBehavior = LinkBehavior.AlwaysUnderline;
            Version = UIGlobal.Version;

            ActiveLinkColor = UIStyles.Blue.MarkLabelForeColor;
            VisitedLinkColor = UIColor.Red;

            base.LinkColor = linkColor = ForeColor = UIStyles.Blue.LabelForeColor;
        }

        [Browsable(false)]
        [Description("Array of property names that need multilingual translation when the control is displayed"), Category("SunnyUI")]
        public string[] FormTranslatorProperties => ["Text"];

        [DefaultValue(true)]
        [Description("Whether the control needs multilingual translation when displayed"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Position of the control in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set the scaling ratio of the control
        /// </summary>
        /// <param name="scale">Scaling ratio</param>
        public void SetZoomScale(float scale)
        {

        }

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        public string Version { get; }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set whether the theme style can be customized"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

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
            linkColor = ForeColor = uiColor.LabelForeColor;
            ActiveLinkColor = uiColor.MarkLabelForeColor;
            base.LinkColor = linkColor;
        }

        private UIStyle _style = UIStyle.Inherited;

        /// <summary>
        /// Theme style
        /// </summary>
        [DefaultValue(UIStyle.Inherited), Description("Theme style"), Category("SunnyUI")]
        public UIStyle Style
        {
            get => _style;
            set => SetStyle(value);
        }

        private Color linkColor;
        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public new Color LinkColor
        {
            get => linkColor;
            set
            {
                if (linkColor != value)
                {
                    linkColor = value;
                    base.LinkColor = value;
                    Invalidate();
                }
            }
        }
    }
}