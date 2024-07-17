using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class IndentTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        Indent2Spaces,
        Indent4Spaces,
        IndentTab
    }

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Indent",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "2 Spaces",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.Indent2Spaces
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "4 Spaces",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.Indent4Spaces
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.IndentTab
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
            case ToolActions.Indent2Spaces:
                return $"  {line}";
            case ToolActions.Indent4Spaces:
                return $"    {line}";
            case ToolActions.IndentTab:
                return $"\t{line}";
        }

        return line;
    }
}
