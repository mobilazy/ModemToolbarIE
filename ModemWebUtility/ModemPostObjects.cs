using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class ModemMwdPostObjects
    {

        //public string ModemNo { get; set; }

        //public mshtml.HTMLDocument Doc { get; set; }

        public MwdBhaPosts MwdBhaPost = new MwdBhaPosts();

        public Dictionary<int, MwdCompPosts> MwdCompPostDict = new Dictionary<int, MwdCompPosts>();

        public Dictionary<int, MwdSoftPosts> MwdSoftPostDict = new Dictionary<int, MwdSoftPosts>();

        public ModemMwdPostObjects()
        {
            //ModemNo = modemNo;
            //Doc = doc;
        }

    }

    public class ModemDdPostObjects
    {
        public DdBhaPosts DdBhaPast = new DdBhaPosts();

        public Dictionary<int, DdCompPosts> DdCompPostDict = new Dictionary<int, DdCompPosts>();

        public ModemDdPostObjects()
        {

        }

    }

    public class ModemGpPostObjects
    {
        public GpBhaPosts GpBhaPost = new GpBhaPosts();

        public Dictionary<int, GpCompPosts> GpCompPostDict = new Dictionary<int, GpCompPosts>();

        public ModemGpPostObjects()
        {

        }
    }

    public class ModemLoosePostObjects
    {
        public Dictionary<int, LooseBhaPosts> LoosePostDict = new Dictionary<int, LooseBhaPosts>();

        public ModemLoosePostObjects()
        {

        }
    }

    public class MwdBhaPosts
    {
        public string P_BHA_NUM { get; set; } = "";
        public string P_L_ORIFFICESIZE { get; set; } = " ";
        public string P_POPPET_STANDOFF { get; set; } = "";
        public string P_L_IMPELLERSIZE { get; set; } = " ";
        public string P_EXP_MAX_TEMP { get; set; } = "";
        public string P_L_STATORSIZE { get; set; } = " ";
        public string P_BHA_DESC { get; set; } = "";
        public string P_PULSER_OILTYPE { get; set; } = "";
        public string P_MWDDWD_ADD_INFO { get; set; } = "";
        public string P_HC_TOOL { get; set; } = "Yes";
        public string P_RASOURCE_ID { get; set; } = "";

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

    public class DdBhaPosts
    {
        public string P_MOTOR_NUM { get; set; } = "";
        public string P_MOTOR_DESC { get; set; } = "";
        public string P_L_MOBSS_HOLESEC { get; set; } = "";
        public string P_FLOWRATE_MIN { get; set; } = "";
        public string P_FLOWRATE_MAX { get; set; } = "";
        public string P_FLOW_RATE_UNIT { get; set; } = "";
        public string P_WOB { get; set; } = "";
        public string P_BIT_PRESSURE_DROP { get; set; } = "";
        public string P_PRES_UNIT { get; set; } = "";
        public string P_MOTOR_DIFFPRESS { get; set; } = "";
        public string P_MUD_TYPE { get; set; } = "";
        public string P_STATOR_RUBBER { get; set; } = "";
        public string P_PLAN_TEMP_START { get; set; } = "";
        public string P_MAX_TEMP { get; set; } = "";
        public string P_TEMP_UNIT { get; set; } = "";
        public string P_SLICK_BORE { get; set; } = "";
        public string P_L_MOBSS_MOTORSIZE { get; set; } = "";
        public string P_L_MOBSS_LOBERATIO { get; set; } = "";
        public string P_L_MOTORTYPE { get; set; } = "";
        public string P_L_MOTOR_CATEGORY { get; set; } = "";
        public string P_FLEXI_STATOR { get; set; } = "";
        public string P_ROTOR_CATCHERS { get; set; } = "";
        public string P_L_ROTOR_COATING { get; set; } = "";
        public string P_L_BEND { get; set; } = "";
        public string P_ROTOR_NOZZLE { get; set; } = "";
        public string P_DUMP_SUB { get; set; } = "";
        public string P_PIN_DOWN_SHAFT { get; set; } = "";
        public string P_L_UPHOLE { get; set; } = "";
        public string P_L_DRIVESHAFT { get; set; } = "";
        public string P_SLEEVE_FITTED { get; set; } = "";
        public string P_SLEEVE_GAUGE { get; set; } = "";
        public string P_PAD { get; set; } = "";
        public string P_PAD_SIZE { get; set; } = "";
        public string P_ABI_REQ { get; set; } = "";
        public string P_BIT_TYPE { get; set; } = "";
        public string P_BIT_TORQUED { get; set; } = "";
        public string P_BIT_MAKEUP_TORQUE { get; set; } = "";
        public string P_BIT_SN { get; set; } = "";
        public string P_COMMENTS { get; set; } = "";

    }

    public class DdCompPosts
    {
        public string P_SEQ_NO { get; set; } = "";
        public string P_L_MWDTORQUE_TORQUE { get; set; } = "";
        public string P_L_THREAD_TOP_THREADSIZE { get; set; } = "";
        public string P_L_THREAD_BTM_THREADSIZE { get; set; } = "";
        public string P_DESCRIPTION { get; set; } = "";
        public string P_COMMENTS { get; set; } = "";

    }

    public class GpBhaPosts
    {
        public Tuple<string, string> P_10 { get; set; } = new Tuple<string, string>("P_10", "");
        public Tuple<string, string> P_GP_ID { get; set; } = new Tuple<string, string>("P_GP_ID", "");
        public Tuple<string, string> O_GP_ID { get; set; } = new Tuple<string, string>("O_GP_ID", "");
        public Tuple<string, string> H_GP_COMPL_WARN { get; set; } = new Tuple<string, string>("H_GP_COMPL_WARN", "");
        public Tuple<string, string> H_DIV0 { get; set; } = new Tuple<string, string>("H_DIV0", "");
        public Tuple<string, string> H_DIV1 { get; set; } = new Tuple<string, string>("H_DIV1", "");
        public Tuple<string, string> P_PILOT_NUM { get; set; } = new Tuple<string, string>("P_PILOT_NUM", "");
        public Tuple<string, string> H_DIV30 { get; set; } = new Tuple<string, string>("H_DIV30", "");
        public Tuple<string, string> P_GP_DESC { get; set; } = new Tuple<string, string>("P_GP_DESC", "");
        public Tuple<string, string> H_LBL_APPDET { get; set; } = new Tuple<string, string>("H_LBL_APPDET", "");
        public Tuple<string, string> H_DIV5 { get; set; } = new Tuple<string, string>("H_DIV5", "");
        public Tuple<string, string> H_DIV2 { get; set; } = new Tuple<string, string>("H_DIV2", "");
        public Tuple<string, string> P_L_HOLESEC { get; set; } = new Tuple<string, string>("P_L_HOLESEC", "");
        public Tuple<string, string> H_DIV25 { get; set; } = new Tuple<string, string>("H_DIV25", "");
        public Tuple<string, string> P_FLOW_RATE { get; set; } = new Tuple<string, string>("P_FLOW_RATE", "");
        public Tuple<string, string> P_FLOW_RATE_TO { get; set; } = new Tuple<string, string>("P_FLOW_RATE_TO", "");
        public Tuple<string, string> P_FLOW_RATE_UNIT { get; set; } = new Tuple<string, string>("P_FLOW_RATE_UNIT", "");
        public Tuple<string, string> P_MAX_HYD_PRES { get; set; } = new Tuple<string, string>("P_MAX_HYD_PRES", "");
        public Tuple<string, string> P_MAX_HYD_PRES_UNIT { get; set; } = new Tuple<string, string>("P_MAX_HYD_PRES_UNIT", "");
        public Tuple<string, string> P_PLAN_TEMP_START { get; set; } = new Tuple<string, string>("P_PLAN_TEMP_START", "");
        public Tuple<string, string> P_TEMP_UNIT { get; set; } = new Tuple<string, string>("P_TEMP_UNIT", "");
        public Tuple<string, string> H_DIV3 { get; set; } = new Tuple<string, string>("H_DIV3", "");
        public Tuple<string, string> P_MAX_TEMP { get; set; } = new Tuple<string, string>("P_MAX_TEMP", "");
        public Tuple<string, string> H_DIV4 { get; set; } = new Tuple<string, string>("H_DIV4", "");
        public Tuple<string, string> P_L_OILTYPE { get; set; } = new Tuple<string, string>("P_L_OILTYPE", "");
        public Tuple<string, string> H_LBL_CONFDET { get; set; } = new Tuple<string, string>("H_LBL_CONFDET", "");
        public Tuple<string, string> H_DIV6 { get; set; } = new Tuple<string, string>("H_DIV6", "");
        public Tuple<string, string> H_DIV12 { get; set; } = new Tuple<string, string>("H_DIV12", "");
        public Tuple<string, string> P_L_GPSIZE { get; set; } = new Tuple<string, string>("P_L_GPSIZE", "");
        public Tuple<string, string> H_DIV11 { get; set; } = new Tuple<string, string>("H_DIV11", "");
        public Tuple<string, string> P_L_MRSC_SUB_CONF_DESCR2 { get; set; } = new Tuple<string, string>("P_L_MRSC_SUB_CONF_DESCR2", "");
        public Tuple<string, string> H_DIV13 { get; set; } = new Tuple<string, string>("H_DIV13", "");
        public Tuple<string, string> P_L_ABITYPE { get; set; } = new Tuple<string, string>("P_L_ABITYPE", "1");
        public Tuple<string, string> H_DIV19 { get; set; } = new Tuple<string, string>("H_DIV19", "");
        public Tuple<string, string> P_FLEXJOINT_REQ { get; set; } = new Tuple<string, string>("P_FLEXJOINT_REQ", "");
        public Tuple<string, string> H_DIV26 { get; set; } = new Tuple<string, string>("H_DIV26", "");
        public Tuple<string, string> P_FLEX_TORQUED { get; set; } = new Tuple<string, string>("P_FLEX_TORQUED", "");
        public Tuple<string, string> H_DIV14 { get; set; } = new Tuple<string, string>("H_DIV14", "");
        public Tuple<string, string> P_GP_CLAMP_REQ { get; set; } = new Tuple<string, string>("P_GP_CLAMP_REQ", "");
        public Tuple<string, string> H_DIV21 { get; set; } = new Tuple<string, string>("H_DIV21", "");
        public Tuple<string, string> P_DM_TYPE { get; set; } = new Tuple<string, string>("P_DM_TYPE", "");
        public Tuple<string, string> H_DIV15 { get; set; } = new Tuple<string, string>("H_DIV15", "");
        public Tuple<string, string> P_L_SOFTWARE_DM { get; set; } = new Tuple<string, string>("P_L_SOFTWARE_DM", "");
        public Tuple<string, string> H_DIV17 { get; set; } = new Tuple<string, string>("H_DIV17", "");
        public Tuple<string, string> P_L_SOFTWARE_GP { get; set; } = new Tuple<string, string>("P_L_SOFTWARE_GP", "");
        public Tuple<string, string> H_DIV24 { get; set; } = new Tuple<string, string>("H_DIV24", "");
        public Tuple<string, string> P_L_THREAD_UP { get; set; } = new Tuple<string, string>("P_L_THREAD_UP", "");
        public Tuple<string, string> H_DIV23 { get; set; } = new Tuple<string, string>("H_DIV23", "");
        public Tuple<string, string> P_L_THREAD_LOW { get; set; } = new Tuple<string, string>("P_L_THREAD_LOW", "");
        public Tuple<string, string> H_DIV18 { get; set; } = new Tuple<string, string>("H_DIV18", "");
        public Tuple<string, string> P_BIT_TYPE { get; set; } = new Tuple<string, string>("P_BIT_TYPE", "");
        public Tuple<string, string> H_DIV16 { get; set; } = new Tuple<string, string>("H_DIV16", "");
        public Tuple<string, string> P_BIT_TORQUED { get; set; } = new Tuple<string, string>("P_BIT_TORQUED", "");
        public Tuple<string, string> H_DIV99 { get; set; } = new Tuple<string, string>("H_DIV99", "");
        public Tuple<string, string> P_BIT_MAKEUP_TORQUE { get; set; } = new Tuple<string, string>("P_BIT_MAKEUP_TORQUE", "");
        public Tuple<string, string> H_DIV20 { get; set; } = new Tuple<string, string>("H_DIV20", "");
        public Tuple<string, string> P_BIT_SN { get; set; } = new Tuple<string, string>("P_BIT_SN", "");
        public Tuple<string, string> H_DIV22 { get; set; } = new Tuple<string, string>("H_DIV22", "");
        public Tuple<string, string> P_LITHIUM_USED { get; set; } = new Tuple<string, string>("P_LITHIUM_USED", "");
        public Tuple<string, string> P_GP_COMMENT { get; set; } = new Tuple<string, string>("P_GP_COMMENT", "");
        public Tuple<string, string> P_L_LOWHOUSEDESC { get; set; } = new Tuple<string, string>("P_L_LOWHOUSEDESC", "");
        public Tuple<string, string> H_DEL_BHA { get; set; } = new Tuple<string, string>("H_DEL_BHA", "");
        public Tuple<string, string> P_S2 { get; set; } = new Tuple<string, string>("P_S2", "");
        public Tuple<string, string> H_L_PRECON_STATUS { get; set; } = new Tuple<string, string>("H_L_PRECON_STATUS", "");
        public Tuple<string, string> z_modified { get; set; } = new Tuple<string, string>("z_modified", "N");
        public Tuple<string, string> O_PILOT_NUM { get; set; } = new Tuple<string, string>("O_PILOT_NUM", "");
        public Tuple<string, string> O_GP_DESC { get; set; } = new Tuple<string, string>("O_GP_DESC", "");
        public Tuple<string, string> O_FLOW_RATE { get; set; } = new Tuple<string, string>("O_FLOW_RATE", "");
        public Tuple<string, string> O_FLOW_RATE_TO { get; set; } = new Tuple<string, string>("O_FLOW_RATE_TO", "");
        public Tuple<string, string> O_FLOW_RATE_UNIT { get; set; } = new Tuple<string, string>("O_FLOW_RATE_UNIT", "");
        public Tuple<string, string> O_MAX_HYD_PRES { get; set; } = new Tuple<string, string>("O_MAX_HYD_PRES", "");
        public Tuple<string, string> O_MAX_HYD_PRES_UNIT { get; set; } = new Tuple<string, string>("O_MAX_HYD_PRES_UNIT", "");
        public Tuple<string, string> O_PLAN_TEMP_START { get; set; } = new Tuple<string, string>("O_PLAN_TEMP_START", "");
        public Tuple<string, string> O_TEMP_UNIT { get; set; } = new Tuple<string, string>("O_TEMP_UNIT", "");
        public Tuple<string, string> O_MAX_TEMP { get; set; } = new Tuple<string, string>("O_MAX_TEMP", "");
        public Tuple<string, string> O_ROLLER_STYLE { get; set; } = new Tuple<string, string>("O_ROLLER_STYLE", "");
        public Tuple<string, string> O_FLEXJOINT_REQ { get; set; } = new Tuple<string, string>("O_FLEXJOINT_REQ", "");
        public Tuple<string, string> O_FLEX_TORQUED { get; set; } = new Tuple<string, string>("O_FLEX_TORQUED", "");
        public Tuple<string, string> O_GP_CLAMP_REQ { get; set; } = new Tuple<string, string>("O_GP_CLAMP_REQ", "");
        public Tuple<string, string> O_DM_TYPE { get; set; } = new Tuple<string, string>("O_DM_TYPE", "");
        public Tuple<string, string> O_BIT_TYPE { get; set; } = new Tuple<string, string>("O_BIT_TYPE", "");
        public Tuple<string, string> O_BIT_TORQUED { get; set; } = new Tuple<string, string>("O_BIT_TORQUED", "");
        public Tuple<string, string> O_BIT_MAKEUP_TORQUE { get; set; } = new Tuple<string, string>("O_BIT_MAKEUP_TORQUE", "");
        public Tuple<string, string> O_BIT_SN { get; set; } = new Tuple<string, string>("O_BIT_SN", "");
        public Tuple<string, string> O_LITHIUM_USED { get; set; } = new Tuple<string, string>("O_LITHIUM_USED", "Y");
        public Tuple<string, string> O_ABITYPE_ID { get; set; } = new Tuple<string, string>("O_ABITYPE_ID", "");
        public Tuple<string, string> O_CONN_LOWER_ID { get; set; } = new Tuple<string, string>("O_CONN_LOWER_ID", "1");
        public Tuple<string, string> O_CONN_UPHOLE_ID { get; set; } = new Tuple<string, string>("O_CONN_UPHOLE_ID", "1");
        public Tuple<string, string> O_GPSIZE_ID { get; set; } = new Tuple<string, string>("O_GPSIZE_ID", "1");
        public Tuple<string, string> O_HOLESEC_ID { get; set; } = new Tuple<string, string>("O_HOLESEC_ID", "1");
        public Tuple<string, string> O_OILTYPE_ID { get; set; } = new Tuple<string, string>("O_OILTYPE_ID", "1");
        public Tuple<string, string> O_ORDER_ID { get; set; } = new Tuple<string, string>("O_ORDER_ID", "");
        public Tuple<string, string> O_SW_DM_ID { get; set; } = new Tuple<string, string>("O_SW_DM_ID", "1");
        public Tuple<string, string> O_SW_GP_ID { get; set; } = new Tuple<string, string>("O_SW_GP_ID", "1");
        public Tuple<string, string> O_GP_COMMENT { get; set; } = new Tuple<string, string>("O_GP_COMMENT", "");
        public Tuple<string, string> O_LWR_SLICK_HOUS { get; set; } = new Tuple<string, string>("O_LWR_SLICK_HOUS", "");
        public Tuple<string, string> O_STATUS { get; set; } = new Tuple<string, string>("O_STATUS", "");
        public Tuple<string, string> O_SUB_CONF_ID { get; set; } = new Tuple<string, string>("O_SUB_CONF_ID", "");
        public Tuple<string, string> O_LOWHOUSEID { get; set; } = new Tuple<string, string>("O_LOWHOUSEID", "");
        public Tuple<string, string> O_S2 { get; set; } = new Tuple<string, string>("O_S2", "");
        public Tuple<string, string> O_PRECON_ID { get; set; } = new Tuple<string, string>("O_PRECON_ID", "");
        public Tuple<string, string> P_GP_ID_o { get; set; } = new Tuple<string, string>("P_GP_ID", "");
        public Tuple<string, string> P_PILOT_NUM_o { get; set; } = new Tuple<string, string>("P_PILOT_NUM", "");
        public Tuple<string, string> P_GP_DESC_o { get; set; } = new Tuple<string, string>("P_GP_DESC", "");
        public Tuple<string, string> P_L_HOLESEC_o { get; set; } = new Tuple<string, string>("P_L_HOLESEC", "");
        public Tuple<string, string> P_FLOW_RATE_o { get; set; } = new Tuple<string, string>("P_FLOW_RATE", "");
        public Tuple<string, string> P_FLOW_RATE_TO_o { get; set; } = new Tuple<string, string>("P_FLOW_RATE_TO", "");
        public Tuple<string, string> P_FLOW_RATE_UNIT_o { get; set; } = new Tuple<string, string>("P_FLOW_RATE_UNIT", "");
        public Tuple<string, string> P_MAX_HYD_PRES_o { get; set; } = new Tuple<string, string>("P_MAX_HYD_PRES", "");
        public Tuple<string, string> P_MAX_HYD_PRES_UNIT_o { get; set; } = new Tuple<string, string>("P_MAX_HYD_PRES_UNIT", "");
        public Tuple<string, string> P_PLAN_TEMP_START_o { get; set; } = new Tuple<string, string>("P_PLAN_TEMP_START", "");
        public Tuple<string, string> P_TEMP_UNIT_o { get; set; } = new Tuple<string, string>("P_TEMP_UNIT", "");
        public Tuple<string, string> P_MAX_TEMP_o { get; set; } = new Tuple<string, string>("P_MAX_TEMP", "");
        public Tuple<string, string> P_L_OILTYPE_o { get; set; } = new Tuple<string, string>("P_L_OILTYPE", "");
        public Tuple<string, string> P_L_GPSIZE_o { get; set; } = new Tuple<string, string>("P_L_GPSIZE", "");
        public Tuple<string, string> P_L_MRSC_SUB_CONF_DESCR2_o { get; set; } = new Tuple<string, string>("P_L_MRSC_SUB_CONF_DESCR2", "");
        public Tuple<string, string> P_L_ABITYPE_o { get; set; } = new Tuple<string, string>("P_L_ABITYPE", "");
        public Tuple<string, string> P_FLEXJOINT_REQ_o { get; set; } = new Tuple<string, string>("P_FLEXJOINT_REQ", "");
        public Tuple<string, string> P_FLEX_TORQUED_o { get; set; } = new Tuple<string, string>("P_FLEX_TORQUED", "");
        public Tuple<string, string> P_GP_CLAMP_REQ_o { get; set; } = new Tuple<string, string>("P_GP_CLAMP_REQ", "");
        public Tuple<string, string> P_DM_TYPE_o { get; set; } = new Tuple<string, string>("P_DM_TYPE", "");
        public Tuple<string, string> P_L_SOFTWARE_DM_o { get; set; } = new Tuple<string, string>("P_L_SOFTWARE_DM", "");
        public Tuple<string, string> P_L_SOFTWARE_GP_o { get; set; } = new Tuple<string, string>("P_L_SOFTWARE_GP", "");
        public Tuple<string, string> P_L_THREAD_UP_o { get; set; } = new Tuple<string, string>("P_L_THREAD_UP", "");
        public Tuple<string, string> P_L_THREAD_LOW_o { get; set; } = new Tuple<string, string>("P_L_THREAD_LOW", "");
        public Tuple<string, string> P_BIT_TYPE_o { get; set; } = new Tuple<string, string>("P_BIT_TYPE", "");
        public Tuple<string, string> P_BIT_TORQUED_o { get; set; } = new Tuple<string, string>("P_BIT_TORQUED", "");
        public Tuple<string, string> P_BIT_MAKEUP_TORQUE_o { get; set; } = new Tuple<string, string>("P_BIT_MAKEUP_TORQUE", "");
        public Tuple<string, string> P_BIT_SN_o { get; set; } = new Tuple<string, string>("P_BIT_SN", "");
        public Tuple<string, string> P_LITHIUM_USED_o { get; set; } = new Tuple<string, string>("P_LITHIUM_USED", "");
        public Tuple<string, string> P_GP_COMMENT_o { get; set; } = new Tuple<string, string>("P_GP_COMMENT", "");
        public Tuple<string, string> P_L_LOWHOUSEDESC_o { get; set; } = new Tuple<string, string>("P_L_LOWHOUSEDESC", "");
        public Tuple<string, string> P_S2_o { get; set; } = new Tuple<string, string>("P_S2", "");
        public Tuple<string, string> O_PILOT_NUM_o { get; set; } = new Tuple<string, string>("O_PILOT_NUM", "");
        public Tuple<string, string> O_GP_DESC_o { get; set; } = new Tuple<string, string>("O_GP_DESC", "");
        public Tuple<string, string> O_FLOW_RATE_o { get; set; } = new Tuple<string, string>("O_FLOW_RATE", "");
        public Tuple<string, string> O_FLOW_RATE_TO_o { get; set; } = new Tuple<string, string>("O_FLOW_RATE_TO", "");
        public Tuple<string, string> O_FLOW_RATE_UNIT_o { get; set; } = new Tuple<string, string>("O_FLOW_RATE_UNIT", "");
        public Tuple<string, string> O_MAX_HYD_PRES_o { get; set; } = new Tuple<string, string>("O_MAX_HYD_PRES", "");
        public Tuple<string, string> O_MAX_HYD_PRES_UNIT_o { get; set; } = new Tuple<string, string>("O_MAX_HYD_PRES_UNIT", "");
        public Tuple<string, string> O_PLAN_TEMP_START_o { get; set; } = new Tuple<string, string>("O_PLAN_TEMP_START", "");
        public Tuple<string, string> O_TEMP_UNIT_o { get; set; } = new Tuple<string, string>("O_TEMP_UNIT", "");
        public Tuple<string, string> O_MAX_TEMP_o { get; set; } = new Tuple<string, string>("O_MAX_TEMP", "");
        public Tuple<string, string> O_ROLLER_STYLE_o { get; set; } = new Tuple<string, string>("O_ROLLER_STYLE", "");
        public Tuple<string, string> O_FLEXJOINT_REQ_o { get; set; } = new Tuple<string, string>("O_FLEXJOINT_REQ", "");
        public Tuple<string, string> O_FLEX_TORQUED_o { get; set; } = new Tuple<string, string>("O_FLEX_TORQUED", "");
        public Tuple<string, string> O_GP_CLAMP_REQ_o { get; set; } = new Tuple<string, string>("O_GP_CLAMP_REQ", "");
        public Tuple<string, string> O_DM_TYPE_o { get; set; } = new Tuple<string, string>("O_DM_TYPE", "");
        public Tuple<string, string> O_BIT_TYPE_o { get; set; } = new Tuple<string, string>("O_BIT_TYPE", "");
        public Tuple<string, string> O_BIT_TORQUED_o { get; set; } = new Tuple<string, string>("O_BIT_TORQUED", "");
        public Tuple<string, string> O_BIT_MAKEUP_TORQUE_o { get; set; } = new Tuple<string, string>("O_BIT_MAKEUP_TORQUE", "");
        public Tuple<string, string> O_BIT_SN_o { get; set; } = new Tuple<string, string>("O_BIT_SN", "");
        public Tuple<string, string> O_LITHIUM_USED_o { get; set; } = new Tuple<string, string>("O_LITHIUM_USED", "");
        public Tuple<string, string> O_ABITYPE_ID_o { get; set; } = new Tuple<string, string>("O_ABITYPE_ID", "");
        public Tuple<string, string> O_CONN_LOWER_ID_o { get; set; } = new Tuple<string, string>("O_CONN_LOWER_ID", "");
        public Tuple<string, string> O_CONN_UPHOLE_ID_o { get; set; } = new Tuple<string, string>("O_CONN_UPHOLE_ID", "");
        public Tuple<string, string> O_GPSIZE_ID_o { get; set; } = new Tuple<string, string>("O_GPSIZE_ID", "");
        public Tuple<string, string> O_HOLESEC_ID_o { get; set; } = new Tuple<string, string>("O_HOLESEC_ID", "");
        public Tuple<string, string> O_OILTYPE_ID_o { get; set; } = new Tuple<string, string>("O_OILTYPE_ID", "");
        public Tuple<string, string> O_ORDER_ID_o { get; set; } = new Tuple<string, string>("O_ORDER_ID", "");
        public Tuple<string, string> O_SW_DM_ID_o { get; set; } = new Tuple<string, string>("O_SW_DM_ID", "");
        public Tuple<string, string> O_SW_GP_ID_o { get; set; } = new Tuple<string, string>("O_SW_GP_ID", "");
        public Tuple<string, string> O_GP_COMMENT_o { get; set; } = new Tuple<string, string>("O_GP_COMMENT", "");
        public Tuple<string, string> O_LWR_SLICK_HOUS_o { get; set; } = new Tuple<string, string>("O_LWR_SLICK_HOUS", "");
        public Tuple<string, string> O_STATUS_o { get; set; } = new Tuple<string, string>("O_STATUS", "");
        public Tuple<string, string> O_SUB_CONF_ID_o { get; set; } = new Tuple<string, string>("O_SUB_CONF_ID", "");
        public Tuple<string, string> O_LOWHOUSEID_o { get; set; } = new Tuple<string, string>("O_LOWHOUSEID", "");
        public Tuple<string, string> O_S2_o { get; set; } = new Tuple<string, string>("O_S2", "");
        public Tuple<string, string> O_PRECON_ID_o { get; set; } = new Tuple<string, string>("O_PRECON_ID", "");
        public Tuple<string, string> H_GP_COMPL_WARN_o { get; set; } = new Tuple<string, string>("H_GP_COMPL_WARN", "");
        public Tuple<string, string> H_DIV0_o { get; set; } = new Tuple<string, string>("H_DIV0", "");
        public Tuple<string, string> H_DIV1_o { get; set; } = new Tuple<string, string>("H_DIV1", "");
        public Tuple<string, string> H_DIV30_o { get; set; } = new Tuple<string, string>("H_DIV30", "");
        public Tuple<string, string> H_LBL_APPDET_o { get; set; } = new Tuple<string, string>("H_LBL_APPDET", "");
        public Tuple<string, string> H_DIV5_o { get; set; } = new Tuple<string, string>("H_DIV5", "");
        public Tuple<string, string> H_DIV2_o { get; set; } = new Tuple<string, string>("H_DIV2", "");
        public Tuple<string, string> H_DIV25_o { get; set; } = new Tuple<string, string>("H_DIV25", "");
        public Tuple<string, string> H_DIV3_o { get; set; } = new Tuple<string, string>("H_DIV3", "");
        public Tuple<string, string> H_DIV4_o { get; set; } = new Tuple<string, string>("H_DIV4", "");
        public Tuple<string, string> H_LBL_CONFDET_o { get; set; } = new Tuple<string, string>("H_LBL_CONFDET", "");
        public Tuple<string, string> H_DIV6_o { get; set; } = new Tuple<string, string>("H_DIV6", "");
        public Tuple<string, string> H_DIV12_o { get; set; } = new Tuple<string, string>("H_DIV12", "");
        public Tuple<string, string> H_DIV11_o { get; set; } = new Tuple<string, string>("H_DIV11", "");
        public Tuple<string, string> H_DIV13_o { get; set; } = new Tuple<string, string>("H_DIV13", "");
        public Tuple<string, string> H_DIV19_o { get; set; } = new Tuple<string, string>("H_DIV19", "");
        public Tuple<string, string> H_DIV26_o { get; set; } = new Tuple<string, string>("H_DIV26", "");
        public Tuple<string, string> H_DIV14_o { get; set; } = new Tuple<string, string>("H_DIV14", "");
        public Tuple<string, string> H_DIV21_o { get; set; } = new Tuple<string, string>("H_DIV21", "");
        public Tuple<string, string> H_DIV15_o { get; set; } = new Tuple<string, string>("H_DIV15", "");
        public Tuple<string, string> H_DIV17_o { get; set; } = new Tuple<string, string>("H_DIV17", "");
        public Tuple<string, string> H_DIV24_o { get; set; } = new Tuple<string, string>("H_DIV24", "");
        public Tuple<string, string> H_DIV23_o { get; set; } = new Tuple<string, string>("H_DIV23", "");
        public Tuple<string, string> H_DIV18_o { get; set; } = new Tuple<string, string>("H_DIV18", "");
        public Tuple<string, string> H_DIV16_o { get; set; } = new Tuple<string, string>("H_DIV16", "");
        public Tuple<string, string> H_DIV99_o { get; set; } = new Tuple<string, string>("H_DIV99", "");
        public Tuple<string, string> H_DIV20_o { get; set; } = new Tuple<string, string>("H_DIV20", "");
        public Tuple<string, string> H_DIV22_o { get; set; } = new Tuple<string, string>("H_DIV22", "");
        public Tuple<string, string> H_DEL_BHA_o { get; set; } = new Tuple<string, string>("H_DEL_BHA", "");
        public Tuple<string, string> H_L_PRECON_STATUS_o { get; set; } = new Tuple<string, string>("H_L_PRECON_STATUS", "");
        public Tuple<string, string> z_modified_o { get; set; } = new Tuple<string, string>("z_modified", "dummy_row");
        public Tuple<string, string> Z_ACTION { get; set; } = new Tuple<string, string>("Z_ACTION", "UPDATE");
        public Tuple<string, string> Z_CHK { get; set; } = new Tuple<string, string>("Z_CHK", "");
        public Tuple<string, string> Q_L_MOBGP_ORDER_SSORD_ID { get; set; } = new Tuple<string, string>("Q_L_MOBGP_ORDER_SSORD_ID", "");
        public Tuple<string, string> Z_START { get; set; } = new Tuple<string, string>("Z_START", "1");

        public GpBhaPosts()
        {

        }

        

        //public void Refresh(HtmlAgilityPack.HtmlDocument hDoc)
        //{
        //    P_10 = Tuple.Create(P_10.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_10.Item1, hDoc));
        //    P_GP_ID = Tuple.Create(P_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_GP_ID.Item1, hDoc));
        //    O_GP_ID = Tuple.Create(O_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_ID.Item1, hDoc));
        //    H_GP_COMPL_WARN = Tuple.Create(H_GP_COMPL_WARN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_GP_COMPL_WARN.Item1, hDoc));
        //    H_DIV0 = Tuple.Create(H_DIV0.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV0.Item1, hDoc));
        //    H_DIV1 = Tuple.Create(H_DIV1.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV1.Item1, hDoc));
        //    P_PILOT_NUM = Tuple.Create(P_PILOT_NUM.Item1, HDocUtility.GetSelectedElementById(P_PILOT_NUM.Item1, hDoc));
        //    H_DIV30 = Tuple.Create(H_DIV30.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV30.Item1, hDoc));
        //    P_GP_DESC = Tuple.Create(P_GP_DESC.Item1, HDocUtility.GetInputById(P_GP_DESC.Item1, hDoc));
        //    H_LBL_APPDET = Tuple.Create(H_LBL_APPDET.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_LBL_APPDET.Item1, hDoc));
        //    H_DIV5 = Tuple.Create(H_DIV5.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV5.Item1, hDoc));
        //    H_DIV2 = Tuple.Create(H_DIV2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV2.Item1, hDoc));
        //    string temp = HDocUtility.GetSelectedElementById(P_L_HOLESEC.Item1, hDoc);
        //    MessageBox.Show("Break 5a: " + temp);
        //    MessageBox.Show("Break 5b: " + P_L_HOLESEC.Item1);
        //    P_L_HOLESEC = Tuple.Create(P_L_HOLESEC.Item1, temp);
        //    MessageBox.Show("Break 5c: " + P_L_HOLESEC.Item1);
        //    MessageBox.Show("Break 5d: " + P_L_HOLESEC.Item2);
        //    //P_L_HOLESEC = Tuple.Create(P_L_HOLESEC.Item1, HDocUtility.GetSelectedElementById(P_L_HOLESEC.Item1, hDoc));
        //    H_DIV25 = Tuple.Create(H_DIV25.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV25.Item1, hDoc));
        //    P_FLOW_RATE = Tuple.Create(P_FLOW_RATE.Item1, HDocUtility.GetInputById(P_FLOW_RATE.Item1, hDoc));
        //    P_FLOW_RATE_TO = Tuple.Create(P_FLOW_RATE_TO.Item1, HDocUtility.GetInputById(P_FLOW_RATE_TO.Item1, hDoc));
        //    P_FLOW_RATE_UNIT = Tuple.Create(P_FLOW_RATE_UNIT.Item1, HDocUtility.GetSelectedElementById(P_FLOW_RATE_UNIT.Item1, hDoc));
        //    P_MAX_HYD_PRES = Tuple.Create(P_MAX_HYD_PRES.Item1, HDocUtility.GetInputById(P_MAX_HYD_PRES.Item1, hDoc));
        //    P_MAX_HYD_PRES_UNIT = Tuple.Create(P_MAX_HYD_PRES_UNIT.Item1, HDocUtility.GetSelectedElementById(P_MAX_HYD_PRES_UNIT.Item1, hDoc));
        //    P_PLAN_TEMP_START = Tuple.Create(P_PLAN_TEMP_START.Item1, HDocUtility.GetInputById(P_PLAN_TEMP_START.Item1, hDoc));
        //    P_TEMP_UNIT = Tuple.Create(P_TEMP_UNIT.Item1, HDocUtility.GetSelectedElementById(P_TEMP_UNIT.Item1, hDoc));
        //    H_DIV3 = Tuple.Create(H_DIV3.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV3.Item1, hDoc));
        //    P_MAX_TEMP = Tuple.Create(P_MAX_TEMP.Item1, HDocUtility.GetInputById(P_MAX_TEMP.Item1, hDoc));
        //    H_DIV4 = Tuple.Create(H_DIV4.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV4.Item1, hDoc));
        //    P_L_OILTYPE = Tuple.Create(P_L_OILTYPE.Item1, HDocUtility.GetSelectedElementById(P_L_OILTYPE.Item1, hDoc));
        //    H_LBL_CONFDET = Tuple.Create(H_LBL_CONFDET.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_LBL_CONFDET.Item1, hDoc));
        //    H_DIV6 = Tuple.Create(H_DIV6.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV6.Item1, hDoc));
        //    H_DIV12 = Tuple.Create(H_DIV12.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV12.Item1, hDoc));
        //    P_L_GPSIZE = Tuple.Create(P_L_GPSIZE.Item1, HDocUtility.GetSelectedElementById(P_L_GPSIZE.Item1, hDoc));
        //    H_DIV11 = Tuple.Create(H_DIV11.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV11.Item1, hDoc));
        //    P_L_MRSC_SUB_CONF_DESCR2 = Tuple.Create(P_L_MRSC_SUB_CONF_DESCR2.Item1, HDocUtility.GetSelectedElementById(P_L_MRSC_SUB_CONF_DESCR2.Item1, hDoc));
        //    H_DIV13 = Tuple.Create(H_DIV13.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV13.Item1, hDoc));
        //    P_L_ABITYPE = Tuple.Create(P_L_ABITYPE.Item1, HDocUtility.GetSelectedElementById(P_L_ABITYPE.Item1, hDoc));
        //    H_DIV19 = Tuple.Create(H_DIV19.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV19.Item1, hDoc));
        //    P_FLEXJOINT_REQ = Tuple.Create(P_FLEXJOINT_REQ.Item1, HDocUtility.GetSelectedElementById(P_FLEXJOINT_REQ.Item1, hDoc));
        //    H_DIV26 = Tuple.Create(H_DIV26.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV26.Item1, hDoc));
        //    P_FLEX_TORQUED = Tuple.Create(P_FLEX_TORQUED.Item1, HDocUtility.GetSelectedElementById(P_FLEX_TORQUED.Item1, hDoc));
        //    H_DIV14 = Tuple.Create(H_DIV14.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV14.Item1, hDoc));
        //    P_GP_CLAMP_REQ = Tuple.Create(P_GP_CLAMP_REQ.Item1, HDocUtility.GetSelectedElementById(P_GP_CLAMP_REQ.Item1, hDoc));
        //    H_DIV21 = Tuple.Create(H_DIV21.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV21.Item1, hDoc));
        //    P_DM_TYPE = Tuple.Create(P_DM_TYPE.Item1, HDocUtility.GetSelectedElementById(P_DM_TYPE.Item1, hDoc));
        //    H_DIV15 = Tuple.Create(H_DIV15.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV15.Item1, hDoc));
        //    P_L_SOFTWARE_DM = Tuple.Create(P_L_SOFTWARE_DM.Item1, HDocUtility.GetSelectedElementById(P_L_SOFTWARE_DM.Item1, hDoc));
        //    H_DIV17 = Tuple.Create(H_DIV17.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV17.Item1, hDoc));
        //    P_L_SOFTWARE_GP = Tuple.Create(P_L_SOFTWARE_GP.Item1, HDocUtility.GetSelectedElementById(P_L_SOFTWARE_GP.Item1, hDoc));
        //    H_DIV24 = Tuple.Create(H_DIV24.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV24.Item1, hDoc));
        //    P_L_THREAD_UP = Tuple.Create(P_L_THREAD_UP.Item1, HDocUtility.GetSelectedElementById(P_L_THREAD_UP.Item1, hDoc));
        //    H_DIV23 = Tuple.Create(H_DIV23.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV23.Item1, hDoc));
        //    P_L_THREAD_LOW = Tuple.Create(P_L_THREAD_LOW.Item1, HDocUtility.GetSelectedElementById(P_L_THREAD_LOW.Item1, hDoc));
        //    H_DIV18 = Tuple.Create(H_DIV18.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV18.Item1, hDoc));
        //    P_BIT_TYPE = Tuple.Create(P_BIT_TYPE.Item1, HDocUtility.GetInputById(P_BIT_TYPE.Item1, hDoc));
        //    H_DIV16 = Tuple.Create(H_DIV16.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV16.Item1, hDoc));
        //    P_BIT_TORQUED = Tuple.Create(P_BIT_TORQUED.Item1, HDocUtility.GetSelectedElementById(P_BIT_TORQUED.Item1, hDoc));
        //    H_DIV99 = Tuple.Create(H_DIV99.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV99.Item1, hDoc));
        //    P_BIT_MAKEUP_TORQUE = Tuple.Create(P_BIT_MAKEUP_TORQUE.Item1, HDocUtility.GetInputById(P_BIT_MAKEUP_TORQUE.Item1, hDoc));
        //    H_DIV20 = Tuple.Create(H_DIV20.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV20.Item1, hDoc));
        //    P_BIT_SN = Tuple.Create(P_BIT_SN.Item1, HDocUtility.GetInputById(P_BIT_SN.Item1, hDoc));
        //    H_DIV22 = Tuple.Create(H_DIV22.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV22.Item1, hDoc));
        //    P_LITHIUM_USED = Tuple.Create(P_LITHIUM_USED.Item1, HDocUtility.GetSelectedElementById(P_LITHIUM_USED.Item1, hDoc));
        //    P_GP_COMMENT = Tuple.Create(P_GP_COMMENT.Item1, HDocUtility.GetTextAreaById(P_GP_COMMENT.Item1, hDoc));
        //    P_L_LOWHOUSEDESC = Tuple.Create(P_L_LOWHOUSEDESC.Item1, HDocUtility.GetSelectedElementById(P_L_LOWHOUSEDESC.Item1, hDoc));
        //    H_DEL_BHA = Tuple.Create(H_DEL_BHA.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DEL_BHA.Item1, hDoc));
        //    P_S2 = Tuple.Create(P_S2.Item1, HDocUtility.GetInputById(P_S2.Item1, hDoc));
        //    H_L_PRECON_STATUS = Tuple.Create(H_L_PRECON_STATUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_L_PRECON_STATUS.Item1, hDoc));

        //    O_PILOT_NUM = Tuple.Create(O_PILOT_NUM.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_PILOT_NUM.Item1, hDoc));
        //    O_GP_DESC = Tuple.Create(O_GP_DESC.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_DESC.Item1, hDoc));
        //    O_FLOW_RATE = Tuple.Create(O_FLOW_RATE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLOW_RATE.Item1, hDoc));
        //    O_FLOW_RATE_TO = Tuple.Create(O_FLOW_RATE_TO.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLOW_RATE_TO.Item1, hDoc));
        //    O_FLOW_RATE_UNIT = Tuple.Create(O_FLOW_RATE_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLOW_RATE_UNIT.Item1, hDoc));
        //    O_MAX_HYD_PRES = Tuple.Create(O_MAX_HYD_PRES.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_MAX_HYD_PRES.Item1, hDoc));
        //    O_MAX_HYD_PRES_UNIT = Tuple.Create(O_MAX_HYD_PRES_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_MAX_HYD_PRES_UNIT.Item1, hDoc));
        //    O_PLAN_TEMP_START = Tuple.Create(O_PLAN_TEMP_START.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_PLAN_TEMP_START.Item1, hDoc));
        //    O_TEMP_UNIT = Tuple.Create(O_TEMP_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_TEMP_UNIT.Item1, hDoc));
        //    O_MAX_TEMP = Tuple.Create(O_MAX_TEMP.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_MAX_TEMP.Item1, hDoc));
        //    O_ROLLER_STYLE = Tuple.Create(O_ROLLER_STYLE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_ROLLER_STYLE.Item1, hDoc));
        //    O_FLEXJOINT_REQ = Tuple.Create(O_FLEXJOINT_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLEXJOINT_REQ.Item1, hDoc));
        //    O_FLEX_TORQUED = Tuple.Create(O_FLEX_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLEX_TORQUED.Item1, hDoc));
        //    O_GP_CLAMP_REQ = Tuple.Create(O_GP_CLAMP_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_CLAMP_REQ.Item1, hDoc));
        //    O_DM_TYPE = Tuple.Create(O_DM_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_DM_TYPE.Item1, hDoc));
        //    O_BIT_TYPE = Tuple.Create(O_BIT_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_TYPE.Item1, hDoc));
        //    O_BIT_TORQUED = Tuple.Create(O_BIT_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_TORQUED.Item1, hDoc));
        //    O_BIT_MAKEUP_TORQUE = Tuple.Create(O_BIT_MAKEUP_TORQUE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_MAKEUP_TORQUE.Item1, hDoc));
        //    O_BIT_SN = Tuple.Create(O_BIT_SN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_SN.Item1, hDoc));
        //    O_LITHIUM_USED = Tuple.Create(O_LITHIUM_USED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_LITHIUM_USED.Item1, hDoc));
        //    O_ABITYPE_ID = Tuple.Create(O_ABITYPE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_ABITYPE_ID.Item1, hDoc));
        //    O_CONN_LOWER_ID = Tuple.Create(O_CONN_LOWER_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_CONN_LOWER_ID.Item1, hDoc));
        //    O_CONN_UPHOLE_ID = Tuple.Create(O_CONN_UPHOLE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_CONN_UPHOLE_ID.Item1, hDoc));
        //    O_GPSIZE_ID = Tuple.Create(O_GPSIZE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GPSIZE_ID.Item1, hDoc));
        //    O_HOLESEC_ID = Tuple.Create(O_HOLESEC_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_HOLESEC_ID.Item1, hDoc));
        //    O_OILTYPE_ID = Tuple.Create(O_OILTYPE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_OILTYPE_ID.Item1, hDoc));
        //    O_ORDER_ID = Tuple.Create(O_ORDER_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_ORDER_ID.Item1, hDoc));
        //    O_SW_DM_ID = Tuple.Create(O_SW_DM_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_SW_DM_ID.Item1, hDoc));
        //    O_SW_GP_ID = Tuple.Create(O_SW_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_SW_GP_ID.Item1, hDoc));
        //    O_GP_COMMENT = Tuple.Create(O_GP_COMMENT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_COMMENT.Item1, hDoc));
        //    O_LWR_SLICK_HOUS = Tuple.Create(O_LWR_SLICK_HOUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_LWR_SLICK_HOUS.Item1, hDoc));
        //    O_STATUS = Tuple.Create(O_STATUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_STATUS.Item1, hDoc));
        //    O_SUB_CONF_ID = Tuple.Create(O_SUB_CONF_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_SUB_CONF_ID.Item1, hDoc));
        //    O_LOWHOUSEID = Tuple.Create(O_LOWHOUSEID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_LOWHOUSEID.Item1, hDoc));
        //    O_S2 = Tuple.Create(O_S2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_S2.Item1, hDoc));
        //    O_PRECON_ID = Tuple.Create(O_PRECON_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_PRECON_ID.Item1, hDoc));
        //    P_GP_ID = Tuple.Create(P_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_GP_ID.Item1, hDoc));
        //    P_PILOT_NUM = Tuple.Create(P_PILOT_NUM.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_PILOT_NUM.Item1, hDoc));
        //    P_GP_DESC = Tuple.Create(P_GP_DESC.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_GP_DESC.Item1, hDoc));
        //    P_L_HOLESEC = Tuple.Create(P_L_HOLESEC.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_HOLESEC.Item1, hDoc));
        //    P_FLOW_RATE = Tuple.Create(P_FLOW_RATE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_FLOW_RATE.Item1, hDoc));
        //    P_FLOW_RATE_TO = Tuple.Create(P_FLOW_RATE_TO.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_FLOW_RATE_TO.Item1, hDoc));
        //    P_FLOW_RATE_UNIT = Tuple.Create(P_FLOW_RATE_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_FLOW_RATE_UNIT.Item1, hDoc));
        //    P_MAX_HYD_PRES = Tuple.Create(P_MAX_HYD_PRES.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_MAX_HYD_PRES.Item1, hDoc));
        //    P_MAX_HYD_PRES_UNIT = Tuple.Create(P_MAX_HYD_PRES_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_MAX_HYD_PRES_UNIT.Item1, hDoc));
        //    P_PLAN_TEMP_START = Tuple.Create(P_PLAN_TEMP_START.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_PLAN_TEMP_START.Item1, hDoc));
        //    P_TEMP_UNIT = Tuple.Create(P_TEMP_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_TEMP_UNIT.Item1, hDoc));
        //    P_MAX_TEMP = Tuple.Create(P_MAX_TEMP.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_MAX_TEMP.Item1, hDoc));
        //    P_L_OILTYPE = Tuple.Create(P_L_OILTYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_OILTYPE.Item1, hDoc));
        //    P_L_GPSIZE = Tuple.Create(P_L_GPSIZE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_GPSIZE.Item1, hDoc));
        //    P_L_MRSC_SUB_CONF_DESCR2 = Tuple.Create(P_L_MRSC_SUB_CONF_DESCR2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_MRSC_SUB_CONF_DESCR2.Item1, hDoc));
        //    P_L_ABITYPE = Tuple.Create(P_L_ABITYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_ABITYPE.Item1, hDoc));
        //    P_FLEXJOINT_REQ = Tuple.Create(P_FLEXJOINT_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_FLEXJOINT_REQ.Item1, hDoc));
        //    P_FLEX_TORQUED = Tuple.Create(P_FLEX_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_FLEX_TORQUED.Item1, hDoc));
        //    P_GP_CLAMP_REQ = Tuple.Create(P_GP_CLAMP_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_GP_CLAMP_REQ.Item1, hDoc));
        //    P_DM_TYPE = Tuple.Create(P_DM_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_DM_TYPE.Item1, hDoc));
        //    P_L_SOFTWARE_DM = Tuple.Create(P_L_SOFTWARE_DM.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_SOFTWARE_DM.Item1, hDoc));
        //    P_L_SOFTWARE_GP = Tuple.Create(P_L_SOFTWARE_GP.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_SOFTWARE_GP.Item1, hDoc));
        //    P_L_THREAD_UP = Tuple.Create(P_L_THREAD_UP.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_THREAD_UP.Item1, hDoc));
        //    P_L_THREAD_LOW = Tuple.Create(P_L_THREAD_LOW.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_THREAD_LOW.Item1, hDoc));
        //    P_BIT_TYPE = Tuple.Create(P_BIT_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_BIT_TYPE.Item1, hDoc));
        //    P_BIT_TORQUED = Tuple.Create(P_BIT_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_BIT_TORQUED.Item1, hDoc));
        //    P_BIT_MAKEUP_TORQUE = Tuple.Create(P_BIT_MAKEUP_TORQUE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_BIT_MAKEUP_TORQUE.Item1, hDoc));
        //    P_BIT_SN = Tuple.Create(P_BIT_SN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_BIT_SN.Item1, hDoc));
        //    P_LITHIUM_USED = Tuple.Create(P_LITHIUM_USED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_LITHIUM_USED.Item1, hDoc));
        //    P_GP_COMMENT = Tuple.Create(P_GP_COMMENT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_GP_COMMENT.Item1, hDoc));
        //    P_L_LOWHOUSEDESC = Tuple.Create(P_L_LOWHOUSEDESC.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_L_LOWHOUSEDESC.Item1, hDoc));
        //    P_S2 = Tuple.Create(P_S2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(P_S2.Item1, hDoc));
        //    O_PILOT_NUM = Tuple.Create(O_PILOT_NUM.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_PILOT_NUM.Item1, hDoc));
        //    O_GP_DESC = Tuple.Create(O_GP_DESC.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_DESC.Item1, hDoc));
        //    O_FLOW_RATE = Tuple.Create(O_FLOW_RATE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLOW_RATE.Item1, hDoc));
        //    O_FLOW_RATE_TO = Tuple.Create(O_FLOW_RATE_TO.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLOW_RATE_TO.Item1, hDoc));
        //    O_FLOW_RATE_UNIT = Tuple.Create(O_FLOW_RATE_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLOW_RATE_UNIT.Item1, hDoc));
        //    O_MAX_HYD_PRES = Tuple.Create(O_MAX_HYD_PRES.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_MAX_HYD_PRES.Item1, hDoc));
        //    O_MAX_HYD_PRES_UNIT = Tuple.Create(O_MAX_HYD_PRES_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_MAX_HYD_PRES_UNIT.Item1, hDoc));
        //    O_PLAN_TEMP_START = Tuple.Create(O_PLAN_TEMP_START.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_PLAN_TEMP_START.Item1, hDoc));
        //    O_TEMP_UNIT = Tuple.Create(O_TEMP_UNIT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_TEMP_UNIT.Item1, hDoc));
        //    O_MAX_TEMP = Tuple.Create(O_MAX_TEMP.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_MAX_TEMP.Item1, hDoc));
        //    O_ROLLER_STYLE = Tuple.Create(O_ROLLER_STYLE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_ROLLER_STYLE.Item1, hDoc));
        //    O_FLEXJOINT_REQ = Tuple.Create(O_FLEXJOINT_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLEXJOINT_REQ.Item1, hDoc));
        //    O_FLEX_TORQUED = Tuple.Create(O_FLEX_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_FLEX_TORQUED.Item1, hDoc));
        //    O_GP_CLAMP_REQ = Tuple.Create(O_GP_CLAMP_REQ.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_CLAMP_REQ.Item1, hDoc));
        //    O_DM_TYPE = Tuple.Create(O_DM_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_DM_TYPE.Item1, hDoc));
        //    O_BIT_TYPE = Tuple.Create(O_BIT_TYPE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_TYPE.Item1, hDoc));
        //    O_BIT_TORQUED = Tuple.Create(O_BIT_TORQUED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_TORQUED.Item1, hDoc));
        //    O_BIT_MAKEUP_TORQUE = Tuple.Create(O_BIT_MAKEUP_TORQUE.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_MAKEUP_TORQUE.Item1, hDoc));
        //    O_BIT_SN = Tuple.Create(O_BIT_SN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_BIT_SN.Item1, hDoc));
        //    O_LITHIUM_USED = Tuple.Create(O_LITHIUM_USED.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_LITHIUM_USED.Item1, hDoc));
        //    O_ABITYPE_ID = Tuple.Create(O_ABITYPE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_ABITYPE_ID.Item1, hDoc));
        //    O_CONN_LOWER_ID = Tuple.Create(O_CONN_LOWER_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_CONN_LOWER_ID.Item1, hDoc));
        //    O_CONN_UPHOLE_ID = Tuple.Create(O_CONN_UPHOLE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_CONN_UPHOLE_ID.Item1, hDoc));
        //    O_GPSIZE_ID = Tuple.Create(O_GPSIZE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GPSIZE_ID.Item1, hDoc));
        //    O_HOLESEC_ID = Tuple.Create(O_HOLESEC_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_HOLESEC_ID.Item1, hDoc));
        //    O_OILTYPE_ID = Tuple.Create(O_OILTYPE_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_OILTYPE_ID.Item1, hDoc));
        //    O_ORDER_ID = Tuple.Create(O_ORDER_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_ORDER_ID.Item1, hDoc));
        //    O_SW_DM_ID = Tuple.Create(O_SW_DM_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_SW_DM_ID.Item1, hDoc));
        //    O_SW_GP_ID = Tuple.Create(O_SW_GP_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_SW_GP_ID.Item1, hDoc));
        //    O_GP_COMMENT = Tuple.Create(O_GP_COMMENT.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_GP_COMMENT.Item1, hDoc));
        //    O_LWR_SLICK_HOUS = Tuple.Create(O_LWR_SLICK_HOUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_LWR_SLICK_HOUS.Item1, hDoc));
        //    O_STATUS = Tuple.Create(O_STATUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_STATUS.Item1, hDoc));
        //    O_SUB_CONF_ID = Tuple.Create(O_SUB_CONF_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_SUB_CONF_ID.Item1, hDoc));
        //    O_LOWHOUSEID = Tuple.Create(O_LOWHOUSEID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_LOWHOUSEID.Item1, hDoc));
        //    O_S2 = Tuple.Create(O_S2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_S2.Item1, hDoc));
        //    O_PRECON_ID = Tuple.Create(O_PRECON_ID.Item1, HDocUtility.GetOneFromListValueOfInputsByName(O_PRECON_ID.Item1, hDoc));
        //    H_GP_COMPL_WARN = Tuple.Create(H_GP_COMPL_WARN.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_GP_COMPL_WARN.Item1, hDoc));
        //    H_DIV0 = Tuple.Create(H_DIV0.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV0.Item1, hDoc));
        //    H_DIV1 = Tuple.Create(H_DIV1.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV1.Item1, hDoc));
        //    H_DIV30 = Tuple.Create(H_DIV30.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV30.Item1, hDoc));
        //    H_LBL_APPDET = Tuple.Create(H_LBL_APPDET.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_LBL_APPDET.Item1, hDoc));
        //    H_DIV5 = Tuple.Create(H_DIV5.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV5.Item1, hDoc));
        //    H_DIV2 = Tuple.Create(H_DIV2.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV2.Item1, hDoc));
        //    H_DIV25 = Tuple.Create(H_DIV25.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV25.Item1, hDoc));
        //    H_DIV3 = Tuple.Create(H_DIV3.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV3.Item1, hDoc));
        //    H_DIV4 = Tuple.Create(H_DIV4.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV4.Item1, hDoc));
        //    H_LBL_CONFDET = Tuple.Create(H_LBL_CONFDET.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_LBL_CONFDET.Item1, hDoc));
        //    H_DIV6 = Tuple.Create(H_DIV6.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV6.Item1, hDoc));
        //    H_DIV12 = Tuple.Create(H_DIV12.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV12.Item1, hDoc));
        //    H_DIV11 = Tuple.Create(H_DIV11.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV11.Item1, hDoc));
        //    H_DIV13 = Tuple.Create(H_DIV13.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV13.Item1, hDoc));
        //    H_DIV19 = Tuple.Create(H_DIV19.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV19.Item1, hDoc));
        //    H_DIV26 = Tuple.Create(H_DIV26.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV26.Item1, hDoc));
        //    H_DIV14 = Tuple.Create(H_DIV14.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV14.Item1, hDoc));
        //    H_DIV21 = Tuple.Create(H_DIV21.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV21.Item1, hDoc));
        //    H_DIV15 = Tuple.Create(H_DIV15.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV15.Item1, hDoc));
        //    H_DIV17 = Tuple.Create(H_DIV17.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV17.Item1, hDoc));
        //    H_DIV24 = Tuple.Create(H_DIV24.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV24.Item1, hDoc));
        //    H_DIV23 = Tuple.Create(H_DIV23.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV23.Item1, hDoc));
        //    H_DIV18 = Tuple.Create(H_DIV18.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV18.Item1, hDoc));
        //    H_DIV16 = Tuple.Create(H_DIV16.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV16.Item1, hDoc));
        //    H_DIV99 = Tuple.Create(H_DIV99.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV99.Item1, hDoc));
        //    H_DIV20 = Tuple.Create(H_DIV20.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV20.Item1, hDoc));
        //    H_DIV22 = Tuple.Create(H_DIV22.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DIV22.Item1, hDoc));
        //    H_DEL_BHA = Tuple.Create(H_DEL_BHA.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_DEL_BHA.Item1, hDoc));
        //    H_L_PRECON_STATUS = Tuple.Create(H_L_PRECON_STATUS.Item1, HDocUtility.GetOneFromListValueOfInputsByName(H_L_PRECON_STATUS.Item1, hDoc));

        //    Z_CHK = Tuple.Create(Z_CHK.Item1, HDocUtility.GetOneFromListValueOfInputsByName(Z_CHK.Item1, hDoc));
        //}
    }

    public class GpCompPosts
    {
        public string P_SEQ_NO { get; set; } = "";
        public string P_L_MWDTORQUE_TORQUE { get; set; } = "";
        public string P_L_THREADTOP_THREADSIZE { get; set; } = "";
        public string P_L_THREADBTM_THREADSIZE { get; set; } = "";
        public string P_DESCRIPTION { get; set; } = "";
        public string P_COMMENTS { get; set; } = "";

    }

    public class LooseBhaPosts
    {
        public string P_QTY { get; set; } = "";
        public string P_L_MOBSS_VEND_VEND_NAME2 { get; set; } = "";
        public string P_DESCRIPTION { get; set; } = "";
        public string P_CUST_STAT { get; set; } = "";
        public string P_COMMENTS { get; set; } = "";
        public string P_L_THREADSIZE_TOP { get; set; } = "";
        public string P_L_THREADSIZE_BTM { get; set; } = "";

    }
}
