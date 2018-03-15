using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using baidumap;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Threading;

namespace BMapWeb
{
    public partial class View : System.Web.UI.Page
    {
        //public string uuu = string.Empty;
        
        //要返回的数组
        //public string[] str;
        public string[] sss = { "123", "liu", "you" };
        public string json;
        public string name;
        protected void Page_Load(object sender, EventArgs e)
        {
            //获取所有的风机点
            FanPoint.FanPointList = FanPointDAL.GetAllPoints();
            //获取每个风机的风场信息
            AllWindField.allWindFieldArray = AllWindFieldDAL.GetWindFieldInformationOfPoints(FanPoint.FanPointList);
            //创建风机点卡片对象数组
            PointCard[] pointCard = new PointCard[FanPoint.FanPointList.Count];
            for (int i = 0; i < pointCard.Length; i++)
			{
                pointCard[i]=new PointCard();
                pointCard[i].GPS_lng = FanPoint.FanPointList[i].GPS_lng;
                pointCard[i].GPS_lat = FanPoint.FanPointList[i].GPS_lat;
                pointCard[i].CompanyName = AllWindField.allWindFieldArray[i].CompanyName;
                pointCard[i].WindFieldName = AllWindField.allWindFieldArray[i].WindFieldName;
                pointCard[i].Address = AllWindField.allWindFieldArray[i].Province + AllWindField.allWindFieldArray[i].City + AllWindField.allWindFieldArray[i].DetailAddress;
                pointCard[i].FanCountOfCompany = AllWindFieldDAL.GetFanCountSum(pointCard[i].CompanyName);//获得当前公司的风机数量
                pointCard[i].FanNumber = FanPoint.FanPointList[i].FanNumber;
                pointCard[i].FanType = AllWindField.allWindFieldArray[i].FanType;
                pointCard[i].SignalKind = AllWindField.allWindFieldArray[i].SignalKind;
			}
            


            JavaScriptSerializer jss = new JavaScriptSerializer();

            json = jss.Serialize(pointCard);
            //Response.Write(json);

            //name = Context.Request["name"];
            //string age = Request["age"];
            //Response.Write(name + age);

            //Response.Write("alert();");

            


            //Response.Write("alert('" + FanPoint.FanPointList.Count + "');");


            //if (!Page.IsPostBack)
            //{
            //    //将数组转换为字符串传到GetArray()函数中去
            //    str = new string[] { "a,eb,c,d" };
            //    for (int i = 0; i < str.Length; i++)
            //    {
            //        uuu += str[i].ToString() + ',';
            //    }
            //    uuu = uuu.Substring(0, uuu.Length - 1);
            //}
            
            
        }
        
    
        public string struu = "liuxulian";
        public void sayHello()
        {           
            //Response.Write("alert('数据');");
            //return "大家好" + DateTime.Now;            
        }

        
    }
}