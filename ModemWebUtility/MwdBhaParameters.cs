using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mshtml;

using System.Windows.Forms;

namespace ModemWebUtility
{
    public class MwdBhaParameters
    {
        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();

        private string bhaEditUrl = HDocUtility.BhaEdittUrl;
        private string bhaEditUrlAlt = HDocUtility.BhaEditUrlAlt;

        private string P_MWDDWD_ID = "";
        private string modemNo = "";
        private string Z_CHK = "";
        private MwdBhaPosts mwdBhaPost = new MwdBhaPosts(); 
        private Dictionary<int, MwdCompPosts> bhaCompPost = new Dictionary<int, MwdCompPosts>();
        private Dictionary<int, MwdSoftPosts> bhaSoftPost = new Dictionary<int, MwdSoftPosts>();
        private int bhaCount;
        private int softCount;
        //private string bhaDescription = "";
        //private string bhaHardConnect = "No";
        //private string bhaAdditionalInfo = "";

        public MwdBhaPosts MwdBhaPost { get { return mwdBhaPost; } } 
        public Dictionary<int, MwdCompPosts> BhaCompPost { get { return bhaCompPost; } }
        public Dictionary<int, MwdSoftPosts> BhaSoftPost { get { return bhaSoftPost; } }
        public int BhaCount { get { return bhaCount; } }
        public int SoftCount { get { return softCount; } }
        //public string BhaDescription { get { return bhaDescription; } }
        //public string BhaHardConnect { get { return bhaHardConnect; } }
        //public string BhaAdditionalInfo { get { return bhaAdditionalInfo; } }

        public MwdBhaParameters(HtmlAgilityPack.HtmlDocument _hDoc)
        {
            hDoc = _hDoc;
            Init();
        }

        public MwdBhaParameters(mshtml.HTMLDocument htmlDocument)
        {
            hDoc.LoadHtml(LoadHtmlAgility(htmlDocument));

            try
            {
                Init();
            }
            catch (Exception ex)
            {
                // Error during initialization - silently continue
            }

            
        }

        private string LoadHtmlAgility(mshtml.HTMLDocument htmlDocument)
        {
            mshtml.IHTMLDocument3 idoc = (mshtml.IHTMLDocument3)htmlDocument;
            return idoc.documentElement.outerHTML;

        }

        private void Init()
        {
            
            if (GetNumberOfTables()>2)
            {
                bhaCompPost = GetMwdBhaTable();
                bhaCount = bhaCompPost.Count;
                bhaSoftPost = GetMwdSoftTable();
                softCount = bhaSoftPost.Count;

            }
            else if (GetNumberOfTables() > 1)
            {
                bhaCompPost = GetMwdBhaTable();
                bhaCount = bhaCompPost.Count;
                softCount = 0;
            }
            else
            {
                bhaCount = 0;
                softCount = 0;
            }

            try
            {
                //bhaDescription = HDocUtility.GetInputByName("O_BHA_DESC", hDoc);
                mwdBhaPost.P_BHA_DESC = HDocUtility.GetInputByName("O_BHA_DESC", hDoc);
                mwdBhaPost.P_MWDDWD_ADD_INFO = HDocUtility.GetInputByName("O_MWDDWD_ADD_INFO", hDoc);
                string hc = HDocUtility.GetInputByName("O_HC_TOOL", hDoc);
                if (hc == "1")
                {
                    mwdBhaPost.P_HC_TOOL = "Yes";
                }
                else
                {
                    mwdBhaPost.P_HC_TOOL = "No";
                }

                //bhaAdditionalInfo = HDocUtility.GetInputByName("O_MWDDWD_ADD_INFO", hDoc);
                mwdBhaPost.P_EXP_MAX_TEMP = "";
                mwdBhaPost.P_L_IMPELLERSIZE = HDocUtility.GetSelectedElementById("P_L_IMPELLERSIZE", hDoc);
                mwdBhaPost.P_L_ORIFFICESIZE = HDocUtility.GetSelectedElementById("P_L_ORIFFICESIZE", hDoc); ;
                mwdBhaPost.P_L_STATORSIZE = HDocUtility.GetSelectedElementById("P_L_STATORSIZE", hDoc); ;
                mwdBhaPost.P_POPPET_STANDOFF = HDocUtility.GetInputById("P_L_STATORSIZE", hDoc);
                mwdBhaPost.P_PULSER_OILTYPE = "";
                mwdBhaPost.P_RASOURCE_ID = HDocUtility.GetSelectedElementById("P_RASOURCE_ID", hDoc); ;
            }
            catch (Exception)
            {

            }
            

        }

        private int GetNumberOfTables()
        {
            
            var query = from table in hDoc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                        select new { table };

            return query.Count();

        }

        private Dictionary<int, MwdCompPosts> GetMwdBhaTable()
        {
            Dictionary<int, MwdCompPosts> mcpDic = new Dictionary<int, MwdCompPosts>();
            int tableId = 0;
            int rowId = 0;
            int cellId = 0;

            List<BhaCell> qCell = new List<BhaCell>();

            foreach (HtmlNode table in hDoc.DocumentNode.SelectNodes("//table"))
            {

                if (tableId != 1)
                {
                    tableId++;
                    continue;
                }
                //System.IO.File.AppendAllText(@"C:\Users\h111765\failure_Editor_tableNode.txt", table.OuterHtml.ToString()+ Environment.NewLine + Environment.NewLine);
                foreach (HtmlNode row in table.SelectNodes(".//tr"))
                {
                    //System.IO.File.AppendAllText(@"C:\Users\h111765\failure_Editor_rowNode.txt", row.OuterHtml.ToString() + Environment.NewLine);
                    if (row.InnerHtml.Contains("</th>"))
                    {
                        continue;
                    }
                    foreach (HtmlNode cell in row.SelectNodes(".//td"))
                    {
                        string ct = " ";
                        if (cell.InnerText != "&nbsp;")
                        {
                            ct = System.Net.WebUtility.HtmlDecode(cell.InnerText);
                        }
                        qCell.Add(new BhaCell { TableId = tableId, RowId = rowId, CellId = cellId, CellText = ct });

                        cellId++;
                    }
                    rowId++;
                }
                tableId++;
            }



            int rowCount = rowId;
            int colNum = 6;
            string[,] tempArray = new string [rowCount, colNum] ;

            foreach (var item in qCell)
            {
                tempArray[item.RowId,item.CellId% colNum] = item.CellText;
            
            }

            for (int i = 0; i < rowCount; i++)
            {
                MwdCompPosts mcp = new MwdCompPosts();
                mcp.P_SEQ_NO = tempArray[i, 0];
                mcp.P_L_TORQUE = tempArray[i, 1];
                mcp.P_L_THREAD_TOP = tempArray[i, 2]; ;
                mcp.P_L_THREAD_BTM = tempArray[i, 3]; ;
                mcp.P_DESCRIPTION = tempArray[i, 4]; ;
                mcp.P_COMMENTS = tempArray[i, 5];
                mcpDic.Add(i, mcp);

            }

            return mcpDic;

        }

        private Dictionary<int, MwdSoftPosts> GetMwdSoftTable()
        {
            Dictionary<int, MwdSoftPosts> mcpDic = new Dictionary<int, MwdSoftPosts>();
            int tableId = 0;
            int rowId = 0;
            int cellId = 0;

            List<BhaCell> qCell = new List<BhaCell>();

            foreach (HtmlNode table in hDoc.DocumentNode.SelectNodes("//table"))
            {

                if (tableId != 2)
                {
                    tableId++;
                    continue;
                }
                //System.IO.File.AppendAllText(@"C:\Users\h111765\failure_Editor_tableNode.txt", table.OuterHtml.ToString() + Environment.NewLine + Environment.NewLine);
                foreach (HtmlNode row in table.SelectNodes(".//tr"))
                {
                    //System.IO.File.AppendAllText(@"C:\Users\h111765\failure_Editor_rowNode.txt", row.OuterHtml.ToString() + Environment.NewLine);
                    if (row.InnerHtml.Contains("</th>"))
                    {
                        continue;
                    }
                    foreach (HtmlNode cell in row.SelectNodes(".//td"))
                    {
                        string ct = " ";
                        if (cell.InnerText != "&nbsp;")
                        {
                            ct = System.Net.WebUtility.HtmlDecode( cell.InnerText);
                        }
                        qCell.Add(new BhaCell { TableId = tableId, RowId = rowId, CellId = cellId, CellText = ct });

                        cellId++;
                    }
                    rowId++;
                }
                tableId++;
            }


            int rowCount = rowId;
            int colNum = 3;
            string[,] tempArray = new string[rowCount, colNum];

            foreach (var item in qCell)
            {
                tempArray[item.RowId, item.CellId % colNum] = item.CellText;

            }

            for (int i = 0; i < rowCount; i++)
            {
                MwdSoftPosts mcp = new MwdSoftPosts();
                mcp.P_L_MSR_SENSOR = tempArray[i,0];
                mcp.P_OPS_VERSION = tempArray[i, 1];
                mcp.P_WS_VERSION = "";

                mcpDic.Add(i, mcp);

            }

            return mcpDic;
        }

    }


    public struct BhaCell
    {
        public int TableId;
        public int RowId;
        public int CellId;
        public HtmlNode CellHtml;
        public string CellText;
    }
}
