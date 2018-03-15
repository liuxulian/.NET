using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BMapWeb
{
    public class FanPointDAL
    {
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
                          new SqlParameter("@FanNumber", fanNumber));
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

    }
}
