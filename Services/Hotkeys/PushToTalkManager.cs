using SharpHook;
using SharpHook.Native;
using Microsoft.Extensions.Logging;

namespace CarelessWhisperV2.Services.Hotkeys;

public class PushToTalkManager : IDisposable
{
    private readonly TaskPoolGlobalHook _hook;
    private readonly KeyCode _pushToTalkKey;
    private readonly ILogger<PushToTalkManager> _logger;
    private bool _isTransmitting;
    private readonly object _transmissionLock = new object();
    private int _hookRestartCount;
    private const int MaxRestartAttempts = 3;
    private bool _disposed;

    public event Action? TransmissionStarted;
    public event Action? TransmissionEnded;

    public PushToTalkManager(ILogger<PushToTalkManager> logger, KeyCode pushToTalkKey = KeyCode.VcF1)
    {
        _pushToTalkKey = pushToTalkKey;
        _logger = logger;
        _hook = new TaskPoolGlobalHook();
        
        _hook.KeyPressed += OnKeyPressed;
        _hook.KeyReleased += OnKeyReleased;
        
        // Start hook on background thread for optimal performance
        Task.Run(async () => await StartHookAsync());
    }

    private async Task StartHookAsync()
    {
        try
        {
            await _hook.RunAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hook failed, attempting restart");
            await RestartHook();
        }
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Data.KeyCode == _pushToTalkKey)
        {
            lock (_transmissionLock)
            {
                if (!_isTransmitting)
                {
                    _isTransmitting = true;
                    _logger.LogDebug("Push-to-talk started");
                    TransmissionStarted?.Invoke();
                }
            }
            
            // Suppress key to prevent interference with other applications
            e.SuppressEvent = true;
        }
    }

    private void OnKeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Data.KeyCode == _pushToTalkKey)
        {
            lock (_transmissionLock)
            {
                if (_isTransmitting)
                {
                    _isTransmitting = false;
                    _logger.LogDebug("Push-to-talk ended");
                    TransmissionEnded?.Invoke();
                }
            }
            
            e.SuppressEvent = true;
        }
    }

    public bool IsTransmitting
    {
        get
        {
            lock (_transmissionLock)
            {
                return _isTransmitting;
            }
        }
    }

    private async Task RestartHook()
    {
        if (_hookRestartCount < MaxRestartAttempts)
        {
            _hookRestartCount++;
            await Task.Delay(1000 * _hookRestartCount); // Exponential backoff
            
            try
            {
                await _hook.RunAsync();
                _hookRestartCount = 0; // Reset on successful restart
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hook restart attempt {Attempt} failed", _hookRestartCount);
                await RestartHook();
            }
        }
        else
        {
            _logger.LogCritical("Hook restart limit exceeded");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _hook?.Dispose();
            _disposed = true;
            _logger.LogInformation("PushToTalkManager disposed");
        }
    }
}
