using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace baidumap.DAL
{
    class LoginDAL
    {
        //从App.config拿到数据库链接字符串，访问ZBHY_USER数据库
        private static string ConnStr_UserData = ConfigurationManager.ConnectionStrings["dbConnStr_UserData"].ToString();

        /// <summary>
        /// 检查数据库连接
        /// </summary>
        /// <returns></returns>
        public static int CheckDBConnection()
        {
            return SqlHelper.CheckDBConnection(ConnStr_UserData);
        }

        /// <summary>
        /// 根据UserName获取用户信息
        /// </summary>
        /// <param name="UserName">需要获取用户信息对应的用户名</param>
        /// <returns></returns>
        public static User GetAccountByUserName(string UserName)
        {
            DataTable table = SqlHelper.ExecuteDataTable(ConnStr_UserData,
                "select * from T_User where UserName=@UserName",
                new SqlParameter("@UserName", UserName));     
            if (table.Rows.Count<=0)
            {               
                return null;//没找到此用户
            }
            else if (table.Rows.Count>1)
            {
                throw new Exception("此用户在服务器端重复！");
            }
            else
            {
                //获取此用户的身份信息
                DataRow row = table.Rows[0];
                User user = new User();
                user.UserID = (Guid)row["UserID"];
                user.UserName = (string)row["UserName"];
                user.Password = (string)row["Password"];
                user.RegisterTime = (DateTime)row["RegisterTime"];
                user.ErrorTimes = (int)row["ErrorTimes"];
                user.LoginTime = (DateTime?)SqlHelper.FromDBValue(row["LoginTime"]);
                return user;
            }
        }


        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static int RegisterUser(string UserName,string Password)
        {
            string pswd = GetMD5(Password);
            //注册用户
            int renum = SqlHelper.ExecuteNonQuery(ConnStr_UserData,
                "insert into T_User(UserName,Password,ErrorTimes,RegisterTime) values (@UserName,@Password,0,getdate())",
                new SqlParameter("@UserName",UserName),
                new SqlParameter("@Password", pswd));

            return renum;
        }


        /// <summary>
        /// 更新指定用户的登录时间
        /// </summary>
        /// <param name="UserName">所要更新登录时间的用户名</param>
        public static void UpdateLoginTimeByUserName(string UserName)
        {
            SqlHelper.ExecuteNonQuery(ConnStr_UserData,
                "update T_User set LoginTime=getdate() where UserName=@UserName",
                new SqlParameter("@UserName", UserName));
        }

        /// <summary>
        /// 给指定用户的登录错误次数清零
        /// </summary>
        /// <param name="UserName">所要清零登录错误次数的用户名</param>
        public static void ResetErrorTimesByUserName(string UserName)
        {
            SqlHelper.ExecuteNonQuery(ConnStr_UserData,
                "update T_User set ErrorTimes=0 where UserName=@UserName",
                new SqlParameter("@UserName", UserName));
        }

        /// <summary>
        /// 给指定用户的登录错误次数+1
        /// </summary>
        /// <param name="UserName"></param>
        public static void UpdateErrorTimesByUserName(string UserName)
        {
            SqlHelper.ExecuteNonQuery(ConnStr_UserData,
               "update T_User set ErrorTimes=ErrorTimes+1 where UserName=@UserName",
               new SqlParameter("@UserName", UserName));
        }

        /// <summary>
        /// 返回服务器当前的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime()
        {
            DateTime dt = (DateTime)SqlHelper.ExecuteScalar(ConnStr_UserData,
                "select getdate()");
            return dt;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            //创建MD5对象
            MD5 md5 = MD5.Create();
            //需要将字符串转成字节数组
            byte[] buffer = Encoding.GetEncoding("GBK").GetBytes(str + "*#*#*#");//加盐处理
            //开始加密，返回一个加密好的字节数组
            byte[] MD5Buffer = md5.ComputeHash(buffer);
            md5.Clear();
            //将字节数组转换成字符串有三种方式
            //第一种：将字节数组中每个元素按照指定的编码格式解析成字节串
            //第二种：直接将数组 ToString();
            //第三种：将字节数组中的每个元素ToString();

            //return Encoding.GetEncoding("GBK").GetString(MD5Buffer);
            string strNew = "";
            for (int i = 0; i < MD5Buffer.Length; i++)
            {
                strNew += MD5Buffer[i].ToString("x2"); //加上"x"是转换成16进制，多了个2起对齐作用
            }
            return strNew.ToLower();
        }
    }
}
