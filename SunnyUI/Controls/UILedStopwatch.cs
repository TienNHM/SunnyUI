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
 * File name: UILedStopwatch.cs
 * Description: LED Stopwatch
 * Current version: V3.1
 * Creation date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
******************************************************************************/

using System;
using System.ComponentModel;

namespace Sunny.UI
{
    /// <summary>
    /// LED Stopwatch
    /// </summary>
    [DefaultEvent("TimerTick")]
    [DefaultProperty("Text")]
    public sealed class UILedStopwatch : UILedDisplay
    {
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        /// <summary>
        /// Triggered once when the timer starts and the Text changes
        /// </summary>
        public event EventHandler TimerTick;

        /// <summary>
        /// Constructor
        /// </summary>
        public UILedStopwatch()
        {
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
        }

        ~UILedStopwatch()
        {
            timer.Stop();
            timer.Dispose();
        }

        public enum TimeShowType
        {
            mmss,
            mmssfff,
            hhmmss
        }

        [DefaultValue(TimeShowType.mmss)]
        [Description("Display mode"), Category("SunnyUI")]
        public TimeShowType ShowType { get; set; } = TimeShowType.mmss;

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan = DateTime.Now - StartTime;
            string text = "";
            switch (ShowType)
            {
                case TimeShowType.mmss:
                    text = TimeSpan.Minutes.ToString("D2") + ":" + TimeSpan.Seconds.ToString("D2");
                    break;

                case TimeShowType.mmssfff:
                    text = TimeSpan.Minutes.ToString("D2") + ":" + TimeSpan.Seconds.ToString("D2") + "." + TimeSpan.Milliseconds.ToString("D3");
                    break;

                case TimeShowType.hhmmss:
                    text = TimeSpan.Hours.ToString("D2") + ":" + TimeSpan.Minutes.ToString("D2") + ":" + TimeSpan.Seconds.ToString("D2");
                    break;
            }

            if (text != Text)
            {
                Text = text;
                TimerTick?.Invoke(this, e);
            }
        }

        /// <summary>
        /// OnCreateControl
        /// </summary>
        protected override void OnCreateControl()
        {
            Text = "00:00";
        }

        /// <summary>
        /// Timing
        /// </summary>
        [Browsable(false)]
        public TimeSpan TimeSpan { get; private set; }

        private DateTime StartTime;

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            Text = "00:00";
            StartTime = DateTime.Now;
            timer.Start();
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// Whether it has started working
        /// </summary>
        [Browsable(false)]
        public bool IsWorking => timer.Enabled;

        private bool _active;

        [DefaultValue(false), Description("Whether it has started working"), Category("SunnyUI")]
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                if (_active)
                    Start();
                else
                    Stop();
            }
        }
    }
}