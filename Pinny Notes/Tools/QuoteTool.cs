using System.Windows.Controls;
using Pinny_Notes.Commands;

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
                Command = new CustomCommand() { ExecuteMethod = QuoteDoubleAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Single",
                Command = new CustomCommand() { ExecuteMethod = QuoteSingleAction }
            }
        );

        return menuItem;
    }

    private bool QuoteDoubleAction()
    {
        ApplyFunctionToEachLine<char>(QuoteText, '"');
        return true;
    }

    private bool QuoteSingleAction()
    {
        ApplyFunctionToEachLine<char>(QuoteText, '\'');
        return true;
    }

    private string? QuoteText(string line, int index, char quoteChar)
    {
        return $"{quoteChar}{line}{quoteChar}";
    }
}
