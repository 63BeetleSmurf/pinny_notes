using CommunityToolkit.Mvvm.Input;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class HashTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        HashSHA512,
        HashSHA384,
        HashSHA256,
        HashSHA1,
        HashMD5
    }

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Hash",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA512",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.HashSHA512
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA384",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.HashSHA384
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA256",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.HashSHA256
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA1",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.HashSHA1
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "MD5",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.HashMD5
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
        HashAlgorithm hasher;
        switch (action)
        {
            case ToolActions.HashSHA512:
                hasher = SHA512.Create();
                break;
            case ToolActions.HashSHA384:
                hasher = SHA384.Create();
                break;
            case ToolActions.HashSHA256:
                hasher = SHA256.Create();
                break;
            case ToolActions.HashSHA1:
                hasher = SHA1.Create();
                break;
            case ToolActions.HashMD5:
                hasher = MD5.Create();
                break;
            default:
                return text;
        }

        return BitConverter.ToString(
            hasher.ComputeHash(Encoding.UTF8.GetBytes(_noteTextBox.Text))
        ).Replace("-", "");
    }
}
