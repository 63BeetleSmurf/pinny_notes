using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Pinny_Notes.Enums;
using Pinny_Notes.Helpers;
using Pinny_Notes.Properties;

namespace Pinny_Notes.ViewModels;

public partial class NoteViewModel : ObservableRecipient, IRecipient<PropertyChangedMessage<object>>
{
    private const double _opaqueOpacity = 1.0;
    private const double _transparentOpacity = 0.8;

    private static readonly Dictionary<ThemeColors, Color[]> _colors = new() {
        {
            ThemeColors.Yellow,
            new[] {
                Color.FromRgb(254, 247, 177),   // #fef7b1
                Color.FromRgb(255, 252, 221),   // #fffcdd
                Color.FromRgb(254, 234, 0),     // #feea00
            }
        },
        {
            ThemeColors.Orange,
            new[] {
                Color.FromRgb(255, 209, 121),   // #ffd179
                Color.FromRgb(254, 232, 185),   // #fee8b9
                Color.FromRgb(255, 171, 0),     // #ffab00
            }
        },
        {
            ThemeColors.Red,
            new[] {
                Color.FromRgb(255, 124, 129),   // #ff7c81
                Color.FromRgb(255, 196, 198),   // #ffc4c6
                Color.FromRgb(227, 48, 54),     // #e33036
            }
        },
        {
            ThemeColors.Pink,
            new[] {
                Color.FromRgb(217, 134, 204),   // #d986cc
                Color.FromRgb(235, 191, 227),   // #ebbfe3
                Color.FromRgb(167, 41, 149),    // #a72995
            }
        },
        {
            ThemeColors.Purple,
            new[] {
                Color.FromRgb(157, 154, 221),   // #9d9add
                Color.FromRgb(208, 206, 243),   // #d0cef3
                Color.FromRgb(98, 91, 184),     // #625bb8
            }
        },
        {
            ThemeColors.Blue,
            new[] {
                Color.FromRgb(122, 195, 230),   // #7ac3e6
                Color.FromRgb(179, 217, 236),   // #b3d9ec
                Color.FromRgb(17, 149, 221),    // #1195dd
            }
        },
        {
            ThemeColors.Aqua,
            new[] {
                Color.FromRgb(151, 207, 198),   // #97cfc6
                Color.FromRgb(192, 226, 225),   // #c0e2e1
                Color.FromRgb(22, 176, 152),    // #16b098
           }
        },
        {
            ThemeColors.Green,
            new[] {
                Color.FromRgb(198, 214, 125),   // #c6d67d
                Color.FromRgb(227, 235, 198),   // #e3ebc6
                Color.FromRgb(170, 204, 4),     // #aacc04
            }
        }
    };

    public void Receive(PropertyChangedMessage<object> message)
    {
        switch (message.PropertyName)
        {
            case "SpellCheck":
                SpellCheck = (bool)message.NewValue;
                break;
            case "TransparentNotes":
            case "OpaqueWhenFocused":
            case "OnlyTransparentWhenPinned":
                UpdateOpacity();
                break;
            case "ColorMode":
                UpdateBrushes(CurrentThemeColor);
                break;
        }
    }

    public NoteViewModel(NoteViewModel? parent = null)
    {
        Messenger.Register(this);

        InitNoteColor(parent);
        InitNotePosition(parent);
    }

    private void InitNoteColor(NoteViewModel? parent = null)
    {
        // Set this first as cycle colors wont trigger a change if the next color if the default for ThemeColors
        CurrentThemeColor = (ThemeColors)Settings.Default.Color;
        if (Settings.Default.CycleColors)
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
        if (parent != null)
        {
            GravityX = parent.GravityX;
            GravityY = parent.GravityY;

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
            switch ((StartupPositions)Settings.Default.StartupPosition)
            {
                case StartupPositions.TopLeft:
                case StartupPositions.MiddleLeft:
                case StartupPositions.BottomLeft:
                    X = screenMargin;
                    GravityX = 1;
                    break;
                case StartupPositions.TopCenter:
                case StartupPositions.MiddleCenter:
                case StartupPositions.BottomCenter:
                    X = SystemParameters.PrimaryScreenWidth / 2 - Width / 2;
                    GravityX = 1;
                    break;
                case StartupPositions.TopRight:
                case StartupPositions.MiddleRight:
                case StartupPositions.BottomRight:
                    X = (SystemParameters.PrimaryScreenWidth - screenMargin) - Width;
                    GravityX = -1;
                    break;
            }

            switch ((StartupPositions)Settings.Default.StartupPosition)
            {
                case StartupPositions.TopLeft:
                case StartupPositions.TopCenter:
                case StartupPositions.TopRight:
                    Y = screenMargin;
                    GravityY = 1;
                    break;
                case StartupPositions.MiddleLeft:
                case StartupPositions.MiddleCenter:
                case StartupPositions.MiddleRight:
                    Y = SystemParameters.PrimaryScreenHeight / 2 - Height / 2;
                    GravityY = -1;
                    break;
                case StartupPositions.BottomLeft:
                case StartupPositions.BottomCenter:
                case StartupPositions.BottomRight:
                    Y = (SystemParameters.PrimaryScreenHeight - screenMargin) - Height;
                    GravityY = -1;
                    break;
            }
        }
    }

    private void UpdateBrushes(ThemeColors themeColor)
    {
        ColorModes colorMode = (ColorModes)Settings.Default.ColorMode;

        // 0 = Title, 1 = Background, 2 = Border
        BorderColorBrush = new(_colors[themeColor][2]);         // #feea00

        if (colorMode == ColorModes.Dark || (colorMode == ColorModes.System && SystemThemeHelper.IsDarkMode()))
        {
            TitleBarColorBrush = new(Color.FromRgb(70, 70, 70));    // #464646
            TitleButtonColorBrush = new(_colors[themeColor][2]);
            BackgroundColorBrush = new(Color.FromRgb(50, 50, 50));  // #323232
            TextColorBrush = new(Colors.White);
        }
        else
        {
            TitleBarColorBrush = new(_colors[themeColor][0]);        // #464646
            TitleButtonColorBrush = new(Color.FromRgb(70, 70, 70));
            BackgroundColorBrush = new(_colors[themeColor][1]);      // #323232
            TextColorBrush = new(Colors.Black);
        }
    }

    public void UpdateOpacity()
    {
        bool transparentNotes = Settings.Default.TransparentNotes;
        bool opaqueWhenFocused = Settings.Default.OpaqueWhenFocused;
        bool onlyTransparentWhenPinned = Settings.Default.OnlyTransparentWhenPinned;

        if (IsFocused)
            Opacity = (transparentNotes && !opaqueWhenFocused) ? _transparentOpacity : _opaqueOpacity;
        else if (IsPinned)
            Opacity = transparentNotes ? _transparentOpacity : _opaqueOpacity;
        else
            Opacity = (transparentNotes && !onlyTransparentWhenPinned) ? _transparentOpacity : _opaqueOpacity;
    }

    [ObservableProperty]
    private ThemeColors _currentThemeColor;
    partial void OnCurrentThemeColorChanged(ThemeColors value)
    {
        Settings.Default.Color = (int)value;
        Settings.Default.Save();
        UpdateBrushes(value);
    }

    [ObservableProperty]
    private SolidColorBrush _titleBarColorBrush = null!;
    [ObservableProperty]
    private SolidColorBrush _titleButtonColorBrush = null!;
    [ObservableProperty]
    private SolidColorBrush _backgroundColorBrush = null!;
    [ObservableProperty]
    private SolidColorBrush _borderColorBrush = null!;
    [ObservableProperty]
    private SolidColorBrush _textColorBrush = null!;

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
    private double _opacity = (Settings.Default.TransparentNotes && !Settings.Default.OnlyTransparentWhenPinned) ? 0.8 : 1.0;

    [ObservableProperty]
    private bool _spellCheck = Settings.Default.SpellCheck;

    [ObservableProperty]
    private bool _isPinned = false;
    [ObservableProperty]
    private bool _isFocused;
    [ObservableProperty]
    private bool _isSaved = false;

    [ObservableProperty]
    private string _content = "";

    [RelayCommand]
    private void ChangeThemeColor(ThemeColors themeColor) => CurrentThemeColor = themeColor;
}
