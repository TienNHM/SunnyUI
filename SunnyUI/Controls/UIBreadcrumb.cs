/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua.
 * QQ Group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIBreadcrumb.cs
 * File Description: Breadcrumb Navigation Bar
 * Current Version: V3.1
 * Creation Date: 2021-04-10
 *
 * 2021-04-10: V3.0.2 Added file description
 * 2022-01-26: V3.1.0 Added align both ends, AlignBothEnds
 * 2022-01-26: V3.1.0 Added unselected step text color
 * 2022-03-19: V3.1.1 Refactored theme colors
 * 2023-05-12: V3.3.6 Refactored DrawString function
 * 2023-09-17: V3.4.2 Added Readonly, disabled mouse click, can set ItemIndex through code
******************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    /// <summary>
    /// Breadcrumb Navigation Bar
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("ItemIndexChanged")]
    [DefaultProperty("ItemIndex")]
    public class UIBreadcrumb : UIControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UIBreadcrumb()
        {
            items.CountChange += Items_CountChange;
            SetStyleFlags(true, false);
            ShowText = false;
            ShowRect = false;
            Height = 29;
            ItemWidth = 120;

            fillColor = UIColor.Blue;
            rectColor = Color.FromArgb(155, 200, 255);
            foreColor = Color.White;
        }

        private void Items_CountChange(object sender, EventArgs e)
        {
            Invalidate();
        }

        private bool alignBothEnds;

        /// <summary>
        /// Align both ends when displayed
        /// </summary>
        [DefaultValue(false)]
        [Description("Align both ends when displayed"), Category("SunnyUI")]
        public bool AlignBothEnds
        {
            get => alignBothEnds;
            set
            {
                if (alignBothEnds != value)
                {
                    alignBothEnds = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Step value change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        public delegate void OnValueChanged(object sender, int value);

        /// <summary>
        /// Step value change event
        /// </summary>
        public event OnValueChanged ItemIndexChanged;

        /// <summary>
        /// Step item list
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [MergableProperty(false)]
        [Description("Item list"), Category("SunnyUI")]
        public UIObjectCollection Items => items;

        private readonly UIObjectCollection items = new UIObjectCollection();

        private readonly ConcurrentDictionary<int, Point[]> ClickArea = new ConcurrentDictionary<int, Point[]>();

        /// <summary>
        /// Number of steps
        /// </summary>
        [Browsable(false)]
        public int Count => Items.Count;

        private int itemIndex = -1;

        /// <summary>
        /// Current node index
        /// </summary>
        [DefaultValue(-1)]
        [Description("Current node index"), Category("SunnyUI")]
        public int ItemIndex
        {
            get => itemIndex;
            set
            {
                if (Count == 0)
                {
                    itemIndex = 0;
                }
                else
                {
                    itemIndex = Math.Max(-1, value);
                    itemIndex = Math.Min(Count - 1, value);
                    ItemIndexChanged?.Invoke(this, itemIndex);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Draw fill color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFill(Graphics g, GraphicsPath path)
        {
            float width = 0;
            if (Items.Count == 0)
            {
                Size sf = TextRenderer.MeasureText(Text, Font);
                width = sf.Width + Height + 6;
                if (itemWidth < width) itemWidth = (int)width;
                List<PointF> points = new List<PointF>();
                points.Add(new PointF(3, 0));
                points.Add(new PointF(Width - 3 - Height / 2.0f, 0));
                points.Add(new PointF(Width - 3, Height / 2.0f));
                points.Add(new PointF(Width - 3 - Height / 2.0f, Height));
                points.Add(new PointF(3, Height));
                points.Add(new PointF(3 + Height / 2.0f, Height / 2.0f));
                points.Add(new PointF(3, 0));

                using Brush br = new SolidBrush(SelectedColor);
                g.FillPolygon(br, points.ToArray());
                g.DrawString(Text, Font, ForeColor, ClientRectangle, ContentAlignment.MiddleCenter);
            }
            else
            {
                foreach (var item in Items)
                {
                    Size sf = TextRenderer.MeasureText(item.ToString(), Font);
                    width = Math.Max(width, sf.Width);
                }

                width = width + Height + 6;
                if (itemWidth < width) itemWidth = (int)width;

                int begin = 0;
                int index = 0;
                foreach (var item in Items)
                {
                    List<PointF> points = new List<PointF>();

                    if (index == 0 && AlignBothEnds)
                    {
                        points.Add(new PointF(begin + 3, 0));
                        points.Add(new PointF(begin + itemWidth - 3 - Height / 2.0f, 0));
                        points.Add(new PointF(begin + itemWidth - 3, Height / 2.0f));
                        points.Add(new PointF(begin + itemWidth - 3 - Height / 2.0f, Height));
                        points.Add(new PointF(begin + 3, Height));
                        points.Add(new PointF(begin + 3, 0));
                    }
                    else if (index == Items.Count - 1 && AlignBothEnds)
                    {
                        points.Add(new PointF(begin + 3, 0));
                        points.Add(new PointF(begin + itemWidth - 3, 0));
                        points.Add(new PointF(begin + itemWidth - 3, Height));
                        points.Add(new PointF(begin + 3, Height));
                        points.Add(new PointF(begin + 3 + Height / 2.0f, Height / 2.0f));
                        points.Add(new PointF(begin + 3, 0));
                    }
                    else
                    {
                        points.Add(new PointF(begin + 3, 0));
                        points.Add(new PointF(begin + itemWidth - 3 - Height / 2.0f, 0));
                        points.Add(new PointF(begin + itemWidth - 3, Height / 2.0f));
                        points.Add(new PointF(begin + itemWidth - 3 - Height / 2.0f, Height));
                        points.Add(new PointF(begin + 3, Height));
                        points.Add(new PointF(begin + 3 + Height / 2.0f, Height / 2.0f));
                        points.Add(new PointF(begin + 3, 0));
                    }

                    Point[] pts = new Point[points.Count];
                    for (int i = 0; i < points.Count; i++)
                    {
                        pts[i] = new Point((int)points[i].X, (int)points[i].Y);
                    }

                    if (!ClickArea.ContainsKey(index))
                    {
                        ClickArea.TryAdd(index, pts);
                    }
                    else
                    {
                        ClickArea[index] = pts;
                    }

                    using Brush br = new SolidBrush(index <= ItemIndex ? SelectedColor : UnSelectedColor);
                    g.FillPolygon(br, points.ToArray());

                    g.DrawString(item.ToString(), Font, index <= ItemIndex ? ForeColor : UnSelectedForeColor,
                        new Rectangle(begin, 0, itemWidth, Height), ContentAlignment.MiddleCenter);
                    begin = begin + itemWidth - 3 - Height / 2 + Interval;
                    index++;
                }
            }
        }

        private int itemWidth;

        /// <summary>
        /// Node width
        /// </summary>
        [DefaultValue(160)]
        [Description("Node width"), Category("SunnyUI")]
        public int ItemWidth
        {
            get => itemWidth;
            set
            {
                itemWidth = value;
                Invalidate();
            }
        }

        private int interval = 1;

        /// <summary>
        /// Node interval
        /// </summary>
        [DefaultValue(1)]
        [Description("Node interval"), Category("SunnyUI")]
        public int Interval
        {
            get => interval;
            set
            {
                interval = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Selected node color
        /// </summary>
        [Description("Selected node color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color SelectedColor
        {
            get => fillColor;
            set => SetFillColor(value);
        }

        /// <summary>
        /// Unselected node color
        /// </summary>
        [Description("Unselected node color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "185, 217, 255")]
        public Color UnSelectedColor
        {
            get => rectColor;
            set => SetRectColor(value);
        }

        private Color unSelectedForeColor = Color.White;

        /// <summary>
        /// Unselected node text color
        /// </summary>
        [Description("Unselected node text color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color UnSelectedForeColor
        {
            get => unSelectedForeColor;
            set
            {
                if (unSelectedForeColor != value)
                {
                    unSelectedForeColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        [Description("Font color")]
        [Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            unSelectedForeColor = uiColor.ButtonForeColor;
            rectColor = uiColor.BreadcrumbUnSelectedColor;
        }

        /// <summary>
        /// Mouse click event
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (Readonly) return;
            foreach (var pair in ClickArea)
            {
                if (e.Location.InRegion(pair.Value))
                {
                    ItemIndex = pair.Key;
                    break;
                }
            }
        }

        [DefaultValue(false)]
        [Description("Disable mouse click, can set ItemIndex through code")]
        [Category("SunnyUI")]
        public bool Readonly { get; set; }
    }
}