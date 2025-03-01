/******************************************************************************
 * SunnyUI Open Source Control Library, Utility Library, Extension Library, 
 * and Multi-Page Development Framework.
 * CopyRight (C) 2012-2025 ShenYongHua.
 * QQ Group: 56829229  QQ: 17612584  EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ****************************************************************************** 
 * File Name: UIFormHelper.cs
 * Description: Form helper class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 * 2020-05-05: V2.2.5 Added dropdown selection box and progress improvement form
 * 2021-05-09: V3.0.3 Added RemovePage interface, changed GetTopMost to native TopMost interface
 * 2021-06-27: V3.0.4 Added a feedback interface, Feedback, Page can send objects to Frame
 * 2021-12-13: V3.0.9 Added full-screen mask and ShowDialogWithMask() extension method for Form
 * 2022-07-17: V3.2.1 Fixed error when multiple ShowNotifier windows were opened and then closed
 * 2023-07-27: V3.4.1 Default prompt pop-up TopMost set to true
 * 2023-07-27: V3.4.1 Added default selection for confirm or cancel button in prompt dialog
 * 2024-04-22: V3.6.5 Refactored, all pop-up windows converted to form extension methods using 'this.'
 * 2024-04-22: V3.6.5 Added "Show" prefix to input dialog
 * 2024-04-27: V3.6.5 Added delayed closing for message box
 * 2024-04-28: V3.6.5 Message prompt window follows the screen where the program is located
 * 2024-05-08: V3.6.6 Default ShowMask for pop-up windows set to false
 * 2024-05-30: V3.6.6 Fixed incorrect pop-up title display
 * 2024-07-30: V3.6.8 Changed default pop-up to be centered on the current form; when showMask=true or centerParent=false, it is centered on the screen
 * 2024-08-09: V3.6.8 Refactored pop-up windows; form extension opens centered on the form by default, removed TopMost parameter, default set to true
 * 2024-08-26: V3.6.9 Fixed ShowAskDialog2 error #IAMA5A
******************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public delegate void OnZoomScaleChanged(object sender, float scale);

    public delegate void OnZoomScaleRectChanged(object sender, Rectangle info);

    public delegate void OnWindowStateChanged(object sender, FormWindowState thisState, FormWindowState lastState);

    public enum UILoginFormFocusControl
    {
        UserName,
        Password,
        ButtonLogin,
        ButtonCancel
    }

    public class UIPageEventArgs : EventArgs
    {
        public UIPage Page { get; set; }

        public UIPageEventArgs()
        {

        }

        public UIPageEventArgs(UIPage page)
        {
            Page = page;
        }
    }

    public class PageDeselectingEventArgs : CancelEventArgs
    {
        private string _cancelReason;

        public PageDeselectingEventArgs(bool cancel, string cancelReason) : base(cancel)
        {
            _cancelReason = cancelReason;
        }

        public string CancelReason => _cancelReason;
    }

    public delegate void PageDeselectingEventHandler(object sender, PageDeselectingEventArgs e);

    public delegate void OnUIPageChanged(object sender, UIPageEventArgs e);

    public static class UIMessageBox
    {
        public static void Show(string message, bool showMask = false, int delay = 0)
        {
            Show(message, UIStyles.CurrentResources.InfoTitle, UIStyle.Blue, UIMessageBoxButtons.OK, showMask, delay);
        }

        public static void ShowInfo(string message, bool showMask = false, int delay = 0)
        {
            Show(message, UIStyles.CurrentResources.InfoTitle, UIStyles.Style, UIMessageBoxButtons.OK, showMask, delay);
        }

        public static void ShowSuccess(string message, bool showMask = false, int delay = 0)
        {
            Show(message, UIStyles.CurrentResources.SuccessTitle, UIStyle.Green, UIMessageBoxButtons.OK, showMask, delay);
        }

        public static void ShowWarning(string message, bool showMask = false, int delay = 0)
        {
            Show(message, UIStyles.CurrentResources.WarningTitle, UIStyle.Orange, UIMessageBoxButtons.OK, showMask, delay);
        }

        public static void ShowError(string message, bool showMask = false, int delay = 0)
        {
            Show(message, UIStyles.CurrentResources.ErrorTitle, UIStyle.Red, UIMessageBoxButtons.OK, showMask, delay);
        }

        public static bool ShowAsk(string message, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Ok)
        {
            return ShowMessageDialog(message, UIStyles.CurrentResources.AskTitle, true, UIStyle.Blue, showMask, defaultButton);
        }

        public static bool Show(string message, string title, UIStyle style = UIStyle.Blue, UIMessageBoxButtons buttons = UIMessageBoxButtons.OK, bool showMask = false, int delay = 0)
        {
            return ShowMessageDialog(message, title, buttons == UIMessageBoxButtons.OKCancel, style, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="showCancelButton">Display cancel button</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Display mask layer</param>
        /// <param name="defaultButton">Default button</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <returns>Result</returns>
        public static bool ShowMessageDialog(string message, string title, bool showCancel, UIStyle style,
            bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Ok, int delay = 0)
        {
            return ShowMessageDialog(null, message, title, showCancel, style, showMask, defaultButton, delay);
        }

        public static void ShowInfo2(string message, bool showMask = false, int delay = 0)
        {
            ShowMessageDialog2(UIStyles.CurrentResources.InfoTitle, message, UINotifierType.INFO, showMask, UIMessageDialogButtons.Ok, delay);
        }

        public static void ShowSuccess2(string message, bool showMask = false, int delay = 0)
        {
            ShowMessageDialog2(UIStyles.CurrentResources.SuccessTitle, message, UINotifierType.OK, showMask, UIMessageDialogButtons.Ok, delay);
        }

        public static void ShowWarning2(string message, bool showMask = false, int delay = 0)
        {
            ShowMessageDialog2(UIStyles.CurrentResources.WarningTitle, message, UINotifierType.WARNING, showMask, UIMessageDialogButtons.Ok, delay);
        }

        public static void ShowError2(string message, bool showMask = false, int delay = 0)
        {
            ShowMessageDialog2(UIStyles.CurrentResources.ErrorTitle, message, UINotifierType.ERROR, showMask, UIMessageDialogButtons.Ok, delay);
        }

        public static bool ShowAsk2(string message, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Ok)
        {
            return ShowMessageDialog2(UIStyles.CurrentResources.AskTitle, message, UINotifierType.Ask, showMask, defaultButton);
        }

        public static bool ShowMessageDialog2(string title, string message, UINotifierType noteType, bool showMask = false,
            UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Cancel, int delay = 0)
        {
            return ShowMessageDialog2(null, title, message, noteType, showMask, defaultButton, delay);
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Information</param>
        /// <param name="showCancel">Show cancel button</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        /// <param name="defaultButton">Default button</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <returns>Result</returns>
        public static bool ShowMessageDialog(Form owner, string message, string title, bool showCancel,
            UIStyle style, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Ok, int delay = 0)
        {
            bool screenCenter = GetShowOnScreenCenter(showMask, owner);
            if (owner == null)
            {
                using UIMessageForm frm = new UIMessageForm();
                frm.ShowMessage(message, title, showCancel, style);
                frm.DefaultButton = showCancel ? defaultButton : UIMessageDialogButtons.Ok;
                frm.Delay = delay;
                return frm.ShowForm(owner, screenCenter, showMask);
            }
            else
            {
                return owner.ThreadSafeCall<bool>(() =>
                {
                    using UIMessageForm frm = new UIMessageForm();
                    frm.ShowMessage(message, title, showCancel, style);
                    frm.DefaultButton = showCancel ? defaultButton : UIMessageDialogButtons.Ok;
                    frm.Delay = delay;
                    return frm.ShowForm(owner, screenCenter, showMask);
                });
            }
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="defaultButton">Default button</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <returns>Result</returns>
        public static bool ShowMessageDialog2(Form owner, string title, string message, UINotifierType noteType, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Cancel, int delay = 0)
        {
            bool screenCenter = GetShowOnScreenCenter(showMask, owner);
            if (owner == null)
            {
                using UIMessageForm2 frm = new UIMessageForm2(title, message, noteType, defaultButton);
                frm.Delay = delay;
                return frm.ShowForm(owner, screenCenter, showMask);
            }
            else
            {
                return owner.ThreadSafeCall<bool>(() =>
                {
                    using UIMessageForm2 frm = new UIMessageForm2(title, message, noteType, defaultButton);
                    frm.Delay = delay;
                    return frm.ShowForm(owner, screenCenter, showMask);
                });
            }
        }

        private static bool GetShowOnScreenCenter(bool showMask, Form owner)
        {
            if (showMask) return true;
            if (owner == null) return true;
            if (owner.TopLevel) return false;
            if (owner.ParentForm == null) return true;
            return false;
        }

        internal static bool ShowForm(this UIForm frm, Form owner, bool screenCenter, bool showMask)
        {
            frm.Owner = owner;
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = frm.Size.GetLocation(owner, screenCenter);
            frm.ShowInTaskbar = false;
            frm.TopMost = true;
            frm.Render();

            if (showMask)
                return frm.ShowDialogWithMask() == DialogResult.OK;
            else
                return frm.ShowDialog() == DialogResult.OK;
        }

        private static Point GetLocation(this Size size, Form owner, bool screenCenter)
        {
            Rectangle screen = Screen.GetBounds(SystemEx.GetCursorPos());
            if (screenCenter || owner == null)
            {
                return new Point(screen.Left + screen.Width / 2 - size.Width / 2, screen.Top + screen.Height / 2 - size.Height / 2);
            }
            else
            {
                Form form = owner.TopLevel ? owner : owner.ParentForm;
                if (form == null)
                    return new Point(screen.Left + screen.Width / 2 - size.Width / 2, screen.Top + screen.Height / 2 - size.Height / 2);
                else
                    return new Point(form.Left + form.Width / 2 - size.Width / 2, form.Top + form.Height / 2 - size.Height / 2);
            }
        }
    }

    public static class UIInputDialog
    {
        private static UIInputForm CreateInputForm(bool checkEmpty, string desc)
        {
            UIInputForm frm = new UIInputForm();
            frm.Text = UIStyles.CurrentResources.InputTitle;
            frm.Label.Text = desc;
            frm.CheckInputEmpty = checkEmpty;
            return frm;
        }

        public static bool ShowInputStringDialog(ref string value, UIStyle style, bool checkEmpty = true, string desc = "请输入字符串：", bool showMask = false, int maxLength = 1000)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Text = value;
            frm.Editor.MaxLength = maxLength;

            frm.Style = style;
            frm.ShowForm(null, true, showMask);

            if (frm.IsOK) value = frm.Editor.Text;
            return frm.IsOK;
        }

        public static bool ShowInputStringDialog(this Form owner, ref string value, bool checkEmpty = true, string desc = "请输入字符串：", bool showMask = false, int maxLength = 1000)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Text = value;
            frm.Editor.MaxLength = maxLength;

            frm.Style = UIStyles.Style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) value = frm.Editor.Text;
            return frm.IsOK;
        }

        public static bool ShowInputPasswordDialog(ref string value, UIStyle style, bool checkEmpty = true, string desc = "请输入密码：", bool showMask = false, int maxLength = 1000)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.PasswordChar = '*';
            frm.Editor.MaxLength = maxLength;

            frm.Style = style;
            frm.ShowForm(null, true, showMask);

            if (frm.IsOK) value = frm.Editor.Text;
            return frm.IsOK;
        }

        public static bool ShowInputPasswordDialog(this Form owner, ref string value, bool checkEmpty = true, string desc = "请输入密码：", bool showMask = false, int maxLength = 1000)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.PasswordChar = '*';
            frm.Editor.MaxLength = maxLength;

            frm.Style = UIStyles.Style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) value = frm.Editor.Text;
            return frm.IsOK;
        }

        public static bool ShowInputIntegerDialog(ref int value, UIStyle style, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Integer;
            frm.Editor.IntValue = value;
            frm.Editor.MaxLength = 11;

            frm.Style = style;
            frm.ShowForm(null, true, showMask);

            if (frm.IsOK) value = frm.Editor.IntValue;
            return frm.IsOK;
        }

        public static bool ShowInputIntegerDialog(ref int value, UIStyle style, int minimum, int maximum, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Integer;
            frm.Editor.IntValue = value;
            frm.Editor.MaxLength = 11;
            frm.Editor.Minimum = minimum;
            frm.Editor.Maximum = maximum;

            frm.Style = style;
            frm.ShowForm(null, true, showMask);

            if (frm.IsOK) value = frm.Editor.IntValue;
            return frm.IsOK;
        }

        public static bool ShowInputIntegerDialog(this Form owner, ref int value, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Integer;
            frm.Editor.IntValue = value;
            frm.Editor.MaxLength = 11;

            frm.Style = UIStyles.Style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) value = frm.Editor.IntValue;
            return frm.IsOK;
        }

        public static bool ShowInputIntegerDialog(this Form owner, ref int value, int minimum, int maximum, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Integer;
            frm.Editor.IntValue = value;
            frm.Editor.MaxLength = 11;
            frm.Editor.Minimum = minimum;
            frm.Editor.Maximum = maximum;

            frm.Style = UIStyles.Style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) value = frm.Editor.IntValue;
            return frm.IsOK;
        }

        public static bool ShowInputDoubleDialog(ref double value, UIStyle style, int decimals = 2, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Double;
            frm.Editor.DecimalPlaces = decimals;
            frm.Editor.DoubleValue = value;

            frm.Style = style;
            frm.ShowForm(null, true, showMask);

            if (frm.IsOK) value = frm.Editor.DoubleValue;
            return frm.IsOK;
        }

        public static bool ShowInputDoubleDialog(ref double value, UIStyle style, double minimum, double maximum, int decimals = 2, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Double;
            frm.Editor.DecimalPlaces = decimals;
            frm.Editor.DoubleValue = value;
            frm.Editor.Minimum = minimum;
            frm.Editor.Maximum = maximum;

            frm.Style = style;
            frm.ShowForm(null, true, showMask);

            if (frm.IsOK) value = frm.Editor.DoubleValue;
            return frm.IsOK;
        }

        public static bool ShowInputDoubleDialog(this Form owner, ref double value, int decimals = 2, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Double;
            frm.Editor.DecimalPlaces = decimals;
            frm.Editor.DoubleValue = value;

            frm.Style = UIStyles.Style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) value = frm.Editor.DoubleValue;
            return frm.IsOK;
        }

        public static bool ShowInputDoubleDialog(this Form owner, ref double value, double minimum, double maximum, int decimals = 2, bool checkEmpty = true, string desc = "请输入数字：", bool showMask = false)
        {
            using var frm = CreateInputForm(checkEmpty, desc);

            frm.Editor.Type = UITextBox.UIEditType.Double;
            frm.Editor.DecimalPlaces = decimals;
            frm.Editor.DoubleValue = value;
            frm.Editor.Minimum = minimum;
            frm.Editor.Maximum = maximum;

            frm.Style = UIStyles.Style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) value = frm.Editor.DoubleValue;
            return frm.IsOK;
        }
    }

    public static class UISelectDialog
    {
        public static bool ShowSelectDialog(this Form owner, ref int selectIndex, IList items, string title, string description, UIStyle style, bool showMask = false)
        {
            using UISelectForm frm = new UISelectForm();

            frm.SetItems(items);
            frm.SelectedIndex = selectIndex;
            if (title.IsValid()) frm.Title = title;
            if (description.IsValid()) frm.Description = description;

            frm.Style = style;
            frm.ShowForm(owner, showMask || owner == null, showMask);

            if (frm.IsOK) selectIndex = frm.SelectedIndex;
            return frm.IsOK;
        }

        public static bool ShowSelectDialog(this Form owner, ref int selectIndex, IList items, string title, string description, bool showMask = false)
        {
            return owner.ShowSelectDialog(ref selectIndex, items, title, description, UIStyles.Style, showMask);
        }

        public static bool ShowSelectDialog(this Form owner, ref int selectIndex, IList items, UIStyle style, bool showMask = false)
        {
            return owner.ShowSelectDialog(ref selectIndex, items, UIStyles.CurrentResources.SelectTitle, "", style, showMask);
        }

        public static bool ShowSelectDialog(this Form owner, ref int selectIndex, IList items, bool showMask = false)
        {
            return owner.ShowSelectDialog(ref selectIndex, items, UIStyles.Style, showMask);
        }
    }

    public static class UINotifierHelper
    {
        public static void ShowNotifier(string desc, UINotifierType type = UINotifierType.INFO, string title = "Notifier", bool isDialog = false, int timeout = 0, Form inApp = null)
        {
            UINotifier.Show(desc, type, title, isDialog, timeout, inApp);
        }

        public static void ShowNotifier(string desc, EventHandler<DescriptionEventArgs> clickEvent, UINotifierType type = UINotifierType.INFO, string title = "Notifier", int timeout = 0)
        {
            UINotifier.Show(desc, type, title, false, timeout, null, clickEvent);
        }
    }

    public enum UIMessageDialogButtons
    {
        Ok,
        Cancel
    }

    public enum UIMessageBoxButtons
    {
        OK = 0,
        OKCancel = 1
    }

    public static class FormEx
    {
        internal class FMask : Form
        {
            public FMask()
            {
                this.SuspendLayout();
                this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
                this.BackColor = Color.Black;
                this.ClientSize = new System.Drawing.Size(800, 450);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Name = "FMask";
                this.Opacity = 0.5D;
                this.ShowInTaskbar = false;
                this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                this.Text = "FMask";
                this.TopMost = true;
                this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FMask_MouseClick);
                this.ResumeLayout(false);
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                int num = 256;
                int num2 = 260;
                if (msg.Msg == num | msg.Msg == num2)
                {
                    if (keyData == Keys.Escape)
                    {
                        Close();
                    }
                }

                return base.ProcessCmdKey(ref msg, keyData);
            }

            private void FMask_MouseClick(object sender, MouseEventArgs e)
            {
                Close();
            }

            private System.ComponentModel.IContainer components = null;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        public static DialogResult ShowDialogWithMask(this Form form)
        {
            Point pt = SystemEx.GetCursorPos();
            Rectangle screen = Screen.GetBounds(pt);
            using FMask mask = new FMask();
            mask.Bounds = screen;
            mask.Show();

            form.ShowInTaskbar = false;
            form.TopMost = true;
            return form.ShowDialog();
        }

        public static UIEditForm CreateForm(this UIEditOption option)
        {
            return new UIEditForm(option);
        }

        /// <summary>
        /// Sets the form's rounded rectangle shape
        /// </summary>
        /// <param name="form">The form to set</param>
        /// <param name="rgnRadius">The radius of the rounded rectangle</param>
        public static void SetFormRoundRectRegion(Form form, int rgnRadius)
        {
            if (form != null && form.FormBorderStyle == FormBorderStyle.None)
            {
                int region = Win32.GDI.CreateRoundRectRgn(0, 0, form.Width + 1, form.Height + 1, rgnRadius, rgnRadius);
                Win32.User.SetWindowRgn(form.Handle, region, true);
                Win32.GDI.DeleteObject(region);
            }
        }

        #region Windows Helpers

        /// <summary>
        /// Success message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowSuccessDialog(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowSuccessDialog(UIStyles.CurrentResources.SuccessTitle, message, UIStyle.Green, showMask, delay);
        }

        /// <summary>
        /// Success message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowSuccessDialog(this Form owner, string title, string message, UIStyle style = UIStyle.Green, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog(owner, message, title, false, style, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Information message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowInfoDialog(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowInfoDialog(UIStyles.CurrentResources.InfoTitle, message, UIStyles.Style, showMask, delay);
        }

        /// <summary>
        /// Information message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowInfoDialog(this Form owner, string title, string message, UIStyle style = UIStyle.Gray, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog(owner, message, title, false, style, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Warning message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowWarningDialog(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowWarningDialog(UIStyles.CurrentResources.WarningTitle, message, UIStyle.Orange, showMask, delay);
        }

        /// <summary>
        /// Warning message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowWarningDialog(this Form owner, string title, string message, UIStyle style = UIStyle.Orange, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog(owner, message, title, false, style, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Error message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowErrorDialog(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowErrorDialog(UIStyles.CurrentResources.ErrorTitle, message, UIStyle.Red, showMask, delay);
        }

        /// <summary>
        /// Error message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowErrorDialog(this Form owner, string title, string message, UIStyle style = UIStyle.Red, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog(owner, message, title, false, style, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        /// <returns>Result</returns>
        public static bool ShowAskDialog(this Form owner, string message, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Ok)
        {
            return UIMessageBox.ShowMessageDialog(owner, message, UIStyles.CurrentResources.AskTitle, true, UIStyles.Style, showMask, defaultButton);
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        /// <returns>Result</returns>
        public static bool ShowAskDialog(this Form owner, string title, string message, UIStyle style = UIStyle.Blue, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Ok)
        {
            return UIMessageBox.ShowMessageDialog(owner, message, title, true, style, showMask, defaultButton);
        }

        //---------------

        /// <summary>
        /// Success message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowSuccessDialog2(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowSuccessDialog2(UIStyles.CurrentResources.SuccessTitle, message, showMask, delay);
        }

        /// <summary>
        /// Success message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowSuccessDialog2(this Form owner, string title, string message, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog2(owner, title, message, UINotifierType.OK, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Information message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowInfoDialog2(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowInfoDialog2(UIStyles.CurrentResources.InfoTitle, message, showMask, delay);
        }

        /// <summary>
        /// Information message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowInfoDialog2(this Form owner, string title, string message, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog2(owner, title, message, UINotifierType.INFO, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Warning message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowWarningDialog2(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowWarningDialog2(UIStyles.CurrentResources.WarningTitle, message, showMask, delay);
        }

        /// <summary>
        /// Warning message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowWarningDialog2(this Form owner, string title, string message, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog2(owner, title, message, UINotifierType.WARNING, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Error message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        public static void ShowErrorDialog2(this Form owner, string message, bool showMask = false, int delay = 0)
        {
            owner.ShowErrorDialog2(UIStyles.CurrentResources.ErrorTitle, message, showMask, delay);
        }

        /// <summary>
        /// Error message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        public static void ShowErrorDialog2(this Form owner, string title, string message, bool showMask = false, int delay = 0)
        {
            UIMessageBox.ShowMessageDialog2(owner, title, message, UINotifierType.ERROR, showMask, UIMessageDialogButtons.Ok, delay);
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="showMask">Show mask layer</param>
        /// <returns>Result</returns>
        public static bool ShowAskDialog2(this Form owner, string message, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Cancel)
        {
            return UIMessageBox.ShowMessageDialog2(owner, UIStyles.CurrentResources.AskTitle, message, UINotifierType.Ask, showMask, defaultButton);
        }

        /// <summary>
        /// Confirmation message dialog box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="style">Theme</param>
        /// <param name="showMask">Show mask layer</param>
        /// <returns>Result</returns>
        public static bool ShowAskDialog2(this Form owner, string title, string message, bool showMask = false, UIMessageDialogButtons defaultButton = UIMessageDialogButtons.Cancel)
        {
            return UIMessageBox.ShowMessageDialog2(owner, title, message, UINotifierType.Ask, showMask, defaultButton);
        }
        //---------------

        /// <summary>
        /// Display a message
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowInfoTip(this Form form, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.Show(text, null, delay, floating);

        /// <summary>
        /// Display a success message
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowSuccessTip(this Form form, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.ShowOk(text, delay, floating);

        /// <summary>
        /// Display a warning message
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowWarningTip(this Form form, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.ShowWarning(text, delay, floating);

        /// <summary>
        /// Display an error message
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowErrorTip(this Form form, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.ShowError(text, delay, floating);

        /// <summary>
        /// Display a message near the specified control
        /// </summary>
        /// <param name="controlOrItem">Control or toolbar item</param>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowInfoTip(this Form form, Component controlOrItem, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.Show(controlOrItem, text, null, delay, floating);

        /// <summary>
        /// Display a success message near the specified control
        /// </summary>
        /// <param name="controlOrItem">Control or toolbar item</param>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowSuccessTip(this Form form, Component controlOrItem, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.ShowOk(controlOrItem, text, delay, floating);

        /// <summary>
        /// Display an error message near the specified control
        /// </summary>
        /// <param name="controlOrItem">Control or toolbar item</param>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowErrorTip(this Form form, Component controlOrItem, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.ShowError(controlOrItem, text, delay, floating);

        /// <summary>
        /// Display a warning message near the specified control
        /// </summary>
        /// <param name="controlOrItem">Control or toolbar item</param>
        /// <param name="text">Message text</param>
        /// <param name="delay">Message duration (ms). Default is 1 second</param>
        /// <param name="floating">Whether the message is floating</param>
        public static void ShowWarningTip(this Form form, Component controlOrItem, string text, int delay = 1000, bool floating = true)
            => UIMessageTip.ShowWarning(controlOrItem, text, delay, floating, false);

        public static void ShowInfoNotifier(this Form form, string desc, bool isDialog = false, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, UINotifierType.INFO, UIStyles.CurrentResources.InfoTitle, isDialog, timeout);
        }

        public static void ShowSuccessNotifier(this Form form, string desc, bool isDialog = false, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, UINotifierType.OK, UIStyles.CurrentResources.SuccessTitle, isDialog, timeout);
        }

        public static void ShowWarningNotifier(this Form form, string desc, bool isDialog = false, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, UINotifierType.WARNING, UIStyles.CurrentResources.WarningTitle, isDialog, timeout);
        }

        public static void ShowErrorNotifier(this Form form, string desc, bool isDialog = false, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, UINotifierType.ERROR, UIStyles.CurrentResources.ErrorTitle, isDialog, timeout);
        }

        public static void ShowInfoNotifier(this Form form, string desc, EventHandler<DescriptionEventArgs> clickEvent, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, clickEvent, UINotifierType.INFO, UIStyles.CurrentResources.InfoTitle, timeout);
        }

        public static void ShowSuccessNotifier(this Form form, string desc, EventHandler<DescriptionEventArgs> clickEvent, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, clickEvent, UINotifierType.OK, UIStyles.CurrentResources.SuccessTitle, timeout);
        }

        public static void ShowWarningNotifier(this Form form, string desc, EventHandler<DescriptionEventArgs> clickEvent, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, clickEvent, UINotifierType.WARNING, UIStyles.CurrentResources.WarningTitle, timeout);
        }

        public static void ShowErrorNotifier(this Form form, string desc, EventHandler<DescriptionEventArgs> clickEvent, int timeout = 2000)
        {
            UINotifierHelper.ShowNotifier(desc, clickEvent, UINotifierType.ERROR, UIStyles.CurrentResources.ErrorTitle, timeout);
        }

        #endregion  Windows Helpers
    }
}