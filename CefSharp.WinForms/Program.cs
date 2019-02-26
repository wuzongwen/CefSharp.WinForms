using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.WinForms
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            LoadApp();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadApp()
        {
            //执行依赖检查以确保所有相关资源都在输出目录中
            var settings = new CefSettings()
            {
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,Environment.Is64BitProcess ? "x64" : "x86","CefSharp.BrowserSubprocess.exe"),
                LogSeverity = LogSeverity.Disable//不显示日志
            };

            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

            var browser = new BrowserForm();
            Application.Run(browser);
        }

        // x86或x64子dir加载缺少的程序集
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }
}
