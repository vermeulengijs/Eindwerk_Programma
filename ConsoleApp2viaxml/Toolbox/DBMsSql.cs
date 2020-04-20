using System;
using System.Data.SqlClient;
using ConsoleAppMMM.JULIETClasses;

namespace ConsoleAppMMM.Toolbox
{
    public static class DBMsSql
    {
        public static SqlConnection GetConnection(string aDatabase, string aServer, string aUserID, string aPassword)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                InitialCatalog = aDatabase,
                DataSource = aServer,
                UserID = aUserID,
                Password = aPassword
            };
            SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            if (connection != null) { connection.Open(); }
            return connection;
        }

        public static bool GetMDNDX(SqlConnection aConnection, string aSchema, out long aMDNDX)
        {
            aMDNDX = 0;
            string sql = string.Format("SELECT NEXT VALUE FOR {0}.ORIS_MACHINEDATA_SEQ", aSchema);
            string mdNDXString = "";

            using (SqlCommand cmd = new SqlCommand(sql, aConnection))
            {
                mdNDXString = cmd.ExecuteScalar().ToString();
            }
            if (string.IsNullOrEmpty(mdNDXString)) { return false; }
            if (!long.TryParse(mdNDXString, out aMDNDX)) { return false; }
            return aMDNDX > 0;
        }

        public static bool Insert(JuMachineDataMMM aMachineDataMMM, SqlConnection aConnection, string aSchema)
        {
            string sql = string.Format("INSERT INTO {0}.ORIS_MACHINEDATAMMM (MDNDX, MACHINEID, MACHINENAME, COMPANY, PROGRAMREFERENCE, PROGRAMNAME, STARTEDBY, SOFTWAREVERSION, " +
                "LASTBDTEST, LASTBDTESTREFERENCE, LASTVACUUMTEST, LASTVACUUMTESTREFERENCE, CYCLEREFERENCE, CYCLESTARTED, CYCLEENDED, ISCYCLEOK, CYCLERESULT, HOLDTIME, " +
                "F0VALUE, SERIALNUMBER, COMPANY2, STOPPEDBY, NEXTMAINTENANCE, ISVACUUMTEST, AIRDETECTORTEMP, AIRDETECTORPRESENT, F0VALUEPRESENT, SENSORCOUNT, " +
                "PRESSUREDIFFERENCE, PRESSUREDIFFERENCE2,  PRESSUREDIFFERENCEALLOWED, AIRREMOVALCOUNT, AIRREMOVALSTART, PREPHASEPRESSUREMIN, PREPHASEPRESSUREMAX, WAITINGTIME, " +
                "HOLDTIMESTART, HOLDTIMESTARTPRESSURE, HOLDTIMESTARTTEMP, HOLDTIMESTARTAIRDETECTOR, HOLDTIMEEND, HOLDTIMEENDPRESSURE, HOLDTIMEENDTEMP, HOLDTIMEENDAIRDETECTOR, " +
                "DRYINGEND, DRYINGENDPRESSURE, STEAMSPYPRESENT, STEAMSPYRESULT, DTCREATED)" +
                "VALUES(@MDNDX, @MACHINEID, @MACHINENAME, @COMPANY, @PROGRAMREFERENCE, @PROGRAMNAME, @STARTEDBY, @SOFTWAREVERSION, " +
                "@LASTBDTEST, @LASTBDTESTREFERENCE, @LASTVACUUMTEST, @LASTVACUUMTESTREFERENCE, @CYCLEREFERENCE, @CYCLESTARTED, @CYCLEENDED, @ISCYCLEOK, @CYCLERESULT, @HOLDTIME, " +
                "@F0VALUE, @SERIALNUMBER, @COMPANY2, @STOPPEDBY, @NEXTMAINTENANCE, @ISVACUUMTEST, @AIRDETECTORTEMP, @AIRDETECTORPRESENT, @F0VALUEPRESENT, @SENSORCOUNT, " +
                "@PRESSUREDIFFERENCE, @PRESSUREDIFFERENCE2, @PRESSUREDIFFERENCEALLOWED, @AIRREMOVALCOUNT, @AIRREMOVALSTART, @PREPHASEPRESSUREMIN, @PREPHASEPRESSUREMAX, @WAITINGTIME, " +
                "@HOLDTIMESTART, @HOLDTIMESTARTPRESSURE, @HOLDTIMESTARTTEMP, @HOLDTIMESTARTAIRDETECTOR, @HOLDTIMEEND, @HOLDTIMEENDPRESSURE, @HOLDTIMEENDTEMP, @HOLDTIMEENDAIRDETECTOR, " +
                "@DRYINGEND, @DRYINGENDPRESSURE, @STEAMSPYPRESENT, @STEAMSPYRESULT, @DTCREATED)", aSchema);
            int rowCount = 0;
            
            using(SqlCommand cmd = new SqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MDNDX", aMachineDataMMM.MDNDX);
                cmd.Parameters.AddWithValue("@MACHINEID", aMachineDataMMM.MachineID);
                cmd.Parameters.AddWithValue("@MACHINENAME", aMachineDataMMM.MachineName);
                cmd.Parameters.AddWithValue("@COMPANY", aMachineDataMMM.Company);
                cmd.Parameters.AddWithValue("@PROGRAMREFERENCE", aMachineDataMMM.ProgramReference);
                cmd.Parameters.AddWithValue("@PROGRAMNAME", aMachineDataMMM.ProgramName);
                cmd.Parameters.AddWithValue("@STARTEDBY", aMachineDataMMM.StartedBy);
                cmd.Parameters.AddWithValue("@SOFTWAREVERSION", aMachineDataMMM.SoftwareVersion);
                cmd.Parameters.AddWithValue("@LASTBDTEST", aMachineDataMMM.LastBDTest);
                cmd.Parameters.AddWithValue("@LASTBDTESTREFERENCE", aMachineDataMMM.LastBDTestReference);
                cmd.Parameters.AddWithValue("@LASTVACUUMTEST", aMachineDataMMM.LastVacuumTest);
                cmd.Parameters.AddWithValue("@LASTVACUUMTESTREFERENCE", aMachineDataMMM.LastVacuumTestReference);
                cmd.Parameters.AddWithValue("@CYCLEREFERENCE", aMachineDataMMM.CycleReference);
                cmd.Parameters.AddWithValue("@CYCLESTARTED", aMachineDataMMM.CycleStarted);
                cmd.Parameters.AddWithValue("@CYCLEENDED", aMachineDataMMM.CycleEnded);
                cmd.Parameters.AddWithValue("@ISCYCLEOK", Conversions.BoolToString(aMachineDataMMM.IsCycleOk));
                cmd.Parameters.AddWithValue("@CYCLERESULT", aMachineDataMMM.CycleResult);
                cmd.Parameters.AddWithValue("@HOLDTIME", aMachineDataMMM.HoldTime);
                cmd.Parameters.AddWithValue("@F0VALUE", aMachineDataMMM.F0Value);
                cmd.Parameters.AddWithValue("@SERIALNUMBER", aMachineDataMMM.SerialNumber);
                cmd.Parameters.AddWithValue("@COMPANY2", aMachineDataMMM.Company2);
                cmd.Parameters.AddWithValue("@STOPPEDBY", aMachineDataMMM.StoppedBy);
                cmd.Parameters.AddWithValue("@NEXTMAINTENANCE", aMachineDataMMM.NextMaintenance);
                cmd.Parameters.AddWithValue("@ISVACUUMTEST", Conversions.BoolToString(aMachineDataMMM.IsVacuumTest));
                cmd.Parameters.AddWithValue("@AIRDETECTORTEMP", aMachineDataMMM.AirDetectorTemp);
                cmd.Parameters.AddWithValue("@AIRDETECTORPRESENT", Conversions.BoolToString(aMachineDataMMM.AirDetectorPresent));
                cmd.Parameters.AddWithValue("@F0VALUEPRESENT", Conversions.BoolToString(aMachineDataMMM.F0ValuePresent));
                cmd.Parameters.AddWithValue("@SENSORCOUNT", aMachineDataMMM.SensorCount);
                cmd.Parameters.AddWithValue("@PRESSUREDIFFERENCE", aMachineDataMMM.PressureDifference);
                cmd.Parameters.AddWithValue("@PRESSUREDIFFERENCE2", aMachineDataMMM.PressureDifference2);
                cmd.Parameters.AddWithValue("@PRESSUREDIFFERENCEALLOWED", aMachineDataMMM.PressureDifferenceAllowed);
                cmd.Parameters.AddWithValue("@AIRREMOVALCOUNT", aMachineDataMMM.AirRemovalCount);
                cmd.Parameters.AddWithValue("@AIRREMOVALSTART", aMachineDataMMM.AirRemovalStart);
                cmd.Parameters.AddWithValue("@PREPHASEPRESSUREMIN", aMachineDataMMM.PrephasePressureMin);
                cmd.Parameters.AddWithValue("@PREPHASEPRESSUREMAX", aMachineDataMMM.PrephasePressureMax);
                cmd.Parameters.AddWithValue("@WAITINGTIME", aMachineDataMMM.WaitingTime);
                cmd.Parameters.AddWithValue("@HOLDTIMESTART", aMachineDataMMM.HoldTimeStart);
                cmd.Parameters.AddWithValue("@HOLDTIMESTARTPRESSURE", aMachineDataMMM.HoldTimeStartPressure);
                cmd.Parameters.AddWithValue("@HOLDTIMESTARTTEMP", aMachineDataMMM.HoldTimeStartTemp);
                cmd.Parameters.AddWithValue("@HOLDTIMESTARTAIRDETECTOR", aMachineDataMMM.HoldTimeStartAirDetector);
                cmd.Parameters.AddWithValue("@HOLDTIMEEND", aMachineDataMMM.HoldTimeEnd);
                cmd.Parameters.AddWithValue("@HOLDTIMEENDPRESSURE", aMachineDataMMM.HoldTimeEndPressure);
                cmd.Parameters.AddWithValue("@HOLDTIMEENDTEMP", aMachineDataMMM.HoldTimeEndTemp);
                cmd.Parameters.AddWithValue("@HOLDTIMEENDAIRDETECTOR", aMachineDataMMM.HoldTimeEndAirDetector);
                cmd.Parameters.AddWithValue("@DRYINGEND", aMachineDataMMM.DryingEnd);
                cmd.Parameters.AddWithValue("@DRYINGENDPRESSURE", aMachineDataMMM.DryingEndPressure);
                cmd.Parameters.AddWithValue("@STEAMSPYPRESENT", Conversions.BoolToString(aMachineDataMMM.SteamSpyPresent));
                cmd.Parameters.AddWithValue("@STEAMSPYRESULT", Conversions.BoolToString(aMachineDataMMM.SteamSpyResult));
                cmd.Parameters.AddWithValue("@DTCREATED", DateTime.Now);

                rowCount = cmd.ExecuteNonQuery();
            }
            return rowCount == 1;
        }

        public static bool Insert(JuMachineSensor aMachineSensor, SqlConnection aConnection, string aSchema)
        {
            string sql = string.Format("INSERT INTO {0}.ORIS_MACHINESENSOR (MSNDX, MDNDX, SENSORID, CAPTION, SENSORTYPE, SENSORUNIT, MINVALUE, MAXVALUE, DTCREATED) " +
                "VALUES(NEXT VALUE FOR {0}.ORIS_MACHINESENSOR_SEQ, @MDNDX, @SENSORID, @CAPTION, @SENSORTYPE, @SENSORUNIT, @MINVALUE, @MAXVALUE, @DTCREATED)", aSchema);
            int rowCount = 0;

            using (SqlCommand cmd = new SqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MDNDX", aMachineSensor.MDNDX);
                cmd.Parameters.AddWithValue("@SENSORID", aMachineSensor.SensorID);
                cmd.Parameters.AddWithValue("@CAPTION", aMachineSensor.Caption);
                cmd.Parameters.AddWithValue("@SENSORTYPE", (int)aMachineSensor.SensorType);
                cmd.Parameters.AddWithValue("@SENSORUNIT", (int)aMachineSensor.SensorUnit);
                cmd.Parameters.AddWithValue("@MINVALUE", aMachineSensor.MinValue);
                cmd.Parameters.AddWithValue("@MAXVALUE", aMachineSensor.MaxValue);
                cmd.Parameters.AddWithValue("@DTCREATED", DateTime.Now);

                rowCount = cmd.ExecuteNonQuery();
            }
            return rowCount == 1;
        }

        public static bool Insert(JuMachineSensorValue aMachineSensorValue, SqlConnection aConnection, string aSchema)
        {
            string sql = string.Format("INSERT INTO {0}.ORIS_MACHINESENSORVALUE (MSVNDX, MDNDX, DTARGUMENT, SENSOR1, SENSOR2, SENSOR3, SENSOR4, SENSOR5, DTCREATED) " +
               "VALUES(NEXT VALUE FOR {0}.ORIS_MACHINESENSORVALUE_SEQ, @MDNDX, @DTARGUMENT, @SENSOR1, @SENSOR2, @SENSOR3, @SENSOR4, @SENSOR5, @DTCREATED)", aSchema);
            int rowCount = 0;

            using (SqlCommand cmd = new SqlCommand(sql, aConnection))
            {
                cmd.Parameters.AddWithValue("@MDNDX", aMachineSensorValue.MDNDX);
                cmd.Parameters.AddWithValue("@DTARGUMENT", aMachineSensorValue.DTArgument);
                cmd.Parameters.AddWithValue("@SENSOR1", aMachineSensorValue.Sensor1);
                cmd.Parameters.AddWithValue("@SENSOR2", aMachineSensorValue.Sensor2);
                cmd.Parameters.AddWithValue("@SENSOR3", aMachineSensorValue.Sensor3);
                cmd.Parameters.AddWithValue("@SENSOR4", aMachineSensorValue.Sensor4);
                cmd.Parameters.AddWithValue("@SENSOR5", aMachineSensorValue.Sensor5);
                cmd.Parameters.AddWithValue("@DTCREATED", DateTime.Now);

                rowCount = cmd.ExecuteNonQuery();
            }
            return rowCount == 1;
        }
    }
}
