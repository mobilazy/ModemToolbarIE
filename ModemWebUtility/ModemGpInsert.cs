using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class ModemGpInsert
    {
        private List<string> existingGpId = new List<string>();
        private List<string> newGpId = new List<string>();
        private ModemParameters mp;
        private ModemGpPostObjects mObj;

        public ModemGpInsert(ModemParameters _mp, ModemGpPostObjects _mObj, bool InstertComonentOnly)
        {
            mp = _mp;
            mObj = _mObj;

            if (!InstertComonentOnly)
            {
                AddGpBha();
            }

            UpdateGpComponent(mp.GpId.Last());
            //MessageBox.Show("Test2");

        }

        private void AddGpBha()
        {
            string urlGpInsert = HDocUtility.UrlGpInsert;
            string urlGpBhaInsert = HDocUtility.UrlGpBhaInsert;
            string urlGpBhaEdit = HDocUtility.UrlGpBhaEdit;

            ModemConnection modem = new ModemConnection(HDocUtility.UrlModemView+mp.ModemNo);
            bool modemActivated = (HDocUtility.GetInputByName("H_STAT_ORDER", modem.GetHtmlAsHdoc()).Equals("2"));

            

            ModemConnection mc = new ModemConnection(urlGpInsert + mp.ModemNo);
            
            

            string headerResponse = mc.AddGpAndGetResponse();
           
            string pattern = @"QueryViewByKey\?P_GP_ID=([0-9]+)";
            string patternZchk = @"z_chk=([0-9]+)";
            string input = headerResponse;
            
            Regex rx = new Regex(pattern);
            Match match = rx.Match(input);
            GroupCollection gr = match.Groups;
            string newGpId = gr[1].Value; // headerResponse.Substring(47, 5);

            rx = new Regex(patternZchk);
            match = rx.Match(input);
            gr = match.Groups;
            string z_chk = gr[1].Value; // headerResponse.Substring(47, 5);


            ModemConnection mc2 = new ModemConnection(urlGpBhaEdit+newGpId);
            

            GpBhaParameters newGp = new GpBhaParameters(mc2.GetHtmlAsHdoc(), modemActivated);
            
            // CRITICAL: Update ALL O_ fields (List of Values IDs) from target modem to avoid invalid ID errors
            // These O_ IDs are modem-specific and must match the target modem's available options
            mObj.GpBhaPost.P_10 = Tuple.Create<string, string>("P_10", mp.ModemNo); // Target modem number
            mObj.GpBhaPost.P_GP_ID = newGp.GpBhaPosts.P_GP_ID; // New GP BHA ID
            mObj.GpBhaPost.O_GP_ID = newGp.GpBhaPosts.O_GP_ID; // Original GP BHA ID for update
            mObj.GpBhaPost.H_DEL_BHA = newGp.GpBhaPosts.H_DEL_BHA; // Delete button HTML
            mObj.GpBhaPost.Z_CHK = newGp.GpBhaPosts.Z_CHK; // Checksum
            mObj.GpBhaPost.H_GP_COMPL_WARN = newGp.GpBhaPosts.H_GP_COMPL_WARN; // Completion warning
            mObj.GpBhaPost.H_L_PRECON_STATUS = newGp.GpBhaPosts.H_L_PRECON_STATUS; // Preconfig status
            
            // Update ALL O_ fields with target modem's valid IDs
            mObj.GpBhaPost.O_PRECON_ID = newGp.GpBhaPosts.O_PRECON_ID;
            mObj.GpBhaPost.O_ABITYPE_ID = newGp.GpBhaPosts.O_ABITYPE_ID;
            mObj.GpBhaPost.O_CONN_LOWER_ID = newGp.GpBhaPosts.O_CONN_LOWER_ID;
            mObj.GpBhaPost.O_CONN_UPHOLE_ID = newGp.GpBhaPosts.O_CONN_UPHOLE_ID;
            mObj.GpBhaPost.O_GPSIZE_ID = newGp.GpBhaPosts.O_GPSIZE_ID;
           mObj.GpBhaPost.O_HOLESEC_ID = newGp.GpBhaPosts.O_HOLESEC_ID;
            mObj.GpBhaPost.O_OILTYPE_ID = newGp.GpBhaPosts.O_OILTYPE_ID;
            mObj.GpBhaPost.O_ORDER_ID = newGp.GpBhaPosts.O_ORDER_ID;
            mObj.GpBhaPost.O_SW_DM_ID = newGp.GpBhaPosts.O_SW_DM_ID;
            mObj.GpBhaPost.O_SW_GP_ID = newGp.GpBhaPosts.O_SW_GP_ID;
            mObj.GpBhaPost.O_LWR_SLICK_HOUS = newGp.GpBhaPosts.O_LWR_SLICK_HOUS;
            mObj.GpBhaPost.O_STATUS = newGp.GpBhaPosts.O_STATUS;
            mObj.GpBhaPost.O_SUB_CONF_ID = newGp.GpBhaPosts.O_SUB_CONF_ID;
            mObj.GpBhaPost.O_LOWHOUSEID = newGp.GpBhaPosts.O_LOWHOUSEID;
            
            // Update ALL O__o fields (original values for Oracle optimistic locking)
            mObj.GpBhaPost.O_PRECON_ID_o = newGp.GpBhaPosts.O_PRECON_ID_o;
            mObj.GpBhaPost.O_ABITYPE_ID_o = newGp.GpBhaPosts.O_ABITYPE_ID_o;
            mObj.GpBhaPost.O_CONN_LOWER_ID_o = newGp.GpBhaPosts.O_CONN_LOWER_ID_o;
            mObj.GpBhaPost.O_CONN_UPHOLE_ID_o = newGp.GpBhaPosts.O_CONN_UPHOLE_ID_o;
            mObj.GpBhaPost.O_GPSIZE_ID_o = newGp.GpBhaPosts.O_GPSIZE_ID_o;
            mObj.GpBhaPost.O_HOLESEC_ID_o = newGp.GpBhaPosts.O_HOLESEC_ID_o;
            mObj.GpBhaPost.O_OILTYPE_ID_o = newGp.GpBhaPosts.O_OILTYPE_ID_o;
            mObj.GpBhaPost.O_ORDER_ID_o = newGp.GpBhaPosts.O_ORDER_ID_o;
            mObj.GpBhaPost.O_SW_DM_ID_o = newGp.GpBhaPosts.O_SW_DM_ID_o;
            mObj.GpBhaPost.O_SW_GP_ID_o = newGp.GpBhaPosts.O_SW_GP_ID_o;
            mObj.GpBhaPost.O_LWR_SLICK_HOUS_o = newGp.GpBhaPosts.O_LWR_SLICK_HOUS_o;
            mObj.GpBhaPost.O_STATUS_o = newGp.GpBhaPosts.O_STATUS_o;
            mObj.GpBhaPost.O_SUB_CONF_ID_o = newGp.GpBhaPosts.O_SUB_CONF_ID_o;
            mObj.GpBhaPost.O_LOWHOUSEID_o = newGp.GpBhaPosts.O_LOWHOUSEID_o;

            existingGpId = mp.GpId;
            int lastItem = 0;

            if (existingGpId.Count > 0)
            {
                lastItem = Int32.Parse(mp.GetElementIdInnerText("PILOT_NUM" + existingGpId.Count.ToString()));
            }

            mObj.GpBhaPost.O_PILOT_NUM = Tuple.Create<string, string>("O_PILOT_NUM", (lastItem+1).ToString());
            mObj.GpBhaPost.P_PILOT_NUM = Tuple.Create<string, string>("P_PILOT_NUM", (lastItem+1).ToString());

            // Truncate text fields to prevent "character string buffer too small" Oracle error
            // Different modems may have different column size limits
            if (mObj.GpBhaPost.P_GP_DESC != null && mObj.GpBhaPost.P_GP_DESC.Item2 != null && mObj.GpBhaPost.P_GP_DESC.Item2.Length > 50)
            {
                mObj.GpBhaPost.P_GP_DESC = Tuple.Create("P_GP_DESC", mObj.GpBhaPost.P_GP_DESC.Item2.Substring(0, 50));
            }
            
            if (mObj.GpBhaPost.P_GP_COMMENT != null && mObj.GpBhaPost.P_GP_COMMENT.Item2 != null && mObj.GpBhaPost.P_GP_COMMENT.Item2.Length > 1000)
            {
                mObj.GpBhaPost.P_GP_COMMENT = Tuple.Create("P_GP_COMMENT", mObj.GpBhaPost.P_GP_COMMENT.Item2.Substring(0, 1000));
            }
            
            if (mObj.GpBhaPost.P_BIT_TYPE != null && mObj.GpBhaPost.P_BIT_TYPE.Item2 != null && mObj.GpBhaPost.P_BIT_TYPE.Item2.Length > 100)
            {
                mObj.GpBhaPost.P_BIT_TYPE = Tuple.Create("P_BIT_TYPE", mObj.GpBhaPost.P_BIT_TYPE.Item2.Substring(0, 100));
            }
            
            if (mObj.GpBhaPost.P_BIT_MAKEUP_TORQUE != null && mObj.GpBhaPost.P_BIT_MAKEUP_TORQUE.Item2 != null && mObj.GpBhaPost.P_BIT_MAKEUP_TORQUE.Item2.Length > 10)
            {
                mObj.GpBhaPost.P_BIT_MAKEUP_TORQUE = Tuple.Create("P_BIT_MAKEUP_TORQUE", mObj.GpBhaPost.P_BIT_MAKEUP_TORQUE.Item2.Substring(0, 10));
            }
            
            if (mObj.GpBhaPost.P_BIT_SN != null && mObj.GpBhaPost.P_BIT_SN.Item2 != null && mObj.GpBhaPost.P_BIT_SN.Item2.Length > 10)
            {
                mObj.GpBhaPost.P_BIT_SN = Tuple.Create("P_BIT_SN", mObj.GpBhaPost.P_BIT_SN.Item2.Substring(0, 10));
            }

            ModemDataPost mdp = new ModemDataPost(urlGpBhaInsert); // + ModemNumber

            foreach (var p in mObj.GpBhaPost.GetType()
                            .GetProperties(
                                    BindingFlags.Public
                                    | BindingFlags.Instance))
            {
                

                if (p.PropertyType == typeof(Tuple<string, string>))
                {
                    Tuple<string, string> temp = (Tuple<string, string>)p.GetValue(mObj.GpBhaPost, null);

                    mdp.AddPostKeys(temp.Item1, temp.Item2);
                }

            }

            mdp.PostData();

            Thread.Sleep(2000);
            mp = new ModemParameters(new ModemConnection(HDocUtility.UrlModemView + mp.ModemNo).GetHtmlAsHdoc(), mp.ModemNo);
            
        }

        private void InsertMwdBhaParts(string id)
        {
            UpdateGpComponent(id);

        }

        private void UpdateGpComponent(string gpId)
        {
            {
                string urlGpCompInsert = HDocUtility.UrlGpCompInsert;

                int inputRow = 10;
                existingGpId = mp.GpId;
                double dCompSize = mObj.GpCompPostDict.Count;
                double dLoop = Math.Ceiling(dCompSize / inputRow);

                int bhaSize = Convert.ToInt32(dCompSize);
                int loop = Convert.ToInt32(dLoop);


                int bhaCounter = 0;
                for (int i = 0; i < loop; i++)
                {
                    ModemDataPost mdp = new ModemDataPost(urlGpCompInsert);
                    mdp.AddPostKeys("P_269", gpId);
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
                            mdp.AddPostKeys("P_GP_ID", gpId);
                            mdp.AddPostKeys("z_modified", "Y");
                            mdp.AddPostKeys("P_SEQ_NO", mObj.GpCompPostDict[bhaCounter].P_SEQ_NO);
                            mdp.AddPostKeys("P_L_MWDTORQUE_TORQUE", mObj.GpCompPostDict[bhaCounter].P_L_MWDTORQUE_TORQUE);
                            mdp.AddPostKeys("P_L_THREADTOP_THREADSIZE", String.IsNullOrEmpty( mObj.GpCompPostDict[bhaCounter].P_L_THREADTOP_THREADSIZE)?" ": mObj.GpCompPostDict[bhaCounter].P_L_THREADTOP_THREADSIZE);
                            mdp.AddPostKeys("P_L_THREADBTM_THREADSIZE", String.IsNullOrEmpty( mObj.GpCompPostDict[bhaCounter].P_L_THREADBTM_THREADSIZE)?" " : mObj.GpCompPostDict[bhaCounter].P_L_THREADBTM_THREADSIZE);
                            mdp.AddPostKeys("P_DESCRIPTION", mObj.GpCompPostDict[bhaCounter].P_DESCRIPTION);
                            mdp.AddPostKeys("P_COMMENTS", mObj.GpCompPostDict[bhaCounter].P_COMMENTS);

                        }
                        else
                        {
                            mdp.AddPostKeys("P_GP_ID", gpId);
                            mdp.AddPostKeys("z_modified", "N");
                            mdp.AddPostKeys("P_SEQ_NO", "");
                            mdp.AddPostKeys("P_L_MWDTORQUE_TORQUE", "");
                            mdp.AddPostKeys("P_L_THREADTOP_THREADSIZE", " ");
                            mdp.AddPostKeys("P_L_THREADBTM_THREADSIZE", " ");
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
