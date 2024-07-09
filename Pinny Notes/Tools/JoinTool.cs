using System;
using System.Windows.Controls;
using Pinny_Notes.Commands;

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
                Command = new CustomCommand() { ExecuteMethod = JoinCommaAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = new CustomCommand() { ExecuteMethod = JoinSpaceAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new CustomCommand() { ExecuteMethod = JoinTabAction }
            }
        );

        return menuItem;
    }

    private bool JoinCommaAction()
    {
        ApplyFunctionToNoteText(JoinText, ",");
        return true;
    }

    private bool JoinSpaceAction()
    {
        ApplyFunctionToNoteText(JoinText, " ");
        return true;
    }

    private bool JoinTabAction()
    {
        ApplyFunctionToNoteText(JoinText, "\t");
        return true;
    }

    private string JoinText(string text, string? joinString)
    {
        return text.Replace(Environment.NewLine, joinString);
    }
}
