using CommunityToolkit.Mvvm.Input;
using System.Net;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class HtmlEntityTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
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
                Command = new RelayCommand(HtmlEntityEncodeAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = new RelayCommand(HtmlEntityDecodeAction)
            }
        );

        return menuItem;
    }

    private void HtmlEntityEncodeAction()
    {
        ApplyFunctionToNoteText<bool?>(HtmlEntityEncodeText);
    }

    private void HtmlEntityDecodeAction()
    {
        ApplyFunctionToNoteText<bool?>(HtmlEntityDecodeText);
    }

    private string HtmlEntityEncodeText(string text, bool? additional = null)
    {
        return WebUtility.HtmlEncode(text);
    }
    private string HtmlEntityDecodeText(string text, bool? additional = null)
    {
        return WebUtility.HtmlDecode(text);
    }
}
