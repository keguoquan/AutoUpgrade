using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using AutoUpgrade.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoUpdate
{
    public partial class FrmUpdate : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tempPath"></param>
        /// <param name="updateFiles"></param>
        public FrmUpdate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                //加载服务器地址
                txtHostUrl.Text = UpgradeHelper.Instance.UpgradeUrl;
                BeginUpgrade();
            }
            catch (Exception ex)
            {
                Output("初始化异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 手动更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butBegin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHostUrl.Text))
                {
                    Output("请先输入服务器地址！");
                    return;
                }
                UpgradeHelper.Instance.UpgradeUrl = txtHostUrl.Text.Trim();
                //清理版本信息 以便重新检测版本
                UpgradeHelper.Instance.ClearUpgradeModel();
                BeginUpgrade();
            }
            catch (Exception ex)
            {
                Output("更新异常：" + ex.Message);
            }
        }

        private void BeginUpgrade()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UpgradeHelper.Instance.UpgradeUrl))
                {
                    return;
                }
                if (!(UpgradeHelper.Instance.UpgradeUrl.StartsWith("http://") || UpgradeHelper.Instance.UpgradeUrl.StartsWith("https://")))
                {
                    Output("错误的服务器地址，地址必须以http://或者https://开头");
                    return;
                }
                //判断是否有更新
                if (UpgradeHelper.Instance.WillUpgrades.Count > 0 && UpgradeHelper.Instance.Server_UpgradeModel != null)
                {
                    SetWinControl(false);
                    //杀死主进程
                    UpgradeHelper.KillProcess(UpgradeHelper.Instance.Server_UpgradeModel.RunMain);
                    RunUpgrade();//启动更新
                }
            }
            catch (Exception ex)
            {
                Output("更新异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 启动更新
        /// </summary>
        private void RunUpgrade()
        {
            //启动更新
            SetCaption(string.Format("共需更新文件{0}个，已更新0个。正在更新下列文件：", UpgradeHelper.Instance.WillUpgrades.Count));
            Task.Factory.StartNew(() =>
            {
                string curFile = "";
                try
                {
                    int idx = 0;
                    foreach (KeyValuePair<string, string> item in UpgradeHelper.Instance.WillUpgrades)
                    {
                        curFile = item.Key;
                        string filePath = string.Format("{0}\\{1}", Application.StartupPath, item.Key);
                        if (item.Key.IndexOf(UpgradeHelper.Instance.Server_UpgradeModel.AutoUpgrade) >= 0)
                        {
                            //如果当前文件为更新主程序
                            filePath = string.Format("{0}\\AutoUpgradeTemp\\{1}", Application.StartupPath, item.Key);
                        }
                        string directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        MyWebResquest.DownloadFile(UpgradeHelper.Instance.UpgradeUrl, item.Key, filePath);
                        idx++;
                        SetCaption(string.Format("共需更新文件{0}个，已更新{1}个。更新文件列表：", UpgradeHelper.Instance.WillUpgrades.Count, idx));
                        Output(string.Format("更新文件{0}完成", curFile));
                    }
                    //保存版本文件
                    File.WriteAllText(UpgradeHelper.Instance.Local_UpgradeXmlPath, UpgradeHelper.Instance.Server_UpgradeXml);

                    SetCaption(string.Format("更新完成，共更新文件{0}个", UpgradeHelper.Instance.WillUpgrades.Count));
                    Output(string.Format("更新完成，共更新文件{0}个", UpgradeHelper.Instance.WillUpgrades.Count));

                    //下载完成后处理
                    UpgradeHelper.StartRunMain(UpgradeHelper.Instance.Server_UpgradeModel.RunMain);

                    //退出当前程序
                    ExitCurrent();
                }
                catch (Exception ex)
                {
                    Output(string.Format("更新文件{0}异常：{1}", curFile, ex.Message));
                    SetWinControl(true);
                }
            });
        }

        /// <summary>
        /// 设置界面控件是否可用
        /// </summary>
        /// <param name="enabled"></param>
        private void SetWinControl(bool enabled)
        {
            if (this.InvokeRequired)
            {
                Action<bool> d = new Action<bool>(SetWinControl);
                this.Invoke(d, enabled);
            }
            else
            {
                txtHostUrl.Enabled = enabled;
                butBegin.Enabled = enabled;
            }
        }

        /// <summary>
        /// 退出当前程序
        /// </summary>
        private void ExitCurrent()
        {
            if (this.InvokeRequired)
            {
                Action d = new Action(ExitCurrent);
                this.Invoke(d);
            }
            else
            {
                Application.Exit();
            }
        }

        #region 日志输出

        /// <summary>
        /// 设置跟踪状态
        /// </summary>
        /// <param name="caption"></param>
        private void SetCaption(string caption)
        {
            if (this.lblCaption.InvokeRequired)
            {
                Action<string> d = new Action<string>(SetCaption);
                this.Invoke(d, caption);
            }
            else
            {
                this.lblCaption.Text = caption;
            }
        }

        /// <summary>
        /// 设置跟踪状态
        /// </summary>
        /// <param name="caption"></param>
        private void Output(string log)
        {
            if (this.txtLog.InvokeRequired)
            {
                Action<string> d = new Action<string>(Output);
                this.Invoke(d, log);
            }
            else
            {
                txtLog.AppendText(string.Format("{0}：{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), log));
                txtLog.ScrollToCaret();
            }
        }

        private void ClearOutput()
        {
            if (this.txtLog.InvokeRequired)
            {
                Action d = new Action(ClearOutput);
                this.Invoke(d);
            }
            else
            {
                txtLog.Text = "";
            }
        }

        #endregion

        private void FrmUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("升级未完成，退出后将导致软件无法正常使用，你确定要退出吗？", "退出提示", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                {
                    //取消"关闭窗口"事件
                    e.Cancel = true;
                }
            }
        }
    }
}