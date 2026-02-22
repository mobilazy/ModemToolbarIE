using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemWebUtility
{
    /// <summary>
    /// This class is about retreiving information from already existing modem
    /// </summary>
    public class DdBhaParameters
    {
        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();

        private string bhaEditUrl = HDocUtility.BhaEdittUrl;
        private string bhaEditUrlAlt = HDocUtility.BhaEditUrlAlt;

        private string P_DD_ID = "";
        private string modemNo = "";
        private string Z_CHK = "";

        private DdBhaPosts ddBhaPost = new DdBhaPosts();
        private Dictionary<int, DdCompPosts> ddBhaCompPost = new Dictionary<int, DdCompPosts>();
        private int ddBhaCount;
        

        public DdBhaPosts DdBhaPosts { get { return ddBhaPost; } }
        public Dictionary<int, DdCompPosts> DdBhaCompPost { get { return ddBhaCompPost; } }
        public int DdBhaCount { get { return ddBhaCount; } }
        

        public DdBhaParameters(HtmlAgilityPack.HtmlDocument _hDoc)
        {
            hDoc = _hDoc;
            Init();
        }

        public DdBhaParameters(mshtml.HTMLDocument htmlDocument)
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

            if (GetNumberOfTables() > 1)
            {
                ddBhaCompPost = GetDdBhaTable();
                ddBhaCount = ddBhaCompPost.Count;
                
            }
            else
            {
                ddBhaCount = 0;
                
            }

            try
            {
                ddBhaPost.P_MOTOR_NUM = HDocUtility.GetInputById("P_MOTOR_NUM", hDoc);
                ddBhaPost.P_MOTOR_DESC = HDocUtility.GetInputById("P_MOTOR_DESC", hDoc);
                ddBhaPost.P_L_MOBSS_HOLESEC = HDocUtility.GetSelectedElementById("P_L_MOBSS_HOLESEC", hDoc);
                ddBhaPost.P_FLOWRATE_MIN = HDocUtility.GetInputById("P_FLOWRATE_MIN", hDoc);
                ddBhaPost.P_FLOWRATE_MAX = HDocUtility.GetInputById("P_FLOWRATE_MAX", hDoc);
                ddBhaPost.P_FLOW_RATE_UNIT = HDocUtility.GetSelectedElementById("P_FLOW_RATE_UNIT", hDoc);
                ddBhaPost.P_WOB = HDocUtility.GetInputById("P_WOB", hDoc);
                ddBhaPost.P_BIT_PRESSURE_DROP = HDocUtility.GetInputById("P_BIT_PRESSURE_DROP", hDoc);
                ddBhaPost.P_PRES_UNIT = HDocUtility.GetSelectedElementById("P_PRES_UNIT", hDoc);
                ddBhaPost.P_MOTOR_DIFFPRESS = HDocUtility.GetInputById("P_MOTOR_DIFFPRESS", hDoc);
                ddBhaPost.P_MUD_TYPE = HDocUtility.GetInputById("P_MUD_TYPE", hDoc);
                ddBhaPost.P_STATOR_RUBBER = HDocUtility.GetInputById("P_STATOR_RUBBER", hDoc);
                ddBhaPost.P_PLAN_TEMP_START = HDocUtility.GetInputById("P_PLAN_TEMP_START", hDoc);
                ddBhaPost.P_MAX_TEMP = HDocUtility.GetInputById("P_MAX_TEMP", hDoc);
                ddBhaPost.P_TEMP_UNIT = HDocUtility.GetSelectedElementById("P_TEMP_UNIT", hDoc);
                ddBhaPost.P_SLICK_BORE = HDocUtility.GetSelectedElementById("P_SLICK_BORE", hDoc);
                ddBhaPost.P_L_MOBSS_MOTORSIZE = HDocUtility.GetSelectedElementById("P_L_MOBSS_MOTORSIZE", hDoc);
                ddBhaPost.P_L_MOBSS_LOBERATIO = HDocUtility.GetSelectedElementById("P_L_MOBSS_LOBERATIO", hDoc);
                ddBhaPost.P_L_MOTORTYPE = HDocUtility.GetSelectedElementById("P_L_MOTORTYPE", hDoc);
                ddBhaPost.P_L_MOTOR_CATEGORY = HDocUtility.GetSelectedElementById("P_L_MOTOR_CATEGORY", hDoc);
                ddBhaPost.P_FLEXI_STATOR = HDocUtility.GetSelectedElementById("P_FLEXI_STATOR", hDoc);
                ddBhaPost.P_ROTOR_CATCHERS = HDocUtility.GetSelectedElementById("P_ROTOR_CATCHERS", hDoc);
                ddBhaPost.P_L_ROTOR_COATING = HDocUtility.GetSelectedElementById("P_L_ROTOR_COATING", hDoc);
                ddBhaPost.P_L_BEND = HDocUtility.GetSelectedElementById("P_L_BEND", hDoc);
                ddBhaPost.P_ROTOR_NOZZLE = HDocUtility.GetSelectedElementById("P_ROTOR_NOZZLE", hDoc);
                ddBhaPost.P_DUMP_SUB = HDocUtility.GetSelectedElementById("P_DUMP_SUB", hDoc);
                ddBhaPost.P_PIN_DOWN_SHAFT = HDocUtility.GetSelectedElementById("P_PIN_DOWN_SHAFT", hDoc);
                ddBhaPost.P_L_UPHOLE = HDocUtility.GetSelectedElementById("P_L_UPHOLE", hDoc);
                ddBhaPost.P_L_DRIVESHAFT = HDocUtility.GetSelectedElementById("P_L_DRIVESHAFT", hDoc);
                ddBhaPost.P_SLEEVE_FITTED = HDocUtility.GetSelectedElementById("P_SLEEVE_FITTED", hDoc);
                ddBhaPost.P_SLEEVE_GAUGE = HDocUtility.GetInputById("P_SLEEVE_GAUGE", hDoc);
                ddBhaPost.P_PAD = HDocUtility.GetSelectedElementById("P_PAD", hDoc);
                ddBhaPost.P_PAD_SIZE = HDocUtility.GetInputById("P_PAD_SIZE", hDoc);
                ddBhaPost.P_ABI_REQ = HDocUtility.GetSelectedElementById("P_ABI_REQ", hDoc);
                ddBhaPost.P_BIT_TYPE = HDocUtility.GetInputById("P_BIT_TYPE", hDoc);
                ddBhaPost.P_BIT_TORQUED = HDocUtility.GetSelectedElementById("P_BIT_TORQUED", hDoc);
                
                ddBhaPost.P_BIT_MAKEUP_TORQUE = HDocUtility.GetInputById("P_BIT_MAKEUP_TORQUE", hDoc);
                ddBhaPost.P_BIT_SN = HDocUtility.GetInputById("P_BIT_SN", hDoc);
                ddBhaPost.P_COMMENTS = HDocUtility.GetTextAreaById("P_COMMENTS", hDoc);

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

        private Dictionary<int, DdCompPosts> GetDdBhaTable()
        {
            Dictionary<int, DdCompPosts> mcpDic = new Dictionary<int, DdCompPosts>();
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
            int colNum = 5;
            string[,] tempArray = new string[rowCount, colNum];

            foreach (var item in qCell)
            {
                tempArray[item.RowId, item.CellId % colNum] = item.CellText;

            }

            for (int i = 0; i < rowCount; i++)
            {
                DdCompPosts  mcp = new DdCompPosts();
                mcp.P_SEQ_NO = tempArray[i, 0];
                mcp.P_L_MWDTORQUE_TORQUE = tempArray[i, 1];
                mcp.P_L_THREAD_TOP_THREADSIZE = tempArray[i, 2]; 
                mcp.P_L_THREAD_BTM_THREADSIZE = tempArray[i, 3]; 
                mcp.P_DESCRIPTION = tempArray[i, 4]; ;
                
                mcpDic.Add(i, mcp);

            }

            return mcpDic;

        }

        
    }
}
