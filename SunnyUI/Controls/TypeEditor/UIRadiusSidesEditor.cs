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
 * File Name: UIRadiusSidesEditor.cs
 * Description: Border property class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#pragma warning disable SYSLIB0003 // 类型或成员已过时

namespace Sunny.UI
{
    /// <summary>
    /// Corner position
    /// </summary>
    [ComVisible(true)]
    [ToolboxItem(false)]
    [Editor("Sunny.UI.UIRadiusSidesEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
    [Flags]
    public enum UICornerRadiusSides
    {
        /// <summary>
        /// All four corners have rounded corners.
        /// </summary>
        All = 15, // 0x0000000F

        /// <summary>
        /// The bottom left corner has a rounded corner
        /// </summary>
        LeftBottom = 8,

        /// <summary>
        /// The top left corner has a rounded corner
        /// </summary>
        LeftTop = 1,

        /// <summary>
        /// The bottom right corner has a rounded corner
        /// </summary>
        RightBottom = 4,

        /// <summary>
        /// The top right corner has a rounded corner
        /// </summary>
        RightTop = 2,

        /// <summary>
        /// No rounded corners
        /// </summary>
        None = 0,
    }

    /// <summary>
    /// Provides an editor for the <see cref="P:System.Windows.Forms.ToolStripStatusLabel.RectSides" /> property.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class UIRadiusSidesEditor : UIDropEditor
    {
        /// <summary>
        /// Create form
        /// </summary>
        /// <returns>Form</returns>
        protected override UIDropEditorUI CreateUI()
        {
            return new UICornerRadiusSidesUI();
        }

        /// <summary>
        /// Property form
        /// </summary>
        [ToolboxItem(false)]
        public class UICornerRadiusSidesUI : UIDropEditorUI
        {
            private TableLayoutPanel tableLayoutPanel1;
            private CheckBox allCheckBox;
            private CheckBox noneCheckBox;
            private CheckBox leftTopCheckBox;
            private CheckBox rightTopCheckBox;
            private CheckBox rightBottomCheckBox;
            private CheckBox leftBottomCheckBox;
            private Label splitterLabel;
            private bool allChecked;
            private bool noneChecked;

            /// <summary>
            /// Constructor
            /// </summary>
            public UICornerRadiusSidesUI()
            {
                End();
                InitializeComponent();
                Size = PreferredSize;
            }

            /// <summary>
            /// Initialize value
            /// </summary>
            /// <param name="value">Value</param>
            protected override void InitValue(object value)
            {
                UICornerRadiusSides sides = (UICornerRadiusSides)value;
                ResetCheckBoxState();
                if ((sides & UICornerRadiusSides.All) == UICornerRadiusSides.All)
                {
                    allCheckBox.Checked = true;
                    leftTopCheckBox.Checked = true;
                    rightTopCheckBox.Checked = true;
                    rightBottomCheckBox.Checked = true;
                    leftBottomCheckBox.Checked = true;
                    allCheckBox.Checked = true;
                }
                else
                {
                    noneCheckBox.Checked = sides == UICornerRadiusSides.None;
                    leftTopCheckBox.Checked = (sides & UICornerRadiusSides.LeftTop) == UICornerRadiusSides.LeftTop;
                    rightTopCheckBox.Checked = (sides & UICornerRadiusSides.RightTop) == UICornerRadiusSides.RightTop;
                    rightBottomCheckBox.Checked = (sides & UICornerRadiusSides.RightBottom) == UICornerRadiusSides.RightBottom;
                    leftBottomCheckBox.Checked = (sides & UICornerRadiusSides.LeftBottom) == UICornerRadiusSides.LeftBottom;
                }
            }

            /// <summary>
            /// Update current value
            /// </summary>
            protected override void UpdateCurrentValue()
            {
                if (!updateCurrentValue)
                    return;
                UICornerRadiusSides labelBorderSides = UICornerRadiusSides.None;
                if (allCheckBox.Checked)
                {
                    currentValue = labelBorderSides | UICornerRadiusSides.All;
                    allChecked = true;
                    noneChecked = false;
                }
                else
                {
                    if (noneCheckBox.Checked)
                        labelBorderSides |= UICornerRadiusSides.None;

                    if (leftTopCheckBox.Checked)
                        labelBorderSides |= UICornerRadiusSides.LeftTop;

                    if (rightTopCheckBox.Checked)
                        labelBorderSides |= UICornerRadiusSides.RightTop;

                    if (rightBottomCheckBox.Checked)
                        labelBorderSides |= UICornerRadiusSides.RightBottom;

                    if (leftBottomCheckBox.Checked)
                        labelBorderSides |= UICornerRadiusSides.LeftBottom;

                    if (labelBorderSides == UICornerRadiusSides.None)
                    {
                        allChecked = false;
                        noneChecked = true;
                        noneCheckBox.Checked = true;
                    }

                    if (labelBorderSides == UICornerRadiusSides.All)
                    {
                        allChecked = true;
                        noneChecked = false;
                        allCheckBox.Checked = true;
                    }

                    currentValue = labelBorderSides;
                }
            }

            /// <summary>
            /// Activate
            /// </summary>
            /// <param name="e"></param>
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                noneCheckBox.Focus();
            }

            private void InitializeComponent()
            {
                ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BorderSidesEditor));
                Height = 163;
                tableLayoutPanel1 = new TableLayoutPanel();
                noneCheckBox = new CheckBox();
                allCheckBox = new CheckBox();
                leftTopCheckBox = new CheckBox();
                rightTopCheckBox = new CheckBox();
                leftBottomCheckBox = new CheckBox();
                rightBottomCheckBox = new CheckBox();
                splitterLabel = new Label();
                tableLayoutPanel1.SuspendLayout();
                SuspendLayout();
                componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
                tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                tableLayoutPanel1.BackColor = SystemColors.Window;
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
                tableLayoutPanel1.Controls.Add(noneCheckBox, 0, 0);
                tableLayoutPanel1.Controls.Add(allCheckBox, 0, 2);
                tableLayoutPanel1.Controls.Add(leftTopCheckBox, 0, 3);
                tableLayoutPanel1.Controls.Add(rightTopCheckBox, 0, 4);
                tableLayoutPanel1.Controls.Add(leftBottomCheckBox, 0, 5);
                tableLayoutPanel1.Controls.Add(rightBottomCheckBox, 0, 6);
                tableLayoutPanel1.Controls.Add(splitterLabel, 0, 1);
                tableLayoutPanel1.Name = "tableLayoutPanel1";
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.RowStyles.Add(new RowStyle());
                tableLayoutPanel1.Margin = new Padding(0);

                noneCheckBox.Name = "noneCheckBox";
                noneCheckBox.Margin = new Padding(3, 3, 3, 1);
                noneCheckBox.Text = "无(&N)";
                noneCheckBox.Height = 20;

                allCheckBox.Name = "allCheckBox";
                allCheckBox.Margin = new Padding(3, 3, 3, 1);
                allCheckBox.Text = "全部(&A)";
                allCheckBox.Height = 20;

                leftTopCheckBox.Margin = new Padding(20, 1, 3, 1);
                leftTopCheckBox.Name = "leftTopCheckBox";
                leftTopCheckBox.Text = "左上角(&LT)";
                leftTopCheckBox.Height = 20;

                rightTopCheckBox.Margin = new Padding(20, 1, 3, 1);
                rightTopCheckBox.Name = "rightTopCheckBox";
                rightTopCheckBox.Text = "右上角(&RT)";
                rightTopCheckBox.Height = 20;

                leftBottomCheckBox.Margin = new Padding(20, 1, 3, 1);
                leftBottomCheckBox.Name = "leftBottomCheckBox";
                leftBottomCheckBox.Text = "左下角(&LB)";
                leftBottomCheckBox.Height = 20;

                rightBottomCheckBox.Margin = new Padding(20, 1, 3, 1);
                rightBottomCheckBox.Name = "rightBottomCheckBox";
                rightBottomCheckBox.Text = "右下角(&RB)";
                rightBottomCheckBox.Height = 20;

                splitterLabel.BackColor = SystemColors.ControlDark;
                splitterLabel.Name = "splitterLabel";
                splitterLabel.Height = 1;

                Controls.Add(tableLayoutPanel1);
                Padding = new Padding(1, 1, 1, 1);
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                AutoScaleMode = AutoScaleMode.None;
                AutoScaleDimensions = new SizeF(6f, 13f);
                tableLayoutPanel1.ResumeLayout(false);
                tableLayoutPanel1.PerformLayout();
                ResumeLayout(false);
                PerformLayout();
                leftBottomCheckBox.CheckedChanged += leftBottomCheckBox_CheckedChanged;
                rightBottomCheckBox.CheckedChanged += rightBottomCheckBox_CheckedChanged;
                rightTopCheckBox.CheckedChanged += rightTopCheckBox_CheckedChanged;
                leftTopCheckBox.CheckedChanged += leftTopBox_CheckedChanged;
                noneCheckBox.CheckedChanged += noneCheckBox_CheckedChanged;
                allCheckBox.CheckedChanged += allCheckBox_CheckedChanged;
                noneCheckBox.Click += noneCheckBoxClicked;
                allCheckBox.Click += allCheckBoxClicked;
            }

            private void leftBottomCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                if (((CheckBox)sender).Checked)
                    noneCheckBox.Checked = false;
                else if (allCheckBox.Checked)
                    allCheckBox.Checked = false;
                UpdateCurrentValue();
            }

            private void rightBottomCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                if (((CheckBox)sender).Checked)
                    noneCheckBox.Checked = false;
                else if (allCheckBox.Checked)
                    allCheckBox.Checked = false;
                UpdateCurrentValue();
            }

            private void rightTopCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                if (((CheckBox)sender).Checked)
                    noneCheckBox.Checked = false;
                else if (allCheckBox.Checked)
                    allCheckBox.Checked = false;
                UpdateCurrentValue();
            }

            private void leftTopBox_CheckedChanged(object sender, EventArgs e)
            {
                if (((CheckBox)sender).Checked)
                    noneCheckBox.Checked = false;
                else if (allCheckBox.Checked)
                    allCheckBox.Checked = false;
                UpdateCurrentValue();
            }

            private void noneCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                if (((CheckBox)sender).Checked)
                {
                    allCheckBox.Checked = false;
                    leftTopCheckBox.Checked = false;
                    rightTopCheckBox.Checked = false;
                    rightBottomCheckBox.Checked = false;
                    leftBottomCheckBox.Checked = false;
                }
                UpdateCurrentValue();
            }

            private void allCheckBox_CheckedChanged(object sender, EventArgs e)
            {
                if (((CheckBox)sender).Checked)
                {
                    noneCheckBox.Checked = false;
                    leftTopCheckBox.Checked = true;
                    rightTopCheckBox.Checked = true;
                    rightBottomCheckBox.Checked = true;
                    leftBottomCheckBox.Checked = true;
                }
                UpdateCurrentValue();
            }

            private void noneCheckBoxClicked(object sender, EventArgs e)
            {
                if (!noneChecked)
                    return;
                noneCheckBox.Checked = true;
            }

            private void allCheckBoxClicked(object sender, EventArgs e)
            {
                if (!allChecked)
                    return;
                allCheckBox.Checked = true;
            }

            private void ResetCheckBoxState()
            {
                allCheckBox.Checked = false;
                noneCheckBox.Checked = false;
                leftTopCheckBox.Checked = false;
                rightTopCheckBox.Checked = false;
                rightBottomCheckBox.Checked = false;
                leftBottomCheckBox.Checked = false;
            }
        }
    }

    /// <summary>
    /// Helper class for corner position property editor
    /// </summary>
    public static class UICornerRadiusSidesHelper
    {
        /// <summary>
        /// Check if the corner is set
        /// </summary>
        /// <param name="sides">All corners</param>
        /// <param name="side">Current corner</param>
        /// <returns>Whether the current corner is set</returns>
        public static bool GetValue(this UICornerRadiusSides sides, UICornerRadiusSides side)
        {
            return (sides & side) == side;
        }

        /// <summary>
        /// Check if the border is set
        /// </summary>
        /// <param name="sides">All borders</param>
        /// <param name="side">Current border</param>
        /// <returns>Whether the current border is set</returns>
        public static bool GetValue(this ToolStripStatusLabelBorderSides sides, ToolStripStatusLabelBorderSides side)
        {
            return (sides & side) == side;
        }
    }
}