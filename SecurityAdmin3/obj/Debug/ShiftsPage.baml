   M S B A M L   `   `   `  ����  SecurityAdmin3    SecurityAdmin3.ShiftsPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$  
ShiftsPage��5	       N� 
Background �	#FF252526.��5
      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   ���	#FF1E1E1E �� Padding$ 10q�65   ._�5      �� $#��График дежурств��6   
�������$��18(�6B   $
��Bold"�6P   5      &   System.Windows.Controls.Calendar  5   
   -   5          Name$ calendar��5      $��1��6%   $
��0,10q�62   ���	#FF2D2D30
����������	#FF007ACC$��1q�5   +   5      �� 5   
   -   5         ��Name$ shiftsListView��5      $��2��6+   $
��0,10q�68   ���	#FF2D2D30
������� �� View 5       �� 5!      .P�5"      �� $��Сотрудник��6%    �� Width$	 200��6f    �� DisplayMemberBinding 68   �� *EmployeeName+6f   �� 5#      $��Клиент��6%   $	 200��6a    65   �� *
ClientName+6a   �� 5$      $��
Время��6%   $	 120��6b    64   �� *TimeInterval+6b   �� 5%      $��Договор��6%   $	 120��6f    66   �� *ContractNumber+6f   5&      5(      �� 5+   
   $��3��6   	 �� Orientation$	 
Horizontal=�6"   $��Right=�6;   $��0,10,0,0q�6W   .+�5,      �� -   5        
 ��Name$
 btnAddShift��5,      $��Добавить��6*   $��0,0,10,0q�6W   $	��100��6i   �� 5-      -   5        $
 btnEditShift��5-      $��Изменить��6+   $��0,0,10,0q�6Y   $	��100��6k   �� 5.      -   5        $
 btnDeleteShift��5.      $��Удалить��6-   $��0,0,10,0q�6\   $	��100��6n   �� 5/      -   5        $
 btnBack��5/      $��
Назад��6&   $	��100��6L   50      51      52      