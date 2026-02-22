using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ModemWebUtility
{
    public class ModemDataPost
    {
        public string url { get; set; }

        private Dictionary<int, Tuple<string, string>> _postKeys = new Dictionary<int, Tuple<string, string>>();
        private string _postData;
        private readonly CookieContainer m_container = new CookieContainer();
        private WebResponse _response;
        private string _result;
        public int StatusCode { get; private set; }

        public ModemDataPost(string _url)
        {
            url = _url;

        }

        public bool Connect(string _url)
        {
            WebResponse response = null;
            bool success = false;
            this.url = _url;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                //request.Credentials = BuildCredentials(this.url);
                request.PreAuthenticate = true;
                request.Method = "GET";
                request.CookieContainer = m_container;

                response = request.GetResponse();
                _response = response;

                success = true;

            }
            catch (Exception ex)
            {
                // handle error
                success = false;

                response.Close();
                //System.IO.File.AppendAllText(@"C:\failure_Editor_Exceptions.txt", ex.ToString() + Environment.NewLine);

            }

            return success;
        }

        private bool postBuilder(Dictionary<int, Tuple<string, string>> postKeys)
        {
            string postData = null;
            int counter = 0;

            foreach (KeyValuePair<int, Tuple<string, string>> item in postKeys)
            {
                counter++;
                if (counter == postKeys.Count)
                {
                    postData += HttpUtility.UrlEncode(item.Value.Item1, HDocUtility.CurrentEncoding) + "=" + HttpUtility.UrlEncode(item.Value.Item2, HDocUtility.CurrentEncoding);
                }
                else
                {
                    postData += HttpUtility.UrlEncode(item.Value.Item1, HDocUtility.CurrentEncoding) + "=" + HttpUtility.UrlEncode(item.Value.Item2, HDocUtility.CurrentEncoding) + "&";
                }

            }

            _postData = postData;

            return true;
        }



        public void AddPostKeys(string key, string value)
        {
            Tuple<string, string> postPairs = new Tuple<string, string>(key, value);
            _postKeys.Add(_postKeys.Count + 1, postPairs);

        }

        private bool PostKeys()
        {
            return postBuilder(_postKeys);
        }

        public bool PostData(string referer = "null")
        {
            string result = "";
            PostKeys();
            byte[] byteArrayDefault = Encoding.Default.GetBytes(_postData);
            byte[] byteArray = Encoding.Convert(Encoding.Default, HDocUtility.CurrentEncoding, byteArrayDefault);
            StreamReader reader = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.url);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.PreAuthenticate = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = m_container;
                request.AllowAutoRedirect = false; // Don't follow redirects automatically so we can detect 302
                if (referer != "null")
                {
                    request.Referer = referer;
                }

                request.ContentLength = byteArray.Length;
                request.CookieContainer = m_container;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                _response = request.GetResponse();
                
                // Capture HTTP status code
                if (_response is HttpWebResponse httpResponse)
                {
                    StatusCode = (int)httpResponse.StatusCode;
                }
                
                reader = new StreamReader(_response.GetResponseStream(), HDocUtility.CurrentEncoding);
                //reader = new StreamReader(_response.GetResponseStream(), Encoding.UTF8);
                _result = reader.ReadToEnd();


            }
            catch (Exception ex)
            {
                // handle error

               // System.IO.File.AppendAllText(@"C:\Failure_Eceptions.txt", ex.ToString() + Environment.NewLine);
            }


            return true;
        }
    }
}
