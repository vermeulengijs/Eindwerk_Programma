using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using ConsoleAppMMM.JULIETClasses;

namespace ConsoleAppMMM
{
    static class Program
    {
        private static string SourceFolder { get; set; }
        private static string ArchiveFolder { get; set; }
        private static bool ArchiveFile { get; set; }

        static void Main()
        {
            // Read App.config file
            ConfigureApplication();
            

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                // Set properties of watcher
                watcher.Path = SourceFolder;
                watcher.Filter = "*.XML";
                watcher.IncludeSubdirectories = true;

                // Set eventhandler of watcher
                watcher.Created += new FileSystemEventHandler(Oncreated);
                watcher.EnableRaisingEvents = true;

                // Enable the user to quit the program
                Console.WriteLine("Monitoring: " + SourceFolder);
                Console.WriteLine("Press 'q' to quit the application.");
                while (Console.Read() != 'q') ;
            }
        }

        private static void Oncreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("New file detected: " + e.Name);
            // TODO: get next value of sequence and use this value to initiate MDNDX of machineDataMMM
            JuMachineDataMMM machineDataMMM = new JuMachineDataMMM(-1);
            if (machineDataMMM.LoadFromFile(e.FullPath))
            {
                Console.WriteLine(e.Name + " was parsed.");
            }
            else
            {
                Console.WriteLine(e.Name + " could not be parsed.");
            }
            // TODO: save machineDataMMM to the database. This function should also save the lists MachineSensors and MachineSensorValues to the database

            if (ArchiveFile)
            {
                if (!MoveFileToArchive(e.FullPath))
                {
                    Console.WriteLine(e.Name + " was archived.");
                }
                else
                {
                    Console.WriteLine(e.Name + " could not be archived.");
                }
            }
        }

        private static bool MoveFileToArchive(string aFileFullPath)
        {
            if (!File.Exists(aFileFullPath)) { return false; }
            
            // TODO: write function
            return true;
        }

        private static void ConfigureApplication()
        {
            try
            {
                SourceFolder = ConfigurationManager.AppSettings.Get("SourceFolder");
                if (!Directory.Exists(SourceFolder))
                {
                    Console.WriteLine(SourceFolder + " does not exists.");
                }
                ArchiveFolder = ConfigurationManager.AppSettings.Get("ArchiveFolder");
                if (ArchiveFile && (!Directory.Exists(ArchiveFolder)))
                {
                    Console.WriteLine(ArchiveFolder + " does not exists.");
                }
                ArchiveFile = !string.IsNullOrEmpty(ArchiveFolder);

                // TODO: add keys for database access
            }
            catch (Exception e)
            {
                Console.WriteLine("ConfigureApplication: " + e.Message);
            }
        }
    }

    public class FileInputMonitor
    {

        public FileSystemWatcher fileSystemWatcher;
        //private string folderToWatchFor = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "toTechcoil";

        // een specifieke path naar de directory waar het bestand in komt.
        //string SourceFolder = @"C:\Users\gijsimus\Desktop\directory\testfile";
        //string DestinationFolder = @"C:\Users\gijsimus\Desktop\directory\Gelezen\GELEZEN.txt";

        string SourceFolder = @"C:\Users\Gijs\Desktop\directory\testfile";
        //string DestinationFolder = @"C:\Users\Gijs\Desktop\directory\Gelezen\GELEZEN.txt";

        int a = 0;  // # files dat er gebeurd zijn


        public FileInputMonitor()
        {

            fileSystemWatcher = new FileSystemWatcher(SourceFolder)
            {
                Filter = "*.XML",            // Filteren op bestanden van het type .xml
                EnableRaisingEvents = true   // events enabelenn 
            };  
            fileSystemWatcher.Created += new FileSystemEventHandler(FileCreated);
           
        }

        private void FileCreated(Object sender, FileSystemEventArgs e)
        {
            // er is een nieuw bestand in de map geplaatst
            Console.WriteLine("New file available");
            FileInfo file = new FileInfo(e.FullPath);
            Console.WriteLine(e.FullPath);
            Console.WriteLine(file.Name);
            ProcessFile(e.FullPath);
            InsertInDatabase();
           // MoveFile(e.FullPath);
        }

        private void ProcessFile(String fileName)
        {
            Console.WriteLine("PROCESSING");
            XDocument doc = XDocument.Load(fileName);
            //XName Language = "Language";

            // De taal eruit halen
            IEnumerable<string> Taal = doc.Descendants("MMM_SimSocket")
                                            .Descendants("Language")
                                            .Attributes()
                                            .Select(a => a.Value);
            foreach (string value in Taal)
            {
                Console.WriteLine(value);
            }

            // De form data eruit halen
            IEnumerable<string> Form = doc.Descendants("MMM_SimSocket")
                                .Descendants("Form")
                                .Attributes()
                                .Select(a => a.Value);
            foreach (string value in Form)
            {
                Console.WriteLine(value);
            }

            // De batchsummary eruit halen
            IEnumerable<string> Batchsummary = doc.Descendants("MMM_SimSocket")
                    .Descendants("BatchSummary")
                    .Descendants("Line")
                    .Attributes()
                    .Select(a => a.Value);
            foreach (string value in Batchsummary)
            {
                Console.WriteLine(value);
            }

            // De batchprocess eruit halen
            IEnumerable<string> BatchProcess = doc.Descendants("MMM_SimSocket")
                 .Descendants("BatchProcess")
                 .Descendants("Line")
                 .Attributes()
                 .Select(a => a.Value);
            foreach (string value in BatchProcess)
            {
                Console.WriteLine(value);
            }

            // De batchResult eruit halen
            IEnumerable<string> BatchResult = doc.Descendants("MMM_SimSocket")
                .Descendants("BatchResult")
                .Descendants("Line")
                .Attributes()
                .Select(a => a.Value);
            foreach (string value in BatchResult)
            {
                Console.WriteLine(value);
            }

            // De values eruit halen
            IEnumerable<string> Values = doc.Descendants("MMM_SimSocket")
              .Descendants("Sensors")
              .Descendants("Values")
              .Descendants("Value")
              .Select(a => a.Value);
            foreach (string value in Values)
            {
                Console.WriteLine(value);
            }


            Console.WriteLine("DONE");

        }

        private void InsertInDatabase( )
        {
            Console.WriteLine("Writing data to the database");
            string connectionString = "Data Source=DESKTOP-81I8VH5\\SQLEXPRESS07; Initial Catalog = DBAutoclaaf; Integrated Security = True";
            string sql = "";
            
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            Console.WriteLine("Connection is open");
            if (con.State== System.Data.ConnectionState.Open)
            {
                sql= "INSERT INTO dataautoclaaf_Batch_results(Index_ID,Language) values(3,'Nederlands')";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
            con.Close();
                       
        }
        /* MoveFile Function
         *  Een functie dat dient om het bestand te verplaatsen en te hernoemen als alle data verwerkt is.
         */
        private void MoveFile(string fileName)
        {
            a++;
            string Destination = @"C:\Users\Gijs\Desktop\directory\Gelezen\Gelezen#" + a +".XML";
            Console.WriteLine("MOVING");
            File.Move(fileName, Destination);
            return;
        }

    }

}

