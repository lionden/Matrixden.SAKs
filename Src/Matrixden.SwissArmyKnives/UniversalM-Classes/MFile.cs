using System.IO;

namespace Matrixden.SwissArmyKnives
{
    public class MFile
    {
        public MFile(FileInfo file)
        {
            File = file;
        }
        public MFile(string file) : this(new FileInfo(file))
        {
        }

        public FileInfo File { get; set; }

        public bool Exists => File.Exists;

        public void MoveTo(string destPath)
        {
            if (!Exists) return;

            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            File.MoveTo(Path.Combine(destPath, File.Name));
        }
    }
}
