using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class CaseTool : BaseTool, ITool
{
    public enum ToolActions
    {
        CaseLower,
        CaseUpper,
        CaseTitle
    }

    public CaseTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Case";
        _menuActions.Add(new("Lower", MenuActionCommand, ToolActions.CaseLower));
        _menuActions.Add(new("Upper", MenuActionCommand, ToolActions.CaseUpper));
        _menuActions.Add(new("Title", MenuActionCommand, ToolActions.CaseTitle));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, Enum action)
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
