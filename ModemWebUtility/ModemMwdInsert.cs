using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class ModemMwdInsert
    {
        private List<string> existingMwdId = new List<string>();
        private List<string> newMwdId = new List<string>();
        private ModemParameters mp;// = new ModemParameters();
        private ModemMwdPostObjects mObj = new ModemMwdPostObjects();

        public ModemMwdInsert(ModemParameters _mp, ModemMwdPostObjects _mObj, bool InstertComonentOnly)
        {
            mp = _mp;
            mObj = _mObj;

            if (!InstertComonentOnly)
            {
                AddMwdBha();
            }
            
            UpdateMwdComponent(mp.MwdId.LastOrDefault());
            UpdateMwdSoftware(mp.MwdId.LastOrDefault());
        }

        private void AddMwdBha()
        {
            string urlMwdInsert = HDocUtility.UrlMwdInsert;
            
            existingMwdId = mp.MwdId;
            int lastItem = 0;

            if (existingMwdId.Count>0)
            {
               lastItem = Int32.Parse(mp.GetElementIdInnerText("BHA_NUM"+existingMwdId.Count.ToString()));
            }


            int inputRow = 4;
            double dBhaSize = 1;
            double dLoop = Math.Ceiling(dBhaSize / inputRow);

            int bhaSize = Convert.ToInt32(dBhaSize);
            int loop = Convert.ToInt32(dLoop);

            int bhaCounter = 0;
            for (int i = 0; i < loop; i++)
            {
                ModemDataPost mdp = new ModemDataPost(urlMwdInsert);
                mdp.AddPostKeys("P_10", mp.ModemNo.ToString());
                mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo.ToString());
                
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
                        mdp.AddPostKeys("z_modified", "Y");
                        mdp.AddPostKeys("P_BHA_NUM", (lastItem+1).ToString());
                        mdp.AddPostKeys("P_L_ORIFFICESIZE", String.IsNullOrEmpty(mObj.MwdBhaPost.P_L_ORIFFICESIZE)? " " : mObj.MwdBhaPost.P_L_ORIFFICESIZE);
                        mdp.AddPostKeys("P_POPPET_STANDOFF", String.IsNullOrEmpty(mObj.MwdBhaPost.P_POPPET_STANDOFF) ? "": mObj.MwdBhaPost.P_POPPET_STANDOFF);
                        mdp.AddPostKeys("P_L_IMPELLERSIZE", String.IsNullOrEmpty(mObj.MwdBhaPost.P_L_IMPELLERSIZE) ? "  " : mObj.MwdBhaPost.P_L_IMPELLERSIZE);
                        mdp.AddPostKeys("P_EXP_MAX_TEMP", String.IsNullOrEmpty (mObj.MwdBhaPost.P_EXP_MAX_TEMP)?"": mObj.MwdBhaPost.P_EXP_MAX_TEMP);
                        mdp.AddPostKeys("P_L_STATORSIZE", String.IsNullOrEmpty(mObj.MwdBhaPost.P_L_STATORSIZE) ? "  " : mObj.MwdBhaPost.P_L_STATORSIZE);
                        mdp.AddPostKeys("P_BHA_DESC", String.IsNullOrEmpty(mObj.MwdBhaPost.P_BHA_DESC)?"": mObj.MwdBhaPost.P_BHA_DESC);
                        mdp.AddPostKeys("P_PULSER_OILTYPE", String.IsNullOrEmpty(mObj.MwdBhaPost.P_PULSER_OILTYPE)?"": mObj.MwdBhaPost.P_PULSER_OILTYPE);
                        mdp.AddPostKeys("P_MWDDWD_ADD_INFO", String.IsNullOrEmpty(mObj.MwdBhaPost.P_MWDDWD_ADD_INFO)?"": mObj.MwdBhaPost.P_MWDDWD_ADD_INFO);
                        mdp.AddPostKeys("P_HC_TOOL", String.IsNullOrEmpty(mObj.MwdBhaPost.P_HC_TOOL)?"": mObj.MwdBhaPost.P_HC_TOOL);
                        mdp.AddPostKeys("P_RASOURCE_ID", String.IsNullOrEmpty(mObj.MwdBhaPost.P_RASOURCE_ID)?"": mObj.MwdBhaPost.P_RASOURCE_ID);
                        mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo.ToString());
                    }
                    else
                    {
                        mdp.AddPostKeys("z_modified", "N");
                        mdp.AddPostKeys("P_BHA_NUM", "");
                        mdp.AddPostKeys("P_L_ORIFFICESIZE", " ");
                        mdp.AddPostKeys("P_POPPET_STANDOFF", "");
                        mdp.AddPostKeys("P_L_IMPELLERSIZE", " ");
                        mdp.AddPostKeys("P_EXP_MAX_TEMP", "");
                        mdp.AddPostKeys("P_L_STATORSIZE", " ");
                        mdp.AddPostKeys("P_BHA_DESC", "");
                        mdp.AddPostKeys("P_PULSER_OILTYPE", "");
                        mdp.AddPostKeys("P_MWDDWD_ADD_INFO", "");
                        mdp.AddPostKeys("P_HC_TOOL", "0");
                        mdp.AddPostKeys("P_RASOURCE_ID", "");
                        mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo.ToString());
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

        private void InsertMwdBhaParts(string mwdId)
        {
            UpdateMwdComponent(mwdId);
            UpdateMwdSoftware(mwdId);
        }

        private void UpdateMwdComponent(string mwdId)
        {
            string urlMwdComponentInsert = HDocUtility.UrlMwdComponentInsert;

            int inputRow = 10;
            existingMwdId = mp.MwdId;
            double dCompSize = mObj.MwdCompPostDict.Count;
            double dLoop = Math.Ceiling(dCompSize / inputRow);

            int bhaSize = Convert.ToInt32(dCompSize);
            int loop = Convert.ToInt32(dLoop);


            int bhaCounter = 0;
            for (int i = 0; i < loop; i++)
            {
                ModemDataPost mdp = new ModemDataPost(urlMwdComponentInsert);
                mdp.AddPostKeys("P_104", mwdId);
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
                        mdp.AddPostKeys("P_MWDDWD_ID", mwdId);
                        mdp.AddPostKeys("z_modified", "Y");
                        mdp.AddPostKeys("P_SEQ_NO", mObj.MwdCompPostDict[bhaCounter].P_SEQ_NO);
                        mdp.AddPostKeys("P_L_TORQUE", mObj.MwdCompPostDict[bhaCounter].P_L_TORQUE);
                        mdp.AddPostKeys("P_L_THREAD_TOP", String.IsNullOrEmpty(mObj.MwdCompPostDict[bhaCounter].P_L_THREAD_TOP) ? "  " : mObj.MwdCompPostDict[bhaCounter].P_L_THREAD_TOP);
                        mdp.AddPostKeys("P_L_THREAD_BTM", String.IsNullOrEmpty(mObj.MwdCompPostDict[bhaCounter].P_L_THREAD_BTM) ? "  " : mObj.MwdCompPostDict[bhaCounter].P_L_THREAD_BTM);
                        mdp.AddPostKeys("P_DESCRIPTION", mObj.MwdCompPostDict[bhaCounter].P_DESCRIPTION);
                        mdp.AddPostKeys("P_COMMENTS", mObj.MwdCompPostDict[bhaCounter].P_COMMENTS);

                    }
                    else
                    {
                        mdp.AddPostKeys("P_MWDDWD_ID", mwdId);
                        mdp.AddPostKeys("z_modified", "N");
                        mdp.AddPostKeys("P_SEQ_NO", "");
                        mdp.AddPostKeys("P_L_TORQUE", "");
                        mdp.AddPostKeys("P_L_THREAD_TOP", " ");
                        mdp.AddPostKeys("P_L_THREAD_BTM", " ");
                        mdp.AddPostKeys("P_DESCRIPTION", "");
                        mdp.AddPostKeys("P_COMMENTS", "");

                    }
                    bhaCounter++;
                    j++;
                }



                mdp.AddPostKeys("P_MWDDWD_ID", "");
                mdp.AddPostKeys("z_modified", "dummy_row");
                mdp.AddPostKeys("P_SEQ_NO", "");
                mdp.AddPostKeys("P_L_TORQUE", "");
                mdp.AddPostKeys("P_L_THREAD_TOP", "");
                mdp.AddPostKeys("P_L_THREAD_BTM", "");
                mdp.AddPostKeys("P_DESCRIPTION", "");
                mdp.AddPostKeys("P_COMMENTS", "");
                mdp.AddPostKeys("Z_ACTION", "INSERT");
                mdp.AddPostKeys("Z_CHK", mp.Zchk.ToString());

                mdp.PostData();
            }
        }

        private void UpdateMwdSoftware(string mwdId)
        {
            string urlMwdSoftInsert = HDocUtility.UrlMwdSoftInsert;

            existingMwdId = mp.MwdId;

            int inputRow = 10;
            double dSoftSize = mObj.MwdSoftPostDict.Count;
            double dLoop = Math.Ceiling(dSoftSize / inputRow);

            int bhaSize = Convert.ToInt32(dSoftSize);
            int loop = Convert.ToInt32(dLoop);


            int bhaCounter = 0;
            for (int i = 0; i < loop; i++)
            {
                ModemDataPost mdp = new ModemDataPost(urlMwdSoftInsert);
                mdp.AddPostKeys("P_104", mwdId);
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
                        mdp.AddPostKeys("z_modified", "Y");
                        mdp.AddPostKeys("P_L_MSR_SENSOR", mObj.MwdSoftPostDict[bhaCounter].P_L_MSR_SENSOR);
                        mdp.AddPostKeys("P_OPS_VERSION", mObj.MwdSoftPostDict[bhaCounter].P_OPS_VERSION);
                        mdp.AddPostKeys("P_WS_VERSION", mObj.MwdSoftPostDict[bhaCounter].P_WS_VERSION);
                        mdp.AddPostKeys("P_MWDDWD_ID", mwdId);

                    }
                    else
                    {
                        mdp.AddPostKeys("z_modified", "N");
                        mdp.AddPostKeys("P_L_MSR_SENSOR", "ABI Rcvr");
                        mdp.AddPostKeys("P_OPS_VERSION", "");
                        mdp.AddPostKeys("P_WS_VERSION", "");
                        mdp.AddPostKeys("P_MWDDWD_ID", mwdId);
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
