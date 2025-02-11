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
 * File Name: UILoginForm.cs
 * File Description: Base class for login form
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2023-04-19: V3.3.5 Added optional control to activate on display
******************************************************************************/

using Sunny.UI.Properties;
using System;
using System.ComponentModel;

namespace Sunny.UI
{
    [DefaultProperty("Text")]
    [DefaultEvent("OnLogin")]
    public partial class UILoginForm : UIForm
    {
        public UILoginForm()
        {
            InitializeComponent();
            lblSubText.Text = lblSubText.Version;
        }

        [Description("Control to activate on display"), Category("SunnyUI")]
        [DefaultValue(UILoginFormFocusControl.UserName)]
        public UILoginFormFocusControl FocusControl { get; set; } = UILoginFormFocusControl.UserName;

        private void UILoginForm_Shown(object sender, EventArgs e)
        {
            switch (FocusControl)
            {
                case UILoginFormFocusControl.UserName:
                    edtUser.Focus();
                    break;
                case UILoginFormFocusControl.Password:
                    edtPassword.Focus();
                    break;
                case UILoginFormFocusControl.ButtonLogin:
                    btnLogin.Focus();
                    break;
                case UILoginFormFocusControl.ButtonCancel:
                    btnCancel.Focus();
                    break;
            }
        }

        [Description("Title"), Category("SunnyUI")]
        [DefaultValue("SunnyUI.Net")]
        public string Title
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }

        [Description("Description"), Category("SunnyUI")]
        [DefaultValue("SunnyUI")]
        public string SubText
        {
            get => lblSubText.Text;
            set => lblSubText.Text = value;
        }

        [Description("User login"), Category("SunnyUI")]
        [DefaultValue("User login")]
        public string LoginText
        {
            get => uiLine1.Text;
            set => uiLine1.Text = value;
        }

        private UILoginImage loginImage;

        [DefaultValue(UILoginImage.Login1)]
        [Description("Background image"), Category("SunnyUI")]
        public UILoginImage LoginImage
        {
            get => loginImage;
            set
            {
                if (loginImage != value)
                {
                    loginImage = value;

                    if (loginImage == UILoginImage.Login1) BackgroundImage = Resources.Login1;
                    if (loginImage == UILoginImage.Login2) BackgroundImage = Resources.Login2;
                    if (loginImage == UILoginImage.Login3) BackgroundImage = Resources.Login3;
                    if (loginImage == UILoginImage.Login4) BackgroundImage = Resources.Login4;
                    if (loginImage == UILoginImage.Login5) BackgroundImage = Resources.Login5;
                    if (loginImage == UILoginImage.Login6) BackgroundImage = Resources.Login6;
                }
            }
        }

        public enum UILoginImage
        {
            Login1,
            Login2,
            Login3,
            Login4,
            Login5,
            Login6
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ButtonCancelClick != null)
                ButtonCancelClick?.Invoke(this, e);
            else
                Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (ButtonLoginClick != null)
            {
                ButtonLoginClick?.Invoke(this, e);
            }
            else
            {
                IsLogin = OnLogin != null && OnLogin(edtUser.Text, edtPassword.Text);
                if (IsLogin) Close();
            }
        }

        protected override void SetStyle(UIStyle style)
        {
            base.SetStyle(style);
            uiLine1.SetStyleColor(style.Colors());
            uiLine1.FillColor = UIColor.White;
        }

        [Description("Confirm button event"), Category("SunnyUI")]
        public event EventHandler ButtonLoginClick;

        [Description("Cancel button event"), Category("SunnyUI")]
        public event EventHandler ButtonCancelClick;

        [DefaultValue(false), Browsable(false)]
        public bool IsLogin { get; protected set; }

        public delegate bool OnLoginHandle(string userName, string password);

        [Description("Login validation event"), Category("SunnyUI")]
        public event OnLoginHandle OnLogin;

        [DefaultValue(null)]
        [Description("Account"), Category("SunnyUI")]
        public string UserName
        {
            get => edtUser.Text;
            set => edtUser.Text = value;
        }

        [DefaultValue(null)]
        [Description("Password"), Category("SunnyUI")]
        public string Password
        {
            get => edtPassword.Text;
            set => edtPassword.Text = value;
        }

        [DefaultValue("Please enter account")]
        [Description("Account watermark text"), Category("SunnyUI")]
        public string UserNameWatermark
        {
            get => edtUser.Watermark;
            set => edtUser.Watermark = value;
        }

        [DefaultValue(null)]
        [Description("Please enter password"), Category("SunnyUI")]
        public string PasswordWatermark
        {
            get => edtPassword.Watermark;
            set => edtPassword.Watermark = value;
        }

        [DefaultValue(null)]
        [Description("Login"), Category("SunnyUI")]
        public string ButtonLoginText
        {
            get => btnLogin.Text;
            set => btnLogin.Text = value;
        }

        [DefaultValue(null)]
        [Description("Cancel"), Category("SunnyUI")]
        public string ButtonCancelText
        {
            get => btnCancel.Text;
            set => btnCancel.Text = value;
        }
    }
}