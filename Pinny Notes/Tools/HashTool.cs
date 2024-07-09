using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class HashTool : BaseTool, ITool
{
    public HashTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "Hash",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA512",
                Command = new CustomCommand() { ExecuteMethod = HashSHA512Action }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA384",
                Command = new CustomCommand() { ExecuteMethod = HashSHA384Action }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA256",
                Command = new CustomCommand() { ExecuteMethod = HashSHA256Action }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA1",
                Command = new CustomCommand() { ExecuteMethod = HashSHA1Action }
            }
        );
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "MD5",
                Command = new CustomCommand() { ExecuteMethod = HashMD5Action }
            }
        );
    }

    private bool HashSHA512Action()
    {
        ApplyFunctionToNoteText(HashText, "sha512");
        return true;
    }

    private bool HashSHA384Action()
    {
        ApplyFunctionToNoteText(HashText, "sha384");
        return true;
    }

    private bool HashSHA256Action()
    {
        ApplyFunctionToNoteText(HashText, "sha256");
        return true;
    }

    private bool HashSHA1Action()
    {
        ApplyFunctionToNoteText(HashText, "sha1");
        return true;
    }

    private bool HashMD5Action()
    {
        ApplyFunctionToNoteText(HashText, "md5");
        return true;
    }

    private string HashText(string text, string? algorithm)
    {
        HashAlgorithm hasher;
        switch (algorithm)
        {
            case "sha512":
                hasher = SHA512.Create();
                break;
            case "sha384":
                hasher = SHA384.Create();
                break;
            case "sha256":
                hasher = SHA256.Create();
                break;
            case "sha1":
                hasher = SHA1.Create();
                break;
            case "md5":
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
