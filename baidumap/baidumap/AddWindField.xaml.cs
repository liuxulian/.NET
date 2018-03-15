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
    /// AddWindField.xaml 的交互逻辑
    /// </summary>
    public partial class AddWindField : Window
    {
        public AddWindField()
        {
            InitializeComponent();
            TransWindow.addWindField = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            baseGrid.Visibility = Visibility.Visible;
            addCompanyGrid.Visibility = Visibility.Hidden;
            addWindFieldGrid.Visibility = Visibility.Hidden;

            //获得所有公司名称
            string[] province = AllWindFieldDAL.GetCompanyName();
            cmbCompanyName.ItemsSource = province;
            cmbCompanyName.SelectedIndex = 0;
        }
        //选择添加新风场
        private void btnAddWindField_Click(object sender, RoutedEventArgs e)
        {
            baseGrid.Visibility = Visibility.Hidden;
            addCompanyGrid.Visibility = Visibility.Hidden;
            addWindFieldGrid.Visibility = Visibility.Visible;
        }
        //选择添加新业主
        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            baseGrid.Visibility = Visibility.Hidden;
            addWindFieldGrid.Visibility = Visibility.Hidden;
            addCompanyGrid.Visibility = Visibility.Visible;

            MessageBox.Show("注意：接下来看到的“数据表名”是在数据库中即将要归属于新业主的数据表名，例如：大唐公司的表名为 T_DaTang，为保持表名风格统一，请按照 'T_' 加名字拼音的格式定义！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                
        }

        //确认提交新风场
        private void btnUploadData_Click(object sender, RoutedEventArgs e)
        {         
            if (cmbCompanyName.SelectedItem == null || txtWindFieldName.Text == string.Empty
                || txtProvince.Text == string.Empty || txtCity.Text == string.Empty)
            {
                MessageBox.Show("必填项不可为空，请填写完整后重试！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //检测该风场是否已在数据库存在
            AllWindField awf = AllWindFieldDAL.GetWindFieldByName(cmbCompanyName.SelectedItem.ToString(), txtWindFieldName.Text);
            if (awf!=null)
            {
                MessageBox.Show("该风场已经在数据库里存在，请检查风场名是否正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
           
            //根据选中的业主名获取公司数据库表名
            string tableName = AllWindFieldDAL.GetTableName(cmbCompanyName.SelectedItem.ToString());
            //创建风场对象
            AllWindField newWindField = new AllWindField();
            newWindField.BranchTableName = tableName;
            newWindField.CompanyName = cmbCompanyName.SelectedItem.ToString();
            newWindField.WindFieldName = txtWindFieldName.Text;
            newWindField.Province = txtProvince.Text;
            newWindField.City = txtCity.Text;
            newWindField.DetailAddress = txtDetailAddress.Text;
            newWindField.FanType = txtFanType.Text;
            newWindField.FanCount = 0;
            newWindField.FanModelNumber = txtFanModelNumber.Text;
            newWindField.FanHeight = txtFanHeight.Text;
            newWindField.AnemoscopeModelNumber = txtAnemoscopeModelNumber.Text;
            newWindField.SignalKind = txtSignalKind.Text;
            
            //上传数据
            int insertResult = AllWindFieldDAL.InsertData(newWindField);
            if (insertResult <= 0)
            {
                MessageBox.Show("数据提交失败，请检查网络或数据库连接是否正常！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                MessageBox.Show("恭喜您，新风场添加成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }


        //确认提交新业主
        private void TbtnUploadData_Click(object sender, RoutedEventArgs e)
        {
            
            if (TtxtCompanyName.Text==string.Empty || TtxtWindFieldName.Text == string.Empty
              ||TtxtTableName.Text==string.Empty || TtxtProvince.Text == string.Empty || TtxtCity.Text == string.Empty)
            {
                MessageBox.Show("必填项不可为空，请填写完整后重试！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //检测该业主是否已在数据库存在
            int isExistOfCompany = AllWindFieldDAL.IsExistOfCompany(TtxtCompanyName.Text);
            if (isExistOfCompany > 0)
            {
                MessageBox.Show("该业主已经在数据库里存在，无需再次添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //判断输入的数据表名是否在数据库里已经存在
            int isExistOfTable = AllWindFieldDAL.IsExistOfTable(TtxtTableName.Text);
            if (isExistOfTable>0)
            {
                MessageBox.Show("该数据表名已经在数据库里存在，请更改后重试！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //创建风场对象
            AllWindField newWindField = new AllWindField();
            newWindField.BranchTableName = TtxtTableName.Text;
            newWindField.CompanyName = TtxtCompanyName.Text;
            newWindField.WindFieldName = TtxtWindFieldName.Text;
            newWindField.Province = TtxtProvince.Text;
            newWindField.City = TtxtCity.Text;
            newWindField.DetailAddress = TtxtDetailAddress.Text;
            newWindField.FanType = TtxtFanType.Text;
            newWindField.FanCount = 0;
            newWindField.FanModelNumber = TtxtFanModelNumber.Text;
            newWindField.FanHeight = TtxtFanHeight.Text;
            newWindField.AnemoscopeModelNumber = TtxtAnemoscopeModelNumber.Text;
            newWindField.SignalKind = TtxtSignalKind.Text;

            //上传数据
            int insertResult = AllWindFieldDAL.InsertData(newWindField);
            if (insertResult <= 0)
            {
                MessageBox.Show("数据提交失败，请检查网络或数据库连接是否正常！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //创建对应公司的表格
            AllWindFieldDAL.CreateTable(TtxtTableName.Text);
            MessageBox.Show("恭喜您，新业主添加成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);                
        }

        //返回首页
        private void btnBackWindField_Click(object sender, RoutedEventArgs e)
        {
            //显示首页，隐藏另外两个子页面
            baseGrid.Visibility = Visibility.Visible;
            addCompanyGrid.Visibility = Visibility.Hidden;
            addWindFieldGrid.Visibility = Visibility.Hidden;
        }

        private void btnBackCompany_Click(object sender, RoutedEventArgs e)
        {
            //显示首页，隐藏另外两个子页面
            baseGrid.Visibility = Visibility.Visible;
            addCompanyGrid.Visibility = Visibility.Hidden;
            addWindFieldGrid.Visibility = Visibility.Hidden;
        }

    
    }
}
