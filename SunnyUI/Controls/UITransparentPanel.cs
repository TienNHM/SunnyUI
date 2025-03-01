using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(false)]
    internal class UITransparentPanel : UIPanel
    {
        public UITransparentPanel()
        {
            Opacity = 200;
            SetStyleFlags(true, false);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            BackColor = Color.FromArgb(Opacity, FillColor);
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
        }
    }
}