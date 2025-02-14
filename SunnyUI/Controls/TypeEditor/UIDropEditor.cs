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
 * File Name: UIDropEditor.cs
 * File Description: Base class for control property editing panel
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
 ******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace Sunny.UI
{
    /// <summary>
    ///   Provides an editor for the <see cref="P:System.Windows.Forms.ToolStripStatusLabel.RectSides" /> property.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public abstract class UIDropEditor : UITypeEditor
    {
        protected UIDropEditorUI EditorUI;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (service != null)
                {
                    if (EditorUI == null)
                    {
                        EditorUI = CreateUI();
                    }

                    EditorUI.Start(service, value);
                    service.DropDownControl(EditorUI);
                    if (EditorUI.Value != null)
                    {
                        value = EditorUI.Value;
                    }

                    EditorUI.End();
                }
            }

            return value;
        }

        protected abstract UIDropEditorUI CreateUI();

        public override UITypeEditorEditStyle GetEditStyle(
            ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }

    public abstract class UIDropEditorUI : UserControl
    {
        private IWindowsFormsEditorService edSvc;

        protected object currentValue;
        protected bool updateCurrentValue;

        public object Value => currentValue;

        public IWindowsFormsEditorService EditorService => edSvc;

        public void Start(IWindowsFormsEditorService editorService, object value)
        {
            edSvc = editorService;
            currentValue = value;
            InitValue(value);
            updateCurrentValue = true;
        }

        protected abstract void InitValue(object value);

        protected abstract void UpdateCurrentValue();

        public void End()
        {
            edSvc = null;
            currentValue = null;
            updateCurrentValue = false;
        }
    }
}