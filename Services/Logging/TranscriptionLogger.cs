using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CarelessWhisperV2.Models;

namespace CarelessWhisperV2.Services.Logging;

public class TranscriptionLogger : ITranscriptionLogger
{
    private readonly ILogger<TranscriptionLogger> _logger;
    private readonly ApplicationSettings _settings;
    private readonly string _logDirectory;
    private readonly string _logFilePath;

    public TranscriptionLogger(ILogger<TranscriptionLogger> logger, IOptions<ApplicationSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
        
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _logDirectory = Path.Combine(appDataPath, "CarelessWhisperV2", "Logs");
        _logFilePath = Path.Combine(_logDirectory, "transcriptions.jsonl");
        
        Directory.CreateDirectory(_logDirectory);
    }

    public async Task LogTranscriptionAsync(TranscriptionEntry entry)
    {
        if (!_settings.Logging.EnableTranscriptionLogging)
            return;

        try
        {
            entry.Timestamp = DateTime.Now;
            if (string.IsNullOrEmpty(entry.Id))
                entry.Id = Guid.NewGuid().ToString();

            var json = JsonSerializer.Serialize(entry);
            await File.AppendAllTextAsync(_logFilePath, json + Environment.NewLine);
            
            _logger.LogInformation("Transcription logged: {Id}", entry.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log transcription entry");
            throw;
        }
    }

    public async Task<List<TranscriptionEntry>> GetTranscriptionsAsync(DateTime? date = null)
    {
        var entries = new List<TranscriptionEntry>();
        
        if (!File.Exists(_logFilePath))
            return entries;

        try
        {
            var lines = await File.ReadAllLinesAsync(_logFilePath);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                    
                try
                {
                    var entry = JsonSerializer.Deserialize<TranscriptionEntry>(line);
                    if (entry != null)
                    {
                        if (date == null || entry.Timestamp.Date == date.Value.Date)
                        {
                            entries.Add(entry);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize transcription entry from line: {Line}", line);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read transcription log file");
            throw;
        }

        return entries;
    }

    public async Task<List<TranscriptionEntry>> GetTranscriptionHistoryAsync()
    {
        return await GetTranscriptionsAsync();
    }

    public async Task<List<TranscriptionEntry>> SearchTranscriptionsAsync(string searchTerm, DateTime? startDate = null, DateTime? endDate = null)
    {
        var allEntries = await GetTranscriptionsAsync();
        
        var filteredEntries = allEntries.Where(entry =>
        {
            // Date filter
            if (startDate.HasValue && entry.Timestamp.Date < startDate.Value.Date)
                return false;
            if (endDate.HasValue && entry.Timestamp.Date > endDate.Value.Date)
                return false;
                
            // Text search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                return entry.FullText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
            }
            
            return true;
        }).ToList();

        return filteredEntries;
    }

    public async Task<int> CleanupOldTranscriptionsAsync(int retentionDays = 30)
    {
        if (!File.Exists(_logFilePath))
            return 0;

        try
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionDays);
            var allEntries = await GetTranscriptionsAsync();
            
            var entriesToKeep = allEntries.Where(entry => entry.Timestamp >= cutoffDate).ToList();
            var deletedCount = allEntries.Count - entriesToKeep.Count;
            
            if (deletedCount > 0)
            {
                // Rewrite the file with only the entries to keep
                var tempFilePath = _logFilePath + ".tmp";
                
                await using (var writer = new StreamWriter(tempFilePath))
                {
                    foreach (var entry in entriesToKeep)
                    {
                        var json = JsonSerializer.Serialize(entry);
                        await writer.WriteLineAsync(json);
                    }
                }
                
                File.Move(tempFilePath, _logFilePath, true);
                
                // Clean up associated audio files if they exist
                foreach (var entry in allEntries.Where(e => e.Timestamp < cutoffDate))
                {
                    if (!string.IsNullOrEmpty(entry.AudioFilePath) && File.Exists(entry.AudioFilePath))
                    {
                        try
                        {
                            File.Delete(entry.AudioFilePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete audio file: {FilePath}", entry.AudioFilePath);
                        }
                    }
                }
                
                _logger.LogInformation("Cleaned up {Count} old transcription entries", deletedCount);
            }
            
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old transcriptions");
            throw;
        }
    }

    public async Task DeleteTranscriptionAsync(string id)
    {
        if (!File.Exists(_logFilePath))
            return;

        try
        {
            var allEntries = await GetTranscriptionsAsync();
            var entryToDelete = allEntries.FirstOrDefault(e => e.Id == id);
            
            if (entryToDelete == null)
                return;

            var entriesToKeep = allEntries.Where(entry => entry.Id != id).ToList();
            
            // Rewrite the file without the deleted entry
            var tempFilePath = _logFilePath + ".tmp";
            
            await using (var writer = new StreamWriter(tempFilePath))
            {
                foreach (var entry in entriesToKeep)
                {
                    var json = JsonSerializer.Serialize(entry);
                    await writer.WriteLineAsync(json);
                }
            }
            
            File.Move(tempFilePath, _logFilePath, true);
            
            // Clean up associated audio file if it exists
            if (!string.IsNullOrEmpty(entryToDelete.AudioFilePath) && File.Exists(entryToDelete.AudioFilePath))
            {
                try
                {
                    File.Delete(entryToDelete.AudioFilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete audio file: {FilePath}", entryToDelete.AudioFilePath);
                }
            }
            
            _logger.LogInformation("Deleted transcription entry: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete transcription entry: {Id}", id);
            throw;
        }
    }

    public async Task<int> GetTranscriptionCountAsync(DateTime? date = null)
    {
        var entries = await GetTranscriptionsAsync(date);
        return entries.Count;
    }
}
