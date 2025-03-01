/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua.
 * QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIStatusBox.cs
 * Description: Control to display images based on status
 * Current Version: V3.8.1
 * Creation Date: 2025-01-18
 *
 * 2025-01-18: V3.8.1 Added file
******************************************************************************/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public class UIStatusBox : PictureBox
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UIStatusBox()
        {
            SetDefaultControlStyles();
            SuspendLayout();
            BorderStyle = BorderStyle.None;
            ResumeLayout(false);
            Width = 36;
            Height = 36;
            Version = UIGlobal.Version;
        }

        private void SetDefaultControlStyles()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            UpdateStyles();
        }

        public string Version { get; }

        [Browsable(false)]
        [DefaultValue(typeof(Image), "null")]
        [Description("Initial image"), Category("SunnyUI")]
        public new Image InitialImage { get; set; }

        [Browsable(false)]
        [DefaultValue(typeof(Image), "null")]
        [Description("Error image"), Category("SunnyUI")]
        public new Image ErrorImage { get; set; }

        [DefaultValue(typeof(Image), "null")]
        [Description("Image 1"), Category("SunnyUI")]
        public Image Status1 { get; set; }

        [DefaultValue(typeof(Image), "null")]
        [Description("Image 2"), Category("SunnyUI")]
        public Image Status2 { get; set; }

        [DefaultValue(typeof(Image), "null")]
        [Description("Image 3"), Category("SunnyUI")]
        public Image Status3 { get; set; }

        [DefaultValue(typeof(Image), "null")]
        [Description("Image 4"), Category("SunnyUI")]
        public Image Status4 { get; set; }

        [DefaultValue(typeof(Image), "null")]
        [Description("Image 5"), Category("SunnyUI")]
        public Image Status5 { get; set; }

        [DefaultValue(typeof(Image), "null")]
        [Description("Image 6"), Category("SunnyUI")]
        public Image Status6 { get; set; }

        private int _status;

        [DefaultValue(0)]
        [Description("Status"), Category("SunnyUI")]
        public int Status
        {
            get => _status;
            set
            {
                _status = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw status image
        /// </summary>
        /// <param name="pe">pe</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Image img = Image;

            if (_status == 1 && Status1 != null) img = Status1;
            if (_status == 2 && Status2 != null) img = Status2;
            if (_status == 3 && Status3 != null) img = Status3;
            if (_status == 4 && Status4 != null) img = Status4;
            if (_status == 5 && Status5 != null) img = Status5;
            if (_status == 6 && Status6 != null) img = Status6;

            if (img != null)
            {
                if (SizeMode == PictureBoxSizeMode.Normal)
                    pe.Graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));

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
        }
    }
}
