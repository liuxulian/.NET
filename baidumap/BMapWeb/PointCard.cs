using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMapWeb
{
    /// <summary>
    /// 地图里需要展示的卡片信息
    /// </summary>
    public class PointCard
    {
        //GPS/北斗
        public string GPS_lng { get; set; }//经度
        public string GPS_lat { get; set; }//纬度
        //公司名字
        public string CompanyName { get; set; }
        //风场名称
        public string WindFieldName { get; set; }
        //地址
        public string Address { get; set; }
        //公司风机数量   (可为空)
        public int FanCountOfCompany { get; set; }
        //风机号
        public int FanNumber { get; set; }
        //风机类型   (可为空)
        public string FanType { get; set; }
        //信号模式   (可为空)
        public string SignalKind { get; set; }
    }
}