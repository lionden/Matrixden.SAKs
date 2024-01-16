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
            Files = directory.GetFiles2(searchPattern).Select(s => new MFile(s)).ToList();
        }

        public MDirectory(string directoryPath, string searchPattern) : this(new DirectoryInfo(directoryPath), searchPattern) { }

        public MDirectory(string path) : this(path, string.Empty) { }

        public DirectoryInfo Directory { get; set; }

        public int FileCount => (Files ?? new List<MFile>()).Count;
        public List<MFile> Files { get; private set; }
    }
}
