//namespace WcfServiceModemToolbarSync.Database
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Data.Entity;
//    using System.Linq;

//    public class LocalDb : DbContext
//    {
//        // Your context has been configured to use a 'LocalDb' connection string from your application's 
//        // configuration file (App.config or Web.config). By default, this connection string targets the 
//        // 'WcfServiceModemToolbarSync.Database.LocalDb' database on your LocalDb instance. 
//        // 
//        // If you wish to target a different database and/or database provider, modify the 'LocalDb' 
//        // connection string in the application configuration file.
//        public LocalDb()
//            : base("name=LocalDb")
//        {
//        }

//        // Add a DbSet for each entity type that you want to include in your model. For more information 
//        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

//        // public virtual DbSet<MyEntity> MyEntities { get; set; }
//    }

//    //public class MyEntity
//    //{
//    //    public int Id { get; set; }
//    //    public string Name { get; set; }
//    //}

        
//    public class AuditEntity
//    {
        
//        public int AuditTrailID { get; set; }
//        public DateTime DateTime { get; set; }
//        public string UserName { get; set; }
//        public string FormName { get; set; }
//        public string Action { get; set; }
//        public string TableName { get; set; }
//        public string RecordID { get; set; }
//        public string FieldName { get; set; }
//        public string OldValue { get; set; }
//        public string NewValue { get; set; }


//    }




//}