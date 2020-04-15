using System;
using System.Collections.Generic;
using ConsoleApp1.Toolbox;

namespace ConsoleAppMMM.JULIETClasses
{
    public abstract class JuMachineData
    {
        public long MDNDX { get; }
        public string MachineID { get; protected set; } = "";
        public string MachineName { get; protected set; } = "";
        public string Company { get; protected set; } = "";
        public string ProgramReference { get; protected set; } = "";
        public string ProgramName { get; protected set; } = "";
        public string StartedBy { get; protected set; } = "";
        public string SoftwareVersion { get; protected set; } = "";
        public DateTime LastBDTest { get; protected set; } = DateTime.MinValue;
        public string LastBDTestReference { get; protected set; } = "";
        public DateTime LastVacuumTest { get; protected set; } = DateTime.MinValue;
        public string LastVacuumTestReference { get; protected set; } = "";
        public string CycleReference { get; protected set; } = "";
        public DateTime CycleStarted { get; protected set; } = DateTime.MinValue;
        public DateTime CycleEnded { get; protected set; } = DateTime.MinValue;
        public bool IsCycleOk { get; protected set; } = false;
        public string CycleResult { get; protected set; } = "";
        public int HoldTime { get; protected set; } = 0; // unit: seconds
        public double F0Value { get; protected set; } = 0.0;

        public abstract JuMachineInterfaceType MachineInterfaceType { get; }
        public abstract string Manufacturer { get; }

        public List<JuMachineSensor> MachineSensors { get; protected set; } = new List<JuMachineSensor>();
        public List<JuMachineSensorValue> MachineSensorValues { get; protected set; } = new List<JuMachineSensorValue>();

        public JuMachineData(long aMDNDX)
        {
            MDNDX = aMDNDX;
        }

        public int MachineInterfaceTypeAsInt => (int)MachineInterfaceType;

        public abstract bool LoadFromFile(string aFileFullPath);
    }
}
