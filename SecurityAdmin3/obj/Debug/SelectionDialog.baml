   M S B A M L   `   `   `  ����  SecurityAdmin3$    SecurityAdmin3.SelectionDialog   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  SelectionDialog��5   	   $	��400��6!   $	��300��6.    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�6-   ���	#FF252526.��5      � $��10q�6    � RowDefinitions 5   
   �� 5	      $��* �6   �� 5
      $
��Auto �6   .+�5   
   �� -   5         ��Name$ ItemsListBox��5      $��0��6(   $��0,0,0,10q�65   ���	#FF2D2D30
���������5      �� 5      .R�5      �� $��5q�6!   .+�5      �� $
��Bold"�6;   ��6$   �� *Brand+y�6M   �� *Brand+   NullToVisibilityConverter �� 	Converter# [  �� 5      
���������6$   �� *	RegNumber+y�6V   �� *	RegNumber+# [  �� 5      $
��Bold"�6:   ��6$   �� *Name+y�6L   �� *Name+# [  �� 5      
���������6$   �� *Number+y�6S   �� *Number+# [  5      5      5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   .+�5       �� -   5         ��Name$ OKButton��5       $��OK��6'   $��75��6K   $��0,0,10,0q�6V   �� 5!      -   5        $ CancelButton��5!      $��Отмена��6+   $��75��6W   5"      5#      5$      