   M S B A M L   `   `   `  ����  MultiClients    MultiClients.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  CreateMultiClients��5   	   $	��500��6$   $	��600��61   .��5      � $��5q�6    � RowDefinitions 5   
   �� 5      $��40 �6   6*   �� 5      6   �� 5	      $��30 �6   6*   �� 5
      6   �� 5      $��30 �6   6*   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   .+�5      �� $��Center=�6   .��66   
Server IP:6B   �� 5      -   5        $��txtServerAddress��5      $	��150��6.   $��25��6:   $��Center=�6F   $��Center=�6h   .��6�   	127.0.0.15      �� 5      $��Center=�6   .��66   Port:6=   �� 5      -   5        $��txtServerPort��5      $��50��6+   $��25��66   $��Center=�6B   $��Center=�6d   .��6�   500005      �� 5      -   5        $��
btnConnect��5      $��15 2 2 2q�6'   $��90��69   $��25��6D   .��6i   	Connect6r   �� 5      -   5        $��	txtbCount��5      $
��10 0q�6)   $��Center=�67   06U   5      �� 5   
   -   5        $��txtRx��5      $��1��6    �� TextWrapping$
 Wrap=�6,   " �� VerticalScrollBarVisibility$
 Auto=�6@    �� 
IsReadOnly . 6w   �� 5   
   -   5        $��btnClear��5      $��2��6   $��70��6.   $��25��69   $��Right=�6E   $	��5 0q�6a   .��6�   	ClearText5      �� 5   
   -   5        $��txtTx��5      $��3��6   $
 Wrap=�6,   $
 Auto=�6@   6e   �� 5   
   -   5        $��btnSend��5      $��4��6   $��70��6-   $��25��68   $��Right=�6D   $	��5 0q�6`   .��6�   
SendData5      5      5      