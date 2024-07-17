using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;


public partial class TrimTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        TrimStart,
        TrimEnd,
        TrimBoth,
        TrimLines
    }

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Trim",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Start",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.TrimStart
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "End",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.TrimEnd
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Both",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.TrimBoth
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Empty Lines",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.TrimLines
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
            case ToolActions.TrimStart:
                return line.TrimStart();
            case ToolActions.TrimEnd:
                return line.TrimEnd();
            case ToolActions.TrimBoth:
                return line.Trim();
            case ToolActions.TrimLines:
                if (string.IsNullOrEmpty(line))
                    return null;
                else
                    return line;
        }

        return line;
    }
}
