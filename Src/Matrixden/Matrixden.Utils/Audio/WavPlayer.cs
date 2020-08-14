using Matrixden.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Audio
{
    /// <summary>
    /// Play wav audio file.
    /// </summary>
    public class WavPlayer : AudioUtil
    {
        static readonly ILog _LOG = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 音频文件的文件夹路径
        /// </summary>
        string AUDIO_FOLDER_PATH { get; set; } = $"{Directory.GetCurrentDirectory()}\\Sound";
        static SoundPlayer _player = new SoundPlayer();

        /// <summary>
        /// 播放指定音频
        /// </summary>
        /// <param name="file">file name with extension.</param>
        public void PlayWav(string file)
        {
            var fp = $@"{AUDIO_FOLDER_PATH}\{file}";
            if (!File.Exists(fp))
            {
                Beep();
                _LOG.Error($"Failed to find audio file at [{fp}]");
                return;
            }

            _player.SoundLocation = fp;
            _player.Load();
            _player.Play();
        }

        public WavPlayer() { }

        public WavPlayer(string wavPath)
        {
            if (!Directory.Exists(wavPath))
                _LOG.Fatal($"Path not found, given path is: \"{wavPath}\".");

            this.AUDIO_FOLDER_PATH = wavPath;
        }
    }
}
