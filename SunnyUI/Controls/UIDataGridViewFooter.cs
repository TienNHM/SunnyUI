/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, Multi-page Development Framework.
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
 * File Name: UIDataGridViewFooter
 * File Description: DataGridView footer, can be used for statistics display
 * Current Version: V3.1
 * Creation Date: 2021-04-20
 *
 * 2021-04-20: V3.0.3 Added file description
 * 2021-09-24: V3.0.7 Text display direction is consistent with Column column display direction
 * 2021-11-22: V3.0.9 Fixed a possible display issue
 * 2022-09-05: V3.2.3 Refactored text display
 * 2023-05-15: V3.3.6 Refactored DrawString function
 * 2024-06-01: V3.6.6 Added binding column's ColumnName or DataPropertyName can be displayed
******************************************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public sealed class UIDataGridViewFooter : UIControl
    {
        public UIDataGridViewFooter()
        {
            SetStyleFlags(true, false, true);
            Height = 29;
            RadiusSides = UICornerRadiusSides.None;
            //Font = UIStyles.Font();
            foreColor = UIStyles.Blue.DataGridViewFooterForeColor;
            fillColor = UIStyles.Blue.PlainColor;
            rectColor = UIStyles.Blue.RectColor;
        }

        private UIDataGridView dgv;
        public UIDataGridView DataGridView
        {
            get => dgv;
            set
            {
                dgv = value;
                if (dgv != null)
                {
                    dgv.Paint += Dgv_Paint;
                }
            }
        }

        private void Dgv_Paint(object sender, PaintEventArgs e)
        {
            Invalidate();
        }

        public void Clear()
        {
            dictionary.Clear();
            Invalidate();
        }

        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public string this[string name]
        {
            get => dictionary.ContainsKey(name) ? dictionary[name] : "";
            set
            {
                if (dictionary.NotContainsKey(name))
                    dictionary.Add(name, value);
                else
                    dictionary[name] = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Draw foreground color
        /// </summary>
        /// <param name="g">Graphics surface</param>
        /// <param name="path">Graphics path</param>
        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            if (dgv != null && dgv.ColumnCount > 0 && dgv.RowCount > 0)
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    bool ShowGridLine = dgv.CellBorderStyle == DataGridViewCellBorderStyle.Single;
                    Rectangle rect = dgv.GetColumnDisplayRectangle(column.Index, false);
                    if (rect.Width == 0) continue;
                    rect = new Rectangle(rect.Right - column.Width, rect.Top, column.Width, rect.Height);
                    int minleft = ShowGridLine ? 1 : 0;

                    if (rect.Left == minleft && rect.Width == 0) continue;
                    if (rect.Left >= minleft && ShowGridLine)
                    {
                        g.DrawLine(dgv.GridColor, rect.Left - minleft, 0, rect.Left - minleft, Height);
                        g.DrawLine(dgv.GridColor, rect.Right - minleft, 0, rect.Right - minleft, Height);
                    }

                    string str = this[column.Name];
                    if (str.IsNullOrEmpty()) str = this[column.DataPropertyName];
                    if (str.IsNullOrEmpty()) continue;

                    var align = column.DefaultCellStyle.Alignment;
                    if (rect.Left == 0 && rect.Width == 0) continue;
                    switch (align)
                    {
                        case DataGridViewContentAlignment.NotSet:
                        case DataGridViewContentAlignment.TopLeft:
                        case DataGridViewContentAlignment.MiddleLeft:
                        case DataGridViewContentAlignment.BottomLeft:
                            if (rect.Left == minleft && rect.Width < column.Width)
                            {
                                g.DrawString(str, Font, ForeColor, new Rectangle(rect.Width - column.Width, 0, Width, Height), ContentAlignment.MiddleLeft);
                            }
                            else
                            {
                                g.DrawString(str, Font, ForeColor, new Rectangle(rect.Left, 0, Width, Height), ContentAlignment.MiddleLeft);
                            }

                            break;
                        case DataGridViewContentAlignment.TopCenter:
                        case DataGridViewContentAlignment.MiddleCenter:
                        case DataGridViewContentAlignment.BottomCenter:
                            if (rect.Left == minleft && rect.Width < column.Width)
                            {
                                g.DrawString(str, Font, ForeColor, new Rectangle(rect.Width - column.Width, 0, column.Width, Height), ContentAlignment.MiddleCenter);
                            }
                            else
                            {
                                g.DrawString(str, Font, ForeColor, new Rectangle(rect.Left, 0, column.Width, Height), ContentAlignment.MiddleCenter);
                            }

                            break;
                        case DataGridViewContentAlignment.TopRight:
                        case DataGridViewContentAlignment.MiddleRight:
                        case DataGridViewContentAlignment.BottomRight:
                            if (rect.Left == minleft && rect.Width < column.Width)
                            {
                                g.DrawString(str, Font, ForeColor, new Rectangle(rect.Left, 0, column.Width, Height), ContentAlignment.MiddleRight);
                            }
                            else
                            {
                                g.DrawString(str, Font, ForeColor, new Rectangle(rect.Left, 0, column.Width, Height), ContentAlignment.MiddleRight);
                            }

                            break;
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
            foreColor = uiColor.DataGridViewFooterForeColor;
            fillColor = uiColor.PlainColor;
            rectColor = uiColor.RectColor;
        }

        /// <summary>
        /// Fill color, if the value is background color or transparent color or empty value, it will not be filled
        /// </summary>
        [Description("Fill color"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "243, 249, 255")]
        public Color FillColor
        {
            get => fillColor;
            set => SetFillColor(value);
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
        [DefaultValue(typeof(Color), "48, 48, 48")]
        public override Color ForeColor
        {
            get => foreColor;
            set => SetForeColor(value);
        }
    }
}
