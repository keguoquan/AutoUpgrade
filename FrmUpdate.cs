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
        /// ���캯��
        /// </summary>
        /// <param name="tempPath"></param>
        /// <param name="updateFiles"></param>
        public FrmUpdate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                //���ط�������ַ
                txtHostUrl.Text = UpgradeHelper.Instance.UpgradeUrl;
                BeginUpgrade();
            }
            catch (Exception ex)
            {
                Output("��ʼ���쳣��" + ex.Message);
            }
        }

        /// <summary>
        /// �ֶ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butBegin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHostUrl.Text))
                {
                    Output("���������������ַ��");
                    return;
                }
                UpgradeHelper.Instance.UpgradeUrl = txtHostUrl.Text.Trim();
                //����汾��Ϣ �Ա����¼��汾
                UpgradeHelper.Instance.ClearUpgradeModel();
                BeginUpgrade();
            }
            catch (Exception ex)
            {
                Output("�����쳣��" + ex.Message);
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
                    Output("����ķ�������ַ����ַ������http://����https://��ͷ");
                    return;
                }
                //�ж��Ƿ��и���
                if (UpgradeHelper.Instance.WillUpgrades.Count > 0 && UpgradeHelper.Instance.Server_UpgradeModel != null)
                {
                    SetWinControl(false);
                    //ɱ��������
                    UpgradeHelper.KillProcess(UpgradeHelper.Instance.Server_UpgradeModel.RunMain);
                    RunUpgrade();//��������
                }
            }
            catch (Exception ex)
            {
                Output("�����쳣��" + ex.Message);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void RunUpgrade()
        {
            //��������
            SetCaption(string.Format("��������ļ�{0}�����Ѹ���0�������ڸ��������ļ���", UpgradeHelper.Instance.WillUpgrades.Count));
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
                            //�����ǰ�ļ�Ϊ����������
                            filePath = string.Format("{0}\\AutoUpgradeTemp\\{1}", Application.StartupPath, item.Key);
                        }
                        string directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        MyWebResquest.DownloadFile(UpgradeHelper.Instance.UpgradeUrl, item.Key, filePath);
                        idx++;
                        SetCaption(string.Format("��������ļ�{0}�����Ѹ���{1}���������ļ��б�", UpgradeHelper.Instance.WillUpgrades.Count, idx));
                        Output(string.Format("�����ļ�{0}���", curFile));
                    }
                    //����汾�ļ�
                    File.WriteAllText(UpgradeHelper.Instance.Local_UpgradeXmlPath, UpgradeHelper.Instance.Server_UpgradeXml);

                    SetCaption(string.Format("������ɣ��������ļ�{0}��", UpgradeHelper.Instance.WillUpgrades.Count));
                    Output(string.Format("������ɣ��������ļ�{0}��", UpgradeHelper.Instance.WillUpgrades.Count));

                    //������ɺ���
                    UpgradeHelper.StartRunMain(UpgradeHelper.Instance.Server_UpgradeModel.RunMain);

                    //�˳���ǰ����
                    ExitCurrent();
                }
                catch (Exception ex)
                {
                    Output(string.Format("�����ļ�{0}�쳣��{1}", curFile, ex.Message));
                    SetWinControl(true);
                }
            });
        }

        /// <summary>
        /// ���ý���ؼ��Ƿ����
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
        /// �˳���ǰ����
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

        #region ��־���

        /// <summary>
        /// ���ø���״̬
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
        /// ���ø���״̬
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
                txtLog.AppendText(string.Format("{0}��{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), log));
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
                if (MessageBox.Show("����δ��ɣ��˳��󽫵�������޷�����ʹ�ã���ȷ��Ҫ�˳���", "�˳���ʾ", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                {
                    //ȡ��"�رմ���"�¼�
                    e.Cancel = true;
                }
            }
        }
    }
}