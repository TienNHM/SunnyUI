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
 * File Name: UISelectForm.cs
 * Description: Dropdown selection form
 * Current Version: V3.1
 * Creation Date: 2020-05-05
 *
 * 2020-05-05: V2.2.5 Added file
******************************************************************************/

using System.Collections;

namespace Sunny.UI
{
    /// <summary>
    /// Dropdown selection form
    /// </summary>
    public sealed partial class UISelectForm : UIEditForm
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UISelectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set dropdown items
        /// </summary>
        /// <param name="items"></param>
        public void SetItems(IList items)
        {
            ComboBox.Items.Clear();

            foreach (var item in items)
            {
                ComboBox.Items.Add(item);
            }
        }

        public string Description
        {
            get => label.Text;
            set => label.Text = value;
        }

        public string Title
        {
            get => Text;
            set => Text = value;
        }

        /// <summary>
        /// Selected index of the dropdown
        /// </summary>
        public int SelectedIndex
        {
            get => ComboBox.SelectedIndex;
            set => ComboBox.SelectedIndex = value;
        }
    }
}