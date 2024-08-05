using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.Generic;
using System.Windows.Controls;
using Pinny_Notes.Enums;
using Pinny_Notes.Properties;

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

    private static readonly KeyValuePair<ColorModes, string>[] _colorModeList = [
        new(ColorModes.Light, "Light"),
        new(ColorModes.Dark, "Dark"),
        new(ColorModes.System, "System Default")
    ];

    public SettingsViewModel()
    {
        _startupPosition = (StartupPositions)Settings.Default.StartupPosition;
        _cycleColors = Settings.Default.CycleColors;
        _trimCopiedText = Settings.Default.TrimCopiedText;
        _trimPastedText = Settings.Default.TrimPastedText;
        _middleClickPaste = Settings.Default.MiddleClickPaste;
        _autoCopy = Settings.Default.AutoCopy;
        _spellChecker = Settings.Default.SpellCheck;
        _newLineAtEnd = Settings.Default.NewLineAtEnd;
        _keepNewLineAtEndVisible = Settings.Default.KeepNewLineAtEndVisible;
        _autoIndent = Settings.Default.AutoIndent;
        _tabSpaces = Settings.Default.TabSpaces;
        _tabWidth = Settings.Default.TabWidth;
        _convertIndentation = Settings.Default.ConvertIndentation;
        _minimizeMode = (MinimizeModes)Settings.Default.MinimizeMode;
        _transparentNotes = Settings.Default.TransparentNotes;
        _opaqueWhenFocused = Settings.Default.OpaqueWhenFocused;
        _onlyTransparentWhenPinned = Settings.Default.OnlyTransparentWhenPinned;
        _colorMode = (ColorModes)Settings.Default.ColorMode;
    }

    public KeyValuePair<StartupPositions, string>[] StartupPositionsList => _startupPositionsList;
    public KeyValuePair<MinimizeModes, string>[] MinimizeModeList => _minimizeModeList;
    public KeyValuePair<ColorModes, string>[] ColorModeList => _colorModeList;

    private void UpdateSetting(string settingName, object oldValue, object newValue)
    {
        Settings.Default[settingName] = newValue;
        Settings.Default.Save();

        Messenger.Send(new PropertyChangedMessage<object>(this, settingName, oldValue, newValue));
    }

    [ObservableProperty]
    private StartupPositions _startupPosition;
    partial void OnStartupPositionChanged(StartupPositions oldValue, StartupPositions newValue) =>
        UpdateSetting(nameof(StartupPosition), (int)oldValue, (int)newValue);

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
    private bool _tabSpaces;
    partial void OnTabSpacesChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(TabSpaces), oldValue, newValue);

    [ObservableProperty]
    private int _tabWidth;
    partial void OnTabWidthChanged(int oldValue, int newValue) =>
        UpdateSetting(nameof(TabWidth), oldValue, newValue);

    [ObservableProperty]
    private bool _convertIndentation;
    partial void OnConvertIndentationChanged(bool oldValue, bool newValue) =>
        UpdateSetting(nameof(ConvertIndentation), oldValue, newValue);

    [ObservableProperty]
    private MinimizeModes _minimizeMode;
    partial void OnMinimizeModeChanged(MinimizeModes oldValue, MinimizeModes newValue) =>
        UpdateSetting(nameof(MinimizeMode), (int)oldValue, (int)newValue);

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

    [ObservableProperty]
    private ColorModes _colorMode;
    partial void OnColorModeChanged(ColorModes oldValue, ColorModes newValue) =>
        UpdateSetting(nameof(ColorMode), (int)oldValue, (int)newValue);
}
