/******************************************************************************
* SunnyUI open source control library, tool library, extension library, multi-page development framework.
* CopyRight (C) 2012-2025 ShenYongHua (沈永华).
* QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
*
* Blog: https://www.cnblogs.com/yhuse
* Gitee: https://gitee.com/yhuse/SunnyUI
* GitHub: https://github.com/yhuse/SunnyUI
*
* SunnyUI.dll can be used for free under the GPL-3.0 license.
* If you use this code, please keep this note.
* If you use this code, please keep this note.
**********************************************************************************
* File name: UIWaitForm.cs
* File description: Waiting form
* Current version: V3.1
* Creation date: 2020-10-13
*
* 2020-10-13: V3.0.0 Added file description
**************************************************************************/

namespace Sunny.UI
{
    public sealed partial class UIWaitForm : UIForm
    {
        public UIWaitForm()
        {
            InitializeComponent();
            base.Text = UIStyles.CurrentResources.InfoTitle;
            SetDescription(UIStyles.CurrentResources.SystemProcessing);
        }

        public UIWaitForm(string desc)
        {
            InitializeComponent();
            base.Text = UIStyles.CurrentResources.InfoTitle;
            SetDescription(desc);
        }

        private delegate void SetTextHandler(string text);

        public void SetDescription(string text)
        {
            if (labelDescription.InvokeRequired)
            {
                Invoke(new SetTextHandler(SetDescription), text);
            }
            else
            {
                labelDescription.Text = text;
                labelDescription.Invalidate();
            }
        }

        private void Bar_Tick(object sender, System.EventArgs e)
        {
            if (UIFormServiceHelper.WaitFormServiceClose)
            {
                UIFormServiceHelper.WaitFormServiceClose = false;
                Close();
            }
        }
    }
}
