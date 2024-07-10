using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows.Controls;

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
                Command = new RelayCommand(CaseLowerAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Upper",
                Command = new RelayCommand(CaseUpperAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Proper",
                Command = new RelayCommand(CaseProperAction)
            }
        );

        return menuItem;
    }

    private void CaseLowerAction()
    {
        ApplyFunctionToEachLine<Cases>(SetTextCase, Cases.Lower);
    }

    private void CaseUpperAction()
    {
        ApplyFunctionToEachLine<Cases>(SetTextCase, Cases.Upper);
    }

    private void CaseProperAction()
    {
        ApplyFunctionToEachLine<Cases>(SetTextCase, Cases.Proper);
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
