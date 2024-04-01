using System.IO;
using System.Security.AccessControl;

namespace Matrixden.SwissArmyKnives
{
    public class MFile
    {
        public MFile(FileInfo file)
        {
            FInfo = file;
        }
        public MFile(string file) : this(new FileInfo(file))
        {
        }

        public FileInfo FInfo { get; set; }

        public bool Exists => FInfo.Exists;

        /// <summary>
        /// Copy current file into given folder (destFolder).
        /// </summary>
        /// <param name="destFolder"></param>
        public void CopyTo(string destFolder, bool overwrite = false)
        {
            if (!Exists) return;

            var df = Path.Combine(destFolder, FInfo.Name);
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            else if (File.Exists(df))
                if (overwrite)
                {
                    File.SetAttributes(df, FileAttributes.Normal);
                    File.Delete(df);
                }
                else
                    return;

            FInfo.CopyTo(df);
        }

        /// <summary>
        /// Move current file into given folder (destFolder).
        /// </summary>
        /// <param name="destFolder"></param>
        public void MoveTo(string destFolder, bool overwrite = false)
        {
            if (!Exists) return;

            var df = Path.Combine(destFolder, FInfo.Name);
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            else if (File.Exists(df))
                if (overwrite)
                {
                    File.SetAttributes(df, FileAttributes.Normal);
                    File.Delete(df);
                }
                else
                    return;

            FInfo.MoveTo(df);
        }

        /// <summary>
        /// Permanently deletes a file.
        /// </summary>
        public void Delete()
        {
            if (!Exists) return;

            File.SetAttributes(FInfo.FullName, FileAttributes.Normal);
            FInfo.Delete();
        }

        /// <summary>
        /// Rename a file.
        /// </summary>
        /// <param name="newName"></param>
        public void Rename(string newName)
        {
            if (!Exists) return;

            File.SetAttributes(FInfo.FullName, FileAttributes.Normal);
            FInfo.MoveTo(Path.Combine(FInfo.DirectoryName, newName));
        }
    }
}
