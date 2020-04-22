using System;

namespace ConsoleAppMMM.JULIETClasses
{
    public class JuMachineSensorValue
    {
        public long MDNDX { get; }
        public DateTime DTArgument { get; }
        public double Sensor1 { get; private set; } = 0.0;
        public double Sensor2 { get; private set; } = 0.0;
        public double Sensor3 { get; private set; } = 0.0;
        public double Sensor4 { get; private set; } = 0.0;
        public double Sensor5 { get; private set; } = 0.0;

        public JuMachineSensorValue(
           long aMDNDX,
           DateTime aDTArgument)
        {
            MDNDX = aMDNDX;
            DTArgument = aDTArgument;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    default: return 0.0;
                    case 1: return Sensor1;
                    case 2: return Sensor2;
                    case 3: return Sensor3;
                    case 4: return Sensor4;
                    case 5: return Sensor5;
                }
            }
            set
            {
                switch (index)
                {
                    default: break;
                    case 1: Sensor1 = value; break;
                    case 2: Sensor2 = value; break;
                    case 3: Sensor3 = value; break;
                    case 4: Sensor4 = value; break;
                    case 5: Sensor5 = value; break;
                }
            }
        }
    }
}
