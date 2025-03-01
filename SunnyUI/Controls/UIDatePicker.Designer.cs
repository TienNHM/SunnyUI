namespace Sunny.UI
{
    partial class UIDatePicker
    {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIDatePicker
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            Name = "UIDatePicker";
            SymbolDropDown = 61555;
            SymbolNormal = 61555;
            ButtonClick += UIDatetimePicker_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
