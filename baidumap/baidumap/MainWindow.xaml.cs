using baidumap.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace baidumap
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string[]> userList = new List<string[]>();
        public MainWindow()
        {
            InitializeComponent();
            InitConfig();//初始化配置数据
            //initEvent(); //加载地图
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {                        
            //将焦点聚集到登录按钮上
            btnLogin.Focus();          
        }
        private void initEvent()
        {

            //获取地图服务器IP
            string WebbrowserURL = ConfigurationManager.ConnectionStrings["WebbrowserURL"].ToString();
            this.wbBaiduMap.Navigate(WebbrowserURL); //给webbrowser控件添加链接
            // 清除IE缓存
            //foreach (string strFileName in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies)))
            //{
            //    if (strFileName.ToLower().IndexOf("index.dat") == -1) { File.Delete(strFileName); }
            //}

            //TransWindow._TWindow = this; //用于传输当前窗口对象
            this.wbBaiduMap.ObjectForScripting = new JSCallback(this);
            //this.wbBaiduMap.LoadCompleted += new LoadCompletedEventHandler(wbBaiduMap_LoadCompleted);
            //this.wbBaiduMap.Navigated += (a, b) => { this.hideScriptErrors(this.wbBaiduMap, true); }; // 阻止JS报错弹窗（可以注释）
        }

        /// <summary>
        /// web控件 与 javascript交互
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wbBaiduMap_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //this.wbBaiduMap.ObjectForScripting = new JSCallback(this);
        }


        #region 阻止JS报错弹窗（建议不使用）
        private void hideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => hideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
            Cursor = Cursors.Arrow;
        }
        #endregion

        /// <summary>
        /// 初始化配置数据
        /// </summary>
        private void InitConfig()
        {
            //创建xml配置文档
            XMLDAL.CreateXML();

            //获取复选框的状态
            bool checkBoxStatus = XMLDAL.GetCheckBoxStatus();
            ckbSavePassword.IsChecked = checkBoxStatus;//设置复选框的状态
            //获取下拉框的user
            userList = XMLDAL.GetUser();
            for (int i = userList.Count - 1; i >= 0; i--)
            {
                cbxUserName.Items.Add(userList[i][0]);//初始化下拉框
            }

            //如果处于保存密码状态，则将最近一次登录过的用户信息填上
            if (checkBoxStatus == true)
            {
                cbxUserName.Text = userList[userList.Count - 1][0];
                pwx.Password = userList[userList.Count - 1][1];
                if (cbxUserName.Text != "用户名")
                {
                    cbxUserName.Foreground = new SolidColorBrush(Colors.Black);
                    pwx.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        #region 设置用户名,密码输入框默认显示的字符
        private void cbxUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (cbxUserName.Text == "用户名")
            {
                cbxUserName.Text = string.Empty;
                Brush brs = new SolidColorBrush(Colors.Black);
                cbxUserName.Foreground = brs;
            }
        }

        private void cbxUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cbxUserName.Text == string.Empty)
            {
                Brush brs = new SolidColorBrush(Colors.Gray);
                cbxUserName.Foreground = brs;
                cbxUserName.Text = "用户名";
            }
        }

        private void pwx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (pwx.Password == "*#*#*#")
            {
                Brush brs = new SolidColorBrush(Colors.Black);
                pwx.Foreground = brs;
                pwx.Password = string.Empty;
            }
        }

        private void pwx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pwx.Password == string.Empty)
            {
                Brush brs = new SolidColorBrush(Colors.Gray);
                pwx.Foreground = brs;
                pwx.Password = "*#*#*#";
            }
        }
        #endregion

        #region 登录相关
        public delegate void DeleLogin(); //定义委托
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.Content = "登录中...";         
            //另开线程处理数据库交互
            Task<int> loginTask = new Task<int>(() => {
                this.Dispatcher.BeginInvoke(new DeleLogin(loginWork));
                return 0;
            });
            loginTask.Start();//开始工作
        }
        private void loginWork()
        {
            //首先检查数据库连接
            if (LoginDAL.CheckDBConnection() != 0)
            {
                MessageBox.Show("连接失败，请检查网络连接！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                btnLogin.Content = "登录失败";     
                return;
            }
            //获取全国的风机数量            
            int pointNum = FanPointDAL.GetAllPoints().Count;
            labPointNum.Content = pointNum + " 台";
            //获取全国风场数量
            int windFieldNum = AllWindFieldDAL.GetWindFieldNumber();
            labWindFieldNumber.Content = windFieldNum + "个";
            //获取全国的公司数量
            int companyNum = AllWindFieldDAL.GetCompanyNumber();
            labCompanyNumber.Content = companyNum + " 家";
            initEvent(); //加载地图


            int ErrorTimeSpanMin = 30;//为保证账户安全，设置多次登录错误后的用户需要等待的时间
            if (cbxUserName.Text == "用户名" || pwx.Password == "*#*#*#")
            {
                MessageBox.Show("用户名或密码有误，请重新输入！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                cbxUserName.Focus();
                return;
            }
            User user = LoginDAL.GetAccountByUserName(cbxUserName.Text);//获取当前的用户信息
            if (user == null)
            {
                MessageBox.Show("不存在此用户！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                cbxUserName.Focus();
                return;
            }
            else
            {
                //如果距离上次登录成功已经超过30分钟，则将登录错误次数清零
                if (user.LoginTime != null)
                {
                    DateTime dt = LoginDAL.GetServerTime();//获取服务器当前时间
                    TimeSpan ts = dt - (DateTime)user.LoginTime;//获取当前用户最新的上一次登录的时间
                    if (ts.TotalMinutes >= ErrorTimeSpanMin)//如果用户长时间没登录过，则将以前的登录错误次数清零
                    {
                        LoginDAL.ResetErrorTimesByUserName(user.UserName); //登录错误次数清零
                        user.ErrorTimes = 0;//本地记录的登录错误次数清零
                    }
                    else if (user.ErrorTimes >= 3)//如果登录错误次数超过了3次，则禁止用户在 ErrorTimeSpanMin 时间内登录(单位/分钟)
                    {
                        MessageBox.Show("登录失败次数过多，请 "
                            + Math.Ceiling(ErrorTimeSpanMin - ts.TotalMinutes)
                            + " 分钟后重试！",
                            "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            string passWord = LoginDAL.GetMD5(pwx.Password);//获取加密后的密码
            if (passWord != user.Password)
            {
                LoginDAL.UpdateErrorTimesByUserName(user.UserName);//登录错误次数+1
                LoginDAL.UpdateLoginTimeByUserName(user.UserName); //更新登录时间
                MessageBox.Show("密码不正确，请重新输入！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                pwx.Focus();
                return;
            }
            if ((cbxUserName.Text == user.UserName) && (passWord == user.Password))
            {
                //登录成功                  
                XMLDAL.UpdateUser(cbxUserName.Text, pwx.Password);//将登录成功的用户信息更新到配置文件里                
                LoginDAL.UpdateLoginTimeByUserName(user.UserName); //更新登录时间
                LoginDAL.ResetErrorTimesByUserName(user.UserName); //登录错误次数清零

                LoginGrid.Visibility = Visibility.Hidden;
                MainGrid.Visibility = Visibility.Visible;
            }
            else
            {
                LoginDAL.UpdateErrorTimesByUserName(user.UserName);//登录错误次数+1
                LoginDAL.UpdateLoginTimeByUserName(user.UserName); //更新登录时间
                MessageBox.Show("登录失败，请重试！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }


        #endregion

        #region 注册相关
        /// <summary>
        /// 点击注册label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterGrid.Visibility = Visibility.Visible;
            LoginGrid.Visibility = Visibility.Hidden;
            tbUserName.Focus();
        }
        /// <summary>
        /// 确认注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (tbUserName.Text == string.Empty)
            {
                MessageBox.Show("请输入用户名!", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbUserName.Focus();
                return;
            }
            if (pwxRegister.Password == string.Empty || pwxRegisterConfirm.Password == string.Empty)
            {
                MessageBox.Show("密码不能为空!", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwxRegister.Focus();
                return;
            }
            if (pwxRegister.Password != pwxRegisterConfirm.Password)
            {
                MessageBox.Show("两次输入的密码不一样，请重新输入!", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                pwxRegister.Focus();
                return;
            }
            User user = LoginDAL.GetAccountByUserName(tbUserName.Text);
            if (user != null)
            {
                MessageBox.Show("此用户已存在，请换个名字试试！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbUserName.Focus();
                return;
            }
            else
            {
                int renum = LoginDAL.RegisterUser(tbUserName.Text, pwxRegister.Password);
                if (renum <= 0)
                {
                    MessageBox.Show("注册失败，请联系管理员！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    MessageBox.Show("恭喜您，注册成功！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    RegisterGrid.Visibility = Visibility.Hidden;
                    LoginGrid.Visibility = Visibility.Visible;
                }
            }
        }
        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RegisterGrid.Visibility = Visibility.Hidden;
            LoginGrid.Visibility = Visibility.Visible;
        }


        // 设置光标靠近注册label的动画
        private void lbRegister_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            lbRegister.FontSize = 18;
            lbRegister.Foreground = new SolidColorBrush(Colors.White);
        }
        // 设置光标离开注册label的动画
        private void lbRegister_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            lbRegister.FontSize = 15;
            lbRegister.Foreground = new SolidColorBrush(Colors.LightSteelBlue);
        }




        #endregion

        //自动填密码
        private void cbxUserName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxUserName.SelectedItem == null)
            {
                pwx.Password = string.Empty;
                return;
            }
            if (ckbSavePassword.IsChecked == true)
            {
                //如果为保存密码状态，则自动将当前用户的密码填上
                foreach (string[] user in userList)
                {
                    if (cbxUserName.SelectedItem.ToString() == user[0])
                    {
                        pwx.Foreground = new SolidColorBrush(Colors.Black);
                        pwx.Password = user[1]; //将密码填到密码框
                        return;
                    }
                }
            }
        }

        #region 公司，风场筛选显示
        private void labCompanyNumber_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            labCompanyNumber.FontSize = 38;
            labCompanyNumber.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        private void labCompanyNumber_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            labCompanyNumber.FontSize = 36;
            labCompanyNumber.Foreground = new SolidColorBrush(Colors.White);
        }

        private void labCompanyNumber_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RankDisplay rd = new RankDisplay();
            rd.Show();
        }
        #endregion


        //添加新风场
        private void btnInsertData_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("此操作涉及数据库安全，请谨慎操作！", "重要提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //添加风场信息，启动窗体
            AddWindField awf = new AddWindField();
            awf.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //关闭子窗体
            if (TransWindow.detailWindow!=null)
            {
                TransWindow.detailWindow.Close();
            }
            if (TransWindow.rankDisplay!=null)
            {
                TransWindow.rankDisplay.Close();
            }
            if (TransWindow.addWindField != null)
            {
                TransWindow.addWindField.Close();
            }

            XMLDAL.UpdateCheckBoxStatus(ckbSavePassword.IsChecked.ToString());//保存保存密码复选框的状态
            if (ckbSavePassword.IsChecked == false)
            {
                //如果不保存密码，则清空所有user的密码
                XMLDAL.CleanPassword();
            }
        }


    }



}
