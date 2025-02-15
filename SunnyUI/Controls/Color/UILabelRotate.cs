using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [ToolboxItem(false)]
    public class LabelRotate : Control, IStyleInterface, IZoomScale
    {
        private float m_textAngle = 0;
        private ContentAlignment m_rotatePointAlignment = ContentAlignment.MiddleCenter;
        private ContentAlignment m_textAlignment = ContentAlignment.MiddleLeft;

        /// <summary>
        /// Disable control zoom scaling
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control zoom scaling")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Control rectangle before zoom scaling
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle ZoomScaleRect { get; set; }

        /// <summary>
        /// Set control zoom scale
        /// </summary>
        /// <param name="scale">Zoom scale</param>
        public virtual void SetZoomScale(float scale)
        {

        }

        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (DesignMode) return;
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Refresh();
            }
        }

        public float TextAngle
        {
            get { return m_textAngle; }
            set
            {
                m_textAngle = value;
                Invalidate();
            }
        }

        public ContentAlignment TextAlign
        {
            get { return m_textAlignment; }
            set
            {
                m_textAlignment = value;
                Invalidate();
            }
        }

        public ContentAlignment RotatePointAlignment
        {
            get { return m_rotatePointAlignment; }
            set
            {
                m_rotatePointAlignment = value;
                Invalidate();
            }
        }

        private Color m_frameColor = UIColor.Blue;

        public Color FrameColor
        {
            get => m_frameColor;
            set
            {
                m_frameColor = value;
                Invalidate();
            }
        }

        public LabelRotate()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.Text = string.Empty;
            base.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            Version = UIGlobal.Version;
        }

        /// <summary>
        /// Override paint method
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(BackColor, ClientRectangle);

            RectangleF lr = ClientRectangleF;
            UIColorUtil.DrawFrame(e.Graphics, lr, 6, m_frameColor);
            if (Text.Length > 0)
            {
                StringFormat format = new StringFormat();
                string alignment = TextAlign.ToString();

                if (((int)TextAlign & (int)(ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft)) != 0)
                    format.Alignment = StringAlignment.Near;

                if (((int)TextAlign & (int)(ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter)) != 0)
                    format.Alignment = StringAlignment.Center;

                if (((int)TextAlign & (int)(ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight)) != 0)
                    format.Alignment = StringAlignment.Far;

                if (((int)TextAlign & (int)(ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight)) != 0)
                    format.LineAlignment = StringAlignment.Far;

                if (((int)TextAlign & (int)(ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight)) != 0)
                    format.LineAlignment = StringAlignment.Center;

                if (((int)TextAlign & (int)(ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight)) != 0)
                    format.LineAlignment = StringAlignment.Near;

                Rectangle r = ClientRectangle;
                r.X += Padding.Left;
                r.Y += Padding.Top;
                r.Width -= Padding.Right;
                r.Height -= Padding.Bottom;

                using SolidBrush b = new SolidBrush(ForeColor);
                if (TextAngle.EqualsFloat(0))
                {
                    e.Graphics.DrawString(Text, Font, ForeColor, r, TextAlign);
                }
                else
                {
                    PointF center = UIColorUtil.Center(ClientRectangle);
                    switch (RotatePointAlignment)
                    {
                        case ContentAlignment.TopLeft:
                            center.X = r.Left;
                            center.Y = r.Top;
                            break;

                        case ContentAlignment.TopCenter:
                            center.Y = r.Top;
                            break;

                        case ContentAlignment.TopRight:
                            center.X = r.Right;
                            center.Y = r.Top;
                            break;

                        case ContentAlignment.MiddleLeft:
                            center.X = r.Left;
                            break;

                        case ContentAlignment.MiddleCenter:
                            break;

                        case ContentAlignment.MiddleRight:
                            center.X = r.Right;
                            break;

                        case ContentAlignment.BottomLeft:
                            center.X = r.Left;
                            center.Y = r.Bottom;
                            break;

                        case ContentAlignment.BottomCenter:
                            center.Y = r.Bottom;
                            break;

                        case ContentAlignment.BottomRight:
                            center.X = r.Right;
                            center.Y = r.Bottom;
                            break;
                    }

                    center.X += Padding.Left;
                    center.Y += Padding.Top;
                    center.X -= Padding.Right;
                    center.Y -= Padding.Bottom;

                    e.Graphics.TranslateTransform(center.X, center.Y);
                    e.Graphics.RotateTransform(TextAngle);

                    e.Graphics.DrawString(Text, Font, b, new PointF(0, 0), format);
                    e.Graphics.ResetTransform();
                }
            }

            RaisePaintEvent(this, e);
        }

        protected RectangleF ClientRectangleF
        {
            get
            {
                RectangleF r = ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                return r;
            }
        }

        private UIStyle _style = UIStyle.Inherited;

        /// <summary>
        /// Style
        /// </summary>
        [DefaultValue(UIStyle.Inherited), Description("Style"), Category("SunnyUI")]
        public UIStyle Style
        {
            get => _style;
            set => SetStyle(value);
        }

        /// <summary>
        /// Set style
        /// </summary>
        /// <param name="style">Style</param>
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
        /// Set style color
        /// </summary>
        /// <param name="uiColor"></param>
        public void SetStyleColor(UIBaseStyle uiColor)
        {
            FrameColor = uiColor.LabelRotateFrameColor;
            ForeColor = uiColor.LabelRotateForeColor;
        }

        /// <summary>
        /// Custom style mode
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set whether to enable custom style mode"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        public string Version { get; }

        /// <summary>
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the tag string related to the control"), Category("SunnyUI")]
        public string TagString { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}