using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class GibberishTool : BaseTool, ITool
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

    public GibberishTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "Gibberish";
        _menuActions.Add(new("Word", MenuActionCommand, ToolActions.GibberishWord));
        _menuActions.Add(new("Title", MenuActionCommand, ToolActions.GibberishTitle));
        _menuActions.Add(new("Sentence", MenuActionCommand, ToolActions.GibberishSentence));
        _menuActions.Add(new("Paragraph", MenuActionCommand, ToolActions.GibberishParagraph));
        _menuActions.Add(new("Article", MenuActionCommand, ToolActions.GibberishArticle));
        _menuActions.Add(new("-"));
        _menuActions.Add(new("Name", MenuActionCommand, ToolActions.GibberishName));
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
