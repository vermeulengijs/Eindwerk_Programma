using ConsoleAppMMM.Toolbox;

namespace ConsoleAppMMM.JULIETClasses
{
    public class JuMachineSensor
    {
        public long MDNDX { get; }
        public int SensorID { get; }
        public string Caption { get; }
        public JuSensorType SensorType { get; }
        public JuSensorUnit SensorUnit { get; }
        public double MinValue { get; }
        public double MaxValue { get; }

        public JuMachineSensor(
                long aMDNDX,
                int aSensorID,
                string aCaption,
                JuSensorType aSensorType,
                JuSensorUnit aSensorUnit,
                double aMinValue,
                double aMaxValue)
        {
            MDNDX = aMDNDX;
            SensorID = aSensorID;
            Caption = aCaption;
            SensorType = aSensorType;
            SensorUnit = aSensorUnit;
            MinValue = aMinValue;
            MaxValue = aMaxValue;
        }
    }
}
