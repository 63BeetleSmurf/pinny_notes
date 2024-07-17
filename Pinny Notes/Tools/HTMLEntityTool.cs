using CommunityToolkit.Mvvm.Input;
using System;
using System.Net;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class HtmlEntityTool : BaseTool, ITool
{
    public enum ToolActions
    {
        EntityEncode,
        EntityDecode
    }

    public HtmlEntityTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "HTML Entity";
        _menuActions.Add(new("Encode", MenuActionCommand, ToolActions.EntityEncode));
        _menuActions.Add(new("Decode", MenuActionCommand, ToolActions.EntityDecode));
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
            case ToolActions.EntityEncode:
                return WebUtility.HtmlEncode(text);
            case ToolActions.EntityDecode:
                return WebUtility.HtmlDecode(text);
        }

        return text;
    }
}
