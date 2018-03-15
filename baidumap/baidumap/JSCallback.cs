using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace baidumap
{
    [ComVisible(true)]
    public class JSCallback
    {
        private MainWindow Main
        {
            get;
            set;
        }
        public JSCallback(MainWindow main)
        {
            this.Main = main;
        }
        public void play(string str1,string str2,string str3)
        {
            //创建风机详细信息窗口
            DetailWindow dw = new DetailWindow(str1,str2,str3);
            dw.Show();                   
        }
    }
}
