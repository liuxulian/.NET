using baidumap.DAL;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace baidumap.Excel
{
    public class SaveExcel
    {
        public static bool CreateExcel(string path, string tableTitle,string address, List<FanPoint> fanDate,ExportProgressBar epg)
        {
            //进度条给用户提示
            epg.PgPercentText = "正在配置...";
            #region 创建表格
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);//创建工作簿（WorkBook：即Excel文件主体本身）
            Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];//创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出            
            //Worksheet excelWS2 = (Worksheet)excelWB.Worksheets.Add(Type.Missing); //新建子表 excelWS2
            excelWS.Cells.NumberFormat = "@";     //  如果数据中存在数字类型 可以让它变文本格式显示  
            #endregion

            #region 表头属性设置
            Range range1 = null;
            range1 = (Range)excelWS.get_Range("A1", "L1"); //获取Excel多个单元格区域：本例做为Excel表头 
            range1.Merge(0); //单元格合并动作   要配合上面的get_Range()进行设计
            range1.Font.Size = 18; //设置字体大小
            range1.Font.Underline = false; //设置字体是否有下划线
            range1.Font.Name = "黑体"; //设置字体的种类 
            range1.HorizontalAlignment = XlHAlign.xlHAlignCenter; //设置字体在单元格内的对其方式 
            range1.ColumnWidth = 50; //设置单元格的宽度，如果已经设置为自动列宽则此设置不起作用
            range1.RowHeight = 50;//设置行高
            range1.Cells.Interior.Color = System.Drawing.Color.FromArgb(10, 11, 253).ToArgb(); //设置单元格的背景色             
            //range1.Borders.LineStyle = 0.5; //设置单元格边框的粗细 
            //range1.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.FromArgb(255, 204, 153).ToArgb()); //给单元格加边框 
            //range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone; //设置单元格上边框为无边框 
            range1.EntireColumn.AutoFit(); //自动调整列宽 
            range1.WrapText = true; //文本自动换行 
            range1.Interior.ColorIndex = 50; //填充颜色为绿色
            //range.Font.Color = System.Drawing.Color.FromArgb(255, 204, 153).ToArgb(); //字体颜色 
            range1.Font.Color = System.Drawing.Color.White; //字体颜色 
            #endregion

            #region 第二行项目属性设置
            Range range2 = (Range)excelWS.get_Range("A2", "L2");
            range2.Merge(0); //单元格合并动作   要配合上面的get_Range()进行设计
            range2.Font.Size = 15; //设置字体大小
            range2.HorizontalAlignment = XlHAlign.xlHAlignCenter; //设置字体在单元格内的对其方式 
            range2.RowHeight = 30;
            range2.Interior.ColorIndex = 50; //填充颜色为绿色
            range2.EntireColumn.AutoFit(); //自动调整列宽
            range2.WrapText = true; //文本自动换行      
            range2.Font.Color = System.Drawing.Color.LightGray; //字体颜色
            //range2.Borders.LineStyle = 0.1; //设置单元格边框的粗细 
            //range2.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.FromArgb(255, 204, 153).ToArgb()); //给单元格加边框 
            //range2.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone; //设置单元格上边框为无边框             
            #endregion   
     
            #region 第三行项目属性设置
            Range range3 = (Range)excelWS.get_Range("A3", "L3");
            range3.Font.Size = 15; //设置字体大小
            range3.HorizontalAlignment = XlHAlign.xlHAlignCenter; //设置字体在单元格内的对其方式 
            range3.Cells.Interior.Color = System.Drawing.Color.LightBlue; //设置背景色
            range3.EntireColumn.AutoFit(); //自动调整列宽
            range3.WrapText = true; //文本自动换行 
            //range3.Borders.LineStyle = 0.1; //设置单元格边框的粗细 
            //range3.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.FromArgb(255, 204, 153).ToArgb()); //给单元格加边框 
            //range3.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone; //设置单元格上边框为无边框             
            #endregion   


            excelWS.Name = "风机数据"; //设置工作表名
            excelWS.Cells[1, 1] = tableTitle; //Excel单元格赋值 
            excelWS.Cells[2, 1] = address; //Excel单元格赋值 
            excelWS.Cells[3, 1] = "时间"; //Excel单元格赋值 
            excelWS.Cells[3, 2] = "风场编号"; //Excel单元格赋值 
            excelWS.Cells[3, 3] = "风机编号"; //Excel单元格赋值 
            excelWS.Cells[3, 4] = "风速(m/s)"; //Excel单元格赋值 
            excelWS.Cells[3, 5] = "风向(度)"; //Excel单元格赋值 
            excelWS.Cells[3, 6] = "临时IP"; //Excel单元格赋值 
            excelWS.Cells[3, 7] = "海拔"; //Excel单元格赋值 
            excelWS.Cells[3, 8] = "航向"; //Excel单元格赋值 
            excelWS.Cells[3, 9] = "航速"; //Excel单元格赋值 
            excelWS.Cells[3, 10] = "温度"; //Excel单元格赋值 
            excelWS.Cells[3, 11] = "湿度"; //Excel单元格赋值 
            excelWS.Cells[3, 12] = "气压"; //Excel单元格赋值 

            //写详细数据
            for (int i = 0; i < fanDate.Count; i++)
            {
                excelWS.Cells[4 + i, 1] = fanDate[i].DateTime;
                excelWS.Cells[4 + i, 2] = fanDate[i].WindFieldNumber;
                excelWS.Cells[4 + i, 3] = fanDate[i].FanNumber;
                excelWS.Cells[4 + i, 4] = fanDate[i].WindSpeed;
                excelWS.Cells[4 + i, 5] = fanDate[i].WindDirection;
                excelWS.Cells[4 + i, 6] = fanDate[i].TempIp;
                excelWS.Cells[4 + i, 7] = fanDate[i].Elevation;
                excelWS.Cells[4 + i, 8] = fanDate[i].Course;
                excelWS.Cells[4 + i, 9] = fanDate[i].NavigationalSpeed;
                excelWS.Cells[4 + i, 10] = fanDate[i].Temperature;
                excelWS.Cells[4 + i, 11] = fanDate[i].Humidity;
                excelWS.Cells[4 + i, 12] = fanDate[i].AirPressure;
                //更新进度条
                epg.PgPercent = 100.0*i / (fanDate.Count-1);
                epg.PgPercentText = epg.PgPercent.ToString("F2")+"%";
            }

            //获得某列，并让其自动适应列宽，此操作需要在数据导入之后进行才有效
            Range allColumn = excelWS.Range["A4", excelWS.Cells[fanDate.Count+3, 12]]; //选择有内容的单元格的范围
            allColumn.EntireColumn.AutoFit();
            allColumn.HorizontalAlignment = XlHAlign.xlHAlignCenter;//设置居中对齐
            allColumn.Borders.LineStyle = 1; //设置单元格边框的粗细 
            allColumn.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.FromArgb(255, 204, 153).ToArgb()); //给单元格加边框 
           
            //冻结单元格操作应该放在导入数据之后，否则会严重拖慢写入数据的速度
            #region 冻结单元格
            Range rr = (Range)excelWS.get_Range("A4", "A4"); //选中一个新的单元格区域，用于冻结此单元格之上的所有行
            rr.Select(); //选中该单元格，否则无法准确冻结指定的区域
            excelApp.ActiveWindow.FreezePanes = true; //冻结选中单元格以上的所有行
            #endregion

            excelApp.DisplayAlerts = false; //设置禁止弹出保存和覆盖的询问提示框，直接进行保存 
            excelWB.SaveAs(path);  //将其进行保存到指定的路径  
            excelWB.Close();
            excelApp.Quit();  
            KillAllExcel(excelApp); //释放可能还没释放的进程                         
            return true;
        }


        //有时候Excel会长时间占用进程，那么我们需要做释放进程的操作。
        #region 释放Excel进程
        public static bool KillAllExcel(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            try
            {
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    //释放COM组件，其实就是将其引用计数减1     
                    //System.Diagnostics.Process theProc;     
                    foreach (System.Diagnostics.Process theProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                    {
                        //先关闭图形窗口。如果关闭失败.有的时候在状态里看不到图形窗口的excel了，     
                        //但是在进程里仍然有EXCEL.EXE的进程存在，那么就需要释放它     
                        if (theProc.CloseMainWindow() == false)
                        {
                            theProc.Kill();
                        }
                    }
                    excelApp = null;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion  
    }
}
