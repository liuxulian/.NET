using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml;

namespace baidumap.DAL
{
    class XMLDAL
    {
        /// <summary>
        /// 创建xml文档
        /// </summary>
        public static void CreateXML()
        {
            //创建xml文档
            XmlDocument doc = new XmlDocument();
            XmlElement controls;
            if (File.Exists("XMLconfig.xml"))
            {
                //若配置文档存在，则不再创建
                return;
            }
            else
            {
                //如果文件不存在
                //创建第一行
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec);
                //创建根节点
                controls = doc.CreateElement("controls");//节点名字可以根据自己需要起
                doc.AppendChild(controls);
                //给节点controls创建子节点
                XmlElement checkBox = doc.CreateElement("checkBox");
                controls.AppendChild(checkBox);//将checkBox添加到根节点
                //给checkBox添加子节点
                XmlElement status = doc.CreateElement("status");
                status.InnerText = "false";
                checkBox.AppendChild(status);


                XmlElement comboBox = doc.CreateElement("comboBox");
                controls.AppendChild(comboBox);//将ComboBox添加到根节点         


                doc.Save("XMLconfig.xml");//保存文档
            }
        }
        /// <summary>
        /// 向xml文档中更新用户user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void UpdateUser(string userName,string password)
        {
            //创建xml文档
            XmlDocument doc = new XmlDocument();
            //加载配置文档
            doc.Load("XMLconfig.xml");

            //获得根节点
            XmlElement controls = doc.DocumentElement;
            //获得子节点
            XmlNode comboBox = controls.SelectSingleNode("/controls/comboBox");
            //获得所有的user节点，返回节点的集合
            //XmlNodeList users = controls.SelectNodes("/controls/comboBox/user");

            //根据节点属性获得节点
            XmlNode currentUser = controls.SelectSingleNode("/controls/comboBox/user[@userName='" + userName + "']");
            if (currentUser!=null)
            {
                //移除同名节点
                comboBox.RemoveChild(currentUser);
            }
            
            //给ComboBox添加子节点
            XmlElement user = doc.CreateElement("user");
            user.SetAttribute("userName", userName);
            user.SetAttribute("password", password);
            comboBox.AppendChild(user); //往comboBox节点中添加新的user节点


            doc.Save("XMLconfig.xml");//保存文档
        }
        /// <summary>
        /// 获取已经存储的用户名和密码,string[0]为用户名，string[1]为密码
        /// </summary>
        /// <returns></returns>
        public static List<string[]> GetUser()
        {
            //创建xml文档
            XmlDocument doc = new XmlDocument();
            //加载配置文档
            doc.Load("XMLconfig.xml");

            //获得根节点
            XmlElement controls = doc.DocumentElement;
            //获得所有的user节点，返回节点的集合
            XmlNodeList users = controls.SelectNodes("/controls/comboBox/user");

            List<string[]> userList = new List<string[]>();            
            foreach (XmlNode item in users)
            {
                string userName = item.Attributes["userName"].Value;//获取存储的用户名
                string password = item.Attributes["password"].Value;//获取存储的密码
                userList.Add(new string[] { userName, password });
            }
            return userList;
        }


        /// <summary>
        /// 清空所有user的password
        /// </summary>
        public static void CleanPassword()
        {
            //创建xml文档
            XmlDocument doc = new XmlDocument();
            //加载配置文档
            doc.Load("XMLconfig.xml");

            //获得根节点
            XmlElement controls = doc.DocumentElement;
            //获得所有的user节点，返回节点的集合
            XmlNodeList users = controls.SelectNodes("/controls/comboBox/user");

            foreach (XmlNode item in users)
            {                
                item.Attributes["password"].Value = string.Empty;//获取存储的密码                
            }

            doc.Save("XMLconfig.xml");//保存文档
        }

        /// <summary>
        /// 获取保存密码复选框的状态
        /// </summary>
        /// <returns>状态 true/false</returns>
        public static bool GetCheckBoxStatus()
        {
            //创建xml文档
            XmlDocument doc = new XmlDocument();
            //加载配置文档
            doc.Load("XMLconfig.xml");

            //获得根节点
            XmlElement controls = doc.DocumentElement;
            //获得子节点
            XmlNode status = controls.SelectSingleNode("/controls/checkBox/status");
            if (Convert.ToBoolean(status.InnerText) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 更新复选框的状态
        /// </summary>
        /// <param name="checkBoxStatus">复选框状态，true / false</param>
        public static void UpdateCheckBoxStatus(string checkBoxStatus)
        {
            //创建xml文档
            XmlDocument doc = new XmlDocument();
            //加载配置文档
            doc.Load("XMLconfig.xml");

            //获得根节点
            XmlElement controls = doc.DocumentElement;
            //获得子节点
            XmlNode status = controls.SelectSingleNode("/controls/checkBox/status");
            //修改status节点的值
            status.InnerText = checkBoxStatus;
            
            doc.Save("XMLconfig.xml");//保存文档
        }
    }
}
