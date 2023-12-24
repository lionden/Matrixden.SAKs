using AudioSwitcher.AudioApi.CoreAudio;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Matrixden.Utils.Audio
{
    /// <summary>
    /// Use Windows's default TTS, use Microsoft Huihui or Lili to speak Chinese characters.
    /// </summary>
    public class TextSpeaker : AudioUtil
    {
        static readonly ILog _LOG = LogProvider.GetCurrentClassLogger();
        string _pre_text = string.Empty;
        Prompt _prompt = new Prompt(string.Empty);
        /// <summary>
        /// 播放指定文字
        /// </summary>
        /// <param name="txt">Text to speak.</param>
        public void SpeakAsync(string txt)
        {
            if (txt.IsNullOrEmptyOrWhiteSpace())
                return;

            if (!_prompt.IsCompleted)
            {
                if (txt.Equals(_pre_text))
                    return;
                else
                    Synth.SpeakAsyncCancel(_prompt);
            }

            _pre_text = txt;
            _prompt = Synth.SpeakAsync(txt);
        }

        /// <summary>
        /// 大声播放指定文字，使用系统80%音量。
        /// </summary>
        /// <param name="txt"></param>
        public void SpeakLoudly(string txt)
        {
            var _preVol = DefaultPlaybackDevice.Volume;

            DefaultPlaybackDevice.Volume = 80;
            SpeakAsync(txt);

            new Thread(() => Instance.WaitToRestoreSystemVolume(_preVol)).Start();
        }

        private void WaitToRestoreSystemVolume(double _preVol)
        {
            while (!_prompt.IsCompleted) { }
            DefaultPlaybackDevice.Volume = _preVol;
        }

        /// <summary>
        /// 结束所有在播放内容
        /// </summary>
        public void SpeakAsyncCancelAll() => Synth.SpeakAsyncCancelAll();

        // Initialize a new instance of the SpeechSynthesizer.  
        SpeechSynthesizer Synth { get; } = new SpeechSynthesizer();

        private static readonly object __locker = new object();
        private static TextSpeaker __instance;

        private static CoreAudioDevice _defaultPlaybackDevice;
        private static CoreAudioDevice DefaultPlaybackDevice
        {
            get
            {
                return _defaultPlaybackDevice;
            }
        }

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static TextSpeaker Instance
        {
            get
            {
                if (__instance == null)
                {
                    lock (__locker)
                    {
                        if (__instance == null)
                        {
                            __instance = new TextSpeaker();
                        }
                    }
                }

                return __instance;
            }
        }

        private TextSpeaker()
        {
            // Configure the audio output.   
            Synth.SetOutputToDefaultAudioDevice();
            Synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.NotSet, 0, CultureInfo.GetCultureInfo("zh-CN"));
            _defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

            if (Synth.Voice.Culture.Name != "zh-CN")
            {
                Beep();
                _LOG.Fatal("该机器未安装中文语音");
            }
        }
    }
}
