using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace baidumap.DAL
{
    public class AllWindFieldDAL
    {
        //从App.config拿到数据库链接字符串，访问ZBHY数据库
        private static string ConnStr_SensorData = ConfigurationManager.ConnectionStrings["dbConnStr_SensorData"].ToString();


        /// <summary>
        /// 获得所有省份名称
        /// </summary>
        /// <returns></returns>
        public static string[]GetAllProvince()
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct Province from T_AllWindField");

            if (table.Rows.Count <= 0)
            {
                //数据为空
                return null;
            }
            else
            {
                string[] province = new string[table.Rows.Count];
                for (int i = 0; i < province.Length; i++)
                {
                    province[i] = (string)table.Rows[i]["Province"];
                }
                return province;
            }
        }

        /// <summary>
        /// 获得某个省份下的城市名称
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        public static string[]GetCityOfProvince(string province)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct City from T_AllWindField where Province=@Province",
                new SqlParameter("@Province",province));

            if (table.Rows.Count <= 0)
            {
                //数据为空
                return null;
            }
            else
            {
                string[] city = new string[table.Rows.Count];
                for (int i = 0; i < city.Length; i++)
                {
                    city[i] = (string)table.Rows[i]["City"];
                }
                return city;
            }
        }

        /// <summary>
        /// 获得某个省份所有公司名称
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        public static string[]GetCompanyOfProvince(string province)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct CompanyName from T_AllWindField where Province=@Province",
                new SqlParameter("@Province", province));

            if (table.Rows.Count <= 0)
            {
                //数据为空
                return null;
            }
            else
            {
                string[] company = new string[table.Rows.Count];
                for (int i = 0; i < company.Length; i++)
                {
                    company[i] = (string)table.Rows[i]["CompanyName"];
                }
                return company;
            }
        }
        /// <summary>
        /// 获得某省某市的公司名称
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public static string[] GetCompanyByProvinceAndCity(string province,string city)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select distinct CompanyName from T_AllWindField where Province=@Province and City=@City",
                new SqlParameter("@Province", province),
                new SqlParameter("@City", city));

            if (table.Rows.Count <= 0)
            {
                //数据为空
                return null;
            }
            else
            {
                string[] company = new string[table.Rows.Count];
                for (int i = 0; i < company.Length; i++)
                {
                    company[i] = (string)table.Rows[i]["CompanyName"];
                }
                return company;
            }
        }

        /// <summary>
        /// 获取某省某市对应业主的风场名称，若city=null，则只获取省内对应业主的风场名称
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static AllWindField[] GetWindFieldNameAndFanCountByPlace(string province, string city, string companyName)
        {
            DataTable table;
            if (city==null)
            {
                //获取省内对应业主的风场名称和风机数量
                table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
                "select WindFieldName,FanCount from T_AllWindField where Province=@Province and CompanyName=@CompanyName",
                new SqlParameter("@Province", province),
                new SqlParameter("@CompanyName", companyName));
            }
            else
            {
                //获取某省某市对应业主的风场名称和风机数量
                table = SqlHelper.ExecuteDataTable(ConnStr_SensorData,
               "select WindFieldName,FanCount from T_AllWindField where Province=@Province and @City=City and CompanyName=@CompanyName",
               new SqlParameter("@Province", province),
               new SqlParameter("@City", city),
               new SqlParameter("@CompanyName", companyName));
            }

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
        public static AllWindField[] GetWindFieldNameAndFanCountByCompanyName(string companyName)
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
                return (int)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select SUM(FanCount) from T_AllWindField"));
            }
            else
            {
                return (int)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select SUM(FanCount) from T_AllWindField where CompanyName=@CompanyName",
                new SqlParameter("@CompanyName", CompanyName)));
            }
        }

        /// <summary>
        /// 根据地点获取某公司在该地点的风机数量
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public static int GetFanCountSumByPlace(string companyName,string province,string city=null)
        {
            if (city==null)
            {
                //只查询该公司在某省份的风机数量
                return (int)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select SUM(FanCount) from T_AllWindField where CompanyName=@CompanyName and Province=@Province",
                new SqlParameter("@CompanyName", companyName),
                new SqlParameter("@Province", province)));
            }
            else
            {
                //查询该公司在某省某市的风机数量
               return (int)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
               "select SUM(FanCount) from T_AllWindField where CompanyName=@CompanyName and Province=@Province and City=@City",
               new SqlParameter("@City", city),
               new SqlParameter("@CompanyName", companyName),
               new SqlParameter("@Province", province)));
            }
        }

        /// <summary>
        /// 根据公司名获得该公司的数据表名
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static string GetTableName(string companyName)
        {
            return (string)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
               "select distinct BranchTableName from T_AllWindField where CompanyName=@CompanyName",
               new SqlParameter("@CompanyName", companyName)));
        }

        /// <summary>
        /// 向数据库中插入新数据
        /// </summary>
        /// <param name="awf"></param>
        public static int InsertData(AllWindField awf)
        {
            //返回受影响的行数
            return SqlHelper.ExecuteNonQuery(ConnStr_SensorData,
                @"insert into T_AllWindField(BranchTableName,CompanyName,WindFieldName,Province,City,
                 DetailAddress,FanType,FanCount,FanModelNumber,FanHeight,AnemoscopeModelNumber,SignalKind)
                 values(@BranchTableName,@CompanyName,@WindFieldName,@Province,@City,@DetailAddress,
                 @FanType,@FanCount,@FanModelNumber,@FanHeight,@AnemoscopeModelNumber,@SignalKind)",
                 new SqlParameter("@BranchTableName", awf.BranchTableName),
                 new SqlParameter("@CompanyName", awf.CompanyName),
                 new SqlParameter("@WindFieldName", awf.WindFieldName),
                 new SqlParameter("@Province", awf.Province),
                 new SqlParameter("@City", awf.City),
                 new SqlParameter("@DetailAddress", SqlHelper.ToDBValue(awf.DetailAddress)),
                 new SqlParameter("@FanType", SqlHelper.ToDBValue(awf.FanType)),
                 new SqlParameter("@FanCount", SqlHelper.ToDBValue(awf.FanCount)),
                 new SqlParameter("@FanModelNumber", SqlHelper.ToDBValue(awf.FanModelNumber)),
                 new SqlParameter("@FanHeight", SqlHelper.ToDBValue(awf.FanHeight)),
                 new SqlParameter("@AnemoscopeModelNumber", SqlHelper.ToDBValue(awf.AnemoscopeModelNumber)),
                 new SqlParameter("@SignalKind", SqlHelper.ToDBValue(awf.SignalKind)));            
        }

        /// <summary>
        /// 判断公司是否存在
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public static int IsExistOfCompany(string companyName)
        {
            return (int)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
               "select count(*) from T_AllWindField where CompanyName=@companyName",
               new SqlParameter("@companyName", companyName)));
        }

        /// <summary>
        /// 判断某个表格在数据库里是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int IsExistOfTable(string tableName)
        {
            return (int)SqlHelper.FromDBValue(SqlHelper.ExecuteScalar(ConnStr_SensorData,
                "select count(*) from T_AllWindField where BranchTableName=@tableName",
                new SqlParameter("@tableName", tableName)));
        }

        /// <summary>
        /// 创建数据表
        /// </summary>        
        /// <param name="tableName">数据表名</param>
        public static void CreateTable(string tableName)
        {
            SqlHelper.CreateTable(ConnStr_SensorData,tableName);
        }

    }
}
