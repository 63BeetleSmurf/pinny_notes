using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Media;
using Pinny_Notes.Models;
using Pinny_Notes.Themes;

namespace Pinny_Notes.ViewModels;

public class NoteViewModel : ObservableObject
{
    private NoteModel _note = new();

    public int Left
    {
        get { return _note.X; }
        set
        {
            if (_note.X != value)
            {
                _note.X = value;
                OnPropertyChanged(nameof(Left));
            }
        }
    }

    public int Top
    {
        get { return _note.Y; }
        set
        {
            if (_note.Y != value)
            {
                _note.Y = value;
                OnPropertyChanged(nameof(Top));
            }
        }
    }

    public int Width
    {
        get { return _note.Width; }
        set
        {
            if (_note.Width != value)
            {
                _note.Width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
    }

    public int Height
    {
        get { return _note.Height; }
        set
        {
            if (_note.Height != value)
            {
                _note.Height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
    }
    public Tuple<bool, bool> Gravity
    {
        get { return _note.Gravity; }
        set
        {
            if (_note.Gravity != value)
            {
                _note.Gravity = value;
                OnPropertyChanged(nameof(Gravity));
            }
        }
    }
    public NoteTheme Theme { get { return _note.Theme; } set { _note.Theme = value; } }

    public SolidColorBrush TitleBarColor { get { return _note.Theme.TitleBarColorBrush; } }

    public SolidColorBrush BackgroundColor { get { return _note.Theme.BackgroundColorBrush; } }

    public SolidColorBrush BorderColor { get { return _note.Theme.BorderColorBrush; } }

    public string Content
    {
        get { return _note.Content; }
        set
        {
            if (_note.Content != value)
            {
                _note.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
    }
}
