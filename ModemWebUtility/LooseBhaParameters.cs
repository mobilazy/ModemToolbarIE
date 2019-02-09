using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class LooseBhaParameters
    {
        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();

        private string bhaEditUrl = HDocUtility.BhaEdittUrl;
        private string bhaEditUrlAlt = HDocUtility.BhaEditUrlAlt;

        private string P_LOOSE_ITEMS_ID = "";
        private string modemNo = "";
        private string Z_CHK = "";

        private Dictionary<int, LooseBhaPosts> looseBhaPost = new Dictionary<int, LooseBhaPosts>();
        
        private int looseBhaCount;


        public Dictionary<int, LooseBhaPosts> LooseBhaPosts { get { return looseBhaPost; } }
        public int LooseBhaCount { get { return looseBhaCount; } }


        public LooseBhaParameters(HtmlAgilityPack.HtmlDocument _hDoc)
        {
            hDoc = _hDoc;
            Init();
        }

        public LooseBhaParameters(mshtml.HTMLDocument htmlDocument)
        {
            hDoc.LoadHtml(LoadHtmlAgility(htmlDocument));

            try
            {
                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private string LoadHtmlAgility(mshtml.HTMLDocument htmlDocument)
        {
            mshtml.IHTMLDocument3 idoc = (mshtml.IHTMLDocument3)htmlDocument;
            return idoc.documentElement.outerHTML;

        }

        private void Init()
        {

            if (GetNumberOfTables() > 0)
            {
                looseBhaPost = GetLooseTable();
                looseBhaCount = looseBhaPost.Count;

            }
            else
            {
                looseBhaCount = 0;

            }

        }

        private int GetNumberOfTables()
        {

            var query = from table in hDoc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                        select new { table };

            return query.Count();

        }

        private Dictionary<int, LooseBhaPosts> GetLooseTable()
        {
            Dictionary<int, LooseBhaPosts> mcpDic = new Dictionary<int, LooseBhaPosts>();
            //int tableId = 0;
            //int rowId = 0;
            //int cellId = 0;

            //List<BhaCell> qCell = new List<BhaCell>();

            //foreach (HtmlNode table in hDoc.DocumentNode.SelectNodes("//table"))
            //{


            //    //System.IO.File.AppendAllText(@"C:\Users\h111765\failure_Editor_tableNode.txt", table.OuterHtml.ToString()+ Environment.NewLine + Environment.NewLine);
            //    foreach (HtmlNode row in table.SelectNodes(".//tr"))
            //    {
            //        //System.IO.File.AppendAllText(@"C:\Users\h111765\failure_Editor_rowNode.txt", row.OuterHtml.ToString() + Environment.NewLine);
            //        if (row.InnerHtml.Contains("</th>"))
            //        {
            //            continue;
            //        }
            //        foreach (HtmlNode cell in row.SelectNodes(".//td"))
            //        {
            //            string ct = " ";
            //            if (cell.InnerText != "&nbsp;")
            //            {
            //                ct = System.Net.WebUtility.HtmlDecode(cell.InnerText);
            //            }
            //            qCell.Add(new BhaCell { TableId = tableId, RowId = rowId, CellId = cellId, CellText = ct });

            //            cellId++;
            //        }
            //        rowId++;
            //    }
            //    tableId++;
            //}



            //int rowCount = rowId;
            //int colNum = 8;
            //string[,] tempArray = new string[rowCount, colNum];

            //foreach (var item in qCell)
            //{
            //    tempArray[item.RowId, item.CellId % colNum] = item.CellText;

            //}

            //for (int i = 0; i < rowCount; i++)
            {
                LooseBhaPosts mcp = new LooseBhaPosts();
                mcp.P_QTY = HDocUtility.GetInputById("P_QTY", hDoc);   //tempArray[i, 0];
                mcp.P_L_MOBSS_VEND_VEND_NAME2 = HDocUtility.GetSelectedElementById("P_L_MOBSS_VEND_VEND_NAME2", hDoc);    // tempArray[i, 1];
                mcp.P_DESCRIPTION = HDocUtility.GetInputById("P_DESCRIPTION", hDoc);              //tempArray[i, 2];
                mcp.P_CUST_STAT = HDocUtility.GetSelectedElementById("P_CUST_STAT", hDoc);                 // tempArray[i, 3];
                mcp.P_COMMENTS = HDocUtility.GetInputById("P_COMMENTS", hDoc);            //tempArray[i, 4];
                mcp.P_L_THREADSIZE_TOP = HDocUtility.GetInputById("P_L_THREADSIZE_TOP", hDoc);            //tempArray[i, 5];
                mcp.P_L_THREADSIZE_BTM = HDocUtility.GetInputById("P_L_THREADSIZE_BTM", hDoc);               // tempArray[i, 6];


                mcpDic.Add(0, mcp);

            }

            return mcpDic;

        }

    }
}
