using System.Threading;
using System.Windows.Forms;

namespace Sunny.UI
{
    public class UIFormService
    {
        protected Thread thread;
        public bool IsRun => thread != null && thread.ThreadState == ThreadState.Running;
    }

    public static class UIFormServiceHelper
    {
        private static UIWaitFormService WaitFormService;
        private static UIProcessIndicatorFormService ProcessFormService;
        private static UIStatusFormService StatusFormService;

        static UIFormServiceHelper()
        {
            WaitFormService = new UIWaitFormService();
            ProcessFormService = new UIProcessIndicatorFormService();
            StatusFormService = new UIStatusFormService();
        }

        /// <summary>
        /// Displays a waiting dialog.
        /// </summary>
        /// <param name="owner">The parent form.</param>
        /// <param name="size">The size of the waiting dialog.</param>
        public static void ShowProcessForm(this Form owner, int size = 200, bool showrect = true)
        {
            if (ProcessFormService.IsRun) return;
            ProcessFormServiceClose = false;
            ProcessFormService.CreateForm(size, showrect);
        }

        internal static bool ProcessFormServiceClose;

        /// <summary>
        /// Hides the waiting dialog.
        /// </summary>
        public static void HideProcessForm(this Form owner)
        {
            ProcessFormServiceClose = true;
        }

        /// <summary>
        /// Displays a waiting dialog with a description.
        /// </summary>
        /// <param name="desc">Description text</param>
        public static void ShowWaitForm(this Form owner, string desc = "The system is processing, please wait...")
        {
            if (WaitFormService.IsRun) return;
            WaitFormServiceClose = false;
            WaitFormService.CreateForm(desc);
        }

        internal static bool WaitFormServiceClose;

        /// <summary>
        /// Hide the waiting prompt window
        /// </summary>
        public static void HideWaitForm(this Form owner)
        {
            WaitFormServiceClose = true;
        }

        /// <summary>
        /// Set the description text for the waiting prompt window
        /// </summary>
        /// <param name="desc">Description text</param
        public static void SetWaitFormDescription(this Form owner, string desc)
        {
            if (!WaitFormService.IsRun) return;
            WaitFormService.SetDescription(desc);
        }

        /// <summary>
        /// Show progress prompt window
        /// </summary>
        /// <param name="desc">Description text</param>
        /// <param name="maximum">Maximum progress value</param>
        /// <param name="decimalCount">Number of decimal places for progress bar</param>
        public static void ShowStatusForm(this Form owner, int maximum = 100, string desc = "The system is processing, please wait...", int decimalCount = 1)
        {
            if (StatusFormService.IsRun) return;
            StatusFormServiceClose = false;
            StatusFormService.CreateForm(maximum, desc, decimalCount);
        }

        internal static bool StatusFormServiceClose;

        /// <summary>
        /// Hide the progress prompt window
        /// </summary>
        public static void HideStatusForm(this Form owner)
        {
            StatusFormServiceClose = true;
        }

        /// <summary>
        /// Increase the progress step value in the progress prompt window by 1
        /// </summary>
        /// <param name="step">Step value to increase (default is 1)</param>
        public static void SetStatusFormStepIt(this Form owner, int step = 1)
        {
            if (!StatusFormService.IsRun) return;
            StatusFormService.SetFormStepIt(step);
        }

        /// <summary>
        /// Set the description text for the progress prompt window
        /// </summary>
        /// <param name="desc">Description text</param>
        public static void SetStatusFormDescription(this Form owner, string desc)
        {
            if (!StatusFormService.IsRun) return;
            StatusFormService.SetFormDescription(desc);
        }
    }

    public class UIWaitFormService : UIFormService
    {
        private UIWaitForm form;

        public void CreateForm(string desc)
        {
            thread = new Thread(delegate ()
            {
                form = new UIWaitForm(desc);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.Render();
                if (IsRun) Application.Run(form);
            });

            thread.Start();
        }

        public void SetDescription(string desc)
        {
            try
            {
                form?.SetDescription(desc);
            }
            catch
            {
            }
        }
    }

    public class UIProcessIndicatorFormService : UIFormService
    {
        private UIProcessIndicatorForm form;

        public void CreateForm(int size = 200, bool showRect = true)
        {
            thread = new Thread(delegate ()
            {
                form = new UIProcessIndicatorForm();
                form.ShowRect = showRect;
                form.Size = new System.Drawing.Size(size, size);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.Render();
                Application.Run(form);
            });

            thread.Start();
        }
    }

    public class UIStatusFormService : UIFormService
    {
        private UIStatusForm form;

        public void CreateForm(int max, string desc, int decimalCount = 1)
        {
            thread = new Thread(delegate ()
            {
                form = new UIStatusForm(max, desc, decimalCount);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.Render();
                Application.Run(form);
            });

            thread.Start();
        }

        public void SetFormDescription(string desc)
        {
            try
            {
                form?.SetDescription(desc);
            }
            catch
            {
            }
        }

        public void SetFormStepIt(int step = 1)
        {
            try
            {
                form?.StepIt(step);
            }
            catch
            {
            }
        }
    }
}