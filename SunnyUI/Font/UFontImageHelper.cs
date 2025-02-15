/******************************************************************************
 * SunnyUI open source control library, tool class library, extension class library, multi-page development framework.
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
 * File Name: UFontImage.cs
 * Description: Font image helper class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-05-21: V2.2.5 Moved font resource files to resource files, thanks to Maikebing https://gitee.com/maikebing
 * 2021-06-15: V3.0.4 Added FontAwesomeV5 font icons, refactored code
 * 2021-06-15: V3.3.5 Added FontAwesomeV6 font icons, refactored code
 * 2023-05-16: V3.3.6 Refactored DrawFontImage method
 * 2022-05-17: V3.3.7 Fixed an issue where the editor icon was not displayed correctly
 * 2023-10-25: V3.5.1 Added 3 new MaterialIcons font icons
 ******************************************************************************/

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace Sunny.UI
{
    /// <summary>
    /// Font image helper class
    /// </summary>
    public static class FontImageHelper
    {
        public static readonly Dictionary<UISymbolType, FontImages> Fonts = new Dictionary<UISymbolType, FontImages>();

        /// <summary>
        /// Constructor
        /// </summary>
        static FontImageHelper()
        {
            Fonts.Add(UISymbolType.FontAwesomeV4, new FontImages(UISymbolType.FontAwesomeV4, ReadFontFileFromResource("Sunny.UI.Font.FontAwesome.ttf")));
            Fonts.Add(UISymbolType.ElegantIcons, new FontImages(UISymbolType.ElegantIcons, ReadFontFileFromResource("Sunny.UI.Font.ElegantIcons.ttf")));
            Fonts.Add(UISymbolType.FontAwesomeV6Brands, new FontImages(UISymbolType.FontAwesomeV6Brands, ReadFontFileFromResource("Sunny.UI.Font.fa-brands-400.ttf")));
            Fonts.Add(UISymbolType.FontAwesomeV6Regular, new FontImages(UISymbolType.FontAwesomeV6Regular, ReadFontFileFromResource("Sunny.UI.Font.fa-regular-400.ttf")));
            Fonts.Add(UISymbolType.FontAwesomeV6Solid, new FontImages(UISymbolType.FontAwesomeV6Solid, ReadFontFileFromResource("Sunny.UI.Font.fa-solid-900.ttf")));
            Fonts.Add(UISymbolType.MaterialIcons, new FontImages(UISymbolType.MaterialIcons, ReadFontFileFromResource("Sunny.UI.Font.MaterialIcons-Regular.ttf")));
        }

        private static byte[] ReadFontFileFromResource(string name)
        {
            byte[] buffer = null;
            Stream fontStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            if (fontStream != null)
            {
                buffer = new byte[fontStream.Length];
                fontStream.Read(buffer, 0, (int)fontStream.Length);
                fontStream.Close();
            }

            return buffer;
        }

        /// <summary>
        /// Get font image size
        /// </summary>
        /// <param name="graphics">GDI graphics</param>
        /// <param name="symbol">Character</param>
        /// <param name="symbolSize">Size</param>
        /// <returns>Font image size</returns>
        internal static SizeF GetFontImageSize(this Graphics graphics, int symbol, int symbolSize)
        {
            Font font = GetFont(symbol, symbolSize);
            if (font == null)
            {
                return new SizeF(0, 0);
            }

            return graphics.MeasureString(char.ConvertFromUtf32(symbol), font);
        }

        private static UISymbolType GetSymbolType(int symbol)
        {
            return (UISymbolType)symbol.Div(100000);
        }

        private static int GetSymbolValue(int symbol)
        {
            return symbol.Mod(100000);
        }

        /// <summary>
        /// Draw font image
        /// </summary>
        /// <param name="graphics">GDI graphics</param>
        /// <param name="symbol">Character</param>
        /// <param name="symbolSize">Size</param>
        /// <param name="color">Color</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="xOffset">X offset</param>
        /// <param name="yOffSet">Y offset</param>
        public static void DrawFontImage(this Graphics graphics, int symbol, int symbolSize, Color color,
            RectangleF rect, int xOffset = 0, int yOffSet = 0, int angle = 0)
        {
            SizeF sf = graphics.GetFontImageSize(symbol, symbolSize);
            float left = rect.Left + ((rect.Width - sf.Width) / 2.0f).RoundEx();
            float top = rect.Top + ((rect.Height - sf.Height) / 2.0f).RoundEx();
            //graphics.DrawFontImage(symbol, symbolSize, color, left, top + 1, xOffset, yOffSet);

            // Move the origin of the drawing (default is the top left corner) to the center of the rectangle
            graphics.TranslateTransform(left + sf.Width / 2, top + sf.Height / 2);
            // Rotate the drawing
            graphics.RotateTransform(angle);
            // Move the drawing back to the original position
            graphics.TranslateTransform(-(left + sf.Width / 2), -(top + sf.Height / 2));

            graphics.DrawFontImage(symbol, symbolSize, color, left, top, xOffset, yOffSet);

            graphics.TranslateTransform(left + sf.Width / 2, top + sf.Height / 2);
            graphics.RotateTransform(-angle);
            graphics.TranslateTransform(-(left + sf.Width / 2), -(top + sf.Height / 2));
        }

        /// <summary>
        /// Draw font image
        /// </summary>
        /// <param name="graphics">GDI graphics</param>
        /// <param name="symbol">Character</param>
        /// <param name="symbolSize">Size</param>
        /// <param name="color">Color</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        /// <param name="xOffset">X offset</param>
        /// <param name="yOffSet">Y offset</param>
        private static void DrawFontImage(this Graphics graphics, int symbol, int symbolSize, Color color,
            float left, float top, int xOffset = 0, int yOffSet = 0)
        {
            Font font = GetFont(symbol, symbolSize);
            if (font == null) return;

            var symbolValue = GetSymbolValue(symbol);
            string text = char.ConvertFromUtf32(symbolValue);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.DrawString(text, font, color, left + xOffset, top + yOffSet);
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.InterpolationMode = InterpolationMode.Default;
        }

        /// <summary>
        /// Create image
        /// </summary>
        /// <param name="symbol">Character</param>
        /// <param name="size">Size</param>
        /// <param name="color">Color</param>
        /// <returns>Image</returns>
        public static Image CreateImage(int symbol, int size, Color color)
        {
            Bitmap image = new Bitmap(size, size);
            using Graphics g = image.Graphics();
            SizeF sf = g.GetFontImageSize(symbol, size);
            g.DrawFontImage(symbol, size, color, (image.Width - sf.Width) / 2.0f, (image.Height - sf.Height) / 2.0f);
            return image;
        }

        /// <summary>
        /// Get font
        /// </summary>
        /// <param name="symbol">Character</param>
        /// <param name="imageSize">Size</param>
        /// <returns>Font</returns>
        public static Font GetFont(int symbol, int imageSize)
        {
            var symbolType = GetSymbolType(symbol);
            var symbolValue = GetSymbolValue(symbol);
            switch (symbolType)
            {
                case UISymbolType.FontAwesomeV4:
                    if (symbol > 0xF000)
                        return Fonts[UISymbolType.FontAwesomeV4].GetFont(symbolValue, imageSize);
                    else
                        return Fonts[UISymbolType.ElegantIcons].GetFont(symbolValue, imageSize);
                case UISymbolType.FontAwesomeV6Brands:
                    return Fonts[UISymbolType.FontAwesomeV6Brands].GetFont(symbolValue, imageSize);
                case UISymbolType.FontAwesomeV6Regular:
                    return Fonts[UISymbolType.FontAwesomeV6Regular].GetFont(symbolValue, imageSize);
                case UISymbolType.FontAwesomeV6Solid:
                    return Fonts[UISymbolType.FontAwesomeV6Solid].GetFont(symbolValue, imageSize);
                case UISymbolType.MaterialIcons:
                    return Fonts[UISymbolType.MaterialIcons].GetFont(symbolValue, imageSize, 3);
                default:
                    return null;
            }
        }
    }
}