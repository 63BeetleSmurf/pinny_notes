using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class JoinTool : BaseTool, ITool
{
    public enum ToolActions
    {
        JoinComma,
        JoinSpace,
        JoinTab
    }

    public JoinTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Join";
        _menuActions.Add(new("Comma", MenuActionCommand, ToolActions.JoinComma));
        _menuActions.Add(new("Space", MenuActionCommand, ToolActions.JoinSpace));
        _menuActions.Add(new("Tab", MenuActionCommand, ToolActions.JoinTab));
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
