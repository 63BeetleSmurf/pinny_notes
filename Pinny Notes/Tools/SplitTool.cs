using System;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class SplitTool : BaseTool, ITool
{
    public SplitTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "Split",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Comma",
                Command = new CustomCommand() { ExecuteMethod = SplitCommaAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = new CustomCommand() { ExecuteMethod = SplitSpaceAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = new CustomCommand() { ExecuteMethod = SplitTabAction }
            }
        );
        MenuItem.Items.Add(new Separator());
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Selected",
                Command = new CustomCommand() { ExecuteMethod = SplitSelectedAction }
            }
        );

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
