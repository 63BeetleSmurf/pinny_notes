using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

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
                Command = new RelayCommand(TrimStartAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "End",
                Command = new RelayCommand(TrimEndAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Both",
                Command = new RelayCommand(TrimBothAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Empty Lines",
                Command = new RelayCommand(TrimEmptyLinesAction)
            }
        );

        return menuItem;
    }

    private void TrimStartAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.Start);
    }

    private void TrimEndAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.End);
    }

    private void TrimBothAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.Both);
    }

    private void TrimEmptyLinesAction()
    {
        ApplyFunctionToEachLine<Trims>(TrimText, Trims.Lines);
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
