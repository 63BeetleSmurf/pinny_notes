using System.Windows.Media;
using Pinny_Notes.Enums;

namespace Pinny_Notes.Themes;

public class NoteTheme(Color titleBarColor, Color backgroundColor, Color borderColor)
{
    private Color _titleBarColor = titleBarColor;
    private Color _backgroundColor = backgroundColor;
    private Color _borderColor = borderColor;

    private SolidColorBrush _titleBarColorBrush = new(titleBarColor);
    private SolidColorBrush _backgroundColorBrush = new(backgroundColor);
    private SolidColorBrush _borderColorBrush = new(borderColor);

    public Color TitleBarColor
    {
        get { return _titleBarColor; }
        set
        {
            _titleBarColor = value;
            _titleBarColorBrush = new(value);
        }
    }

    public Color BackgroundColor
    {
        get { return _backgroundColor; }
        set
        {
            _backgroundColor = value;
            _backgroundColorBrush = new(value);
        }
    }

    public Color BorderColor
    {
        get { return _borderColor; }
        set
        {
            _borderColor = value;
            _borderColorBrush = new(value);
        }
    }

    public SolidColorBrush TitleBarColorBrush { get { return _titleBarColorBrush; } }
    public SolidColorBrush BackgroundColorBrush { get { return _backgroundColorBrush; } }
    public SolidColorBrush BorderColorBrush { get { return _borderColorBrush; } }
}
