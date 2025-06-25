using Microsoft.Extensions.Logging;
using CarelessWhisperV2.Services.Hotkeys;
using CarelessWhisperV2.Services.Audio;
using CarelessWhisperV2.Services.Transcription;
using CarelessWhisperV2.Services.Clipboard;
using CarelessWhisperV2.Services.Logging;
using CarelessWhisperV2.Services.Settings;
using CarelessWhisperV2.Models;
using System.IO;
using System.Windows;

namespace CarelessWhisperV2.Services.Orchestration;

public class TranscriptionOrchestrator : IDisposable
{
    private readonly PushToTalkManager _hotkeyManager;
    private readonly IAudioService _audioService;
    private readonly ITranscriptionService _transcriptionService;
    private readonly IClipboardService _clipboardService;
    private readonly ITranscriptionLogger _transcriptionLogger;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<TranscriptionOrchestrator> _logger;
    
    private string _currentRecordingPath = "";
    private bool _disposed;
    private ApplicationSettings _settings = new();

    public event EventHandler<TranscriptionCompletedEventArgs>? TranscriptionCompleted;
    public event EventHandler<TranscriptionErrorEventArgs>? TranscriptionError;

    public TranscriptionOrchestrator(
        PushToTalkManager hotkeyManager,
        IAudioService audioService,
        ITranscriptionService transcriptionService,
        IClipboardService clipboardService,
        ITranscriptionLogger transcriptionLogger,
        ISettingsService settingsService,
        ILogger<TranscriptionOrchestrator> logger)
    {
        _hotkeyManager = hotkeyManager;
        _audioService = audioService;
        _transcriptionService = transcriptionService;
        _clipboardService = clipboardService;
        _transcriptionLogger = transcriptionLogger;
        _settingsService = settingsService;
        _logger = logger;

        _hotkeyManager.TransmissionStarted += OnTransmissionStarted;
        _hotkeyManager.TransmissionEnded += OnTransmissionEnded;
        
        // Load settings
        _ = Task.Run(LoadSettingsAsync);
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing TranscriptionOrchestrator");
            
            // Initialize transcription service with current model
            await _transcriptionService.InitializeAsync(_settings.Whisper.ModelSize);
            
            _logger.LogInformation("TranscriptionOrchestrator initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize TranscriptionOrchestrator");
            throw;
        }
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            _settings = await _settingsService.LoadSettingsAsync<ApplicationSettings>();
            _logger.LogDebug("Settings loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings, using defaults");
            _settings = new ApplicationSettings();
        }
    }

    private async void OnTransmissionStarted()
    {
        try
        {
            _currentRecordingPath = GenerateRecordingPath();
            await _audioService.StartRecordingAsync(_currentRecordingPath);
            _logger.LogInformation("Recording started: {Path}", _currentRecordingPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start recording");
            TranscriptionError?.Invoke(this, new TranscriptionErrorEventArgs
            {
                Exception = ex,
                Message = "Failed to start recording"
            });
        }
    }

    private async void OnTransmissionEnded()
    {
        try
        {
            await _audioService.StopRecordingAsync();
            _logger.LogInformation("Recording stopped: {Path}", _currentRecordingPath);

            // Wait for file to be fully released - same fix as settings test
            await Task.Delay(1000);

            if (File.Exists(_currentRecordingPath))
            {
                var fileInfo = new FileInfo(_currentRecordingPath);
                _logger.LogInformation("Audio file created: {Path}, Size: {Size} bytes", _currentRecordingPath, fileInfo.Length);
                
                // Process transcription in background
                _ = Task.Run(async () => await ProcessTranscriptionAsync(_currentRecordingPath));
            }
            else
            {
                _logger.LogWarning("Audio file not found after recording: {Path}", _currentRecordingPath);
                TranscriptionError?.Invoke(this, new TranscriptionErrorEventArgs
                {
                    Message = "Audio file not found after recording"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop recording");
            TranscriptionError?.Invoke(this, new TranscriptionErrorEventArgs
            {
                Exception = ex,
                Message = "Failed to stop recording"
            });
        }
    }

    private async Task ProcessTranscriptionAsync(string audioFilePath)
    {
        var startTime = DateTime.Now;
        
        try
        {
            _logger.LogInformation("Starting transcription: {Path}", audioFilePath);
            
            var result = await _transcriptionService.TranscribeAsync(audioFilePath);
            
            if (!string.IsNullOrWhiteSpace(result.FullText))
            {
                // Copy to clipboard on UI thread (required for STA thread)
                _logger.LogInformation("Attempting to copy text to clipboard: {Text}", result.FullText.Substring(0, Math.Min(50, result.FullText.Length)));
                
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Use direct WPF clipboard - safe and synchronous
                        System.Windows.Clipboard.SetText(result.FullText);
                        _logger.LogInformation("Successfully copied text to clipboard");
                    });
                }
                catch (Exception clipboardEx)
                {
                    _logger.LogError(clipboardEx, "Failed to copy text to clipboard: {Error}", clipboardEx.Message);
                    // Don't throw - continue with logging and event notification
                }
                
                // Log to file if enabled
                if (_settings.Logging.EnableTranscriptionLogging)
                {
                    var transcriptionEntry = new TranscriptionEntry
                    {
                        Timestamp = startTime,
                        FullText = result.FullText,
                        Segments = result.Segments,
                        Language = result.Language,
                        Duration = DateTime.Now - startTime,
                        ModelUsed = _settings.Whisper.ModelSize,
                        AudioFilePath = _settings.Logging.SaveAudioFiles ? audioFilePath : null
                    };
                    
                    await _transcriptionLogger.LogTranscriptionAsync(transcriptionEntry);
                }
                
                _logger.LogInformation("Transcription completed: {Text}", result.FullText);
                
                TranscriptionCompleted?.Invoke(this, new TranscriptionCompletedEventArgs
                {
                    TranscriptionResult = result,
                    ProcessingTime = DateTime.Now - startTime
                });
            }
            else
            {
                _logger.LogWarning("Transcription returned empty result");
                TranscriptionError?.Invoke(this, new TranscriptionErrorEventArgs
                {
                    Message = "No speech detected in audio"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transcription failed: {Path}. Error details: {ErrorMessage}", audioFilePath, ex.Message);
            TranscriptionError?.Invoke(this, new TranscriptionErrorEventArgs
            {
                Exception = ex,
                Message = $"Transcription processing failed: {ex.Message}"
            });
        }
        finally
        {
            // Clean up temporary audio file (unless settings say to keep it)
            if (!_settings.Logging.SaveAudioFiles)
            {
                try
                {
                    if (File.Exists(audioFilePath))
                    {
                        File.Delete(audioFilePath);
                        _logger.LogDebug("Deleted temporary audio file: {Path}", audioFilePath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete temporary file: {Path}", audioFilePath);
                }
            }
        }
    }

    private string GenerateRecordingPath()
    {
        var tempPath = Path.GetTempPath();
        var fileName = $"whisper_recording_{DateTime.Now:yyyyMMdd_HHmmss_fff}.wav";
        return Path.Combine(tempPath, fileName);
    }

    public bool IsRecording => _audioService.IsRecording;

    public bool IsTransmitting => _hotkeyManager.IsTransmitting;

    public async Task UpdateSettingsAsync(ApplicationSettings newSettings)
    {
        try
        {
            await _settingsService.SaveSettingsAsync(newSettings);
            _settings = newSettings;
            
            // Reinitialize transcription service if model changed
            if (!_transcriptionService.IsInitialized || _settings.Whisper.ModelSize != newSettings.Whisper.ModelSize)
            {
                await _transcriptionService.InitializeAsync(newSettings.Whisper.ModelSize);
            }
            
            _logger.LogInformation("Settings updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update settings");
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _hotkeyManager?.Dispose();
            _audioService?.Dispose();
            _transcriptionService?.Dispose();
            _disposed = true;
            _logger.LogInformation("TranscriptionOrchestrator disposed");
        }
    }
}

public class TranscriptionCompletedEventArgs : EventArgs
{
    public TranscriptionResult TranscriptionResult { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}

public class TranscriptionErrorEventArgs : EventArgs
{
    public string Message { get; set; } = "";
    public Exception? Exception { get; set; }
}
