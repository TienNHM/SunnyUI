/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Class Library, Extension Class Library, Multi-page Development Framework.
 * CopyRight (C) 2012-2025 ShenYongHua(Shen Yonghua).
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UITurnSwitch.cs
 * File Description: Rotary Switch
 * Current Version: V3.3
 * Creation Date: 2023-07-05
 *
 * 2023-07-05: V3.3.9 Added file description
 * 2023-07-06: V3.3.9 Adjusted color scheme, added custom angle
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("ValueChanged")]
    [DefaultProperty("Active")]
    [ToolboxItem(true)]
    public class UITurnSwitch : UIControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="value">Switch value</param>
        public delegate void OnValueChanged(object sender, bool value);

        public UITurnSwitch()
        {
            SetStyleFlags();
            Height = 160;
            Width = 160;
            ShowText = false;
            ShowRect = false;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["ActiveText", "InActiveText"];

        [DefaultValue(false)]
        [Description("Read-only"), Category("SunnyUI")]
        public bool ReadOnly { get; set; }

        public event OnValueChanged ValueChanged;

        public event EventHandler ActiveChanged;

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        private bool activeValue;

        [DefaultValue(false)]
        [Description("Is active"), Category("SunnyUI")]
        public bool Active
        {
            get => activeValue;
            set
            {
                if (!ReadOnly && activeValue != value)
                {
                    activeValue = value;
                    ValueChanged?.Invoke(this, value);
                    ActiveChanged?.Invoke(this, new EventArgs());
                    Invalidate();
                }
            }
        }

        private string activeText = "On";

        [DefaultValue("On")]
        [Description("Active text"), Category("SunnyUI")]
        public string ActiveText
        {
            get => activeText;
            set
            {
                activeText = value;
                Invalidate();
            }
        }

        private string inActiveText = "Off";

        [DefaultValue("Off")]
        [Description("Inactive text"), Category("SunnyUI")]
        public string InActiveText
        {
            get => inActiveText;
            set
            {
                inActiveText = value;
                Invalidate();
            }
        }

        private Color inActiveColor = Color.Red;

        [DefaultValue(typeof(Color), "Red")]
        [Description("Inactive color"), Category("SunnyUI")]
        public Color InActiveColor
        {
            get => inActiveColor;
            set
            {
                inActiveColor = value;
                Invalidate();
            }
        }

        private Color activeColor = Color.Lime;
        /// <summary>
        /// Border color
        /// </summary>
        [Description("Active color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Lime")]
        public Color ActiveColor
        {
            get => activeColor;
            set
            {
                activeColor = value;
                Invalidate();
            }
        }

        private int inActiveAngle = 315;

        [DefaultValue(315)]
        [Description("Inactive angle"), Category("SunnyUI")]
        public int InActiveAngle
        {
            get => inActiveAngle;
            set
            {
                inActiveAngle = value;
                Invalidate();
            }
        }

        private int activeAngle = 45;
        /// <summary>
        /// Border color
        /// </summary>
        [Description("Active angle"), Category("SunnyUI")]
        [DefaultValue(45)]
        public int ActiveAngle
        {
            get => activeAngle;
            set
            {
                activeAngle = value;
                Invalidate();
            }
        }

        private Color backColor = Color.Silver;

        /// <summary>
        /// Fill color
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "Silver")]
        public Color FillColor
        {
            get => backColor;
            set
            {
                backColor = value;
                Invalidate();
            }
        }

        private Color handleColor = Color.DarkGray;

        /// <summary>
        /// Handle color
        /// </summary>
        [Description("Handle color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color HandleColor
        {
            get => handleColor;
            set
            {
                handleColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Click event
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnClick(EventArgs e)
        {
            ActiveChange();
            base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (!UseDoubleClick)
            {
                ActiveChange();
                base.OnClick(e);
            }
            else
            {
                base.OnDoubleClick(e);
            }
        }

        public event OnCancelEventArgs ActiveChanging;

        private void ActiveChange()
        {
            CancelEventArgs e = new CancelEventArgs();
            if (ActiveChanging != null)
            {
                ActiveChanging?.Invoke(this, e);
            }

            if (!e.Cancel)
            {
                Active = !Active;
            }
        }

        private int backSize = 100;

        [Description("Switch size"), Category("SunnyUI")]
        [DefaultValue(100)]
        public int BackSize
        {
            get => backSize;
            set
            {
                backSize = value;
                Invalidate();
            }
        }

        private int backInnerSize = 80;

        [Description("Inner switch size"), Category("SunnyUI")]
        [DefaultValue(80)]
        public int BackInnerSize
        {
            get => backInnerSize;
            set
            {
                backInnerSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            Color color = Active ? ActiveColor : InActiveColor;
            if (!Enabled) color = rectDisableColor;

            Point center = new Point(Width / 2, Height / 2);
            g.FillEllipse(rectColor, new Rectangle(center.X - BackSize / 2, center.Y - BackSize / 2, BackSize, BackSize));
            int size = backSize - 10;
            g.FillEllipse(Color.White, new Rectangle(center.X - size / 2, center.Y - size / 2, size, size));
            g.FillEllipse(FillColor, new Rectangle(center.X - backInnerSize / 2, center.Y - backInnerSize / 2, backInnerSize, backInnerSize));

            int size2 = 6;
            using Pen pn = rectColor.Pen(2);
            PointF pt;
            if (Active)
            {
                PointF[] points = GetHandles(ActiveAngle);
                g.FillPolygon(HandleColor, points);
                g.DrawPolygon(pn, points);
                pt = center.CalcAzRangePoint(BackSize / 2 - size2, ActiveAngle);
            }
            else
            {
                PointF[] points = GetHandles(InActiveAngle);
                g.FillPolygon(HandleColor, points);
                g.DrawPolygon(pn, points);
                pt = center.CalcAzRangePoint(BackSize / 2 - size2, InActiveAngle);
            }

            g.FillEllipse(color, pt.X - size2, pt.Y - size2, size2 * 2, size2 * 2);
            Size sz = TextRenderer.MeasureText(ActiveText, Font);
            pt = center.CalcAzRangePoint(BackSize / 2 + size2 + 4 + sz.Width / 2, ActiveAngle);
            g.DrawString(ActiveText, Font, ActiveColor, new Rectangle((int)(pt.X - sz.Width / 2), (int)(pt.Y - sz.Height / 2), sz.Width, sz.Height), ContentAlignment.MiddleCenter);

            sz = TextRenderer.MeasureText(InActiveText, Font);
            pt = center.CalcAzRangePoint(BackSize / 2 + size2 + 4 + sz.Width / 2, InActiveAngle);
            g.DrawString(InActiveText, Font, InActiveColor, new Rectangle((int)(pt.X - sz.Width / 2), (int)(pt.Y - sz.Height / 2), sz.Width, sz.Height), ContentAlignment.MiddleCenter);

        }

        private PointF[] GetHandles(int angle)
        {
            int size1 = 10;
            int size2 = 4;
            Point center = new Point(Width / 2, Height / 2);
            PointF pt1 = center.CalcAzRangePoint(size1, angle - 90);
            PointF pt2 = center.CalcAzRangePoint(size1, angle + 90);
            PointF pt3 = pt1.CalcAzRangePoint(BackSize / 2 + size2, angle);
            PointF pt4 = pt2.CalcAzRangePoint(BackSize / 2 + size2, angle);
            PointF pt5 = pt1.CalcAzRangePoint(BackSize / 2 + size2, angle + 180);
            PointF pt6 = pt2.CalcAzRangePoint(BackSize / 2 + size2, angle + 180);

            PointF pt11 = center.CalcAzRangePoint(size1 - 2, angle - 90);
            PointF pt12 = center.CalcAzRangePoint(size1 - 2, angle + 90);
            PointF pt13 = pt11.CalcAzRangePoint(BackSize / 2 + size2 + 2, angle);
            PointF pt14 = pt12.CalcAzRangePoint(BackSize / 2 + size2 + 2, angle);
            PointF pt15 = pt11.CalcAzRangePoint(BackSize / 2 + size2 + 2, angle + 180);
            PointF pt16 = pt12.CalcAzRangePoint(BackSize / 2 + size2 + 2, angle + 180);

            return new PointF[] { pt3, pt13, pt14, pt4, pt6, pt16, pt15, pt5 };
        }
    }
}
