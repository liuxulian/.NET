using baidumap.DAL;
using baidumap.Excel;
using baidumap.Graph;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace baidumap
{
    /// <summary>
    /// DigitalWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DetailWindow : Window
    {
        //用于记录该风机的公司名称，风场名称，风机号
        private string CompanyName { get; set; }
        private string WindFieldName { get; set; }
        private string FanNumber { get; set; }

        //定义当前风机的风场信息
        private AllWindField windFieldOfFan;
        //用于记录加入对比的风机的风场信息，主要用其中的 BranchTableName 属性来获取公司表名
        private AllWindField windFieldOfCompareFan;
        //定义风速集合
        private List<double> windSpeedList;
        //定义对比风机的风速集合
        private List<double> compareWindSpeedList;
        //定义风向集合
        private List<double> windDirectionList;
        //定义对比风机的风向集合
        private List<double> compareWindDirectionList;
        //定义风速最大值最小值 [0]是max  [1]是min
        private double[] windSpeedMaxAndMin;
        //定义风向最大值最小值 [0]是max  [1]是min
        private double[] windDirectionMaxAndMin;
        //定义对比风机的风速最大值最小值 [0]是max  [1]是min
        private double[] compareWindSpeedMaxAndMin;
        //定义对比风机的风向最大值最小值 [0]是max  [1]是min
        private double[] compareWindDirectionMaxAndMin;
        //日期选择正确的标志
        private bool selectDateFlag = false;
        //定义当前风机的时间集合
        private List<DateTime?> dateTimeCollection;
        //定义对比风机的时间集合
        private List<DateTime?> compareDateTimeCollection;
        //存储所有省份名称
        private string[] provinceArray;
        //存储全国的公司名称
        private string[] companyArray;
        //定义起始时间
        private DateTime? startTime;
        //定义终止时间
        private DateTime? endTime;
        //存储所有原始风机和对比风机的时间集合
        private List<DateTime?> allDate;
        //标志着是否启用对比功能
        private bool compareFlag = false;
        //定义比较之后的风速最大值和最小值
        private double speedMax;
        private double speedMin;
        //定义比较之后的风向最大值和最小值
        private double directionMax;
        private double directionMin;
        public delegate void DeleDisplay(); //定义委托
        //定义进度条的数据绑定对象
        ExportProgressBar epg;
        //启用生成EXCEL功能的标志位，当生成excel功能启动后，把鼠标十字线绘制功能关闭，否则进度条不更新
        private bool excelFlag = false;
        //定义excel文件路径
        private string filePath;


        public DetailWindow()
        {
            InitializeComponent();
        }

        public DetailWindow(string companyName, string windFieldName, string fanNumber)
        {
            InitializeComponent();
            //初始化字段
            this.CompanyName = companyName;
            this.WindFieldName = windFieldName;
            this.FanNumber = fanNumber;
            //存储此窗体
            TransWindow.detailWindow = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {     
            //失能对比按钮
            btnAddCompare.IsEnabled = false;
            this.Title = this.CompanyName + "---" + this.WindFieldName + "---第 " + this.FanNumber + " 号风机";
            //获取当前风机的风场信息
            windFieldOfFan = AllWindFieldDAL.GetWindFieldByName(this.CompanyName, this.WindFieldName);
            //获得当前风机的最新数据
            FanPoint fp = FanPointDAL.GetSinglePoint(windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber);



            txtbkDigital.Text = "业主名称：" + windFieldOfFan.CompanyName + "\n"
                + "风场名称：" + windFieldOfFan.WindFieldName + "\n"
                + "地址：" + windFieldOfFan.Province + windFieldOfFan.City + windFieldOfFan.DetailAddress + "\n\n"
                + "-------------------------------\n"
                + "风机型号:" + windFieldOfFan.FanModelNumber + "\n"
                + "风机高度:" + windFieldOfFan.FanHeight + "\n"
                + "测风仪型号:" + windFieldOfFan.AnemoscopeModelNumber + "\n"
                + "-------------------------------\n"
                + "经纬度：" + fp.GPS_lng + "E  " + fp.GPS_lat + "N\n"
                + "风机号：" + fp.FanNumber + "\n"
                + "风速：" + fp.WindSpeed + "\n"
                + "风向：" + fp.WindDirection + "\n"
                + "临时IP：" + fp.TempIp + "\n"
                + "海拔：" + fp.Elevation + "\n"
                + "航向：" + fp.Course + "\n"
                + "航速：" + fp.NavigationalSpeed + "\n"
                + "温度：" + fp.Temperature + "\n"
                + "湿度：" + fp.Humidity + "\n"
                + "气压：" + fp.AirPressure + "\n";




            //预加载省份
            provinceArray = AllWindFieldDAL.GetAllProvince();
            if (provinceArray != null)
            {
                //如果查到了省份，则加载到复选框
                List<string> provinceList = new List<string>();
                provinceList.Add("全国范围");
                provinceList.AddRange(provinceArray);
                cmbProvince.ItemsSource = provinceList;//加载省份
                cmbProvince.SelectedItem = "全国范围";
            }

            //预加载所有公司名称            
            companyArray = AllWindFieldDAL.GetCompanyName();
            LoadCompany("全国范围", companyArray);
        }

        #region 日期选择器用户输入警告
        private void dpStart_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MessageBox.Show("用户输入了一个非法日期！" + e.Text);
        }

        private void dpStop_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MessageBox.Show("用户输入了一个非法日期！" + e.Text);
        }
        #endregion

        //风速绘图区尺寸变化时重绘
        private void cvsWindSpeed_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (compareFlag)
            {
                //如果启用了对比功能，则绘制对比图像
                Draw.DrawGrid(cvsWindSpeed, "#FF95310C");//此方法有清空canvas的操作
                Draw.DrawWord(cvsWindSpeed, this.CompanyName, this.WindFieldName, this.FanNumber, speedMax, speedMin, "#FF95310C", "(米/秒)", allDate, compareDateTimeCollection);
                Draw.DrawCompareCurve(cvsWindSpeed, windSpeedList, speedMax, speedMin, "#FF95310C", allDate, dateTimeCollection);
                Draw.DrawCompareCurve(cvsWindSpeed, compareWindSpeedList, speedMax, speedMin, "#FF004B00", allDate, compareDateTimeCollection);

            }
            else if (selectDateFlag)
            {
                //如果已经选择了正确的日期，则可以刷新绘图区

                Draw.DrawGrid(cvsWindSpeed, "#FF95310C");//此方法有清空canvas的操作
                Draw.DrawWord(cvsWindSpeed, this.CompanyName, this.WindFieldName, this.FanNumber, windSpeedMaxAndMin[0], windSpeedMaxAndMin[1], "#FF95310C", "(米/秒)", dateTimeCollection,null);
                Draw.DrawCurve(cvsWindSpeed, windSpeedList, windSpeedMaxAndMin[0], windSpeedMaxAndMin[1], "#FF95310C");

            }
            else
            {
                //否则只绘制网格
                Draw.DrawGrid(cvsWindSpeed, "#FF95310C");//此方法有清空canvas的操作
            }

        }
        //风向绘图区尺寸变化时重绘
        private void cvsWindDirection_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (compareFlag)
            {
                //如果启用了对比功能，则绘制对比图像
                //绘制风向图形
                Draw.DrawGrid(cvsWindDirection, "#FF4D056E");//此方法有清空canvas的操作
                Draw.DrawWord(cvsWindDirection, this.CompanyName, this.WindFieldName, this.FanNumber, directionMax, directionMin, "#FF4D056E", "(度)", allDate, compareDateTimeCollection);
                Draw.DrawCompareCurve(cvsWindDirection, windDirectionList, directionMax, directionMin, "#FF4D056E", allDate, dateTimeCollection);
                Draw.DrawCompareCurve(cvsWindDirection, compareWindDirectionList, directionMax, directionMin, "#FF004B00", allDate, compareDateTimeCollection);

            }
            else if (selectDateFlag)
            {
                //如果已经选择了正确的日期，则可以刷新绘图区
                Draw.DrawGrid(cvsWindDirection, "#FF4D056E");//此方法有清空canvas的操作
                Draw.DrawWord(cvsWindDirection, this.CompanyName, this.WindFieldName, this.FanNumber, windDirectionMaxAndMin[0], windDirectionMaxAndMin[1], "#FF4D056E", "(度)", dateTimeCollection, compareDateTimeCollection);
                Draw.DrawCurve(cvsWindDirection, windDirectionList, windDirectionMaxAndMin[0], windDirectionMaxAndMin[1], "#FF4D056E");

            }
            else
            {
                //否则只绘制网格
                Draw.DrawGrid(cvsWindDirection, "#FF4D056E");//此方法有清空canvas的操作
            }
        }


        #region 鼠标十字线提示
        //鼠标进入显示区域，刷新鼠标的十字线和提示信息
        private void cvsWindSpeed_MouseMove(object sender, MouseEventArgs e)
        {
            if (excelFlag)
            {
                //如果启动了生成excel的功能，则关闭鼠标十字线的绘制
                return;
            }
            if (compareFlag)
            {
                //如果启用了对比功能
                Draw.MouseMoveHint(cvsWindSpeed, e, speedMax, speedMin, " m/s", allDate);
                return;
            }

            if (selectDateFlag)
            {
                //没启用对比功能
                //如果已经选择了正确的日期，则可以刷新绘图区
                Draw.MouseMoveHint(cvsWindSpeed, e, windSpeedMaxAndMin[0], windSpeedMaxAndMin[1], " m/s", dateTimeCollection);
            }
        }        
        private void cvsWindDirection_MouseMove(object sender, MouseEventArgs e)
        {
            if (excelFlag)
            {
                //如果启动了生成excel的功能，则关闭鼠标十字线的绘制
                return;
            }
            if (compareFlag)
            {
                //如果启用了对比功能
                Draw.MouseMoveHint(cvsWindDirection, e, directionMax, directionMin, "°", allDate);
                return;
            }

            if (selectDateFlag)
            {
                //没启用对比功能
                //如果已经选择了正确的日期，则可以刷新绘图区
                Draw.MouseMoveHint(cvsWindDirection, e, windDirectionMaxAndMin[0], windDirectionMaxAndMin[1], "°", dateTimeCollection);
            }
        }

        private void cvsWindSpeed_MouseLeave(object sender, MouseEventArgs e)
        {
            //移除残留的鼠标轨迹
            Draw.RemoveMouseHint(cvsWindSpeed);
        }

        private void cvsWindDirection_MouseLeave(object sender, MouseEventArgs e)
        {
            //移除残留的鼠标轨迹
            Draw.RemoveMouseHint(cvsWindDirection);
        }
        #endregion

        #region 选择日期绘制曲线

        
        //选择日期加载数据
        private void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            //另开线程绘制曲线
            Task<int> drawTask = new Task<int>(() => { return DrawTask(); });
            drawTask.Start();//开始工作

        }
        private int DrawTask()
        {
            //执行委托
            this.Dispatcher.BeginInvoke(new DeleDisplay(DrawWave));
            return 0;
        }
        private void DrawWave()
        {

            //标志已经关闭对比功能
            compareFlag = false;
            //获取起始时间
            startTime = dpStart.SelectedDate;
            //获取终止时间
            endTime = dpEnd.SelectedDate;
            if (startTime == null || endTime == null)
            {
                //重置日期标志为false
                selectDateFlag = false;
                MessageBox.Show("要查询的起止时间不能为空，请选择时间！");
                return;
            }
            if (startTime >= endTime)
            {
                //重置日期标志为false
                selectDateFlag = false;
                MessageBox.Show("您选择的时间不对，请重新输入");
                return;
            }


            //获取当前风机的时间集合
            dateTimeCollection = FanPointDAL.GetDateTimeCollectionOfSingleFan(windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber, startTime, endTime);
            if (dateTimeCollection == null)
            {
                MessageBox.Show("没有检索到该风机的任何风速风向数据！", "啥也没找到", MessageBoxButton.OK, MessageBoxImage.Warning);
                Draw.DrawGrid(cvsWindSpeed, "#FF95310C");//此方法有清空canvas的操作
                Draw.DrawGrid(cvsWindDirection, "#FF4D056E");//此方法有清空canvas的操作
                //重置日期标志为false
                selectDateFlag = false;
                return;
            }

            //重置日期标志为true
            selectDateFlag = true;
            //使能对比按钮
            btnAddCompare.IsEnabled = true;

            //获取当前风机的风速集合
            windSpeedList = FanPointDAL.GetWindSpeedOrWindDirectionOfSinglePoint(FanPointDAL.Type.风速, windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber, startTime, endTime);
            //获取当前风机的风向集合
            windDirectionList = FanPointDAL.GetWindSpeedOrWindDirectionOfSinglePoint(FanPointDAL.Type.风向, windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber, startTime, endTime);
            //获取风速最大值和最小值
            windSpeedMaxAndMin = new double[2];
            windSpeedMaxAndMin = FanPointDAL.GetMaxMinDataOfSinglePoint(FanPointDAL.Type.风速, windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber, startTime, endTime);
            //获取风向最大值和最小值
            windDirectionMaxAndMin = new double[2];
            windDirectionMaxAndMin = FanPointDAL.GetMaxMinDataOfSinglePoint(FanPointDAL.Type.风向, windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber, startTime, endTime);



            //绘制风速数据
            Draw.DrawGrid(cvsWindSpeed, "#FF95310C");//此方法有清空canvas的操作
            Draw.DrawWord(cvsWindSpeed, this.CompanyName, this.WindFieldName, this.FanNumber, windSpeedMaxAndMin[0], windSpeedMaxAndMin[1], "#FF95310C", "(米/秒)", dateTimeCollection, null);
            Draw.DrawCurve(cvsWindSpeed, windSpeedList, windSpeedMaxAndMin[0], windSpeedMaxAndMin[1], "#FF95310C");

            //绘制风向数据
            Draw.DrawGrid(cvsWindDirection, "#FF4D056E");//此方法有清空canvas的操作
            Draw.DrawWord(cvsWindDirection, this.CompanyName, this.WindFieldName, this.FanNumber, windDirectionMaxAndMin[0], windDirectionMaxAndMin[1], "#FF4D056E", "(度)", dateTimeCollection, null);
            Draw.DrawCurve(cvsWindDirection, windDirectionList, windDirectionMaxAndMin[0], windDirectionMaxAndMin[1], "#FF4D056E");

            return;
        }

        #endregion

        #region 下拉列表加载
        /// <summary>
        /// 加载公司名称到下拉列表
        /// </summary>
        /// <param name="place"></param>
        /// <param name="companyS"></param>
        private void LoadCompany(string place, string[] companyS)
        {
            List<string> companyList = new List<string>();
            if (companyS == null)
            {
                return;
            }
            companyList.Add("公司名称");
            companyList.AddRange(companyS);
            cmbCompanyName.ItemsSource = companyList;
            cmbCompanyName.SelectedItem = "公司名称";
        }
        /// <summary>
        /// 加载风场名称
        /// </summary>
        /// <param name="windField"></param>
        /// <param name="fanCount"></param>
        private void LoadWindFieldName(string company, string province, string city, AllWindField[] windField, int fanCount)
        {
            //填充风场下拉列表
            List<string> windFieldNameList = new List<string>();
            windFieldNameList.Add("风场名称");
            for (int i = 0; i < windField.Length; i++)
            {
                windFieldNameList.Add(windField[i].WindFieldName);
            }
            cmbWindFieldName.ItemsSource = windFieldNameList;
            cmbWindFieldName.SelectedItem = "风场名称";
        }
        /// <summary>
        /// 显示风场信息
        /// </summary>
        private void DisplayNameOfWindField()
        {
            //获取选中的公司名字
            string selectedCompany = cmbCompanyName.SelectedItem.ToString();
            if (cmbProvince.SelectedItem.ToString() == "全国范围")
            {
                //获取全国范围对应业主的风场名称
                AllWindField[] windField = AllWindFieldDAL.GetWindFieldNameAndFanCountByCompanyName(selectedCompany);
                if (windField == null)
                {
                    return;
                }
                //获取该公司的风机数量
                int fanCount = AllWindFieldDAL.GetFanCountSum(selectedCompany);
                //加载风场下拉列表
                LoadWindFieldName(selectedCompany, "全国范围", "", windField, fanCount);
            }
            else if (cmbProvince.SelectedItem.ToString() != "全国范围" && cmbCity.SelectedItem == null)
            {
                //获取某省份对应业主的风场名称
                AllWindField[] windfield = AllWindFieldDAL.GetWindFieldNameAndFanCountByPlace(cmbProvince.SelectedItem.ToString(), null, cmbCompanyName.SelectedItem.ToString());
                if (windfield == null)
                {
                    return;
                }
                //获取该公司在该省份的风机数量
                int fanCount = AllWindFieldDAL.GetFanCountSumByPlace(selectedCompany, cmbProvince.SelectedItem.ToString());
                //加载风场下拉列表
                LoadWindFieldName(selectedCompany, cmbProvince.SelectedItem.ToString(), "", windfield, fanCount);
            }
            else if (cmbProvince.SelectedItem.ToString() != "全国范围" && cmbCity.SelectedItem != null)
            {
                //获取某省某市对应业主的风场信息
                //获取某省份对应业主的风场名称
                AllWindField[] windfield = AllWindFieldDAL.GetWindFieldNameAndFanCountByPlace(cmbProvince.SelectedItem.ToString(), cmbCity.SelectedItem.ToString(), cmbCompanyName.SelectedItem.ToString());
                if (windfield == null)
                {
                    return;
                }
                //获取该公司在该省份的风机数量
                int fanCount = AllWindFieldDAL.GetFanCountSumByPlace(selectedCompany, cmbProvince.SelectedItem.ToString(), cmbCity.SelectedItem.ToString());
                //加载风场下拉列表
                LoadWindFieldName(selectedCompany, cmbProvince.SelectedItem.ToString(), cmbCity.SelectedItem.ToString(), windfield, fanCount);
            }
        }

        //选择省份
        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProvince.SelectedItem == null)
            {
                return;
            }
            if ((string)cmbProvince.SelectedItem == "全国范围")
            {
                //清空城市列表
                cmbCity.ItemsSource = null;
                //加载全国范围的公司
                LoadCompany("全国范围", companyArray);
                //清空风场列表
                cmbWindFieldName.ItemsSource = null;
                return;
            }
            //选中的省份改变时，获取对应城市信息，并加载到城市列表中
            //获取选中的省份名称
            string selectedProvince = cmbProvince.SelectedItem.ToString();
            //根据选中的省份名称去数据库查询包含的城市
            string[] citys = AllWindFieldDAL.GetCityOfProvince(selectedProvince);
            cmbCity.ItemsSource = citys;//加载城市名称
            //加载选中省份的公司名称
            string[] companys = AllWindFieldDAL.GetCompanyOfProvince(selectedProvince);
            LoadCompany(selectedProvince, companys);//记载该省份的公司名到下拉列表
        }
        //选择城市
        private void cmbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCity.SelectedItem == null)
            {
                return;
            }
            //获取选择的城市名称
            string selectedCity = cmbCity.SelectedItem.ToString();
            //获取对应省份和城市名称的风场
            string[] companys = AllWindFieldDAL.GetCompanyByProvinceAndCity(cmbProvince.SelectedItem.ToString(), selectedCity);
            LoadCompany(cmbProvince.SelectedItem.ToString() + selectedCity, companys);//加载城市列表
        }
        //选择公司
        private void cmbCompanyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCompanyName.SelectedItem == null)
            {
                return;
            }
            if (cmbCompanyName.SelectedItem.ToString()=="公司名称")
            {
                cmbWindFieldName.ItemsSource = null;
                cmbWindFanNumber.ItemsSource = null;
                return;
            }
            //选择的公司改变时，展示相应公司的风场名字
            DisplayNameOfWindField();
        }
        //选择风场
        private void cmbWindFieldName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbWindFieldName.SelectedItem == null)
            {
                return;
            }
            if (cmbWindFieldName.SelectedItem.ToString() == "风场名称")
            {
                cmbWindFanNumber.ItemsSource = null;
                return;
            }
            //获得此风机所在的表名 awf.BranchTableName
            windFieldOfCompareFan = AllWindFieldDAL.GetWindFieldByName(cmbCompanyName.SelectedItem.ToString(), cmbWindFieldName.SelectedItem.ToString());
            //获得此风场下的所有风机编号
            int[] fanNumber = FanPointDAL.GetFanNumber(windFieldOfCompareFan.BranchTableName, cmbWindFieldName.SelectedItem.ToString());
            cmbWindFanNumber.ItemsSource = fanNumber;
        }


        //选择风机
        private void cmbWindFanNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbWindFanNumber.ItemsSource == null)
            {
                return;
            }
        }
        #endregion

        #region 绘制对比曲线

        //返回最大值
        private double GetMax(double d1, double d2)
        {
            return d1 > d2 ? d1 : d2;
        }
        //返回最小值
        private double GetMin(double d1, double d2)
        {
            return d1 < d2 ? d1 : d2;
        }

        //加入对比
        private void btnAddCompare_Click(object sender, RoutedEventArgs e)
        {
            if (cmbWindFanNumber.SelectedItem==null)
            {
                //标志已经关闭对比功能
                compareFlag = false;
                MessageBox.Show("请先选择要加入对比的风机再执行此操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //获取当前风机的时间集合
            compareDateTimeCollection = FanPointDAL.GetDateTimeCollectionOfSingleFan(windFieldOfCompareFan.BranchTableName, cmbWindFieldName.SelectedItem.ToString(), cmbWindFanNumber.SelectedItem.ToString(), startTime, endTime);
            if (compareDateTimeCollection == null)
            {
                //标志已经关闭对比功能
                compareFlag = false;
                MessageBox.Show("没有检索到该风机的任何风速风向数据！", "啥也没找到", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //另开线程绘制对比曲线
            Task<int> drawTask = new Task<int>(() => { return DrawCompareTask(); });
            drawTask.Start();//开始工作
        }
        private int DrawCompareTask()
        {
            //执行委托
            this.Dispatcher.BeginInvoke(new DeleDisplay(DrawCompareWave));
            return 0;
        }

        private void DrawCompareWave()
        {

            //关闭日期标志
            selectDateFlag = false;
            
            //标志已经启用对比功能
            compareFlag = true;
            //获取该风机点的风速集合
            compareWindSpeedList = FanPointDAL.GetWindSpeedOrWindDirectionOfSinglePoint(FanPointDAL.Type.风速, windFieldOfCompareFan.BranchTableName, cmbWindFieldName.SelectedItem.ToString(), cmbWindFanNumber.SelectedItem.ToString(), startTime, endTime);
            //获取该风机点的风向集合
            compareWindDirectionList = FanPointDAL.GetWindSpeedOrWindDirectionOfSinglePoint(FanPointDAL.Type.风向, windFieldOfCompareFan.BranchTableName, cmbWindFieldName.SelectedItem.ToString(), cmbWindFanNumber.SelectedItem.ToString(), startTime, endTime);
            //获取风速最大值和最小值
            compareWindSpeedMaxAndMin = new double[2];
            compareWindSpeedMaxAndMin = FanPointDAL.GetMaxMinDataOfSinglePoint(FanPointDAL.Type.风速, windFieldOfCompareFan.BranchTableName, cmbWindFieldName.SelectedItem.ToString(), cmbWindFanNumber.SelectedItem.ToString(), startTime, endTime);
            //获取风向最大值和最小值
            compareWindDirectionMaxAndMin = new double[2];
            compareWindDirectionMaxAndMin = FanPointDAL.GetMaxMinDataOfSinglePoint(FanPointDAL.Type.风向, windFieldOfCompareFan.BranchTableName, cmbWindFieldName.SelectedItem.ToString(), cmbWindFanNumber.SelectedItem.ToString(), startTime, endTime);

            //获取比较之后的风速最大值和最小值
            speedMax = GetMax(windSpeedMaxAndMin[0], compareWindSpeedMaxAndMin[0]);
            speedMin = GetMin(windSpeedMaxAndMin[1], compareWindSpeedMaxAndMin[1]);
            //获取比较之后的风向最大值和最小值
            directionMax = GetMax(windDirectionMaxAndMin[0], compareWindDirectionMaxAndMin[0]);
            directionMin = GetMin(windDirectionMaxAndMin[1], compareWindDirectionMaxAndMin[1]);

            //重新构建时间集合
            //将原始风机的时间加入新集合
            allDate = new List<DateTime?>();
            for (int i = 0; i < dateTimeCollection.Count; i++)
            {
                dateTimeCollection[i] = dateTimeCollection[i].Value.Date;
                allDate.Add(dateTimeCollection[i].Value.Date);
            }
            //将对比风机的时间加入新集合
            for (int i = 0; i < compareDateTimeCollection.Count; i++)
            {
                compareDateTimeCollection[i] = compareDateTimeCollection[i].Value.Date;
                if (allDate.Contains(compareDateTimeCollection[i].Value.Date) == false)
                {
                    allDate.Add(compareDateTimeCollection[i].Value.Date);
                }
            }
            allDate = allDate.OrderBy(d => d.Value.Date).ToList();//对日期排序

            //绘制风速图形
            Draw.DrawGrid(cvsWindSpeed, "#FF95310C");//此方法有清空canvas的操作
            Draw.DrawWord(cvsWindSpeed, this.CompanyName, this.WindFieldName, this.FanNumber, speedMax, speedMin, "#FF95310C", "(米/秒)", allDate, compareDateTimeCollection);
            Draw.DrawCompareCurve(cvsWindSpeed, windSpeedList, speedMax, speedMin, "#FF95310C", allDate, dateTimeCollection);
            Draw.DrawCompareCurve(cvsWindSpeed, compareWindSpeedList, speedMax, speedMin, "#FF104B00", allDate, compareDateTimeCollection);

            //绘制风向图形
            Draw.DrawGrid(cvsWindDirection, "#FF4D056E");//此方法有清空canvas的操作
            Draw.DrawWord(cvsWindDirection, this.CompanyName, this.WindFieldName, this.FanNumber, directionMax, directionMin, "#FF4D056E", "(度)", allDate, compareDateTimeCollection);
            Draw.DrawCompareCurve(cvsWindDirection, windDirectionList, directionMax, directionMin, "#FF4D056E", allDate, dateTimeCollection);
            Draw.DrawCompareCurve(cvsWindDirection, compareWindDirectionList, directionMax, directionMin, "#FF104B00", allDate, compareDateTimeCollection);
        }
        #endregion

        #region 创建 EXCEL 表格
        //生成EXCEL
        private void btnExistExcel_Click(object sender, RoutedEventArgs e)
        {
            if (dateTimeCollection==null)
            {
                MessageBox.Show("需要加载数据才能生成Excel", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "选择保存Excel的路径";
            saveDialog.Filter = "工作表|*.xlsx|所有文件|*.*";
            bool? saveFlag = saveDialog.ShowDialog();
            if (!(bool)saveFlag)
            {
                //取消保存
                return;
            }
            filePath = saveDialog.FileName;
            if (filePath == null)
            {
                MessageBox.Show("保存文件名不可为空");
                return;
            }

            //excel标志位置为true
            excelFlag = true;
            //创建进度条数据绑定对象
            epg = new ExportProgressBar();
            //绑定
            pgb.DataContext = epg;
            pgbCount.DataContext = epg;
            //启动新线程，开始创建excel文件
            task_Task();
           
        }
        /// <summary>
        /// task线程执行导出Excel表的任务
        /// </summary>
        private void task_Task()
        {
            //另开线程工作
            Task<int> createTask = new Task<int>(CreateExcel);
            createTask.ContinueWith(taskLater);//工作完毕后执行的方法
            createTask.Start();//开始工作              
        }

        // 创建excel
        private int CreateExcel()
        {
            //获取该风机点的所有数据
            List<FanPoint> fanDate = FanPointDAL.GetDateOfSingleFanByDate(windFieldOfFan.BranchTableName, this.WindFieldName, this.FanNumber, startTime, endTime);
            //设置excel的标题名
            string excelTitle = this.CompanyName + "---" + this.WindFieldName + this.FanNumber + "号风机数据(" + fanDate[0].DateTime + "-" + fanDate[fanDate.Count - 1].DateTime + ")";
            //设置地址
            string address = "地址：" + windFieldOfFan.Province + windFieldOfFan.City + windFieldOfFan.DetailAddress;
            //创建EXCEL
            bool excelResult = SaveExcel.CreateExcel(filePath, excelTitle, address, fanDate, epg);
            return 0;
        }
        //生成成功后提醒用户是否打开文件
        private void taskLater(Task<int> arg)
        {
            //excel标志位置为false
            excelFlag = false;

            MessageBoxResult mbr = MessageBox.Show("导出成功！是否打开导出的Excel文件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (mbr == MessageBoxResult.Yes) //打开导出的文件
            {
                //打开指定的文件                
                System.Diagnostics.Process.Start(filePath);
            }
        }
        #endregion
    }
}
