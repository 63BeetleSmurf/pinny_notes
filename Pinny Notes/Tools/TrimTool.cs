using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

enum Trims
{
    Start = 0,
    End = 1,
    Both = 2,
    Lines = 3
}

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
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.Start);
        return true;
    }

    private bool TrimEndAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.End);
        return true;
    }

    private bool TrimBothAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.Both);
        return true;
    }

    private bool TrimEmptyLinesAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.Lines);
        return true;
    }

    private string? TrimText(string line, int index, Trims trimType)
    {
        switch (trimType)
        {
            case Trims.Start:
                return line.TrimStart();
            case Trims.End:
                return line.TrimEnd();
            case Trims.Both:
                return line.Trim();
            case Trims.Lines:
                if (string.IsNullOrEmpty(line))
                    return null;
                else
                    return line;
            default:
                return line;

        }
    }
}
