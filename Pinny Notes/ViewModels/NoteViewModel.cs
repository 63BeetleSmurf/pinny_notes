using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Media;
using Pinny_Notes.Enums;

namespace Pinny_Notes.ViewModels;

public partial class NoteViewModel : ObservableObject
{
    public NoteViewModel(NoteViewModel? parent = null)
    {
        InitNoteColor(parent);
        InitNotePosition(parent);
    }

    private void InitNoteColor(NoteViewModel? parent = null)
    {
        // Set this first as cycle colors wont trigger a change if the next color if the default for ThemeColors
        CurrentThemeColor = (ThemeColors)Properties.Settings.Default.Color;
        if (Properties.Settings.Default.CycleColors)
        {
            int themeColorIndex = GetNextThemeColorIndex((int)CurrentThemeColor);
            if (parent != null && themeColorIndex == (int)parent.CurrentThemeColor)
                themeColorIndex = GetNextThemeColorIndex(themeColorIndex);
            CurrentThemeColor = (ThemeColors)themeColorIndex;
        }
    }

    private int GetNextThemeColorIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        if (!Enum.IsDefined((ThemeColors)nextIndex))
            nextIndex = 0;
        return nextIndex;
    }

    private void InitNotePosition(NoteViewModel? parent = null)
    {
        GravityX = parent?.GravityX ?? (Properties.Settings.Default.StartupPositionLeft ? 1 : -1);
        GravityY = parent?.GravityY ?? (Properties.Settings.Default.StartupPositionTop ? 1 : -1);

        if (parent != null)
        {
            double newX = parent.X + (45 * GravityX);
            if (newX < 0)
                X = 0;
            else if (newX + Width > SystemParameters.PrimaryScreenWidth)
                X = SystemParameters.PrimaryScreenWidth - Width;
            else
                X = newX;

            double newY = parent.Y + (45 * GravityY);
            if (newY < 0)
                Y = 0;
            else if (newY + Height > SystemParameters.PrimaryScreenHeight)
                Y = SystemParameters.PrimaryScreenHeight - Height;
            else
                Y = newY;
        }
        else
        {
            int screenMargin = 78;
            if (Properties.Settings.Default.StartupPositionLeft)
                X = screenMargin;
            else // Right
                X = (SystemParameters.PrimaryScreenWidth - screenMargin) - Width;

            if (Properties.Settings.Default.StartupPositionTop)
                Y = screenMargin;
            else // Bottom
                Y = (SystemParameters.PrimaryScreenHeight - screenMargin) - Height;
        }
    }

    private void UpdateBrushes(ThemeColors themeColor)
    {
        switch (themeColor)
        {
            default:
            case ThemeColors.Yellow:
                TitleBarColorBrush = new(Color.FromRgb(254, 247, 177));     // #fef7b1
                BackgroundColorBrush = new(Color.FromRgb(255, 252, 221));   // #fffcdd
                BorderColorBrush = new(Color.FromRgb(254, 234, 0));         // #feea00
                break;
            case ThemeColors.Orange:
                TitleBarColorBrush = new(Color.FromRgb(255, 209, 121));     // #ffd179
                BackgroundColorBrush = new(Color.FromRgb(254, 232, 185));   // #fee8b9
                BorderColorBrush = new(Color.FromRgb(255, 171, 0));         // #ffab00
                break;
            case ThemeColors.Red:
                TitleBarColorBrush = new(Color.FromRgb(255, 124, 129));     // #ff7c81
                BackgroundColorBrush = new(Color.FromRgb(255, 196, 198));   // #ffc4c6
                BorderColorBrush = new(Color.FromRgb(227, 48, 54));         // #e33036
                break;
            case ThemeColors.Pink:
                TitleBarColorBrush = new(Color.FromRgb(217, 134, 204));     // #d986cc
                BackgroundColorBrush = new(Color.FromRgb(235, 191, 227));   // #ebbfe3
                BorderColorBrush = new(Color.FromRgb(167, 41, 149));        // #a72995
                break;
            case ThemeColors.Purple:
                TitleBarColorBrush = new(Color.FromRgb(157, 154, 221));     // #9d9add
                BackgroundColorBrush = new(Color.FromRgb(208, 206, 243));   // #d0cef3
                BorderColorBrush = new(Color.FromRgb(98, 91, 184));         // #625bb8
                break;
            case ThemeColors.Blue:
                TitleBarColorBrush = new(Color.FromRgb(122, 195, 230));     // #7ac3e6
                BackgroundColorBrush = new(Color.FromRgb(179, 217, 236));   // #b3d9ec
                BorderColorBrush = new(Color.FromRgb(17, 149, 221));        // #1195dd
                break;
            case ThemeColors.Aqua:
                TitleBarColorBrush = new(Color.FromRgb(151, 207, 198));     // #97cfc6
                BackgroundColorBrush = new(Color.FromRgb(192, 226, 225));   // #c0e2e1
                BorderColorBrush = new(Color.FromRgb(22, 176, 152));        // #16b098
                break;
            case ThemeColors.Green:
                TitleBarColorBrush = new(Color.FromRgb(198, 214, 125));     // #c6d67d
                BackgroundColorBrush = new(Color.FromRgb(227, 235, 198));   // #e3ebc6
                BorderColorBrush = new(Color.FromRgb(170, 204, 4));         // #aacc04
                break;
        }
    }

    [ObservableProperty]
    private ThemeColors _currentThemeColor;
    partial void OnCurrentThemeColorChanged(ThemeColors value)
    {
        Properties.Settings.Default.Color = (int)value;
        Properties.Settings.Default.Save();
        UpdateBrushes(value);
    }

    [ObservableProperty]
    private SolidColorBrush _titleBarColorBrush = null!;
    [ObservableProperty]
    private SolidColorBrush _backgroundColorBrush = null!;
    [ObservableProperty]
    private SolidColorBrush _borderColorBrush = null!;

    public int GravityX;
    public int GravityY;

    [ObservableProperty]
    private double _x;
    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _width = 300;
    [ObservableProperty]
    private double _height = 300;

    [ObservableProperty]
    private bool _isPinned = false;
    [ObservableProperty]
    private bool _isSaved = false;

    [ObservableProperty]
    private string _content = "";

    [RelayCommand]
    private void ChangeThemeColor(ThemeColors themeColor) => CurrentThemeColor = themeColor;
}
