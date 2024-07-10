using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

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
                Command = new RelayCommand(Indent2SpacesAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "4 Spaces",
                Command = new RelayCommand(Indent4SpacesAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new RelayCommand(IndentTabAction)
            }
        );

        return menuItem;
    }
    private void Indent2SpacesAction()
    {
        ApplyFunctionToEachLine<string>(IndentText, "  ");
    }

    private void Indent4SpacesAction()
    {
        ApplyFunctionToEachLine<string>(IndentText, "    ");
    }

    private void IndentTabAction()
    {
        ApplyFunctionToEachLine<string>(IndentText, "\t");
    }

    private string? IndentText(string line, int index, string? indentString)
    {
        return indentString + line;
    }
}
