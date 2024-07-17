using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class HtmlEntityTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        EntityEncode,
        EntityDecode
    }

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "HTML Entity",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Encode",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.EntityEncode
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.EntityDecode
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
            case ToolActions.EntityEncode:
                return WebUtility.HtmlEncode(text);
            case ToolActions.EntityDecode:
                return WebUtility.HtmlDecode(text);
        }

        return text;
    }
}
