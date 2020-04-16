using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            FileInputMonitor fileInputMonitor = new FileInputMonitor();
            Console.Read();

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

        /* GetAmountOfValues Function
         *  Een functie dat dient om het # waardes dat de sensoren hebben opgemeten te bepalen.
         */ 
        private int GetAmountOfValues(string data, int startlocatie)
        {
            // De hoeveel values dat er in de file zitten binnenhalen

            // Checken of er een V staat op de index
            int i = 0;                                      // # counter
            int begin;                                      // De plaats in de string waar de V check op gebeurd
            int afstand = Getfilling(data, startlocatie);   // De filling afstand tussen de waardes
            begin = startlocatie - 6;                       // De startlocatie van de waarde - 6 om op de plaats van de V te zitten
            
            bool Check = true;                              // Is er nog een volgende waarde ja == true nee == false;

            char[] Controle;                                // Tijdelijke array om de te checken char in te steken
    
            while(Check == true)                            // Is er een volgende waarde Ja ga door / nee stop
            {
                
                Controle = data.ToCharArray(begin, 1);      // Haal de te checken char op
                if (Controle[0].Equals('V'))                // is het een V, ja er is nog een waarde
                {
                    //Console.WriteLine("Het is een V");
                    
                    Check = true;                           // Blijven zoeken naar waardes
                    begin = begin + afstand;                // Zet de begin index klaar voor een mogelijk volgende waarde
                    i++;                                    // +1 waarde
                }
                else
                {
                    Check = false;                          // er is geen volgende waarde, exit de while loop
                    break;
                }

                afstand = Getfilling(data, begin+6);
            }
            return i;                                       // return de counter
        }

        /* GetLength Function
         *  Een functie dat dient om de lengte van de waardes te bepalen.
         */ 
        private int GetLengthValue(string data, int startlocatie )
        {
            int i = 0;
            int begin;

            begin = startlocatie;
            char[] Controle;

            bool Check = true;

            while(Check == true)
            {
                Controle = data.ToCharArray(begin, 1);
                if (!(Controle[0].Equals('<')))
                {
                    Check = true;
                    begin++;
                    i++;
                }
                else
                {
                    Check = false;
                    //Console.WriteLine(i);
                }
            }

            return i;
        }

        /* GetFilling Function
         *   Een functie dat dient om het aantal karakters tussen de eigenlijke waardes te bepalen.
         */ 
        private int Getfilling(string data, int startlocatie)
        {
            int i = 0;
            int begin;
            int aantal = 0;

            begin = startlocatie;
            char[] Controle;

            while (aantal < 2)
            {
                Controle = data.ToCharArray(begin, 1);
                if ((Controle[0].Equals('>') ) )
                {
                    aantal++;
                    
                }

                if (aantal <= 2)
                {
                    begin++;
                    i++;
                }

            }
           // Console.Write("De filling waarde is : ");
           // Console.WriteLine(i);
            return i;
        }

        /* GetArrayValues Function
         * Een functie dat dient om de waardes van de sensoren op te slaan in een 2D array.
         */ 
        private char[ , ] GetArrayValues(string data, int aantal, int index)
        {
            int grootte = GetLengthValue(data, index);
            int afstand = Getfilling(data, index);
            int i;
            int a = 0;

            char[] values = new char[50];
            char[ , ] values2 = new char[aantal, 50];

            
            for (i = 0; i < aantal; i++)
            {
                
                values = data.ToCharArray(index, grootte);
                for (a = 0; a < grootte; a++)
                {
                    values2[i,a] = values[a];
                    Console.Write(values2[i,a]);
                }
                Console.WriteLine();
                index = index + afstand;
                grootte = GetLengthValue(data, index);
                afstand = Getfilling(data, index);

            }

            return values2;
        }

        /* Get SingleValue Function
         * Een functie dat dient om een string van data uit het bestand te halen.
         */ 
        private char[] GetSingleValue(string data, int startlocatie)
        {
            int grootte = 0;

            grootte = GetLength(data, startlocatie);

            char[] value = new char[grootte];
            value = data.ToCharArray(startlocatie, grootte);
            return value;

        }

        /* Get Length function
         * Een functie dat dient om de lengte van waardes tussen 2 " tekens te berekenen
         */ 
        private int GetLength(string data, int startlocatie)
        {
            int i = 0;
            int aantal = 0;
            int begin = 0;
            char[] Controle;

            begin = startlocatie;

            while (aantal < 2)
            {
                Controle = data.ToCharArray(begin, 1);
                if ((Controle[0].Equals('"')))
                {
                    aantal++;
                }

                if (aantal <= 2)
                {
                    begin++;
                    i++;
                }

            }

            return i;
        }

        /* FindIndex Function
         * Een functie dat dient om de Index van alle waardes te bepalen.
         */ 
        private int FindIndex(string data, string Text, int Index)
        {
            int i = Index;
            int a = 0;
            int len = Text.Length;
            int aantal = 0;

            char[] Checksum = new char[len];
            char[] Controle;

            Checksum = Text.ToCharArray(0, len);
            //Console.WriteLine(Checksum);
            
            while (aantal != len)
            {
                Controle = data.ToCharArray(i, 1);
                if (Controle[0].Equals(Checksum[a]))
                {
                    a++;
                    aantal++;
                }
                else
                {
                    i++;
                }
            }
            i++;

            return i;

        }
    }

}

