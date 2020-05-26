using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using ConsoleAppMMM.JULIETClasses;
using ConsoleAppMMM.Toolbox;

namespace ConsoleAppMMM
{
    static class Program
    {
        private static string SourceFolder { get; set; }
        private static string ArchiveFolder { get; set; }
        private static bool ArchiveFile { get; set; }
        private static string Database { get; set; }
        private static string Schema { get; set; }
        private static string Server { get; set; }
        private static string UserID { get; set; }
        private static string Password { get; set; }

        private const string FileFilter = "*.XML";

        static void Main()
        {
            if (ConfigureApplication())
            {
                using (FileSystemWatcher watcher = new FileSystemWatcher())
                {
                    // Set properties of watcher
                    watcher.Path = SourceFolder;
                    watcher.Filter = FileFilter;
                    watcher.IncludeSubdirectories = true;

                    // Set eventhandler of watcher
                    watcher.Created += new FileSystemEventHandler(Oncreated);
                    watcher.EnableRaisingEvents = true;

                    // Enable the user to quit the program
                    Console.WriteLine("Monitoring: " + SourceFolder);
                    Console.WriteLine("Press 'q' to quit the application.");
                    Console.WriteLine();
                    while (Console.Read() != 'q') ;
                }
            }
            else
            {
                Console.WriteLine("Press any key to quit the application.");
                Console.Read();
            }
        }

        private static void Oncreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("New file detected: " + e.Name);
            SqlConnection connection = null;
            try
            {
                Console.WriteLine("trying to connect");
                connection = DBMsSql.GetConnection(Database, Server, UserID, Password);
                //string connectionString = "Data Source=DESKTOP-81I8VH5\\SQLEXPRESS07; Initial Catalog = Galenus; Integrated Security = True"; // Deze gegevens moeten instelbaar zijn, dus staan in App.config. Zo'n gegevens mag je nooit hardcoded in een applicatie zetten.
                //string sql = ""; // VW 25/05/2020: wordt niet gebruikt en heb je niet nodig hier. Alle SQL code staat in DBMsSql.
                //connection = new SqlConnection(connectionString); // VW 25/05/2020: Je connection vraag je op via methode DBMsSql.GetConnection(...)
                //connection.Open(); // VW 25/05/2020: wordt al gedaan in methode DBMsSql.GetConnection(...)
                if (connection != null)
                {
                    if (!DBMsSql.GetMDNDX(connection, Schema, out long mdNDX))
                    {
                        Console.WriteLine(e.Name + ": could not get a valid MDNDX");
                        return;
                    }
                    JuMachineDataMMM machineDataMMM = new JuMachineDataMMM(mdNDX);
                    if (machineDataMMM.LoadFromFile(e.FullPath))
                    {
                        Console.WriteLine(e.Name + " was parsed.");
                    }
                    else
                    {
                        Console.WriteLine(e.Name + " could not be parsed.");
                        return;
                    }
                    if (!DBMsSql.Insert(machineDataMMM, connection, Schema))
                    {
                        Console.WriteLine(e.Name + ": MachineData could not be written to database");
                        return;
                    }
                    foreach (JuMachineSensor machineSensor in machineDataMMM.MachineSensors)
                    {
                        if (!DBMsSql.Insert(machineSensor, connection, Schema))
                        {
                            Console.WriteLine(e.Name + ": MachineSensor " + machineSensor.SensorID + " could not be written to database");
                            return;
                        }
                    }
                    foreach (JuMachineSensorValue machineSensorValue in machineDataMMM.MachineSensorValues)
                    {
                        if (!DBMsSql.Insert(machineSensorValue, connection, Schema))
                        {
                            Console.WriteLine(e.Name + ": MachineSensorValue " + machineSensorValue.DTArgument.ToString() + " could not be written to database");
                            return;
                        }
                    }
                    if (ArchiveFile)
                    {
                        if (MoveFileToArchive(e.FullPath))
                        {
                            Console.WriteLine(e.Name + " was archived.");
                        }
                        else
                        {
                            Console.WriteLine(e.Name + " could not be archived.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Connection to database failed");
                }
                //connection.Close(); // VW 25/05/2020: staat reeds in het Finally block
            }
            catch (Exception exc)
            {
                Console.WriteLine(e.Name + " database handling error: " + exc.Message);
                return;
            }
            finally
            {
                if (connection != null) { connection.Close(); }
                Console.WriteLine();
            }
        }

        private static bool MoveFileToArchive(string aFileFullPath)
        {
            if (!File.Exists(aFileFullPath)) { return false; }
            string destFile = aFileFullPath.Replace(SourceFolder, ArchiveFolder);
            try
            {
                File.Copy(aFileFullPath, destFile, false);
                File.Delete(aFileFullPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        private static bool ConfigureApplication()
        {
            try
            {
                SourceFolder = ConfigurationManager.AppSettings.Get("SourceFolder");
                if (!Directory.Exists(SourceFolder))
                {
                    Console.WriteLine(SourceFolder + " does not exists.");
                    return false;
                }
                ArchiveFolder = ConfigurationManager.AppSettings.Get("ArchiveFolder");
                ArchiveFile = !string.IsNullOrEmpty(ArchiveFolder);
                if (ArchiveFile && (!Directory.Exists(ArchiveFolder)))
                {
                    Console.WriteLine(ArchiveFolder + " does not exists.");
                    return false;
                }
                Database = ConfigurationManager.AppSettings.Get("Database");
                Schema = ConfigurationManager.AppSettings.Get("Schema");
                Server = ConfigurationManager.AppSettings.Get("Server");
                UserID = ConfigurationManager.AppSettings.Get("UserID");
                Password = ConfigurationManager.AppSettings.Get("Password");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ConfigureApplication: " + e.Message);
            }
            return false;


        }
        //static private bool MoveFile(string aFileFullPath)
        //{
            
            //string FilePath = aFileFullPath;
            //int Length = SourceFolder.Length;
            //string Ksubstring = FilePath.Substring(Length + 1);
            //string destFile = System.IO.Path.Combine(ArchiveFolder, Ksubstring);
            ////coppy the map structure
            //string stringCutted = destFile.Split('\\').Last();
            //int LengthFile = stringCutted.Length;
            //string destfile2 = destFile.Remove(destFile.Length - LengthFile);
            //System.IO.Directory.CreateDirectory(destfile2);
            ////stop coppy the map sturcture
            //if (System.IO.Directory.Exists(SourceFolder)) 
            //{
            //    System.IO.File.Copy(FilePath, destFile, true);
            //}
            //else
            //{
            //    Console.WriteLine("Source path does not exist!");
            //    return false;
            //}
            //try
            //{
            //    System.IO.File.Delete(@FilePath);
            //}
            //catch (System.IO.IOException e)
            //{
            //    Console.WriteLine(e.Message);
            //    return false;
            //}
            //return true;

         //}
    }


       //  string SourceFolder = @"C:\Users\Gijs\Desktop\directory\testfile";


        /*
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
         
        /*
        private void MoveFile(string fileName)
        {
            a++;
            string Destination = @"C:\Users\Gijs\Desktop\directory\Gelezen\Gelezen#" + a +".XML";
            Console.WriteLine("MOVING");
            File.Move(fileName, Destination);
            return;
        }
        */
    

    }


