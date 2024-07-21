using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class RemoveTool : BaseTool, ITool
{
    private string? _selectedText = null;

    public enum ToolActions
    {
        RemoveSpaces,
        RemoveTabs,
        RemoveNewLines,
        RemoveForwardSlashes,
        RemoveBackSlashes,
        RemoveAllSlashes,
        RemoveSelected
    }

    public RemoveTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Remove";
        _menuActions.Add(new("Spaces", MenuActionCommand, ToolActions.RemoveSpaces));
        _menuActions.Add(new("Tabs", MenuActionCommand, ToolActions.RemoveTabs));
        _menuActions.Add(new("New Lines", MenuActionCommand, ToolActions.RemoveNewLines));
        _menuActions.Add(new("-"));
        _menuActions.Add(new("Forward Slashes (/)", MenuActionCommand, ToolActions.RemoveForwardSlashes));
        _menuActions.Add(new("Back Slashes (\\)", MenuActionCommand, ToolActions.RemoveBackSlashes));
        _menuActions.Add(new("All Slashes", MenuActionCommand, ToolActions.RemoveAllSlashes));
        _menuActions.Add(new("-"));
        _menuActions.Add(new("Selected", MenuActionCommand, ToolActions.RemoveSelected));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        if (action != ToolActions.RemoveSelected)
        {
            _selectedText = null;
        }
        else
        {
            _selectedText = _noteTextBox.SelectedText;
            _noteTextBox.SelectionLength = 0;
        }

        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, Enum action)
    {
        switch (action)
        {
            case ToolActions.RemoveSpaces:
                return RemoveCharacters(text, [' ']);
            case ToolActions.RemoveTabs:
                return RemoveCharacters(text, ['\t']);
            case ToolActions.RemoveNewLines:
                return RemoveCharacters(text, ['\r', '\n']);
            case ToolActions.RemoveForwardSlashes:
                return RemoveCharacters(text, ['/']);
            case ToolActions.RemoveBackSlashes:
                return RemoveCharacters(text, ['\\']);
            case ToolActions.RemoveAllSlashes:
                return RemoveCharacters(text, ['\\', '/']);
            case ToolActions.RemoveSelected:
                if (!string.IsNullOrEmpty(_selectedText))
                    return RemoveString(text, _selectedText);
                break;
        }

        return text;
    }

    private string RemoveCharacters(string text, HashSet<char> character)
    {
        StringBuilder stringBuilder = new(text.Length);

        foreach (char currentChar in text)
        {
            if (!character.Contains(currentChar))
                stringBuilder.Append(currentChar);
        }

        return stringBuilder.ToString();
    }

    private string RemoveString(string text, string toRemove)
    {
        return text.Replace(toRemove, "");
    }
}
