using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppMMM.ORISConsts;

namespace ConsoleAppMMM.JULIETClasses
{
    public class JuMachineDataMMM : JuMachineData
    {
        public string Language { get; private set; } = "";
        public string SerialNumber { get; private set; } = "";
        public string Company2 { get; private set; } = "";
        public string StoppedBy { get; private set; } = "";
        public long NextMaintenance { get; private set; } = 0;
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

        public override bool LoadFromFile()
        {
            throw new NotImplementedException();
        }
    }
}
