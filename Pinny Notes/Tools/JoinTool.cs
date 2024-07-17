using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class JoinTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        JoinComma,
        JoinSpace,
        JoinTab
    }

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Join",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Comma",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.JoinComma
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.JoinComma
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.JoinComma
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
        switch (action)
        {
            case ToolActions.JoinComma:
                return text.Replace(Environment.NewLine, ",");
            case ToolActions.JoinSpace:
                return text.Replace(Environment.NewLine, " ");
            case ToolActions.JoinTab:
                return text.Replace(Environment.NewLine, "\t");
        }

        return text;
    }
}
