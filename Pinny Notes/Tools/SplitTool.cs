using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

// TO DO: Need to fix split by selected text. Possibly TextAction needs to get both text and selected text.

public partial class SplitTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        SplitComma,
        SplitSpace,
        SplitTab,
        SplitSelected
    }

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Split",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Comma",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SplitComma
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Space",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SplitSpace
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Tab",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.SplitTab
            }
        );

        //menuItem.Items.Add(new Separator());

        //menuItem.Items.Add(
        //    new MenuItem()
        //    {
        //        Header = "Selected",
        //        Command = MenuActionCommand,
        //        CommandParameter = ToolActions.SplitSelected
        //    }
        //);

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
            case ToolActions.SplitComma:
                return text.Replace(",", Environment.NewLine);
            case ToolActions.SplitSpace:
                return text.Replace(" ", Environment.NewLine);
            case ToolActions.SplitTab:
                return text.Replace("\t", Environment.NewLine);
                //case ToolActions.SplitSelected:
                //    return text.Replace(splitString, Environment.NewLine);
        }

        return text;
    }
}
