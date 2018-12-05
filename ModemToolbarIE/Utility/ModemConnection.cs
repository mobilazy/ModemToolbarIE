using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModemToolbarIE.Utility
{
    public class ModemConnection
    {
        const string UrlView = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID=";

        public string ModemNumber { get; set; } = "";

        public ModemConnection(string _modemNumber)
        {
            ModemNumber = _modemNumber;
        }

        public string GetHtmlAsString()
        {
            if (ModemNumber == "")
            {
                return "";
            }

            string url = UrlView + ModemNumber;
            string result = "";
            WebResponse response = null;
            StreamReader reader = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.PreAuthenticate = true;
                request.Method = "GET";

                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }

            return result;

        }

        public HtmlAgilityPack.HtmlDocument GetHtmlAsHdoc()
        {
            HtmlAgilityPack.HtmlDocument hDoc = new HtmlDocument();
            string htmlString = GetHtmlAsString();
            hDoc.LoadHtml(htmlString);

            return hDoc;
        }

    }
}
