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
 * File Name: IFrame.cs
 * File Description: Page frame interface
 * Current Version: V3.1
 * Creation Date: 2020-12-01
 *
 * 2021-12-01: V3.0.9 Document creation
******************************************************************************/

using System;
using System.Collections.Generic;

namespace Sunny.UI
{
    public interface IFrame
    {
        UITabControl MainTabControl { get; }

        UIPage AddPage(UIPage page, int pageIndex);

        UIPage AddPage(UIPage page, Guid pageGuid);

        UIPage AddPage(UIPage page);

        bool SelectPage(int pageIndex);

        bool SelectPage(Guid pageGuid);

        UIPage GetPage(int pageIndex);

        UIPage GetPage(Guid pageGuid);

        bool TopMost { get; set; }

        bool RemovePage(int pageIndex);

        bool RemovePage(Guid pageGuid);

        bool ExistPage(int pageIndex);

        bool ExistPage(Guid pageGuid);

        void Init();

        void Final();

        T GetPage<T>() where T : UIPage;

        List<T> GetPages<T>() where T : UIPage;

        UIPage SelectedPage { get; }
    }

    public class UIPageParamsArgs : EventArgs
    {
        public UIPage SourcePage { get; set; }

        public UIPage DestPage { get; set; }

        public object Value { get; set; }

        public bool Handled { get; set; } = false;

        public UIPageParamsArgs()
        {

        }

        public UIPageParamsArgs(UIPage sourcePage, UIPage destPage, object value)
        {
            SourcePage = sourcePage;
            DestPage = destPage;
            Value = value;
        }
    }

    public delegate void OnReceiveParams(object sender, UIPageParamsArgs e);
}
