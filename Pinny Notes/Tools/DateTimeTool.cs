using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class DateTimeTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Date Time",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sortable Date Time",
                Command = new RelayCommand(DateSortableDateTimeAction)
            }
        );

        return menuItem;
    }

    private void DateSortableDateTimeAction()
    {
        string selectedText = _noteTextBox.SelectedText;
        InsertIntoNoteText(GetDateTime("s", selectedText));
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
