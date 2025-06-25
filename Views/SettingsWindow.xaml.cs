using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using CarelessWhisperV2.Models;
using CarelessWhisperV2.Services.Settings;
using CarelessWhisperV2.Services.Audio;
using CarelessWhisperV2.Services.Orchestration;
using SharpHook.Native;

namespace CarelessWhisperV2.Views;

public partial class SettingsWindow : Window
{
    private readonly ISettingsService _settingsService;
    private readonly IAudioService _audioService;
    private readonly TranscriptionOrchestrator _orchestrator;
    private readonly ILogger<SettingsWindow> _logger;
    private ApplicationSettings _settings;
    private string _capturedHotkey = "";

    public SettingsWindow(
        ISettingsService settingsService, 
        IAudioService audioService,
        TranscriptionOrchestrator orchestrator,
        ILogger<SettingsWindow> logger)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _audioService = audioService;
        _orchestrator = orchestrator;
        _logger = logger;
        _settings = new ApplicationSettings();

        Loaded += SettingsWindow_Loaded;
    }

    private async void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await LoadCurrentSettings();
            LoadAudioDevices();
            UpdateModelInfo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings window");
            MessageBox.Show($"Failed to load settings: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadCurrentSettings()
    {
        _settings = await _settingsService.LoadSettingsAsync<ApplicationSettings>();
        
        // General tab
        AutoStartCheckBox.IsChecked = _settings.AutoStartWithWindows;
        ThemeComboBox.SelectedItem = ThemeComboBox.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(item => item.Content.ToString() == _settings.Theme);
        EnableLoggingCheckBox.IsChecked = _settings.Logging.EnableTranscriptionLogging;
        SaveAudioFilesCheckBox.IsChecked = _settings.Logging.SaveAudioFiles;
        RetentionDaysTextBox.Text = _settings.Logging.LogRetentionDays.ToString();
        
        // Hotkeys tab
        HotkeyTextBox.Text = _settings.Hotkeys.PushToTalkKey;
        RequireModifiersCheckBox.IsChecked = _settings.Hotkeys.RequireModifiers;
        
        // Audio tab
        SampleRateComboBox.SelectedItem = SampleRateComboBox.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(item => item.Content.ToString()?.StartsWith(_settings.Audio.SampleRate.ToString()) == true);
        BufferSizeComboBox.SelectedItem = BufferSizeComboBox.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(item => item.Content.ToString() == _settings.Audio.BufferSize.ToString());
        
        // Whisper tab
        ModelSizeComboBox.SelectedItem = ModelSizeComboBox.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(item => item.Tag?.ToString() == _settings.Whisper.ModelSize);
        LanguageComboBox.SelectedItem = LanguageComboBox.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(item => item.Tag?.ToString() == _settings.Whisper.Language);
        EnableGpuCheckBox.IsChecked = _settings.Whisper.EnableGpuAcceleration;
    }

    private void LoadAudioDevices()
    {
        try
        {
            var devices = _audioService.GetAvailableMicrophones();
            MicrophoneComboBox.Items.Clear();
            
            foreach (var device in devices)
            {
                var item = new ComboBoxItem
                {
                    Content = device.IsDefault ? $"{device.Name} (Default)" : device.Name,
                    Tag = device.Id
                };
                
                MicrophoneComboBox.Items.Add(item);
                
                if (device.Id == _settings.Audio.PreferredDeviceId || 
                    (string.IsNullOrEmpty(_settings.Audio.PreferredDeviceId) && device.IsDefault))
                {
                    MicrophoneComboBox.SelectedItem = item;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load audio devices");
            MessageBox.Show("Failed to load audio devices. Please try refreshing.", "Audio Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;
        
        var modifiers = new List<string>();
        var key = "";
        
        // Capture modifier keys
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            modifiers.Add("Ctrl");
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            modifiers.Add("Alt");
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            modifiers.Add("Shift");
        if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
            modifiers.Add("Win");
            
        // Capture main key (ignore modifier keys themselves)
        if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl &&
            e.Key != Key.LeftAlt && e.Key != Key.RightAlt &&
            e.Key != Key.LeftShift && e.Key != Key.RightShift &&
            e.Key != Key.LWin && e.Key != Key.RWin)
        {
            key = e.Key.ToString();
        }
        
        if (!string.IsNullOrEmpty(key))
        {
            if (modifiers.Count > 0)
            {
                _capturedHotkey = string.Join("+", modifiers) + "+" + key;
            }
            else
            {
                _capturedHotkey = key;
            }
            
            HotkeyTextBox.Text = _capturedHotkey;
        }
    }

    private void ClearHotkey_Click(object sender, RoutedEventArgs e)
    {
        HotkeyTextBox.Text = "";
        _capturedHotkey = "";
    }

    private void MicrophoneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TestResultTextBlock.Text = "";
    }

    private void RefreshDevices_Click(object sender, RoutedEventArgs e)
    {
        LoadAudioDevices();
        TestResultTextBlock.Text = "Devices refreshed";
        TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Green;
    }

    private async void TestMicrophone_Click(object sender, RoutedEventArgs e)
    {
        if (MicrophoneComboBox.SelectedItem is not ComboBoxItem selectedItem)
        {
            TestResultTextBlock.Text = "Please select a microphone first";
            TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            return;
        }

        try
        {
            TestResultTextBlock.Text = "Testing microphone and transcription... Speak now!";
            TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Blue;
            
            TestMicrophoneButton.IsEnabled = false;
            
            // Enhanced test - record for 5 seconds and test transcription
            var tempFile = Path.Combine(Path.GetTempPath(), $"mic_test_{DateTime.Now:yyyyMMdd_HHmmss_fff}.wav");
            
            // Step 1: Test audio recording
            TestResultTextBlock.Text = "Step 1: Testing audio recording...";
            await Task.Run(async () =>
            {
                await _audioService.StartRecordingAsync(tempFile);
                await Task.Delay(5000); // Record for 5 seconds
                await _audioService.StopRecordingAsync();
            });
            
            // Wait for file to be fully released
            TestResultTextBlock.Text = "Waiting for recording to finalize...";
            await Task.Delay(1000);
            
            if (!File.Exists(tempFile))
            {
                TestResultTextBlock.Text = "✗ Audio recording failed - no file created";
                TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }
            
            var fileInfo = new FileInfo(tempFile);
            if (fileInfo.Length < 1000)
            {
                TestResultTextBlock.Text = "⚠ No audio detected. Check microphone connection.";
                TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                File.Delete(tempFile);
                return;
            }
            
            // Step 2: Test transcription
            TestResultTextBlock.Text = $"Step 2: Testing transcription... (Audio: {fileInfo.Length / 1024}KB)";
            
            try
            {
                var transcriptionService = (CarelessWhisperV2.Services.Transcription.WhisperTranscriptionService)_orchestrator.GetType()
                    .GetField("_transcriptionService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(_orchestrator);
                
                if (transcriptionService == null)
                {
                    TestResultTextBlock.Text = "✗ Transcription service not found";
                    TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    return;
                }
                
                // Check if transcription service is initialized
                var isInitialized = transcriptionService.IsInitialized;
                if (!isInitialized)
                {
                    TestResultTextBlock.Text = "⚠ Initializing transcription service...";
                    await transcriptionService.InitializeAsync("Base");
                }
                
                // Attempt transcription
                var result = await transcriptionService.TranscribeAsync(tempFile);
                
                if (!string.IsNullOrWhiteSpace(result.FullText))
                {
                    TestResultTextBlock.Text = $"✓ Test successful! Transcribed: \"{result.FullText.Substring(0, Math.Min(100, result.FullText.Length))}...\"";
                    TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    TestResultTextBlock.Text = "⚠ Audio recorded but no speech detected. Try speaking louder or closer to microphone.";
                    TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                }
            }
            catch (Exception transcriptionEx)
            {
                _logger.LogError(transcriptionEx, "Transcription test failed");
                TestResultTextBlock.Text = $"✗ Transcription failed: {transcriptionEx.Message}";
                TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            }
            
            // Cleanup
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Microphone test failed");
            TestResultTextBlock.Text = $"✗ Test failed: {ex.Message}";
            TestResultTextBlock.Foreground = System.Windows.Media.Brushes.Red;
        }
        finally
        {
            TestMicrophoneButton.IsEnabled = true;
        }
    }

    private void ModelSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateModelInfo();
    }

    private void UpdateModelInfo()
    {
        if (ModelSizeComboBox.SelectedItem is not ComboBoxItem selectedItem)
            return;
            
        var modelSize = selectedItem.Tag?.ToString() ?? "Base";
        
        switch (modelSize.ToLower())
        {
            case "tiny":
                ModelInfoTextBlock.Text = "Tiny Model - Fast processing, basic accuracy";
                ModelSizeTextBlock.Text = "Size: ~39M parameters, ~1GB RAM";
                ModelPerformanceTextBlock.Text = "Performance: Very fast, suitable for testing";
                break;
            case "base":
                ModelInfoTextBlock.Text = "Base Model - Balanced performance and accuracy";
                ModelSizeTextBlock.Text = "Size: ~74M parameters, ~1GB RAM";
                ModelPerformanceTextBlock.Text = "Performance: Good accuracy with reasonable speed";
                break;
            case "small":
                ModelInfoTextBlock.Text = "Small Model - Good accuracy";
                ModelSizeTextBlock.Text = "Size: ~244M parameters, ~2GB RAM";
                ModelPerformanceTextBlock.Text = "Performance: Better accuracy, slower processing";
                break;
            case "medium":
                ModelInfoTextBlock.Text = "Medium Model - High accuracy";
                ModelSizeTextBlock.Text = "Size: ~769M parameters, ~5GB RAM";
                ModelPerformanceTextBlock.Text = "Performance: High accuracy, requires more resources";
                break;
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Validate inputs
            if (!ValidateInputs())
                return;
                
            // Update settings object
            UpdateSettingsFromUI();
            
            // Save settings
            await _orchestrator.UpdateSettingsAsync(_settings);
            
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings");
            MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateInputs()
    {
        // Validate retention days
        if (!int.TryParse(RetentionDaysTextBox.Text, out var retentionDays) || retentionDays < 1)
        {
            MessageBox.Show("Retention period must be a positive number.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        
        // Validate hotkey
        if (string.IsNullOrWhiteSpace(HotkeyTextBox.Text))
        {
            MessageBox.Show("Please set a hotkey for push-to-talk.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        
        return true;
    }

    private void UpdateSettingsFromUI()
    {
        // General
        _settings.AutoStartWithWindows = AutoStartCheckBox.IsChecked ?? false;
        _settings.Theme = ((ComboBoxItem)ThemeComboBox.SelectedItem)?.Content?.ToString() ?? "Dark";
        _settings.Logging.EnableTranscriptionLogging = EnableLoggingCheckBox.IsChecked ?? true;
        _settings.Logging.SaveAudioFiles = SaveAudioFilesCheckBox.IsChecked ?? false;
        _settings.Logging.LogRetentionDays = int.Parse(RetentionDaysTextBox.Text);
        
        // Hotkeys
        _settings.Hotkeys.PushToTalkKey = HotkeyTextBox.Text;
        _settings.Hotkeys.RequireModifiers = RequireModifiersCheckBox.IsChecked ?? false;
        
        // Audio
        _settings.Audio.PreferredDeviceId = ((ComboBoxItem)MicrophoneComboBox.SelectedItem)?.Tag?.ToString() ?? "";
        
        var sampleRateText = ((ComboBoxItem)SampleRateComboBox.SelectedItem)?.Content?.ToString() ?? "16000 Hz";
        _settings.Audio.SampleRate = int.Parse(sampleRateText.Split(' ')[0]);
        
        var bufferSizeText = ((ComboBoxItem)BufferSizeComboBox.SelectedItem)?.Content?.ToString() ?? "1024";
        _settings.Audio.BufferSize = int.Parse(bufferSizeText);
        
        // Whisper
        _settings.Whisper.ModelSize = ((ComboBoxItem)ModelSizeComboBox.SelectedItem)?.Tag?.ToString() ?? "Base";
        _settings.Whisper.Language = ((ComboBoxItem)LanguageComboBox.SelectedItem)?.Tag?.ToString() ?? "auto";
        _settings.Whisper.EnableGpuAcceleration = EnableGpuCheckBox.IsChecked ?? true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
