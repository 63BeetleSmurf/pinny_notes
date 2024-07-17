using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class QuoteTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        QuoteDouble,
        QuoteSingle
    }

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
                Command = MenuActionCommand,
                CommandParameter = ToolActions.QuoteDouble
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Single",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.QuoteSingle
            }
        );

        return menuItem;
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToEachLine(ModifyLineCallback, action);
    }

    private string? ModifyLineCallback(string line, int index, ToolActions action)
    {
        switch (action)
        {
            case ToolActions.QuoteDouble:
                return $"\"{line}\"";
            case ToolActions.QuoteSingle:
                return $"'{line}'";
        }

        return line;
    }
}
