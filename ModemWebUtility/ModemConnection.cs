using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class ModemConnection
    {
        string UrlView = "";

        //public string ModemNumber { get; set; } = "";

        public ModemConnection(string _url)
        {
            UrlView = _url;
        }

        public string AddGpAndGetResponse()
        {

            string url = UrlView;
            string result = "";
            string header = "";
            HttpWebResponse response = null;


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.PreAuthenticate = true;
                request.Method = "GET";
                request.AllowAutoRedirect = false;

                response = (HttpWebResponse)request.GetResponse();
                header = response.Headers.Get("Location");

            }
            catch (NotSupportedException e)
            { }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {

                if (response != null)
                    response.Close();
            }

            return header;
        }

        public string GetHtmlAsString()
        {
            
            string url = UrlView;
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

                //reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                reader = new StreamReader(response.GetResponseStream(), HDocUtility.CurrentEncoding);
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
            HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();
            string htmlString = GetHtmlAsString();
            hDoc.LoadHtml(htmlString);

            return hDoc;
        }

    }
}
