using CommunityToolkit.Mvvm.Input;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class HashTool : BaseTool, ITool
{
    public enum ToolActions
    {
        HashSHA512,
        HashSHA384,
        HashSHA256,
        HashSHA1,
        HashMD5
    }

    public HashTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Hash";
        _menuActions.Add(new("SHA512", MenuActionCommand, ToolActions.HashSHA512));
        _menuActions.Add(new("SHA384", MenuActionCommand, ToolActions.HashSHA384));
        _menuActions.Add(new("SHA256", MenuActionCommand, ToolActions.HashSHA256));
        _menuActions.Add(new("SHA1", MenuActionCommand, ToolActions.HashSHA1));
        _menuActions.Add(new("MD5", MenuActionCommand, ToolActions.HashMD5));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, Enum action)
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
