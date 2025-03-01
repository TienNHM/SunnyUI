/******************************************************************************
 * SunnyUI open source control library, utility library, extension library, multi-page development framework.
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
 * File Name: UIAsideMainFrame.cs
 * File Description: Page framework (Aside-Main)
 * Current Version: V3.1
 * Creation Date: 2020-05-05
 *
 * 2020-05-05: V2.2.5 Page framework (Aside-Main)
******************************************************************************/

using System.Windows.Forms;

namespace Sunny.UI
{
    public partial class UIAsideMainFrame : UIMainFrame
    {
        public UIAsideMainFrame()
        {
            InitializeComponent();
            Controls.SetChildIndex(MainTabControl, 0);
            Aside.Parent = this;
            MainTabControl.Parent = this;
            MainTabControl.BringToFront();
            Aside.TabControl = MainTabControl;
        }

        public override bool SelectPage(int pageIndex)
        {
            bool result = base.SelectPage(pageIndex);
            TreeNode node = Aside.GetTreeNode(pageIndex);
            if (node != null) Aside.SelectedNode = node;
            return result;
        }
    }
}