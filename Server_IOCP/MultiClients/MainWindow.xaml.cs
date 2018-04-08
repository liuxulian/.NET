//#define MULTICLIENTS;
#define SINGLE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    
namespace MultiClients
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        //线程列表
        List<Thread> thList = new List<Thread>();
        //定义委托
        private delegate void Del(string str);
        private delegate void Del2();
        //线程数量
        private int threadQuantity = 0;
        Mutex mutex = new Mutex();

        public MainWindow()
        {
            InitializeComponent();
        }
#if SINGLE
        Socket socketSend;
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //创建负责通信的Socket
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(txtServerAddress.Text);//服务器ip
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtServerPort.Text));//服务器应用的端口号
                //获得要连接的远程服务器应用程序的IP地址和端口号
                socketSend.Connect(point);
                ShowMsg("连接成功！");
                //开启新线程不停地接收服务端发来地消息
                Thread th = new Thread(Recive);
                th.IsBackground = true;
                th.Start();
            }
            catch { }           
        }
        void Recive()
        {
            while (true)
            {
                try
                {
                    //客户端连接成功后，服务器应该接受客户端发来的消息
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //实际接受到的有效字节数
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }                   
                                                                                      
                    string str = Encoding.UTF8.GetString(buffer, 0, r);                   
                    this.Dispatcher.BeginInvoke(new Del(ShowMsg), socketSend.RemoteEndPoint + ":" + str);
                }
                catch
                { }
            }
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string str = txtTx.Text.Trim();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            //byte[] buffer = System.Text.Encoding.ASCII.GetBytes(str);
            //while (true)
            //{
            //    Thread.Sleep(300);
            //    socketSend.Send(buffer);
            //}
            socketSend.Send(buffer);           
        }

#endif
#if MULTICLIENTS
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

            Thread th = new Thread(RunBuilt);
            th.IsBackground = true;
            th.Start();          

        }

        private void RunBuilt()
        {
            for (int i = 1; i <= 800; i++)
            {
                Thread th = new Thread(CreateClient);
                th.IsBackground = true;
                th.Start(i);
                threadQuantity++;
                //Thread.Sleep(1);
            }       
        }
        //创建多个客户端      
        private void CreateClient(object o)
        {
            int num = Convert.ToInt32(o);
            //创建负责通信的Socket
            Socket socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");//服务器ip
            IPEndPoint point = new IPEndPoint(ip, 1086);//服务器应用的端口号
            //获得要连接的远程服务器应用程序的IP地址和端口号
            socketSend.Connect(point);
            //this.Dispatcher.BeginInvoke(new Del(ShowMsg), num + "号" + socketSend.RemoteEndPoint.ToString() + ": " + "连接成功");
            this.Dispatcher.BeginInvoke(new Del(ShowMsg2), threadQuantity.ToString());
            Thread.Sleep(15000);
            mutex.WaitOne();
            threadQuantity--;
            mutex.ReleaseMutex();
            this.Dispatcher.BeginInvoke(new Del(ShowMsg2), threadQuantity.ToString());
        }
#endif
        void ShowMsg(string str)
        {
            txtRx.AppendText(str + "\r\n");
        }
        void ShowMsg2(string str)
        {

            txtbCount.Text = str;
        }
       
      

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtRx.Clear();
        }
    }
}
