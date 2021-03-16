using System.Windows.Forms;

namespace AutoUpdate
{
    partial class FrmUpdate
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdate));
            this.lblCaption = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtHostUrl = new System.Windows.Forms.TextBox();
            this.butBegin = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCaption.Location = new System.Drawing.Point(5, 43);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(101, 12);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "升级状态：未开始";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(3, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "服务器地址：";
            // 
            // txtHostUrl
            // 
            this.txtHostUrl.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtHostUrl.Location = new System.Drawing.Point(75, 11);
            this.txtHostUrl.Name = "txtHostUrl";
            this.txtHostUrl.Size = new System.Drawing.Size(324, 21);
            this.txtHostUrl.TabIndex = 7;
            // 
            // butBegin
            // 
            this.butBegin.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.butBegin.Location = new System.Drawing.Point(404, 8);
            this.butBegin.Name = "butBegin";
            this.butBegin.Size = new System.Drawing.Size(75, 28);
            this.butBegin.TabIndex = 8;
            this.butBegin.Text = "开始更新";
            this.butBegin.UseVisualStyleBackColor = true;
            this.butBegin.Click += new System.EventHandler(this.butBegin_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(5, 61);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(521, 247);
            this.txtLog.TabIndex = 9;
            // 
            // FrmUpdate
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(532, 313);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.txtHostUrl);
            this.Controls.Add(this.butBegin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自动升级程序";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmUpdate_FormClosing);
            this.Load += new System.EventHandler(this.FrmUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label lblCaption;
        private Label label1;
        private TextBox txtHostUrl;
        private Button butBegin;
        private TextBox txtLog;
    }
}