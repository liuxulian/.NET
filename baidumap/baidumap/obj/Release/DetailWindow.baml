   M S B A M L   `   `   `  ����   ZhengBangOcean    baidumap.DetailWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  详细信息��$	��700��$
��1300�� � WindowStartupLocation$ CenterScreen=� � Icon$ ICO/wind.ico��.���  � ColumnDefinitions �� �� $	��280 �.+��� -   $��txtbkDigital��$��1�� �� 
IsReadOnly .  �� IsReadOnlyCaretVisible .   �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=�
�������$��18(�$��10q�.����
� .���  � Color$ 	#FF0C6EAE��� $ 	#FF003F68��	 � Offset$	 2Q�.��	liuxulian� $��0��$��2q�
 � RowDefinitions
 �� $��50 ��� $��35 ��� .+���  �� Orientation$ 
Horizontal=�$��0��.+�(  "System.Windows.Controls.DatePicker  -   $��dpStart��$	��100��$��25��   IsDropDownOpen .    -   $��dpEnd��$	��100��$��25�� .  �� -   $��btnLoadData��$��0q�
������ 
���   �$	��100��$��30��$��5q�$
��Left=� J� RenderTransformOrigin$ 0.857,0.467&����� $ 
Horizontal=�.+��� $��ICO/search.ico��$��2q��� $��检索数据��$��Center=�$��14(��� -   $��btnExistExcel��$��0q�
������ 
���   �$	��110��$��30��$��5q�$
��Left=����� $ 
Horizontal=�.+��� $��ICO/EXCEL.ico��$��2q��� $��导出Excel��$��Center=�$��14(�� -   $	��pgb��$��20��$	��315��$��20 0 0 0q� � Value �� *	PgPercent+�� -   $��pgbCount��$��Center=�$��5q�$��15(�
��� ������� *PgPercentText+�� $��1��$ 
Horizontal=�.+��� -	   $��cmbProvince��$	��110��$��5q�$��15(�$��Center=�$��0q� �� MaxDropDownHeight$	 200���� -
   $��cmbCity��$	��110��$��5q�$��15(�$��Center=�$��0q�$	 200���� -   $��cmbCompanyName��$	��160��$��5q�$��15(�$��Center=�$��0q�$	 200���� -   $��cmbWindFieldName��$	��160��$��5q�$��15(�$��Center=�$��0q�$	 200���� -   $��cmbWindFanNumber��$��75��$��5q�$��15(�$��Center=�$��0q�$	 200���� -   $��btnAddCompare��$��0q�$��2q�
������ 
��� � �$	��100������ $ 
Horizontal=�.+��� $��ICO/compare.ico��$��2q��� $��加入对比��$��Center=�$��15(��� -   $��tabCtrl��$��2��$��15(�$��0q�.@��� -   $��tabWindSpeed��$��风速走势图��.���� -   $��cvsWindSpeed��$��1��
��������� -   $��tabWindDirection��$��风向走势图��.���� -   $��cvsWindDirection��$��1��
�������