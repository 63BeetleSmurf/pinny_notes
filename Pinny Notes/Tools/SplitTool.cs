using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class SplitTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Split",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Comma",
                Command = new RelayCommand(SplitCommaAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = new RelayCommand(SplitSpaceAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new RelayCommand(SplitTabAction)
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Selected",
                Command = new RelayCommand(SplitSelectedAction)
            }
        );

        return menuItem;
    }

    private void SplitCommaAction()
    {
        ApplyFunctionToEachLine<string>(SplitText, ",");
    }

    private void SplitSpaceAction()
    {
        ApplyFunctionToEachLine<string>(SplitText, " ");
    }

    private void SplitTabAction()
    {
        ApplyFunctionToEachLine<string>(SplitText, "\t");
    }

    private void SplitSelectedAction()
    {
        string splitString = _noteTextBox.SelectedText;
        _noteTextBox.SelectionLength = 0;
        ApplyFunctionToEachLine(SplitText, splitString);
    }

    private string? SplitText(string line, int index, string? splitString)
    {
        if (splitString == null)
            return null;
        return line.Replace(splitString, Environment.NewLine);
    }
}
