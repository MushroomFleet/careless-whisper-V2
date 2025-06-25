using System.ComponentModel.DataAnnotations;

namespace CarelessWhisperV2.Models;

public class ApplicationSettings : IValidatableObject
{
    public string Theme { get; set; } = "Dark";
    public bool AutoStartWithWindows { get; set; } = false;
    public HotkeySettings Hotkeys { get; set; } = new();
    public AudioSettings Audio { get; set; } = new();
    public WhisperSettings Whisper { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Hotkeys == null)
            yield return new ValidationResult("Hotkeys configuration is required");
        
        if (Audio == null)
            yield return new ValidationResult("Audio configuration is required");
        
        if (Whisper == null)
            yield return new ValidationResult("Whisper configuration is required");
        
        if (Logging == null)
            yield return new ValidationResult("Logging configuration is required");
    }
}

public class HotkeySettings
{
    public string PushToTalkKey { get; set; } = "F1";
    public bool RequireModifiers { get; set; } = false;
    public List<string> Modifiers { get; set; } = new();
}

public class AudioSettings
{
    public string PreferredDeviceId { get; set; } = "";
    public int SampleRate { get; set; } = 16000;
    public int BufferSize { get; set; } = 1024;
}

public class WhisperSettings
{
    public string ModelSize { get; set; } = "Base";
    public bool EnableGpuAcceleration { get; set; } = true;
    public string Language { get; set; } = "auto";
}

public class LoggingSettings
{
    public bool EnableTranscriptionLogging { get; set; } = true;
    public bool SaveAudioFiles { get; set; } = false;
    public int LogRetentionDays { get; set; } = 30;
}
