using System;
using System.Globalization;

namespace ConsoleApp1.Toolbox
{
    public enum JuMachineInterfaceType
    {
        NotDefined = 0,
        Belimed = 1,
        MMM = 2
    }

    public enum JuSensorType
    {
        NotDefined = 0,
        Integer = 1,
        Double = 2,
        String = 3,
        Time = 4
    }

    public enum JuSensorUnit
    {
        NotDefined = 0,
        DegreesCelsius = 1, // °C
        MBar = 2, // mbar
        MBarA = 3, // mbar a
        Min = 4 // min
    }

    public static class Conversions
    {
        public static bool StringToBool(string aBoolString)
        {
            if (string.IsNullOrEmpty(aBoolString)) { return false; }
            aBoolString = aBoolString.ToLower();
            return aBoolString.Equals("true");
        }

        // DateTime format YYYY-MM-DDTHH:mm:ssZ
        public static DateTime StringToDateTime(string aDateTimeString)
        {
            if (string.IsNullOrEmpty(aDateTimeString)) { return DateTime.MinValue; }
            if (aDateTimeString.Length < 24) { return DateTime.MinValue; }
            if (DateTime.TryParse(aDateTimeString, out DateTime result)) { return result; }
            return DateTime.MinValue;
        }

        // Time format HH:mm:ss
        public static DateTime StringToDateTime(string aTimeString, DateTime aDate)
        {
            if (string.IsNullOrEmpty(aTimeString)) { return DateTime.MinValue; }
            if (aTimeString.Length < 8) { return DateTime.MinValue; }
            if (aDate == DateTime.MinValue || aDate == DateTime.MaxValue) { return DateTime.MinValue; }
            if (!DateTime.TryParseExact(aTimeString, "HH:m:ss", null, DateTimeStyles.None, out DateTime time)) { return DateTime.MinValue; }
            return new DateTime(aDate.Year, aDate.Month, aDate.Day, time.Hour, time.Minute, time.Second);
        }

        public static JuSensorType StringToSensorType(string aSensorTypeAsString)
        {
            if (string.IsNullOrEmpty(aSensorTypeAsString)) { return JuSensorType.NotDefined; }
            switch (aSensorTypeAsString.ToLower())
            {
                default:
                    return JuSensorType.NotDefined;
                case "integer":
                    return JuSensorType.Integer;
                case "double":
                    return JuSensorType.Double;
                case "string":
                    return JuSensorType.String;
                case "time":
                    return JuSensorType.Time;
            }
        }

        public static JuSensorUnit StringToSensorUnit(string aSensorUnitAsString)
        {
            if (string.IsNullOrEmpty(aSensorUnitAsString)) { return JuSensorUnit.NotDefined; }
            switch (aSensorUnitAsString.ToLower())
            {
                default:
                    return JuSensorUnit.NotDefined;
                case "°c":
                    return JuSensorUnit.DegreesCelsius;
                case "mbar":
                    return JuSensorUnit.MBar;
                case "mbar a":
                    return JuSensorUnit.MBarA;
                case "min":
                    return JuSensorUnit.Min;
            }
        }
    }
}
