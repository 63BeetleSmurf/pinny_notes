using System;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class ListTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "List",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Enumerate",
                Command = new CustomCommand() { ExecuteMethod = ListEnumerateAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Dash",
                Command = new CustomCommand() { ExecuteMethod = ListDashAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove",
                Command = new CustomCommand() { ExecuteMethod = ListRemoveAction }
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sort Asc.",
                Command = new CustomCommand() { ExecuteMethod = ListSortAscAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sort Dec.",
                Command = new CustomCommand() { ExecuteMethod = ListSortDecAction }
            }
        );

        return menuItem;
    }

    private bool ListEnumerateAction()
    {
        ApplyFunctionToEachLine(EnumerateLine);
        return true;
    }

    private bool ListDashAction()
    {
        ApplyFunctionToEachLine(DashLine);
        return true;
    }

    private bool ListRemoveAction()
    {
        ApplyFunctionToEachLine(RemoveFirstWordInLine);
        return true;
    }

    private bool ListSortAscAction()
    {
        ApplyFunctionToNoteText(SortNoteText);
        return true;
    }

    private bool ListSortDecAction()
    {
        ApplyFunctionToNoteText(SortNoteText, "rev");
        return true;
    }

    private string EnumerateLine(string line, int index, string? additional)
    {
        return (index + 1).ToString() + ". " + line;
    }

    private string DashLine(string line, int index, string? additional)
    {
        return "- " + line;
    }

    private string RemoveFirstWordInLine(string line, int index, string? additional)
    {
        return line[(line.IndexOf(' ') + 1)..];
    }

    private string SortNoteText(string text, string? reverse = null)
    {
        string[] lines = text.Split(Environment.NewLine);
        Array.Sort(lines);
        if (reverse == "rev")
            Array.Reverse(lines);
        return string.Join(Environment.NewLine, lines);
    }
}
