using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

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
                Command = new RelayCommand(ListEnumerateAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Dash",
                Command = new RelayCommand(ListDashAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove",
                Command = new RelayCommand(ListRemoveAction)
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sort Asc.",
                Command = new RelayCommand(ListSortAscAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sort Dec.",
                Command = new RelayCommand(ListSortDecAction)
            }
        );

        return menuItem;
    }

    private void ListEnumerateAction()
    {
        ApplyFunctionToEachLine<bool?>(EnumerateLine);
    }

    private void ListDashAction()
    {
        ApplyFunctionToEachLine<bool?>(DashLine);
    }

    private void ListRemoveAction()
    {
        ApplyFunctionToEachLine<bool?>(RemoveFirstWordInLine);
    }

    private void ListSortAscAction()
    {
        ApplyFunctionToNoteText<bool>(SortNoteText);
    }

    private void ListSortDecAction()
    {
        ApplyFunctionToNoteText<bool>(SortNoteText, true);
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
