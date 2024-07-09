using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class QuoteTool : BaseTool, ITool
{
    public QuoteTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "Quote",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Double",
                Command = new CustomCommand() { ExecuteMethod = QuoteDoubleAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Single",
                Command = new CustomCommand() { ExecuteMethod = QuoteSingleAction }
            }
        );

    }

    private bool QuoteDoubleAction()
    {
        ApplyFunctionToEachLine(QuoteText, "\"");
        return true;
    }

    private bool QuoteSingleAction()
    {
        ApplyFunctionToEachLine(QuoteText, "'");
        return true;
    }

    private string? QuoteText(string line, int index, string? additional)
    {
        return additional + line + additional;
    }
}
