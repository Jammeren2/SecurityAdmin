   M S B A M L   `   `   `  ����I  ESecurityAdmin3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:SecurityAdmin3SecurityAdmin3  5   	       SecurityAdmin3.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:SecurityAdmin3     5   	     � Title$  Security Admin��5   	   $	��600��5	   	   $	��900��6    � WindowStyle$
 None=�5
   	    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5       �� 
TargetType   WindowButton �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value��%  &	        �� 5   
   " ��6   .�5      ��  � ��
 ���� 6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       �@�Cursor$
 Hand��6'   �� 5       � ��$ 14(�6)   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   ._�5      �� $��Center=�6/   $��Center=�6L   5      5      5      	 5      P� 5       
 �@J�IsMouseOver . . �5!      ��  � �� �	#3FFFFFFF63   5"      5$      5%      .��5'      �� ���	#FF252526 �� CornerRadius$ 8��6$   $��1q�65   ���	#FF3E3E40._�5(   
   �  � RowDefinitions 5)      �� 5*      $��32 �6    �� 5+      $��* �6    .+�5/      �� $��0��6   ���	#FF1E1E1E$ 8,8,0,0��69   ._�50      �  � ColumnDefinitions 51      �� 52      $��* �6+   �� 53      $
��Auto �6+   .+�56      �� $��Security Admin��6    
�������$��Center=�58       $��10,0,0,0q�59       $��14(�5:       �� 5<      $��1��6!    �� Orientation$ 
Horizontal=�61   .+�5=      �� -   5        #��[  5=   !   $	��─��6G   $��45��5>   >   �� 5?      -   5        #��[  5?   !   $	��□��6G   $��45��5@   >   �� 5A      -   5        #��[  5A   !   $	��✕��6G   $��45��5B   ;   5C      5D      5E       � 5H      -   5          �Name$ 	mainFrame��5H      $��1��6'     � NavigationUIVisibility$ Hidden=�64   5I      5J      5K      