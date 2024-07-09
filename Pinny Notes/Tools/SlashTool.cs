using System;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class SlashTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
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
                Command = new CustomCommand() { ExecuteMethod = SlashAllForwardAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "All Back (\\)",
                Command = new CustomCommand() { ExecuteMethod = SlashAllBackAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Swap",
                Command = new CustomCommand() { ExecuteMethod = SlashSwapAction }
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove Forward (/)",
                Command = new CustomCommand() { ExecuteMethod = SlashRemoveForwardAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove Back (\\)",
                Command = new CustomCommand() { ExecuteMethod = SlashRemoveBackAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove All",
                Command = new CustomCommand() { ExecuteMethod = SlashRemoveAllAction }
            }
        );

        return menuItem;
    }

    private bool SlashAllForwardAction()
    {
        ApplyFunctionToNoteText<Tuple<string, string>>(ReplaceCharacters, new("\\", "/"));
        return true;
    }

    private bool SlashAllBackAction()
    {
        ApplyFunctionToNoteText<Tuple<string, string>>(ReplaceCharacters, new("/", "\\"));
        return true;
    }

    private bool SlashSwapAction()
    {
        ApplyFunctionToNoteText<Tuple<string, string>>(SwapCharacters, new("\\", "/"));
        return true;
    }
    private bool SlashRemoveForwardAction()
    {
        ApplyFunctionToNoteText<string[]>(RemoveCharacters, ["/"]);
        return true;
    }

    private bool SlashRemoveBackAction()
    {
        ApplyFunctionToNoteText<string[]>(RemoveCharacters, ["\\"]);
        return true;
    }

    private bool SlashRemoveAllAction()
    {
        ApplyFunctionToNoteText<string[]>(RemoveCharacters, ["\\", "/"]);
        return true;
    }

    private string ReplaceCharacters(string text, Tuple<string, string>? characters)
    {
        if (characters == null)
            return text;

        return text.Replace(characters.Item1, characters.Item2);
    }

    private string SwapCharacters(string text, Tuple<string, string>? characters)
    {
        string placeholder = "#|_PHOLD_#|";

        if (characters == null)
            return text;

        string newText = text.Replace(characters.Item1, placeholder);
        newText = newText.Replace(characters.Item2, characters.Item1);
        return newText.Replace(placeholder, characters.Item2);
    }

    private string RemoveCharacters(string text, string[]? characters)
    {
        if (characters == null)
            return text;

        string newText = text;
        foreach (string character in characters)
            newText = newText.Replace(character, "");

        return newText;
    }
}
