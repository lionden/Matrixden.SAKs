using Matrixden.SAK.Extensions;
using Matrixden.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives
{
    public class MDirectory
    {
        public MDirectory(DirectoryInfo directory, string searchPattern)
        {
            Directory = directory;
            SearchFiles = Directory.GetFiles2(searchPattern).Select(s => new MFile(s)).ToList();
        }

        public MDirectory(string directoryPath, string searchPattern) : this(new DirectoryInfo(directoryPath), searchPattern) { }

        public MDirectory(string path) : this(path, string.Empty) { }

        public DirectoryInfo Directory { get; set; }

        public string Name => Directory.Name;
        public string FullName => Directory.FullName;

        public List<MFile> SearchFiles { get; private set; }
        public int SearchFileCount => (SearchFiles ?? new List<MFile>()).Count;

        public List<MFile> Files => Directory.GetFiles().Select(s => new MFile(s)).ToList();
        public int FileCount => (SearchFiles ?? new List<MFile>()).Count;

        public bool Exists => Directory.Exists;

        public bool IsEmpty => !HasSubdirectories && Files.Count == 0;

        public bool HasSubdirectories => Directory.GetDirectories().Length > 0;

        public List<MDirectory> Subdirectories
        {
            get
            {
                if (!HasSubdirectories) return new();
                return Directory.GetDirectories().Select(s => new MDirectory(s.FullName)).ToList();
            }
        }

        public MDirectory Parent => new(Directory.Parent, string.Empty);

        /// <summary>
        /// Move current folder into given folder (destFolder).
        /// </summary>
        /// <param name="destFolder"></param>
        public void MoveTo(string destFolder)
        {
            if (!Exists) return;
            if (HasSubdirectories)
            {
                Subdirectories.ForEach(f => f.MoveTo(Path.Combine(destFolder, f.Name)));
            }

            SearchFiles.ForEach(f => f.MoveTo(destFolder));
            if (Exists && IsEmpty)
                Directory.Delete();
        }

        /// <summary>
        /// Move all files (or folders) under source folder to dest folder.
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        public static void Move(string sourceFolder, string destFolder)
        {
            if (sourceFolder.IsNullOrEmptyOrWhiteSpace() || destFolder.IsNullOrEmptyOrWhiteSpace()) return;
            new MDirectory(sourceFolder).MoveTo(destFolder);
        }
    }
}
