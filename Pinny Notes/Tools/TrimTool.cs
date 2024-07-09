using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class TrimTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Trim",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Start",
                Command = new CustomCommand() { ExecuteMethod = TrimStartAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "End",
                Command = new CustomCommand() { ExecuteMethod = TrimEndAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Both",
                Command = new CustomCommand() { ExecuteMethod = TrimBothAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Empty Lines",
                Command = new CustomCommand() { ExecuteMethod = TrimEmptyLinesAction }
            }
        );

        return menuItem;
    }

    private bool TrimStartAction()
    {
        ApplyFunctionToEachLine(TrimText, "Start");
        return true;
    }

    private bool TrimEndAction()
    {
        ApplyFunctionToEachLine(TrimText, "End");
        return true;
    }

    private bool TrimBothAction()
    {
        ApplyFunctionToEachLine(TrimText, "Both");
        return true;
    }

    private bool TrimEmptyLinesAction()
    {
        ApplyFunctionToEachLine(TrimText, "Lines");
        return true;
    }

    private string? TrimText(string line, int index, string? trimType)
    {
        switch (trimType)
        {
            case "Start":
                return line.TrimStart();
            case "End":
                return line.TrimEnd();
            case "Both":
                return line.Trim();
            case "Lines":
                if (string.IsNullOrEmpty(line))
                    return null;
                else
                    return line;
            default:
                return line;

        }
    }
}
