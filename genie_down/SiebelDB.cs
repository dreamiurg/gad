using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace genie_down
{
   [Table(Name = "S_PROD_DEFECT")]
   class Defect
   {
      [Column(IsPrimaryKey = true, Name = "ROW_ID")]
      public string RowID = null;
      [Column(Name = "DEFECT_NUM")]
      public string Number = null;
      [Column(Name = "ABSTRACT")]
      public string Summary = null;
      [Column(Name = "LAST_UPD")]
      public DateTime LastUpd = new DateTime();

      private EntitySet<DefectAttachment> _DefectAttachments = null;
      [Association(Storage = "_DefectAttachments", OtherKey = "DefectID")]
      public EntitySet<DefectAttachment> DefectAttachments
      {
         get { return this._DefectAttachments; }
      }
   }

   [Table(Name = "S_DEFECT_ATT")]
   class DefectAttachment
   {
      [Column(IsPrimaryKey = true, Name = "ROW_ID")]
      public string RowID = null;

      [Column(Name = "FILE_NAME")]
      public string FileName = null;
      [Column(Name = "FILE_EXT")]
      public string FileExt = null;
      [Column(Name = "FILE_REV_NUM")]
      public string Revision = null;

      [Column(Name = "PAR_ROW_ID")]
      public string DefectID = null;
      private EntityRef<Defect> _Defect;
      [Association(Storage = "_Defect", ThisKey = "DefectID")]
      public Defect Defect
      {
         get { return this._Defect.Entity; }
      }

      public string FileNameFull
      {
         get { return this.FileName + "." + this.FileExt; }
      }
   }


   class SiebelDB
   {
      private DataContext db = null;
      private Program.Options _options = null;

      public SiebelDB(Program.Options options)
      {
         _options = options;

         System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
         builder.DataSource = _options.DbServer;
         builder.UserID = _options.Username;
         builder.Password = _options.Password;
         builder.InitialCatalog = _options.Database;

         if (_options.Verbose)
         {
            Console.WriteLine("Connecting to: '{0}'", builder.ConnectionString);
         }

         db = new DataContext(builder.ConnectionString);
      }

      public EntitySet<DefectAttachment> GetDefectAttachments(string number)
      {
         // create typed table 
         Table<Defect> Defects = db.GetTable<Defect>();

         if (_options.Verbose)
         {
            Console.WriteLine("Selecting attachments for defect {0}", number);
         }

         // query database
         var q =
            from d in Defects
            where d.Number == number
            select d
         ;

         if (q.Count() == 0)
            return null;

         return q.First().DefectAttachments;
      }
   }
}
