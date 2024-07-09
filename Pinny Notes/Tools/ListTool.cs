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
        ApplyFunctionToEachLine<bool?>(EnumerateLine);
        return true;
    }

    private bool ListDashAction()
    {
        ApplyFunctionToEachLine<bool?>(DashLine);
        return true;
    }

    private bool ListRemoveAction()
    {
        ApplyFunctionToEachLine<bool?>(RemoveFirstWordInLine);
        return true;
    }

    private bool ListSortAscAction()
    {
        ApplyFunctionToNoteText<bool>(SortNoteText);
        return true;
    }

    private bool ListSortDecAction()
    {
        ApplyFunctionToNoteText<bool>(SortNoteText, true);
        return true;
    }

    private string EnumerateLine(string line, int index, bool? additional = null)
    {
        return $"{index + 1}. {line}";
    }

    private string DashLine(string line, int index, bool? additional = null)
    {
        return $"- {line}";
    }

    private string RemoveFirstWordInLine(string line, int index, bool? additional = null)
    {
        return line[(line.IndexOf(' ') + 1)..];
    }

    private string SortNoteText(string text, bool reverse = false)
    {
        string[] lines = text.Split(Environment.NewLine);
        Array.Sort(lines);
        if (reverse)
            Array.Reverse(lines);
        return string.Join(Environment.NewLine, lines);
    }
}
