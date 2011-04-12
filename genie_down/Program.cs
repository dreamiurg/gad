using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace genie_down
{
   class Program
   {
      private static readonly HeadingInfo headingInfo = new HeadingInfo(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

      public class Options
      {
         #region Standard Options

         [Option("s", "server", Required = false, HelpText = "Database server to connect to (default: sblsdb)")]
         public string DbServer = "sblsdb";

         [Option("u", "user", Required = false, HelpText = "Username to connect to DB (default: Windows logon username)")]
         public string Username = Environment.UserName.ToUpper();

         [Option("p", "password", Required = false, HelpText = "Password to connect to DB (default: Windows logon username)")]
         public string Password = Environment.UserName.ToUpper();

         [Option("d", "database", Required = false, HelpText = "Database to use (default: SiebelDB)")]
         public string Database = "SiebelDB";

         [Option("n", "number", Required = true, HelpText = "Defect/Suggestion/Inquiry number")]
         public string Number = null;

         [Option("v", "verbose", Required = false, HelpText = "Verbose mode")]
         public bool Verbose = false;

         #endregion

         #region Usage
         [HelpOption(HelpText = "Display this help screen.")]
         public string GetUsage()
         {
            HelpText help = new HelpText(Program.headingInfo);
            help.Copyright = new CopyrightInfo("Dmitry Guyvoronsky", 2010);
            help.AddPreOptionsLine("http://github.com/dreamiurg/gad");
            help.AddOptions(this);
            return help; 
         }
         #endregion
      }

      static void Main(string[] args)
      {
         try
         {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
               SiebelDB sdb = new SiebelDB(options);
               var atts = sdb.GetDefectAttachments(options.Number);

               if (atts != null && atts.Count() > 0)
               {
                  Console.WriteLine(@"@echo off");
                  Console.WriteLine();
                  foreach (var att in atts)
                  {
                     Console.WriteLine(@"echo -- Copying {0}", att.FileNameFull);
                     Console.WriteLine(@"copy ""\\SBLAPP.denver.cqg\Siebel File System\S_DEFECT_ATT_{0}_{1}.SAF"" ""{2}.saf"" /z",
                         att.RowID, att.Revision, att.FileNameFull);
                     Console.WriteLine(@"sseunzip.exe 2>nul ""{0}.saf"" ""{0}""",
                         att.FileNameFull);
                     Console.WriteLine(@"echo -- Done {0}", att.FileNameFull);
                     Console.WriteLine();
                  }
               }
               else
               {
                  Console.Error.Write("No such defects or no attachments.");
               }
            }
            else
            {
               Console.Error.Write(options.GetUsage());
            }

         }
         catch (System.Exception ex)
         {
            Console.WriteLine(ex.Message);
         }
      }
   }
}
