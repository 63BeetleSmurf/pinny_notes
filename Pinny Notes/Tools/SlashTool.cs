using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class SlashTool : BaseTool, ITool
{
    public enum ToolActions
    {
        SlashAllForward,
        SlashAllBack,
        SlashSwap,
        SlashRemoveForward,
        SlashRemoveBack,
        SlashRemoveAll
    }

    public SlashTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Slash";
        _menuActions.Add(new("All Forward (/)", MenuActionCommand, ToolActions.SlashAllForward));
        _menuActions.Add(new("All Back (\\)", MenuActionCommand, ToolActions.SlashAllBack));
        _menuActions.Add(new("Swap", MenuActionCommand, ToolActions.SlashSwap));
        _menuActions.Add(new("-"));
        _menuActions.Add(new("Remove Forward (/)", MenuActionCommand, ToolActions.SlashRemoveForward));
        _menuActions.Add(new("Remove Back (\\)", MenuActionCommand, ToolActions.SlashRemoveBack));
        _menuActions.Add(new("Remove All", MenuActionCommand, ToolActions.SlashRemoveAll));
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
            case ToolActions.SlashAllForward:
                return text.Replace('\\', '/');
            case ToolActions.SlashAllBack:
                return text.Replace('/', '\\');
            case ToolActions.SlashSwap:
                return SwapCharacters(text, '\\', '/');
            case ToolActions.SlashRemoveForward:
                return RemoveCharacters(text, ['/']);
            case ToolActions.SlashRemoveBack:
                return RemoveCharacters(text, ['\\']);
            case ToolActions.SlashRemoveAll:
                return RemoveCharacters(text, ['\\', '/']);
        }

        return text;
    }

    private string SwapCharacters(string text, char character1, char? character2 = null)
    {
        StringBuilder stringBuilder = new(text.Length);

        foreach (char currentChar in text)
        {
            if (currentChar == character1)
                stringBuilder.Append(character2);
            else if (currentChar == character2)
                stringBuilder.Append(character1);
            else
                stringBuilder.Append(currentChar);
        }

        return stringBuilder.ToString();
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
