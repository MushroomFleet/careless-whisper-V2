   M S B A M L   `   `   `  ����  CarelessWhisperV2,    &CarelessWhisperV2.Views.SettingsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$$  Careless Whisper V2 - Settings��5   	   $	��600��5   	   $	��500��5   	    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	   .��5
      � $��20q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   .@�5      �� $��General��6   .��5      �� $��15q�6   .+�5      �� $��General Settings��6    $��16(�68   $
��Bold"�6F   $��0,0,0,15q�6X   �� 5      -   5         ��Name$ AutoStartCheckBox��5      $��Start with Windows��5      $��0,0,0,10q�5      �� 5      $��Theme:��6    $��SemiBold"�6.   $��0,10,0,5q�6D   �� 5      -   5         ��Name$ ThemeComboBox��5      $	��200��66   $
��Left=�6B   .@�5      �� $��Light��6'   �� 5      $
��Dark��6'    �� 
IsSelected$
 True��66   �� 5      $��System��6'   5       �� 5"      $��Logging:��6    $��SemiBold"�60   $��0,20,0,5q�6F   �� 5#      -   5        $ EnableLoggingCheckBox��5#      $"��Enable transcription logging��5$       d� 	IsChecked$
 TrueY�5%      $��0,0,0,5q�5&      �� 5(      -   5        $ SaveAudioFilesCheckBox��5(      $*��$Save audio files with transcriptions��5)      $��0,0,0,5q�5*      �� 5,       �� Orientation$ 
Horizontal=�6!   $��0,10,0,0q�6:   .+�5-      �� $��Retention period:��6$   $��Center=�6=   $��0,0,10,0q�6X   �� 5.      -   5        	 ��Name$	 RetentionDaysTextBox��5.   "   $��30��5/   "   $��50��50   "   $��Center=�51   "   �� 52      $
��days��6$   $��Center=�60   $��5,0,0,0q�6K   53      54      55      �� 58      $��Hotkeys��6   .��59      �� $��15q�6   .+�5:      �� $��Hotkey Configuration��6    $��16(�6<   $
��Bold"�6J   $��0,0,0,15q�6\   �� 5<      $��Push-to-Talk Key:��6    $��SemiBold"�69   $��0,0,0,5q�6O   � 5=      
 � ColumnDefinitions
 5>      �� 5?      $��* �6/   �� 5@      $
��Auto �6/   .+�5C      �� -   5        $	 HotkeyTextBox��5C   "   $��0��5D   "   $��F1��5E   "    �� 
IsReadOnly$
 True��5F   "   $��0,0,10,0q�5H   "   �� 5J      -   5         ��Name$ ClearHotkeyButton��5J   !   $��1��5K   !   $��Clear��5L   !   $��60��5N   !   5O      �� 5Q      $X��RInstructions: Click in the text box and press the key combination you want to use.��6    $��Italic$�5R       
�������$
��Wrap=�5T       $��0,5,0,0q�5U       �� 5W      -   5        $ RequireModifiersCheckBox��5W      $.��(Require modifier keys (Ctrl, Alt, Shift)��5X      $��0,15,0,0q�5Y      �� 5[      $��Supported Keys:��6    $��SemiBold"�67   $��0,20,0,5q�6M   �� 5\      $��• Function keys: F1-F12��6    $��10,0,0,2q�6?   �� 5]      $��• Letters: A-Z��6    $��10,0,0,2q�66   �� 5^      $��• Numbers: 0-9��6    $��10,0,0,2q�66   �� 5_      $8��2• Special keys: Space, Tab, Insert, Delete, etc.��6    $��10,0,0,2q�6X   �� 5`      $4��.• Modifier combinations: Ctrl+, Alt+, Shift+��6    $��10,0,0,2q�6T   5a      5b      �� 5e      $��Audio��6   .��5f      �� $��15q�6   .+�5g      �� $��Audio Configuration��6    $��16(�6;   $
��Bold"�6I   $��0,0,0,15q�6[   �� 5i      $��Microphone:��6    $��SemiBold"�63   $��0,0,0,5q�6I   �� 5j      -	   5        $ MicrophoneComboBox��5j      $��0,0,0,10q�5l      �� 5n      -
   5        $ RefreshDevicesButton��5n      $��Refresh Devices��5o      $	��120��5q      $
��Left=�5r      $��0,0,0,20q�5s      �� 5u      $��Audio Quality:��6    $��SemiBold"�66   $��0,0,0,5q�6L   �� 5v      $ 
Horizontal=�6!   $��0,0,0,10q�6:   .+�5w      �� $��Sample Rate:��6$   $��Center=�68   $��0,0,10,0q�6S   �� 5x      -   5        $ SampleRateComboBox��5x   #   $	��100��6?   .@�5y      �� $��8000 Hz��6+   �� 5z      $��16000 Hz��6+   $
 True��6>   �� 5{      $��22050 Hz��6+   �� 5|      $��44100 Hz��6+   �� 5}      $��48000 Hz��6+   5~      5      �� 5�      $ 
Horizontal=�6!   $��0,0,0,20q�6:   .+�5�      �� $��Buffer Size:��6$   $��Center=�68   $��0,0,10,0q�6S   �� 5�      -   5        $ BufferSizeComboBox��5�   #   $	��100��6?   .@�5�      �� $	��512��6+   �� 5�      $
��1024��6+   $
 True��6:   �� 5�      $
��2048��6+   �� 5�      $
��4096��6+   5�      5�      �� 5�      -   5        $ TestMicrophoneButton��5�      $��Test Microphone��5�      $	��120��5�      $
��Left=�5�      �� 5�      -   5         ��Name$ TestResultTextBlock��5�       $�� ��5�       $��0,10,0,0q�5�       
��� � �5�       5�      5�      �� 5�      $��Whisper��6   .��5�      �� $��15q�6   .+�5�      �� $��Speech Recognition��6    $��16(�6:   $
��Bold"�6H   $��0,0,0,15q�6Z   �� 5�      $��Model Size:��6    $��SemiBold"�63   $��0,0,0,5q�6I   �� 5�      -   5        $ ModelSizeComboBox��5�      $��0,0,0,10q�6:   .@�5�      �� $
��Tiny��6'   
 � Tag$
 Tiny��66   �� 5�      $��Base (Recommended)��6'   $
 Base��6D   $
 True��6O   �� 5�      $��Small��6'   $ Small��67   �� 5�      $��Medium��6'   $ Medium��68   5�      �� 5�      $��Model Information:��6    $��SemiBold"�6:   $��0,10,0,5q�6P   �� 5�      
�������$��1q�65    �� Padding$ 10q�6I   $��0,0,0,15q�6V   ._�5�      �� .+�5�      �� -   5        $ ModelInfoTextBlock��5�   (   $4��.Base Model - Balanced performance and accuracy��6D   $��Medium"�6z   �� 5�      -   5        $ ModelSizeTextBlock��5�   (   $%��Size: ~74M parameters, ~1GB RAM��6D   $��0,5,0,0q�6k   �� 5�      -   5        $ ModelPerformanceTextBlock��5�   (   $6��0Performance: Good accuracy with reasonable speed��6K   $��0,2,0,0q�6�   5�      5�      �� 5�      $��	Language:��6    $��SemiBold"�61   $��0,0,0,5q�6G   �� 5�      -   5        $ LanguageComboBox��5�      $��0,0,0,15q�69   .@�5�      �� $��Auto-detect��6'   $
 auto��6=   $
 True��6H   �� 5�      $��English��6'   $ en��69   �� 5�      $��Spanish��6'   $ es��69   �� 5�      $��French��6'   $ fr��68   �� 5�      $��German��6'   $ de��68   �� 5�      $��Italian��6'   $ it��69   �� 5�      $��
Portuguese��6'   $ pt��6<   �� 5�      $��Dutch��6'   $ nl��67   5�      �� 5�      -   5        $ EnableGpuCheckBox��5�      $,��&Enable GPU acceleration (if available)��5�      $
 TrueY�5�      $��0,0,0,10q�5�      �� 5�      $l��fNote: GPU acceleration may significantly improve transcription speed but requires compatible hardware.��6    $��Italic$�5�       
�������$
��Wrap=�5�       5�      5�      5�      �� 5�   
   $��1��6   $ 
Horizontal=�5�      $��Right=�5�      $��0,15,0,0q�5�      .+�5�      �� -   5        $ 
SaveButton��5�      $
��Save��5�      $��80��5�      $��30��5�      $��0,0,10,0q�5�       �� 	IsDefault$
 True��5�      �� 5�      -   5        $ CancelButton��5�      $��Cancel��5�      $��80��5�      $��30��5�       �� IsCancel$
 True��5�      5�      5�      5�      