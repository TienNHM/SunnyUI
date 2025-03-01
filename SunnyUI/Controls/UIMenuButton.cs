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
 * File Name: UIMenuButton.cs
 * Description: Dropdown menu button
 * Current Version: V3.8
 * Creation Date: 2024-12-16
 *
 * 2024-12-16: V3.8.0 Added file description
 * 2025-01-15: V3.8.1 Changed property description
 ******************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public class UIMenuButton : UISymbolButton
    {
        private bool _showDropArrow = true;

        [Description("The shortcut menu displayed when the user left-clicks the button"), Category("SunnyUI")]
        public UIContextMenuStrip Menu { get; set; }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                Menu?.Show(this, 0, Height);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ShowDropArrow)
            {
                e.Graphics.DrawFontImage(61703, SymbolSize, GetForeColor(), new Rectangle(Width - SymbolSize - 4, 0, SymbolSize, Height));
            }
        }

        /// <summary>
        /// Font icon
        /// </summary>
        [DefaultValue(true)]
        [Description("Show dropdown button"), Category("SunnyUI")]
        public bool ShowDropArrow
        {
            get => _showDropArrow;
            set
            {
                _showDropArrow = value;
                Invalidate();
            }
        }
    }
}
