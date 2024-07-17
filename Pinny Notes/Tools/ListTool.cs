using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class ListTool : BaseTool, ITool
{
    public enum ToolActions
    {
        ListEnumerate,
        ListDash,
        ListRemove,
        ListSortAsc,
        ListSortDec
    }

    public ListTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "List";
        _menuActions.Add(new("Enumerate", MenuActionCommand, ToolActions.ListEnumerate));
        _menuActions.Add(new("Dash", MenuActionCommand, ToolActions.ListDash));
        _menuActions.Add(new("Remove", MenuActionCommand, ToolActions.ListRemove));
        _menuActions.Add(new("-"));
        _menuActions.Add(new("Sort Asc.", MenuActionCommand, ToolActions.ListSortAsc));
        _menuActions.Add(new("Sort Dec.", MenuActionCommand, ToolActions.ListSortDec));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        if (action == ToolActions.ListSortAsc || action == ToolActions.ListSortDec)
            ApplyFunctionToNoteText(ModifyTextCallback, action);
        else
            ApplyFunctionToEachLine(ModifyLineCallback, action);
    }

    private string ModifyTextCallback(string text, Enum action)
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

    private string? ModifyLineCallback(string line, int index, Enum action)
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
