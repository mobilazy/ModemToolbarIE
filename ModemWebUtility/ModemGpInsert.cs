using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModemWebUtility
{
    public class ModemGpInsert
    {
        private List<string> existingGpId = new List<string>();
        private List<string> newGpId = new List<string>();
        private ModemParameters mp;// = new ModemParameters();
        private ModemGpPostObjects mObj = new ModemGpPostObjects();

        public ModemGpInsert(ModemParameters _mp, ModemGpPostObjects _mObj, bool InstertComonentOnly)
        {
            mp = _mp;
            mObj = _mObj;

            if (!InstertComonentOnly)
            {
                AddGpBha();
            }

            UpdateGpComponent(mp.GpId.Last());

        }

        private void AddGpBha()
        {
            string urlGpInsert = HDocUtility.UrlGpInsert;
            string urlGpBhaInsert = HDocUtility.UrlGpBhaInsert;
            string urlGpBhaEdit = HDocUtility.UrlGpBhaEdit;

            ModemConnection mc = new ModemConnection(urlGpInsert + mp.ModemNo);

            string headerResponse = mc.AddGpAndGetResponse();
           
            string pattern = @"QueryViewByKey\?P_GP_ID=([0-9]+)";
            string input = headerResponse;
            
            Regex rx = new Regex(pattern);
            Match match = rx.Match(input);
            GroupCollection gr = match.Groups;
            string newGpId = gr[1].Value; // headerResponse.Substring(47, 5);


            ModemConnection mc2 = new ModemConnection(urlGpBhaEdit+newGpId);
            GpBhaParameters newGp = new GpBhaParameters(mc2.GetHtmlAsHdoc());
            

            mObj.GpBhaPost.P_10 = Tuple.Create<string, string>("P_10", ""); //mp.ModemNo
            mObj.GpBhaPost.P_GP_ID = newGp.GpBhaPosts.P_GP_ID;
            mObj.GpBhaPost.O_GP_ID = newGp.GpBhaPosts.O_GP_ID;
            mObj.GpBhaPost.H_DEL_BHA = newGp.GpBhaPosts.H_DEL_BHA;
            mObj.GpBhaPost.Z_CHK = newGp.GpBhaPosts.Z_CHK;

           
            existingGpId = mp.GpId;
            int lastItem = 0;

            if (existingGpId.Count > 0)
            {
                lastItem = Int32.Parse(mp.GetElementIdInnerText("PILOT_NUM" + existingGpId.Count.ToString()));
            }

            mObj.GpBhaPost.O_PILOT_NUM = Tuple.Create<string, string>("O_PILOT_NUM", (lastItem+1).ToString());
            mObj.GpBhaPost.P_PILOT_NUM = Tuple.Create<string, string>("P_PILOT_NUM", (lastItem+1).ToString());


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
