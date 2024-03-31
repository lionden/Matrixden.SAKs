using System;
using static System.Environment;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// Extend environment class.
    /// </summary>
    public class MEnvironment
    {
        static readonly Version win7 = new(6, 1);
        static readonly Version win8_1 = new(6, 3);
        static readonly Version win10 = new(10, 0);
        static readonly Version win11 = new(10, 0, 22000);
        public static string Desktop => SpecialFolder.Desktop.FolderPath();
        public static string DesktopDirectory => SpecialFolder.DesktopDirectory.FolderPath();

        public static string Startup => SpecialFolder.Startup.FolderPath();

        public static bool IsWindows7 => OSVersion.Version >= win7 && OSVersion.Version < win8_1;

        public static bool IsWindows10 => OSVersion.Version >= win10 && OSVersion.Version < win11;

        public static bool IsWindows11OrAbove => OSVersion.Version >= win11;
    }

    public static class SpecialFolderExtensions
    {
        public static string FolderPath(this SpecialFolder @this) => GetFolderPath(@this);
    }
}
