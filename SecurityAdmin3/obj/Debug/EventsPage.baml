   M S B A M L   `   `   `  ����  SecurityAdmin3    SecurityAdmin3.EventsPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$  
EventsPage��5       N� 
Background �	#FF252526.��5
      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   ���	#FF1E1E1E �� Padding$ 10q�65   ._�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $��Мероприятия��6   
�������$��18(�6B   $
��Bold"�6P   5      5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ 
EventsGrid��5      $��1��6'   $��10q�64      AutoGenerateColumns .  ���	#FF2D2D30
�������   RowBackground �	#FF2D2D30   AlternatingRowBackground �	#FF252526	   HeadersVisibility$	 Column=�5      
   
IsReadOnly
 .    ColumnHeaderStyle 5       �� 5!       �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5"      ��  �� Property � �� �� Value �	#FF1E1E1E63   �� 5#       � ��
 �����63   �� 5$       � �� �	#FF3E3E4064   �� 5%       � ��$ 0,0,0,1q�68   �� 5&       � ��$ Center=�6C   �� 5'       � ��$
 Bold"�63   5(         Columns 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Id+6H     5-      $ Название��6%   $ * 6P    67   �� *Name+6P     5.      $ Коэффициент��6%   $	 100 6Z    6:   �� *Coefficient+6Z     5/      $ Цена за час��6%   $	 120 6[    6:   �� *PricePerHour+6[   51      �� 54   
   $��2��6   $ 
Horizontal=�6"   $��Right=�6;   $��10q�6W   .+�55      �� -   5         ��Name$ btnAdd��55      $��Добавить��6%   $��0,0,10,0q�6M   $��10q�6_   $	��100��6l   �� 56      -   5        $ btnBack��56      $��
Назад��6&   $��10q�6L   $��56��6Y   57      58      59      