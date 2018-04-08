using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server_IOCP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //定义IOCP，最大连接数设置为20000，每个I/O的缓存设置为1024
        private IoServer iocp = new IoServer(20000, 1024);
        //定义委托
        public delegate void SetListBoxCallBack(string str);
        public SetListBoxCallBack setlistboxcallback;
        public MainWindow()
        {
            InitializeComponent();
            setlistboxcallback = new SetListBoxCallBack(SetListBox);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            if ((string)btnListen.Content == "start listen")
            {
                try
                {
                    //开始监听                
                    iocp.Start(txtServerAddress.Text, int.Parse(txtServerPort.Text));
                    iocp.mainForm = this;
                    btnListen.Content = "stop Listen";
                    SetListBox("监听开启...");
                }
                catch { }                            
            }
            else if ((string)btnListen.Content == "stop Listen")
            {
                //停止监听   
                iocp.Stop();
                btnListen.Content = "start listen";
                SetListBox("监听停止...");
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lsb.Items.Clear();
        }

      
        public void SetListBox(string str)
        {
            lsb.Items.Insert(0, str);
            if (lsb.Items.Count > 10000)
            {
                lsb.Items.RemoveAt(lsb.Items.Count - 1);
            }   
        }
    }
}
