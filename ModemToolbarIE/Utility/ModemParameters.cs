using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace ModemToolbarIE.Utility
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

        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlDocument();

        public ModemParameters(mshtml.HTMLDocument htmlDocument, bool isWithElementName, string ElementName)
        {
            hDoc.LoadHtml(LoadHtmlAgility(htmlDocument));
            ModemNo = GetElementByName(ElementName);
            ModemParametersInit();
        }

        public ModemParameters(HtmlAgilityPack.HtmlDocument htmlDocument, bool isWithElementName, string ElementName)
        {
            hDoc = htmlDocument;
            ModemNo = GetElementByName(ElementName);
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
            Zchk = GetElementByName("Z_CHK") ?? GetElementByName("z_chk");

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

                if (!text.Contains("P_MOTORS_ID"))
                {
                    continue;
                }

                scriptText += text;
            }

            
            try
            {
                DdId = GetStringFromScript("P_MOTORS_ID", scriptText);
            }
            catch (Exception)
            {

                DdId = new List<string>();
            }

            return DdId.Count;
        }

        private int GetGpBhaCount()
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

                if (!text.Contains("P_GP_ID"))
                {
                    continue;
                }

                scriptText += text;
            }

            
            try
            {
                GpId = GetStringFromScript("P_GP_ID", scriptText);
            }
            catch (Exception)
            {
                GpId = new List<string>();
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

        public string GetElementByName(string name)
        {
            string value1 = hDoc.DocumentNode.SelectSingleNode("//input[@name='" + name + "']")
                             .Attributes["value"].Value;

            return value1;

        }

        public string GetElementByType(string name)
        {
            string value1 = hDoc.DocumentNode.SelectSingleNode("//input[@type='" + name + "']")
                             .Attributes["value"].Value;

            return value1;

        }

        public string GetSelectElementByName(string name)
        {
            string value1 = hDoc.DocumentNode.SelectSingleNode("//select[@name='" + name + "']")
                             .Attributes["value"].Value;

            return value1;

        }

        public string GetTextAreaByName(string name)
        {
            string value1 = hDoc.DocumentNode.SelectSingleNode("//textarea[@name='" + name + "']")
                             .Attributes["value"].Value;

            return value1;

        }

        public string GetElementById(string elementId)
        {
            string value1 = hDoc.DocumentNode.SelectSingleNode("//input[@id='" + elementId + "']")
                             .Attributes["value"].Value;

            return value1;

        }

        public string GetElementByIdReturnValue(string elementId, string value)
        {

            string value1;

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//input[@id='" + elementId + "']")
                             .Attributes[value].Value;
            }
            catch (Exception e)
            {
                value1 = "";
            }

            return value1;

        }

    }
}
