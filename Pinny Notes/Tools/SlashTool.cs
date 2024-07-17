using CommunityToolkit.Mvvm.Input;
using System;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class SlashTool : BaseTool, ITool
{
    public enum ToolActions
    {
        SlashAllForward,
        SlashAllBack,
        SlashSwap
    }

    public SlashTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Slash";
        _menuActions.Add(new("All Forward (/)", MenuActionCommand, ToolActions.SlashAllForward));
        _menuActions.Add(new("All Back (\\)", MenuActionCommand, ToolActions.SlashAllBack));
        _menuActions.Add(new("Swap", MenuActionCommand, ToolActions.SlashSwap));
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
}
