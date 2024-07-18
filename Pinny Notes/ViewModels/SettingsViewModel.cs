using CommunityToolkit.Mvvm.ComponentModel;
using Pinny_Notes.Enums;
using System.Collections.Generic;

namespace Pinny_Notes.ViewModels;

public partial class SettingsViewModel : ObservableObject
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

    public SettingsViewModel()
    {
        _startupPosition = (StartupPositions)Properties.Settings.Default.StartupPosition;
        _cycleColors = Properties.Settings.Default.CycleColors;
        _trimCopiedText = Properties.Settings.Default.TrimCopiedText;
        _trimPastedText = Properties.Settings.Default.TrimPastedText;
        _middleClickPaste = Properties.Settings.Default.MiddleClickPaste;
        _autoCopy = Properties.Settings.Default.AutoCopy;
        _spellChecker = Properties.Settings.Default.SpellCheck;
        _newLineAtEnd = Properties.Settings.Default.NewLine;
        _keepNewLineAtEndVisible = Properties.Settings.Default.KeepNewLineAtEndVisible;
        _autoIndent = Properties.Settings.Default.AutoIndent;
        _allowMinimizeWhenPinned = Properties.Settings.Default.AllowMinimizeWhenPinned;
        _transparentNotes = Properties.Settings.Default.TransparentNotes;
        _opaqueWhenFocused = Properties.Settings.Default.OpaqueWhenFocused;
        _onlyTransparentWhenPinned = Properties.Settings.Default.OnlyTransparentWhenPinned;
    }

    public KeyValuePair<StartupPositions, string>[] StartupPositionsList => _startupPositionsList;

    [ObservableProperty]
    private StartupPositions _startupPosition;
    partial void OnStartupPositionChanged(StartupPositions value)
    {
        Properties.Settings.Default.StartupPosition = (int)value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _cycleColors;
    partial void OnCycleColorsChanged(bool value)
    {
        Properties.Settings.Default.CycleColors = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _trimCopiedText;
    partial void OnTrimCopiedTextChanged(bool value)
    {
        Properties.Settings.Default.TrimCopiedText = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _trimPastedText;
    partial void OnTrimPastedTextChanged(bool value)
    {
        Properties.Settings.Default.TrimPastedText = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _middleClickPaste;
    partial void OnMiddleClickPasteChanged(bool value)
    {
        Properties.Settings.Default.MiddleClickPaste = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _autoCopy;
    partial void OnAutoCopyChanged(bool value)
    {
        Properties.Settings.Default.AutoCopy = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _spellChecker;
    partial void OnSpellCheckerChanged(bool value)
    {
        Properties.Settings.Default.SpellCheck = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _newLineAtEnd;
    partial void OnNewLineAtEndChanged(bool value)
    {
        Properties.Settings.Default.NewLine = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _keepNewLineAtEndVisible;
    partial void OnKeepNewLineAtEndVisibleChanged(bool value)
    {
        Properties.Settings.Default.KeepNewLineAtEndVisible = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _autoIndent;
    partial void OnAutoIndentChanged(bool value)
    {
        Properties.Settings.Default.AutoIndent = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _allowMinimizeWhenPinned;
    partial void OnAllowMinimizeWhenPinnedChanged(bool value)
    {
        Properties.Settings.Default.AllowMinimizeWhenPinned = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _transparentNotes;
    partial void OnTransparentNotesChanged(bool value)
    {
        Properties.Settings.Default.TransparentNotes = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _opaqueWhenFocused;
    partial void OnOpaqueWhenFocusedChanged(bool value)
    {
        Properties.Settings.Default.OpaqueWhenFocused = value;
        Properties.Settings.Default.Save();
    }

    [ObservableProperty]
    private bool _onlyTransparentWhenPinned;
    partial void OnOnlyTransparentWhenPinnedChanged(bool value)
    {
        Properties.Settings.Default.OnlyTransparentWhenPinned = value;
        Properties.Settings.Default.Save();
    }
}
