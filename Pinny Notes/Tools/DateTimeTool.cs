using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class DateTimeTool : BaseTool, ITool
{
    public enum ToolActions
    {
        DateTimeSortableDateTime
    }

    public DateTimeTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Date Time";
        _menuActions.Add(new("Sortable Date Time", MenuActionCommand, ToolActions.DateTimeSortableDateTime));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        switch (action)
        {
            case ToolActions.DateTimeSortableDateTime:
                InsertIntoNoteText(GetSortableDateTime());
                break;
        }
    }

    private string GetSortableDateTime()
    {
        string selectedText = _noteTextBox.SelectedText;
        return GetDateTime("s", selectedText);
    }

    private string GetDateTime(string format, string? dateString = null)
    {
        if (string.IsNullOrEmpty(dateString))
            return DateTime.UtcNow.ToString(format);

        DateTime parsedDateTime;
        if (DateTime.TryParse(dateString, out parsedDateTime))
            return parsedDateTime.ToString(format);

        return "";
    }
}
