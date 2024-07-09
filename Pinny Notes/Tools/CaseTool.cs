using System.Globalization;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

enum Cases
{
    Lower = 0,
    Upper = 1,
    Proper = 2
}

public class CaseTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Case",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Lower",
                Command = new CustomCommand() { ExecuteMethod = CaseLowerAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Upper",
                Command = new CustomCommand() { ExecuteMethod = CaseUpperAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Proper",
                Command = new CustomCommand() { ExecuteMethod = CaseProperAction }
            }
        );

        return menuItem;
    }

    private bool CaseLowerAction()
    {
        ApplyFunctionToEachLine<Cases>(SetTextCase, Cases.Lower);
        return true;
    }

    private bool CaseUpperAction()
    {
        ApplyFunctionToEachLine<Cases>(SetTextCase, Cases.Upper);
        return true;
    }

    private bool CaseProperAction()
    {
        ApplyFunctionToEachLine<Cases>(SetTextCase, Cases.Proper);
        return true;
    }

    private string? SetTextCase(string line, int index, Cases textCase)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textCase switch
        {
            Cases.Upper => textInfo.ToUpper(line),
            Cases.Proper => textInfo.ToTitleCase(textInfo.ToLower(line)),
            _ => textInfo.ToLower(line),
        };
    }
}
