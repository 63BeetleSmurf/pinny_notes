using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public abstract class BaseTool(TextBox noteTextBox) : ITool
{
    public MenuItem MenuItem { get; init; } = null!;

    protected TextBox _noteTextBox = noteTextBox;

    protected void ApplyFunctionToNoteText(Func<string, string?, string> function, string? additional = null)
    {
        if (_noteTextBox.SelectionLength > 0)
            _noteTextBox.SelectedText = function(_noteTextBox.SelectedText, additional);
        else
        {
            string noteText = _noteTextBox.Text;
            // Ignore trailing new line if it was automatically added
            if (Properties.Settings.Default.NewLine && _noteTextBox.Text.EndsWith(Environment.NewLine))
                noteText = noteText.Remove(noteText.Length - Environment.NewLine.Length);
            _noteTextBox.Text = function(noteText, additional);
            if (_noteTextBox.Text.Length > 0)
                _noteTextBox.CaretIndex = _noteTextBox.Text.Length - 1;
        }
    }

    protected void ApplyFunctionToEachLine(Func<string, int, string?, string?> function, string? additional = null)
    {
        string[] lines;
        List<string> newLines = [];

        if (_noteTextBox.SelectionLength > 0)
            lines = _noteTextBox.SelectedText.Split(Environment.NewLine);
        else
            lines = _noteTextBox.Text.Split(Environment.NewLine);

        int lineCount = lines.Length;
        // Ignore trailing new line if it was automatically added
        if (Properties.Settings.Default.NewLine && lines[lineCount - 1] == "")
            lineCount--;
        for (int i = 0; i < lineCount; i++)
        {
            string? line = function(lines[i], i, additional);
            if (line != null)
                newLines.Add(line);
        }

        if (_noteTextBox.SelectionLength > 0)
            _noteTextBox.SelectedText = string.Join(Environment.NewLine, newLines);
        else
        {
            _noteTextBox.Text = string.Join(Environment.NewLine, newLines);
            if (_noteTextBox.Text.Length > 0)
                _noteTextBox.CaretIndex = _noteTextBox.Text.Length - 1;
        }
    }
}
