using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class ListTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        ListEnumerate,
        ListDash,
        ListRemove,
        ListSortAsc,
        ListSortDec
    }

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
                Command = MenuActionCommand,
                CommandParameter = ToolActions.ListEnumerate
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Dash",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.ListDash
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Remove",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.ListRemove
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sort Asc.",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.ListSortAsc
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sort Dec.",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.ListSortDec
            }
        );

        return menuItem;
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        if (action == ToolActions.ListSortAsc || action == ToolActions.ListSortDec)
            ApplyFunctionToNoteText(ModifyTextCallback, action);
        else
            ApplyFunctionToEachLine(ModifyLineCallback, action);
    }

    private string ModifyTextCallback(string text, ToolActions action)
    {
        switch (action)
        {
            case ToolActions.ListSortAsc:
                return SortNoteText(text);
            case ToolActions.ListSortDec:
                return SortNoteText(text, true);
        }

        return text;
    }

    private string? ModifyLineCallback(string line, int index, ToolActions action)
    {
        switch (action)
        {
            case ToolActions.ListEnumerate:
                return $"{index + 1}. {line}";
            case ToolActions.ListDash:
                return $"- {line}";
            case ToolActions.ListRemove:
                return line[(line.IndexOf(' ') + 1)..];
        }

        return line;
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
