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
 * File Name: UIVerScrollBarEx.cs
 * Description: Vertical Scroll Bar
 * Current Version: V3.1
 * Creation Date: 2020-08-29
 *
 * 2020-08-29: V2.2.7 Added vertical scroll bar
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2022-11-03: V3.2.6 Added property to set vertical scroll bar width
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public class UIVerScrollBarEx : UIControl
    {
        public UIVerScrollBarEx()
        {
            SetStyleFlags(true, false);
            ShowText = false;
            ShowRect = false;
            Width = ScrollBarInfo.VerticalScrollBarWidth() + 1;

            fillColor = UIStyles.Blue.ScrollBarFillColor;
            foreColor = UIStyles.Blue.ScrollBarForeColor;
            fillHoverColor = UIStyles.Blue.ScrollBarFillHoverColor;
            fillPressColor = UIStyles.Blue.ScrollBarFillPressColor;
        }

        private int fillWidth = 6;

        [DefaultValue(6)]
        public int FillWidth
        {
            get => fillWidth;
            set
            {
                fillWidth = Math.Max(6, value);
                Invalidate();
            }
        }

        private int maximum = 100;
        [DefaultValue(100)]
        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = value.GetLowerLimit(2);
                Invalidate();
            }
        }

        [DefaultValue(10)]
        public int LargeChange { get; set; } = 10;

        private int thisValue;
        public event EventHandler ValueChanged;

        [DefaultValue(0)]
        public int Value
        {
            get => thisValue;
            set
            {
                thisValue = value.GetLowerLimit(0);
                thisValue = value.GetUpperLimit(Maximum - BoundsHeight);
                Invalidate();
            }
        }

        private int boundsHeight = 10;
        [DefaultValue(10)]
        public int BoundsHeight
        {
            get => boundsHeight;
            set
            {
                boundsHeight = value.GetLowerLimit(1);
                Invalidate();
            }
        }

        public int LeftButtonPos => 16;

        public int RightButtonPos => Height - 16;

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            base.OnPaintFill(g, path);
            g.Clear(fillColor);

            DrawUpDownArrow(g, true);
            DrawUpDownArrow(g, false);
            DrawValueBar(g);
        }

        private void DrawValueBar(Graphics g)
        {
            Color clr = foreColor;
            if (inCenterArea && IsPress)
            {
                clr = fillPressColor;
            }

            int top = 16 + Value * (Height - 32) / Maximum;
            int height = BoundsHeight * (Height - 32) / Maximum;

            g.SetHighQuality();
            int w = Math.Max(6, fillWidth);
            g.FillRoundRectangle(clr, new Rectangle(Width / 2 - w / 2, top, w, height), 5);
            g.SetDefaultQuality();
        }

        private Rectangle GetUpRect()
        {
            var rect = new Rectangle(1, 1, Width - 2, 16);
            return rect;
        }

        private Rectangle GetDownRect()
        {
            var rect = new Rectangle(1, Height - 17, Width - 2, 16);
            return rect;
        }

        /// <summary>
        /// Override mouse down event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            IsPress = true;

            if (inLeftArea)
            {
                int value = (Value - LargeChange).GetLimit(0, Maximum - BoundsHeight);
                Value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            if (inRightArea)
            {
                int value = (Value + LargeChange).GetLimit(0, Maximum - BoundsHeight);
                Value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            if (inCenterArea)
            {
                int y = BoundsHeight * (Height - 32) / Maximum;
                int value = (e.Location.Y - y / 2) * maximum / (Height - 32);
                value = value.GetLimit(0, Maximum - BoundsHeight);
                Value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Override mouse up event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            IsPress = false;
            Invalidate();
        }

        /// <summary>
        /// Override mouse leave event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsPress = false;
            Invalidate();
        }

        /// <summary>
        /// Override mouse enter event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        private void DrawUpDownArrow(Graphics g, bool isUp)
        {
            Color clr_arrow = foreColor;
            if ((inLeftArea || inRightArea) && IsPress)
            {
                clr_arrow = fillPressColor;
            }

            g.FillRectangle(fillColor, isUp ? GetUpRect() : GetDownRect());
            g.SetHighQuality();
            using var pen = new Pen(clr_arrow, 2);
            Point pt1, pt2, pt3;
            if (!isUp)
            {
                pt1 = new Point(Width / 2 - 4, Height - 16 / 2 - 4);
                pt2 = new Point(Width / 2, Height - 16 / 2);
                pt3 = new Point(Width / 2 + 4, Height - 16 / 2 - 4);
            }
            else
            {
                pt1 = new Point(Width / 2 - 4, 16 / 2 + 4 - 1);
                pt2 = new Point(Width / 2, 16 / 2 - 1);
                pt3 = new Point(Width / 2 + 4, 16 / 2 + 4 - 1);
            }

            g.DrawLines(pen, new[] { pt1, pt2, pt3 });
            g.SetDefaultQuality();
        }

        private bool inLeftArea, inRightArea, inCenterArea;

        /// <summary>
        /// Override mouse move event
        /// </summary>
        /// <param name="e">Mouse event arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            inLeftArea = e.Location.Y < LeftButtonPos;
            inRightArea = e.Location.Y > RightButtonPos;
            inCenterArea = e.Location.Y >= LeftButtonPos && e.Location.Y <= RightButtonPos;

            if (inCenterArea && IsPress)
            {
                int y = BoundsHeight * (Height - 32) / Maximum;
                int value = (e.Location.Y - y / 2) * maximum / (Height - 32);
                value = value.GetLimit(0, Maximum - BoundsHeight);
                Value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            fillColor = uiColor.ScrollBarFillColor;
            foreColor = uiColor.ScrollBarForeColor;
            fillHoverColor = uiColor.ScrollBarFillHoverColor;
            fillPressColor = uiColor.ScrollBarFillPressColor;
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
        /// Fill color, if the value is background color or transparent color or null, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        [Description("Mouse hover color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "115, 179, 255")]
        public Color HoverColor
        {
            get => fillHoverColor;
            set => SetFillHoverColor(value);
        }

        [Description("Mouse press color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "64, 128, 204")]
        public Color PressColor
        {
            get => fillPressColor;
            set => SetFillPressColor(value);
        }
    }
}
