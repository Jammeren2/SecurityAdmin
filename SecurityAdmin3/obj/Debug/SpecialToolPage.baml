   M S B A M L   `   `   `  ����  SecurityAdmin3$    SecurityAdmin3.SpecialToolPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$  SpecialToolPage��5	       N� 
Background �	#FF252526.��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   ���	#FF1E1E1E �� Padding$ 10q�65   ._�5      �� -   5         ��Name$ 	pageTitle��5      $)��#Новое спецсредство��6+   
�������$��18(�6X   $
��Bold"�6f   5      �� 5   
   $
��Auto=�6   $��	0,43,0,12q�6:   $��2��6M   .��5      �� .+�5      �� $��Название:��6   
�������$��0,5,0,0q�6@   �� 5      -   5         ��Name$ txtName��5      $��29��6+   �� 5      $��Номер:��6   
�������$��0,10,0,0q�6=   �� 5      -   5        $ 	txtNumber��5      $��29��6-   �� 5       $��Описание:��6   
�������$��0,10,0,0q�6@   �� 5!      -   5        $ txtDescription��5!      $	��100��62    �� AcceptsReturn .  �� TextWrapping$
 Wrap=�6T   5"      5#      �� 5&   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,10,0,0q�6W   .+�5'      �� -   5        	 ��Name$	 	btnDelete��5'      $��Удалить��6(   $��0,0,10,0q�6R   $y�	Collapsed=�6d   �� 5(      -   5        $	 btnSave��5(      $��Сохранить��6&   $��0,0,10,0q�6P   $��87��6b   �� 5)      -   5        $	 btnBack��5)      $��
Назад��6&   $��53��6L   5*      5+      5,      