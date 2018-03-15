using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
//using System.Windows;

namespace BMapWeb
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


    }
}
