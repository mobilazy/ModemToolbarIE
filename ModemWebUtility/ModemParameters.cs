using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace ModemWebUtility
{
    public class ModemParameters
    {
        public string ModemNo { get; set; }
        public string Zchk { get; set; }
        public int MwdBhaCount { get; set; }
        public int DdBhaCount { get; set; }
        public int GpBhaCount { get; set; }
        public int LooseItemCount { get; set; }

        public List<string> MwdId { get; set; } = new List<string>();
        public List<string> GpId { get; set; } = new List<string>();
        public List<string> DdId { get; set; } = new List<string>();
        public List<string> LooseId { get; set; } = new List<string>();

        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();

        //public ModemParameters()
        //{

        //}

        public ModemParameters(mshtml.HTMLDocument htmlDocument, bool isWithElementName, string ElementName)
        {
            hDoc.LoadHtml(LoadHtmlAgility(htmlDocument));
            ModemNo = HDocUtility.GetInputByName(ElementName, hDoc);
            ModemParametersInit();
        }

        public ModemParameters(HtmlAgilityPack.HtmlDocument htmlDocument, bool isWithElementName, string ElementName)
        {
            hDoc = htmlDocument;
            ModemNo = HDocUtility.GetInputByName(ElementName, hDoc);
            ModemParametersInit();
        }

        public ModemParameters(HtmlAgilityPack.HtmlDocument htmlDocument, string modemNo)
        {
            hDoc = htmlDocument;
            ModemNo = modemNo; 
            ModemParametersInit();
        }

        public ModemParameters(mshtml.HTMLDocument htmlDocument,  string modemNo)
        {

            hDoc.LoadHtml(LoadHtmlAgility(htmlDocument));
            ModemNo = modemNo;
            ModemParametersInit();

        }

        private void ModemParametersInit()
        {
            Zchk = HDocUtility.GetInputByName("Z_CHK", hDoc) ?? HDocUtility.GetInputByName("z_chk", hDoc);

            MwdBhaCount = GetMwdBhaCount();
            DdBhaCount = GetDdBhaCount();
            GpBhaCount = GetGpBhaCount();
            LooseItemCount = GetLooseItemsCount();

        }

        private string LoadHtmlAgility(mshtml.HTMLDocument htmlDocument)
        {
            mshtml.IHTMLDocument3 idoc = (mshtml.IHTMLDocument3)htmlDocument;
            return idoc.documentElement.outerHTML;
        

        }

        private List<string> GetStringFromScript(string keyWord, string scriptText)
        {
            var engine = new V8ScriptEngine();

            engine.AddHostObject("list", new List<string>());
            engine.Execute("function getArray() { " + scriptText + " return " + keyWord + "; } var arr = getArray(); arr.forEach(myFunc); function myFunc(value) {list.Add(value);} ");

            return engine.Script.list;
        }

        private int GetMwdBhaCount()
        {
            var script = hDoc.DocumentNode.Descendants()
                             .Where(n => n.Name == "script");

            var scriptText = "";


            foreach (var item in script)
            {
                string text = item.InnerText;

                while (text.Contains("<!-- Hide from old browsers"))
                {
                    int s = text.IndexOf("<!-- Hide from old browsers");
                    int count = ("<!-- Hide from old browsers").Length;

                    text = text.Remove(s, count);

                }
                while (text.Contains("//-->"))
                {
                    int s = text.IndexOf("//-->");
                    int count = ("//-->").Length;

                    text = text.Remove(s, count);
                }
                while (text.Contains("<!--"))
                {
                    int s = text.IndexOf("<!--");
                    int count = ("<!--").Length;

                    text = text.Remove(s, count);
                }

                if (!text.Contains("P_MWDDWD_ID"))
                {
                    continue;
                }

                scriptText += text;
            }


            try
            {
                MwdId = GetStringFromScript("P_MWDDWD_ID", scriptText);

            }
            catch (Exception)
            {
                MwdId = new List<string>();
            }

            return MwdId.Count;
        }

        private int GetDdBhaCount()
        {
            
            string pattern = @"QueryViewByKey\?P_MOTORS_ID=([0-9]+)";
            string input = hDoc.DocumentNode.OuterHtml;
            MatchCollection matches;
            GroupCollection groups;

            Regex rx = new Regex(pattern);
            matches = rx.Matches(input);

            foreach (Match match in matches)
            {
                groups = match.Groups;
                DdId.Add(groups[1].Value);
            }

            return DdId.Count;
        }

        private int GetGpBhaCount()
        {
            string pattern = @"QueryViewByKey\?P_GP_ID=([0-9]+)";
            string input = hDoc.DocumentNode.OuterHtml;
            MatchCollection matches;
            GroupCollection groups;

            Regex rx = new Regex(pattern);
            matches = rx.Matches(input);

            foreach (Match match in matches)
            {
                groups = match.Groups;
                GpId.Add(groups[1].Value);
            }

            return GpId.Count;
        }

        private int GetLooseItemsCount()
        {
            var script = hDoc.DocumentNode.Descendants()
                             .Where(n => n.Name == "script");

            var scriptText = "";


            foreach (var item in script)
            {
                string text = item.InnerText;

                while (text.Contains("<!-- Hide from old browsers"))
                {
                    int s = text.IndexOf("<!-- Hide from old browsers");
                    int count = ("<!-- Hide from old browsers").Length;

                    text = text.Remove(s, count);

                }
                while (text.Contains("//-->"))
                {
                    int s = text.IndexOf("//-->");
                    int count = ("//-->").Length;

                    text = text.Remove(s, count);
                }
                while (text.Contains("<!--"))
                {
                    int s = text.IndexOf("<!--");
                    int count = ("<!--").Length;

                    text = text.Remove(s, count);
                }

                if (!text.Contains("P_LOOSE_ITEMS_ID"))
                {
                    continue;
                }

                scriptText += text;
            }

            
            try
            {
                LooseId = GetStringFromScript("P_LOOSE_ITEMS_ID", scriptText);
            }
            catch (Exception)
            {
                LooseId = new List<string>();
            }
            return LooseId.Count;
        }

        public string GetElementIdInnerText(string id)
        {
            return HDocUtility.GetInnerTextByTdId(id, hDoc);
        }

    }
}
