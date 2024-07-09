﻿using System.Net;
using System.Windows.Controls;
using Pinny_Notes.Commands;

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
                Command = new CustomCommand() { ExecuteMethod = HtmlEntityEncodeAction }
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Decode",
                Command = new CustomCommand() { ExecuteMethod = HtmlEntityDecodeAction }
            }
        );

        return menuItem;
    }

    private bool HtmlEntityEncodeAction()
    {
        ApplyFunctionToNoteText<bool?>(HtmlEntityEncodeText);
        return true;
    }

    private bool HtmlEntityDecodeAction()
    {
        ApplyFunctionToNoteText<bool?>(HtmlEntityDecodeText);
        return true;
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
