using System;

namespace ConsoleAppMMM.JULIETClasses
{
    public class JuMachineSensorValue
    {
        public long MDNDX { get; }
        public DateTime DTArgument { get; }
        public double Sensor1 { get; }
        public double Sensor2 { get; }
        public double Sensor3 { get; }
        public double Sensor4 { get; }
        public double Sensor5 { get; }

        public JuMachineSensorValue(
           long aMDNDX,
           DateTime aDTArgument,
           double aSensor1,
           double aSensor2,
           double aSensor3,
           double aSensor4,
           double aSensor5)
        {
            MDNDX = aMDNDX;
            DTArgument = aDTArgument;
            Sensor1 = aSensor1;
            Sensor2 = aSensor2;
            Sensor3 = aSensor3;
            Sensor4 = aSensor4;
            Sensor5 = aSensor5;
        }

        public JuMachineSensorValue(
          long aMDNDX,
          DateTime aDTArgument,
          double aSensor1,
          double aSensor2) 
            : this(aMDNDX, aDTArgument, aSensor1, aSensor2, 0, 0, 0)
        { }
    }
}
