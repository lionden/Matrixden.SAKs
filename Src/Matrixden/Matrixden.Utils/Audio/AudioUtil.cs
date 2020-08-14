using System.Runtime.InteropServices;

namespace Matrixden.Utils.Audio
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioUtil
    {
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        public static void Beep() => Beep(800, 300);
    }
}