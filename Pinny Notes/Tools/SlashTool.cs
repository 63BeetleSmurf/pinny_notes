using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

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
                Command = new RelayCommand(SlashAllForwardAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "All Back (\\)",
                Command = new RelayCommand(SlashAllBackAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Swap",
                Command = new RelayCommand(SlashSwapAction)
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove Forward (/)",
                Command = new RelayCommand(SlashRemoveForwardAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove Back (\\)",
                Command = new RelayCommand(SlashRemoveBackAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove All",
                Command = new RelayCommand(SlashRemoveAllAction)
            }
        );

        return menuItem;
    }

    private void SlashAllForwardAction()
    {
        ApplyFunctionToNoteText<Tuple<string, string>>(ReplaceCharacters, new("\\", "/"));
    }

    private void SlashAllBackAction()
    {
        ApplyFunctionToNoteText<Tuple<string, string>>(ReplaceCharacters, new("/", "\\"));
    }

    private void SlashSwapAction()
    {
        ApplyFunctionToNoteText<Tuple<string, string>>(SwapCharacters, new("\\", "/"));
    }

    private void SlashRemoveForwardAction()
    {
        ApplyFunctionToNoteText<string[]>(RemoveCharacters, ["/"]);
    }

    private void SlashRemoveBackAction()
    {
        ApplyFunctionToNoteText<string[]>(RemoveCharacters, ["\\"]);
    }

    private void SlashRemoveAllAction()
    {
        ApplyFunctionToNoteText<string[]>(RemoveCharacters, ["\\", "/"]);
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
