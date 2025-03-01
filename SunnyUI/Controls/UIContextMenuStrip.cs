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
 * File Name: UIContextMenuStrip.cs
 * Description: Context menu
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-04-25: V2.2.4 Updated theme configuration class
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-10-17: V3.5.1 Corrected text vertical alignment
 * 2023-10-17: V3.5.1 When the right-click menu is not bound to ImageList and ImageIndex>0, bind ImageIndex to Symbol for drawing
 * 2024-02-21: V3.6.3 Fixed the display position of shortcut key text
 * 2024-02-22: V3.6.3 Do not redraw when node AutoSize, consider color display when Enabled is False during redraw
******************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public sealed class UIContextMenuStrip : ContextMenuStrip, IStyleInterface, IZoomScale
    {
        private ContextMenuColorTable ColorTable = new ContextMenuColorTable();

        public UIContextMenuStrip()
        {
            Font = UIStyles.Font();
            //RenderMode = ToolStripRenderMode.Custom;
            Renderer = new UIToolStripRenderer(ColorTable);
            Version = UIGlobal.Version;

            ColorTable.SetStyleColor(UIStyles.Blue);
            BackColor = UIStyles.Blue.ContextMenuColor;
        }

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Control position in its container before scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set control scaling ratio
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

        protected override void OnOpening(CancelEventArgs e)
        {
            base.OnOpening(e);
            SetDPIScale();
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

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
            ColorTable.SetStyleColor(uiColor);
            BackColor = uiColor.ContextMenuColor;
            ForeColor = uiColor.ContextMenuForeColor;
        }

        public string Version { get; }

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

        public void CalcHeight()
        {
            if (Items.Count > 0 && !AutoSize)
            {
                int height = 0;
                foreach (ToolStripItem item in Items)
                {
                    height += item.Height;
                }

                Height = height + 4;
            }
        }
    }

    internal class UIToolStripRenderer : ToolStripProfessionalRenderer
    {
        public UIToolStripRenderer() { }

        public UIToolStripRenderer(ProfessionalColorTable professionalColorTable) : base(professionalColorTable) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item.AutoSize)
            {
                base.OnRenderItemText(e);
                return;
            }

            // Adjust the position and size of the text area to achieve vertical centering
            Rectangle textRect = new Rectangle(e.TextRectangle.Left, e.Item.ContentRectangle.Top, e.TextRectangle.Width, e.Item.ContentRectangle.Height);
            ToolStripMenuItem stripItem = (ToolStripMenuItem)e.Item;
            Rectangle backRect = new Rectangle(e.Item.Bounds.Left + 2, e.Item.Bounds.Top - 2, e.Item.Bounds.Width - 4, e.Item.Bounds.Height);

            if (e.Item.Enabled)
            {
                if (e.Item.Selected)
                {
                    e.Graphics.FillRectangle(ColorTable.MenuItemSelected, backRect);
                }
                else
                {
                    e.Graphics.FillRectangle(ColorTable.ImageMarginGradientBegin, backRect);
                }
            }
            else
            {
                e.Graphics.FillRectangle(ColorTable.ImageMarginGradientBegin, backRect);
            }

            Color textColor = e.TextColor;
            if (!e.Item.Enabled) textColor = Color.Gray;
            // Set text drawing format
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.Left;
            TextRenderer.DrawText(e.Graphics, e.Item.Text, e.TextFont, textRect, textColor, flags);

            if (stripItem.ShowShortcutKeys)
            {
                flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.Right;
                TextRenderer.DrawText(e.Graphics, ShortcutToText(stripItem.ShortcutKeys, stripItem.ShortcutKeyDisplayString), e.TextFont, textRect, textColor, flags);
            }

            // When the right-click menu is not bound to ImageList and ImageIndex>0, bind ImageIndex to Symbol for drawing
            ToolStripItem item = e.Item;
            while (!(item.Owner is ContextMenuStrip))
            {
                if (item.Owner is ToolStripDropDownMenu)
                    item = item.OwnerItem;
            }

            ContextMenuStrip ownerContextMenu = item.Owner as ContextMenuStrip;
            if (ownerContextMenu.ImageList != null) return;
            if (e.Item.ImageIndex <= 0) return;
            if (e.Item.Image != null) return;
            Rectangle imageRect = new Rectangle(0, e.Item.ContentRectangle.Top, e.TextRectangle.Left, e.Item.ContentRectangle.Height);
            e.Graphics.DrawFontImage(e.Item.ImageIndex, 24, e.TextColor, imageRect);
        }

        internal static string ShortcutToText(Keys shortcutKeys, string shortcutKeyDisplayString)
        {
            if (!string.IsNullOrEmpty(shortcutKeyDisplayString))
            {
                return shortcutKeyDisplayString;
            }

            if (shortcutKeys == Keys.None)
            {
                return string.Empty;
            }

            //KeysConverter kc = new KeysConverter();
            //kc.ConvertToString(item1.ShortcutKeys).WriteConsole();

            return TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(shortcutKeys);
        }
    }

    internal class ContextMenuColorTable : ProfessionalColorTable
    {
        private UIBaseStyle StyleColor = UIStyles.GetStyleColor(UIStyle.Blue);

        public void SetStyleColor(UIBaseStyle color)
        {
            StyleColor = color;
        }

        public override Color MenuItemSelected => StyleColor.ContextMenuSelectedColor;

        public override Color MenuItemPressedGradientBegin => StyleColor.ButtonFillPressColor;

        public override Color MenuItemPressedGradientMiddle => StyleColor.ButtonFillPressColor;

        public override Color MenuItemPressedGradientEnd => StyleColor.ButtonFillPressColor;

        public override Color MenuBorder => StyleColor.ButtonRectColor;

        public override Color MenuItemBorder => StyleColor.PrimaryColor;

        public override Color ImageMarginGradientBegin => StyleColor.ContextMenuColor;

        public override Color ImageMarginGradientEnd => StyleColor.ContextMenuColor;

        public override Color ImageMarginGradientMiddle => StyleColor.ContextMenuColor;
    }
}