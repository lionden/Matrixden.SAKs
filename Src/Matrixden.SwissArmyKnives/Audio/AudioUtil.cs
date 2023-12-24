using System.Runtime.InteropServices;

namespace Matrixden.Utils.Audio
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioUtil
    {
        /// <summary>
        /// 调用WIN32的API，播放系统提示声
        /// </summary>
        /// <param name="freq">声音的调子</param>
        /// <param name="duration">发声的时长</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        /// <summary>
        /// 模拟主板蜂鸣声
        /// </summary>
        public static void Beep() => Beep(800, 300);
    }
}