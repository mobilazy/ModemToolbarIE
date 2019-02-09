using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace ModemWebUtility
{
    public static class HDocUtility
    {
        public static string UrlMwdInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$bha_mc.actioninsert";
        public static string UrlModemView = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_vieword$order_mc.QueryViewByKey?P_SSORD_ID=";
        public static string UrlMwdComponentInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$bhaitm_mc.actioninsert";
        public static string UrlMwdSoftInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$mc_swversion.actioninsert	";
        public static string BhaEdittUrl = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$bha_mc.QueryViewByKey";
        public static string BhaEditUrlAlt = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$bha_mc.actionview";
        public static string BhaEditUrlwMwdId = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$bha_mc.QueryViewByKey?P_MWDDWD_ID=";
        public static string UrlModemEdit = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$header_mc.QueryViewByKey?P_SSORD_ID=";
        public static string UrlModemHeaderUpdate = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$header_mc.actionview";
        public static string UrlLooseItemEdit = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$items_mc.QueryViewByKey?P_LOOSE_ITEMS_ID=";
        public static string UrlGpBhaEdit = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$gp_mc.QueryViewByKey?P_GP_ID=";
        public static string UrlDdBhaEdit = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$motor_mc.QueryViewByKey?P_MOTORS_ID=";
        public static string UrlLooseItemInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$items_mc.actioninsert";
        public static string UrlGpBhaInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$gp_mc.actionview";
        public static string UrlGpCompInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$gpitm_mc.actioninsert";
        public static string UrlDdBhaInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$motor_mc.actioninsert";
        public static string UrlDdCompInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_order_new$motoritm_mc.actioninsert";
        public static string UrlGpInsert = @"http://tanwebs.corp.halliburton.com/pls/log_web/mobssus_gp.InsertDefaultGP?p_ssord_id=";

        public static string ConvertMshtmlToString(mshtml.HTMLDocument htmlDocument)
        {
            mshtml.IHTMLDocument3 idoc = (mshtml.IHTMLDocument3)htmlDocument;
            return idoc.documentElement.outerHTML;


        }

        public static string GetInputByName(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//input[@name='" + name + "']")
                             .Attributes["value"].Value;
            }
            catch (NullReferenceException)
            {

            }


            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static List<string> GetValueListOfInputsByName(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            HtmlNodeCollection collection = hDoc.DocumentNode.SelectNodes("//input[@name='" + name + "']");

            List<string> stringCollection = new List<string>();

            foreach (var item in collection)
            {
                string value1 = "";

                try
                {
                    value1 = System.Net.WebUtility.HtmlDecode(item.Attributes["value"].Value);
                }
                catch (NullReferenceException)
                {

                }
                stringCollection.Add(value1);
            }


            return stringCollection;

        }

        public static string GetOneFromListValueOfInputsByName(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value = "";

            foreach (var item in GetValueListOfInputsByName(name, hDoc))
            {
                if (String.IsNullOrEmpty(item))
                {
                    continue;
                }
                else
                {
                    value = item;
                    break;
                }
            }



            return value;

        }

        public static string GetUniqueFromListValueOfInputsByName(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value = "";



            foreach (var item in GetValueListOfInputsByName(name, hDoc).Distinct())
            {
                if (GetValueListOfInputsByName(name, hDoc).Count(t => (t == item)) == 1)
                {
                    value = item;
                    break;
                }
                else
                {
                    continue;
                }
            }


            return value;

        }

        public static string GetInputByType(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//input[@type='" + name + "']")
                             .Attributes["value"].Value;
            }
            catch (NullReferenceException)
            {

            }


            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static List<string> GetValueListOfInputsByType(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {

            HtmlNodeCollection collection = hDoc.DocumentNode.SelectNodes("//input[@type='" + name + "']");

            List<string> stringCollection = new List<string>();

            foreach (var item in collection)
            {
                string value1 = "";

                try
                {
                    value1 = System.Net.WebUtility.HtmlDecode(item.Attributes["value"].Value);
                }
                catch (NullReferenceException)
                {

                }

                stringCollection.Add(value1);
            }


            return stringCollection;

        }

        public static string GetOneFromListValueOfInputsByType(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value = "";

            foreach (var item in GetValueListOfInputsByType(name, hDoc))
            {
                if (String.IsNullOrEmpty(item))
                {
                    continue;
                }
                else
                {
                    value = item;
                    break;
                }
            }



            return value;

        }

        public static string GetSelectedElementByName(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//select[@name='" + name + "']/option[@selected='selected']")
                             .Attributes["value"].Value;
            }
            catch (NullReferenceException)
            {

            }



            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static string GetSelectedElementById(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//select[@id='" + name + "']/option[@selected='selected']")
                             .Attributes["value"].Value;
            }
            catch (NullReferenceException)
            {

            }

            //MessageBox.Show(System.Net.WebUtility.HtmlDecode(value1));
            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static string GetTextAreaByName(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//textarea[@name='" + name + "']").InnerText;
            }
            catch (NullReferenceException)
            {

            }


            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static string GetTextAreaById(string name, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//textarea[@id='" + name + "']").InnerText;
                             
            }
            catch (NullReferenceException)
            {

            }


            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static string GetInputById(string elementId, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//input[@id='" + elementId + "']")
                             .Attributes["value"].Value;
            }
            catch (NullReferenceException)
            {

            }


            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static string GetInnerTextByTdId(string elementId, HtmlAgilityPack.HtmlDocument hDoc)
        {
            string value1 = "";

            try
            {
                value1 = hDoc.DocumentNode.SelectSingleNode("//td[@id='" + elementId + "']").InnerText;
            }
            catch (NullReferenceException)
            {

            }

            return System.Net.WebUtility.HtmlDecode(value1);

        }

        public static string GetInputByIdReturnValue(string elementId, string value, HtmlAgilityPack.HtmlDocument hDoc)
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

            return System.Net.WebUtility.HtmlDecode(value1);

        }
    }
}
