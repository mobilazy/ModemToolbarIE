using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class GpBhaParameters
    {
        private HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();

        private string bhaEditUrl = HDocUtility.BhaEdittUrl;
        private string bhaEditUrlAlt = HDocUtility.BhaEditUrlAlt;

        private string P_GP_ID = "";
        private string modemNo = "";
        private string Z_CHK = "";

        private GpBhaPosts gpBhaPost = new GpBhaPosts();
        private Dictionary<int, GpCompPosts> gpBhaCompPost = new Dictionary<int, GpCompPosts>();
        private int gpBhaCount;


        public GpBhaPosts GpBhaPosts => gpBhaPost;
        public Dictionary<int, GpCompPosts> GpBhaCompPost => gpBhaCompPost;
        public int GpBhaCount => gpBhaCount;


        public GpBhaParameters(HtmlAgilityPack.HtmlDocument _hDoc)
        {
            hDoc = _hDoc;
            Init();

            
            //foreach (var p in gpBhaPost.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    if (p.PropertyType == typeof(Tuple<string, string>))
            //    {
            //        Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(gpBhaPost, null);
            //        if (p.Name == "P_L_HOLESEC")
            //        {
            //            MessageBox.Show("In Gp Parameters after Init: " + p.Name + " => " + temp.Item1 + " => " + temp.Item2);
            //        }


            //    }

            //}

            //MessageBox.Show("Break 4: => " + gpBhaPost.P_L_HOLESEC.Item1 + " => " + gpBhaPost.P_L_HOLESEC.Item2);
        }

        public GpBhaParameters(mshtml.HTMLDocument htmlDocument)
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
            //MessageBox.Show("Break 0");

            if (GetNumberOfTables() > 1)
            {
                gpBhaCompPost = GetGpBhaTable();
                gpBhaCount = gpBhaCompPost.Count;

            }
            else
            {
                gpBhaCount = 0;

            }

            //MessageBox.Show("Break 1");
            //MessageBox.Show("Break 1a: => " + HDocUtility.GetSelectedElementById(gpBhaPost.P_L_HOLESEC.Item1, hDoc));

            ////gpBhaPost.Refresh(hDoc);


            //MessageBox.Show("Break 2");
            //MessageBox.Show("Break 2a: => " + gpBhaPost.P_L_HOLESEC.Item1 + " => " + gpBhaPost.P_L_HOLESEC.Item2);

            gpBhaPost.P_10 = Tuple.Create(gpBhaPost.P_10.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_10.Item1, hDoc));
            gpBhaPost.P_GP_ID = Tuple.Create(gpBhaPost.P_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_GP_ID.Item1, hDoc));
            gpBhaPost.O_GP_ID = Tuple.Create(gpBhaPost.O_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_ID.Item1, hDoc));
            gpBhaPost.H_GP_COMPL_WARN = Tuple.Create(gpBhaPost.H_GP_COMPL_WARN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_GP_COMPL_WARN.Item1, hDoc));
            gpBhaPost.H_DIV0 = Tuple.Create(gpBhaPost.H_DIV0.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV0.Item1, hDoc));
            gpBhaPost.H_DIV1 = Tuple.Create(gpBhaPost.H_DIV1.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV1.Item1, hDoc));
            gpBhaPost.P_PILOT_NUM = Tuple.Create(gpBhaPost.P_PILOT_NUM.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_PILOT_NUM.Item1, hDoc));
            gpBhaPost.H_DIV30 = Tuple.Create(gpBhaPost.H_DIV30.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV30.Item1, hDoc));
            gpBhaPost.P_GP_DESC = Tuple.Create(gpBhaPost.P_GP_DESC.Item1, HDocUtility.GetInputById(gpBhaPost.P_GP_DESC.Item1, hDoc));
            gpBhaPost.H_LBL_APPDET = Tuple.Create(gpBhaPost.H_LBL_APPDET.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_LBL_APPDET.Item1, hDoc));
            gpBhaPost.H_DIV5 = Tuple.Create(gpBhaPost.H_DIV5.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV5.Item1, hDoc));
            gpBhaPost.H_DIV2 = Tuple.Create(gpBhaPost.H_DIV2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV2.Item1, hDoc));
            gpBhaPost.P_L_HOLESEC = Tuple.Create(gpBhaPost.P_L_HOLESEC.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_HOLESEC.Item1, hDoc));
            gpBhaPost.H_DIV25 = Tuple.Create(gpBhaPost.H_DIV25.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV25.Item1, hDoc));
            gpBhaPost.P_FLOW_RATE = Tuple.Create(gpBhaPost.P_FLOW_RATE.Item1, HDocUtility.GetInputById(gpBhaPost.P_FLOW_RATE.Item1, hDoc));
            gpBhaPost.P_FLOW_RATE_TO = Tuple.Create(gpBhaPost.P_FLOW_RATE_TO.Item1, HDocUtility.GetInputById(gpBhaPost.P_FLOW_RATE_TO.Item1, hDoc));
            gpBhaPost.P_FLOW_RATE_UNIT = Tuple.Create(gpBhaPost.P_FLOW_RATE_UNIT.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_FLOW_RATE_UNIT.Item1, hDoc));
            gpBhaPost.P_MAX_HYD_PRES = Tuple.Create(gpBhaPost.P_MAX_HYD_PRES.Item1, HDocUtility.GetInputById(gpBhaPost.P_MAX_HYD_PRES.Item1, hDoc));
            gpBhaPost.P_MAX_HYD_PRES_UNIT = Tuple.Create(gpBhaPost.P_MAX_HYD_PRES_UNIT.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_MAX_HYD_PRES_UNIT.Item1, hDoc));
            gpBhaPost.P_PLAN_TEMP_START = Tuple.Create(gpBhaPost.P_PLAN_TEMP_START.Item1, HDocUtility.GetInputById(gpBhaPost.P_PLAN_TEMP_START.Item1, hDoc));
            gpBhaPost.P_TEMP_UNIT = Tuple.Create(gpBhaPost.P_TEMP_UNIT.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_TEMP_UNIT.Item1, hDoc));
            gpBhaPost.H_DIV3 = Tuple.Create(gpBhaPost.H_DIV3.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV3.Item1, hDoc));
            gpBhaPost.P_MAX_TEMP = Tuple.Create(gpBhaPost.P_MAX_TEMP.Item1, HDocUtility.GetInputById(gpBhaPost.P_MAX_TEMP.Item1, hDoc));
            gpBhaPost.H_DIV4 = Tuple.Create(gpBhaPost.H_DIV4.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV4.Item1, hDoc));
            gpBhaPost.P_L_OILTYPE = Tuple.Create(gpBhaPost.P_L_OILTYPE.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_OILTYPE.Item1, hDoc));
            gpBhaPost.H_LBL_CONFDET = Tuple.Create(gpBhaPost.H_LBL_CONFDET.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_LBL_CONFDET.Item1, hDoc));
            gpBhaPost.H_DIV6 = Tuple.Create(gpBhaPost.H_DIV6.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV6.Item1, hDoc));
            gpBhaPost.H_DIV12 = Tuple.Create(gpBhaPost.H_DIV12.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV12.Item1, hDoc));
            gpBhaPost.P_L_GPSIZE = Tuple.Create(gpBhaPost.P_L_GPSIZE.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_GPSIZE.Item1, hDoc));
            gpBhaPost.H_DIV11 = Tuple.Create(gpBhaPost.H_DIV11.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV11.Item1, hDoc));
            gpBhaPost.P_L_MRSC_SUB_CONF_DESCR2 = Tuple.Create(gpBhaPost.P_L_MRSC_SUB_CONF_DESCR2.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_MRSC_SUB_CONF_DESCR2.Item1, hDoc));
            gpBhaPost.H_DIV13 = Tuple.Create(gpBhaPost.H_DIV13.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV13.Item1, hDoc));
            gpBhaPost.P_L_ABITYPE = Tuple.Create(gpBhaPost.P_L_ABITYPE.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_ABITYPE.Item1, hDoc));
            gpBhaPost.H_DIV19 = Tuple.Create(gpBhaPost.H_DIV19.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV19.Item1, hDoc));
            gpBhaPost.P_FLEXJOINT_REQ = Tuple.Create(gpBhaPost.P_FLEXJOINT_REQ.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_FLEXJOINT_REQ.Item1, hDoc));
            gpBhaPost.H_DIV26 = Tuple.Create(gpBhaPost.H_DIV26.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV26.Item1, hDoc));
            gpBhaPost.P_FLEX_TORQUED = Tuple.Create(gpBhaPost.P_FLEX_TORQUED.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_FLEX_TORQUED.Item1, hDoc));
            gpBhaPost.H_DIV14 = Tuple.Create(gpBhaPost.H_DIV14.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV14.Item1, hDoc));
            gpBhaPost.P_GP_CLAMP_REQ = Tuple.Create(gpBhaPost.P_GP_CLAMP_REQ.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_GP_CLAMP_REQ.Item1, hDoc));
            gpBhaPost.H_DIV21 = Tuple.Create(gpBhaPost.H_DIV21.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV21.Item1, hDoc));
            gpBhaPost.P_DM_TYPE = Tuple.Create(gpBhaPost.P_DM_TYPE.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_DM_TYPE.Item1, hDoc));
            gpBhaPost.H_DIV15 = Tuple.Create(gpBhaPost.H_DIV15.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV15.Item1, hDoc));
            gpBhaPost.P_L_SOFTWARE_DM = Tuple.Create(gpBhaPost.P_L_SOFTWARE_DM.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_SOFTWARE_DM.Item1, hDoc));
            gpBhaPost.H_DIV17 = Tuple.Create(gpBhaPost.H_DIV17.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV17.Item1, hDoc));
            gpBhaPost.P_L_SOFTWARE_GP = Tuple.Create(gpBhaPost.P_L_SOFTWARE_GP.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_SOFTWARE_GP.Item1, hDoc));
            gpBhaPost.H_DIV24 = Tuple.Create(gpBhaPost.H_DIV24.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV24.Item1, hDoc));
            gpBhaPost.P_L_THREAD_UP = Tuple.Create(gpBhaPost.P_L_THREAD_UP.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_THREAD_UP.Item1, hDoc));
            gpBhaPost.H_DIV23 = Tuple.Create(gpBhaPost.H_DIV23.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV23.Item1, hDoc));
            gpBhaPost.P_L_THREAD_LOW = Tuple.Create(gpBhaPost.P_L_THREAD_LOW.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_THREAD_LOW.Item1, hDoc));
            gpBhaPost.H_DIV18 = Tuple.Create(gpBhaPost.H_DIV18.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV18.Item1, hDoc));
            gpBhaPost.P_BIT_TYPE = Tuple.Create(gpBhaPost.P_BIT_TYPE.Item1, HDocUtility.GetInputById(gpBhaPost.P_BIT_TYPE.Item1, hDoc));
            gpBhaPost.H_DIV16 = Tuple.Create(gpBhaPost.H_DIV16.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV16.Item1, hDoc));
            gpBhaPost.P_BIT_TORQUED = Tuple.Create(gpBhaPost.P_BIT_TORQUED.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_BIT_TORQUED.Item1, hDoc));
            gpBhaPost.H_DIV99 = Tuple.Create(gpBhaPost.H_DIV99.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV99.Item1, hDoc));
            gpBhaPost.P_BIT_MAKEUP_TORQUE = Tuple.Create(gpBhaPost.P_BIT_MAKEUP_TORQUE.Item1, HDocUtility.GetInputById(gpBhaPost.P_BIT_MAKEUP_TORQUE.Item1, hDoc));
            gpBhaPost.H_DIV20 = Tuple.Create(gpBhaPost.H_DIV20.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV20.Item1, hDoc));
            gpBhaPost.P_BIT_SN = Tuple.Create(gpBhaPost.P_BIT_SN.Item1, HDocUtility.GetInputById(gpBhaPost.P_BIT_SN.Item1, hDoc));
            gpBhaPost.H_DIV22 = Tuple.Create(gpBhaPost.H_DIV22.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV22.Item1, hDoc));
            gpBhaPost.P_LITHIUM_USED = Tuple.Create(gpBhaPost.P_LITHIUM_USED.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_LITHIUM_USED.Item1, hDoc));
            gpBhaPost.P_GP_COMMENT = Tuple.Create(gpBhaPost.P_GP_COMMENT.Item1, HDocUtility.GetTextAreaById(gpBhaPost.P_GP_COMMENT.Item1, hDoc));
            gpBhaPost.P_L_LOWHOUSEDESC = Tuple.Create(gpBhaPost.P_L_LOWHOUSEDESC.Item1, HDocUtility.GetSelectedElementById(gpBhaPost.P_L_LOWHOUSEDESC.Item1, hDoc));
            gpBhaPost.H_DEL_BHA = Tuple.Create(gpBhaPost.H_DEL_BHA.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DEL_BHA.Item1, hDoc));
            gpBhaPost.P_S2 = Tuple.Create(gpBhaPost.P_S2.Item1, HDocUtility.GetInputById(gpBhaPost.P_S2.Item1, hDoc));
            gpBhaPost.H_L_PRECON_STATUS = Tuple.Create(gpBhaPost.H_L_PRECON_STATUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_L_PRECON_STATUS.Item1, hDoc));

            gpBhaPost.O_PILOT_NUM = Tuple.Create(gpBhaPost.O_PILOT_NUM.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_PILOT_NUM.Item1, hDoc));
            //gpBhaPost.O_GP_DESC = Tuple.Create(gpBhaPost.O_GP_DESC.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_DESC.Item1, hDoc));
            //gpBhaPost.O_FLOW_RATE = Tuple.Create(gpBhaPost.O_FLOW_RATE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLOW_RATE.Item1, hDoc));
            //gpBhaPost.O_FLOW_RATE_TO = Tuple.Create(gpBhaPost.O_FLOW_RATE_TO.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLOW_RATE_TO.Item1, hDoc));
            //gpBhaPost.O_FLOW_RATE_UNIT = Tuple.Create(gpBhaPost.O_FLOW_RATE_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLOW_RATE_UNIT.Item1, hDoc));
            //gpBhaPost.O_MAX_HYD_PRES = Tuple.Create(gpBhaPost.O_MAX_HYD_PRES.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_MAX_HYD_PRES.Item1, hDoc));
            //gpBhaPost.O_MAX_HYD_PRES_UNIT = Tuple.Create(gpBhaPost.O_MAX_HYD_PRES_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_MAX_HYD_PRES_UNIT.Item1, hDoc));
            //gpBhaPost.O_PLAN_TEMP_START = Tuple.Create(gpBhaPost.O_PLAN_TEMP_START.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_PLAN_TEMP_START.Item1, hDoc));
            //gpBhaPost.O_TEMP_UNIT = Tuple.Create(gpBhaPost.O_TEMP_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_TEMP_UNIT.Item1, hDoc));
            //gpBhaPost.O_MAX_TEMP = Tuple.Create(gpBhaPost.O_MAX_TEMP.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_MAX_TEMP.Item1, hDoc));
            //gpBhaPost.O_ROLLER_STYLE = Tuple.Create(gpBhaPost.O_ROLLER_STYLE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_ROLLER_STYLE.Item1, hDoc));
            //gpBhaPost.O_FLEXJOINT_REQ = Tuple.Create(gpBhaPost.O_FLEXJOINT_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLEXJOINT_REQ.Item1, hDoc));
            //gpBhaPost.O_FLEX_TORQUED = Tuple.Create(gpBhaPost.O_FLEX_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLEX_TORQUED.Item1, hDoc));
            //gpBhaPost.O_GP_CLAMP_REQ = Tuple.Create(gpBhaPost.O_GP_CLAMP_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_CLAMP_REQ.Item1, hDoc));
            //gpBhaPost.O_DM_TYPE = Tuple.Create(gpBhaPost.O_DM_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_DM_TYPE.Item1, hDoc));
            //gpBhaPost.O_BIT_TYPE = Tuple.Create(gpBhaPost.O_BIT_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_TYPE.Item1, hDoc));
            //gpBhaPost.O_BIT_TORQUED = Tuple.Create(gpBhaPost.O_BIT_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_TORQUED.Item1, hDoc));
            //gpBhaPost.O_BIT_MAKEUP_TORQUE = Tuple.Create(gpBhaPost.O_BIT_MAKEUP_TORQUE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_MAKEUP_TORQUE.Item1, hDoc));
            //gpBhaPost.O_BIT_SN = Tuple.Create(gpBhaPost.O_BIT_SN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_SN.Item1, hDoc));
            gpBhaPost.O_LITHIUM_USED = Tuple.Create(gpBhaPost.O_LITHIUM_USED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_LITHIUM_USED.Item1, hDoc));
            gpBhaPost.O_ABITYPE_ID = Tuple.Create(gpBhaPost.O_ABITYPE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_ABITYPE_ID.Item1, hDoc));
            gpBhaPost.O_CONN_LOWER_ID = Tuple.Create(gpBhaPost.O_CONN_LOWER_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_CONN_LOWER_ID.Item1, hDoc));
            gpBhaPost.O_CONN_UPHOLE_ID = Tuple.Create(gpBhaPost.O_CONN_UPHOLE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_CONN_UPHOLE_ID.Item1, hDoc));
            gpBhaPost.O_GPSIZE_ID = Tuple.Create(gpBhaPost.O_GPSIZE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GPSIZE_ID.Item1, hDoc));
            gpBhaPost.O_HOLESEC_ID = Tuple.Create(gpBhaPost.O_HOLESEC_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_HOLESEC_ID.Item1, hDoc));
            gpBhaPost.O_OILTYPE_ID = Tuple.Create(gpBhaPost.O_OILTYPE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_OILTYPE_ID.Item1, hDoc));
            gpBhaPost.O_ORDER_ID = Tuple.Create(gpBhaPost.O_ORDER_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_ORDER_ID.Item1, hDoc));
            gpBhaPost.O_SW_DM_ID = Tuple.Create(gpBhaPost.O_SW_DM_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_SW_DM_ID.Item1, hDoc));
            gpBhaPost.O_SW_GP_ID = Tuple.Create(gpBhaPost.O_SW_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_SW_GP_ID.Item1, hDoc));
            //gpBhaPost.O_GP_COMMENT = Tuple.Create(gpBhaPost.O_GP_COMMENT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_COMMENT.Item1, hDoc));
            gpBhaPost.O_LWR_SLICK_HOUS = Tuple.Create(gpBhaPost.O_LWR_SLICK_HOUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_LWR_SLICK_HOUS.Item1, hDoc));
            gpBhaPost.O_STATUS = Tuple.Create(gpBhaPost.O_STATUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_STATUS.Item1, hDoc));
            //gpBhaPost.O_SUB_CONF_ID = Tuple.Create(gpBhaPost.O_SUB_CONF_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_SUB_CONF_ID.Item1, hDoc));
            //gpBhaPost.O_LOWHOUSEID = Tuple.Create(gpBhaPost.O_LOWHOUSEID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_LOWHOUSEID.Item1, hDoc));
            //gpBhaPost.O_S2 = Tuple.Create(gpBhaPost.O_S2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_S2.Item1, hDoc));
            gpBhaPost.O_PRECON_ID = Tuple.Create(gpBhaPost.O_PRECON_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_PRECON_ID.Item1, hDoc));
            //gpBhaPost.P_GP_ID_o = Tuple.Create(gpBhaPost.P_GP_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_GP_ID_o.Item1, hDoc));
            //gpBhaPost.P_PILOT_NUM_o = Tuple.Create(gpBhaPost.P_PILOT_NUM_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_PILOT_NUM_o.Item1, hDoc));
            //gpBhaPost.P_GP_DESC_o = Tuple.Create(gpBhaPost.P_GP_DESC_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_GP_DESC_o.Item1, hDoc));
            //gpBhaPost.P_L_HOLESEC_o = Tuple.Create(gpBhaPost.P_L_HOLESEC_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_HOLESEC_o.Item1, hDoc));
            //gpBhaPost.P_FLOW_RATE_o = Tuple.Create(gpBhaPost.P_FLOW_RATE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_FLOW_RATE_o.Item1, hDoc));
            //gpBhaPost.P_FLOW_RATE_TO_o = Tuple.Create(gpBhaPost.P_FLOW_RATE_TO_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_FLOW_RATE_TO_o.Item1, hDoc));
            //gpBhaPost.P_FLOW_RATE_UNIT_o = Tuple.Create(gpBhaPost.P_FLOW_RATE_UNIT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_FLOW_RATE_UNIT_o.Item1, hDoc));
            //gpBhaPost.P_MAX_HYD_PRES_o = Tuple.Create(gpBhaPost.P_MAX_HYD_PRES_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_MAX_HYD_PRES_o.Item1, hDoc));
            //gpBhaPost.P_MAX_HYD_PRES_UNIT_o = Tuple.Create(gpBhaPost.P_MAX_HYD_PRES_UNIT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_MAX_HYD_PRES_UNIT_o.Item1, hDoc));
            //gpBhaPost.P_PLAN_TEMP_START_o = Tuple.Create(gpBhaPost.P_PLAN_TEMP_START_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_PLAN_TEMP_START_o.Item1, hDoc));
            //gpBhaPost.P_TEMP_UNIT_o = Tuple.Create(gpBhaPost.P_TEMP_UNIT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_TEMP_UNIT_o.Item1, hDoc));
            //gpBhaPost.P_MAX_TEMP_o = Tuple.Create(gpBhaPost.P_MAX_TEMP_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_MAX_TEMP_o.Item1, hDoc));
            //gpBhaPost.P_L_OILTYPE_o = Tuple.Create(gpBhaPost.P_L_OILTYPE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_OILTYPE_o.Item1, hDoc));
            //gpBhaPost.P_L_GPSIZE_o = Tuple.Create(gpBhaPost.P_L_GPSIZE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_GPSIZE_o.Item1, hDoc));
            //gpBhaPost.P_L_MRSC_SUB_CONF_DESCR2_o = Tuple.Create(gpBhaPost.P_L_MRSC_SUB_CONF_DESCR2_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_MRSC_SUB_CONF_DESCR2_o.Item1, hDoc));
            //gpBhaPost.P_L_ABITYPE_o = Tuple.Create(gpBhaPost.P_L_ABITYPE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_ABITYPE_o.Item1, hDoc));
            //gpBhaPost.P_FLEXJOINT_REQ_o = Tuple.Create(gpBhaPost.P_FLEXJOINT_REQ_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_FLEXJOINT_REQ_o.Item1, hDoc));
            //gpBhaPost.P_FLEX_TORQUED_o = Tuple.Create(gpBhaPost.P_FLEX_TORQUED_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_FLEX_TORQUED_o.Item1, hDoc));
            //gpBhaPost.P_GP_CLAMP_REQ_o = Tuple.Create(gpBhaPost.P_GP_CLAMP_REQ_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_GP_CLAMP_REQ_o.Item1, hDoc));
            //gpBhaPost.P_DM_TYPE_o = Tuple.Create(gpBhaPost.P_DM_TYPE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_DM_TYPE_o.Item1, hDoc));
            //gpBhaPost.P_L_SOFTWARE_DM_o = Tuple.Create(gpBhaPost.P_L_SOFTWARE_DM_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_SOFTWARE_DM_o.Item1, hDoc));
            //gpBhaPost.P_L_SOFTWARE_GP_o = Tuple.Create(gpBhaPost.P_L_SOFTWARE_GP_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_SOFTWARE_GP_o.Item1, hDoc));
            //gpBhaPost.P_L_THREAD_UP_o = Tuple.Create(gpBhaPost.P_L_THREAD_UP_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_THREAD_UP_o.Item1, hDoc));
            //gpBhaPost.P_L_THREAD_LOW_o = Tuple.Create(gpBhaPost.P_L_THREAD_LOW_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_THREAD_LOW_o.Item1, hDoc));
            //gpBhaPost.P_BIT_TYPE_o = Tuple.Create(gpBhaPost.P_BIT_TYPE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_BIT_TYPE_o.Item1, hDoc));
            //gpBhaPost.P_BIT_TORQUED_o = Tuple.Create(gpBhaPost.P_BIT_TORQUED_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_BIT_TORQUED_o.Item1, hDoc));
            //gpBhaPost.P_BIT_MAKEUP_TORQUE_o = Tuple.Create(gpBhaPost.P_BIT_MAKEUP_TORQUE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_BIT_MAKEUP_TORQUE_o.Item1, hDoc));
            //gpBhaPost.P_BIT_SN_o = Tuple.Create(gpBhaPost.P_BIT_SN_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_BIT_SN_o.Item1, hDoc));
            //gpBhaPost.P_LITHIUM_USED_o = Tuple.Create(gpBhaPost.P_LITHIUM_USED_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_LITHIUM_USED_o.Item1, hDoc));
            //gpBhaPost.P_GP_COMMENT_o = Tuple.Create(gpBhaPost.P_GP_COMMENT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_GP_COMMENT_o.Item1, hDoc));
            //gpBhaPost.P_L_LOWHOUSEDESC_o = Tuple.Create(gpBhaPost.P_L_LOWHOUSEDESC_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_L_LOWHOUSEDESC_o.Item1, hDoc));
            //gpBhaPost.P_S2_o = Tuple.Create(gpBhaPost.P_S2_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.P_S2_o.Item1, hDoc));
            //gpBhaPost.O_PILOT_NUM_o = Tuple.Create(gpBhaPost.O_PILOT_NUM_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_PILOT_NUM_o.Item1, hDoc));
            //gpBhaPost.O_GP_DESC_o = Tuple.Create(gpBhaPost.O_GP_DESC_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_DESC_o.Item1, hDoc));
            //gpBhaPost.O_FLOW_RATE_o = Tuple.Create(gpBhaPost.O_FLOW_RATE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLOW_RATE_o.Item1, hDoc));
            //gpBhaPost.O_FLOW_RATE_TO_o = Tuple.Create(gpBhaPost.O_FLOW_RATE_TO_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLOW_RATE_TO_o.Item1, hDoc));
            //gpBhaPost.O_FLOW_RATE_UNIT_o = Tuple.Create(gpBhaPost.O_FLOW_RATE_UNIT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLOW_RATE_UNIT_o.Item1, hDoc));
            //gpBhaPost.O_MAX_HYD_PRES_o = Tuple.Create(gpBhaPost.O_MAX_HYD_PRES_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_MAX_HYD_PRES_o.Item1, hDoc));
            //gpBhaPost.O_MAX_HYD_PRES_UNIT_o = Tuple.Create(gpBhaPost.O_MAX_HYD_PRES_UNIT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_MAX_HYD_PRES_UNIT_o.Item1, hDoc));
            //gpBhaPost.O_PLAN_TEMP_START_o = Tuple.Create(gpBhaPost.O_PLAN_TEMP_START_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_PLAN_TEMP_START_o.Item1, hDoc));
            //gpBhaPost.O_TEMP_UNIT_o = Tuple.Create(gpBhaPost.O_TEMP_UNIT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_TEMP_UNIT_o.Item1, hDoc));
            //gpBhaPost.O_MAX_TEMP_o = Tuple.Create(gpBhaPost.O_MAX_TEMP_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_MAX_TEMP_o.Item1, hDoc));
            //gpBhaPost.O_ROLLER_STYLE_o = Tuple.Create(gpBhaPost.O_ROLLER_STYLE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_ROLLER_STYLE_o.Item1, hDoc));
            //gpBhaPost.O_FLEXJOINT_REQ_o = Tuple.Create(gpBhaPost.O_FLEXJOINT_REQ_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLEXJOINT_REQ_o.Item1, hDoc));
            //gpBhaPost.O_FLEX_TORQUED_o = Tuple.Create(gpBhaPost.O_FLEX_TORQUED_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_FLEX_TORQUED_o.Item1, hDoc));
            //gpBhaPost.O_GP_CLAMP_REQ_o = Tuple.Create(gpBhaPost.O_GP_CLAMP_REQ_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_CLAMP_REQ_o.Item1, hDoc));
            //gpBhaPost.O_DM_TYPE_o = Tuple.Create(gpBhaPost.O_DM_TYPE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_DM_TYPE_o.Item1, hDoc));
            //gpBhaPost.O_BIT_TYPE_o = Tuple.Create(gpBhaPost.O_BIT_TYPE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_TYPE_o.Item1, hDoc));
            //gpBhaPost.O_BIT_TORQUED_o = Tuple.Create(gpBhaPost.O_BIT_TORQUED_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_TORQUED_o.Item1, hDoc));
            //gpBhaPost.O_BIT_MAKEUP_TORQUE_o = Tuple.Create(gpBhaPost.O_BIT_MAKEUP_TORQUE_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_MAKEUP_TORQUE_o.Item1, hDoc));
            //gpBhaPost.O_BIT_SN_o = Tuple.Create(gpBhaPost.O_BIT_SN_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_BIT_SN_o.Item1, hDoc));
            //gpBhaPost.O_LITHIUM_USED_o = Tuple.Create(gpBhaPost.O_LITHIUM_USED_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_LITHIUM_USED_o.Item1, hDoc));
            //gpBhaPost.O_ABITYPE_ID_o = Tuple.Create(gpBhaPost.O_ABITYPE_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_ABITYPE_ID_o.Item1, hDoc));
            //gpBhaPost.O_CONN_LOWER_ID_o = Tuple.Create(gpBhaPost.O_CONN_LOWER_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_CONN_LOWER_ID_o.Item1, hDoc));
            //gpBhaPost.O_CONN_UPHOLE_ID_o = Tuple.Create(gpBhaPost.O_CONN_UPHOLE_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_CONN_UPHOLE_ID_o.Item1, hDoc));
            //gpBhaPost.O_GPSIZE_ID_o = Tuple.Create(gpBhaPost.O_GPSIZE_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GPSIZE_ID_o.Item1, hDoc));
            //gpBhaPost.O_HOLESEC_ID_o = Tuple.Create(gpBhaPost.O_HOLESEC_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_HOLESEC_ID_o.Item1, hDoc));
            //gpBhaPost.O_OILTYPE_ID_o = Tuple.Create(gpBhaPost.O_OILTYPE_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_OILTYPE_ID_o.Item1, hDoc));
            //gpBhaPost.O_ORDER_ID_o = Tuple.Create(gpBhaPost.O_ORDER_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_ORDER_ID_o.Item1, hDoc));
            //gpBhaPost.O_SW_DM_ID_o = Tuple.Create(gpBhaPost.O_SW_DM_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_SW_DM_ID_o.Item1, hDoc));
            //gpBhaPost.O_SW_GP_ID_o = Tuple.Create(gpBhaPost.O_SW_GP_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_SW_GP_ID_o.Item1, hDoc));
            //gpBhaPost.O_GP_COMMENT_o = Tuple.Create(gpBhaPost.O_GP_COMMENT_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_GP_COMMENT_o.Item1, hDoc));
            //gpBhaPost.O_LWR_SLICK_HOUS_o = Tuple.Create(gpBhaPost.O_LWR_SLICK_HOUS_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_LWR_SLICK_HOUS_o.Item1, hDoc));
            //gpBhaPost.O_STATUS_o = Tuple.Create(gpBhaPost.O_STATUS_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_STATUS_o.Item1, hDoc));
            //gpBhaPost.O_SUB_CONF_ID_o = Tuple.Create(gpBhaPost.O_SUB_CONF_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_SUB_CONF_ID_o.Item1, hDoc));
            //gpBhaPost.O_LOWHOUSEID_o = Tuple.Create(gpBhaPost.O_LOWHOUSEID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_LOWHOUSEID_o.Item1, hDoc));
            //gpBhaPost.O_S2_o = Tuple.Create(gpBhaPost.O_S2_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_S2_o.Item1, hDoc));
            //gpBhaPost.O_PRECON_ID_o = Tuple.Create(gpBhaPost.O_PRECON_ID_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.O_PRECON_ID_o.Item1, hDoc));
            //gpBhaPost.H_GP_COMPL_WARN_o = Tuple.Create(gpBhaPost.H_GP_COMPL_WARN_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_GP_COMPL_WARN_o.Item1, hDoc));
            //gpBhaPost.H_DIV0_o = Tuple.Create(gpBhaPost.H_DIV0_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV0_o.Item1, hDoc));
            //gpBhaPost.H_DIV1_o = Tuple.Create(gpBhaPost.H_DIV1_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV1_o.Item1, hDoc));
            //gpBhaPost.H_DIV30_o = Tuple.Create(gpBhaPost.H_DIV30_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV30_o.Item1, hDoc));
            //gpBhaPost.H_LBL_APPDET_o = Tuple.Create(gpBhaPost.H_LBL_APPDET_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_LBL_APPDET_o.Item1, hDoc));
            //gpBhaPost.H_DIV5_o = Tuple.Create(gpBhaPost.H_DIV5_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV5_o.Item1, hDoc));
            //gpBhaPost.H_DIV2_o = Tuple.Create(gpBhaPost.H_DIV2_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV2_o.Item1, hDoc));
            //gpBhaPost.H_DIV25_o = Tuple.Create(gpBhaPost.H_DIV25_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV25_o.Item1, hDoc));
            //gpBhaPost.H_DIV3_o = Tuple.Create(gpBhaPost.H_DIV3_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV3_o.Item1, hDoc));
            //gpBhaPost.H_DIV4_o = Tuple.Create(gpBhaPost.H_DIV4_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV4_o.Item1, hDoc));
            //gpBhaPost.H_LBL_CONFDET_o = Tuple.Create(gpBhaPost.H_LBL_CONFDET_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_LBL_CONFDET_o.Item1, hDoc));
            //gpBhaPost.H_DIV6_o = Tuple.Create(gpBhaPost.H_DIV6_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV6_o.Item1, hDoc));
            //gpBhaPost.H_DIV12_o = Tuple.Create(gpBhaPost.H_DIV12_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV12_o.Item1, hDoc));
            //gpBhaPost.H_DIV11_o = Tuple.Create(gpBhaPost.H_DIV11_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV11_o.Item1, hDoc));
            //gpBhaPost.H_DIV13_o = Tuple.Create(gpBhaPost.H_DIV13_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV13_o.Item1, hDoc));
            //gpBhaPost.H_DIV19_o = Tuple.Create(gpBhaPost.H_DIV19_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV19_o.Item1, hDoc));
            //gpBhaPost.H_DIV26_o = Tuple.Create(gpBhaPost.H_DIV26_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV26_o.Item1, hDoc));
            //gpBhaPost.H_DIV14_o = Tuple.Create(gpBhaPost.H_DIV14_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV14_o.Item1, hDoc));
            //gpBhaPost.H_DIV21_o = Tuple.Create(gpBhaPost.H_DIV21_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV21_o.Item1, hDoc));
            //gpBhaPost.H_DIV15_o = Tuple.Create(gpBhaPost.H_DIV15_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV15_o.Item1, hDoc));
            //gpBhaPost.H_DIV17_o = Tuple.Create(gpBhaPost.H_DIV17_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV17_o.Item1, hDoc));
            //gpBhaPost.H_DIV24_o = Tuple.Create(gpBhaPost.H_DIV24_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV24_o.Item1, hDoc));
            //gpBhaPost.H_DIV23_o = Tuple.Create(gpBhaPost.H_DIV23_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV23_o.Item1, hDoc));
            //gpBhaPost.H_DIV18_o = Tuple.Create(gpBhaPost.H_DIV18_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV18_o.Item1, hDoc));
            //gpBhaPost.H_DIV16_o = Tuple.Create(gpBhaPost.H_DIV16_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV16_o.Item1, hDoc));
            //gpBhaPost.H_DIV99_o = Tuple.Create(gpBhaPost.H_DIV99_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV99_o.Item1, hDoc));
            //gpBhaPost.H_DIV20_o = Tuple.Create(gpBhaPost.H_DIV20_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV20_o.Item1, hDoc));
            //gpBhaPost.H_DIV22_o = Tuple.Create(gpBhaPost.H_DIV22_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DIV22_o.Item1, hDoc));
            //gpBhaPost.H_DEL_BHA_o = Tuple.Create(gpBhaPost.H_DEL_BHA_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_DEL_BHA_o.Item1, hDoc));
            //gpBhaPost.H_L_PRECON_STATUS_o = Tuple.Create(gpBhaPost.H_L_PRECON_STATUS_o.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.H_L_PRECON_STATUS_o.Item1, hDoc));


            gpBhaPost.Z_CHK = Tuple.Create(gpBhaPost.Z_CHK.Item1, HDocUtility.GetOneFromListValueOfInputsByName(gpBhaPost.Z_CHK.Item1, hDoc));





        }

        private int GetNumberOfTables()
        {

            var query = from table in hDoc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                        select new { table };

            return query.Count();

        }

        private Dictionary<int, GpCompPosts> GetGpBhaTable()
        {
            Dictionary<int, GpCompPosts> mcpDic = new Dictionary<int, GpCompPosts>();
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
                GpCompPosts mcp = new GpCompPosts();
                mcp.P_SEQ_NO = tempArray[i, 0];
                mcp.P_L_MWDTORQUE_TORQUE = tempArray[i, 1];
                mcp.P_L_THREADTOP_THREADSIZE = tempArray[i, 2];
                mcp.P_L_THREADBTM_THREADSIZE = tempArray[i, 3];
                mcp.P_DESCRIPTION = tempArray[i, 4]; ;

                mcpDic.Add(i, mcp);

            }

            return mcpDic;

        }

    }
}
