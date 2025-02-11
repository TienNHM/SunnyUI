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
 * File Name: UIMessageBox.cs
 * Description: Message prompt form
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2021-11-09: V3.0.8 Added FocusLine when multiple buttons are displayed
 * 2022-07-13: V3.2.1 Added scrollbar to message prompt text
******************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public sealed partial class UIMessageForm : UIForm
    {
        /// <summary>
        /// Message prompt form
        /// </summary>
        public UIMessageForm()
        {
            InitializeComponent();

            btnOK.Text = UIStyles.CurrentResources.OK;
            btnCancel.Text = UIStyles.CurrentResources.Cancel;
        }

        /// <summary>
        /// Is OK
        /// </summary>
        public bool IsOK
        {
            get; private set;
        }

        private bool _showCancel = true;

        /// <summary>
        /// Show cancel button
        /// </summary>
        public bool ShowCancel
        {
            get => _showCancel;
            set
            {
                _showCancel = value;
                btnCancel.Visible = value;
                OnSizeChanged(null);
            }
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameters</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (btnOK == null || btnCancel == null)
            {
                return;
            }

            if (_showCancel)
            {
                //btnOK.RectSides = ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right;
                //btnOK.RectSides = btnCancel.RectSides = ToolStripStatusLabelBorderSides.All;
                btnOK.Width = btnCancel.Width = Width / 2 - 2;
                btnCancel.Left = btnOK.Left + btnOK.Width - 1;
                btnCancel.Width = Width - btnCancel.Left - 2;
            }
            else
            {
                //btnOK.RectSides = ToolStripStatusLabelBorderSides.Top;
                btnOK.Width = Width - 4;
            }

            btnCancel.Left = btnOK.Right - 1;
        }

        /// <summary>
        /// Enter event
        /// </summary>
        protected override void DoEnter()
        {
            base.DoEnter();
            if (!ShowCancel)
            {
                btnOK_Click(null, null);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            IsOK = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            IsOK = false;
            Close();
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
                btnCancel.FillColor = BackColor;
                btnCancel.RectColor = Color.FromArgb(36, uiColor.ButtonRectColor);
                btnCancel.ForeColor = uiColor.LabelForeColor;
            }

            if (btnOK != null)
            {
                btnOK.SetStyleColor(uiColor);
                btnOK.FillColor = BackColor;
                btnOK.RectColor = Color.FromArgb(36, uiColor.ButtonRectColor);
                btnOK.ForeColor = uiColor.LabelForeColor;
            }

            if (lbMsg != null)
            {
                lbMsg.SetStyleColor(uiColor);
                lbMsg.ForeColor = uiColor.LabelForeColor;
                lbMsg.BackColor = uiColor.PlainColor;
                lbMsg.FillColor = uiColor.PlainColor;
                lbMsg.SelectionColor = RectColor;
                lbMsg.ScrollBarColor = uiColor.RectColor;
            }
        }

        public UIMessageDialogButtons DefaultButton { get; set; } = UIMessageDialogButtons.Ok;

        /// <summary>
        /// Show message prompt form
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="title">Title</param>
        /// <param name="showCancel">Show cancel button</param>
        /// <param name="style">Theme style</param>
        public void ShowMessage(string message, string title, bool showCancel, UIStyle style = UIStyle.Blue)
        {
            Style = style;
            Text = title;
            lbMsg.Text = message;
            ShowCancel = showCancel;
            //btnOK.ShowFocusLine = btnCancel.ShowFocusLine = showCancel;
            btnOK.ShowFocusColor = btnCancel.ShowFocusColor = showCancel;
        }

        private void UIMessageForm_Shown(object sender, EventArgs e)
        {
            if (ShowCancel)
            {
                if (DefaultButton == UIMessageDialogButtons.Ok)
                    btnOK.Focus();
                else
                    btnCancel.Focus();
            }

            if (delay <= 0) return;
            if (text == "") text = Text;
            Text = text + " [" + delay + "]";
        }

        int delay = 0;

        public int Delay
        {
            set
            {
                if (value > 0)
                {
                    delay = value / 1000;
                    timer1.Start();
                }
            }
        }

        string text = "";

        private void timer1_Tick(object sender, EventArgs e)
        {
            delay--;
            Text = text + " [" + delay + "]";

            if (delay <= 0) Close();
        }

        private void UIMessageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
        }
    }
}