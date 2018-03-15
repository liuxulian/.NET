using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace baidumap.DAL
{
    public class AllWindField
    {
        /// <summary>
        /// 存储所有风机点对应的风场信息
        /// </summary>
        public static AllWindField[] allWindFieldArray;



        //分支表名，分支表存储该公司旗下各个风场的数据
        public string BranchTableName { get; set; }
        //公司名字
        public string CompanyName { get; set; }
        //风场名称
        public string WindFieldName { get; set; }
        //省
        public string Province { get; set; }
        //城市
        public string City { get; set; }
        //详细地址（自填）   (可为空)
        public string DetailAddress { get; set; }
        //风机数量   (可为空)
        public int FanCount { get; set; }
        //风机类型   (可为空)
        public string FanType { get; set; }
        //风机型号（自填）   (可为空)
        public string FanModelNumber { get; set; }
        //风机高度（自填）   (可为空)
        public string FanHeight { get; set; }
        //测风仪型号（自填）   (可为空)
        public string AnemoscopeModelNumber { get; set; }
        //信号模式   (可为空)
        public string SignalKind { get; set; }
    }
}
