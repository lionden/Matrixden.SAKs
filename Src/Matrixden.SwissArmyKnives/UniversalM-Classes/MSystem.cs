using IWshRuntimeLibrary;
using Matrixden.SAK.Extensions;
using Matrixden.SwissArmyKnives.Models;
using Matrixden.Utils.Extensions;
using Matrixden.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Matrixden.SwissArmyKnives
{
    public class MSystem
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        ///  向目标路径创建指定文件的快捷方式
        /// </summary>
        /// <param name="location">目标目录</param>
        /// <param name="name">快捷方式名字</param>
        /// <param name="target">文件完全路径</param>
        /// <param name="startIn"></param>
        /// <param name="comment">描述</param>
        /// <param name="icon">图标地址</param>
        /// <returns>成功或失败</returns>
        public static OperationResult CreateShortcut(string location, string name, string target, string startIn = null, string comment = null, string icon = null)
        {
            OperationResult or = new();
            try
            {
                //目录不存在则创建
                if (!Directory.Exists(location))
                    Directory.CreateDirectory(location);

                //添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(location, string.Format("{0}.lnk", name));          //合成路径
                WshShell shell = new();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);    //创建快捷方式对象
                shortcut.TargetPath = target;                                                               //指定目标路径
                shortcut.WorkingDirectory = startIn.IsNotNullNorEmptyNorWhitespace() ? startIn : Path.GetDirectoryName(target);                                  //设置起始位置
                shortcut.WindowStyle = 1;                                                                       //设置运行方式，默认为常规窗口
                shortcut.Description = comment;                                                             //设置备注
                shortcut.IconLocation = icon.IsNotNullNorEmptyNorWhitespace() ? icon : target;    //设置图标路径
                shortcut.Save();                                                                                //保存快捷方式

                return OperationResult.True;
            }
            catch (Exception ex)
            {
                log.FatalException($"Failed during create shortcut: {name}.", ex);
                or.Message = ex.Message;
            }

            return or;
        }

        public static OperationResult CreateStartupQuick(string target) => CreateShortcut(MEnvironment.Startup, Path.GetFileName(target), target);

        public static OperationResult CreateDesktopQuick(string target) => CreateShortcut(MEnvironment.Desktop, Path.GetFileName(target), target);

        /// <summary>
        /// 设置开机自动启动-只需要调用改方法就可以了参数里面的bool变量是控制开机启动的开关的，默认为开启自启启动
        /// </summary>
        /// <param name="onOff">自启开关</param>
        public static void AddToStartup(string targetPath)
        {
            //获取启动路径应用程序快捷方式的路径集合
            List<string> shortcutPaths = GetQuicksByPath(MEnvironment.Startup, targetPath);
            //存在2个以快捷方式则保留一个快捷方式-避免重复多于
            if (shortcutPaths.Count >= 2)
            {
                shortcutPaths.Skip(1).ForEach(s => File.Delete(s));
            }
            else if (shortcutPaths.Count < 1)//不存在则创建快捷方式
            {
                CreateStartupQuick(targetPath);
            }
        }

        /// <summary>
        /// 设置开机自动启动-只需要调用改方法就可以了参数里面的bool变量是控制开机启动的开关的，默认为开启自启启动
        /// </summary>
        /// <param name="onOff">自启开关</param>
        public static void RemoveFromStartup(string targetPath)
        {
            //获取启动路径应用程序快捷方式的路径集合
            List<string> shortcutPaths = GetQuicksByPath(MEnvironment.Startup, targetPath);
            //存在快捷方式则遍历全部删除
            if (shortcutPaths.Count > 0)
            {
                shortcutPaths.ForEach(s => File.Delete(s));
            }
        }

        /// <summary>
        /// 获取指定文件夹下指定应用程序的快捷方式路径集合
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="targetPath">目标应用程序路径</param>
        /// <returns>目标应用程序的快捷方式</returns>
        private static List<string> GetQuicksByPath(string directory, string targetPath)
        {
            List<string> tempStrs = new List<string>();
            string[] files = Directory.GetFiles(directory, "*.lnk");
            if (files == null || files.Length < 1)
            {
                return tempStrs;
            }

            for (int i = 0; i < files.Length; i++)
            {
                //files[i] = string.Format("{0}\\{1}", directory, files[i]);
                string tempStr = GetTargetFromQuick(files[i]);
                if (tempStr == targetPath)
                {
                    tempStrs.Add(files[i]);
                }
            }

            return tempStrs;
        }

        /// <summary>
        /// 获取快捷方式的目标文件路径-用于判断是否已经开启了自动启动
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        private static string GetTargetFromQuick(string shortcutPath)
        {
            //快捷方式文件的路径 = @"d:\Test.lnk";
            if (File.Exists(shortcutPath))
            {
                WshShell shell = new();
                IWshShortcut shortct = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                //快捷方式文件指向的路径.Text = 当前快捷方式文件IWshShortcut类.TargetPath;
                //快捷方式文件指向的目标目录.Text = 当前快捷方式文件IWshShortcut类.WorkingDirectory;
                return shortct.TargetPath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
