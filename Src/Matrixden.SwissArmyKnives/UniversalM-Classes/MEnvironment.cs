using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// Extend environment class.
    /// </summary>
    public class MEnvironment
    {
        public static string Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string DesktopDirectory => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public static string Startup => Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    }
}
