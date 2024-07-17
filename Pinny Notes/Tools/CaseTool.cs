using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class CaseTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        CaseLower,
        CaseUpper,
        CaseTitle
    }

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
                Command = MenuActionCommand,
                CommandParameter = ToolActions.CaseLower
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Upper",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.CaseUpper
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Title",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.CaseTitle
            }
        );

        return menuItem;
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, ToolActions action)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        switch (action)
        {
            case ToolActions.CaseLower:
                return textInfo.ToLower(text);
            case ToolActions.CaseUpper:
                return textInfo.ToUpper(text);
            case ToolActions.CaseTitle:
                return textInfo.ToTitleCase(text);
        }

        return text;
    }
}
