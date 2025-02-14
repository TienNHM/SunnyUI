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
 * If you use this code, please keep this note.
 ******************************************************************************
 * File Name: UAudio.cs
 * File Description: Audio playback helper class
 * Current Version: V3.1
 * Creation Date: 2020-01-01
 *
 * 2020-01-01: V2.2.0 Added file description
******************************************************************************/

using System;
using System.ComponentModel;
using System.Media;
using System.Security;
using System.Security.Permissions;
#pragma warning disable SYSLIB0003 // Type or member is obsolete

namespace Sunny.UI
{
    /// <summary>
    /// Wav audio playback helper class
    /// </summary>
    [HostProtection(SecurityAction.LinkDemand, Resources = HostProtectionResource.ExternalProcessMgmt)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class WavPlayer
    {
        /// <summary>
        /// Indicates how to play a sound when calling audio methods.
        /// </summary>
        public enum AudioPlayMode
        {
            /// <summary>
            /// Play the sound and wait until it completes before the calling code continues.
            /// </summary>
            WaitToComplete,

            /// <summary>
            /// Play the sound in the background. The calling code continues to execute.
            /// </summary>
            Background,

            /// <summary>
            /// Play the background sound until the stop method is called. The calling code continues to execute.
            /// </summary>
            BackgroundLoop
        }

        private static SoundPlayer _soundPlayer;

        #region Methods

        /// <summary>
        /// Stop playing the sound.
        /// </summary>
        /// <param name="sound">SoundPlayer object.</param>
        private static void InternalStop(SoundPlayer sound)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
            try
            {
                sound.Stop();
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>
        /// Play a .wav sound file.
        /// </summary>
        /// <param name="location">A string containing the name of the sound file.</param>
        public static void Play(string location)
        {
            Play(location, AudioPlayMode.Background);
        }

        /// <summary>
        /// Play a .wav sound file.
        /// </summary>
        /// <param name="location">A string containing the name of the sound file.</param>
        /// <param name="playMode">AudioPlayMode enumeration indicating the play mode.</param>
        public static void Play(string location, AudioPlayMode playMode)
        {
            ValidateAudioPlayModeEnum(playMode, nameof(playMode));
            var validatedLocation = ValidateFilename(location);
            SoundPlayer player = new SoundPlayer(validatedLocation);
            Play(player, playMode);
        }

        /// <summary>
        /// Play the sound based on the play mode.
        /// </summary>
        /// <param name="sound">SoundPlayer object.</param>
        /// <param name="mode">AudioPlayMode enumeration indicating the play mode.</param>
        private static void Play(SoundPlayer sound, AudioPlayMode mode)
        {
            if (_soundPlayer != null)
            {
                InternalStop(_soundPlayer);
            }

            _soundPlayer = sound;
            switch (mode)
            {
                case AudioPlayMode.WaitToComplete:
                    _soundPlayer.PlaySync();
                    break;

                case AudioPlayMode.Background:
                    _soundPlayer.Play();
                    break;

                case AudioPlayMode.BackgroundLoop:
                    _soundPlayer.PlayLooping();
                    break;
            }
        }

        /// <summary>
        /// Play a system sound.
        /// </summary>
        /// <param name="systemSound">SystemSound object representing the system sound to play.</param>
        public static void PlaySystemSound(SystemSound systemSound)
        {
            if (systemSound == null)
            {
                throw new ArgumentNullException(nameof(systemSound));
            }

            systemSound.Play();
        }

        /// <summary>
        /// Stop playing the background sound.
        /// </summary>
        public static void Stop()
        {
            SoundPlayer player = new SoundPlayer();
            InternalStop(player);
        }

        /// <summary>
        /// Validate AudioPlayMode enumeration value.
        /// </summary>
        /// <param name="value">AudioPlayMode enumeration value.</param>
        /// <param name="paramName">Parameter name.</param>
        private static void ValidateAudioPlayModeEnum(AudioPlayMode value, string paramName)
        {
            if (!Enum.IsDefined(typeof(AudioPlayMode), value))
            {
                throw new InvalidEnumArgumentException(paramName, (int)value, typeof(AudioPlayMode));
            }
        }

        /// <summary>
        /// Validate the filename.
        /// </summary>
        /// <param name="location">Filename string.</param>
        /// <returns>Validated filename.</returns>
        private static string ValidateFilename(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException(nameof(location));
            }

            return location;
        }

        #endregion Methods
    }

    /// <summary>
    /// MP3 file playback helper class
    /// </summary>
    public static class Mp3Player
    {
        /// <summary>
        /// Play an MP3 file.
        /// </summary>
        /// <param name="mp3FileName">Filename.</param>
        /// <param name="repeat">Whether to repeat playback.</param>
        public static void Play(string mp3FileName, bool repeat)
        {
            Win32.WinMM.mciSendString($"open \"{mp3FileName}\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
            Win32.WinMM.mciSendString($"play MediaFile{(repeat ? " repeat" : string.Empty)}", null, 0, IntPtr.Zero);
        }

        /// <summary>
        /// Pause playback.
        /// </summary>
        public static void Pause()
        {
            Win32.WinMM.mciSendString("stop MediaFile", null, 0, IntPtr.Zero);
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        public static void Stop()
        {
            Win32.WinMM.mciSendString("close MediaFile", null, 0, IntPtr.Zero);
        }
    }
}