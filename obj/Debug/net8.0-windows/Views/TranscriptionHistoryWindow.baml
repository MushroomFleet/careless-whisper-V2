   M S B A M L   `   `   `  ����  CarelessWhisperV28    2CarelessWhisperV2.Views.TranscriptionHistoryWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$1  +Transcription History - Careless Whisper V2��5   	   $	��700��5   	   $	��900��5   	    � WindowStartupLocation$ CenterOwner=�5   	   $	��500��5   	   $	��600��5	   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $	��200 �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   
������� �� Padding$ 10q�65   ._�5      �  � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'   �� 5      $
��Auto �6'   �� 5      $
��Auto �6'   �� 5      $
��Auto �6'   .+�5      �� $��0��6   $��Search:��5       $��Center=�5!      $��0,0,10,0q�5"      �� 5$      -   5         ��Name$ SearchTextBox��5$      $��1��5%      $��0,0,10,0q�5&       � ToolTip$' !Search through transcription text��5(      �� 5*      -   5         ��Name$ RefreshButton��5*      $��2��5+      $��Refresh��5,      $��0,0,10,0q�5.      $
��10,5q�5/      �� 51      -   5        $ ExportButton��51      $��3��52      $��	Export...��53      $��0,0,10,0q�55      $
��10,5q�56      �� 58      -   5        $ CleanupButton��58      $��4��59      $��Cleanup Old...��5:      $
��10,5q�5<      5=      5>      �� 5A   
   -   5         ��Name$ TranscriptionListView��5A      $��1��5B      	 �� View	 5E      �� 5F      .P�5G      �� $��	Date/Time��6%   
 �� Width$	
 150��5H   %    �� DisplayMemberBinding 5I   %   �� *TimestampFormatted+�� 5J      $��Duration��6%   $
 80��5K   %    5L   %   �� *DurationFormatted+�� 5M      $��Model��6%   $
 80��5N   %    5O   %   �� *	ModelUsed+�� 5P      $��Preview��6%   $	
 400��5Q   %    5R   %   �� *TextPreview+�� 5S      $��
Characters��6%   $
 80��5T   %    5U   %   �� *CharacterCount+5V      5X      �� 5[   
   $��2��6   $��5��5\      $��Stretch=�5]      
�������5^      � 5a   
   $��3��6   $��10q�6    5b      �� 5c      $��2* �6#   �� 5d      $
��Auto �6#   �� 5e      $��* �6#   .+�5i      �� $��0��6   $��Full Transcription Text��6'   $��0,0,5,0q�6H   .��5j      �� .��5k      �� -   5         ��Name$ FullTextTextBlock��5k       $
��Wrap=�5l        �� Padding$ 5q�5m       $��12(�5n       
�������5o       5p      5q      �� 5t      $��1��6   $��5��5u      $��Stretch=�5v      
�������5w      �� 5z      $��2��6   $��Details��6'   $��5,0,0,0q�68   .��5{      �� $��5q�6   .+�5|      �� -   5        $ TimestampDetail��5|       $
��Bold"�69   $��0,0,0,5q�6K   �� 5}      -   5        $ DurationDetail��5}       $��0,0,0,5q�68   �� 5~      -	   5        $ ModelDetail��5~       $��0,0,0,5q�65   �� 5      -
   5        $ ProcessingTimeDetail��5       $��0,0,0,5q�6>   �� 5�      -   5        $ LanguageDetail��5�       $��0,0,0,5q�68   �� 5�      -   5        $ CharacterCountDetail��5�       $��0,0,0,5q�6>   �� 5�      -   5        $ SegmentCountDetail��5�       $��0,0,0,15q�6<   �� 5�      -   5        $ CopyTextButton��5�      $��	Copy Text��5�      $��0,0,0,5q�5�      ${�False��5�      �� 5�      -   5        $ PlayAudioButton��5�      $��
Play Audio��5�      $��0,0,0,5q�5�      ${�False��5�      $y�	Collapsed=�5�      �� 5�      -   5        $ DeleteEntryButton��5�      $��Delete Entry��5�      $��0,5,0,0q�5�      
�������${�False��5�      5�      5�      5�      �� 5�   
   $��4��6   .@�5�      �� .��5�      �� -   5        $ StatusTextBlock��5�      $��Ready��65   5�      �� 5�      $��Right=�6   .��5�      �� -   5        $ CountTextBlock��5�      $��	0 entries��64   5�      5�      5�      5�      