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
 * File Name: UINumPadTextBox.cs
 * File Description: Simulated numeric keypad input box
 * Current Version: V3.3
 * Creation Date: 2023-03-18
 *
 * 2023-03-18: V3.3.3 Added file description
 * 2023-03-26: V3.3.3 Added default event ValueChanged, the Enter key on the keyboard triggers this event
 * 2023-03-26: V3.3.4 Added properties such as maximum value and minimum value
 * 2023-06-11: V3.6.6 Drop-down box selectable magnification is 2
 * 2024-09-03: V3.7.0 Added ShowDropDown() popup method
 * 2024-11-10: V3.7.2 Added StyleDropDown property, set this property to modify the drop-down box theme when manually modifying the Style
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultEvent("ValueChanged")]
    public class UINumPadTextBox : UIDropControl, IToolTip, IHideDropDown
    {
        public UINumPadTextBox()
        {
            InitializeComponent();
            edit.KeyDown += Edit_KeyDown;
            edit.CanEmpty = true;
            fullControlSelect = true;
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => null;

        public delegate void OnValueChanged(object sender, string value);
        public event OnValueChanged ValueChanged;
        private NumPadType numPadType = NumPadType.Text;

        [DefaultValue(NumPadType.Text)]
        [Description("Numeric keypad type"), Category("SunnyUI")]
        public NumPadType NumPadType
        {
            get => numPadType;
            set
            {
                numPadType = value;
                edit.MaxLength = 32767;
                switch (numPadType)
                {
                    case NumPadType.Text:
                        edit.Type = UITextBox.UIEditType.String;
                        break;
                    case NumPadType.Integer:
                        edit.Type = UITextBox.UIEditType.Integer;
                        break;
                    case NumPadType.Double:
                        edit.Type = UITextBox.UIEditType.Double;
                        break;
                    case NumPadType.IDNumber:
                        edit.Type = UITextBox.UIEditType.String;
                        edit.MaxLength = 18;
                        break;
                    default:
                        edit.Type = UITextBox.UIEditType.String;
                        break;
                }

                edit.Text = "";
            }
        }

        private void Edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                if (!NumPadForm.Visible)
                    ShowDropDown();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                NumPadForm.Close();
            }
            else if (e.KeyCode == Keys.Return)
            {
                if (NumPadForm.Visible)
                {
                    NumPadForm.Close();
                }
                else
                {
                    ShowDropDown();
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        public Control ExToolTipControl()
        {
            return edit;
        }

        private readonly UINumPadItem item = new UINumPadItem();

        private UIDropDown numPadForm;

        private UIDropDown NumPadForm
        {
            get
            {
                if (numPadForm == null)
                {
                    numPadForm = new UIDropDown(item);

                    if (numPadForm != null)
                    {
                        numPadForm.VisibleChanged += NumBoardForm_VisibleChanged;
                        numPadForm.ValueChanged += NumBoardForm_ValueChanged;
                    }
                }

                return numPadForm;
            }
        }

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        public static extern int PostMessage(IntPtr hWnd, int Msg, Keys wParam, int lParam);
        public const int WM_CHAR = 256;
        private void NumBoardForm_ValueChanged(object sender, object value)
        {
            int start = edit.SelectionStart;
            switch ((int)value)
            {
                case 88:
                    if (Text.Length == 17)
                    {
                        Win32.User.PostMessage(edit.Handle, WM_CHAR, (int)value, 0);
                        edit.SelectionStart = start;
                        edit.Select(start, 0);
                        //this.Focus();
                    }
                    break;
                case 13:
                    ValueChanged?.Invoke(this, Text);
                    break;
                default:
                    Win32.User.PostMessage(edit.Handle, WM_CHAR, (int)value, 0);
                    edit.SelectionStart = start;
                    edit.Select(start, 0);
                    //this.Focus();
                    break;
            }
        }

        const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        const uint KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public enum VirtualKeys : byte
        {
            VK_NUMLOCK = 0x90, //数字锁定键
            VK_SCROLL = 0x91,  //滚动锁定
            VK_CAPITAL = 0x14, //大小写锁定
            VK_A = 62
        }

        public bool CapsState;

        public static bool GetState(VirtualKeys Key)
        {
            return (GetKeyState((int)Key) == 1);
        }

        public static void SetState(VirtualKeys Key, bool State)
        {
            if (State != GetState(Key))
            {
                keybd_event((byte)Key, 0x45, KEYEVENTF_EXTENDEDKEY | 0, 0);
                keybd_event((byte)Key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        private void NumBoardForm_VisibleChanged(object sender, System.EventArgs e)
        {
            dropSymbol = SymbolNormal;
            if (NumPadForm.Visible)
            {
                dropSymbol = SymbolDropDown;
            }

            if (!NumPadForm.Visible)
            {
                SetState(VirtualKeys.VK_CAPITAL, CapsState);
            }

            Invalidate();
        }

        public void ShowDropDown()
        {
            UIKeyBoardTextBox_ButtonClick(this, EventArgs.Empty);
        }

        private void UIKeyBoardTextBox_ButtonClick(object sender, System.EventArgs e)
        {
            if (NumPadForm.Visible)
            {
                NumPadForm.Close();
            }
            else
            {
                ShowDropDownEx();
            }
        }

        private void ShowDropDownEx()
        {
            NumPadForm.AutoClose = false;
            item.NumPadType = NumPadType;
            item.SetDPIScale();
            item.SetStyleColor(UIStyles.ActiveStyleColor);
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;

            if (numPadType == NumPadType.IDNumber)
            {
                CapsState = GetState(VirtualKeys.VK_CAPITAL);
                SetState(VirtualKeys.VK_CAPITAL, true);
            }

            if (!NumPadForm.Visible)
            {
                Size size = SizeMultiple == 1 ? new Size(320, 195) : new Size(320, 390);
                NumPadForm.Show(this, size);
            }

            edit.Focus();
        }

        [DefaultValue(1)]
        [Description("Popup magnification, can be 1 or 2"), Category("SunnyUI")]
        public int SizeMultiple { get => item.SizeMultiple; set => item.SizeMultiple = value; }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // edit
            // 
            edit.Leave += edit_Leave;
            // 
            // UINumPadTextBox
            // 
            Name = "UINumPadTextBox";
            ButtonClick += UIKeyBoardTextBox_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up all resources being used.
        /// </summary>
        /// <param name="disposing">If managed resources should be disposed, true; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            item?.Dispose();
            numPadForm?.Dispose();
            base.Dispose(disposing);
        }

        private void edit_Leave(object sender, EventArgs e)
        {
            HideDropDown();
        }

        public void HideDropDown()
        {
            try
            {
                if (NumPadForm != null && NumPadForm.Visible)
                    NumPadForm.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// When InputType is a numeric type, the maximum value that can be entered
        /// </summary>
        [Description("When InputType is a numeric type, the maximum value that can be entered"), Category("SunnyUI")]
        [DefaultValue(2147483647D)]
        public double Maximum
        {
            get => edit.MaxValue;
            set => edit.MaxValue = value;
        }

        /// <summary>
        /// When InputType is a numeric type, the minimum value that can be entered
        /// </summary>
        [Description("When InputType is a numeric type, the minimum value that can be entered"), Category("SunnyUI")]
        [DefaultValue(-2147483648D)]
        public double Minimum
        {
            get => edit.MinValue;
            set => edit.MinValue = value;
        }

        [Description("Floating point number, number of decimal places displayed"), Category("SunnyUI")]
        [DefaultValue(2)]
        public int DecimalPlaces
        {
            get => edit.DecLength;
            set => edit.DecLength = Math.Max(value, 0);
        }
    }
}
