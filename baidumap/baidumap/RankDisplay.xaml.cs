using baidumap.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace baidumap
{
    /// <summary>
    /// RankDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class RankDisplay : Window
    {
        //存储所有省份名称
        private string[] provinceArray;
        //存储全国的公司名称
        private string[] companyArray;
        
        public RankDisplay()
        {
            InitializeComponent();
            //存储此窗体
            TransWindow.rankDisplay = this;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //预加载省份
            provinceArray = AllWindFieldDAL.GetAllProvince();
            if (provinceArray!=null)
            {
                //如果查到了省份，则加载到复选框
                List<string> provinceList = new List<string>();
                provinceList.Add("全国范围");
                provinceList.AddRange(provinceArray);
                cmbProvince.ItemsSource = provinceList;//加载省份
                cmbProvince.SelectedItem = "全国范围";
            }

            //预加载所有公司名称            
            companyArray=AllWindFieldDAL.GetCompanyName();
            LoadCompany("全国范围", companyArray);
        }


        //选择省份
        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProvince.SelectedItem==null)
            {
                return;
            }
            if ((string)cmbProvince.SelectedItem=="全国范围")
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
            if (cmbCity.SelectedItem==null)
            {
                return;
            }
            //获取选择的城市名称
            string selectedCity = cmbCity.SelectedItem.ToString();
            //获取对应省份和城市名称的风场
            string[] companys = AllWindFieldDAL.GetCompanyByProvinceAndCity(cmbProvince.SelectedItem.ToString(), selectedCity);
            LoadCompany(cmbProvince.SelectedItem.ToString() + selectedCity, companys);//加载城市列表
        }


        /// <summary>
        /// 选择公司后的相应操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCompanyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCompanyName.SelectedItem==null)
            {
                return;
            }
            if ((string)cmbCompanyName.SelectedItem=="公司名称")
            {
                cmbWindFieldName.ItemsSource = null;//清空风场选择框的信息     
                if (cmbProvince.SelectedItem.ToString()!="全国范围"&&cmbCity.SelectedItem==null)
                {
                    //只加载该省包含的公司信息
                    //加载选中省份的公司名称
                    string[] companys = AllWindFieldDAL.GetCompanyOfProvince(cmbProvince.SelectedItem.ToString());
                    txtDisplay.Text = cmbProvince.SelectedItem.ToString() + "接入服务的业主有 " + companys.Length + " 家：\n\n";
                    for (int i = 0; i < companys.Length; i++)
                    {
                        txtDisplay.AppendText(companys[i] + "   ");
                    }
                }
                else if (cmbCity.SelectedItem!=null)
                {
                    //加载某省某市的公司信息
                    //获取对应省份和城市名称的风场
                    string[] companys = AllWindFieldDAL.GetCompanyByProvinceAndCity(cmbProvince.SelectedItem.ToString(), cmbCity.SelectedItem.ToString());
                    txtDisplay.Text = cmbProvince.SelectedItem.ToString()+cmbCity.SelectedItem.ToString() + "接入服务的业主有 " + companys.Length + " 家：\n\n";
                    for (int i = 0; i < companys.Length; i++)
                    {
                        txtDisplay.AppendText(companys[i] + "   ");
                    }
                }

                return;
            }


            //选择的公司改变时，展示相应公司的风场名字
            DisplayNameOfWindField();

        }
        /// <summary>
        /// 选择风场
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbWindFieldName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( cmbWindFieldName.SelectedItem==null)
            {
                return;
            }
            if ((string)cmbWindFieldName.SelectedItem == "风场名称")
            {
                //展示所有的风场名字
                DisplayNameOfWindField();
                return;
            }

            AllWindField windField = AllWindFieldDAL.GetWindFieldByName((string)cmbCompanyName.SelectedItem, (string)cmbWindFieldName.SelectedItem);
            if (windField==null)
            {
                txtDisplay.Text = "没有查到该风场的信息...";
                return;
            }
            txtDisplay.Text = "业主名称：" + windField.CompanyName
                + "\n风场名称：" + windField.WindFieldName
                + "\n详细地址：" + windField.Province + windField.City + windField.DetailAddress
                + "\n风机数量：" + windField.FanCount + " 台"
                + "\n风机类型：" + windField.FanType
                + "\n风机型号：" + windField.FanModelNumber
                + "\n风机高度：" + windField.FanHeight + " 米"
                + "\n测风仪型号：" + windField.AnemoscopeModelNumber
                + "\n信号模式：" + windField.SignalKind;
        }
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
                txtDisplay.Text = "没有找到任何公司...";
                return;
            }
            companyList.Add("公司名称");
            companyList.AddRange(companyS);
            cmbCompanyName.ItemsSource = companyList;
            cmbCompanyName.SelectedItem = "公司名称";
            txtDisplay.Text = place + "接入服务的业主有 " + companyS.Length + " 家：\n\n";
            for (int i = 0; i < companyS.Length; i++)
            {
                txtDisplay.AppendText(companyS[i] + "   ");
            }
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

            txtDisplay.Text = company+"在"+province+city+ "有风机 " + fanCount + " 台\n"
                + "接入服务的风场有 " + windField.Length + " 个：\n\n";
            for (int i = 0; i < windField.Length; i++)
            {
                txtDisplay.AppendText(windField[i].WindFieldName + " " + windField[i].FanCount + "台    ");
            }
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
                    txtDisplay.Text = "该业主还没有接入服务的风场...";
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
                    txtDisplay.Text = "该业主在" + cmbProvince.SelectedItem.ToString() + "还没有接入服务的风场...";
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
                    txtDisplay.Text = "该业主在" + cmbProvince.SelectedItem.ToString() + cmbCity.SelectedItem.ToString() + "还没有接入服务的风场...";
                    return;
                }
                //获取该公司在该省份的风机数量
                int fanCount = AllWindFieldDAL.GetFanCountSumByPlace(selectedCompany, cmbProvince.SelectedItem.ToString(), cmbCity.SelectedItem.ToString());
                //加载风场下拉列表
                LoadWindFieldName(selectedCompany, cmbProvince.SelectedItem.ToString(), cmbCity.SelectedItem.ToString(), windfield, fanCount);
            }
        }

    }
}
