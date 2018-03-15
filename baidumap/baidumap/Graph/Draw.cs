using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace baidumap.Graph
{
    class Draw
    {
        //定义边框和canvas边界的距离
        private static int distanceL = 35;//左
        private static int distanceT = 35;//顶
        private static int distanceR = 10;//右
        private static int distanceB = 25;//底
        //定义纵向格线的间距
        private static double unitX;
        //定义横向格线的间距
        private static double unitY;
        //定义数据的宽容度
        private static double dataTole = 1.2;


        /// <summary>
        /// 绘制直线 #FF4D056E(紫色)  #FF0968A6(蓝色)
        /// </summary>
        /// <param name="strokeThickness">线宽</param>
        /// <param name="x1">第一个点的 x 坐标</param>
        /// <param name="y1">第一个点的 y 坐标</param>
        /// <param name="x2">第二个点的 x 坐标</param>
        /// <param name="y2">第二个点的 y 坐标</param>
        public static Line DrawLine(string color, double strokeThickness, double x1, double y1, double x2, double y2)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            line.StrokeThickness = strokeThickness;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            return line;
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="cvs">需要绘制网格的Canvas</param>
        public static void DrawGrid(Canvas cvs, string color)
        {
            cvs.Children.Clear(); //清空之前的网格


            //绘制周围的边框
            Line leftBorder = DrawLine(color, 0.7, distanceL, distanceT, distanceL, cvs.ActualHeight - distanceB);//左边框
            Line bottomBorder = DrawLine(color, 0.7, distanceL, cvs.ActualHeight - distanceB, cvs.ActualWidth - distanceR, cvs.ActualHeight - distanceB);//下边框
            Line rightBorder = DrawLine(color, 0.7, cvs.ActualWidth - distanceR, cvs.ActualHeight - distanceB, cvs.ActualWidth - distanceR, distanceT);//右边框
            Line topBorder = DrawLine(color, 0.7, cvs.ActualWidth - distanceR, distanceT, distanceL, distanceT);//上边框

            //定义纵向格线的间距(准备分为20个小格)
            unitX = (cvs.ActualWidth - distanceL - distanceR) / 20;
            //定义横向格线的间距(准备分为20个小格)
            unitY = (cvs.ActualHeight - distanceT - distanceB) / 20;
            Line[] lineX = new Line[19];
            Line[] lineY = new Line[19];
            for (int i = 1; i < 20; i++)//绘制 9 条线，得到 10 个格
            {
                //横线
                lineX[i - 1] = new Line();
                lineX[i - 1] = DrawLine(color, 0.15, distanceL, distanceT + unitY * i, cvs.ActualWidth - distanceR, distanceT + unitY * i);
                cvs.Children.Add(lineX[i - 1]);
                //竖线
                lineY[i - 1] = new Line();
                lineY[i - 1] = DrawLine(color, 0.15, distanceL + unitX * i, distanceT, distanceL + unitX * i, cvs.ActualHeight - distanceB);
                cvs.Children.Add(lineY[i - 1]);
            }

            cvs.Children.Add(leftBorder);
            cvs.Children.Add(bottomBorder);
            cvs.Children.Add(rightBorder);
            cvs.Children.Add(topBorder);
        }

        /// <summary>
        /// 绘制文字信息
        /// </summary>
        /// <param name="cvs"></param>
        /// <param name="companyName"></param>
        /// <param name="windFieldName"></param>
        /// <param name="fanNumber"></param>
        /// <param name="maxData"></param>
        /// <param name="minData"></param>
        /// <param name="color"></param>
        /// <param name="unit"></param>
        /// <param name="dateTimeCollection"></param>
        public static void DrawWord(Canvas cvs, string companyName, string windFieldName, string fanNumber, double maxData, double minData, string color, string unit, List<DateTime?> dateTimeCollection, List<DateTime?> compareDateTimeCollection)
        {
            //设定要写的数字的单位值
            maxData = dataTole * maxData;//设置最大的数据宽容值
            minData = minData - (dataTole - 1) * minData;//设置最小的数据宽容值
            double numberUnit = (maxData - minData) / 10;
            //创建纵轴文字对象
            TextBlock[] txtVer = new TextBlock[11];
            for (int i = 0; i < 11; i++)
            {
                txtVer[i] = new TextBlock();
                txtVer[i].Text = (minData + numberUnit * i).ToString("F1");//要写的数字
                txtVer[i].FontSize = 12;
                txtVer[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                txtVer[i].Margin = new Thickness(0, cvs.ActualHeight - distanceB - 8 - unitY * i * 2, 0, 0);
                cvs.Children.Add(txtVer[i]);
            }
            //绘制单位
            TextBlock txtUnit = new TextBlock();
            txtUnit.Text = "单位：" + unit;
            txtUnit.FontSize = 12;
            txtUnit.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            txtUnit.Margin = new Thickness(0, 0, 0, 0);
            cvs.Children.Add(txtUnit);

            //绘制时间轴
            int dateTimeCount = dateTimeCollection.Count;//获取时间集合长度
            if (dateTimeCount < 21)
            {
                //如果数据量太少，则只在横轴两头绘制两个时间
                TextBlock txtDate1 = new TextBlock();
                txtDate1.Text = dateTimeCollection[0].Value.Year + "/" + dateTimeCollection[0].Value.Month + "/" + dateTimeCollection[0].Value.Day;
                txtDate1.FontSize = 12;
                txtDate1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                txtDate1.Margin = new Thickness(10, cvs.ActualHeight - distanceB + 7, 0, 0);
                cvs.Children.Add(txtDate1);
                TextBlock txtDate2 = new TextBlock();
                txtDate2.Text = dateTimeCollection[dateTimeCount - 1].Value.Year + "/" + dateTimeCollection[dateTimeCount - 1].Value.Month + "/" + dateTimeCollection[dateTimeCount - 1].Value.Day;
                txtDate2.FontSize = 12;
                txtDate2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                txtDate2.Margin = new Thickness(cvs.ActualWidth - 65, cvs.ActualHeight - distanceB + 7, 0, 0);
                cvs.Children.Add(txtDate2);
            }
            else
            {
                //否则就在横轴上绘制 10 个时间
                TextBlock[] txtDate = new TextBlock[10];
                //获得纵向每条格线的间距，像素
                double pixelUnit = (cvs.ActualWidth - distanceL - distanceR) / 20;
                for (int i = 1; i <= 10; i++)
                {
                    int countTemp = ((2 * i) - 1) * dateTimeCount / 20;
                    txtDate[i - 1] = new TextBlock();
                    txtDate[i - 1].Text = dateTimeCollection[countTemp].Value.Year + "/" + dateTimeCollection[countTemp].Value.Month + "/" + dateTimeCollection[countTemp].Value.Day;
                    txtDate[i - 1].FontSize = 12;
                    txtDate[i - 1].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                    txtDate[i - 1].Margin = new Thickness(distanceL + pixelUnit / 2 + 2 * (i - 1) * pixelUnit, cvs.ActualHeight - distanceB + 7, 0, 0);
                    cvs.Children.Add(txtDate[i - 1]);
                }
            }
            
            //绘制标题
            if (compareDateTimeCollection!=null)
            {
                //绘制对比标题
                TextBlock txtTitle = new TextBlock();
                txtTitle.Text = "检索到" + companyName + windFieldName + "风场第" + fanNumber + "号风机" 
                    + dateTimeCount + "条数据(对比风机"+compareDateTimeCollection.Count+"条数据)";
                txtTitle.FontSize = 21;
                txtTitle.FontFamily = new FontFamily("黑体");
                txtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                txtTitle.Margin = new Thickness(cvs.ActualWidth / 5, 3, 0, 0);
                cvs.Children.Add(txtTitle);
            }
            else
            {
                //绘制原风机标题
                TextBlock txtTitle = new TextBlock();
                txtTitle.Text = "检索到(" + dateTimeCollection[0].Value.Year + "/"
                    + dateTimeCollection[0].Value.Month + "/"
                    + dateTimeCollection[0].Value.Day + "-"
                    + dateTimeCollection[dateTimeCount - 1].Value.Year + "/"
                    + dateTimeCollection[dateTimeCount - 1].Value.Month + "/"
                    + dateTimeCollection[dateTimeCount - 1].Value.Day + ")"
                    + companyName + windFieldName + "风场第" + fanNumber + "号风机" + dateTimeCount + "条数据";
                txtTitle.FontSize = 21;
                txtTitle.FontFamily = new FontFamily("黑体");
                txtTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
                txtTitle.Margin = new Thickness(cvs.ActualWidth / 5, 3, 0, 0);
                cvs.Children.Add(txtTitle);
            }
            
        }

        /// <summary>
        /// 绘制曲线
        /// </summary>
        /// <param name="cvs"></param>
        /// <param name="dataList"></param>
        /// <param name="maxData"></param>
        /// <param name="minData"></param>
        /// <param name="color"></param>
        public static void DrawCurve(Canvas cvs, List<double> dataList, double maxData, double minData, string color)
        {
            maxData = dataTole * maxData;//设置最大的数据宽容值
            minData = minData - (dataTole - 1) * minData;//设置最小的数据宽容值
            
            Path path = new Path();
            PathGeometry pathGeometrg = new PathGeometry();//组合绘制的线段
            PathFigure pathFigure = new PathFigure();//运动轨迹线段绘制  

            Polygon polygon = new Polygon();//创建填充对象
            polygon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            polygon.StrokeThickness = 0;
            polygon.Opacity = 0.1;//设置填充透明度
            PointCollection polygonPoints = new PointCollection();//创建填充点的集合

            path.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));//绘制颜色，紫色
            path.StrokeThickness = 1;//绘制的线宽
            double cvsWidth = cvs.ActualWidth - distanceL - distanceR; //获得当前Canvas允许绘制波形的宽度
            double cvsHight = cvs.ActualHeight - distanceT - distanceB; //获得当前Canvas允许绘制波形的高度
            double unitDatePixel = cvsHight / (maxData - minData);//计算单位数据所需要的像素数目
            double drawStep = cvsWidth / (dataList.Count - 1);//计算横向绘制的最小单位
            //定义填充的第一个点和最后一个点
            Point fillPointStart = new Point(distanceL, cvs.ActualHeight - distanceB);
            Point fillPointEnd = new Point(cvs.ActualWidth - distanceR, cvs.ActualHeight - distanceB);
            //定义绘制的第一个点
            Point startPoint = new Point(distanceL, cvsHight + distanceT - unitDatePixel * (dataList[0] - minData));
            pathFigure.StartPoint = startPoint;//设置曲线的第一个点
            polygonPoints.Add(fillPointStart);//设置填充的第一个点
            polygonPoints.Add(startPoint);//设置填充的第二个点
            for (int i = 1; i < dataList.Count; i++)
            {
                Point pointTemp = new Point(distanceL + drawStep * i, cvsHight + distanceT - unitDatePixel * (dataList[i] - minData));
                //添加绘制曲线的其他所有点
                pathFigure.Segments.Add(new LineSegment(pointTemp, true));
                //为填充区域添加点
                polygonPoints.Add(pointTemp);
            }
            polygonPoints.Add(fillPointEnd);//填充区域添加最后一个点
            polygon.Points = polygonPoints;//给填充区域添加点的集合
            cvs.Children.Add(polygon);

            pathGeometrg.Figures.Add(pathFigure); //组合绘制的线段，只要操作一次
            path.Data = pathGeometrg;//作为Path对象的数据
            cvs.Children.Add(path);

        }

        /// <summary>
        /// 对比曲线绘制
        /// </summary>
        /// <param name="cvs"></param>
        /// <param name="dataList"></param>
        /// <param name="maxData"></param>
        /// <param name="minData"></param>
        /// <param name="color"></param>
        /// <param name="allDate"></param>
        /// <param name="pointDate"></param>
        public static void DrawCompareCurve(Canvas cvs, List<double> dataList, double maxData, double minData, string color, List<DateTime?> allDate, List<DateTime?> pointDate)
        {
            

            maxData = dataTole * maxData;//设置最大的数据宽容值
            minData = minData - (dataTole - 1) * minData;//设置最小的数据宽容值

            Path path = new Path();
            PathGeometry pathGeometrg = new PathGeometry();//组合绘制的线段
            PathFigure pathFigure = new PathFigure();//运动轨迹线段绘制  

            Polygon polygon = new Polygon();//创建填充对象
            polygon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            polygon.StrokeThickness = 0;
            polygon.Opacity = 0.1;//设置填充透明度
            PointCollection polygonPoints = new PointCollection();//创建填充点的集合

            path.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));//绘制颜色，紫色
            path.StrokeThickness = 1;//绘制的线宽
            double cvsWidth = cvs.ActualWidth - distanceL - distanceR; //获得当前Canvas允许绘制波形的宽度
            double cvsHight = cvs.ActualHeight - distanceT - distanceB; //获得当前Canvas允许绘制波形的高度
            //计算曲线第一个点的横坐标偏移，大小为相对于可绘制区域的像素数
            double startXoff = cvsWidth * allDate.IndexOf(pointDate[0]) / (allDate.Count-1);
            double endXoff = cvsWidth * allDate.IndexOf(pointDate[pointDate.Count - 1]) / (allDate.Count-1);
            
            double unitDatePixel = cvsHight / (maxData - minData);//计算单位数据所需要的像素数目
            double drawStep = (endXoff - startXoff) / (dataList.Count - 1);//计算横向绘制的最小单位
            //定义填充的第一个点和最后一个点
            Point fillPointStart = new Point(distanceL + startXoff, cvs.ActualHeight - distanceB);
            Point fillPointEnd = new Point(distanceL+endXoff, cvs.ActualHeight - distanceB);
            //定义绘制的第一个点
            Point startPoint = new Point(distanceL+startXoff, cvsHight + distanceT - unitDatePixel * (dataList[0] - minData));
            pathFigure.StartPoint = startPoint;//设置曲线的第一个点
            polygonPoints.Add(fillPointStart);//设置填充的第一个点
            polygonPoints.Add(startPoint);//设置填充的第二个点
            for (int i = 1; i < dataList.Count; i++)
            {
                Point pointTemp = new Point(distanceL + startXoff + drawStep * i, cvsHight + distanceT - unitDatePixel * (dataList[i] - minData));
                //添加绘制曲线的其他所有点
                pathFigure.Segments.Add(new LineSegment(pointTemp, true));
                //为填充区域添加点
                polygonPoints.Add(pointTemp);
            }
            polygonPoints.Add(fillPointEnd);//填充区域添加最后一个点
            polygon.Points = polygonPoints;//给填充区域添加点的集合
            cvs.Children.Add(polygon);

            pathGeometrg.Figures.Add(pathFigure); //组合绘制的线段，只要操作一次
            path.Data = pathGeometrg;//作为Path对象的数据
            cvs.Children.Add(path);
        }

        //用于记录上一次的绘制元素
        private static Line prevLineX;
        private static Line prevLineY;
        private static TextBlock prevTxt;
        private static TextBlock prevDate;
        public static void MouseMoveHint(Canvas cvs, MouseEventArgs e, double maxData, double minData, string unit, List<DateTime?> dateTimeCollection)
        {
            cvs.Children.Remove(prevLineX);
            cvs.Children.Remove(prevLineY);
            cvs.Children.Remove(prevTxt);
            cvs.Children.Remove(prevDate);

            Point mousePoint = e.GetPosition(cvs);
            double mouseX = mousePoint.X;
            double mouseY = mousePoint.Y;
            //限定鼠标 x，y 的范围
            if (mouseX <= distanceL)
            {
                mouseX = distanceL;
            }
            if (mouseX >= cvs.ActualWidth - distanceR)
            {
                mouseX = cvs.ActualWidth - distanceR;
            }
            if (mouseY <= distanceT)
            {
                mouseY = distanceT;
            }
            if (mouseY >= cvs.ActualHeight - distanceB)
            {
                mouseY = cvs.ActualHeight - distanceB;
            }
            //绘制鼠标所在位置的横线
            Line xline = DrawLine("#FFFF3A00", 0.8, distanceL, mouseY, cvs.ActualWidth - distanceR, mouseY);

            //绘制鼠标所在位置的竖线
            Line yline = DrawLine("#FFFF3A00", 0.8, mouseX, distanceT, mouseX, cvs.ActualHeight - distanceB);


            //设定要写的数字的单位值
            maxData = dataTole * maxData;//设置最大的数据宽容值
            minData = minData - (dataTole - 1) * minData;//设置最小的数据宽容值
            double cvsHight = cvs.ActualHeight - distanceT - distanceB; //获得当前Canvas允许绘制波形的高度
            double unitDate = (maxData - minData) / cvsHight;//计算单位像素代表数据的大小

            //定义文字相对于鼠标点的左右位置
            double marginL=0;
            if (mouseX<=cvs.ActualWidth/2)
            {
                marginL = mouseX+5;
            }
            else
            {
                marginL = mouseX - 72;
            }
            //---------------创建描述文字对象
            //定义数据数值大小
            TextBlock txtVer = new TextBlock();
            txtVer = new TextBlock();
            txtVer.Text = (minData + (cvs.ActualHeight - distanceB - mouseY) * unitDate).ToString("F1") + unit;//要写的数字
            txtVer.FontSize = 12;
            txtVer.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF3A00"));
            txtVer.Margin = new Thickness(marginL, mouseY - 30, 0, 0);

            int local = (int)((mouseX - distanceL) / (cvs.ActualWidth - distanceL - distanceR)*(dateTimeCollection.Count-1));
      
            //定义数据时间
            TextBlock txtDate = new TextBlock();
            txtDate = new TextBlock();            
            txtDate.Text = dateTimeCollection[local].Value.Year + "/" + dateTimeCollection[local].Value.Month + "/" + dateTimeCollection[local].Value.Day;
            txtDate.FontSize = 12;
            txtDate.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF3A00"));
            txtDate.Margin = new Thickness(marginL, mouseY - 15, 0, 0);

            prevLineX = xline;
            prevLineY = yline;
            prevTxt = txtVer;
            prevDate = txtDate;

            cvs.Children.Add(txtVer);
            cvs.Children.Add(xline);
            cvs.Children.Add(yline);
            cvs.Children.Add(txtDate);
        }

        /// <summary>
        /// 移除上次残留的鼠标轨迹
        /// </summary>
        /// <param name="cvs"></param>
        public static void RemoveMouseHint(Canvas cvs)
        {
            cvs.Children.Remove(prevLineX);
            cvs.Children.Remove(prevLineY);
            cvs.Children.Remove(prevTxt);
            cvs.Children.Remove(prevDate);
        }
    }
}
