using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Pinny_Notes.Enums;
using Pinny_Notes.Commands;
using Pinny_Notes.Themes;
using Pinny_Notes.Tools;

namespace Pinny_Notes;

public partial class MainWindow : Window
{
    private readonly char[] _wordSeparators = [' ', '\t', '\r', '\n'];

    // Title Bar, Background, Border
    private readonly Dictionary<ThemeColors, NoteTheme> _noteThemes = new()
    {
        {ThemeColors.Yellow, new NoteTheme("Yellow", Color.FromRgb(254, 247, 177), Color.FromRgb(255, 252, 221), Color.FromRgb(254, 234, 0))},  // #fef7b1 #fffcdd #feea00
        {ThemeColors.Orange, new NoteTheme("Orange", Color.FromRgb(255, 209, 121), Color.FromRgb(254, 232, 185), Color.FromRgb(255, 171, 0))},  // #ffd179 #fee8b9 #ffab00
        {ThemeColors.Red, new NoteTheme("Red", Color.FromRgb(255, 124, 129), Color.FromRgb(255, 196, 198), Color.FromRgb(227, 48, 54))},        // #ff7c81 #ffc4c6 #e33036
        {ThemeColors.Pink, new NoteTheme("Pink", Color.FromRgb(217, 134, 204), Color.FromRgb(235, 191, 227), Color.FromRgb(167, 41, 149))},     // #d986cc #ebbfe3 #a72995
        {ThemeColors.Purple, new NoteTheme("Purple", Color.FromRgb(157, 154, 221), Color.FromRgb(208, 206, 243), Color.FromRgb(98, 91, 184))},  // #9d9add #d0cef3 #625bb8
        {ThemeColors.Blue, new NoteTheme("Blue", Color.FromRgb(122, 195, 230), Color.FromRgb(179, 217, 236), Color.FromRgb(17, 149, 221))},     // #7ac3e6 #b3d9ec #1195dd
        {ThemeColors.Aqua, new NoteTheme("Aqua", Color.FromRgb(151, 207, 198), Color.FromRgb(192, 226, 225), Color.FromRgb(22, 176, 152))},     // #97cfc6 #c0e2e1 #16b098
        {ThemeColors.Green, new NoteTheme("Green", Color.FromRgb(198, 214, 125), Color.FromRgb(227, 235, 198), Color.FromRgb(170, 204, 4))}     // #c6d67d #e3ebc6 #aacc04
    };

    private ThemeColors _noteCurrentTheme;
    private Tuple<bool, bool> _noteGravity = new(true, true);
    private bool _noteSaved = false;

    private readonly CustomCommand _copyCommand = new();
    private readonly CustomCommand _cutCommand = new();
    private readonly CustomCommand _pasteCommand = new();

    private readonly CustomCommand _clearCommand = new();
    private readonly CustomCommand _saveCommand = new();

    private IEnumerable<ITool> _tools = [];

    #region MainWindow

    public MainWindow()
    {
        MainWindowInitialize();
        LoadSettings();
        PositionNote();
    }

    public MainWindow(double parentLeft, double parentTop, ThemeColors? parentColour, Tuple<bool, bool>? parentGravity)
    {
        MainWindowInitialize();
        LoadSettings(parentColour, parentGravity);
        PositionNote(parentLeft, parentTop);
    }

    private void MainWindowInitialize()
    {
        InitializeComponent();

        _tools = [
            new Base64Tool(NoteTextBox),
            new CaseTool(NoteTextBox),
            new HashTool(NoteTextBox),
            new HtmlEntityTool(NoteTextBox),
            new IndentTool(NoteTextBox),
            new JoinTool(NoteTextBox),
            new JsonTool(NoteTextBox),
            new ListTool(NoteTextBox),
            new QuoteTool(NoteTextBox),
            new SplitTool(NoteTextBox),
            new TrimTool(NoteTextBox)
        ];

        NoteTextBox.ContextMenu = new();

        _copyCommand.ExecuteMethod = NoteTextBox_Copy;
        NoteTextBox.InputBindings.Add(new InputBinding(_copyCommand, new KeyGesture(Key.C, ModifierKeys.Control)));
        _cutCommand.ExecuteMethod = NoteTextBox_Cut;
        NoteTextBox.InputBindings.Add(new InputBinding(_cutCommand, new KeyGesture(Key.X, ModifierKeys.Control)));
        _pasteCommand.ExecuteMethod = NoteTextBox_Paste;
        NoteTextBox.InputBindings.Add(new InputBinding(_pasteCommand, new KeyGesture(Key.V, ModifierKeys.Control)));

        _clearCommand.ExecuteMethod = NoteTextBox_Clear;
        ClearMenuItem.Command = _clearCommand;
        _saveCommand.ExecuteMethod = NoteTextBox_Save;
        SaveMenuItem.Command = _saveCommand;
    }

    private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Check mouse button is pressed as a missed click of a button
        // can cause issues with DragMove().
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();

            // Reset gravity depending what position the note was moved to.
            // This does not effect the saved start up setting, only what
            // direction new child notes will go towards.
            bool gravityLeft = true;
            bool gravityTop = true;
            if (Left > SystemParameters.PrimaryScreenWidth / 2)
                gravityLeft = false;
            if (Top > SystemParameters.PrimaryScreenHeight / 2)
                gravityTop = false;
            _noteGravity = new Tuple<bool, bool>(
                gravityLeft,
                gravityTop
            );
        }
    }

    #endregion

    #region MiscFunctions

    private void LoadSettings(ThemeColors? parentColour = null, Tuple<bool, bool>? parentGravity = null)
    {
        AutoCopyMenuItem.IsChecked = Properties.Settings.Default.AutoCopy;
        TrimCopiedTextMenuItem.IsChecked = Properties.Settings.Default.TrimCopiedText;
        TrimPastedTextMenuItem.IsChecked = Properties.Settings.Default.TrimPastedText;
        MiddleClickPasteMenuItem.IsChecked = Properties.Settings.Default.MiddleClickPaste;
        SpellCheckMenuItem.IsChecked = Properties.Settings.Default.SpellCheck;
        NoteTextBox.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
        NewLineEnabledMenuItem.IsChecked = Properties.Settings.Default.NewLine;
        NewLineKeepVisibleMenuItem.IsChecked = Properties.Settings.Default.KeepNewLineAtEndVisible;
        AutoIndentMenuItem.IsChecked = Properties.Settings.Default.AutoIndent;
        ColourCycleMenuItem.IsChecked = Properties.Settings.Default.CycleColours;
        SetColour(parentColour: parentColour);
        if (parentGravity == null)
            _noteGravity = new Tuple<bool, bool>(
                Properties.Settings.Default.StartupPositionLeft,
                Properties.Settings.Default.StartupPositionTop
            );
        else
            _noteGravity = parentGravity;

        if (Properties.Settings.Default.StartupPositionLeft)
        {
            if (Properties.Settings.Default.StartupPositionTop)
                StartupPositionTopLeftMenuItem.IsChecked = true;
            else
                StartupPositionBottomLeftMenuItem.IsChecked = true;
        }
        else
        {
            if (Properties.Settings.Default.StartupPositionTop)
                StartupPositionTopRightMenuItem.IsChecked = true;
            else
                StartupPositionBottomRightMenuItem.IsChecked = true;
        }

    }

    private void SetColour(ThemeColors? colour = null, ThemeColors? parentColour = null)
    {
        if (colour != null)
        {
            _noteCurrentTheme = (ThemeColors)colour;
        }
        else if (!Properties.Settings.Default.CycleColours)
        {
            _noteCurrentTheme = (ThemeColors)Properties.Settings.Default.Colour;
        }
        else
        {
            // Get the next colour ensuring it is not the same as the parent notes colour.
            ThemeColors? nextColour = null;
            int nextColourIndex = _noteThemes.Keys.ToList().IndexOf((ThemeColors)Properties.Settings.Default.Colour) + 1;
            while (nextColour == null)
            {
                nextColour = _noteThemes.Keys.ElementAtOrDefault(nextColourIndex);
                if (nextColour == null)
                {
                    nextColourIndex = 0;
                }
                else if (nextColour == parentColour)
                {
                    nextColour = null;
                    nextColourIndex++;
                }
            }
            _noteCurrentTheme = (ThemeColors)nextColour;
        }

        TitleBarGrid.Background = new SolidColorBrush(_noteThemes[_noteCurrentTheme].TitleBarColor);
        Background = new SolidColorBrush(_noteThemes[_noteCurrentTheme].BackgroundColor);
        BorderBrush = new SolidColorBrush(_noteThemes[_noteCurrentTheme].BorderColor);

        Properties.Settings.Default.Colour = (int)_noteCurrentTheme;
        Properties.Settings.Default.Save();

        // Tick correct menu item for colour or note.
        foreach (object childObject in ColoursMenuItem.Items)
        {
            if (childObject.GetType().Name != "MenuItem")
                continue;

            MenuItem childMenuItem = (MenuItem)childObject;
            if (childMenuItem.Header.ToString() != "Cycle" && childMenuItem.Header.ToString() != _noteThemes[_noteCurrentTheme].Name)
                childMenuItem.IsChecked = false;
            else if (childMenuItem.Header.ToString() == _noteThemes[_noteCurrentTheme].Name)
                childMenuItem.IsChecked = true;
        }
    }

    private void PositionNote(double? parentLeft = null, double? parentTop = null)
    {
        double positionTop;
        double positionLeft;

        // If there is no parent, position relative to screen
        if (parentLeft == null || parentTop == null)
        {
            int screenMargin = 78;
            if (_noteGravity.Item1) // Left
                positionLeft = screenMargin;
            else // Right
                positionLeft = (SystemParameters.PrimaryScreenWidth - screenMargin) - Width;

            if (_noteGravity.Item2) // Top
                positionTop = screenMargin;
            else // Bottom
                positionTop = (SystemParameters.PrimaryScreenHeight - screenMargin) - Height;
        }
        // Position relative to parent
        else
        {
            if (_noteGravity.Item1)
                positionLeft = (double)parentLeft + 45;
            else
                positionLeft = (double)parentLeft - 45;

            if (_noteGravity.Item2)
                positionTop = (double)parentTop + 45;
            else
                positionTop = (double)parentTop - 45;
        }

        // Don't allow note to open off screen. Will eventually end up stuck
        // in a corner, but that's only after opening a silly number of notes.
        if (positionLeft < 0)
            Left = 0;
        else if (positionLeft + Width > SystemParameters.PrimaryScreenWidth)
            Left = SystemParameters.PrimaryScreenWidth - Width;
        else
            Left = positionLeft;

        if (positionTop < 0)
            Top = 0;
        else if (positionTop + Height > SystemParameters.PrimaryScreenHeight)
            Top = SystemParameters.PrimaryScreenHeight - Height;
        else
            Top = positionTop;
    }

    private MessageBoxResult SaveNote()
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "Text Documents (*.txt)|*.txt|All Files|*"
        };
        if (saveFileDialog.ShowDialog(this) == true)
        {
            File.WriteAllText(saveFileDialog.FileName, NoteTextBox.Text);
            _noteSaved = true;
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
        new MainWindow(
            Left,
            Top,
            _noteCurrentTheme,
            _noteGravity
        ).Show();
    }

    private void TopButton_Click(object sender, RoutedEventArgs e)
    {
        Topmost = !Topmost;

        if (Topmost)
            TopButton.Content = (BitmapImage)Resources["PinImageSource"];
        else
            TopButton.Content = (BitmapImage)Resources["Pin45ImageSource"];
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_noteSaved && NoteTextBox.Text != "")
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

    #region ContextMenu

    #region Colours

    private void ColourCycleMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.CycleColours = ColourCycleMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void ColourMenuItem_Click(object sender, RoutedEventArgs e)
    {
        // Don't allow uncheckign active item.
        MenuItem menuItem = (MenuItem)sender;
        if (!menuItem.IsChecked)
        {
            menuItem.IsChecked = true;
            return;
        }

        // To Do - Implement commands which will pass parameter for color.
        foreach (KeyValuePair<ThemeColors, NoteTheme> noteTheme in _noteThemes)
        {
            if (noteTheme.Value.Name == menuItem.Header.ToString())
            {
                SetColour(noteTheme.Key);
                break;
            }
        }
    }

    #endregion

    #region Settings

    private void StartupPositionMenuItem_Click(object sender, RoutedEventArgs e)
    {
        // Don't allow uncheckign active item.
        MenuItem menuItem = (MenuItem)sender;
        if (!menuItem.IsChecked)
        {
            menuItem.IsChecked = true;
            return;
        }

        // Uncheck all other items when this is checked.
        foreach (object childObject in StartupPositionMenuItem.Items)
        {
            MenuItem childMenuItem = (MenuItem)childObject;
            if (childMenuItem != menuItem)
                childMenuItem.IsChecked = false;
        }

        string menuItemText = menuItem.Header.ToString() ?? "";
        string[] position = menuItemText.Split(" ");
        if (position[0] == "Top")
            Properties.Settings.Default.StartupPositionTop = true;
        else
            Properties.Settings.Default.StartupPositionTop = false;
        if (position[1] == "Left")
            Properties.Settings.Default.StartupPositionLeft = true;
        else
            Properties.Settings.Default.StartupPositionLeft = false;
        Properties.Settings.Default.Save();
    }

    private void AutoCopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AutoCopy = AutoCopyMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void TrimCopiedTextMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.TrimCopiedText = TrimCopiedTextMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void TrimPastedTextMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.TrimPastedText = TrimPastedTextMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void MiddleClickPasteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.MiddleClickPaste = MiddleClickPasteMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void SpellCheckMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NoteTextBox.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
        Properties.Settings.Default.SpellCheck = SpellCheckMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void NewLineEnabledMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.NewLine = NewLineEnabledMenuItem.IsChecked;
        Properties.Settings.Default.Save();

        // Check for new line when this option is activated
        if (Properties.Settings.Default.NewLine)
            NoteTextBox_TextChanged(sender, e);
    }

    private void NewLineKeepVisibleMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.KeepNewLineAtEndVisible = NewLineKeepVisibleMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    private void AutoIndentMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AutoIndent = AutoIndentMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    #endregion

    #endregion


    #endregion

    #region TextBox

    private void NoteTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        _noteSaved = false;
        if (Properties.Settings.Default.NewLine && NoteTextBox.Text != "" && !NoteTextBox.Text.EndsWith(Environment.NewLine))
        {
            bool caretAtEnd = (NoteTextBox.CaretIndex == NoteTextBox.Text.Length);
            // Preserving selection when adding new line
            int selectionStart = NoteTextBox.SelectionStart;
            int selectionLength = NoteTextBox.SelectionLength;

            NoteTextBox.Text += Environment.NewLine;

            NoteTextBox.SelectionStart = selectionStart;
            NoteTextBox.SelectionLength = selectionLength;

            if (Properties.Settings.Default.KeepNewLineAtEndVisible && caretAtEnd)
                NoteTextBox.ScrollToEnd();
        }
    }

    private void NoteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (Properties.Settings.Default.AutoCopy && NoteTextBox.SelectionLength > 0)
            NoteTextBox_Copy();
    }

    private void NoteTextBox_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private void NoteTextBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            NoteTextBox.Text = File.ReadAllText(
                ((string[])e.Data.GetData(DataFormats.FileDrop))[0]
            );
        }
        else if (e.Data.GetDataPresent(DataFormats.StringFormat))
        {
            NoteTextBox.Text = (string)e.Data.GetData(DataFormats.StringFormat);
        }
    }

    private void NoteTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (NoteTextBox.SelectionLength > 0
            && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        )
            NoteTextBox_Copy();
    }

    private void NoteTextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Triple click to select line, quadruple click to select entire wrapped line
        if (e.ClickCount >= 3)
        {
            TextBox textBox = (TextBox)sender;
            int lineIndex = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
            int lineLength = textBox.GetLineLength(lineIndex);

            // Don't select new line char(s)
            if (textBox.GetLineText(lineIndex).EndsWith(Environment.NewLine))
                lineLength -= 2;
            // If no following new line and not the last row, line must be wrapped.
            else if (e.ClickCount == 4 && lineIndex < textBox.LineCount - 1)
            {
                // Expand length until new line or last line found
                int nextLineIndex = lineIndex;
                do
                {
                    nextLineIndex++;
                    lineLength += textBox.GetLineLength(nextLineIndex);
                } while (!textBox.GetLineText(nextLineIndex).EndsWith(Environment.NewLine) && nextLineIndex < textBox.LineCount - 1);
                // Don't select new line char(s)
                if (textBox.GetLineText(nextLineIndex).EndsWith(Environment.NewLine))
                    lineLength -= 2;
            }

            textBox.SelectionStart = textBox.GetCharacterIndexFromLineIndex(lineIndex);
            textBox.SelectionLength = lineLength;

            NoteTextBox_MouseDoubleClick(sender, e);
        }
    }

    private void NoteTextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (Properties.Settings.Default.MiddleClickPaste && e.ChangedButton == MouseButton.Middle)
            NoteTextBox_Paste();
    }

    private void NoteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return && Properties.Settings.Default.AutoIndent)
        {
            e.Handled = true;

            TextBox textBox = (TextBox)sender;

            if (textBox.SelectionLength > 0)
            {
                int selectionStart = textBox.SelectionStart;
                textBox.Text = $"{textBox.Text[..selectionStart]}{textBox.Text[(selectionStart + textBox.SelectionLength)..]}";
                textBox.CaretIndex = selectionStart;
            }

            int caretIndex = textBox.CaretIndex;
            string line = textBox.GetLineText(textBox.GetLineIndexFromCharacterIndex(caretIndex));

            int i = 0;
            string indent = Environment.NewLine;
            while (i < line.Length && (line[i] == ' ' || line[i] == '\t'))
            {
                indent += line[i];
                i++;
            }

            textBox.Text = textBox.Text.Insert(caretIndex, indent);
            textBox.CaretIndex = caretIndex + indent.Length;
        }
    }

    public bool NoteTextBox_Copy()
    {
        string copiedText = NoteTextBox.SelectedText;
        if (Properties.Settings.Default.TrimCopiedText)
            copiedText = copiedText.Trim();
        Clipboard.SetDataObject(copiedText);
        return true;
    }

    public bool NoteTextBox_Cut()
    {
        string copiedText = NoteTextBox.SelectedText;
        if (Properties.Settings.Default.TrimCopiedText)
            copiedText = copiedText.Trim();
        Clipboard.SetDataObject(copiedText);
        int selectionStart = NoteTextBox.SelectionStart;
        NoteTextBox.Text = $"{ NoteTextBox.Text[..selectionStart] }{ NoteTextBox.Text[(selectionStart + NoteTextBox.SelectionLength)..] }";
        NoteTextBox.CaretIndex = selectionStart;
        return true;
    }

    public bool NoteTextBox_Paste()
    {
        IDataObject clipboardData = Clipboard.GetDataObject();
        if (clipboardData.GetDataPresent(DataFormats.Text))
        {
            string clipboardString = (String)clipboardData.GetData(DataFormats.Text);
            if (Properties.Settings.Default.TrimPastedText)
                clipboardString = clipboardString.Trim();

            if (NoteTextBox.SelectionLength == 0)
            {
                int caretIndex = NoteTextBox.CaretIndex;
                bool caretAtEnd = (caretIndex == NoteTextBox.Text.Length);
                NoteTextBox.Text = $"{ NoteTextBox.Text[..caretIndex] }{ clipboardString }{ NoteTextBox.Text[caretIndex..] }";
                NoteTextBox.CaretIndex = caretIndex + clipboardString.Length;
                if (Properties.Settings.Default.KeepNewLineAtEndVisible && caretAtEnd)
                    NoteTextBox.ScrollToEnd();
            }
            else
            {
                int selectionStart = NoteTextBox.SelectionStart;
                NoteTextBox.Text = $"{ NoteTextBox.Text[..selectionStart] }{ clipboardString }{ NoteTextBox.Text[(selectionStart + NoteTextBox.SelectionLength)..] }";
                NoteTextBox.CaretIndex = selectionStart + clipboardString.Length;
            }
        }
        return true;
    }

    public bool NoteTextBox_SelectAll()
    {
        NoteTextBox.SelectAll();
        return true;
    }

    public bool NoteTextBox_Clear()
    {
        NoteTextBox.Clear();
        return true;
    }

    public bool NoteTextBox_Save()
    {
        SaveNote();
        return true;
    }

    #region ContextMenu

    private ContextMenu GetNoteTextBoxContextMenu()
    {
        ContextMenu menu = new();

        int caretIndex = NoteTextBox.CaretIndex;
        SpellingError spellingError = NoteTextBox.GetSpellingError(caretIndex);
        if (spellingError != null)
        {
            if (spellingError.Suggestions.Any())
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
                Command = new CustomCommand() { ExecuteMethod = NoteTextBox_SelectAll },
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
            count = NoteTextBox.GetLineIndexFromCharacterIndex(NoteTextBox.Text.Length);
        return count;
    }

    private int GetWordCount()
    {
        int count = 0;
        string text;
        if (NoteTextBox.SelectionLength > 0)
            text = NoteTextBox.SelectedText;
        else
            text = NoteTextBox.Text;
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            string[] words = text.Split(_wordSeparators, StringSplitOptions.RemoveEmptyEntries);
            count = words.Length;
        }
        return count;
    }

    private int GetCharCount()
    {
        int count = 0;
        string text;
        if (NoteTextBox.SelectionLength > 0)
            text = NoteTextBox.SelectedText;
        else
            text = NoteTextBox.Text;
        if (!string.IsNullOrEmpty(text))
        {
            text = text.Replace(Environment.NewLine, "");
            count = text.Length;
        }
        return count;
    }

    private void NoteTextBox_ContextMenuOpening(object sender, RoutedEventArgs e)
    {
        NoteTextBox.ContextMenu = GetNoteTextBoxContextMenu();
    }

    #endregion

    #endregion

}
