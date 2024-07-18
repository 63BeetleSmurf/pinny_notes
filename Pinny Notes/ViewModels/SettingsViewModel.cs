using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Pinny_Notes.Enums;
using Pinny_Notes.Properties;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Pinny_Notes.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private static readonly KeyValuePair<StartupPositions, string>[] _startupPositionsList = [
        new(StartupPositions.TopLeft, "Top Left"),
        new(StartupPositions.TopCenter, "Top Center"),
        new(StartupPositions.TopRight, "Top Right"),
        new(StartupPositions.MiddleLeft, "Middle Left"),
        new(StartupPositions.MiddleCenter, "Middle Center"),
        new(StartupPositions.MiddleRight, "Middle Right"),
        new(StartupPositions.BottomLeft, "Bottom Left"),
        new(StartupPositions.BottomCenter, "Bottom Center"),
        new(StartupPositions.BottomRight, "Bottom Right")
    ];

    private static readonly KeyValuePair<MinimizeModes, string>[] _minimizeModeList = [
        new(MinimizeModes.Allow, "Yes"),
        new(MinimizeModes.Prevent, "No"),
        new(MinimizeModes.PreventIfPinned, "When not pinned")
    ];

    public SettingsViewModel()
    {
        _startupPosition = (StartupPositions)Properties.Settings.Default.StartupPosition;
        _cycleColors = Properties.Settings.Default.CycleColors;
        _trimCopiedText = Properties.Settings.Default.TrimCopiedText;
        _trimPastedText = Properties.Settings.Default.TrimPastedText;
        _middleClickPaste = Properties.Settings.Default.MiddleClickPaste;
        _autoCopy = Properties.Settings.Default.AutoCopy;
        _spellChecker = Properties.Settings.Default.SpellCheck;
        _newLineAtEnd = Properties.Settings.Default.NewLineAtEnd;
        _keepNewLineAtEndVisible = Properties.Settings.Default.KeepNewLineAtEndVisible;
        _autoIndent = Properties.Settings.Default.AutoIndent;
        _minimizeMode = (MinimizeModes)Properties.Settings.Default.MinimizeMode;
        _transparentNotes = Properties.Settings.Default.TransparentNotes;
        _opaqueWhenFocused = Properties.Settings.Default.OpaqueWhenFocused;
        _onlyTransparentWhenPinned = Properties.Settings.Default.OnlyTransparentWhenPinned;
    }

    public KeyValuePair<StartupPositions, string>[] StartupPositionsList => _startupPositionsList;
    public KeyValuePair<MinimizeModes, string>[] MinimizeModeList => _minimizeModeList;

    private void UpdateSetting(string settingName, object oldValue, object newValue)
    {
        Settings.Default[settingName] = newValue;
        Settings.Default.Save();

        Messenger.Send(new PropertyChangedMessage<object>(this, settingName, oldValue, newValue));
    }

    [ObservableProperty]
    private StartupPositions _startupPosition;
    partial void OnStartupPositionChanged(StartupPositions oldValue, StartupPositions newValue) =>
        UpdateSetting(nameof(StartupPosition), oldValue, newValue);

    [ObservableProperty]
    private bool _cycleColors;
    partial void OnCycleColorsChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(CycleColors), oldValue, newValue);

    [ObservableProperty]
    private bool _trimCopiedText;
    partial void OnTrimCopiedTextChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(TrimCopiedText), oldValue, newValue);

    [ObservableProperty]
    private bool _trimPastedText;
    partial void OnTrimPastedTextChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(TrimPastedText), oldValue, newValue);

    [ObservableProperty]
    private bool _middleClickPaste;
    partial void OnMiddleClickPasteChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(MiddleClickPaste), oldValue, newValue);

    [ObservableProperty]
    private bool _autoCopy;
    partial void OnAutoCopyChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(AutoCopy), oldValue, newValue);

    [ObservableProperty]
    private bool _spellChecker;
    partial void OnSpellCheckerChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(SpellCheck), oldValue, newValue);

    [ObservableProperty]
    private bool _newLineAtEnd;
    partial void OnNewLineAtEndChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(NewLineAtEnd), oldValue, newValue);

    [ObservableProperty]
    private bool _keepNewLineAtEndVisible;
    partial void OnKeepNewLineAtEndVisibleChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(KeepNewLineAtEndVisible), oldValue, newValue);

    [ObservableProperty]
    private bool _autoIndent;
    partial void OnAutoIndentChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(AutoIndent), oldValue, newValue);

    [ObservableProperty]
    private MinimizeModes _minimizeMode;
    partial void OnMinimizeModeChanged(MinimizeModes oldValue, MinimizeModes newValue) =>
        UpdateSetting(nameof(MinimizeMode), oldValue, newValue);

    [ObservableProperty]
    private bool _transparentNotes;
    partial void OnTransparentNotesChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(TransparentNotes), oldValue, newValue);

    [ObservableProperty]
    private bool _opaqueWhenFocused;
    partial void OnOpaqueWhenFocusedChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(OpaqueWhenFocused), oldValue, newValue);

    [ObservableProperty]
    private bool _onlyTransparentWhenPinned;
    partial void OnOnlyTransparentWhenPinnedChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(OnlyTransparentWhenPinned), oldValue, newValue);
}
