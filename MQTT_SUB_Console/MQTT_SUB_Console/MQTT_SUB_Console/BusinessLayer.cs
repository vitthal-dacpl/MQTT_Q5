
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_SUB_Console
{
    public class BusinessLayer
    {
        dbLayer dbl = new dbLayer();

        public int Insert_Weldlog(object Name, object WeldTimer, object BIWNo, object iActual2,
            object ProgramNo, object iDemand2,
             object weldTimeActualValue, object resistanceActualValue, object CurrentCurve,
            object VoltageCurve, object ForceCurve, object Timestamp)
        {
            try
            {
                var listParas = new List<SqlParameter>()
            {
             new SqlParameter("@Name", Name),
             new SqlParameter("@WeldTimer", WeldTimer),
             new SqlParameter("@BIWNo", BIWNo),
             new SqlParameter("@iActual2", iActual2),
             new SqlParameter("@ProgramNo", ProgramNo),
             new SqlParameter("@iDemand2", iDemand2),
             new SqlParameter("@weldTimeActualValue", weldTimeActualValue),
             new SqlParameter("@resistanceActualValue", resistanceActualValue),
             new SqlParameter("@CurrentCurve", CurrentCurve),
             new SqlParameter("@VoltageCurve", VoltageCurve),
             new SqlParameter("@ForceCurve", ForceCurve),
             new SqlParameter("@Timestamp", Timestamp)
            };
                return dbl.ExecSqlNonQuery("SP_Insert_Weldlog", CommandType.StoredProcedure, listParas);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int Insert_ProgParameter(object Name, object WeldTimer, object subIndex,
            object SP_HOLD_MS, object SP_SQZ_MS, object SP_IMPULS_CNT, object Timestamp)
        {
            try
            {
                var listParas = new List<SqlParameter>()
            {
             new SqlParameter("@Name", Name),
             new SqlParameter("@WeldTimer", WeldTimer),
             new SqlParameter("@subIndex", subIndex),
             new SqlParameter("@SP_HOLD_MS", SP_HOLD_MS),
             new SqlParameter("@SP_SQZ_MS", SP_SQZ_MS),
             new SqlParameter("@SP_IMPULS_CNT", SP_IMPULS_CNT),
             new SqlParameter("@Timestamp", Timestamp)
            };
                return dbl.ExecSqlNonQuery("SP_Insert_ProgParameter", CommandType.StoredProcedure, listParas);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public int Insert_ErrorLog(object EventName, object Message, object StackTrace)
        {
            try
            {
                var listParas = new List<SqlParameter>()
            {

             new SqlParameter("@EventName", EventName),
             new SqlParameter("@Message", Message),
             new SqlParameter("@StackTrace", StackTrace)

            };
                return dbl.ExecSqlNonQuery("SP_Insert_ErrorLog", CommandType.StoredProcedure, listParas);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public int Isalive(object EM_Id, object IsAlive)
        //{
        //    try
        //    {
        //        var listParas = new List<SqlParameter>()
        //    {
        //     new SqlParameter("@em_id", EM_Id),
        //     new SqlParameter("@IsAlive", IsAlive)

        //    };
        //        return dbl.ExecSqlNonQuery("sp_Alive", CommandType.StoredProcedure, listParas);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public int Ins_tbl_Log(object LogNm)
        //{
        //    try
        //    {
        //        var listParas = new List<SqlParameter>()
        //    {
        //     new SqlParameter("@LogNm", LogNm)

        //    };
        //        return dbl.ExecSqlNonQuery("sp_tbl_log", CommandType.StoredProcedure, listParas);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //public DataSet GetAllEM()
        //{

        //    return dbl.ExecSqlDataSet("sp_GetAllEM", CommandType.StoredProcedure);

        //}

    }
}

