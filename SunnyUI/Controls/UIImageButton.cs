/******************************************************************************
 * SunnyUI open source control library, utility library, extension library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * If you use this code, please retain this note.
 ******************************************************************************
 * File Name: UIImageButton.cs
 * Description: Image Button
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-05-04: V3.3.6 Added PerformClick method to trigger click event
 * 2023-05-13: V3.3.6 Refactored DrawString function
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Image Button
    /// </summary>
    public sealed class UIImageButton : PictureBox, IStyleInterface, IZoomScale, IFormTranslator
    {
        private bool IsPress;
        private bool IsHover;

        private Image imageDisabled;
        private Image imagePress;
        private Image imageHover;
        private Image imageSelected;
        private bool selected;
        private string text;
        private ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        private Color foreColor = UIFontColor.Primary;

        private bool isClick;

        [Browsable(false)]
        [Description("Array of property names that need multi-language translation when the control is displayed"), Category("SunnyUI")]
        public string[] FormTranslatorProperties => ["Text"];

        [DefaultValue(true)]
        [Description("Whether the control needs multi-language translation when displayed"), Category("SunnyUI")]
        public bool MultiLanguageSupport { get; set; } = true;


        /// <summary>
        /// Trigger click event
        /// </summary>
        public void PerformClick()
        {
            if (isClick) return;
            if (Enabled)
            {
                isClick = true;
                OnClick(EventArgs.Empty);
                isClick = false;
            }
        }

        /// <summary>
        /// Disable control scaling with the form
        /// </summary>
        [DefaultValue(false), Category("SunnyUI"), Description("Disable control scaling with the form")]
        public bool ZoomScaleDisabled { get; set; }

        /// <summary>
        /// Control's position in its container before scaling
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
        /// Tag string
        /// </summary>
        [DefaultValue(null)]
        [Description("Get or set the object string containing data about the control"), Category("SunnyUI")]
        public string TagString { get; set; }

        public void SetStyleColor(UIBaseStyle uiColor)
        {
            foreColor = uiColor.ImageButtonForeColor;
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

        private UIStyle _style = UIStyle.Inherited;
        private float DefaultFontSize = -1;

        public void SetDPIScale()
        {
            if (!UIDPIScale.NeedSetDPIFont()) return;
            if (DefaultFontSize < 0) DefaultFontSize = this.Font.Size;
            this.SetDPIScaleFont(DefaultFontSize);
        }

        [Category("SunnyUI")]
        [Description("Button text")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    Invalidate();
                }
            }
        }

        [Description("Text alignment"), Category("SunnyUI")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get => textAlign;
            set
            {
                textAlign = value;
                Invalidate();
            }
        }

        [Category("SunnyUI")]
        [Description("Text font")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                Invalidate();
            }
        }

        [Category("SunnyUI")]
        [Description("Text color")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public override Color ForeColor
        {
            get => foreColor;
            set
            {
                foreColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DefaultValue(typeof(Image), "null")]
        [Description("Initial image"), Category("SunnyUI")]
        public new Image InitialImage { get; set; }

        [Browsable(false)]
        [DefaultValue(typeof(Image), "null")]
        [Description("Error image"), Category("SunnyUI")]
        public new Image ErrorImage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UIImageButton()
        {
            SetDefaultControlStyles();
            SuspendLayout();
            BorderStyle = BorderStyle.None;
            ResumeLayout(false);
            Width = 100;
            Height = 35;
            Version = UIGlobal.Version;
            Cursor = Cursors.Hand;
            base.Font = UIStyles.Font();
        }

        /// <summary>
        /// Custom theme style
        /// </summary>
        [DefaultValue(false), Browsable(false)]
        [Description("Get or set the ability to customize the theme style"), Category("SunnyUI")]
        public bool StyleCustomMode { get; set; }

        public string Version { get; }

        /// <summary>
        /// Mouse hover image
        /// </summary>
        [DefaultValue(typeof(Image), "null")]
        [Description("Mouse hover image"), Category("SunnyUI")]
        public Image ImageHover
        {
            get => imageHover;

            set
            {
                imageHover = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Mouse press image
        /// </summary>
        [DefaultValue(typeof(Image), "null")]
        [Description("Mouse press image"), Category("SunnyUI")]
        public Image ImagePress
        {
            get => imagePress;

            set
            {
                imagePress = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Disabled image
        /// </summary>
        [DefaultValue(typeof(Image), "null")]
        [Description("Disabled image"), Category("SunnyUI")]
        public Image ImageDisabled
        {
            get => imageDisabled;
            set
            {
                imageDisabled = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Selected image
        /// </summary>
        [DefaultValue(typeof(Image), "null")]
        [Description("Selected image"), Category("SunnyUI")]
        public Image ImageSelected
        {
            get => imageSelected;
            set
            {
                imageSelected = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Whether selected
        /// </summary>
        [DefaultValue(typeof(bool), "false")]
        [Description("Whether selected"), Category("SunnyUI")]
        public bool Selected
        {
            get => selected;

            set
            {
                if (selected != value)
                {
                    selected = value;
                    Invalidate();
                }
            }
        }

        private void SetDefaultControlStyles()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            UpdateStyles();
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
        /// Override mouse enter event
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!DesignMode)
            {
                Cursor = Cursors.Hand;
            }

            IsHover = true;
            Invalidate();
        }

        /// <summary>
        /// Override mouse leave event
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsHover = false;
            IsPress = false;
            Invalidate();
        }

        private Point imageOffset;

        [DefaultValue(typeof(Point), "0, 0")]
        [Description("Image offset position"), Category("SunnyUI")]
        public Point ImageOffset
        {
            get => imageOffset;
            set
            {
                imageOffset = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw button
        /// </summary>
        /// <param name="pe">Paint event args</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Image img = Image;

            if (!Enabled)
            {
                img = imageDisabled;
            }
            else
            {
                if (IsPress)
                {
                    img = imagePress;
                }
                else if (IsHover)
                {
                    img = imageHover;
                }

                if (Selected)
                {
                    img = imageSelected;
                }
            }

            if (img == null)
            {
                img = Image;
            }

            if (img != null)
            {
                if (SizeMode == PictureBoxSizeMode.Normal)
                    pe.Graphics.DrawImage(img, new Rectangle(ImageOffset.X, ImageOffset.Y, img.Width, img.Height));

                if (SizeMode == PictureBoxSizeMode.StretchImage)
                    pe.Graphics.DrawImage(img, new Rectangle(0, 0, Width, Height));

                if (SizeMode == PictureBoxSizeMode.AutoSize)
                {
                    Width = img.Width;
                    Height = img.Height;
                    pe.Graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
                }

                if (SizeMode == PictureBoxSizeMode.Zoom)
                    pe.Graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));

                if (SizeMode == PictureBoxSizeMode.CenterImage)
                    pe.Graphics.DrawImage(img, new Rectangle((Width - img.Width) / 2, (Height - img.Height) / 2, img.Width, img.Height));
            }
            else
            {
                base.OnPaint(pe);
            }

            pe.Graphics.DrawString(text, Font, ForeColor, new Rectangle(Padding.Left, Padding.Top, Width - Padding.Left - Padding.Right, Height - Padding.Top - Padding.Bottom), TextAlign);
        }
    }
}