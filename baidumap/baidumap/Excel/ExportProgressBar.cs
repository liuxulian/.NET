using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace baidumap.Excel
{
    public class ExportProgressBar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 导出数据进度条数值
        /// </summary>
        private double _pgPercent;
        public double PgPercent
        {
            get { return _pgPercent; }
            set
            {
                _pgPercent = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PgPercent")); //触发事件，刷新该属性的UI显示
                }
            }
        }
        /// <summary>
        /// 数据百分数
        /// </summary>
        private string _pgPercentText;
        public string PgPercentText
        {
            get { return _pgPercentText; }
            set
            {
                _pgPercentText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PgPercentText")); //触发事件，刷新该属性的UI显示
                }
            }
        }
    }
}
