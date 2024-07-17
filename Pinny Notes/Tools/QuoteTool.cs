using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class QuoteTool : BaseTool, ITool
{
    public enum ToolActions
    {
        QuoteDouble,
        QuoteSingle
    }

    public QuoteTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Quote";
        _menuActions.Add(new("Double", MenuActionCommand, ToolActions.QuoteDouble));
        _menuActions.Add(new("Single", MenuActionCommand, ToolActions.QuoteSingle));
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
            case ToolActions.QuoteDouble:
                return $"\"{line}\"";
            case ToolActions.QuoteSingle:
                return $"'{line}'";
        }

        return line;
    }
}
