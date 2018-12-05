using System.Collections.Generic;

namespace ModemToolbarIE
{
    public class ModemPostObjects
    {

        public string ModemNo { get; set; }

        public mshtml.HTMLDocument Doc { get; set; }

        public MwdBhaPosts MwdBhaPost = new MwdBhaPosts();

        public Dictionary<int, MwdCompPosts> MwdCompPostDict = new Dictionary<int, MwdCompPosts>();

        public Dictionary<int, MwdSoftPosts> MwdSoftPostDict = new Dictionary<int, MwdSoftPosts>();

        public ModemPostObjects(string modemNo, mshtml.HTMLDocument doc)
        {
            ModemNo = modemNo;
            Doc = doc;
        }

    }

    public class MwdBhaPosts
    {
        public string P_BHA_NUM { get; set; }
        public string P_L_ORIFFICESIZE { get; set; }
        public string P_POPPET_STANDOFF { get; set; }
        public string P_L_IMPELLERSIZE { get; set; }
        public string P_EXP_MAX_TEMP { get; set; }
        public string P_L_STATORSIZE { get; set; }
        public string P_BHA_DESC { get; set; }
        public string P_PULSER_OILTYPE { get; set; }
        public string P_MWDDWD_ADD_INFO { get; set; }
        public string P_HC_TOOL { get; set; }
        public string P_RASOURCE_ID { get; set; }

    }

    public class MwdCompPosts
    {
        public string P_SEQ_NO { get; set; }
        public string P_L_TORQUE { get; set; }
        public string P_L_THREAD_TOP { get; set; }
        public string P_L_THREAD_BTM { get; set; }
        public string P_DESCRIPTION { get; set; }
        public string P_COMMENTS { get; set; }

    }

    public class MwdSoftPosts
    {
        public string P_L_MSR_SENSOR { get; set; }
        public string P_OPS_VERSION { get; set; }
        public string P_WS_VERSION { get; set; }

    }
}
