namespace Sunny.UI
{
    partial class UIComboBox
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

            dropForm?.Dispose();    
            filterForm?.Dispose();
            filterItemForm?.Dispose();

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
            // edit
            // 
            edit.Leave += edit_Leave;
            // 
            // UIComboBox
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            Name = "UIComboBox";
            KeyDown += UIComboBox_KeyDown;
            ButtonClick += UIComboBox_ButtonClick;
            FontChanged += UIComboBox_FontChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
