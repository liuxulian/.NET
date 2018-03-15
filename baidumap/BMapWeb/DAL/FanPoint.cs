using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMapWeb
{
    public class FanPoint
    {        
        /// <summary>
        /// 备用静态变量，用于记录全国范围内的风机点，软件第一次启动时就存储信息，以供后续操作调用 
        /// </summary>
        public static List<FanPoint> FanPointList;



        //当前表名
        public string CurrentTableName { get; set; }
        //风场编号
        public int WindFieldNumber { get; set; }
        //风场名字
        public string WindFieldName { get; set; }

        //GPS/北斗
        public string GPS_lng { get; set; }//经度
        public string GPS_lat { get; set; }//纬度
        //风机号
        public int FanNumber { get; set; }
        //风速
        public double WindSpeed { get; set; }
        //风向
        public double WindDirection { get; set; }
        //临时IP
        public string TempIp { get; set; }
        //海拔（可为空）
        public string Elevation { get; set; }
        //航向（可为空）
        public string Course { get; set; }
        //航速（可为空）
        public string NavigationalSpeed { get; set; }
        //温度（可为空）
        public string Temperature { get; set; }
        //湿度（可为空）
        public string Humidity { get; set; }
        //气压（可为空）
        public string AirPressure { get; set; }
    }
}
