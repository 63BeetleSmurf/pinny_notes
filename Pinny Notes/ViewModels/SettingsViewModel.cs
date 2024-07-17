using CommunityToolkit.Mvvm.ComponentModel;

namespace Pinny_Notes.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel()
    {
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
}
