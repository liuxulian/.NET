using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;

namespace baidumap.DAL
{
    class SqlHelper
    {
        /// <summary>
        /// 检查数据库链接，成功返回则 0
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static int CheckDBConnection(string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    return 0;
                }
                catch
                {
                    return -1;
                }
                //catch (Exception)
                //{
                    
                //    throw new Exception("数据库未连接，请检查网络！");
                //}
                
            }
        }
        /// <summary>
        /// ExecuteNonQuery一般用来执行Update、Delete、Insert语句
        /// </summary>
        /// <param name="sql">SQL命令</param>
        /// <param name="parameters">parameters为SQL命令中涉及到的参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connStr, string sql, params SqlParameter[] parameters) //使用长度可变参数来简化
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    //foreach (SqlParameter param in parameters)
                    //{
                    //    cmd.Parameters.Add(param);
                    //}
                    cmd.Parameters.AddRange(parameters); //添加参数
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// ExecuteScalar一般用来执行一般用来执行有且仅有一行一列返回值的SQL语句(select语句)
        /// </summary>
        /// <param name="sql">SQL命令</param>
        /// <param name="parameters">parameters为SQL命令中涉及到的参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string connStr, string sql, params SqlParameter[] parameters) //使用长度可变参数来简化
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters); //添加参数
                    return cmd.ExecuteScalar();
                }
            }
        }



        
        /// <summary>
        /// 离线数据集，只用来执行查询结果比较小的sql
        /// </summary>
        /// <param name="sql">SQL命令</param>
        /// <param name="parameters">parameters为SQL命令中涉及到的参数</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string connStr, string sql, params SqlParameter[] parameters) //使用长度可变参数来简化
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();            
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters); //添加参数                    
                    //SqlDataAdapter是一个帮我们把SqlCommand查询到的结果填充到DataSet中的类
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    //DataSet相当于本地的一个复杂集合
                    DataSet dataset = new DataSet();
                    adapter.Fill(dataset); //执行cmd并且把SqlCommand查询到的结果填充到DataSet中
                    return dataset.Tables[0];
                }
            }
        }
        /// <summary>
        /// 从数据库中取数据如果出现 DBNULL 的处理方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object FromDBValue(object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 向数据库中写数据如果出现 null 的处理方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToDBValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="dataBase">数据库名</param>
        /// <param name="tableName">数据表名</param>
        public static void CreateTable(string connStr,string tableName)
        {
            string sql = @"use ZBHY "                        
                      + " create table "+tableName
                     +@"(ID  bigint not null identity(1,1) PRIMARY KEY,   --设置为主键，并自动增长
                        CurrentTableName    nvarchar(50)    not null, --流程步骤名称 
                        WindFieldNumber   int    not null,    --流程步骤描述
                        WindFieldName    nvarchar(MAX) not null,     --时限
                        DateTime     datetime  default(getdate()) not null,     --二级菜单链接 
                        GPS_lng    nvarchar(MAX)    not null, 
                        GPS_lat    nvarchar(MAX)    not null,
                        FanNumber   int   not null,
                        WindSpeed	float	not null,
                        WindDirection	float not null,
                        TempIp	nvarchar(MAX) not null,
                        Elevation   nvarchar(MAX),
                        Course   nvarchar(MAX),
                        NavigationalSpeed   nvarchar(MAX),
                        Temperature   nvarchar(MAX),
                        Humidity   nvarchar(MAX),
                        AirPressure   nvarchar(MAX),
                        )
                        ";
            SqlHelper.ExecuteNonQuery(connStr, sql);            
        }


    }
}
