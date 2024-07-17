using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

// TO DO: Need to fix split by selected text. Possibly TextAction needs to get both text and selected text.

public partial class SplitTool : BaseTool, ITool
{
    public enum ToolActions
    {
        SplitComma,
        SplitSpace,
        SplitTab,
        SplitSelected
    }

    public SplitTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Split";
        _menuActions.Add(new("Comma", MenuActionCommand, ToolActions.SplitComma));
        _menuActions.Add(new("Space", MenuActionCommand, ToolActions.SplitSpace));
        _menuActions.Add(new("Tab", MenuActionCommand, ToolActions.SplitTab));
        //_menuActions.Add(new("-"));
        //_menuActions.Add(new("Selected", MenuActionCommand, ToolActions.SplitSelected));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, Enum action)
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
