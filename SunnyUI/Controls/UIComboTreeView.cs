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
 * File Name: UIComboTreeView.cs
 * Description: Tree list box
 * Current Version: V3.1
 * Creation Date: 2020-11-11
 *
 * 2021-07-29: V3.0.5 Fixed the issue of SelectedNode=null
 * 2021-11-11: V3.0.0 Added file description
 * 2022-05-15: V3.0.8 When displaying CheckBoxes, the text of the selected node can be toggled
 * 2022-06-16: V3.2.0 Added dropdown width and height
 * 2022-07-12: V3.2.1 Fixed the issue of expanding child nodes when CanSelectRootNode is true
 * 2022-11-30: V3.3.0 Added Clear method
 * 2023-02-04: V3.3.1 Added a select all checkbox in the dropdown
 * 2023-04-02: V3.3.4 Added clear button
 * 2023-06-12: V3.3.8 Fixed the issue where the previous selection still exists after using the clear button and reopening the dropdown
 * 2024-03-22: V3.6.5 Added ShowDropDown()
 * 2024-07-13: V3.6.7 Modified the select all button in the dropdown to follow the theme, and modified a built-in international translation
 * 2024-11-10: V3.7.2 Added StyleDropDown property, set this property to modify the dropdown theme when manually changing the Style
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("NodeSelected")]
    [DefaultProperty("Nodes")]
    [ToolboxItem(true)]
    public class UIComboTreeView : UIDropControl, IToolTip
    {
        public UIComboTreeView()
        {
            InitializeComponent();
            fullControlSelect = true;
            CreateInstance();
            DropDownWidth = 250;
            DropDownHeight = 220;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIComboTreeView
            // 
            DropDownStyle = UIDropDownStyle.DropDownList;
            Name = "UIComboTreeView";
            ButtonClick += UIComboTreeView_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            item?.Dispose();
            base.Dispose(disposing);
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        [DefaultValue(false)]
        [Description("Show clear button"), Category("SunnyUI")]
        public bool ShowClearButton
        {
            get => showClearButton;
            set => showClearButton = value;
        }

        [DefaultValue(true)]
        [Description("Show select all checkbox in dropdown"), Category("SunnyUI")]
        public bool ShowSelectedAllCheckBox { get; set; } = true;

        [DefaultValue(250)]
        [Description("Dropdown width"), Category("SunnyUI")]
        public int DropDownWidth { get; set; }

        [DefaultValue(220)]
        [Description("Dropdown height"), Category("SunnyUI")]
        public int DropDownHeight { get; set; }

        /// <summary>
        /// Control that needs additional ToolTip settings
        /// </summary>
        /// <returns>Control</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        public override void Clear()
        {
            base.Clear();
            TreeView.Nodes.Clear();
        }

        [Browsable(false)]
        public UITreeView TreeView => item.TreeView;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Description("Display node collection"), Category("SunnyUI")]
        public TreeNodeCollection Nodes => item.TreeView.Nodes;

        [DefaultValue(false), Description("Show checkboxes, this property is mutually exclusive with CanSelectRootNode"), Category("SunnyUI")]
        public bool CheckBoxes
        {
            get => item.CheckBoxes;
            set
            {
                item.CheckBoxes = value;
                if (value)
                {
                    CanSelectRootNode = false;
                }
            }
        }

        [DefaultValue(false), Description("Can select root node in single selection mode, this property is mutually exclusive with CheckBoxes"), Category("SunnyUI")]
        public bool CanSelectRootNode
        {
            get => item.CanSelectRootNode;
            set
            {
                item.CanSelectRootNode = value;
                if (value)
                {
                    CheckBoxes = false;
                }
            }
        }

        [DefaultValue(false), Description("Show lines"), Category("SunnyUI")]
        public bool ShowLines
        {
            get => item.TreeView.ShowLines;
            set => item.TreeView.ShowLines = value;
        }

        private readonly UIComboTreeViewItem item = new UIComboTreeViewItem();

        /// <summary>
        /// Create object
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        [Browsable(false), DefaultValue(null)]
        public TreeNode SelectedNode
        {
            get => item.TreeView.SelectedNode;
            set
            {
                item.TreeView.SelectedNode = value;
                Text = value?.Text;
            }
        }

        public delegate void OnNodeSelected(object sender, TreeNode node);
        public delegate void OnNodesSelected(object sender, TreeNodeCollection nodes);

        public event OnNodeSelected NodeSelected;
        public event OnNodesSelected NodesSelected;

        /// <summary>
        /// Value changed event
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="value">Value</param>
        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            if (!CheckBoxes)
            {
                TreeNode node = (TreeNode)value;
                Text = node.Text;
                NodeSelected?.Invoke(this, node);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (TreeNode node in Nodes)
                {
                    if (node.Checked) sb.Append(node.Text + "; ");
                    AddChildNodeText(node, sb);
                }

                Text = sb.ToString();
                NodesSelected?.Invoke(this, Nodes);
            }

            Invalidate();
        }

        private void AddChildNodeText(TreeNode node, StringBuilder sb)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                        sb.Append(child.Text + "; ");

                    if (child.Nodes.Count > 0)
                        AddChildNodeText(child, sb);
                }
            }
        }

        public void ShowDropDown()
        {
            UIComboTreeView_ButtonClick(this, EventArgs.Empty);
        }

        private void UIComboTreeView_ButtonClick(object sender, EventArgs e)
        {
            if (NeedDrawClearButton)
            {
                NeedDrawClearButton = false;
                Text = "";
                TreeView.SelectedNode = null;
                TreeView.UnCheckedAll();
                Invalidate();
                return;
            }

            ItemForm.Size = ItemSize;
            //item.TreeView.ExpandAll();
            item.CanSelectRootNode = CanSelectRootNode;
            item.Translate();
            item.SetDPIScale();
            //ItemForm.Show(this);
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;
            int width = DropDownWidth < Width ? Width : DropDownWidth;
            width = Math.Max(250, width);
            item.ShowSelectedAllCheckBox = ShowSelectedAllCheckBox;
            ItemForm.Show(this, new Size(width, DropDownHeight));
        }

        [DefaultValue(typeof(Size), "250, 220"), Description("Dropdown interface size"), Category("SunnyUI")]
        public Size ItemSize { get; set; } = new Size(250, 220);
    }
}
