﻿/******************************************************************************
 * SunnyUI open source control library, tool class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua.
 * QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIStatusForm.cs
 * Description: Progress prompt form
 * Current Version: V3.1
 * Creation Date: 2020-05-05
 *
 * 2020-05-05: V2.2.5 Added file
 ******************************************************************************/

using System.ComponentModel;

namespace Sunny.UI
{
    public sealed partial class UIStatusForm : UIForm
    {
        public UIStatusForm()
        {
            InitializeComponent();
            Text = UIStyles.CurrentResources.InfoTitle;
            Description = UIStyles.CurrentResources.SystemProcessing;
            timer1.Start();
        }

        public UIStatusForm(int max, string desc, int decimalPlaces = 1)
        {
            InitializeComponent();
            Text = UIStyles.CurrentResources.InfoTitle;
            Maximum = max;
            Description = desc;
            Value = 0;
            DecimalPlaces = decimalPlaces;
            timer1.Start();
        }

        [DefaultValue(100)]
        public int Maximum
        {
            get => processBar.Maximum;
            set => processBar.Maximum = value;
        }

        [DefaultValue(0)]
        public int Value
        {
            get => processBar.Value;
            set => processBar.Value = value;
        }

        [DefaultValue(1)]
        public int Step
        {
            get => processBar.Step;
            set => processBar.Step = value;
        }

        [DefaultValue(true)]
        public bool ShowValue
        {
            get => processBar.ShowValue;
            set => processBar.ShowValue = value;
        }

        /// <summary>
        /// Automatically hide when progress reaches maximum value
        /// </summary>
        [DefaultValue(true)]
        public bool MaxAutoHide { get; set; } = true;

        private void processBar_ValueChanged(object sender, int value)
        {
            if (MaxAutoHide && value == Maximum)
            {
                Close();
            }
        }

        public void Show(string title, string desc, int max = 100, int value = 0)
        {
            Text = title;
            labelDescription.Text = desc;
            processBar.Maximum = max;
            processBar.Value = value;
            Show();
        }

        public string Description
        {
            get => labelDescription.Text;
            set => labelDescription.Text = value;
        }

        public int DecimalPlaces
        {
            get => processBar.DecimalPlaces;
            set => processBar.DecimalPlaces = value;
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

        private delegate void StepItHandler(int step);

        public void StepIt(int step = 1)
        {
            if (processBar.InvokeRequired)
            {
                Invoke(new StepItHandler(StepIt), step);
            }
            else
            {
                processBar.Step = step;
                processBar.StepIt();
            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (UIFormServiceHelper.StatusFormServiceClose)
            {
                timer1.Stop();
                UIFormServiceHelper.StatusFormServiceClose = false;
                Close();
            }
        }
    }
}
