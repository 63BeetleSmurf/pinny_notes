using CommunityToolkit.Mvvm.Input;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

enum HashAlgorithms
{
    SHA512 = 0,
    SHA384 = 1,
    SHA256 = 2,
    SHA1 = 3,
    MD5 = 4
}

public class HashTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
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
                Command = new RelayCommand(HashSHA512Action)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA384",
                Command = new RelayCommand(HashSHA384Action)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA256",
                Command = new RelayCommand(HashSHA256Action)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "SHA1",
                Command = new RelayCommand(HashSHA1Action)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "MD5",
                Command = new RelayCommand(HashMD5Action)
            }
        );

        return menuItem;
    }

    private void HashSHA512Action()
    {
        ApplyFunctionToNoteText<HashAlgorithms>(HashText, HashAlgorithms.SHA512);
    }

    private void HashSHA384Action()
    {
        ApplyFunctionToNoteText<HashAlgorithms>(HashText, HashAlgorithms.SHA384);
    }

    private void HashSHA256Action()
    {
        ApplyFunctionToNoteText<HashAlgorithms>(HashText, HashAlgorithms.SHA256);
    }

    private void HashSHA1Action()
    {
        ApplyFunctionToNoteText<HashAlgorithms>(HashText, HashAlgorithms.SHA1);
    }

    private void HashMD5Action()
    {
        ApplyFunctionToNoteText<HashAlgorithms>(HashText, HashAlgorithms.MD5);
    }

    private string HashText(string text, HashAlgorithms algorithm)
    {
        HashAlgorithm hasher;
        switch (algorithm)
        {
            case HashAlgorithms.SHA512:
                hasher = SHA512.Create();
                break;
            case HashAlgorithms.SHA384:
                hasher = SHA384.Create();
                break;
            case HashAlgorithms.SHA256:
                hasher = SHA256.Create();
                break;
            case HashAlgorithms.SHA1:
                hasher = SHA1.Create();
                break;
            case HashAlgorithms.MD5:
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
