using System;
using Pinny_Notes.Themes;

namespace Pinny_Notes.Models;

public class NoteModel
{
    public NoteTheme Theme { get; set; }
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 300;

    public Tuple<bool, bool> Gravity { get; set; } = new(true, true);
    public bool IsSaved { get; set; }

    public string Content { get; set; } = "";
}
