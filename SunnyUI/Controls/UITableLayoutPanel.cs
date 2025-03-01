/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Class Library, Extension Class Library, Multi-page Development Framework.
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
 * File Name: UITableLayoutPanel.cs
 * File Description: Dynamic Layout Panel
 * Current Version: V3.1
 * Creation Date: 2021-07-18
 *
 * 2021-07-18: V3.0.5 Added file description
 * 2021-07-18: V3.0.5 Updated custom color issue for controls placed in TableLayoutPanel
 * 2023-11-12: V3.5.2 Refactored theme
******************************************************************************/

using System.ComponentModel;
using System.Windows.Forms;

namespace Sunny.UI
{
    public sealed class UITableLayoutPanel : TableLayoutPanel, IStyleInterface
    {
        public UITableLayoutPanel()
        {
            Version = UIGlobal.Version;
        }

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

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

        public string Version
        {
            get;
        }

        public string TagString
        {
            get; set;
        }

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
            //
        }
    }
}
