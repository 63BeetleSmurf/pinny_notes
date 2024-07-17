using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class TrimTool : BaseTool, ITool
{
    public enum ToolActions
    {
        TrimStart,
        TrimEnd,
        TrimBoth,
        TrimLines
    }

    public TrimTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Trim";
        _menuActions.Add(new("Start", MenuActionCommand, ToolActions.TrimStart));
        _menuActions.Add(new("End", MenuActionCommand, ToolActions.TrimEnd));
        _menuActions.Add(new("Both", MenuActionCommand, ToolActions.TrimBoth));
        _menuActions.Add(new("Empty Lines", MenuActionCommand, ToolActions.TrimLines));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToEachLine(ModifyLineCallback, action);
    }

    private string? ModifyLineCallback(string line, int index, Enum action)
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
