using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class JoinTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Join",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Comma",
                Command = new RelayCommand(JoinCommaAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = new RelayCommand(JoinSpaceAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new RelayCommand(JoinTabAction)
            }
        );

        return menuItem;
    }

    private void JoinCommaAction()
    {
        ApplyFunctionToNoteText<string>(JoinText, ",");
    }

    private void JoinSpaceAction()
    {
        ApplyFunctionToNoteText<string>(JoinText, " ");
    }

    private void JoinTabAction()
    {
        ApplyFunctionToNoteText<string>(JoinText, "\t");
    }

    private string JoinText(string text, string? joinString)
    {
        return text.Replace(Environment.NewLine, joinString);
    }
}
