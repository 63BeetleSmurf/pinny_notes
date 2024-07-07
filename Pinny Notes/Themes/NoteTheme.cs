using System.Windows.Media;

namespace Pinny_Notes.Themes;

public class NoteTheme(string name, Color titleBarColor, Color backgroundColor, Color borderColor)
{
    public string Name { get; set; } = name;
    public Color TitleBarColor { get; set; } = titleBarColor;
    public Color BackgroundColor { get; set; } = backgroundColor;
    public Color BorderColor { get; set; } = borderColor;
}
