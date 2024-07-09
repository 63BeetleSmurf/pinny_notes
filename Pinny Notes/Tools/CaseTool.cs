using System.Globalization;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

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
        ApplyFunctionToEachLine(SetTextCase, "l");
        return true;
    }

    private bool CaseUpperAction()
    {
        ApplyFunctionToEachLine(SetTextCase, "u");
        return true;
    }

    private bool CaseProperAction()
    {
        ApplyFunctionToEachLine(SetTextCase, "p");
        return true;
    }

    private string? SetTextCase(string line, int index, string? textCase)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textCase switch
        {
            "u" => textInfo.ToUpper(line),
            "p" => textInfo.ToTitleCase(textInfo.ToLower(line)),
            _ => textInfo.ToLower(line),
        };
    }
}
