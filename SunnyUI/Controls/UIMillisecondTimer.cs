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
 * File Name: UIMillisecondTimer.cs
 * Description: Millisecond timer
 * Current Version: V3.1
 * Creation Date: 2021-08-15
 *
 * 2021-08-15: V3.0.6 Added file description
 ******************************************************************************/

using Sunny.UI.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Sunny.UI
{
    [DefaultEvent("Tick")]
    [DefaultProperty("Interval")]
    public class UIMillisecondTimer : Component
    {
        public event EventHandler Tick;

        /// <summary>
        /// Initializes a new instance of the UIMillisecondTimer class.
        /// </summary>
        public UIMillisecondTimer()
        {
            int result = WinMM.timeGetDevCaps(ref TimeCaps, Marshal.SizeOf(TimeCaps));
            if (result != WinMM.TIMERR_NOERROR)
            {
                throw new Exception("Millisecond timer initialization failed");
            }

            Version = UIGlobal.Version;
            interval = 50;
            SetEventCallback = DoSetEventCallback;
        }

        /// <summary>
        /// Initializes a new instance of the UIMillisecondTimer class with the specified container.
        /// </summary>
        public UIMillisecondTimer(IContainer container) : this()
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        public static bool CanUse()
        {
            TIMECAPS timeCaps = new TIMECAPS();
            int result = WinMM.timeGetDevCaps(ref timeCaps, Marshal.SizeOf(timeCaps));
            return result != WinMM.TIMERR_NOERROR;
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }

        private void DoSetEventCallback(int uTimerID, uint uMsg, uint dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        [
            Localizable(false),
            Bindable(true),
            DefaultValue(null),
            TypeConverter(typeof(StringConverter))
        ]
        public object Tag { get; set; }

        [DefaultValue(null)]
        public string TagString { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; }

        private readonly TIMECAPS TimeCaps;

        private int interval;

        /// <summary>
        /// Occurs when the specified timer interval has elapsed and the timer is enabled.
        /// </summary>
        [DefaultValue(50)]
        public int Interval
        {
            get => interval;
            set
            {
                if (interval == value || value < TimeCaps.wPeriodMin || value > TimeCaps.wPeriodMax)
                    return;

                interval = value;

                if (Enabled)
                {
                    ReStart();
                }
            }
        }

        private bool enabled;

        /// <summary>
        /// Indicates whether the timer is running.
        /// </summary>
        [DefaultValue(false)]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;

                if (!enabled)
                {
                    int result = WinMM.timeSetEvent(interval, Math.Min(1, TimeCaps.wPeriodMin), SetEventCallback, 0, WinMM.TIME_MS);
                    if (result == 0)
                    {
                        throw new Exception("Millisecond timer startup failed");
                    }

                    TimerID = result;
                }
                else
                {
                    if (TimerID > 0)
                    {
                        WinMM.timeKillEvent(TimerID);
                        TimerID = 0;
                    }
                }

                enabled = value;
            }
        }

        private readonly WinMM.TimerSetEventCallback SetEventCallback;
        private int TimerID;

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            Enabled = true;
        }

        /// <summary>
        /// Restarts the timer
        /// </summary>
        public void ReStart()
        {
            Enabled = false;
            Enabled = true;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            Enabled = false;
        }

    }
}
