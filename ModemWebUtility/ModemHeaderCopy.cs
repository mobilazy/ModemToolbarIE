using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ModemWebUtility
{
    public class ModemHeaderCopy
    {
        private string sourceModemNo;
        private string targetModemNo;
        private HtmlDocument sourceHDoc;
        private HtmlDocument targetHDoc;

        public ModemHeaderCopy(string sourceModemNo, string targetModemNo)
        {
            this.sourceModemNo = sourceModemNo;
            this.targetModemNo = targetModemNo;
        }

        public bool CopyHeaderFields()
        {
            try
            {
                // Load source modem edit page
                ModemConnection sourceConn = new ModemConnection(HDocUtility.UrlModemEdit + sourceModemNo);
                sourceHDoc = sourceConn.GetHtmlAsHdoc();

                // Load target modem edit page to get Z_CHK and other required fields
                ModemConnection targetConn = new ModemConnection(HDocUtility.UrlModemEdit + targetModemNo);
                targetHDoc = targetConn.GetHtmlAsHdoc();

                // Extract fields from source
                var headerFields = ExtractHeaderFields(sourceHDoc);

                // Update target with source values
                return UpdateTargetHeader(headerFields);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy header from modem {sourceModemNo} to {targetModemNo}: {ex.Message}", ex);
            }
        }

        private Dictionary<string, string> ExtractHeaderFields(HtmlDocument hDoc)
        {
            var fields = new Dictionary<string, string>();

            // Copy ALL fields from source modem (complete 1:1 copy)
            
            // Dropdowns (select fields)
            fields["P_L_PERNR_NAVN"] = HDocUtility.GetSelectedElementById("P_L_PERNR_NAVN", hDoc) ?? "";
            fields["P_L_DD_PERS_NAVN"] = HDocUtility.GetSelectedElementById("P_L_DD_PERS_NAVN", hDoc) ?? "";
            fields["P_L_MDW_PERS_NAVN"] = HDocUtility.GetSelectedElementById("P_L_MDW_PERS_NAVN", hDoc) ?? "";
            fields["P_L_SHIPFROM_LOC2"] = HDocUtility.GetSelectedElementById("P_L_SHIPFROM_LOC2", hDoc) ?? "";
            fields["P_L_SHIPAGENT"] = HDocUtility.GetSelectedElementById("P_L_SHIPAGENT", hDoc) ?? "";
            fields["P_L_MOB_RIG_RIGNAME"] = HDocUtility.GetSelectedElementById("P_L_MOB_RIG_RIGNAME", hDoc) ?? "";
            fields["P_L_PRECON_MEAS_DESC"] = HDocUtility.GetSelectedElementById("P_L_PRECON_MEAS_DESC", hDoc) ?? "";
            
            // Text inputs
            fields["P_L_SO_JOB"] = HDocUtility.GetInputById("P_L_SO_JOB", hDoc) ?? "";
            fields["P_L_SHIPTO"] = HDocUtility.GetInputById("P_L_SHIPTO", hDoc) ?? "";
            fields["P_L_SO_ITEM"] = HDocUtility.GetInputById("P_L_SO_ITEM", hDoc) ?? "";
            fields["P_SHIPTO_1"] = HDocUtility.GetInputById("P_SHIPTO_1", hDoc) ?? "";
            fields["P_SHIPFROM_1"] = HDocUtility.GetInputById("P_SHIPFROM_1", hDoc) ?? "";
            fields["P_DATE_LOAD"] = HDocUtility.GetInputById("P_DATE_LOAD", hDoc) ?? "";
            fields["P_SHIPTO_2"] = HDocUtility.GetInputById("P_SHIPTO_2", hDoc) ?? "";
            fields["P_SHIPFROM_2"] = HDocUtility.GetInputById("P_SHIPFROM_2", hDoc) ?? "";
            fields["P_LOADOUT_DATE"] = HDocUtility.GetInputById("P_LOADOUT_DATE", hDoc) ?? "";
            fields["P_SHIPTO_3"] = HDocUtility.GetInputById("P_SHIPTO_3", hDoc) ?? "";
            fields["P_SHIPFROM_3"] = HDocUtility.GetInputById("P_SHIPFROM_3", hDoc) ?? "";
            fields["P_DATE_ETA"] = HDocUtility.GetInputById("P_DATE_ETA", hDoc) ?? "";
            fields["P_SHIPTO_4"] = HDocUtility.GetInputById("P_SHIPTO_4", hDoc) ?? "";
            fields["P_SHIPFROM_4"] = HDocUtility.GetInputById("P_SHIPFROM_4", hDoc) ?? "";
            fields["P_MOB_DURATION"] = HDocUtility.GetInputById("P_MOB_DURATION", hDoc) ?? "";
            fields["P_SHIPTO_5"] = HDocUtility.GetInputById("P_SHIPTO_5", hDoc) ?? "";
            fields["P_SHIPFROM_5"] = HDocUtility.GetInputById("P_SHIPFROM_5", hDoc) ?? "";
            fields["P_COSTCENTER"] = HDocUtility.GetInputById("P_COSTCENTER", hDoc) ?? "";
            fields["P_MOB_GANT_TEXT"] = HDocUtility.GetInputById("P_MOB_GANT_TEXT", hDoc) ?? "";
            fields["P_REFNO"] = HDocUtility.GetInputById("P_REFNO", hDoc) ?? "";
            fields["P_PO_NO"] = HDocUtility.GetInputById("P_PO_NO", hDoc) ?? "";
            fields["P_DOC_NO"] = HDocUtility.GetInputById("P_DOC_NO", hDoc) ?? "";
            fields["P_WELL_NUMBER"] = HDocUtility.GetInputById("P_WELL_NUMBER", hDoc) ?? "";
            fields["P_WELL_SECTION"] = HDocUtility.GetInputById("P_WELL_SECTION", hDoc) ?? "";
            fields["P_CLIENT_REP"] = HDocUtility.GetInputById("P_CLIENT_REP", hDoc) ?? "";
            
            // Textareas
            fields["P_REASON_FOR_SHIPMENT"] = HDocUtility.GetInputById("P_REASON_FOR_SHIPMENT", hDoc) ?? "";
            fields["P_SHIP_INSTRUCT"] = HDocUtility.GetInputById("P_SHIP_INSTRUCT", hDoc) ?? "";
            
            // Checkbox
            var demobMailNode = hDoc.DocumentNode.SelectSingleNode("//input[@id='P_ORD_DEMOB_MAIL']");
            if (demobMailNode != null && demobMailNode.GetAttributeValue("checked", "") != "")
            {
                fields["P_ORD_DEMOB_MAIL"] = "1";
            }
            else
            {
                fields["P_ORD_DEMOB_MAIL"] = "0";
            }

            return fields;
        }

        private bool UpdateTargetHeader(Dictionary<string, string> headerFields)
        {
            // Collect all fields from target (OLD values)
            var oldFields = CollectAllTargetFields(targetHDoc);

            // Create NEW values - clone old and override with source
            var newFields = new Dictionary<string, string>(oldFields);
            foreach (var field in headerFields)
            {
                newFields[field.Key] = field.Value;
            }

            // Ensure critical fields are set
            newFields["P_SSORD_ID"] = targetModemNo;
            newFields["P_10"] = targetModemNo;

            // Create POST following exact Oracle Forms structure
            ModemDataPost mdp = new ModemDataPost(HDocUtility.UrlModemHeaderUpdate);

            // Block 1: P_ fields with NEW values
            foreach (var field in newFields.Where(f => f.Key.StartsWith("P_") || f.Key == "P_10").OrderBy(f => f.Key))
            {
                mdp.AddPostKeys(field.Key, field.Value);
            }

            // Block 2: P_ fields EMPTY
            foreach (var field in newFields.Where(f => f.Key.StartsWith("P_") || f.Key == "P_10").OrderBy(f => f.Key))
            {
                mdp.AddPostKeys(field.Key, "");
            }

            // Block 3: H_ fields with values
            foreach (var field in oldFields.Where(f => f.Key.StartsWith("H_")).OrderBy(f => f.Key))
            {
                mdp.AddPostKeys(field.Key, field.Value);
            }

            // Block 4: H_ fields EMPTY
            foreach (var field in oldFields.Where(f => f.Key.StartsWith("H_")).OrderBy(f => f.Key))
            {
                mdp.AddPostKeys(field.Key, "");
            }

            // Markers
            mdp.AddPostKeys("z_modified", "N");
            mdp.AddPostKeys("z_modified", "dummy_row");

            // Block 5: O_ fields with OLD values (from HTML form inputs, not derived from P_)
            foreach (var field in oldFields.Where(f => f.Key.StartsWith("O_")).OrderBy(f => f.Key))
            {
                mdp.AddPostKeys(field.Key, field.Value);
            }

            // Block 6: O_ fields EMPTY
            foreach (var field in oldFields.Where(f => f.Key.StartsWith("O_")).OrderBy(f => f.Key))
            {
                mdp.AddPostKeys(field.Key, "");
            }

            // Control fields
            mdp.AddPostKeys("Z_ACTION", "UPDATE");
            mdp.AddPostKeys("Z_CHK", oldFields.ContainsKey("Z_CHK") ? oldFields["Z_CHK"] : "");

            // V_ fields
            mdp.AddPostKeys("V_DATE_ETA", "");
            mdp.AddPostKeys("V_DATE_LOAD", "");
            mdp.AddPostKeys("V_L_SO_JOB", "");
            mdp.AddPostKeys("V_LOADOUT_DATE", "");

            // Q_ query fields (only specific ones, not all P_ fields)
            string[] queryFields = { "COSTCENTER", "DATE_ETA", "DATE_LOAD", "L_MOB_RIG_RIGNAME", 
                "L_PERNR_NAVN", "L_SO_CUSTOMER", "L_SO_JOB", "LOADOUT_DATE", "PO_NO", 
                "REASON_FOR_SHIPMENT", "SHIPTO_1", "SHIPTO_2", "SHIPTO_3", "SHIPTO_4", 
                "SHIPTO_5", "SSORD_ID", "WELL_NUMBER" };

            foreach (var qField in queryFields.OrderBy(f => f))
            {
                mdp.AddPostKeys("Q_" + qField, "");
            }

            mdp.AddPostKeys("Z_START", "1");

            // Post the update
            bool result = mdp.PostData();

            return result;
        }

        private Dictionary<string, string> CollectAllTargetFields(HtmlDocument hDoc)
        {
            var fields = new Dictionary<string, string>();

            // Get all input fields from target to preserve existing data
            var inputs = hDoc.DocumentNode.Descendants("input")
                .Where(n => n.GetAttributeValue("name", "") != ""
                         && n.GetAttributeValue("type", "").ToLower() != "submit"
                         && n.GetAttributeValue("type", "").ToLower() != "button"
                         && n.GetAttributeValue("type", "").ToLower() != "reset");

            foreach (var input in inputs)
            {
                string name = input.GetAttributeValue("name", "");
                string value = input.GetAttributeValue("value", "");
                string type = input.GetAttributeValue("type", "").ToLower();

                if (type == "checkbox")
                {
                    value = input.GetAttributeValue("checked", "") != "" ? "1" : "0";
                }

                if (!string.IsNullOrEmpty(name) && !fields.ContainsKey(name))
                {
                    fields[name] = value;
                }
            }

            // Get all select elements
            var selects = hDoc.DocumentNode.Descendants("select")
                .Where(n => n.GetAttributeValue("name", "") != "");

            foreach (var select in selects)
            {
                string name = select.GetAttributeValue("name", "");
                var selectedOption = select.Descendants("option")
                    .FirstOrDefault(o => o.GetAttributeValue("selected", "") == "selected");

                if (selectedOption != null)
                {
                    string value = selectedOption.GetAttributeValue("value", "");
                    if (string.IsNullOrEmpty(value))
                    {
                        value = selectedOption.InnerText.Trim();
                    }
                    
                    if (!fields.ContainsKey(name))
                    {
                        fields[name] = value;
                    }
                }
            }

            // Get all textarea elements
            var textareas = hDoc.DocumentNode.Descendants("textarea")
                .Where(n => n.GetAttributeValue("name", "") != "");

            foreach (var textarea in textareas)
            {
                string name = textarea.GetAttributeValue("name", "");
                string value = textarea.InnerText;
                
                if (!fields.ContainsKey(name))
                {
                    fields[name] = value;
                }
            }

            return fields;
        }
    }
}
