using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace WcfServiceModemToolbarSync
{
    public class DbHelper
    {
        private OleDbConnection accessConn = null;
        private string connString = "";
        

        public  string localConnectionString { get;  } = @"Provider=Microsoft.ACE.OLEDB.12.0;Mode=Read;" +
                @"Data source= " + localFile + ";";
        //public string remoteConnectionString { get; set; } = @"Provider=Microsoft.ACE.OLEDB.12.0;Mode=Read;" +
        //    @"Data Source=""\\corp.halliburton.com\eu\NOR\TAN\DATA\Sperry Sun\09 RTO-Insite\RigAccessFolder\Common\StandardMwdDatabase\ModemToolDbServer.accdb"";";

        public string remoteConnectionString { get;  } = @"Provider=Microsoft.ACE.OLEDB.12.0;Mode=Read;" +
            @"Data Source= "+ remoteFile +";";

        public static string remoteFile  = @"\\corp.halliburton.com\eu\NOR\TAN\DATA\Sperry Sun\09 RTO-Insite\RigAccessFolder\Common\StandardMwdDatabase\ModemToolDbServer.accdb";
        public static string localFile  = WCFModemService.DataFolder + @"\ModemToolDbLocal.accdb";

        public void OpenConnection(string connectionString)
        {
            connString = connectionString;
            accessConn = new OleDbConnection();
            accessConn.ConnectionString = connectionString;
            accessConn.Open();
        }

        public void CloseConnection()
        {
            accessConn.Close();
        }

        public object GetRecordObject(int id, string tableName)
        {
            
            if (tableName.Contains("LooseItemMenu"))
            {
                return GetLooseItemMenu(id);
            }
            else if (tableName.Contains("LooseItems"))
            {
                return GetLooseItems(id);
            }
            else if (tableName.Contains("Software"))
            {
                return GetSoftware(id);
            }
            else if (tableName.Contains("SubTools"))
            {
                return GetSubTools(id);
            }
            else if (tableName.Contains("Tools"))
            {
                return GetTools(id);
            }
            else
            {
                return null;
            }
           
        }

        public bool InsertRecordObject(object obj, string tableName)
        {
            bool returnValue = false;
            try
            {

                if (tableName.Contains("LooseItemMenu"))
                {
                    InsertLooseItemMenuRecord((LooseItemMenuClass)obj);
                }
                else if (tableName.Contains("LooseItems"))
                {
                    InsertLooseItemsRecord((LooseItemsClass)obj);
                }
                else if (tableName.Contains("Software"))
                {
                    InsertSoftwareRecord((SoftwareClass)obj);
                }
                else if (tableName.Contains("SubTools"))
                {
                    InsertSubToolsRecord((SubToolsClass)obj);
                }
                else if (tableName.Contains("Tools"))
                {
                    InsertToolsRecord((ToolsClass)obj);
                }
                else
                {
                    return false;
                }

                returnValue = true;
            }
            catch (Exception)
            {

                throw;
            }

            return returnValue;

        }


        public Dictionary<int, TblAuditClass> GetTblAuditAsList()
        {
            // This will hold the records.
            Dictionary<int, TblAuditClass> inv = new Dictionary<int, TblAuditClass>();
            int counter = 0;
            // Prep command object.
            string sql = "Select * From tblAuditTrail Order By AuditTrailID";
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                OleDbDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    TblAuditClass tmp = new TblAuditClass();
                    if (!dataReader["AuditTrailID"].Equals(System.DBNull.Value))
                        tmp.tblId = (int)dataReader["AuditTrailID"];
                    else
                        break;
                    tmp.dateTime = (DateTime)dataReader["DateTime"];
                    tmp.userName = (string)dataReader["UserName"] ?? "";
                    tmp.formName = (string)dataReader["FormName"];
                    tmp.action = (string)dataReader["Action"];
                    tmp.tableName = (string)dataReader["TableName"];
                    if (!dataReader["RecordID"].Equals(System.DBNull.Value))
                        tmp.recordId = (string)dataReader["RecordID"];
                    if (!dataReader["FieldName"].Equals(System.DBNull.Value))
                        tmp.fieldName = (string)dataReader["FieldName"];
                    if(!dataReader["OldValue"].Equals(System.DBNull.Value))
                        tmp.oldValue = (string)dataReader["OldValue"];
                    if(!dataReader["NewValue"].Equals(System.DBNull.Value))
                        tmp.newValue = (string)dataReader["NewValue"];

                    inv.Add(counter++, tmp);
                }
                dataReader.Close();
            }
            return inv;
        }

        public LooseItemMenuClass GetLooseItemMenu(int id)
        {
            // This will hold the records.
            LooseItemMenuClass inv = new LooseItemMenuClass();
            // Prep command object.
            string sql = "Select * From LooseItemMenu Where ID=" + id.ToString();
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                OleDbDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    inv = (new LooseItemMenuClass
                    {
                        id = (int) dataReader["ID"], 
                        description = (string)dataReader["Description"]
                       
                    });
                }
                dataReader.Close();
            }
            return inv;
        }

        public LooseItemsClass GetLooseItems(int id)
        {
            // This will hold the records.
            LooseItemsClass inv = new LooseItemsClass();
            // Prep command object.
            string sql = "Select * From LooseItems Where ID=" + id.ToString();
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                OleDbDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    LooseItemsClass tmp = new LooseItemsClass();
                    tmp.id = (int)dataReader["ID"];
                    tmp.qty = ((int)dataReader["Qty"]).ToString();
                    if (!dataReader["Vendor"].Equals(System.DBNull.Value))
                        tmp.vendor = (string)dataReader["Vendor"];
                    tmp.description = (string)dataReader["Description"];
                    if (!dataReader["Comments"].Equals(System.DBNull.Value))
                        tmp.comments = (string)dataReader["Comments"];
                    if (!dataReader["ThreadTop"].Equals(System.DBNull.Value))
                        tmp.threadTop = (string)dataReader["ThreadTop"];
                    if (!dataReader["ThreadBottom"].Equals(System.DBNull.Value))
                        tmp.threadBottom = (string)dataReader["ThreadBottom"];
                    tmp.looseItemId = (int)dataReader["LooseItemId"];
                    
                        inv = tmp;
                }
                dataReader.Close();
            }
            return inv;
        }

        public SoftwareClass GetSoftware(int id)
        {
            // This will hold the records.
            SoftwareClass inv = new SoftwareClass();
            // Prep command object.
            string sql = "Select * From Software Where ID=" + id.ToString();
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                OleDbDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    inv = (new SoftwareClass
                    {
                        id = (int)dataReader["ID"],
                        softwareSensor  = (string)dataReader["SoftwareSensor"],
                        softwareVersion = (string)dataReader["SoftwareVersion"]
                       
                    });
                }
                dataReader.Close();
            }
            return inv;
        }

        public SubToolsClass GetSubTools(int id)
        {
            // This will hold the records.
            SubToolsClass inv = new SubToolsClass();
            // Prep command object.
            string sql = "Select * From SubTools Where ID=" + id.ToString();
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                OleDbDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    SubToolsClass tmp = new SubToolsClass();
                    tmp.id = (int)dataReader["ID"];
                    tmp.sequence = (dataReader["Sequence"]).ToString();
                    if (!dataReader["Torque"].Equals(System.DBNull.Value))
                        tmp.torque = ((int)dataReader["Torque"]).ToString();
                    if (!dataReader["ThreadTop"].Equals(System.DBNull.Value))
                        tmp.threadTop = (string)dataReader["ThreadTop"];
                    if (!dataReader["ThreadBottom"].Equals(System.DBNull.Value))
                        tmp.threadBottom = (string)dataReader["ThreadBottom"];
                    tmp.description = (string)dataReader["Description"];
                    if (!dataReader["Comments"].Equals(System.DBNull.Value))
                        tmp.comments = (string)dataReader["Comments"];
                    tmp.toolId = (int)dataReader["ToolID"];

                    
                }
                dataReader.Close();
            }
            return inv;
        }

        public ToolsClass GetTools(int id)
        {
            // This will hold the records.
            ToolsClass inv = new ToolsClass();
            // Prep command object.
            string sql = "Select * From Tools Where ID=" + id.ToString();
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                OleDbDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    inv = (new ToolsClass
                    {
                        id = (int)dataReader["ID"],
                        toolName = (string)dataReader["ToolName"],
                        toolSize = (dataReader["ToolSize"]).ToString(),
                        category = (string)dataReader["Category"],


                    });
                }
                dataReader.Close();
            }
            return inv;
        }

        public void InsertTblAuditRecord(TblAuditClass tblAudit, string sCase)
        {
            // Format and execute SQL statement.
            string sql = "";

            switch (sCase)
            {
                case "NEW":
                    sql = "Insert Into tblAuditTrail " + $"( DateTime, UserName, FormName, Action, TableName, RecordID) " +
                            $"Values ( '{tblAudit.dateTime.ToLongDateString()}', '{tblAudit.userName}', '{tblAudit.formName}', '{tblAudit.action}', '{tblAudit.tableName}', '{tblAudit.recordId}');";

                    break;
                case "EDIT":
                    sql = "Insert Into tblAuditTrail " + $"( DateTime, UserName, FormName, Action, TableName, RecordID, FieldName, OldValue, NewValue) " +
                            $"Values ( '{tblAudit.dateTime.ToLongDateString()}', '{tblAudit.userName}', '{tblAudit.formName}', '{tblAudit.action}', '{tblAudit.tableName}', '{tblAudit.recordId}', '{tblAudit.fieldName}', '{tblAudit.oldValue}', '{tblAudit.newValue}');";

                    break;
                case "DELETE":
                    sql = "Insert Into tblAuditTrail " + $"( DateTime, UserName, FormName, Action, TableName) " +
                            $"Values ( '{tblAudit.dateTime.ToLongDateString()}', '{tblAudit.userName}', '{tblAudit.formName}', '{tblAudit.action}', '{tblAudit.tableName}');";


                    break;
                default:
                    break;
            }

            sql = "Insert Into tblAuditTrail " + $"( DateTime, UserName, FormName, Action, TableName, RecordID, FieldName, OldValue, NewValue) " +
                $"Values ( '{tblAudit.dateTime.ToLongDateString()}', '{tblAudit.userName}', '{tblAudit.formName}', '{tblAudit.action}', '{tblAudit.tableName}', '{tblAudit.recordId}', '{tblAudit.fieldName}', '{tblAudit.oldValue}', '{tblAudit.newValue}');";
            // Execute using our connection.


            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }

        }

        public void InsertLooseItemMenuRecord(LooseItemMenuClass item)
        {
            // Format and execute SQL statement.
            string sql = "Insert Into LooseItemMenu " + $"( Description) " +
                $"Values ('{item.description}');";
            // Execute using our connection.
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }

        }

        public void InsertLooseItemsRecord(LooseItemsClass item)
        {
            // Format and execute SQL statement.
            string sql = "Insert Into LooseItems " + $"( Qty, Vendor, Description, Comments, ThreadTop, ThreadBottom, LooseItemId) " +
                $"Values ( {item.qty}, '{item.vendor}', '{item.description}', '{item.comments}', '{item.threadTop}', '{item.threadBottom}', {item.looseItemId.ToString()});";
            // Execute using our connection.
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }

        }

        public void InsertSoftwareRecord(SoftwareClass item)
        {
            // Format and execute SQL statement.
            string sql = "Insert Into Software " + $"( SoftwareSensor, SoftwareVersion, ToolID) " +
                $"Values ( '{item.softwareSensor}', '{item.softwareVersion}', {item.toolId.ToString()});";
            // Execute using our connection.
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }

        }

        public void InsertSubToolsRecord(SubToolsClass item)
        {
            // Format and execute SQL statement.
            string sql = "Insert Into SubTools " + $"( Sequence, Torque, ThreadTop, ThreadBottom, Description, Comments, ToolID) " +
                $"Values ( {item.sequence}, {item.torque}, '{item.threadTop}', '{item.threadBottom}', '{item.description}', '{item.comments}', {item.toolId.ToString()});";
            // Execute using our connection.
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }

        }

        public void InsertToolsRecord(ToolsClass item)
        {
            // Format and execute SQL statement.
            string sql = "Insert Into Tools " + $"( ToolName, ToolSize, Category) " +
                $"Values ( '{item.toolName}', {item.toolSize}, '{item.category}')";

            
            // Execute using our connection.
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }

        }

        public void UpdateRecord(string tableName, int recordId, string fieldName, string newValue)
        {
            // Update the PetName of the car with the specified CarId.
            string sql = $"Update '{tableName}' Set '{fieldName}' = '{newValue}' Where ID = {recordId.ToString()};";
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                command.ExecuteNonQuery();
            }
        }


        public void DeleteRecord(string tableName, int id)
        {
            // Delete the car with the specified CarId
            string sql = $"Delete from '{tableName}' where ID = {id};";
            using (OleDbCommand command = new OleDbCommand(sql, accessConn))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OleDbException ex)
                {
                    Exception error = new Exception("Sorry! not possible!", ex);
                   // throw error;
                }
            }
        }

        //private string StringDbNull(ToolDb.SubToolsRow row, string fieldName)
        //{
        //    if (!DBNull.Value.Equals(row[fieldName]))
        //    {
        //        return (string)row[fieldName];
        //    }
        //    else
        //    {
        //        return ""; // returns the default value for the type
        //    }

        //}

        //private string StringDbNull(ToolDb.SoftwareRow row, string fieldName)
        //{
        //    if (!DBNull.Value.Equals(row[fieldName]))
        //    {
        //        return (string)row[fieldName];
        //    }
        //    else
        //    {
        //        return ""; // returns the default value for the type
        //    }

        //}

        //private string IntDbNull(ToolDb.SubToolsRow row, string fieldName)
        //{
        //    if (!DBNull.Value.Equals(row[fieldName]))
        //    {
        //        return ((int)row[fieldName]).ToString();
        //    }
        //    else
        //    {
        //        return ""; // returns the default value for the type
        //    }

        //}

    }

    

    public class TblAuditClass
    {
        public int tblId { get; set; }
        public DateTime dateTime  { get; set; }
        public string userName { get; set; }
        public string formName { get; set; }
        public string action { get; set; }
        public string tableName { get; set; }
        public string recordId { get; set; }
        public string fieldName { get; set; } = "";
        public string oldValue { get; set; } = "";
        public string newValue { get; set; } = "";



    }

    public class LooseItemMenuClass
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public class LooseItemsClass
    {
        public int id { get; set; }
        public string qty { get; set; }
        public string vendor { get; set; } = "";
        public string description { get; set; }
        public string comments { get; set; } = "";
        public string threadTop { get; set; } = "";
        public string threadBottom { get; set; } = "";
        public int looseItemId { get; set; }
    }

    public class SoftwareClass
    {
        public int id { get; set; }
        public string softwareSensor { get; set; }
        public string softwareVersion { get; set; }
        public int toolId { get; set; }

    }

    public class SubToolsClass
    {
        public int id { get; set; }
        public string sequence { get; set; }
        public string torque { get; set; } = "";
        public string threadTop { get; set; } = "";
        public string threadBottom { get; set; } = "";
        public string description { get; set; }
        public string comments { get; set; } = "";
        public int toolId { get; set; }
    }

    public class ToolsClass
    {
        public int id { get; set; }
        public string toolName { get; set; }
        public string toolSize { get; set; }
        public string category { get; set; }

    }
}
