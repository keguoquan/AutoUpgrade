using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;

namespace AutoUpgrade.Helper
{
    /// <summary>
    /// 更新帮助类
    /// </summary>
    public class UpgradeHelper
    {
        /// <summary>
        /// 默认服务器地址
        /// 在配置文件中未找到地址时，使用此地址进行更新
        /// </summary>
        public string DefaultUrl { get; set; }

        public string _upgradeUrl;
        /// <summary>
        /// 获取或设置服务器地址
        /// </summary>
        public string UpgradeUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_upgradeUrl))
                {
                    return DefaultUrl;
                }
                return _upgradeUrl;
            }
            set
            {
                _upgradeUrl = value;
            }
        }

        /// <summary>
        /// 本地配置文件路径
        /// </summary>
        public string Local_UpgradeXmlPath = Path.Combine(Application.StartupPath, "UpgradeList.xml");

        private UpgradeModel _local_UpgradeModel;
        /// <summary>
        /// 本地版本信息
        /// </summary>
        public UpgradeModel Local_UpgradeModel
        {
            get
            {
                try
                {
                    if (_local_UpgradeModel == null)
                    {
                        if (File.Exists(Local_UpgradeXmlPath))
                        {
                            _local_UpgradeModel = new UpgradeModel();
                            _local_UpgradeModel.LoadUpgrade(File.ReadAllText(Local_UpgradeXmlPath));
                        }
                    }
                    return _local_UpgradeModel;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("获取本地版本文件UpgradeList.xml异常：{0}", ex.Message));
                }
            }
        }

        private UpgradeModel _server_UpgradeModel;
        /// <summary>
        /// 服务器版本信息
        /// </summary>
        public UpgradeModel Server_UpgradeModel
        {
            get
            {
                try
                {
                    if (_server_UpgradeModel == null && !string.IsNullOrWhiteSpace(UpgradeUrl))
                    {
                        string resXml = MyWebResquest.GetUpgradeList(UpgradeUrl);
                        if (!string.IsNullOrWhiteSpace(resXml))
                        {
                            _server_UpgradeModel = new UpgradeModel();
                            _server_UpgradeModel.LoadUpgrade(resXml);
                            _server_UpgradeXml = resXml;
                        }
                    }
                    return _server_UpgradeModel;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("获取服务端版本文件UpgradeList.xml异常：{0}", ex.Message));
                }
            }
        }

        private string _server_UpgradeXml;
        /// <summary>
        /// 服务端版本配置xml
        /// </summary>
        public string Server_UpgradeXml
        {
            get
            {
                return _server_UpgradeXml;
            }
        }

        private Dictionary<string, string> _willUpgrades;
        /// <summary>
        /// 待更新文件列表,如果为0，则表示不需要更新
        /// </summary>
        public Dictionary<string, string> WillUpgrades
        {
            get
            {
                if (_willUpgrades == null)
                {
                    _willUpgrades = new Dictionary<string, string>();
                    //如果服务器端未获取到版本信息  则不更新
                    if (Server_UpgradeModel != null)
                    {
                        if (Local_UpgradeModel == null)//本地版本信息为空 全部更新
                        {
                            _willUpgrades = Server_UpgradeModel.DictFiles;
                        }
                        else
                        {
                            //对比需要更新的文件
                            foreach (var item in Server_UpgradeModel.DictFiles)
                            {
                                //如果找到
                                if (Local_UpgradeModel.DictFiles.ContainsKey(item.Key))
                                {
                                    //如果版本不匹配
                                    if (Local_UpgradeModel.DictFiles[item.Key] != item.Value)
                                    {
                                        _willUpgrades.Add(item.Key, item.Value);
                                    }
                                }
                                else
                                {
                                    //没有找到
                                    _willUpgrades.Add(item.Key, item.Value);
                                }
                            }
                        }
                    }
                }
                return _willUpgrades;
            }
        }

        /// <summary>
        /// 清空版本信息
        /// </summary>
        public void ClearUpgradeModel()
        {
            if (File.Exists(Local_UpgradeXmlPath))
            {

                try
                {
                    string xmlStr = File.ReadAllText(Local_UpgradeXmlPath);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlStr);

                    XmlNode node = xmlDoc.SelectSingleNode("Upgrade/Files");
                    if (node != null && node.ChildNodes.Count > 0)
                    {
                        node.RemoveAll();
                    }
                    File.WriteAllText(UpgradeHelper.Instance.Local_UpgradeXmlPath, xmlDoc.InnerXml);
                }
                catch (Exception)
                { }
            }
            _local_UpgradeModel = null;
            _server_UpgradeModel = null;
            _willUpgrades = null;
        }



        #region 单例对象

        private static UpgradeHelper _instance;
        /// <summary>
        /// 单例对象
        /// </summary>
        public static UpgradeHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UpgradeHelper();
                    //初始化本地配置文件，以及服务器地址
                    if (_instance.Local_UpgradeModel != null)
                    {
                        _instance.UpgradeUrl = _instance.Local_UpgradeModel.UpgradeUrl;
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 启动主程序
        /// </summary>
        /// <param name="fileName"></param>
        public static bool StartRunMain(string fileName)
        {
            string fullPath = fileName;
            try
            {
                Process process = GetProcess(fileName);
                if (process != null)//以及存在运行中的主进程
                {
                    return true;
                }
                fullPath = string.Format("{0}\\{1}", Application.StartupPath, fileName);

                ProcessStartInfo main = new ProcessStartInfo(fullPath);
                Process.Start(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("主程序{0}调用失败：\n{1}", fullPath, ex.Message), "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="process"></param>
        public static void KillProcess(string processName)
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

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static Process GetProcess(string pName)
        {
            if (string.IsNullOrWhiteSpace(pName)) return null;
            pName = pName.ToLower();
            pName = pName.Replace(".exe", "");
            //杀死主进程
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (!string.IsNullOrWhiteSpace(process.ProcessName))
                {
                    if (process.ProcessName.ToLower() == pName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        #endregion

    }


}
