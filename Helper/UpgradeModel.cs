using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AutoUpgrade.Helper
{
    public class UpgradeModel
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="xml"></param>
        public void LoadUpgrade(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            //读取UpgradeUrl
            XmlNode node = xmlDoc.SelectSingleNode("//UpgradeUrl");
            if (node != null)
            {
                this.UpgradeUrl = node.InnerText;
            }
            //读取RunMain
            node = xmlDoc.SelectSingleNode("//RunMain");
            if (node != null)
            {
                this.RunMain = node.InnerText;
            }
            //读取RunMain
            node = xmlDoc.SelectSingleNode("//AutoUpgrade");
            if (node != null)
            {
                this.AutoUpgrade = node.InnerText;
            }
            //读取Files
            node = xmlDoc.SelectSingleNode("Upgrade/Files");
            this.DictFiles = new Dictionary<string, string>();
            if (node != null && node.ChildNodes.Count > 0)
            {
                foreach (XmlNode item in node.ChildNodes)
                {
                    if (item.Name != "#comment")
                    {
                        string name = GetNodeAttrVal(item, "Name");
                        string version = GetNodeAttrVal(item, "Version");
                        if (!this.DictFiles.ContainsKey(name))
                        {
                            this.DictFiles.Add(name, version);
                        }
                    }
                }
            }
        }

        private string GetNodeAttrVal(XmlNode node, string attr)
        {
            if (node != null && node.Attributes != null && node.Attributes[attr] != null)
            {
                string val = node.Attributes[attr].Value;
                if (!string.IsNullOrWhiteSpace(val))
                {
                    return val.Trim();
                }
                return val;
            }
            return string.Empty;
        }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string UpgradeUrl { get; set; }

        /// <summary>
        /// 更新完成后运行的主程序名称
        /// </summary>
        public string RunMain { get; set; }

        /// <summary>
        /// 更新程序名称
        /// </summary>
        public string AutoUpgrade { get; set; }

        /// <summary>
        /// 文件列表
        /// string 文件名
        /// string 版本号
        /// </summary>
        public Dictionary<string, string> DictFiles { get; set; }
    }
}
