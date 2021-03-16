using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System;
using System.Web;

namespace AutoUpgrade.Helper
{
    /// <summary>
    /// web请求处理
    /// </summary>
    public class MyWebResquest
    {
        /// <summary>
        /// 获取服务端版本文件  失败抛出异常
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns>版本文件的Xml内容</returns>
        public static string GetUpgradeList(string url)
        {
            try
            {
                HttpWebClient client = new HttpWebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                url = string.Format("{0}/Upgrade/UpgradeList", url);
                return client.DownloadString(url);
            }
            catch (WebException ex)
            {
                //响应不为空异常时，获取服务器返回的错误信息
                if (ex.Response != null)
                {
                    Stream stream = ex.Response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string result = reader.ReadToEnd();
                    throw new Exception(result);
                }
                else
                {
                    Exception baseEx = ex.GetBaseException();
                    if (baseEx != null)
                    {
                        throw baseEx;
                    }
                    else if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 同步下载文件  失败抛出异常
        /// </summary>
        /// <param name="hostUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="localFileName"></param>
        public static void DownloadFile(string hostUrl, string fileName, string localFileName)
        {
            try
            {
                HttpWebClient client = new HttpWebClient();
                string url = string.Format("{0}/Upgrade/DownloadFile?fileName={1}", hostUrl, HttpUtility.UrlEncode(fileName));
                client.DownloadFile(url, localFileName);
            }
            catch (WebException ex)
            {
                //响应不为空异常时，获取服务器返回的错误信息
                if (ex.Response != null)
                {
                    Stream stream = ex.Response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string result = reader.ReadToEnd();
                    throw new Exception(result);
                }
                else
                {
                    Exception baseEx = ex.GetBaseException();
                    if (baseEx != null)
                    {
                        throw baseEx;
                    }
                    else if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    /// <summary>
    /// 重载WebClient
    /// </summary>
    public class HttpWebClient : WebClient
    {
        private string _key;
        /// <summary>
        /// 一次web情况的唯一代码
        /// </summary>
        public string Key
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_key))
                {
                    _key = Guid.NewGuid().ToString("N");
                }
                return _key;
            }
        }

        // Cookie 容器
        private CookieContainer cookieContainer;

        /// <summary>
        /// 创建一个新的 WebClient 实例。
        /// </summary>
        public HttpWebClient()
        {
            this.cookieContainer = new CookieContainer();
        }

        /// <summary>
        /// 创建一个新的 WebClient 实例。
        /// </summary>
        /// <param name="cookie">Cookie 容器</param>
        public HttpWebClient(CookieContainer cookies)
        {
            this.cookieContainer = cookies;
        }

        /// <summary>
        /// Cookie 容器
        /// </summary>
        public CookieContainer Cookies
        {
            get { return this.cookieContainer; }
            set { this.cookieContainer = value; }
        }

        private int _timeout = 5000;
        /// <summary>
        /// 超时时间(以毫秒为单位)
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// 返回带有 Cookie 的 HttpWebRequest。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                HttpWebRequest httpRequest = request as HttpWebRequest;
                httpRequest.CookieContainer = cookieContainer;
                httpRequest.Timeout = Timeout;
            }
            return request;
        }
    }
}
