using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

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
                Command = new RelayCommand(Base64EncodeAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = new RelayCommand(Base64DecodeAction)
            }
        );

        return menuItem;
    }

    private void Base64EncodeAction()
    {
        ApplyFunctionToNoteText<bool?>(Base64EncodeText);
    }

    private void Base64DecodeAction()
    {
        ApplyFunctionToNoteText<bool?>(Base64DecodeText);
    }

    private string Base64EncodeText(string text, bool? additional = null)
    {
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        return System.Convert.ToBase64String(textBytes);
    }

    private string Base64DecodeText(string text, bool? additional = null)
    {
        byte[] base64Bytes = System.Convert.FromBase64String(text);
        return System.Text.Encoding.UTF8.GetString(base64Bytes);
    }
}
