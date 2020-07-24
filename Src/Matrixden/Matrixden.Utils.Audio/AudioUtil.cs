using Matrixden.Utils.Audio.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Audio
{
    public class AudioUtil
    {
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        public static void Beep() => Beep(800, 300);
    }
}