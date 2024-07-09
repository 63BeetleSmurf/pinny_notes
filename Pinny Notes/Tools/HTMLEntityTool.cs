using System.Net;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class HtmlEntityTool : BaseTool, ITool
{
    public HtmlEntityTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "HTML Entity",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Encode",
                Command = new CustomCommand() { ExecuteMethod = HtmlEntityEncodeAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = new CustomCommand() { ExecuteMethod = HtmlEntityDecodeAction }
            }
        );

    }

    private bool HtmlEntityEncodeAction()
    {
        ApplyFunctionToNoteText(HtmlEntityEncodeText);
        return true;
    }

    private bool HtmlEntityDecodeAction()
    {
        ApplyFunctionToNoteText(HtmlEntityDecodeText);
        return true;
    }

    private string HtmlEntityEncodeText(string text, string? additional = null)
    {
        return WebUtility.HtmlEncode(text);
    }
    private string HtmlEntityDecodeText(string text, string? additional = null)
    {
        return WebUtility.HtmlDecode(text);
    }
}
