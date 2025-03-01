/******************************************************************************
 * SunnyUI open source control library, utility class library, extension class library, multi-page development framework.
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ group: 56829229 QQ: 17612584 EMail: SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UIGlobal.cs
 * Description: Global parameter class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Reflection;

namespace Sunny.UI
{
    /// <summary>
    /// Global parameter class
    /// </summary>
    public static class UIGlobal
    {
        //public const string Version = "SunnyUI.Net V3.2.6";

        /// <summary>
        /// Version
        /// </summary>
        public static string Version = Assembly.GetExecutingAssembly().GetName().Name + " V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public const int EditorMinHeight = 20;
        public const int EditorMaxHeight = 60;
    }

    public interface IHideDropDown
    {
        public void HideDropDown();
    }

    public class UIDateTimeArgs : EventArgs
    {
        public DateTime DateTime { get; set; }

        public UIDateTimeArgs()
        {

        }

        public UIDateTimeArgs(DateTime datetime)
        {
            DateTime = datetime;
        }
    }

    public class UITextBoxSelectionArgs : EventArgs
    {
        public int SelectionStart { get; set; }

        public string Text { get; set; }
    }

    public delegate void OnSelectionChanged(object sender, UITextBoxSelectionArgs e);

    public delegate void OnDateTimeChanged(object sender, UIDateTimeArgs e);

    public delegate void OnCancelEventArgs(object sender, CancelEventArgs e);

    public enum NumPadType
    {
        Text,
        Integer,
        Double,
        IDNumber
    }
}