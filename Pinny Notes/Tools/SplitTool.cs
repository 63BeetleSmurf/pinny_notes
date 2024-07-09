using System;
using System.Windows.Controls;
using Pinny_Notes.Commands;

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
                Command = new CustomCommand() { ExecuteMethod = SplitCommaAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = new CustomCommand() { ExecuteMethod = SplitSpaceAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new CustomCommand() { ExecuteMethod = SplitTabAction }
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Selected",
                Command = new CustomCommand() { ExecuteMethod = SplitSelectedAction }
            }
        );

        return menuItem;
    }

    private bool SplitCommaAction()
    {
        ApplyFunctionToEachLine(SplitText, ",");
        return true;
    }

    private bool SplitSpaceAction()
    {
        ApplyFunctionToEachLine(SplitText, " ");
        return true;
    }

    private bool SplitTabAction()
    {
        ApplyFunctionToEachLine(SplitText, "\t");
        return true;
    }

    private bool SplitSelectedAction()
    {
        string splitString = _noteTextBox.SelectedText;
        _noteTextBox.SelectionLength = 0;
        ApplyFunctionToEachLine(SplitText, splitString);
        return true;
    }

    private string? SplitText(string line, int index, string? splitString)
    {
        if (splitString == null)
            return null;
        return line.Replace(splitString, Environment.NewLine);
    }
}
