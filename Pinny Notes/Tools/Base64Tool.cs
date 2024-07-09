using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class Base64Tool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Base64",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Encode",
                Command = new CustomCommand() { ExecuteMethod = Base64EncodeAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = new CustomCommand() { ExecuteMethod = Base64DecodeAction }
            }
        );

        return menuItem;
    }

    private bool Base64EncodeAction()
    {
        ApplyFunctionToNoteText(Base64EncodeText);
        return true;
    }

    private bool Base64DecodeAction()
    {
        ApplyFunctionToNoteText(Base64DecodeText);
        return true;
    }

    private string Base64EncodeText(string text, string? additional = null)
    {
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        return System.Convert.ToBase64String(textBytes);
    }

    private string Base64DecodeText(string text, string? additional = null)
    {
        byte[] base64Bytes = System.Convert.FromBase64String(text);
        return System.Text.Encoding.UTF8.GetString(base64Bytes);
    }
}
