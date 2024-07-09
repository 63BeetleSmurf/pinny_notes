using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class Base64Tool : BaseTool, ITool
{
    public Base64Tool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "Base64",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Encode",
                Command = new CustomCommand() { ExecuteMethod = Base64EncodeAction }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = new CustomCommand() { ExecuteMethod = Base64DecodeAction }
            }
        );
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
