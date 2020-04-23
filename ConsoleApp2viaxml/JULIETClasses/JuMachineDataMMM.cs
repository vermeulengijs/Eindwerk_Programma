using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ConsoleAppMMM.Toolbox;

namespace ConsoleAppMMM.JULIETClasses
{
    public class JuMachineDataMMM : JuMachineData
    {
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

            // TODO: "deltadruck2" - "evakkamdrmin"

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

            // TODO: "fabriknr" -  "res"

            if (!LoadFromEntries("sensorCount", 0, aEntryItems, out int sensorCount)) { return false; }
            SensorCount = sensorCount;

            // TODO: "steamSpyPresent" - "SW_VERSION"

            if (!LoadFromEntries("START_AIR_REMOVAL", CycleStarted, aEntryItems, out DateTime airRemovelStart)) { return false; }
            AirRemovalStart = airRemovelStart;

            // TODO: "HOLD_TIME_START" - "RESULT"

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
                    return double.TryParse(aEntryItems.Single(i => i.Key == aKey).Value, out aValue);
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
                            if (!double.TryParse(valuesElement.ElementAt(i).Attribute("MIN_VALUE").Value, out minValue)) { return false; }
                            if (!double.TryParse(valuesElement.ElementAt(i).Attribute("MAX_VALUE").Value, out maxValue)) { return false; }
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
                        if (!double.TryParse(valueElements.ElementAt(i).Value, out double value)) { return false; }
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
