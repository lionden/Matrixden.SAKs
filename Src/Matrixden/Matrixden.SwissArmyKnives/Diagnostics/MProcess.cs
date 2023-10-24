using Matrixden.Utils.Extensions;
using Matrixden.Utils.Logging;
using Matrixden.Utils.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives.Diagnostics
{
    /// <summary>
    /// Process utils.
    /// </summary>
    public class MProcess
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <returns></returns>
        public static Process[] Gets(string name)
        {
            if (name.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentNullException("process name");

            var ps = Process.GetProcessesByName(name);
            if (ps == null)
                ps = new Process[0];

            return ps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <param name="path">进程全路径</param>
        /// <returns></returns>
        public static Process[] Gets(string name, string path)
        {
            var ps = Gets(name);
            if (path.IsNullOrEmptyOrWhiteSpace())
                return ps;

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found at: {path}");

            return ps.Where(p => string.Equals(p.MainModule.FileName, Path.GetFullPath(path), StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <returns></returns>
        public static int Count(string name)
        {
            var ps = Gets(name);

            return ps.Length;
        }

        /// <summary>
        /// 判断进程是否正在运行，可通过程序全路径
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <param name="path">进程全路径</param>
        public static bool Has(string name, string path)
        {
            var ps = Gets(name);
            if (ps.Length <= 0)
                return false;

            if (path.IsNullOrEmptyOrWhiteSpace())
                return true;
            else if (!File.Exists(path))
                return false;

            return ps.Any(p => string.Equals(p.MainModule.FileName, Path.GetFullPath(path), StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 判断进程是否正在运行
        /// </summary>
        /// <param name="name">进程名称</param>
        public static bool Has(string name) => Has(name, string.Empty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The name of an application file to run in the process.</param>
        /// <param name="arguments">Command-line arguments to pass when starting the process.</param>
        /// <returns></returns>
        public static OperationResult<Process> TryStart(string path, string arguments)
        {
            if (path.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentNullException("process path");

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found at: {path}");

            try
            {
                var p = Process.Start(path, arguments);
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);

                return new OperationResult<Process>(p);
            }
            catch (Exception ex)
            {
                log.FatalException($"Failed to start process: {path}.", ex);
                return new OperationResult<Process>(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">进程全路径</param>
        /// <returns></returns>
        public static OperationResult<Process> TryStart(string path) => TryStart(path, string.Empty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">进程全路径</param>
        /// <returns></returns>
        public static bool Start(string path) => TryStart(path).Result;

        /// <summary>
        /// 结束进程树
        /// </summary>
        /// <param name="pid">进程ID</param>
        internal static void EndTask(int pid)
        {
            var searcher = new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={pid}");
            var moc = searcher.Get();
            if (moc.Count > 0)
                foreach (var mo in moc)
                    EndTask(mo["ProcessID"].ToInt32());

            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException _)
            {
                log.TraceException("Failed to get process by id.", _);
            }
            catch (Exception _)
            {
                log.FatalException(string.Empty, _);
            }
        }

        /// <summary>
        /// 结束进程树
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <param name="path">进程全路径</param>
        public static void EndTask(string name, string path)
        {
            if (!Has(name, path))
                return;

            var ps = Gets(name, path);
            foreach (var p in ps)
                EndTask(p.Id);
        }

        /// <summary>
        /// 结束进程树
        /// </summary>
        /// <param name="name">进程名称</param>
        public static void EndTask(string name) => EndTask(name, string.Empty);

        internal static void EndTaskByPath(string path)
        {
            if (path.IsNullOrEmptyOrWhiteSpace())
                throw new ArgumentNullException("process path");

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found at: {path}");

            EndTask(Path.GetFileNameWithoutExtension(path), path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">进程全路径</param>
        /// <returns></returns>
        public static bool ReStart(string path)
        {
            EndTaskByPath(path);

            return Start(path);
        }
    }
}
