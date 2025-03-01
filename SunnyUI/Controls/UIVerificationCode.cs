/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIVerificationCode.cs
 * Description: Verification code control
 * Current Version: V3.1
 * Creation Date: 2022-06-11
 *
 * 2022-06-11: V3.1.9 Added file description
 * 2023-05-16: V3.3.6 Refactored DrawString function
 * 2022-05-28: V3.3.7 Modified display when scaling fonts
 * 2024-12-12: V3.8.0 Customizable colors #IBABW1
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    [ToolboxItem(true)]
    public class UIVerificationCode : UIControl
    {
        public UIVerificationCode()
        {
            SetStyleFlags();
            fillColor = UIStyles.Blue.PlainColor;
            foreColor = UIStyles.Blue.RectColor;
            Width = 100;
            Height = 35;
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            fillColor = uiColor.PlainColor;
            foreColor = uiColor.RectColor;
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Invalidate();
        }

        /// <summary>
        /// Border color
        /// </summary>
        [Description("Border color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color RectColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            base.OnPaintFill(g, path);

            using var bmp = CreateImage(RandomEx.RandomChars(CodeLength));
            g.DrawImage(bmp, Width / 2 - bmp.Width / 2, 1);
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            if (Text != "") Text = "";
            //base.OnPaintFore(g, path);
        }

        [DefaultValue(4)]
        [Description("Verification code length"), Category("SunnyUI")]
        public int CodeLength { get; set; } = 4;

        [DefaultValue(18)]
        [Description("Verification code font size"), Category("SunnyUI")]
        public int CodeFontSize { get; set; } = 18;

        [DefaultValue(null)]
        [Description("Verification code text"), Category("SunnyUI")]
        public string Code { get; private set; }

        /// <summary>
        /// Generate image
        /// </summary>
        /// <param name="code">Verification code expression</param>
        private Bitmap CreateImage(string code)
        {
            byte gdiCharSet = UIStyles.GetGdiCharSet(Font.Name);
            using Font font = new Font(Font.Name, CodeFontSize, FontStyle.Bold, GraphicsUnit.Point, gdiCharSet);
            using Font fontex = font.DPIScaleFont(font.Size);
            Code = code;
            Size sf = TextRenderer.MeasureText(code, fontex);
            using Bitmap image = new Bitmap((int)sf.Width + 16, Height - 2);

            //Create canvas
            Graphics g = Graphics.FromImage(image);
            Random random = new Random();

            //Image background color
            g.Clear(fillColor);

            //Draw image background lines
            for (int i = 0; i < 5; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(Color.Black, x1, y1, x2, y2, true);
            }

            //Draw image foreground noise points
            for (int i = 0; i < 30; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }

            using Brush br = new SolidBrush(foreColor);
            g.DrawString(code, fontex, br, image.Width / 2 - sf.Width / 2, image.Height / 2 - sf.Height / 2);
            return TwistImage(image, true, 3, 5);
        }

        ///<summary>
        ///Sine wave distortion of the image
        ///</summary>
        ///<param name="srcBmp">Image path</param>
        ///<param name="bXDir">If distorted, choose True</param>
        ///<param name="nMultValue">Amplitude multiplier of the waveform, the larger the value, the higher the degree of distortion, generally 3</param>
        ///<param name="dPhase">Starting phase of the waveform, range [0-2*PI)</param>
        ///<returns></returns>
        private Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // Fill the bitmap background with white
            using Graphics graph = Graphics.FromImage(destBmp);
            using SolidBrush br = new SolidBrush(fillColor);
            graph.FillRectangle(br, 0, 0, destBmp.Width, destBmp.Height);
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (Math.PI * 2 * (double)j) / dBaseAxisLen : (Math.PI * 2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // Get the color of the current point
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);
                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }
    }
}
