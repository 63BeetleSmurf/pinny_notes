using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class QuoteTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Quote",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Double",
                Command = new RelayCommand(QuoteDoubleAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Single",
                Command = new RelayCommand(QuoteSingleAction)
            }
        );

        return menuItem;
    }

    private void QuoteDoubleAction()
    {
        ApplyFunctionToEachLine<char>(QuoteText, '"');
    }

    private void QuoteSingleAction()
    {
        ApplyFunctionToEachLine<char>(QuoteText, '\'');
    }

    private string? QuoteText(string line, int index, char quoteChar)
    {
        return $"{quoteChar}{line}{quoteChar}";
    }
}
