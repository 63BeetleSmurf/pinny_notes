using System;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class JoinTool : BaseTool, ITool
{
    public JoinTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "Join",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Comma",
                Command = new CustomCommand() { ExecuteMethod = JoinCommaAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = new CustomCommand() { ExecuteMethod = JoinSpaceAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new CustomCommand() { ExecuteMethod = JoinTabAction }
            }
        );
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
