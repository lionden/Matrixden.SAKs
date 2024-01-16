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
    }
}
