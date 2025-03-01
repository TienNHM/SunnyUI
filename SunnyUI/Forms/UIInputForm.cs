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
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIInputForm.cs
 * File Description: Input form
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2024-05-30: V3.6.6 Fixed issue where custom theme parameters did not work when called
******************************************************************************/

namespace Sunny.UI
{
    public sealed partial class UIInputForm : UIEditForm
    {
        public UIInputForm()
        {
            InitializeComponent();
        }

        public int MaxLength
        {
            get => edit.MaxLength;
            set => edit.MaxLength = value;
        }

        public UITextBox Editor => edit;

        public UILabel Label => label;

        public bool CheckInputEmpty
        {
            get; set;
        }

        protected override bool CheckData()
        {
            Editor.CheckMaxMin();

            if (CheckInputEmpty)
            {
                bool result = edit.Text.IsValid();
                if (!result) this.ShowWarningDialog(UIStyles.CurrentResources.EditorCantEmpty);
                return result;
            }

            return true;
        }

        protected override void DoEnter()
        {
            if (btnCancel.Focused || btnOK.Focused) return;
            btnOK.PerformClick();
        }

        private void UIInputForm_Shown(object sender, System.EventArgs e)
        {
            edit.SelectAll();
        }

        /// <summary>
        /// Set theme style
        /// </summary>
        /// <param name="uiColor">Theme style</param>
        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            if (btnCancel != null)
            {
                btnCancel.SetStyleColor(uiColor);
            }

            if (label != null)
            {
                label.SetStyleColor(uiColor);
            }

            if (btnOK != null)
            {
                btnOK.SetStyleColor(uiColor);
            }

            if (pnlBtm != null)
            {
                pnlBtm.SetStyleColor(uiColor);
            }

            if (edit != null)
            {
                edit.SetStyleColor(uiColor);
            }
        }
    }
}