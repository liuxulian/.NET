using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BMapWeb
{
    public class AllWindFieldDAL
    {
        //从App.config拿到数据库链接字符串，访问ZBHY数据库
        private static string ConnStr_SensorData = ConfigurationManager.ConnectionStrings["dbConnStr_SensorData"].ToString();

        /// <summary>
        /// 获取全国风场公司数量
        /// </summary>
        /// <returns></returns>
        public static int GetCompanyNumber()
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct CompanyName from T_AllWindField");
            if (table.Rows.Count <= 0)
            {
                //数据为空
                return 0;
            }
            else
            {
                return table.Rows.Count;
            }
        }
        /// <summary>
        /// 获取全国风场数量
        /// </summary>
        /// <returns></returns>
        public static int GetWindFieldNumber()
        {
            return (int)SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select count(*) from T_AllWindField");
        }
        /// <summary>
        /// 获得所有公司的名字
        /// </summary>
        /// <returns></returns>
        public static string[] GetCompanyName()
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct CompanyName from T_AllWindField");
            
            if (table.Rows.Count <= 0)
            {
                //数据为空
                return null;
            }
            else
            {
                string[] companyName = new string[table.Rows.Count];
                for (int i = 0; i < companyName.Length; i++)
                {
                    companyName[i] = (string)table.Rows[i]["CompanyName"];
                }
                return companyName;
            }
        }
        /// <summary>
        /// 根据公司名获取对应的风场名称和风机数量
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static AllWindField[] GetWindFieldNameByCompanyName(string companyName)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select WindFieldName,FanCount from T_AllWindField where CompanyName=@CompanyName",
                new SqlParameter("@CompanyName",companyName));

            if (table.Rows.Count <= 0)
            {
                //数据为空
                return null;
            }
            else
            {
                AllWindField[] windField = new AllWindField[table.Rows.Count];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    windField[i] = new AllWindField();
                    windField[i].WindFieldName = (string)table.Rows[i]["WindFieldName"];
                    windField[i].FanCount = (int)table.Rows[i]["FanCount"];
                }
                return windField;
            }
        }
        /// <summary>
        /// 根据公司名和风场名获得该风场的详细信息
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="windFieldName"></param>
        /// <returns></returns>
        public static AllWindField GetWindFieldByName(string companyName,string windFieldName)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select * from T_AllWindField where CompanyName=@CompanyName and WindFieldName=@WindFieldName",
                new SqlParameter("@CompanyName", companyName),
                new SqlParameter("@WindFieldName", windFieldName));
            if (table.Rows.Count<=0)
            {
                return null;
            }
            else
            {
                //创建风场对象
                AllWindField windField = new AllWindField();
                DataRow row = table.Rows[0];
                windField = new AllWindField();
                windField.BranchTableName = (string)row["BranchTableName"];
                windField.CompanyName = (string)row["CompanyName"];
                windField.WindFieldName = (string)row["WindFieldName"];
                windField.Province = (string)SqlHelper.FromDBValue(row["Province"]);
                windField.City = (string)SqlHelper.FromDBValue(row["City"]);
                windField.DetailAddress = (string)SqlHelper.FromDBValue(row["DetailAddress"]);
                windField.FanCount = (int)SqlHelper.FromDBValue(row["FanCount"]);
                windField.FanType = (string)SqlHelper.FromDBValue(row["FanType"]);
                windField.FanModelNumber = (string)SqlHelper.FromDBValue(row["FanModelNumber"]);
                windField.FanHeight = (string)SqlHelper.FromDBValue(row["FanHeight"]);
                windField.AnemoscopeModelNumber = (string)SqlHelper.FromDBValue(row["AnemoscopeModelNumber"]);
                windField.SignalKind = (string)SqlHelper.FromDBValue(row["SignalKind"]);
                return windField;
            }
        }

        /// <summary>
        /// 获取每个风机所在的风场信息，返回各个风机所在风场信息的数组，如果个别风机没查到风场信息，数组元素为null
        /// </summary>
        /// <param name="wpList">所有风机的集合</param>
        /// <returns>返回所有风机对应的风场，返回值为风场数组</returns>
        public static AllWindField[] GetWindFieldInformationOfPoints(List<FanPoint> fpList)
        {
            //创建风场数组，用于存储所有风机的风场信息，数组长度为风机个数
            AllWindField[] awfArray = new AllWindField[fpList.Count];                        

            for (int i = 0; i < fpList.Count; i++)
            {
                //去数据库查询当前风机对应的风场信息，正确结果只有一个
                DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                    "select * from T_AllWindField where BranchTableName=@BranchTableName and WindFieldName=@WindFieldName",
                    new SqlParameter("@BranchTableName", fpList[i].CurrentTableName),
                    new SqlParameter("@WindFieldName", fpList[i].WindFieldName));
                if (table.Rows.Count <= 0)
                {
                    //没查到对应的风场信息
                    awfArray[i] = null;
                    continue;
                }
                DataRow row = table.Rows[0];
                awfArray[i] = new AllWindField();
                awfArray[i].BranchTableName = (string)row["BranchTableName"];
                awfArray[i].CompanyName = (string)row["CompanyName"];
                awfArray[i].WindFieldName = (string)row["WindFieldName"];
                awfArray[i].Province = (string)SqlHelper.FromDBValue(row["Province"]);
                awfArray[i].City = (string)SqlHelper.FromDBValue(row["City"]);
                awfArray[i].DetailAddress = (string)SqlHelper.FromDBValue(row["DetailAddress"]);
                awfArray[i].FanCount = (int)SqlHelper.FromDBValue(row["FanCount"]);
                awfArray[i].FanType = (string)SqlHelper.FromDBValue(row["FanType"]);
                awfArray[i].FanModelNumber = (string)SqlHelper.FromDBValue(row["FanModelNumber"]);
                awfArray[i].FanHeight = (string)SqlHelper.FromDBValue(row["FanHeight"]);
                awfArray[i].AnemoscopeModelNumber = (string)SqlHelper.FromDBValue(row["AnemoscopeModelNumber"]);
                awfArray[i].SignalKind = (string)SqlHelper.FromDBValue(row["SignalKind"]);
            }
            return awfArray;
        }

       /// <summary>
        /// 更新指定风场的风机数量
       /// </summary>
       /// <param name="fanCount">当前风场的风机数量</param>
       /// <param name="tableName">当前表名</param>
       /// <param name="windFieldName">当前风场名</param>
        public static void UpdateFanNumber(int fanCount,string tableName,string windFieldName)
        {
            SqlHelper.ExecuteNonQuery(ConnStr_SensorData,
                "update T_AllWindField set FanCount=@FanCount where BranchTableName=@BranchTableName and WindFieldName=@WindFieldName",
                new SqlParameter("@FanCount", fanCount),
                new SqlParameter("@BranchTableName", tableName),
                new SqlParameter("@WindFieldName", windFieldName));
        }
        
        /// <summary>
        /// 获得某个公司或所有公司的风机数量
        /// </summary>
        /// <param name="CompanyName">公司名，如果没不传参数，则返回全国风机总量，否则返回各个公司风机数量</param>
        /// <returns>风机数量</returns>
        public static int GetFanCountSum(string CompanyName="*")
        {
            if (CompanyName=="*")
            {
                return (int)SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select SUM(FanCount) from T_AllWindField");
            }
            else
            {
                return (int)SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select SUM(FanCount) from T_AllWindField where CompanyName=@CompanyName",
                new SqlParameter("@CompanyName",CompanyName));
            }
        }
    }
}
