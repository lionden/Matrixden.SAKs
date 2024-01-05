using static System.Environment;

namespace Matrixden.SwissArmyKnives
{
    /// <summary>
    /// Extend environment class.
    /// </summary>
    public class MEnvironment
    {
        public static string Desktop => SpecialFolder.Desktop.FolderPath();
        public static string DesktopDirectory => SpecialFolder.DesktopDirectory.FolderPath();

        public static string Startup => SpecialFolder.Startup.FolderPath();
    }

    public static class SpecialFolderExtensions
    {
        public static string FolderPath(this SpecialFolder @this) => GetFolderPath(@this);
    }
}
