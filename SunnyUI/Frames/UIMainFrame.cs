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
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * File Name: UIMainFrame.cs
 * File Description: Base class for page framework
 * Current Version: V3.1
 * Creation Date: 2020-05-05
 *
 * 2020-05-05: V2.2.5 Base class for page framework
 * 2021-08-17: V3.0.8 Removed IFrame interface, moved to parent class UIForm
 * 2022-05-17: V3.1.9 Fixed the issue of showing page close button and error when removing the last page
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sunny.UI
{
    public partial class UIMainFrame : UIForm
    {
        public UIMainFrame()
        {
            InitializeComponent();
            MainContainer.TabVisible = false;
            MainContainer.BringToFront();
            MainContainer.TabPages.Clear();

            MainContainer.BeforeRemoveTabPage += MainContainer_BeforeRemoveTabPage;
            MainContainer.AfterRemoveTabPage += MainContainer_AfterRemoveTabPage;
        }

        private void MainContainer_AfterRemoveTabPage(object sender, int index)
        {
            AfterRemoveTabPage?.Invoke(this, index);
        }

        private bool MainContainer_BeforeRemoveTabPage(object sender, int index)
        {
            return BeforeRemoveTabPage == null || BeforeRemoveTabPage.Invoke(this, index);
        }

        public event UITabControl.OnBeforeRemoveTabPage BeforeRemoveTabPage;

        public event UITabControl.OnAfterRemoveTabPage AfterRemoveTabPage;


        protected override void OnShown(EventArgs e)
        {
            MainContainer.BringToFront();
            base.OnShown(e);
        }

        public bool TabVisible
        {
            get => MainContainer.TabVisible;
            set => MainContainer.TabVisible = value;
        }

        public bool TabShowCloseButton
        {
            get => MainContainer.ShowCloseButton;
            set => MainContainer.ShowCloseButton = value;
        }

        public bool TabShowActiveCloseButton
        {
            get => MainContainer.ShowActiveCloseButton;
            set => MainContainer.ShowActiveCloseButton = value;
        }

        private void MainContainer_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Selecting != null && e.TabPage != null)
            {
                List<UIPage> pages = e.TabPage.GetControls<UIPage>();
                Selecting?.Invoke(this, e, pages.Count == 0 ? null : pages[0]);
            }
        }

        public delegate void OnSelecting(object sender, TabControlCancelEventArgs e, UIPage page);

        [Description("Page selection event"), Category("SunnyUI")]
        public event OnSelecting Selecting;
    }
}