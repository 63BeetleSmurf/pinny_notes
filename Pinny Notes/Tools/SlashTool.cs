using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class SlashTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
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

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Slash",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "All Forward (/)",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SlashAllForward
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "All Back (\\)",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SlashAllBack
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Swap",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SlashSwap
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove Forward (/)",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SlashRemoveForward
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove Back (\\)",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SlashRemoveBack
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove All",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SlashRemoveAll
            }
        );

        return menuItem;
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, ToolActions action)
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
