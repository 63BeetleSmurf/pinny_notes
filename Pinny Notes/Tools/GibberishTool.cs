using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class GibberishTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    private const string _characters = "zqxjkvbbppyyggffwwmmuuccllldddrrrhhhsssnnniiiiooooaaaaattttteeeeeeeeee";
    private const string _doubleNewLine = "\r\n\r\n";

    private Random random = new Random();

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
                Command = new RelayCommand(GibberishWordAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Title",
                Command = new RelayCommand(GibberishTitleAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Sentence",
                Command = new RelayCommand(GibberishSentenceAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Paragraph",
                Command = new RelayCommand(GibberishParagraphAction)
            }
        );
        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Article",
                Command = new RelayCommand(GibberishArticleAction)
            }
        );

        menuItem.Items.Add(new Separator());

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Name",
                Command = new RelayCommand(GibberishNameAction)
            }
        );

        return menuItem;
    }

    private void GibberishWordAction()
    {
        InsertIntoNoteText(GenerateGibberishWord());
    }

    private void GibberishTitleAction()
    {
        InsertIntoNoteText(GenerateGibberishTitle());
    }

    private void GibberishSentenceAction()
    {
        InsertIntoNoteText(GenerateGibberishSentence());
    }

    private void GibberishParagraphAction()
    {
        InsertIntoNoteText(GenerateGibberishParagraph());
    }

    private void GibberishArticleAction()
    {
        InsertIntoNoteText(GenerateGibberishArticle());
    }

    private void GibberishNameAction()
    {
        InsertIntoNoteText(GenerateGibberishTitle(2));
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
