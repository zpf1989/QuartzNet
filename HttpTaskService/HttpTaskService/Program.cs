using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpTaskService
{
    static class Program
    {
        static string srvNameFile = AppDomain.CurrentDomain.BaseDirectory + "srvname";
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            //作为服务运行
            if (args != null && args.Count() > 0 && !string.IsNullOrEmpty(args[0]))
            {
                var cmd = args[0];
                if (cmd.ToLower() == "s")
                {
                    RunService();
                }
                return;
            }

            //命令行安装程序
            Console.WriteLine("欢迎使用HttpTaskService");
            Console.WriteLine("请选择：[1]安装服务 [2]卸载服务");
            var input = 0;
            int.TryParse(Console.ReadLine(), out input);
            switch (input)
            {
                case 1:
                    InstallService();
                    break;
                case 2:
                    UninstallService();
                    break;
                default:
                    break;
            }


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void InstallService()
        {
            var srvName = "HttpTaskService";
            Console.WriteLine("请输入服务名称（默认HttpTaskService）：");
            var input = Console.ReadLine();
            srvName = string.IsNullOrEmpty(input) ? srvName : input;

            var path = Process.GetCurrentProcess().MainModule.FileName + " s";
            string installCmd = string.Format("sc create {0} binpath= \"{1}\" displayName= {0} start= auto", srvName, path);//注意等号后面有空格
            bool rst = ExecuteCmd(installCmd);
            if (rst == false)
            {
                Console.WriteLine("安装失败");
                return;
            }
            Console.WriteLine(string.Format("服务【{0}】已安装成功", srvName));
            //将服务名称写入文件，卸载时使用
            File.WriteAllText(srvNameFile, srvName);

            //启动服务
            Console.WriteLine();
            Console.WriteLine(string.Format("将要启动服务【{0}】", srvName));
            string startCmd = string.Format("net start {0}", srvName);
            rst = ExecuteCmd(startCmd);
        }

        static void UninstallService()
        {
            if (!File.Exists(srvNameFile))
            {
                Console.WriteLine("卸载失败，未找到服务名称文件，请检查文件是否存在，并将要卸载的服务名称写入文件");
                return;
            }
            var srvName = File.ReadAllText(srvNameFile);
            if (string.IsNullOrEmpty(srvName))
            {
                Console.WriteLine("卸载失败，服务名称不能为空，请检查文件是否存在，并将要卸载的服务名称写入文件");
            }
            //停止服务
            string stopCmd = string.Format("net stop {0}", srvName);
            ExecuteCmd(stopCmd);

            string uninstallCmd = string.Format("sc delete {0}", srvName);
            bool rst = ExecuteCmd(uninstallCmd);
            if (rst == false)
            {
                Console.WriteLine("卸载失败");
                return;
            }
            Console.WriteLine(string.Format("服务【{0}】已卸载", srvName));
        }

        static bool ExecuteCmd(string cmd)
        {
            Console.WriteLine("将要执行命令 {0}，是否继续？yes", cmd);
            var input = Console.ReadLine().ToLower();
            if (!string.IsNullOrEmpty(input) && input != "y" && input != "yes")
            {
                return false;
            }
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.Arguments = "/c " + cmd;
            proc.Start();
            proc.WaitForExit();
            string ret = proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();
            Console.WriteLine(ret);

            return true;
        }

        static void RunService()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new HttpTaskService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
