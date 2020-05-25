using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ConsoleAppMMM.Toolbox;

namespace ConsoleAppMMM.JULIETClasses
{
    public class JuMachineDataMMM : JuMachineData
    {
        private const string RegExWhiteSpace = @"\s+";
        public string SerialNumber { get; private set; } = "";
        public string Company2 { get; private set; } = "";
        public string StoppedBy { get; private set; } = "";
        public int NextMaintenance { get; private set; } = 0;
        public bool IsVacuumTest { get; private set; } = false;
        public double AirDetectorTemp { get; private set; } = 0.0; // unit: degrees C
        public bool AirDetectorPresent { get; private set; } = false;
        public bool F0ValuePresent { get; private set; } = false;
        public int SensorCount { get; private set; } = 0;
        public int PressureDifference { get; private set; } = 0; // unit: mbar
        public int PressureDifference2 { get; private set; } = 0; // unit: mbar
        public int PressureDifferenceAllowed { get; private set; } = 0; // unit: mbar
        public int AirRemovalCount { get; private set; } = 0;
        public DateTime AirRemovalStart { get; private set; } = DateTime.MinValue;
        public int PrephasePressureMin { get; private set; } = 0; // unit: mbar
        public int PrephasePressureMax { get; private set; } = 0; // unit: mbar
        public int WaitingTime { get; private set; } = 0; // unit: seconds
        public DateTime HoldTimeStart { get; private set; } = DateTime.MinValue;
        public int HoldTimeStartPressure { get; private set; } = 0; // unit: mbar
        public double HoldTimeStartTemp { get; private set; } = 0.0; // unit: degrees C
        public double HoldTimeStartAirDetector { get; private set; } = 0.0; // unit degrees C
        public DateTime HoldTimeEnd { get; private set; } = DateTime.MinValue;
        public int HoldTimeEndPressure { get; private set; } = 0; // unit: mbar
        public double HoldTimeEndTemp { get; private set; } = 0.0; // unit: degrees C
        public double HoldTimeEndAirDetector { get; private set; } = 0.0; // unit degrees C
        public DateTime DryingEnd { get; private set; } = DateTime.MinValue;
        public int DryingEndPressure { get; private set; } = 0; // unit: mbar
        public bool SteamSpyPresent { get; private set; } = false;
        public bool SteamSpyResult { get; private set; } = false;

        public override JuMachineInterfaceType MachineInterfaceType => JuMachineInterfaceType.MMM;
        public override string Manufacturer => "MMM GmbH";

        public JuMachineDataMMM(long aMDNDX) : base(aMDNDX) { }

        public override bool LoadFromFile(string aFileFullPath)
        {
            if (!File.Exists(aFileFullPath)) { return false; }
            if (!Conversions.CheckFileName(aFileFullPath, MachineInterfaceType)) { return false; }
            
            try
            {
                XElement mmmSimSocket = XElement.Load(aFileFullPath);
                if (mmmSimSocket == null) { return false; }
                
                XElement formElement = mmmSimSocket.Element("Form");
                if (formElement != null)
                {
                    MachineName = formElement.Attribute("Caption").Value;
                    IsVacuumTest = Conversions.StringToBool(formElement.Attribute("IsVacuumTest").Value);
                }

                List<EntryItem> entryItems = LoadEntries(mmmSimSocket);
                if (!LoadCycleData(entryItems)) { return false; }
                if (!LoadMachineSensors(mmmSimSocket, entryItems)) { return false; }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("JuMachineDataMMM.LoadFromFile: " + aFileFullPath + ": " + e.Message);
            }
            return false;
        }

        private bool LoadCycleData(List<EntryItem> aEntryItems)
        {
            if ((aEntryItems == null) || (aEntryItems.Count == 0)) { return false; }
            
            if (!LoadFromEntries("airdetector", aEntryItems, out bool airDetectorPresent)) { return false; }
            AirDetectorPresent = airDetectorPresent;
            if (AirDetectorPresent)
            {
                if (!LoadFromEntries("airDetectorTemp", 2, aEntryItems, out double airDetectorTemp)) { return false; }
                AirDetectorTemp = airDetectorTemp;
            }
            else
            {
                AirDetectorTemp = 0.0;
            }
            if (!LoadFromEntries("chargende", aEntryItems, out DateTime cycleEnded)) { return false; }
            CycleEnded = cycleEnded;
            if (!LoadFromEntries("chargnr", aEntryItems, out string cycleReference)) { return false; }
            CycleReference = cycleReference;
            if (!LoadFromEntries("chargstart", aEntryItems, out DateTime cycleStarted)) { return false; }
            CycleStarted = cycleStarted;
            if (!LoadFromEntries("cycleCompletedBy", aEntryItems, out string stoppedBy)) { return false; }
            StoppedBy = stoppedBy;
            if (!LoadFromEntries("deltadruck", 4, aEntryItems, out int pressureDifference)) { return false; }
            PressureDifference = pressureDifference;
            if (!LoadFromEntries("deltadruck2", 4, aEntryItems, out int pressureDifference2)) { return false; }
            PressureDifference2 = pressureDifference2;
            if (!LoadFromEntries("deltadrucknominal", 4, aEntryItems, out int pressureDifferenceAllowed)) { return false; }
            PressureDifferenceAllowed = pressureDifferenceAllowed;
            if (!LoadFromEntries("evakanz", 0, aEntryItems, out int airRemovalCount)) { return false; }
            AirRemovalCount = airRemovalCount;
            if (!LoadFromEntries("evakkamdrmax", 4, aEntryItems, out int prephasePressureMax)) { return false; }
            PrephasePressureMax = prephasePressureMax;
            if (!LoadFromEntries("evakkamdrmin", 4, aEntryItems, out int prephasePressureMin)) { return false; }
            PrephasePressureMin = prephasePressureMin;
            if (!LoadFromEntries("f0ValuePresent", aEntryItems, out bool f0ValuePresent)) { return false; }
            F0ValuePresent = f0ValuePresent;
            if (F0ValuePresent)
            {
                if (!LoadFromEntries("f0Value", 0, aEntryItems, out double f0Value)) { return false; }
                F0Value = f0Value;
            }
            else
            {
                F0Value = 0.0;
            }
            if (!LoadFromEntries("fabriknr", aEntryItems, out string serialNumber)) { return false; }
            SerialNumber = serialNumber;
            if (!LoadFromEntries("firm1", aEntryItems, out string company)) { return false; }
            Company = company;
            if (!LoadFromEntries("firm2", aEntryItems, out string company2)) { return false; }
            Company2 = company2;
            if (!LoadFromEntries("lastbd", aEntryItems, out DateTime lastBDTest)) { return false; }
            LastBDTest = lastBDTest;
            if (!LoadFromEntries("lastbd_no", aEntryItems, out string lastBDTestReference)) { return false; }
            LastBDTestReference = lastBDTestReference;
            if (!LoadFromEntries("lastvak", aEntryItems, out DateTime lastVacuumTest)) { return false; }
            LastVacuumTest = lastVacuumTest;
            if (!LoadFromEntries("lastvak_no", aEntryItems, out string lastVacuumTestReference)) { return false; }
            LastVacuumTestReference = lastVacuumTestReference;
            if (!LoadFromEntries("nextwart", 0, aEntryItems, out int nextMaintenance)) { return false; }
            NextMaintenance = nextMaintenance;
            if (!LoadFromEntries("programNo", aEntryItems, out string programReference)) { return false; }
            ProgramReference = programReference;
            if (!LoadFromEntries("programmname", aEntryItems, out string programName)) { return false; }
            ProgramName = programName;
            if (!LoadFromEntries("res", aEntryItems, out bool isCycleOk)) { return false; }
            IsCycleOk = isCycleOk;
            if (!LoadFromEntries("sensorCount", 0, aEntryItems, out int sensorCount)) { return false; }
            SensorCount = sensorCount;
            if (!LoadFromEntries("steamSpyPresent", aEntryItems, out bool steamSpyPresent)) { return false; }
            SteamSpyPresent = steamSpyPresent;
            if (SteamSpyPresent)
            {
                if (!LoadFromEntries("steamSpyResult", aEntryItems, out bool steamSpyResult)) { return false; }
                SteamSpyResult = steamSpyResult;
            }
            else
            {
                SteamSpyResult = false;
            }
            if (!LoadFromEntries("sterinr", aEntryItems, out string machineID)) { return false; }
            MachineID = machineID;
            if (!LoadFromEntries("userID", aEntryItems, out string startedBy)) { return false; }
            StartedBy = startedBy;
            if (!LoadFromEntries("vakevakzeit", 0, aEntryItems, out int waitingTime)) { return false; }
            WaitingTime = waitingTime;
            if (!LoadFromEntries("SW_VERSION", aEntryItems, out string softwareVersion)) { return false; }
            SoftwareVersion = softwareVersion;
            if (!LoadFromEntries("START_AIR_REMOVAL", CycleStarted, aEntryItems, out DateTime airRemovelStart)) { return false; }
            AirRemovalStart = airRemovelStart;
            if (!LoadFromEntries("HOLD_TIME_START", CycleStarted, aEntryItems, out DateTime holdTimeStart)) { return false; }
            HoldTimeStart = holdTimeStart;
            if (!LoadFromEntries("HOLD_TIME_START_PRESSURE", 4, aEntryItems, out int holdTimeStartPressure)) { return false; }
            HoldTimeStartPressure = holdTimeStartPressure;
            if (!IsVacuumTest)
            {
                if (!LoadFromEntries("HOLD_TIME_START_TEMPERATURE", 2, aEntryItems, out double holdTimeStartTemp)) { return false; }
                HoldTimeStartTemp = holdTimeStartTemp;
                if (!LoadFromEntries("HOLD_TIME_START_AIRDETECTOR", 2, aEntryItems, out double holdTimeStartAirDetector)) { return false; }
                HoldTimeStartAirDetector = holdTimeStartAirDetector;
            }
            if (!LoadFromEntries("HOLD_TIME_END", CycleStarted, aEntryItems, out DateTime holdTimeEnd)) { return false; }
            HoldTimeEnd = holdTimeEnd;
            if (!LoadFromEntries("HOLD_TIME_END_PRESSURE", 4, aEntryItems, out int holdTimeEndPressure)) { return false; }
            HoldTimeEndPressure = holdTimeEndPressure;
            if (!IsVacuumTest)
            {
                if (!LoadFromEntries("HOLD_TIME_END_TEMPERATURE", 2, aEntryItems, out double holdTimeEndTemp)) { return false; }
                HoldTimeEndTemp = holdTimeEndTemp;
                if (!LoadFromEntries("HOLD_TIME_END_AIRDETECTOR", 2, aEntryItems, out double holdTimeEndAirDetector)) { return false; }
                HoldTimeEndAirDetector = holdTimeEndAirDetector;
            }
            if (!LoadFromEntries("HOLD_TIME_END_PRESSURE", aEntryItems, out int holdTime)) { return false; }
            HoldTime = holdTime;
            if (!IsVacuumTest)
            {
                if (!LoadFromEntries("DRYING_END", CycleStarted, aEntryItems, out DateTime dryingEnd)) { return false; }
                DryingEnd = dryingEnd;
                if (!LoadFromEntries("DRYING_END_PRESSURE", 4, aEntryItems, out int dryingEndPressure)) { return false; }
                DryingEndPressure = dryingEndPressure;
            }
            if (!LoadFromEntries("RESULT", aEntryItems, out string cycleResult)) { return false; }
            CycleResult = cycleResult;

            return true;
        }

        private List<EntryItem> LoadEntries(XElement aMMMSimSocket)
        {
            List<EntryItem> cycleDataEntries = new List<EntryItem>();
            // CycleData_Full
            XElement cycleDataFullElement = aMMMSimSocket.Element("CycleData_Full");
            if (cycleDataFullElement != null)
            {
                IEnumerable<XElement> EntryElements = cycleDataFullElement.Elements("Entry");
                if (EntryElements != null)
                {
                    foreach (XElement entryElement in EntryElements)
                    {
                        cycleDataEntries.Add(new EntryItem()
                        {
                            Key = entryElement.Attribute("Key").Value,
                            Value = entryElement.Attribute("Value").Value
                        });
                    }
                }
            }
            // CycleData_Formular
            XElement cycleDataFormularElement = aMMMSimSocket.Element("CycleData_Formular");
            if (cycleDataFormularElement != null)
            {
                IEnumerable<XElement> EntryElements = cycleDataFormularElement.Elements("Entry");
                if (EntryElements != null)
                {
                    foreach (XElement entryElement in EntryElements)
                    {
                        cycleDataEntries.Add(new EntryItem()
                        {
                            Key = entryElement.Attribute("DocuKey").Value,
                            Value = entryElement.Attribute("Value").Value
                        });
                    }
                }
            }
            return cycleDataEntries;
        }

        private bool LoadFromEntries(string aKey, List<EntryItem> aEntryItems, out bool aValue)
        {
            aValue = false;
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }
           
            if (aEntryItems.Any(i => i.Key == aKey))
            {
                aValue = Conversions.StringToBool(aEntryItems.Single(i => i.Key == aKey).Value);
                return true;
            }
            return false;
        }

        private bool LoadFromEntries(string aKey, List<EntryItem> aEntryItems, out string aValue)
        {
            aValue = "";
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }
            
            if (aEntryItems.Any(i => i.Key == aKey))
            {
                aValue = aEntryItems.Single(i => i.Key == aKey).Value;
                return true;
            }
            return false;
        }

        private bool LoadFromEntries(string aKey, List<EntryItem> aEntryItems, out DateTime aValue)
        {
            aValue = DateTime.MinValue;
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }
            
            if (aEntryItems.Any(i => i.Key == aKey))
            {
                aValue = Conversions.StringToDateTime(aEntryItems.Single(i => i.Key == aKey).Value);
                return true;
            }
            return false;
        }

        private bool LoadFromEntries(string aKey, DateTime aDate, List<EntryItem> aEntryItems, out DateTime aValue)
        {
            aValue = DateTime.MinValue;
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }
            
            if (aEntryItems.Any(i => i.Key == aKey))
            {
                aValue = Conversions.StringToDateTime(aEntryItems.Single(i => i.Key == aKey).Value, aDate);
                return true;
            }
            return false;
        }

        private bool LoadFromEntries(string aKey, List<EntryItem> aEntryItems, out int aValue)
        {
            aValue = 0;
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }

            if (aEntryItems.Any(i => i.Key == aKey))
            {
                aValue = Conversions.StringToSeconds(aEntryItems.Single(i => i.Key == aKey).Value);
                return true;
            }
            return false;
        }

        private bool LoadFromEntries(string aKey, int aToRemoveFromEnd, List<EntryItem> aEntryItems, out int aValue)
        {
            aValue = 0;
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }
           
            if (aEntryItems.Any(i => i.Key == aKey))
            {
                string valueStringComplete = aEntryItems.Single(i => i.Key == aKey).Value;
                if (!string.IsNullOrEmpty(valueStringComplete))
                {
                    string valueString = valueStringComplete.Substring(0, valueStringComplete.Length - aToRemoveFromEnd);
                    valueString = Regex.Replace(valueString, RegExWhiteSpace, "");
                    return int.TryParse(valueString, out aValue);
                }
                return true;
            }
            return false;
        }

        private bool LoadFromEntries(string aKey, int aToRemoveFromEnd, List<EntryItem> aEntryItems, out double aValue)
        {
            aValue = 0.0;
            if (string.IsNullOrEmpty(aKey) || (aEntryItems == null)) { return false; }
            
            if (aEntryItems.Any(i => i.Key == aKey))
            {
                string valueStringComplete = aEntryItems.Single(i => i.Key == aKey).Value;
                if (!string.IsNullOrEmpty(valueStringComplete))
                {
                    string valueString = valueStringComplete.Substring(0, valueStringComplete.Length - aToRemoveFromEnd);
                    valueString = Regex.Replace(valueString, RegExWhiteSpace, "");
                    valueString = valueString.Replace(',', '.');
                    return double.TryParse(valueString, out aValue);
                }
                return true;
            }
            return false;
        }

        private bool LoadMachineSensors(XElement aMMMSimSocket, List<EntryItem> aEntryItems)
        {
            if ((aMMMSimSocket == null) || (aEntryItems == null)) { return false; }
            if (SensorCount <= 0) { return false; }
            
            MachineSensors = new List<JuMachineSensor>();
            XElement sensorsElement = aMMMSimSocket.Element("Sensors");
            if (sensorsElement != null)
            {
                IEnumerable<XElement> valuesElement = sensorsElement.Elements("Values");
                if ((valuesElement != null) && (valuesElement.Count() == SensorCount + 1))
                {
                    for (int i = 0; i <= SensorCount; i++)
                    {
                        if (!LoadFromEntries("sensorID_" + i.ToString(), aEntryItems, out string caption) || (string.IsNullOrEmpty(caption))) { return false; }
                        JuSensorType sensorType = Conversions.StringToSensorType(valuesElement.ElementAt(i).Attribute("Type").Value);
                        JuSensorUnit sensorUnit = JuSensorUnit.NotDefined;
                        double minValue = 0.0;
                        double maxValue = 0.0;
                        if (i > 0)
                        {
                            sensorUnit = Conversions.StringToSensorUnit(valuesElement.ElementAt(i).Attribute("Unit").Value);
                            string minValueString = valuesElement.ElementAt(i).Attribute("MIN_VALUE").Value;
                            string maxValueString = valuesElement.ElementAt(i).Attribute("MAX_VALUE").Value;
                            minValueString = Regex.Replace(minValueString, RegExWhiteSpace, "");
                            minValueString = minValueString.Replace(',', '.');
                            maxValueString = Regex.Replace(maxValueString, RegExWhiteSpace, "");
                            maxValueString = maxValueString.Replace(',', '.');
                            if (!double.TryParse(minValueString, out minValue)) { return false; }
                            if (!double.TryParse(maxValueString, out maxValue)) { return false; }
                        }
                        MachineSensors.Add(new JuMachineSensor(MDNDX, i, caption, sensorType, sensorUnit, minValue, maxValue));
                        if(!LoadMachineSensorValues(i, valuesElement.ElementAt(i))) { return false; }
                    }
                }
            }
            return true;
        }

        private bool LoadMachineSensorValues(int aSensorID, XElement aValuesElement)
        {
            if ((aSensorID < 0) || (aSensorID > SensorCount)) { return false; }
            if (aValuesElement == null) { return false; }
            
            if (aSensorID == 0)
            {
                MachineSensorValues = new List<JuMachineSensorValue>();
            }
            IEnumerable<XElement> valueElements = aValuesElement.Elements("Value");
            if (valueElements != null)
            {
                int count = valueElements.Count();
                if ((aSensorID > 0) && (MachineSensorValues.Count != count)) { return false; }
                for (int i = 0; i < count; i++)
                {
                    if (aSensorID == 0)
                    {
                        DateTime dtArgument = Conversions.StringToDateTime(valueElements.ElementAt(i).Value);
                        if (dtArgument <= DateTime.MinValue) { return false; }
                        MachineSensorValues.Add(new JuMachineSensorValue(MDNDX, dtArgument));
                    }
                    else
                    {
                        string valueString = valueElements.ElementAt(i).Value;
                        valueString = Regex.Replace(valueString, RegExWhiteSpace, "");
                        valueString = valueString.Replace(',', '.');
                        if (!double.TryParse(valueString, out double value)) { return false; }
                        MachineSensorValues[i][aSensorID] = value;
                    }
                }
                return true;
            }
            return false;
        }

        class EntryItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
