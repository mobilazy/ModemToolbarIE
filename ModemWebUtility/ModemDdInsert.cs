using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemWebUtility
{
    public class ModemDdInsert
    {
        private List<string> existingDdId = new List<string>();
        private List<string> newDdId = new List<string>();
        private ModemParameters mp;// = new ModemParameters();
        private ModemDdPostObjects  mObj = new ModemDdPostObjects();

        public ModemDdInsert(ModemParameters _mp, ModemDdPostObjects _mObj, bool InstertComonentOnly)
        {
            mp = _mp;
            mObj = _mObj;

            if (!InstertComonentOnly)
            {
                AddDdBha();
            }

            UpdateDdComponent(mp.DdId.Last());
            
        }

        private void AddDdBha()
        {
            string urlDdInsert = HDocUtility.UrlDdBhaInsert;

            existingDdId = mp.DdId;
            int lastItem = 0;

            if (existingDdId.Count > 0)
            {
                lastItem = Int32.Parse(mp.GetElementIdInnerText("MOTOR_NUM" + existingDdId.Count.ToString()));
            }


            int inputRow = 5;
            double dBhaSize = 1;
            double dLoop = Math.Ceiling(dBhaSize / inputRow);

            int bhaSize = Convert.ToInt32(dBhaSize);
            int loop = Convert.ToInt32(dLoop);

            int bhaCounter = 0;
            for (int i = 0; i < loop; i++)
            {
                ModemDataPost mdp = new ModemDataPost(urlDdInsert);
                mdp.AddPostKeys("P_10", mp.ModemNo.ToString());
                

                int j = 0;
                int limit = 0;

                if ((i + 1) * inputRow < bhaSize)
                {
                    limit = inputRow;
                }
                else
                {
                    limit = bhaSize - (i) * inputRow;
                }

                while (j < inputRow)
                {
                    if (j < limit)
                    {
                        mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo.ToString());
                        mdp.AddPostKeys("P_MOTOR_NUM", (lastItem+1).ToString());
                        mdp.AddPostKeys("P_MOTOR_DESC", mObj.DdBhaPast.P_MOTOR_DESC);
                        mdp.AddPostKeys("P_L_MOBSS_HOLESEC", mObj.DdBhaPast.P_L_MOBSS_HOLESEC);
                        mdp.AddPostKeys("P_FLOWRATE_MIN", mObj.DdBhaPast.P_FLOWRATE_MIN);
                        mdp.AddPostKeys("P_FLOWRATE_MAX", mObj.DdBhaPast.P_FLOWRATE_MAX);
                        mdp.AddPostKeys("P_FLOW_RATE_UNIT", mObj.DdBhaPast.P_FLOW_RATE_UNIT);
                        mdp.AddPostKeys("P_WOB", mObj.DdBhaPast.P_WOB);
                        mdp.AddPostKeys("P_BIT_PRESSURE_DROP", mObj.DdBhaPast.P_BIT_PRESSURE_DROP);
                        mdp.AddPostKeys("P_PRES_UNIT", mObj.DdBhaPast.P_PRES_UNIT);
                        mdp.AddPostKeys("P_MOTOR_DIFFPRESS", mObj.DdBhaPast.P_MOTOR_DIFFPRESS);
                        mdp.AddPostKeys("P_MUD_TYPE", mObj.DdBhaPast.P_MUD_TYPE);
                        mdp.AddPostKeys("P_STATOR_RUBBER", mObj.DdBhaPast.P_STATOR_RUBBER);
                        mdp.AddPostKeys("P_PLAN_TEMP_START", mObj.DdBhaPast.P_PLAN_TEMP_START);
                        mdp.AddPostKeys("P_MAX_TEMP", mObj.DdBhaPast.P_MAX_TEMP);
                        mdp.AddPostKeys("P_TEMP_UNIT", mObj.DdBhaPast.P_TEMP_UNIT);
                        mdp.AddPostKeys("P_SLICK_BORE", mObj.DdBhaPast.P_SLICK_BORE);
                        mdp.AddPostKeys("P_L_MOBSS_MOTORSIZE", mObj.DdBhaPast.P_L_MOBSS_MOTORSIZE);
                        mdp.AddPostKeys("P_L_MOBSS_LOBERATIO", mObj.DdBhaPast.P_L_MOBSS_LOBERATIO);
                        mdp.AddPostKeys("P_L_MOTORTYPE", mObj.DdBhaPast.P_L_MOTORTYPE);
                        mdp.AddPostKeys("P_L_MOTOR_CATEGORY", mObj.DdBhaPast.P_L_MOTOR_CATEGORY);
                        mdp.AddPostKeys("P_FLEXI_STATOR", mObj.DdBhaPast.P_FLEXI_STATOR);
                        mdp.AddPostKeys("P_ROTOR_CATCHERS", mObj.DdBhaPast.P_ROTOR_CATCHERS);
                        mdp.AddPostKeys("P_L_ROTOR_COATING", mObj.DdBhaPast.P_L_ROTOR_COATING);
                        mdp.AddPostKeys("P_L_BEND", mObj.DdBhaPast.P_L_BEND);
                        mdp.AddPostKeys("P_ROTOR_NOZZLE", mObj.DdBhaPast.P_ROTOR_NOZZLE);
                        mdp.AddPostKeys("P_DUMP_SUB", mObj.DdBhaPast.P_DUMP_SUB);
                        mdp.AddPostKeys("P_PIN_DOWN_SHAFT", mObj.DdBhaPast.P_PIN_DOWN_SHAFT);
                        mdp.AddPostKeys("P_L_UPHOLE", mObj.DdBhaPast.P_L_UPHOLE);
                        mdp.AddPostKeys("P_L_DRIVESHAFT", mObj.DdBhaPast.P_L_DRIVESHAFT);
                        mdp.AddPostKeys("P_SLEEVE_FITTED", mObj.DdBhaPast.P_SLEEVE_FITTED);
                        mdp.AddPostKeys("P_SLEEVE_GAUGE", mObj.DdBhaPast.P_SLEEVE_GAUGE);
                        mdp.AddPostKeys("P_PAD", mObj.DdBhaPast.P_PAD);
                        mdp.AddPostKeys("P_PAD_SIZE", mObj.DdBhaPast.P_PAD_SIZE);
                        mdp.AddPostKeys("P_ABI_REQ", mObj.DdBhaPast.P_ABI_REQ);
                        mdp.AddPostKeys("P_BIT_TYPE", mObj.DdBhaPast.P_BIT_TYPE);
                        mdp.AddPostKeys("P_BIT_TORQUED", mObj.DdBhaPast.P_BIT_TORQUED);
                        mdp.AddPostKeys("z_modified", "Y");
                        mdp.AddPostKeys("P_BIT_MAKEUP_TORQUE", mObj.DdBhaPast.P_BIT_MAKEUP_TORQUE);
                        mdp.AddPostKeys("P_BIT_SN", mObj.DdBhaPast.P_BIT_SN);
                        mdp.AddPostKeys("P_COMMENTS", mObj.DdBhaPast.P_COMMENTS);

                    }
                    else
                    {
                        mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo.ToString());
                        mdp.AddPostKeys("P_MOTOR_NUM", "");
                        mdp.AddPostKeys("P_MOTOR_DESC", "");
                        mdp.AddPostKeys("P_L_MOBSS_HOLESEC", " ");
                        mdp.AddPostKeys("P_FLOWRATE_MIN", "");
                        mdp.AddPostKeys("P_FLOWRATE_MAX", "");
                        mdp.AddPostKeys("P_FLOW_RATE_UNIT", " ");
                        mdp.AddPostKeys("P_WOB", "");
                        mdp.AddPostKeys("P_BIT_PRESSURE_DROP", "");
                        mdp.AddPostKeys("P_PRES_UNIT", " ");
                        mdp.AddPostKeys("P_MOTOR_DIFFPRESS", "");
                        mdp.AddPostKeys("P_MUD_TYPE", "");
                        mdp.AddPostKeys("P_STATOR_RUBBER", "");
                        mdp.AddPostKeys("P_PLAN_TEMP_START", "");
                        mdp.AddPostKeys("P_MAX_TEMP", "");
                        mdp.AddPostKeys("P_TEMP_UNIT", " ");
                        mdp.AddPostKeys("P_SLICK_BORE", " ");
                        mdp.AddPostKeys("P_L_MOBSS_MOTORSIZE", " ");
                        mdp.AddPostKeys("P_L_MOBSS_LOBERATIO", " ");
                        mdp.AddPostKeys("P_L_MOTORTYPE", " ");
                        mdp.AddPostKeys("P_L_MOTOR_CATEGORY", " ");
                        mdp.AddPostKeys("P_FLEXI_STATOR", " ");
                        mdp.AddPostKeys("P_ROTOR_CATCHERS", " ");
                        mdp.AddPostKeys("P_L_ROTOR_COATING", " ");
                        mdp.AddPostKeys("P_L_BEND", " ");
                        mdp.AddPostKeys("P_ROTOR_NOZZLE", " ");
                        mdp.AddPostKeys("P_DUMP_SUB", " ");
                        mdp.AddPostKeys("P_PIN_DOWN_SHAFT", " ");
                        mdp.AddPostKeys("P_L_UPHOLE", " ");
                        mdp.AddPostKeys("P_L_DRIVESHAFT", " ");
                        mdp.AddPostKeys("P_SLEEVE_FITTED", " ");
                        mdp.AddPostKeys("P_SLEEVE_GAUGE", "");
                        mdp.AddPostKeys("P_PAD", " ");
                        mdp.AddPostKeys("P_PAD_SIZE", "");
                        mdp.AddPostKeys("P_ABI_REQ", " ");
                        mdp.AddPostKeys("P_BIT_TYPE", "");
                        mdp.AddPostKeys("P_BIT_TORQUED", " ");
                        mdp.AddPostKeys("z_modified", "N");
                        mdp.AddPostKeys("P_BIT_MAKEUP_TORQUE", "");
                        mdp.AddPostKeys("P_BIT_SN", "");
                        mdp.AddPostKeys("P_COMMENTS", "");
                    }
                    bhaCounter++;
                    j++;
                }



                mdp.AddPostKeys("z_modified", "dummy_row");
                mdp.AddPostKeys("Z_ACTION", "INSERT");
                mdp.AddPostKeys("Z_CHK", mp.Zchk.ToString());

                mdp.PostData();

                mp = new ModemParameters(new ModemConnection(HDocUtility.UrlModemView + mp.ModemNo).GetHtmlAsHdoc(), mp.ModemNo);
            }
        }

        private void InsertMwdBhaParts(string id)
        {
            UpdateDdComponent(id);
            
        }

        private void UpdateDdComponent(string ddId)
        {
            {
                string urlDdCompInsert = HDocUtility.UrlDdCompInsert;

                int inputRow = 10;
                existingDdId = mp.DdId;
                double dCompSize = mObj.DdCompPostDict.Count;
                double dLoop = Math.Ceiling(dCompSize / inputRow);

                int bhaSize = Convert.ToInt32(dCompSize);
                int loop = Convert.ToInt32(dLoop);


                int bhaCounter = 0;
                for (int i = 0; i < loop; i++)
                {
                    ModemDataPost mdp = new ModemDataPost(urlDdCompInsert);
                    mdp.AddPostKeys("P_183", ddId);
                    mdp.AddPostKeys("P_10", mp.ModemNo.ToString());

                    int j = 0;
                    int limit = 0;

                    if ((i + 1) * inputRow < bhaSize)
                    {
                        limit = inputRow;
                    }
                    else
                    {
                        limit = bhaSize - (i) * inputRow;
                    }

                    while (j < inputRow)
                    {
                        if (j < limit)
                        {
                            mdp.AddPostKeys("P_DD_ID", ddId);
                            mdp.AddPostKeys("z_modified", "Y");
                            mdp.AddPostKeys("P_SEQ_NO", mObj.DdCompPostDict[bhaCounter].P_SEQ_NO);
                            mdp.AddPostKeys("P_L_MWDTORQUE_TORQUE", mObj.DdCompPostDict[bhaCounter].P_L_MWDTORQUE_TORQUE);
                            mdp.AddPostKeys("P_L_THREAD_TOP_THREADSIZE", String.IsNullOrEmpty( mObj.DdCompPostDict[bhaCounter].P_L_THREAD_TOP_THREADSIZE)?" " : mObj.DdCompPostDict[bhaCounter].P_L_THREAD_TOP_THREADSIZE);
                            mdp.AddPostKeys("P_L_THREAD_BTM_THREADSIZE", String.IsNullOrEmpty( mObj.DdCompPostDict[bhaCounter].P_L_THREAD_BTM_THREADSIZE)?" ": mObj.DdCompPostDict[bhaCounter].P_L_THREAD_BTM_THREADSIZE);
                            mdp.AddPostKeys("P_DESCRIPTION", mObj.DdCompPostDict[bhaCounter].P_DESCRIPTION);
                            mdp.AddPostKeys("P_COMMENTS", mObj.DdCompPostDict[bhaCounter].P_COMMENTS);



                        }
                        else
                        {
                            mdp.AddPostKeys("P_DD_ID", ddId);
                            mdp.AddPostKeys("z_modified", "N");
                            mdp.AddPostKeys("P_SEQ_NO", "");
                            mdp.AddPostKeys("P_L_MWDTORQUE_TORQUE", "");
                            mdp.AddPostKeys("P_L_THREAD_TOP_THREADSIZE", " ");
                            mdp.AddPostKeys("P_L_THREAD_BTM_THREADSIZE", " ");
                            mdp.AddPostKeys("P_DESCRIPTION", "");
                            mdp.AddPostKeys("P_COMMENTS", "");


                        }
                        bhaCounter++;
                        j++;
                    }

                    mdp.AddPostKeys("z_modified", "dummy_row");
                    mdp.AddPostKeys("Z_ACTION", "INSERT");
                    mdp.AddPostKeys("Z_CHK", mp.Zchk.ToString());

                    mdp.PostData();
                }
            }
        }

    }
}
