# Careless Whisper V2

**Speech-to-Paste for Windows** • Silent system tray interface • Built with .NET 8.0

Transform your voice into text instantly with a simple push-to-talk hotkey. No windows, no interruptions—just speak and paste anywhere.

## ✨ Core Features

- **🎙️ Push-to-Talk**: Hold F1 → Speak → Release → Paste
- **🔒 100% Local**: All processing happens on your machine (no cloud, no data sharing)
- **👻 Silent Interface**: Lives quietly in your system tray
- **📋 Instant Paste**: Text automatically copied to clipboard for immediate use
- **⚡ Fast & Accurate**: Powered by OpenAI's Whisper for high-quality transcription

## 🚀 Quick Start

### Prerequisites
- Windows 10/11
- .NET 8.0 Runtime
- Any microphone

### Installation
1. Download the latest release
2. Extract and run `CarelessWhisperV2.exe`
3. The app minimizes to your system tray (look for the icon near your clock)

### Usage
1. **Hold F1** and speak clearly
2. **Release F1** when finished
3. **Paste anywhere** with Ctrl+V

That's it! Your speech is now text ready to use in any application.

## 🎯 Perfect For

- **Quick Notes**: Capture thoughts without breaking your workflow
- **Dictation**: Write emails, documents, and messages hands-free
- **Accessibility**: Voice input for any Windows application
- **Productivity**: Skip typing for repetitive text entry

## ⚙️ Configuration

Right-click the system tray icon to:
- Change hotkey (default: F1)
- Select microphone
- Adjust Whisper model (Tiny → Large for speed vs. accuracy)
- Configure auto-start with Windows

## 🔧 Technical Details

- **Architecture**: Clean .NET 8.0 with dependency injection
- **Audio**: NAudio for high-quality recording
- **Transcription**: Whisper.NET for local speech recognition
- **Hotkeys**: SharpHook for global key detection
- **UI**: Modern system tray with H.NotifyIcon

### Whisper Models
- **Tiny**: Fastest, good for simple speech
- **Base**: Balanced (recommended)
- **Small/Medium/Large**: Higher accuracy, slower processing

Models download automatically on first use and are cached locally.

## 🛠️ Build from Source

```bash
git clone https://github.com/[username]/careless-whisper-v2
cd careless-whisper-v2/src/CarelessWhisperV2
dotnet build
dotnet run
```

Requires .NET 8.0 SDK for development.

## 🔐 Privacy & Security

- **No internet required**: All transcription happens locally
- **No data collection**: Your voice never leaves your computer
- **Optional logging**: Transcription history saved locally (can be disabled)
- **Open source**: Full transparency of how your data is handled

## 📝 Status

**Current Version**: 2.0.0 - First stable release

✅ **Working**: Core speech-to-paste functionality  
🚧 **Coming Soon**: Settings UI, transcription history viewer, custom themes

## 🤝 Contributing

This project implements production-ready .NET 8.0 patterns. See [DOTNET-DEVTEAM-HANDOFF.md](docs/DOTNET-DEVTEAM-HANDOFF.md) for architecture details.

## 📄 License

[License to be determined]

---

**Made for developers, writers, and anyone who thinks faster than they type.**
