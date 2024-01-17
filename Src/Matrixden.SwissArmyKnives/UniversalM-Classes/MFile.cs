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

        /// <summary>
        /// Move current file into given folder (destFolder).
        /// </summary>
        /// <param name="destFolder"></param>
        public void MoveTo(string destFolder)
        {
            if (!Exists) return;

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            File.MoveTo(Path.Combine(destFolder, File.Name));
        }
    }
}
