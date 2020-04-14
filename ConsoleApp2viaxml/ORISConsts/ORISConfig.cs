using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppMMM.ORISConsts
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
        MBarA = 3 // mbar a
    }

    public static class ORISConfig
    {

    }
}
