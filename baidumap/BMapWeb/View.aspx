<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="BMapWeb.View" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <%--<meta http-equiv="X-UA-Compatible" content="IE=11" />--%>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <style type="text/css">
        html, body {
            width: 100%;
            height: 100%;
            margin: 0;
            padding: 0;
            overflow: hidden;
        }

        #map {
            width: 100%;
            height: 100%;
        }
    </style>
</head>
<body>
    <%--    <div>
        <form action="View.aspx" method="get">
            姓名<input type="text" name="name" value="haha" disabled="disabled" style="border:0;background:white;color:black;font-size:36px;"/><br />
            年龄<input type="text" name="age" value="567"/><br />
            <textarea name="name" disabled="disabled">yukkkk</textarea>
            <input type="submit" value="说明"/>            
        </form>
    </div>--%>
    <div id="map"></div>
    <canvas id="canvas"></canvas>

    <script type="text/javascript" src="http://api.map.baidu.com/api?v=2.0&ak=1XjLLEhZhQNUzd93EjU5nOGQ"></script>
    <script type="text/javascript" src="http://apps.bdimg.com/libs/jquery/2.1.1/jquery.min.js"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/library/TextIconOverlay/1.2/src/TextIconOverlay_min.js"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/library/MarkerClusterer/1.2/src/MarkerClusterer_min.js"></script>
    <script type="text/javascript" src="mapv.js"></script>



    <script type="text/javascript">
        //解析JSON字符串
        var allpoint = JSON.parse('<%=json%>');

        // 百度地图API功能
        var map = new BMap.Map("map", {
            enableMapClick: false
        });    // 创建Map实例
        map.centerAndZoom(new BMap.Point(105.403119, 38.028658), 5);  // 初始化地图,设置中心点坐标和地图级别
        map.enableScrollWheelZoom(true); // 开启鼠标滚轮缩放
        var menu = new BMap.ContextMenu();//添加鼠标右键菜单
        var txtMenuItem =
            {
                text: '缩放至全国范围',
                callback: function () { map.clearOverlays(); Fugai('中国'); AddPoints(); map.centerAndZoom(new BMap.Point(105.403119, 38.028658), 5); }
            }
        menu.addItem(new BMap.MenuItem(txtMenuItem.text, txtMenuItem.callback, 100));
        map.addContextMenu(menu);

        Fugai('中国');//添加覆盖物

        //// 添加带有定位的导航控件
        //var navigationControl = new BMap.NavigationControl({
        //    // 靠左上角位置
        //    anchor: BMAP_ANCHOR_TOP_LEFT,
        //    // LARGE类型
        //    type: BMAP_NAVIGATION_CONTROL_LARGE,
        //    // 启用显示定位
        //    enableGeolocation: true
        //});
        //map.addControl(navigationControl);


        var geoc = new BMap.Geocoder();
        //地图点击事件
        map.addEventListener("click", function (e) {
            var pt = e.point;
            geoc.getLocation(pt, function (rs) {
                var addComp = rs.addressComponents;
                if (addComp.province!='') {
                    //将点击位置的省份调整到最佳视野
                    map.centerAndZoom(addComp.province);//调整视野               
                    Fugai(addComp.province);//添加覆盖物
                }                    
            });        
        });


        
        function Fugai(PROVINCE)
        {
            //添加覆盖物
            var bdary = new BMap.Boundary();
            bdary.get(PROVINCE, function (rs) {       //获取行政区域
                var count = rs.boundaries.length; //行政区域的点有多少个

                var pointArray = [];
                var data = [];
                for (var i = 0; i < count; i++) {
                    var ply = new BMap.Polygon(rs.boundaries[i], { strokeWeight: 2, strokeColor: "#ff0000" }); //建立多边形覆盖物
                    var coordinates = [];
                    var path = rs.boundaries[i].split(';');
                    for (var j = 0; j < path.length; j++) {
                        coordinates.push(path[j].split(','));
                    }
                    pointArray = pointArray.concat(ply.getPath());

                    data.push({
                        geometry: {
                            type: 'Polygon',
                            coordinates: [coordinates]
                        }
                    });
                }
                console.log(data);

                //map.setViewport(pointArray);    //调整视野  

                var options = {
                    //fillStyle: 'rgba(60, 60, 60, 1)',
                    //strokeStyle: 'rgba(250, 250, 255, 1)',
                    fillStyle: "#1383CD",
                    strokeStyle: "#1383CD",
                    lineWidth: 1,
                    draw: 'clip'
                }

                var mapvLayer = new mapv.baiduMapLayer(map, new mapv.DataSet(data), options);
            });
        }

        
        


        // 地图自定义样式        
        map.setMapStyle({
            styleJson: [
          {
              "featureType": "water",
              "elementType": "all",
              "stylers": {
                  //"color": "#031b32"
                  "color": "#1383CD"                  
              }
          },
          {
              "featureType": "land",
              "elementType": "all",
              "stylers": {
                  //"color": "#202e5a"                  
                  "color": "#073763"
              }
          },
          {
              "featureType": "road",
              "elementType": "all",
              "stylers": {
                  "visibility": "off"
              }
          },
          {
              "featureType": "poi",
              "elementType": "all",
              "stylers": {
                  "visibility": "off"
              }
          },
          {
              "featureType": "boundary",
              "elementType": "all",
              "stylers": {
                  //"color": "#ffff00"
                  "color": "#00f0ff"
              }
          },
          {
              "featureType": "green",
              "elementType": "all",
              "stylers": {
                  "color": "#0b5394",
                  "visibility": "off"
              }
          },
          {
              "featureType": "manmade",
              "elementType": "all",
              "stylers": {
                  "visibility": "off"
              }
          },
          {
              "featureType": "building",
              "elementType": "all",
              "stylers": {
                  "visibility": "off"
              }
          },
          {
              "featureType": "all",
              "elementType": "labels.text.fill",
              "stylers": {
                  "color": "#ffffff",
                  "visibility": "off"
              }
          },
          {
              "featureType": "all",
              "elementType": "labels.text.stroke",
              "stylers": {
                  "color": "#000000",
                  "visibility": "off"
              }
          }
            ]
        });



        //设置信息窗口
        var opts = {
            width: 300,     // 信息窗口宽度
            height: 350,     // 信息窗口高度
            title: "<strong style='color:#003F68;font-size:21px;font-family:微软雅黑'>基本信息：</strong><br/><br/>", // 信息窗口标题            
            enableAutoPan: true,     //自动平移
            enableMessage: true//设置允许信息窗发送短息            
        };
        var markers = [];//创建多点数组

        

        //地图上添加风机点的方法
        function AddPoints()
        {
            for (var i = 0; i < allpoint.length; i++) {
                var point = new BMap.Point(allpoint[i].GPS_lng, allpoint[i].GPS_lat);//创建点            
                var marker = new BMap.Marker(point);  // 创建标注
                ////marker.setAnimation(BMAP_ANIMATION_BOUNCE);            
                markers.push(marker);
                var content = '<div id="LoginBox" style="margin:0">'
                                + '<div class="row" style="width: 300px;height: 35px;margin-top:-10px;font-family:微软雅黑">业主名称: ' + allpoint[i].CompanyName + '</div>'
                                + '<div class="row" style="width: 300px;height: 35px;font-family:微软雅黑">风场名称: ' + allpoint[i].WindFieldName + '</div>'
                                + '<div class="row" style="width: 300px;height: 35px;font-family:微软雅黑">地址: ' + allpoint[i].Address + '</div><br/>'
                                + '<div class="row" style="width: 300px;height: 35px;font-family:微软雅黑">业主风机数量: ' + allpoint[i].FanCountOfCompany + '</div>'
                                + '<div class="row" style="width: 300px;height: 35px;font-family:微软雅黑">风机编号: ' + allpoint[i].FanNumber + '</div>'
                                + '<div class="row" style="width: 300px;height: 35px;font-family:微软雅黑">风机类型: ' + allpoint[i].FanType + '</div>'
                                + '<div class="row" style="width: 300px;height: 35px;font-family:微软雅黑">信号模式: ' + allpoint[i].SignalKind + '</div>'
                                + '<div class="row">'
                                    + '<input onclick="openDetailWindow(allpoint[' + i + '].CompanyName,allpoint[' + i + '].WindFieldName,allpoint[' + i + '].FanNumber)" type="button" value="详细信息" style="width: 100px;height:30px;margin-left:190px;margin-top:10px;background:#0C6EAE;color:white;border:0;font-family:微软雅黑"/>'
                                + '</div>'
                            + '</div>';
                //map.addOverlay(marker);               // 将标注添加到地图中，若启动多点聚合，请屏蔽此句           
                addClickHandler(content, marker); //添加点的单击事件
                
            }
            
        }
        
        AddPoints();//添加风机点

        var markerClusterer = new BMapLib.MarkerClusterer(map, { markers: markers });//添加多点聚合
        
        function addClickHandler(content, marker) {
            marker.addEventListener("click", function (e) {
                openInfo(content, e)
            }
            );
        }
        //var temptxt = 0;//测试
        function openInfo(content, e) {
            //temptxt = content;//测试
            //temptxt = temptxt + 1;//测试
            var p = e.target;
            var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
            var infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象               
            map.openInfoWindow(infoWindow, point); //开启信息窗口  

        }


        //创建WPF新窗口展示此站点的详细信息
        function openDetailWindow(companyName, windFieldName, fanNumber) {
            window.external.play(companyName, windFieldName, fanNumber);
        }

        function ConnectDB() {



            alert("message");
            var pps = JSON.parse('<%=json%>');

            for (var i = 0; i < pps.length; i++) {

                alert(pps[i].GPS_lng + '  ' + pps[i].GPS_lat);
            }


            //alert("<%=sss[0]%>");
            <%sayHello();%>
        }


        function display(str) {

            alert(str);
        }







        /**************************测试：随机产生多个点，并伴随点动画*****************************/
        //var MAX = 30;

        //    content66 = '<div id="LoginBox">'
        //                      + '<div class="row" style="width: 300px;height: 35px;margin-top:-10px;">名字: </div>'
        //                      + '<div class="row" style="width: 300px;height: 35px;">覆盖范围: </div>'
        //                      + '<div class="row" style="width: 300px;height: 35px;">安装地址: </div>'
        //                      + '<div class="row" style="width: 300px;height: 35px;">一线维护人:</div>'                             
        //                      + '<div class="row">'
        //                      + '<input onclick="quxiao()" type="button" id="btn1" value="取消" style="width: 100px;height:30px;margin-top: 20px;margin-left: 120px;">'
        //                      + '</div></div>';

        ////设置信息窗口
        //var opts = {
        //    width: 250,     // 信息窗口宽度
        //    height: 300,     // 信息窗口高度
        //    title: "<strong>设备信息：</strong><br><br>", // 信息窗口标题
        //    enableMessage: true//设置允许信息窗发送短息
        //};

        //var markers = [];
        //for (var i = 0; i < MAX; i++) {
        //    var point = new BMap.Point(Math.random() * 40 + 85, Math.random() * 30 + 21);//创建点
        //    var marker = new BMap.Marker(point);  // 创建标注
        //    marker.setAnimation(BMAP_ANIMATION_BOUNCE);
        //    markers.push(marker);
        //    //var content = data_info[i][2];
        //    map.addOverlay(marker);               // 将标注添加到地图中            
        //    addClickHandler(content66, marker); //添加点的单击事件
        //}
        //function addClickHandler(content, marker) {
        //    marker.addEventListener("click", function (e) {
        //        openInfo(content, e)
        //    }
        //    );
        //}
        //function openInfo(content, e) {
        //    var p = e.target;
        //    var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
        //    var infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象               
        //    map.openInfoWindow(infoWindow, point); //开启信息窗口
        //}
        ////var markerClusterer = new BMapLib.MarkerClusterer(map, { markers: markers });//添加多点聚合

        ////创建WPF新窗口展示此站点的详细信息
        //function openDetailWindow() {
        //    alert("打开成功");
        //}








        /**************************测试：信息窗口添加按钮*****************************/
        //function CreateOpts(content) {
        //    var opts = {
        //        width: 350,
        //        height: 300,
        //        title: "<strong>设备信息：</strong><br><br>"
        //    }
        //    content = '<div id="LoginBox">'
        //                      + '<div class="row" style="width: 300px;height: 35px;margin-top:-10px;">名字: </div>'
        //                      + '<div class="row" style="width: 300px;height: 35px;">覆盖范围: </div>'
        //                      + '<div class="row" style="width: 300px;height: 35px;">安装地址: </div>'
        //                      + '<div class="row" style="width: 300px;height: 35px;">一线维护人:</div>'                             
        //                      + '<div class="row">'
        //                      + '<input onclick="quxiao()" type="button" id="btn1" value="取消" style="width: 100px;height:30px;margin-top: 20px;margin-left: 120px;">'
        //                      + '</div></div>';
        //    var infoWindow = new BMap.InfoWindow(content, opts); // 创建信息窗口对象    第一个参数为内容 
        //    var poi = new BMap.Point(105.403119, 38.028658);
        //    var marker = new BMap.Marker(poi);
        //    map.addOverlay(marker);
        //    marker.addEventListener("click", function () {
        //        map.openInfoWindow(infoWindow, poi); // 打开信息窗口
        //    });

        //}

        function quxiao() {
            alert("取消成功");
        }


        //CreateOpts();


    </script>




</body>
</html>
