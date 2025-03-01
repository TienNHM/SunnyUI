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
 * File Name: UIRuler.cs
 * Description: Ruler
 * Current Version: V3.6.1
 * Creation Date: 2023-11-29
 *
 * 2023-11-29: V3.6.1 Added file description
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public class UIRuler : UIControl
    {
        private UITrackBar.BarDirection direction = UITrackBar.BarDirection.Horizontal;

        [DefaultValue(UITrackBar.BarDirection.Horizontal)]
        [Description("Line direction"), Category("SunnyUI")]
        public UITrackBar.BarDirection Direction
        {
            get => direction;
            set
            {
                direction = value;
                Invalidate();
            }
        }

        public UIRuler()
        {
            SetStyleFlags();
            Width = 150;
            Height = 29;

            ShowText = false;
            ShowRect = false;

            fillColor = UIColor.Transparent;
            BackColor = UIColor.Transparent;
        }

        private int interval = 15;

        [DefaultValue(15)]
        [Description("Interval on both sides"), Category("SunnyUI")]
        public int Interval
        {
            get => interval;
            set
            {
                interval = value;
                Invalidate();
            }
        }

        public enum UITextPos
        {
            Front,
            Behind
        }

        UITextPos textDirection = UITextPos.Front;

        [DefaultValue(UITextPos.Front)]
        [Description("Text position"), Category("SunnyUI")]
        public UITextPos TextDirection
        {
            get => textDirection;
            set
            {
                textDirection = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            double value = StartValue;

            //水平显示
            if (Direction == UITrackBar.BarDirection.Horizontal)
            {
                float step = (Width - interval * 2) * 1.0f / stepCount;
                float mstep = 0;
                if (minorCount > 0) mstep = step * 1.0f / (minorCount + 1);
                float left = interval;

                //文字在上面
                if (TextDirection == UITextPos.Front)
                {
                    for (int i = 0; i <= stepCount; i++)
                    {
                        e.Graphics.DrawLine(LineColor, left, Height, left, Height - MajorSize);
                        if (i < stepCount)
                        {
                            for (int j = 0; j < minorCount; j++)
                            {
                                e.Graphics.DrawLine(LineColor, left + mstep * (j + 1), Height, left + mstep * (j + 1), Height - minorSize);
                            }
                        }

                        string str = value.ToString("F" + decimalPlaces);
                        e.Graphics.DrawString(str, Font, ForeColor, new Rectangle((int)left - 100, Height - MajorSize - 200 - 1, 200, 200), ContentAlignment.BottomCenter, 1, 0);
                        left += step;
                        value += StepValue;
                    }
                }

                if (TextDirection == UITextPos.Behind)
                {
                    for (int i = 0; i <= stepCount; i++)
                    {
                        e.Graphics.DrawLine(LineColor, left, 0, left, MajorSize);
                        if (i < stepCount)
                        {
                            for (int j = 0; j < minorCount; j++)
                            {
                                e.Graphics.DrawLine(LineColor, left + mstep * (j + 1), 0, left + mstep * (j + 1), minorSize);
                            }
                        }

                        string str = value.ToString("F" + decimalPlaces);
                        e.Graphics.DrawString(str, Font, ForeColor, new Rectangle((int)left - 100, MajorSize + 1, 200, 200), ContentAlignment.TopCenter, 1, 0);
                        left += step;
                        value += StepValue;
                    }
                }
            }

            //垂直显示，从下往上
            if (Direction == UITrackBar.BarDirection.Vertical)
            {
                float step = (Height - interval * 2) * 1.0f / stepCount;
                float mstep = 0;
                if (minorCount > 0) mstep = step * 1.0f / (minorCount + 1);
                float top = Height - interval;

                //文字在左面
                if (TextDirection == UITextPos.Front)
                {
                    for (int i = 0; i <= stepCount; i++)
                    {
                        e.Graphics.DrawLine(LineColor, Width, top, Width - MajorSize, top);
                        if (i < stepCount)
                        {
                            for (int j = 0; j < minorCount; j++)
                            {
                                e.Graphics.DrawLine(LineColor, Width, top - mstep * (j + 1), Width - minorSize, top - mstep * (j + 1));
                            }
                        }

                        string str = value.ToString("F" + decimalPlaces);
                        e.Graphics.DrawString(str, Font, ForeColor, new Rectangle(Width - MajorSize - 200 + 1, (int)top - 100, 200, 200), ContentAlignment.MiddleRight, 0, 0);
                        top -= step;
                        value += StepValue;
                    }
                }

                //文字在右边
                if (TextDirection == UITextPos.Behind)
                {
                    for (int i = 0; i <= stepCount; i++)
                    {
                        e.Graphics.DrawLine(LineColor, 0, top, MajorSize, top);
                        if (i < stepCount)
                        {
                            for (int j = 0; j < minorCount; j++)
                            {
                                e.Graphics.DrawLine(LineColor, 0, top - mstep * (j + 1), minorSize, top - mstep * (j + 1));
                            }
                        }

                        string str = value.ToString("F" + decimalPlaces);
                        e.Graphics.DrawString(str, Font, ForeColor, new Rectangle(MajorSize + 1, (int)top - 100, 200, 200), ContentAlignment.MiddleLeft, 0, 0);
                        top -= step;
                        value += StepValue;
                    }
                }
            }

            //垂直显示，从上往下
            if (Direction == UITrackBar.BarDirection.VerticalDown)
            {
                float step = (Height - interval * 2) * 1.0f / stepCount;
                float mstep = 0;
                if (minorCount > 0) mstep = step * 1.0f / (minorCount + 1);
                float top = interval;

                //文字在左面
                if (TextDirection == UITextPos.Front)
                {
                    for (int i = 0; i <= stepCount; i++)
                    {
                        e.Graphics.DrawLine(LineColor, Width, top, Width - MajorSize, top);
                        if (i < stepCount)
                        {
                            for (int j = 0; j < minorCount; j++)
                            {
                                e.Graphics.DrawLine(LineColor, Width, top + mstep * (j + 1), Width - minorSize, top + mstep * (j + 1));
                            }
                        }

                        string str = value.ToString("F" + decimalPlaces);
                        e.Graphics.DrawString(str, Font, ForeColor, new Rectangle(Width - MajorSize - 200 + 1, (int)top - 100, 200, 200), ContentAlignment.MiddleRight, 0, 0);
                        top += step;
                        value += StepValue;
                    }
                }

                //文字在右边
                if (TextDirection == UITextPos.Behind)
                {
                    for (int i = 0; i <= stepCount; i++)
                    {
                        e.Graphics.DrawLine(LineColor, 0, top, MajorSize, top);
                        if (i < stepCount)
                        {
                            for (int j = 0; j < minorCount; j++)
                            {
                                e.Graphics.DrawLine(LineColor, 0, top + mstep * (j + 1), minorSize, top + mstep * (j + 1));
                            }
                        }

                        string str = value.ToString("F" + decimalPlaces);
                        e.Graphics.DrawString(str, Font, ForeColor, new Rectangle(MajorSize + 1, (int)top - 100, 200, 200), ContentAlignment.MiddleLeft, 0, 0);
                        top += step;
                        value += StepValue;
                    }
                }
            }
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            rectColor = uiColor.LineRectColor;
            ForeColor = uiColor.LineForeColor;
            fillColor = UIColor.Transparent;
            BackColor = UIColor.Transparent;
        }

        /// <summary>
        /// Line color
        /// </summary>
        [Description("Line color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color LineColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        private double startValue = 0;

        [Description("Starting value"), Category("SunnyUI")]
        [DefaultValue(typeof(double), "0")]
        public double StartValue
        {
            get => startValue;
            set
            {
                startValue = value;
                Invalidate();
            }
        }

        private double stepValue = 20;

        [Description("Increment value"), Category("SunnyUI")]
        [DefaultValue(typeof(double), "20")]
        public double StepValue
        {
            get => stepValue;
            set
            {
                stepValue = Math.Max(1, value);
                Invalidate();
            }
        }

        private int stepCount = 5;

        [Description("Number of increments"), Category("SunnyUI")]
        [DefaultValue(5)]
        public int StepCount
        {
            get => stepCount;
            set
            {
                stepCount = Math.Max(1, value);
                Invalidate();
            }
        }

        private int majorSize = 6;

        [Description("Major tick length"), Category("SunnyUI")]
        [DefaultValue(6)]
        public int MajorSize
        {
            get => majorSize;
            set
            {
                majorSize = Math.Max(4, value);
                Invalidate();
            }
        }

        private int minorSize = 3;

        [Description("Minor tick length"), Category("SunnyUI")]
        [DefaultValue(3)]
        public int MinorSize
        {
            get => minorSize;
            set
            {
                minorSize = Math.Max(2, value);
                Invalidate();
            }
        }

        private int minorCount = 1;

        [Description("Number of minor ticks"), Category("SunnyUI")]
        [DefaultValue(1)]
        public int MinorCount
        {
            get => minorCount;
            set
            {
                minorCount = Math.Max(0, value);
                Invalidate();
            }
        }

        private int decimalPlaces = 0;
        [Description("Decimal places for displayed text"), Category("SunnyUI")]
        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get => decimalPlaces;
            set
            {
                decimalPlaces = Math.Max(0, value);
                Invalidate();
            }
        }
    }
}
