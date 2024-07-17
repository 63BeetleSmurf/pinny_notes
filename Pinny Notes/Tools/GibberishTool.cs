using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class GibberishTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        GibberishWord,
        GibberishTitle,
        GibberishSentence,
        GibberishParagraph,
        GibberishArticle,
        GibberishName,
    }

    private const string _characters = "zqxjkvbbppyyggffwwmmuuccllldddrrrhhhsssnnniiiiooooaaaaattttteeeeeeeeee";
    private const string _doubleNewLine = "\r\n\r\n";

    private Random random = new();

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "Gibberish",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Word",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.GibberishWord
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Title",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.GibberishTitle
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sentence",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.GibberishSentence
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Paragraph",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.GibberishParagraph
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Article",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.GibberishArticle
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Name",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.GibberishName
            }
        );

        return menuItem;
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        switch (action)
        {
            case ToolActions.GibberishWord:
                InsertIntoNoteText(GenerateGibberishWord());
                break;
            case ToolActions.GibberishTitle:
                InsertIntoNoteText(GenerateGibberishTitle());
                break;
            case ToolActions.GibberishSentence:
                InsertIntoNoteText(GenerateGibberishSentence());
                break;
            case ToolActions.GibberishParagraph:
                InsertIntoNoteText(GenerateGibberishParagraph());
                break;
            case ToolActions.GibberishArticle:
                InsertIntoNoteText(GenerateGibberishArticle());
                break;
            case ToolActions.GibberishName:
                InsertIntoNoteText(GenerateGibberishTitle(2));
                break;
        }
    }

    private string GenerateGibberishWord(bool titleCase = false)
    {
        int length = random.Next(2, 11);
        char[] chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            chars[i] = _characters[random.Next(_characters.Length)];
            if (i == 0 && titleCase)
                chars[i] = char.ToUpper(chars[i]);
        }

        return new(chars);
    }

    private string GenerateGibberishTitle(int? wordCount = null)
    {
        int length;
        if (wordCount != null)
            length = (int)wordCount;
        else
            length = random.Next(2, 6);

        string[] words = new string[length];

        for (int i = 0; i < length; i++)
            words[i] = GenerateGibberishWord(true);

        return string.Join(' ', words);
    }

    private string GenerateGibberishSentence()
    {
        int length = random.Next(15, 20);
        string[] words = new string[length];

        for (int i = 0; i < length; i++)
        {
            words[i] = GenerateGibberishWord((i == 0));
            if (i >= 2 && i < length - 2 && random.Next(1, 100) <= 10)
                words[i] = $"{words[i]},";
        }

        return $"{string.Join(' ', words)}.";
    }

    private string GenerateGibberishParagraph()
    {
        int length = random.Next(2, 6);
        string[] sentences = new string[length];

        for (int i = 0; i < length; i++)
            sentences[i] = GenerateGibberishSentence();

        return string.Join(' ', sentences);
    }

    private string GenerateGibberishArticle()
    {
        int length = random.Next(5, 10);
        string[] paragraphs = new string[length];

        for (int i = 0; i < length; i++)
        {
            if (random.Next(1, 100) <= 40)
                paragraphs[i] = $"{GenerateGibberishTitle()}{_doubleNewLine}{GenerateGibberishParagraph()}";
            else
                paragraphs[i] = GenerateGibberishParagraph();
        }

        return $"{GenerateGibberishTitle()}{_doubleNewLine}{string.Join(_doubleNewLine, paragraphs)}";
    }
}
