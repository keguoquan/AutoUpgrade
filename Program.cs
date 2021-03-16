using AutoUpgrade.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoUpdate
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //在主程序中 更新替换自动升级程序
            //ReplaceAutoUpgrade();

            bool isEnterMain = false;
            try
            {
                //设置默认更新地址，如果不设置，后面会从配置文件，或界面上进行设置
                UpgradeHelper.Instance.DefaultUrl = "http://localhost:17580";
                if (UpgradeHelper.Instance.Local_UpgradeModel != null)
                {
                    UpgradeHelper.Instance.UpgradeUrl = UpgradeHelper.Instance.Local_UpgradeModel.UpgradeUrl;
                }

                if (UpgradeHelper.Instance.WillUpgrades.Count == 0 && UpgradeHelper.Instance.Local_UpgradeModel != null)
                {
                    //没有待更新，并且本地版本信息文件不为空，则直接启动主程序
                    bool isSucced = UpgradeHelper.StartRunMain(UpgradeHelper.Instance.Local_UpgradeModel.RunMain);
                    if (isSucced)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        //清理版本信息 以便重新检测版本
                        UpgradeHelper.Instance.ClearUpgradeModel();
                        isEnterMain = true;
                    }
                }
                else
                {
                    isEnterMain = true;
                }
            }
            catch (Exception ex)
            {
                isEnterMain = true;
                MessageBox.Show("运行更新程序异常：\n" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (isEnterMain)
            {
                //进入更新主界面
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmUpdate());
            }
        }

        /// <summary>
        /// 在主程序中 更新替换自动升级程序
        /// </summary>
        private static void ReplaceAutoUpgrade()
        {
            string upgradePath_temp = Path.Combine(Application.StartupPath, "AutoUpgradeTemp", "AutoUpgrade.exe");
            if (System.IO.File.Exists(upgradePath_temp))
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        System.Threading.Thread.Sleep(3000);
                        //杀死自动更新程序
                        KillProcess("AutoUpgrade.exe");

                        //如果存在，表示需要更新自动更新程序
                        string upgradePath = Path.Combine(Application.StartupPath, "AutoUpgrade.exe");
                        if (System.IO.File.Exists(upgradePath))
                        {
                            System.IO.File.Delete(upgradePath);
                        }
                        System.IO.File.Move(upgradePath_temp, upgradePath);
                        string upgradeDir_temp = Path.Combine(Application.StartupPath, "AutoUpgradeTemp");
                        if (Directory.Exists(upgradeDir_temp))
                        {
                            Directory.Delete(upgradeDir_temp);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("替换更新程序异常：\n" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="process"></param>
        private static void KillProcess(string processName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(processName)) return;
                processName = processName.ToLower();
                processName = processName.Replace(".exe", "");
                //杀死主进程
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (!string.IsNullOrWhiteSpace(process.ProcessName))
                    {
                        if (process.ProcessName.ToLower() == processName)
                        {
                            process.Kill();
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
