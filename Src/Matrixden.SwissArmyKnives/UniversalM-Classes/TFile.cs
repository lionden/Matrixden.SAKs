using System;
using System.IO;

namespace Matrixden.SwissArmyKnives
{
    public class TFile : IDisposable
    {
        private readonly string _path;

        public TFile(string filename)
        {
            _path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-{filename}");
        }

        public static implicit operator string(TFile temporaryFile) => temporaryFile._path;

        public override string ToString() => this;

        public void Dispose()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }
    }
}
