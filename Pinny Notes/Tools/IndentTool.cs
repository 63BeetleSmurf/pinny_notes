using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class IndentTool : BaseTool, ITool
{
    public IndentTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "Indent",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "2 Spaces",
                Command = new CustomCommand() { ExecuteMethod = Indent2SpacesAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "4 Spaces",
                Command = new CustomCommand() { ExecuteMethod = Indent4SpacesAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new CustomCommand() { ExecuteMethod = IndentTabAction }
            }
        );
    }
    private bool Indent2SpacesAction()
    {
        ApplyFunctionToEachLine(IndentText, "  ");
        return true;
    }

    private bool Indent4SpacesAction()
    {
        ApplyFunctionToEachLine(IndentText, "    ");
        return true;
    }

    private bool IndentTabAction()
    {
        ApplyFunctionToEachLine(IndentText, "\t");
        return true;
    }

    private string? IndentText(string line, int index, string? indentString)
    {
        return indentString + line;
    }
}
