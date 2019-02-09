using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemWebUtility
{
    public class ModemLooseInsert
    {

        private ModemParameters mp;// = new ModemParameters();
        private ModemLoosePostObjects mObj = new ModemLoosePostObjects();

        public ModemLooseInsert(ModemParameters _mp, ModemLoosePostObjects _mObj, bool InstertComonentOnly)
        {
            mp = _mp;
            mObj = _mObj;

            UpdateLooseComponent();

        }


        private void UpdateLooseComponent()
        {
            {
                string urlLooseItemInsert = HDocUtility.UrlLooseItemInsert;

                int inputRow = 10;

                double dCompSize = mObj.LoosePostDict.Count;
                double dLoop = Math.Ceiling(dCompSize / inputRow);

                int bhaSize = Convert.ToInt32(dCompSize);
                int loop = Convert.ToInt32(dLoop);


                int bhaCounter = 0;
                for (int i = 0; i < loop; i++)
                {
                    ModemDataPost mdp = new ModemDataPost(urlLooseItemInsert);

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
                            mdp.AddPostKeys("P_QTY", mObj.LoosePostDict[bhaCounter].P_QTY);
                            mdp.AddPostKeys("P_L_MOBSS_VEND_VEND_NAME2", mObj.LoosePostDict[bhaCounter].P_L_MOBSS_VEND_VEND_NAME2);
                            mdp.AddPostKeys("P_DESCRIPTION", mObj.LoosePostDict[bhaCounter].P_DESCRIPTION);
                            mdp.AddPostKeys("P_CUST_STAT", mObj.LoosePostDict[bhaCounter].P_CUST_STAT);
                            mdp.AddPostKeys("P_COMMENTS", mObj.LoosePostDict[bhaCounter].P_COMMENTS);
                            mdp.AddPostKeys("P_L_THREADSIZE_TOP", String.IsNullOrEmpty(mObj.LoosePostDict[bhaCounter].P_L_THREADSIZE_TOP)? " " : mObj.LoosePostDict[bhaCounter].P_L_THREADSIZE_TOP );
                            mdp.AddPostKeys("P_L_THREADSIZE_BTM", string.IsNullOrEmpty(mObj.LoosePostDict[bhaCounter].P_L_THREADSIZE_BTM) ? " ": mObj.LoosePostDict[bhaCounter].P_L_THREADSIZE_BTM);
                            mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo);

                        }
                        else
                        {
                            mdp.AddPostKeys("z_modified", "N");
                            mdp.AddPostKeys("P_QTY", "");
                            mdp.AddPostKeys("P_L_MOBSS_VEND_VEND_NAME2", "GXT");
                            mdp.AddPostKeys("P_DESCRIPTION", "");
                            mdp.AddPostKeys("P_CUST_STAT", "FCG");
                            mdp.AddPostKeys("P_COMMENTS", "");
                            mdp.AddPostKeys("P_L_THREADSIZE_TOP", " ");
                            mdp.AddPostKeys("P_L_THREADSIZE_BTM", " ");
                            mdp.AddPostKeys("P_SSORD_ID", mp.ModemNo);


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
