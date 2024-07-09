using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class IndentTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Indent",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "2 Spaces",
                Command = new CustomCommand() { ExecuteMethod = Indent2SpacesAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "4 Spaces",
                Command = new CustomCommand() { ExecuteMethod = Indent4SpacesAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new CustomCommand() { ExecuteMethod = IndentTabAction }
            }
        );

        return menuItem;
    }
    private bool Indent2SpacesAction()
    {
        ApplyFunctionToEachLine<string>(IndentText, "  ");
        return true;
    }

    private bool Indent4SpacesAction()
    {
        ApplyFunctionToEachLine<string>(IndentText, "    ");
        return true;
    }

    private bool IndentTabAction()
    {
        ApplyFunctionToEachLine<string>(IndentText, "\t");
        return true;
    }

    private string? IndentText(string line, int index, string? indentString)
    {
        return indentString + line;
    }
}
