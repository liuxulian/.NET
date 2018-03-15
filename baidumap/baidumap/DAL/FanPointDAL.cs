using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace baidumap.DAL
{
    public class FanPointDAL
    {
        /// <summary>
        /// 查询类型，值为风速或风向
        /// </summary>
        public enum Type
        {
            风速,
            风向
        }



        //从Web.config拿到数据库链接字符串，访问ZBHY数据库
        private static string ConnStr_SensorData = ConfigurationManager.ConnectionStrings["dbConnStr_SensorData"].ToString();

        /// <summary>
        /// 获取全国所有风机的数据，返回其对象的集合
        /// </summary>
        /// <returns>全国所有风机数据的集合</returns>
        public static List<FanPoint> GetAllPoints()
        {
            List<FanPoint> fpList = new List<FanPoint>();
            //获取各个公司的数据表名
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct BranchTableName from T_AllWindField");//distinct 去重
            if (table.Rows.Count <= 0)
            {
                return null;//没有公司
            }
            //创建一个数组，存放所有公司的风场表名
            string[] TableName = new string[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                TableName[i] = (string)table.Rows[i]["BranchTableName"];
            }

            //定义int数组，用于存储风场编号
            int[] windFieldNumbers;        
            //通过表名获得所有表的数据
            for (int i = 0; i < TableName.Length; i++)
            {
                //获取该表的风场编号
                DataTable tableWindFieldNumber = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                    "select distinct WindFieldNumber from "+TableName[i]);
                //创建风场编号缓存
                windFieldNumbers = new int[tableWindFieldNumber.Rows.Count];
                for (int m = 0; m < tableWindFieldNumber.Rows.Count; m++)
                {
                    //存储风场编号
                    windFieldNumbers[m] = (int)tableWindFieldNumber.Rows[m]["WindFieldNumber"];
                }                
                //获取每个风场的风机编号
                for (int n = 0; n < windFieldNumbers.Length; n++)
                {
                    //获取该风场的风机编号
                    DataTable tableFanNumber = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                        "select distinct FanNumber from "+TableName[i]
                        + " where FanNumber in (select FanNumber from " + TableName[i] + " where WindFieldNumber=@WindFieldNumber)",
                        new SqlParameter("@WindFieldNumber",windFieldNumbers[n]));
                    //获取该风场每台风机的数据
                    for (int k = 0; k < tableFanNumber.Rows.Count; k++)
                    {
                        int fanNumber = (int)tableFanNumber.Rows[k]["FanNumber"];
                        DataTable fanData = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                            "select * from " + TableName[i] + " where WindFieldNumber=@WindFieldNumber and FanNumber=@FanNumber and DateTime=(select MAX(DateTime) from " + TableName[i] + " where WindFieldNumber=@WindFieldNumber and FanNumber=@FanNumber)",
                            new SqlParameter("@WindFieldNumber", windFieldNumbers[n]),
                            new SqlParameter("@FanNumber",fanNumber));
                        //创建风机参数对象，存储数据
                        FanPoint wp = new FanPoint();
                        wp.CurrentTableName = (string)fanData.Rows[0]["CurrentTableName"];
                        wp.WindFieldNumber = (int)fanData.Rows[0]["WindFieldNumber"];
                        wp.WindFieldName = (string)fanData.Rows[0]["WindFieldName"];
                        wp.GPS_lng = (string)fanData.Rows[0]["GPS_lng"];
                        wp.GPS_lat = (string)fanData.Rows[0]["GPS_lat"];
                        wp.FanNumber = (int)fanData.Rows[0]["FanNumber"];
                        wp.WindSpeed = (double)fanData.Rows[0]["WindSpeed"];
                        wp.WindDirection = (double)fanData.Rows[0]["WindDirection"];
                        wp.TempIp = (string)fanData.Rows[0]["TempIp"];
                        wp.Elevation = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Elevation"]);
                        wp.Course = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Course"]);
                        wp.NavigationalSpeed = (string)SqlHelper.FromDBValue(fanData.Rows[0]["NavigationalSpeed"]);
                        wp.Temperature = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Temperature"]);
                        wp.Humidity = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Humidity"]);
                        wp.AirPressure = (string)SqlHelper.FromDBValue(fanData.Rows[0]["AirPressure"]);
                        fpList.Add(wp);
                    }

                    //更新 T_AllWindField 中各个风场的风机数量
                    try
                    {
                        //获得当前风场的风场名称
                        string windFieldName = (string)SqlHelper.ExecuteScalar(ConnStr_SensorData,
                            "select distinct WindFieldName from " + TableName[i] + " where WindFieldNumber=@WindFieldNumber",
                            new SqlParameter("@WindFieldNumber", windFieldNumbers[n]));
                        //更新风机数量字段
                        AllWindFieldDAL.UpdateFanNumber(tableFanNumber.Rows.Count, TableName[i], windFieldName);
                    }
                    catch 
                    { }
                    


                }
            }
            return fpList;
        }
        /// <summary>
        /// 获得单个风机的最新数据
        /// </summary>
        /// <param name="tableName">风机所在的数据库表名</param>
        /// <param name="windFieldName">风机所在的风场名称</param>
        /// <param name="fanNumber">风机编号</param>
        /// <returns>返回风机对象</returns>
        public static FanPoint GetSinglePoint(string tableName,string windFieldName,string fanNumber)
        {
            DataTable fanData = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
               "select * from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber and DateTime=(select MAX(DateTime) from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber)",
               new SqlParameter("@WindFieldName", windFieldName),
               new SqlParameter("@FanNumber", Convert.ToInt32(fanNumber)));
         
            if (fanData.Rows.Count <= 0)
            {
                //没查到数据
                return null;
            }
            else
            {
                //创建风机参数对象，存储数据
                FanPoint wp = new FanPoint();
                wp.CurrentTableName = (string)fanData.Rows[0]["CurrentTableName"];
                wp.WindFieldNumber = (int)fanData.Rows[0]["WindFieldNumber"];
                wp.WindFieldName = (string)fanData.Rows[0]["WindFieldName"];
                wp.GPS_lng = (string)fanData.Rows[0]["GPS_lng"];
                wp.GPS_lat = (string)fanData.Rows[0]["GPS_lat"];
                wp.FanNumber = (int)fanData.Rows[0]["FanNumber"];
                wp.WindSpeed = (double)fanData.Rows[0]["WindSpeed"];
                wp.WindDirection = (double)fanData.Rows[0]["WindDirection"];
                wp.TempIp = (string)fanData.Rows[0]["TempIp"];
                wp.Elevation = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Elevation"]);
                wp.Course = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Course"]);
                wp.NavigationalSpeed = (string)SqlHelper.FromDBValue(fanData.Rows[0]["NavigationalSpeed"]);
                wp.Temperature = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Temperature"]);
                wp.Humidity = (string)SqlHelper.FromDBValue(fanData.Rows[0]["Humidity"]);
                wp.AirPressure = (string)SqlHelper.FromDBValue(fanData.Rows[0]["AirPressure"]);
                return wp;
            }

        }

        /// <summary>
        /// 获得指定风机在指定时间内的所有数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="windFieldName"></param>
        /// <param name="fanNumber"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<FanPoint> GetDateOfSingleFanByDate(string tableName, string windFieldName, string fanNumber, DateTime? startTime, DateTime? endTime)
        {
            DataTable fanData = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
               "select * from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber and DateTime>=@startTime and DateTime<=@endTime order by DateTime",         
               new SqlParameter("@WindFieldName", windFieldName),
               new SqlParameter("@FanNumber", Convert.ToInt32(fanNumber)),
               new SqlParameter("@startTime", startTime),
               new SqlParameter("@endTime", endTime));

            if (fanData.Rows.Count <= 0)
            {
                //没查到数据
                return null;
            }
            else
            {
                //创建风机参数对象，存储数据
                List<FanPoint> fp = new List<FanPoint>();

                for (int i = 0; i < fanData.Rows.Count; i++)
                {
                    FanPoint fan = new FanPoint();
                    fan.CurrentTableName = (string)fanData.Rows[i]["CurrentTableName"];
                    fan.WindFieldNumber = (int)fanData.Rows[i]["WindFieldNumber"];
                    fan.WindFieldName = (string)fanData.Rows[i]["WindFieldName"];
                    fan.DateTime = (DateTime)fanData.Rows[i]["DateTime"];
                    fan.GPS_lng = (string)fanData.Rows[i]["GPS_lng"];
                    fan.GPS_lat = (string)fanData.Rows[i]["GPS_lat"];
                    fan.FanNumber = (int)fanData.Rows[i]["FanNumber"];
                    fan.WindSpeed = (double)fanData.Rows[i]["WindSpeed"];
                    fan.WindDirection = (double)fanData.Rows[i]["WindDirection"];
                    fan.TempIp = (string)fanData.Rows[i]["TempIp"];
                    fan.Elevation = (string)SqlHelper.FromDBValue(fanData.Rows[i]["Elevation"]);
                    fan.Course = (string)SqlHelper.FromDBValue(fanData.Rows[i]["Course"]);
                    fan.NavigationalSpeed = (string)SqlHelper.FromDBValue(fanData.Rows[i]["NavigationalSpeed"]);
                    fan.Temperature = (string)SqlHelper.FromDBValue(fanData.Rows[i]["Temperature"]);
                    fan.Humidity = (string)SqlHelper.FromDBValue(fanData.Rows[i]["Humidity"]);
                    fan.AirPressure = (string)SqlHelper.FromDBValue(fanData.Rows[i]["AirPressure"]);
                    fp.Add(fan);
                }
               
             
                return fp;
            }
        }

        /// <summary>
        /// 查询单一风机在指定时间内的风速或风向
        /// </summary>
        /// <param name="type">要查询的类型，风速还是风向</param>
        /// <param name="tableName">表名</param>
        /// <param name="windFieldName">风场名字</param>
        /// <param name="fanNumber">风机编号</param>
        /// <returns></returns>
        public static List<double> GetWindSpeedOrWindDirectionOfSinglePoint(Type type, string tableName, string windFieldName, string fanNumber, DateTime? startTime, DateTime? endTime)
        {
            //定义查询类型
            string searchType = string.Empty;
            if (type==Type.风速)
            {
                searchType = "WindSpeed";
            }
            else if(type==Type.风向)
            {
                searchType = "WindDirection";
            }

            DataTable dataTable = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
               "select " + searchType + " from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber and DateTime>=@startTime and DateTime<=@endTime order by DateTime",         
               new SqlParameter("@WindFieldName", windFieldName),
               new SqlParameter("@FanNumber", Convert.ToInt32(fanNumber)),
               new SqlParameter("@startTime", startTime),
               new SqlParameter("@endTime", endTime));

            if (dataTable.Rows.Count <= 0)
            {
                //没查到数据
                return null;
            }
            else
            {
                List<double> dataList = new List<double>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataList.Add(Convert.ToDouble(dataTable.Rows[i][searchType]));
                }
                return dataList;
            }
        }

        /// <summary>
        /// 查询单一风机在查询范围内的时间集合
        /// </summary>
        /// <param name="type">要查询的类型，风速还是风向</param>
        /// <param name="tableName">表名</param>
        /// <param name="windFieldName">风场名字</param>
        /// <param name="fanNumber">风机编号</param>
        /// <returns></returns>
        public static List<DateTime?> GetDateTimeCollectionOfSingleFan(string tableName, string windFieldName, string fanNumber, DateTime? startTime, DateTime? endTime)
        {
            DataTable dataTable = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
               "select DateTime from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber and DateTime>=@startTime and DateTime<=@endTime order by DateTime",
               new SqlParameter("@WindFieldName", windFieldName),
               new SqlParameter("@FanNumber", Convert.ToInt32(fanNumber)),
               new SqlParameter("@startTime", startTime),
               new SqlParameter("@endTime", endTime));

            if (dataTable.Rows.Count <= 0)
            {
                //没查到数据
                return null;
            }
            else
            {
                List<DateTime?> dataList = new List<DateTime?>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataList.Add((DateTime?)SqlHelper.FromDBValue(dataTable.Rows[i]["DateTime"]));
                }
                return dataList;
            }
        }


        /// <summary>
        /// 查询单一风机的风速或风向在指定时间内的最大值或最小值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        /// <param name="windFieldName"></param>
        /// <param name="fanNumber"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static double[] GetMaxMinDataOfSinglePoint(Type type, string tableName, string windFieldName, string fanNumber, DateTime? startTime, DateTime? endTime)
        {
            //定义查询类型
            string searchType = string.Empty;
            if (type == Type.风速)
            {
                searchType = "WindSpeed";
            }
            else if (type == Type.风向)
            {
                searchType = "WindDirection";
            }

            double[] data = new double[2];
            object maxTemp = SqlHelper.FromDBValue( SqlHelper.ExecuteScalar(ConnStr_SensorData,
              "select max(" + searchType + ") from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber and DateTime>=@startTime and DateTime<=@endTime",
              new SqlParameter("@WindFieldName", windFieldName),
              new SqlParameter("@FanNumber", Convert.ToInt32(fanNumber)),
              new SqlParameter("@startTime", startTime),
              new SqlParameter("@endTime", endTime)));
            if (maxTemp!=null)
            {
                data[0] = (double)maxTemp;
            }
            object minTemp = SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
              "select min(" + searchType + ") from " + tableName + " where WindFieldName=@WindFieldName and FanNumber=@FanNumber and DateTime>=@startTime and DateTime<=@endTime",
              new SqlParameter("@WindFieldName", windFieldName),
              new SqlParameter("@FanNumber", Convert.ToInt32(fanNumber)),
              new SqlParameter("@startTime", startTime),
              new SqlParameter("@endTime", endTime)));
            if (minTemp!=null)
            {
                data[1] = (double)minTemp;
            }
            return data;          
        }


        /// <summary>
        /// 获得指定风场的所有风机号
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="windFieldName"></param>
        /// <returns></returns>
        public static int[] GetFanNumber(string tableName, string windFieldName)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                    "select distinct FanNumber from "+tableName+" where WindFieldName=@WindFieldName order by FanNumber",                    
                    new SqlParameter("@WindFieldName", windFieldName));
            if (table.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                int[] fanNumber = new int[table.Rows.Count];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    fanNumber[i] = (int)table.Rows[i]["FanNumber"];
                }
                return fanNumber;
            }
        }

    }
}
