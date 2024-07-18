using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Pinny_Notes.Tools;
using Pinny_Notes.ViewModels;
using Pinny_Notes.Enums;

namespace Pinny_Notes.Views;

public partial class NoteWindow : Window
{
    private NoteViewModel _viewModel { get; }

    private RelayCommand _copyCommand = null!;
    private RelayCommand _cutCommand = null!;
    private RelayCommand _pasteCommand = null!;

    private RelayCommand _clearCommand = null!;
    private RelayCommand _saveCommand = null!;

    private IEnumerable<ITool> _tools = [];

    #region NoteWindow
    public NoteWindow() : this(null) { }
    public NoteWindow(NoteViewModel? parentViewModel = null)
    {
        DataContext = _viewModel = new NoteViewModel(parentViewModel);

        InitializeComponent();

        _tools = [
            new Base64Tool(NoteTextBox),
            new CaseTool(NoteTextBox),
            new DateTimeTool(NoteTextBox),
            new GibberishTool(NoteTextBox),
            new HashTool(NoteTextBox),
            new HtmlEntityTool(NoteTextBox),
            new IndentTool(NoteTextBox),
            new JoinTool(NoteTextBox),
            new JsonTool(NoteTextBox),
            new ListTool(NoteTextBox),
            new QuoteTool(NoteTextBox),
            new RemoveTool(NoteTextBox),
            new SlashTool(NoteTextBox),
            new SplitTool(NoteTextBox),
            new TrimTool(NoteTextBox)
        ];

        NoteTextBox.ContextMenu = new();

        _copyCommand = new(CopyCommandExecute);
        NoteTextBox.InputBindings.Add(new InputBinding(_copyCommand, new KeyGesture(Key.C, ModifierKeys.Control)));
        _cutCommand = new(CutCommandExecute);
        NoteTextBox.InputBindings.Add(new InputBinding(_cutCommand, new KeyGesture(Key.X, ModifierKeys.Control)));
        _pasteCommand = new(PasteCommandExecute);
        NoteTextBox.InputBindings.Add(new InputBinding(_pasteCommand, new KeyGesture(Key.V, ModifierKeys.Control)));

        _clearCommand = new(NoteTextBox.Clear);
        ClearMenuItem.Command = _clearCommand;
        _saveCommand = new(SaveCommandExecute);
        SaveMenuItem.Command = _saveCommand;
    }

    private void NoteWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Check mouse button is pressed as a missed click of a button
        // can cause issues with DragMove().
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();

            // Reset gravity depending what position the note was moved to.
            // This does not effect the saved start up setting, only what
            // direction new child notes will go towards.
            _viewModel.X = Left;
            _viewModel.Y = Top;
            _viewModel.GravityX = (Left < SystemParameters.PrimaryScreenWidth / 2) ? 1 : -1;
            _viewModel.GravityY = (Top < SystemParameters.PrimaryScreenHeight / 2) ? 1 : -1;
        }
    }

    private void NoteWindow_StateChanged(object sender, EventArgs e)
    {
        MinimizeModes minimizeMode = (MinimizeModes)Properties.Settings.Default.MinimizeMode;

        if (WindowState == WindowState.Minimized
            && (
                minimizeMode == MinimizeModes.Prevent 
                || (minimizeMode == MinimizeModes.PreventIfPinned && Topmost)
            )
        )
            WindowState = WindowState.Normal;
    }

    private void NoteWindow_ActivatedChanged(object sender, EventArgs e)
    {
        if (IsActive)
        {
            Topmost = true;
            if (Opacity != 1 && Properties.Settings.Default.OpaqueWhenFocused)
                Opacity = 1;
        }
        else if (_viewModel.IsPinned)
        {
            Opacity = Properties.Settings.Default.TransparentNotes ? 0.8 : 1.0;
        }
        else
        {
            Topmost = false;
            Opacity = (Properties.Settings.Default.TransparentNotes && !Properties.Settings.Default.OnlyTransparentWhenPinned) ? 0.8 : 1.0;
        }
    }

    #endregion

    #region Commands

    public void CopyCommandExecute()
    {
        if (NoteTextBox.SelectionLength == 0)
            return;

        string copiedText = NoteTextBox.SelectedText;
        if (Properties.Settings.Default.TrimCopiedText)
            copiedText = copiedText.Trim();
        Clipboard.SetText(copiedText);
    }

    public void CutCommandExecute()
    {
        if (NoteTextBox.SelectionLength == 0)
            return;

        string copiedText = NoteTextBox.SelectedText;
        if (Properties.Settings.Default.TrimCopiedText)
            copiedText = copiedText.Trim();
        Clipboard.SetText(copiedText);
        NoteTextBox.SelectedText = "";
    }

    public void PasteCommandExecute()
    {
        // Do nothing if clipboard does not contain text.
        if (!Clipboard.ContainsText())
            return;

        // Get text from clipboard and trim if specified
        string clipboardString = Clipboard.GetText();
        if (Properties.Settings.Default.TrimPastedText)
            clipboardString = clipboardString.Trim();

        bool hasSelectedText = (NoteTextBox.SelectionLength > 0);
        int caretIndex = (hasSelectedText) ? NoteTextBox.SelectionStart : NoteTextBox.CaretIndex;
        bool caretAtEnd = (caretIndex == NoteTextBox.Text.Length);

        NoteTextBox.SelectedText = clipboardString;

        NoteTextBox.CaretIndex = caretIndex + clipboardString.Length;
        if (!hasSelectedText && Properties.Settings.Default.KeepNewLineAtEndVisible && caretAtEnd)
            NoteTextBox.ScrollToEnd();

    }

    public void SaveCommandExecute()
    {
        SaveNote();
    }

    #endregion

    #region MiscFunctions

    private MessageBoxResult SaveNote()
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "Text Documents (*.txt)|*.txt|All Files|*"
        };
        if (saveFileDialog.ShowDialog(this) == true)
        {
            File.WriteAllText(saveFileDialog.FileName, NoteTextBox.Text);
            _viewModel.IsSaved = true;
            return MessageBoxResult.OK;
        }
        return MessageBoxResult.Cancel;
    }

    #endregion

    #region TitleBar

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount >= 2)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
    }

    private void NewButton_Click(object sender, RoutedEventArgs e)
    {
        new NoteWindow(_viewModel).Show();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.IsSaved && NoteTextBox.Text != "")
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                this,
                "Do you want to save this note?",
                "Pinny Notes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
            );
            // If the user presses cancel on the message box or 
            // save dialog, do not close.
            if (
                (messageBoxResult == MessageBoxResult.Yes && SaveNote() == MessageBoxResult.Cancel)
                || messageBoxResult == MessageBoxResult.Cancel
            )
                return;
        }
        Close();
    }

    // NewLineEnabledMenuItem - Should trigger NoteTextBox_TextChanged when enabled.

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ((App)Application.Current).ShowSettingsWindow(this);
    }

    #endregion

    #region TextBox

    private void NoteTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.IsSaved = false;
        if (!Properties.Settings.Default.NewLineAtEnd || NoteTextBox.Text == "" || NoteTextBox.Text.EndsWith(Environment.NewLine))
            return;

        bool caretAtEnd = (NoteTextBox.CaretIndex == NoteTextBox.Text.Length);
        // Preserving selection when adding new line
        int selectionStart = NoteTextBox.SelectionStart;
        int selectionLength = NoteTextBox.SelectionLength;

        NoteTextBox.AppendText(Environment.NewLine);

        NoteTextBox.SelectionStart = selectionStart;
        NoteTextBox.SelectionLength = selectionLength;

        if (Properties.Settings.Default.KeepNewLineAtEndVisible && caretAtEnd)
            NoteTextBox.ScrollToEnd();
    }

    private void NoteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (Properties.Settings.Default.AutoCopy && NoteTextBox.SelectionLength > 0)
            _copyCommand.Execute(null);
    }

    private void NoteTextBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    private void NoteTextBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            NoteTextBox.Text = File.ReadAllText(
                ((string[])e.Data.GetData(DataFormats.FileDrop))[0]
            );
        }
    }

    private void NoteTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (NoteTextBox.SelectionLength > 0
            && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        )
            _copyCommand.Execute(null);
    }

    private void NoteTextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Triple click to select line, quadruple click to select entire wrapped line
        if (e.ClickCount < 3 || e.ClickCount > 4)
            return;

        TextBox textBox = (TextBox)sender;

        int lineIndex = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
        int lineLength = textBox.GetLineLength(lineIndex);

        // If there was no new line and is not the last row, line must be wrapped.
        if (e.ClickCount == 4 && lineIndex < textBox.LineCount - 1 && !textBox.GetLineText(lineIndex).EndsWith(Environment.NewLine))
        {
            // Expand length until new line or last line found
            int nextLineIndex = lineIndex;
            do
            {
                nextLineIndex++;
                lineLength += textBox.GetLineLength(nextLineIndex);
            } while (nextLineIndex < textBox.LineCount - 1 && !textBox.GetLineText(nextLineIndex).EndsWith(Environment.NewLine));
        }

        textBox.SelectionStart = textBox.GetCharacterIndexFromLineIndex(lineIndex);
        textBox.SelectionLength = lineLength;

        // Don't select new line char(s)
        if (textBox.SelectedText.EndsWith(Environment.NewLine))
            textBox.SelectionLength -= Environment.NewLine.Length;

        NoteTextBox_MouseDoubleClick(sender, e);
    }

    private void NoteTextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle && Properties.Settings.Default.MiddleClickPaste)
            _pasteCommand.Execute(null);
    }

    private void NoteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && NoteTextBox.SelectionLength > 0)
        {
            int selectionStart = NoteTextBox.SelectionStart;
            int selectionEnd = NoteTextBox.SelectionStart + NoteTextBox.SelectionLength;

            // Fix the selection so full lines, not wrapped lines, are selected
            // Find the starting line by making sure the previous line ends with a new line
            int startLineIndex = NoteTextBox.GetLineIndexFromCharacterIndex(selectionStart);
            while (startLineIndex > 0 && !NoteTextBox.GetLineText(startLineIndex - 1).Contains(Environment.NewLine))
                startLineIndex--;
            selectionStart = NoteTextBox.GetCharacterIndexFromLineIndex(startLineIndex);

            // Find the end line by making sure it ends with a new line
            int endLineIndex = NoteTextBox.GetLineIndexFromCharacterIndex(selectionEnd);
            int lineCount = GetLineCount();
            while (endLineIndex < lineCount - 1 && !NoteTextBox.GetLineText(endLineIndex).Contains(Environment.NewLine))
                endLineIndex++;
            selectionEnd = NoteTextBox.GetCharacterIndexFromLineIndex(endLineIndex) + NoteTextBox.GetLineLength(endLineIndex) - Environment.NewLine.Length;

            // Select the full lines so we can now easily get the required selected text
            NoteTextBox.Select(selectionStart, selectionEnd - selectionStart);

            // Loop though each line adding or removing a tab where required
            string[] lines = NoteTextBox.SelectedText.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (lines[i].Length > 0 && lines[i][0] == '\t')
                        lines[i] = lines[i].Remove(0, 1);
                }
                else
                {
                    lines[i] = $"\t{lines[i]}";
                }
            }

            NoteTextBox.SelectedText = string.Join(Environment.NewLine, lines);

            e.Handled = true;
        }
        else if (e.Key == Key.Return && Properties.Settings.Default.AutoIndent)
        {
            // If there is selected text remove it and set caret to correct position
            if (NoteTextBox.SelectionLength > 0)
            {
                int selectionStart = NoteTextBox.SelectionStart;
                NoteTextBox.Text = NoteTextBox.Text.Remove(selectionStart, NoteTextBox.SelectionLength);
                NoteTextBox.CaretIndex = selectionStart;
            }

            // Store caret position for positioning later
            int caretIndex = NoteTextBox.CaretIndex;

            // Get the current line of text, trimming any new lines
            string line = NoteTextBox.GetLineText(NoteTextBox.GetLineIndexFromCharacterIndex(caretIndex)).TrimEnd(Environment.NewLine.ToCharArray());

            // Get the whitespace from the beginning of the line and create our indent string
            string preceedingWhitespace = new(line.TakeWhile(char.IsWhiteSpace).ToArray());
            string indent = Environment.NewLine + preceedingWhitespace;

            // Add the indent and restore caret position
            NoteTextBox.Text = NoteTextBox.Text.Insert(caretIndex, indent);
            NoteTextBox.CaretIndex = caretIndex + indent.Length;

            e.Handled = true;
        }
    }

    #region ContextMenu

    private ContextMenu GetNoteTextBoxContextMenu()
    {
        ContextMenu menu = new();

        int caretIndex = NoteTextBox.CaretIndex;
        SpellingError spellingError = NoteTextBox.GetSpellingError(caretIndex);
        if (spellingError != null)
        {
            if (!spellingError.Suggestions.Any())
                menu.Items.Add(
                        new MenuItem()
                        {
                            Header = "(no spelling suggestions)",
                            IsEnabled = false
                        }
                    );
            else
                foreach (string spellingSuggestion in spellingError.Suggestions)
                {
                    menu.Items.Add(
                        new MenuItem()
                        {
                            Header = spellingSuggestion,
                            FontWeight = FontWeights.Bold,
                            Command = EditingCommands.CorrectSpellingError,
                            CommandParameter = spellingSuggestion,
                            CommandTarget = NoteTextBox
                        }
                    );
                }
            menu.Items.Add(new Separator());
        }

        menu.Items.Add(
            new MenuItem()
            {
                Header = "Copy",
                Command = _copyCommand,
                InputGestureText = "Ctrl+C",
                IsEnabled = (NoteTextBox.SelectionLength > 0)
            }
        );
        menu.Items.Add(
            new MenuItem()
            {
                Header = "Cut",
                Command = _cutCommand,
                InputGestureText = "Ctrl+X",
                IsEnabled = (NoteTextBox.SelectionLength > 0)
            }
        );
        menu.Items.Add(
            new MenuItem()
            {
                Header = "Paste",
                Command = _pasteCommand,
                InputGestureText = "Ctrl+V",
                IsEnabled = Clipboard.ContainsText()
            }
        );
            
        menu.Items.Add(new Separator());

        menu.Items.Add(
            new MenuItem()
            {
                Header = "Select All",
                Command = new RelayCommand(NoteTextBox.SelectAll),
                IsEnabled = (NoteTextBox.Text.Length > 0)
            }
        );
        menu.Items.Add(
            new MenuItem()
            {
                Header = "Clear",
                Command = _clearCommand,
                IsEnabled = (NoteTextBox.Text.Length > 0)
            }
        );
        menu.Items.Add(
            new MenuItem()
            {
                Header = "Save",
                Command = _saveCommand,
                IsEnabled = (NoteTextBox.Text.Length > 0)
            }
        );

        menu.Items.Add(new Separator());

        MenuItem countsMenuItem = new()
        {
            Header = "Counts"
        };
        countsMenuItem.Items.Add(
            new MenuItem()
            {
                Header = $"Lines: {GetLineCount()}",
                IsEnabled = false
            }
        );
        countsMenuItem.Items.Add(
            new MenuItem()
            {
                Header = $"Words: {GetWordCount()}",
                IsEnabled = false
            }
        );
        countsMenuItem.Items.Add(
            new MenuItem()
            {
                Header = $"Chars: {GetCharCount()}",
                IsEnabled = false
            }
        );
        menu.Items.Add(countsMenuItem);

        menu.Items.Add(new Separator());

        MenuItem toolsMenu = new()
        {
            Header = "Tools"
        };
        foreach (ITool tool in _tools)
            toolsMenu.Items.Add(
                tool.GetMenuItem()
            );
        menu.Items.Add(toolsMenu);

        return menu;
    }

    private int GetLineCount()
    {
        int count;
        if (NoteTextBox.SelectionLength > 0)
            count = NoteTextBox.GetLineIndexFromCharacterIndex(NoteTextBox.SelectionStart + NoteTextBox.SelectionLength)
                -  NoteTextBox.GetLineIndexFromCharacterIndex(NoteTextBox.SelectionStart)
                + 1;
        else
            count = NoteTextBox.GetLineIndexFromCharacterIndex(NoteTextBox.Text.Length) + 1;
        return count;
    }

    private int GetWordCount()
    {
        string text = (NoteTextBox.SelectionLength > 0) ? NoteTextBox.SelectedText : NoteTextBox.Text;
        if (text.Length == 0)
            return 0;
        return text.Split((char[])[' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private int GetCharCount()
    {
        string text = (NoteTextBox.SelectionLength > 0) ? NoteTextBox.SelectedText : NoteTextBox.Text;
        if (text.Length == 0)
            return 0;
        return text.Length - text.Count(c => c == '\n' || c == '\r'); // Substract new lines from count.
    }

    private void NoteTextBox_ContextMenuOpening(object sender, RoutedEventArgs e)
    {
        NoteTextBox.ContextMenu = GetNoteTextBoxContextMenu();
    }

    #endregion

    #endregion

}
