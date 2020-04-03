using System;
using System.IO;
using System.Threading;

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
            MoveFile(e.FullPath);
            
        }

        private void ProcessFile(String fileName)
        {
            Console.WriteLine("PROCESSING");
            FileStream inputFileStream;
            while (true)
            {
                try
                {
                    // lees het bestand en schrijf dit naar de console.
                    inputFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(inputFileStream);

                    /// MMM simsocket
                    string buffer;
                    char[] NaamAutoclaaf;       // naam van de autoclaaf
                    char[] Vacuumtest;          // true of false of het een vacuum test is
                    char[] Machinetype;         // machine type

                    // Batchsummary
                    char[] cyclus_nummer;       // cyclus nummer
                    char[] cyclus_user_start;   // cyclus is gestart door
                    char[] cyclus_user_stop;    // cyclus is gestopt door
                    char[] programma;           // programma nummer

                    // batchprocess 
                    char[] Start_tijd_programma;// start tijd van het programma
                    char[] stop_tijd_programma; // stop tijd van het programma
                    char[] Evacuation;          // Evacuatie
                    char[] start_tijd_vacuum;   // start tijd van de vacuum test
                    char[] stop_tijd_vacuum;    // stop  tijd van de vacuum test
                    char[] duur_vacuum;         // Duur van de vacuum test
                    char[] status_naam;         // De naam van de status
                    char[] status_value;        // De status waarde
                    char[] start_tijd_fase;     // start tijd van de fase
                    char[] stop_tijd_fase;      // stop  tijd van de fase
                    char[] duur_fase;           // duur van de fase
                    char[] stop_tijd_chauffage; // stop tijd van de chauffage

                    int Index;                  // De index 

                    // Values 
                    //--------
                    // tijd
                    char[,] waardes_tijd;
                    //int Index_tijd = 53589;      // De begin index van tijd waardes
                    int aantal_tijd;             // # tijd waardes

                    // druk
                    char[,] waardes_druk;
                    //int Index_druk = 102333;    // De begin index van druk waardes
                    int aantal_druk;            // # druk waardes

                    // temperatuur
                    char[,] waardes_temp;
                    //int Index_temp = 127680;    // De begin index van de temperatuur waardes
                    int aantal_temp;            // # Temperatuur waardes
                    
                    // de grootte van de array is afhankelijk van de lengte van de file
                    int numbytesToread = (int)inputFileStream.Length;
                    Console.WriteLine(numbytesToread); // de grootte afprinten
                    buffer = reader.ReadToEnd();
                    reader.Close();

                    // Niet alle bestanden zijn op dezelfde manier opgebouwd.
                    // Een Vacuumtest bestand is anders opgebouwd dan het bestand van een normale cyclus --> deze moeten dus verschillend bewerkt worden
                    // Hoe het XML bestand is opgebouwd 
                    // VACUUM test

                    // MMM simsocket Bestand
                    // De gedeelde waardes ophalen
                    // ----------------------------
                    Index = FindIndex(buffer, "Caption=", 0);
                    NaamAutoclaaf = GetSingleValue(buffer, Index);
                    Index = FindIndex(buffer, "IsVacuumTest=", Index);
                    Vacuumtest = GetSingleValue(buffer, Index);

                    // De gedeelde waardes afprinten
                    // ------------------------------
                    Console.Write("Naam van de autoclaaf : ");
                    Console.WriteLine(NaamAutoclaaf);
                    Console.Write("Is het een Vacuumtest : ");
                    Console.WriteLine(Vacuumtest);

                    if (Vacuumtest[1] == 't')
                    {
                        // de  batchsummary van een vacuumtest bestand
                        // De waardes ophalen
                        // -------------------
                        Index = FindIndex(buffer, "MachineType=", Index);
                        Machinetype = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Numéro de charge", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        cyclus_nummer = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Cycle démarré par", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        cyclus_user_start = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Cycle terminé par", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        cyclus_user_stop = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Numéro de programme", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        programma = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "DEBUT DU CYCLE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        Start_tijd_programma = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "EVACUATION", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        Evacuation = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "DEBUT TEST DE VIDE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        start_tijd_vacuum = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "FIN TEST DE VIDE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        stop_tijd_vacuum = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Durée test de vide", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        duur_vacuum = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "FIN DE CYCLE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        stop_tijd_programma = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Résultat:", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        status_naam = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "isHold=", Index);
                        status_value = GetSingleValue(buffer, Index);

                        // De waardes afprinten
                        // ---------------------
                        Console.Write("Type machine : ");
                        Console.WriteLine(Machinetype);
                        Console.Write("Cyclus nummer: ");
                        Console.WriteLine(cyclus_nummer);
                        Console.Write("Wie heeft de cyclus gestart : ");
                        Console.WriteLine(cyclus_user_start);
                        Console.Write("Wie heeft de cyclus gestopt : ");
                        Console.WriteLine(cyclus_user_stop);
                        Console.Write("Het programma nummer: ");
                        Console.WriteLine(programma);
                        Console.Write("Start tijd van het programma :");
                        Console.WriteLine(Start_tijd_programma);
                        Console.Write("Autoclaaf start: ");
                        Console.WriteLine(Evacuation);
                        Console.Write("Start van de vacuumtest : ");
                        Console.WriteLine(start_tijd_vacuum);
                        Console.Write("stop van de vacuumtest : ");
                        Console.WriteLine(stop_tijd_vacuum);
                        Console.Write("Vacuumtest lengte : ");
                        Console.WriteLine(duur_vacuum);
                        Console.Write("Stop tijd van het programma : ");
                        Console.WriteLine(stop_tijd_programma);
                        Console.Write("Het resultaat van de cyclus : ");
                        Console.Write(status_naam);
                        Console.Write("  Is gelukt : ");
                        Console.WriteLine(status_value);

                        // De Data ophalen en afprinten
                        // -------------------
                        Index = FindIndex(buffer, "MMM_SelectomatPLFormular.sensorID:0 NL", Index);
                        Index = FindIndex(buffer, "<Value>", Index);
                        aantal_tijd = GetAmountOfValues(buffer, Index);
                        waardes_tijd = GetArrayValues(buffer, aantal_tijd, Index);
                        Index = FindIndex(buffer, "MMM_SelectomatPLFormular.sensorID:1 NL", Index);
                        Index = FindIndex(buffer, "<Value>", Index);
                        aantal_druk = GetAmountOfValues(buffer, Index);
                        waardes_druk = GetArrayValues(buffer, aantal_druk, Index);
                        Index = FindIndex(buffer, "MMM_SelectomatPLFormular.sensorID:2 NL", Index);
                        Index = FindIndex(buffer, "<Value>", Index);
                        aantal_temp = GetAmountOfValues(buffer, Index);
                        waardes_temp = GetArrayValues(buffer, aantal_temp, Index);

                    }
                    else if (Vacuumtest[1] == 'f')
                    {
                        // de  batchsummary van een cyclus bestand
                        Console.WriteLine("Het is geen vacuumtest bestand");

                        // de  batchsummary van een bestand van een normale cyclus
                        // De waardes ophalen
                        // -------------------
                        Index = FindIndex(buffer, "MachineType=", Index);
                        Machinetype = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Numéro de charge", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        cyclus_nummer = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Cycle démarré par", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        cyclus_user_start = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Cycle terminé par", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        cyclus_user_stop = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Numéro de programme", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        programma = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "DEBUT DU CYCLE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        Start_tijd_programma = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "EVACUATION", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        Evacuation = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "DEBUT PHASE DE PLATEAU", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        start_tijd_fase = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "FIN PHASE DE PLATEAU", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        stop_tijd_fase = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Durée phase de plateau", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        duur_fase = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "FIN SECHAGE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        stop_tijd_chauffage = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "FIN DE CYCLE", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        stop_tijd_programma = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "Résultat:", Index);
                        Index = FindIndex(buffer, "Value=", Index);
                        status_naam = GetSingleValue(buffer, Index);
                        Index = FindIndex(buffer, "isHold=", Index);
                        status_value = GetSingleValue(buffer, Index);

                        // De waardes afprinten
                        // ---------------------
                        Console.Write("Type machine : ");
                        Console.WriteLine(Machinetype);
                        Console.Write("Cyclus nummer: ");
                        Console.WriteLine(cyclus_nummer);
                        Console.Write("Wie heeft de cyclus gestart : ");
                        Console.WriteLine(cyclus_user_start);
                        Console.Write("Wie heeft de cyclus gestopt : ");
                        Console.WriteLine(cyclus_user_stop);
                        Console.Write("Het programma nummer: ");
                        Console.WriteLine(programma);
                        Console.Write("Start tijd van het programma :");
                        Console.WriteLine(Start_tijd_programma);
                        Console.Write("Autoclaaf start: ");
                        Console.WriteLine(Evacuation);
                        Console.Write("Start van de vacuumtest : ");
                        Console.WriteLine(start_tijd_fase);
                        Console.Write("stop van de vacuumtest : ");
                        Console.WriteLine(stop_tijd_fase);
                        Console.Write("Vacuumtest lengte : ");
                        Console.WriteLine(duur_fase);
                        Console.Write("stpo van de chauffage : ");
                        Console.WriteLine(stop_tijd_chauffage);
                        Console.Write("Stop tijd van het programma : ");
                        Console.WriteLine(stop_tijd_programma);
                        Console.Write("Het resultaat van de cyclus : ");
                        Console.Write(status_naam);
                        Console.Write("  Is gelukt : ");
                        Console.WriteLine(status_value);

                        // De Data ophalen en afprinten
                        // -------------------
                        Index = FindIndex(buffer, "MMM_SelectomatPLFormular.sensorID:0 NL", Index);
                        Index = FindIndex(buffer, "<Value>", Index);
                        aantal_tijd = GetAmountOfValues(buffer, Index);
                        waardes_tijd = GetArrayValues(buffer, aantal_tijd, Index);
                        Index = FindIndex(buffer, "MMM_SelectomatPLFormular.sensorID:1 NL", Index);
                        Index = FindIndex(buffer, "<Value>", Index);
                        aantal_druk = GetAmountOfValues(buffer, Index);
                        waardes_druk = GetArrayValues(buffer, aantal_druk, Index);
                        Index = FindIndex(buffer, "MMM_SelectomatPLFormular.sensorID:2 NL", Index);
                        Index = FindIndex(buffer, "<Value>", Index);
                        aantal_temp = GetAmountOfValues(buffer, Index);
                        waardes_temp = GetArrayValues(buffer, aantal_temp, Index);


                    }
                    else
                    {

                    }
                   
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(3000);
                }
            }
           
            Console.WriteLine("DONE");

          //File.Move(folderToWatchFor, folderTomoveTo);
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

