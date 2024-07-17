using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class RemoveTool : BaseTool, ITool
{
    public enum ToolActions
    {
        RemoveSpaces,
        RemoveTabs,
        RemoveNewLines
    }

    public RemoveTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Remove";
        _menuActions.Add(new("Spaces", MenuActionCommand, ToolActions.RemoveSpaces));
        _menuActions.Add(new("Tabs", MenuActionCommand, ToolActions.RemoveTabs));
        _menuActions.Add(new("New Lines", MenuActionCommand, ToolActions.RemoveNewLines));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
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
}
