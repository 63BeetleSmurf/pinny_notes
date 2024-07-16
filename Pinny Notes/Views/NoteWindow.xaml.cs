﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pinny_Notes.Enums;
using Pinny_Notes.Themes;
using Pinny_Notes.Tools;

namespace Pinny_Notes.Views;

public partial class NoteWindow : Window
{
    private readonly char[] _wordSeparators = [' ', '\t', '\r', '\n'];

    // Title Bar, Background, Border
    private readonly Dictionary<ThemeColors, NoteTheme> _noteThemes = new()
    {
        {ThemeColors.Yellow, new NoteTheme(Color.FromRgb(254, 247, 177), Color.FromRgb(255, 252, 221), Color.FromRgb(254, 234, 0))},    // #fef7b1 #fffcdd #feea00
        {ThemeColors.Orange, new NoteTheme(Color.FromRgb(255, 209, 121), Color.FromRgb(254, 232, 185), Color.FromRgb(255, 171, 0))},    // #ffd179 #fee8b9 #ffab00
        {ThemeColors.Red, new NoteTheme(Color.FromRgb(255, 124, 129), Color.FromRgb(255, 196, 198), Color.FromRgb(227, 48, 54))},       // #ff7c81 #ffc4c6 #e33036
        {ThemeColors.Pink, new NoteTheme(Color.FromRgb(217, 134, 204), Color.FromRgb(235, 191, 227), Color.FromRgb(167, 41, 149))},     // #d986cc #ebbfe3 #a72995
        {ThemeColors.Purple, new NoteTheme(Color.FromRgb(157, 154, 221), Color.FromRgb(208, 206, 243), Color.FromRgb(98, 91, 184))},    // #9d9add #d0cef3 #625bb8
        {ThemeColors.Blue, new NoteTheme(Color.FromRgb(122, 195, 230), Color.FromRgb(179, 217, 236), Color.FromRgb(17, 149, 221))},     // #7ac3e6 #b3d9ec #1195dd
        {ThemeColors.Aqua, new NoteTheme(Color.FromRgb(151, 207, 198), Color.FromRgb(192, 226, 225), Color.FromRgb(22, 176, 152))},     // #97cfc6 #c0e2e1 #16b098
        {ThemeColors.Green, new NoteTheme(Color.FromRgb(198, 214, 125), Color.FromRgb(227, 235, 198), Color.FromRgb(170, 204, 4))}      // #c6d67d #e3ebc6 #aacc04
    };

    private ThemeColors _noteCurrentTheme;
    private Tuple<bool, bool> _noteGravity = new(true, true);
    private bool _noteSaved = false;

    private RelayCommand _copyCommand = null!;
    private RelayCommand _cutCommand = null!;
    private RelayCommand _pasteCommand = null!;

    private RelayCommand _clearCommand = null!;
    private RelayCommand _saveCommand = null!;

    private RelayCommand<ThemeColors> _changeThemeColorCommand = null!;

    private IEnumerable<ITool> _tools = [];

    private bool _isPinned = false;

    #region NoteWindow

    public NoteWindow()
    {
        NoteWindowInitialize();
        LoadSettings();
        PositionNote();
    }

    public NoteWindow(double parentLeft, double parentTop, ThemeColors? parentColor, Tuple<bool, bool>? parentGravity)
    {
        NoteWindowInitialize();
        LoadSettings(parentColor, parentGravity);
        PositionNote(parentLeft, parentTop);
    }

    private void NoteWindowInitialize()
    {
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

        _clearCommand = new(ClearCommandExecute);
        ClearMenuItem.Command = _clearCommand;
        _saveCommand = new(SaveCommandExecute);
        SaveMenuItem.Command = _saveCommand;

        _changeThemeColorCommand = new(ChangeThemeColorCommandExecute);
        ColorYellowMenuItem.Command = _changeThemeColorCommand;
        ColorYellowMenuItem.CommandParameter = ThemeColors.Yellow;
        ColorOrangeMenuItem.Command = _changeThemeColorCommand;
        ColorOrangeMenuItem.CommandParameter = ThemeColors.Orange;
        ColorRedMenuItem.Command = _changeThemeColorCommand;
        ColorRedMenuItem.CommandParameter = ThemeColors.Red;
        ColorPinkMenuItem.Command = _changeThemeColorCommand;
        ColorPinkMenuItem.CommandParameter = ThemeColors.Pink;
        ColorPurpleMenuItem.Command = _changeThemeColorCommand;
        ColorPurpleMenuItem.CommandParameter = ThemeColors.Purple;
        ColorBlueMenuItem.Command = _changeThemeColorCommand;
        ColorBlueMenuItem.CommandParameter = ThemeColors.Blue;
        ColorAquaMenuItem.Command = _changeThemeColorCommand;
        ColorAquaMenuItem.CommandParameter = ThemeColors.Aqua;
        ColorGreenMenuItem.Command = _changeThemeColorCommand;
        ColorGreenMenuItem.CommandParameter = ThemeColors.Green;
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

    private void NoteWindow_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized && Topmost && !Properties.Settings.Default.AllowMinimizeWhenPinned)
            WindowState = WindowState.Normal;
    }

    private void NoteWindow_ActivatedChanged(object sender, EventArgs e)
    {
        if (IsActive)
            Topmost = true;
        else if (!_isPinned)
            Topmost = false;
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

    public void SelectAllCommandExecute()
    {
        NoteTextBox.SelectAll();
    }

    public void ClearCommandExecute()
    {
        NoteTextBox.Clear();
    }

    public void SaveCommandExecute()
    {
        SaveNote();
    }
    private void ChangeThemeColorCommandExecute(ThemeColors color)
    {
        SetColor(color);
    }

    #endregion

    #region MiscFunctions

    private void LoadSettings(ThemeColors? parentColor = null, Tuple<bool, bool>? parentGravity = null)
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
        AllowMinimizeWhenPinnedMenuItem.IsChecked = Properties.Settings.Default.AllowMinimizeWhenPinned;
        ColorCycleMenuItem.IsChecked = Properties.Settings.Default.CycleColors;
        SetColor(parentColor: parentColor);
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

    private void SetColor(ThemeColors? color = null, ThemeColors? parentColor = null)
    {
        if (color != null)
        {
            _noteCurrentTheme = (ThemeColors)color;
        }
        else if (!Properties.Settings.Default.CycleColors)
        {
            _noteCurrentTheme = (ThemeColors)Properties.Settings.Default.Color;
        }
        else
        {
            // Get the next color ensuring it is not the same as the parent notes color.
            List<ThemeColors> colors = _noteThemes.Keys.ToList();
            int nextColorIndex = colors.IndexOf((ThemeColors)Properties.Settings.Default.Color) + 1;
            // Get color thats not equal to parent and has a higher index, or return the first element by default.
            _noteCurrentTheme = colors.FirstOrDefault(c => c != parentColor && colors.IndexOf(c) >= nextColorIndex);
        }

        TitleBarGrid.Background = _noteThemes[_noteCurrentTheme].TitleBarColorBrush;
        Background = _noteThemes[_noteCurrentTheme].BackgroundColorBrush;
        BorderBrush = _noteThemes[_noteCurrentTheme].BorderColorBrush;

        Properties.Settings.Default.Color = (int)_noteCurrentTheme;
        Properties.Settings.Default.Save();

        // Tick correct menu item for color or note.
        foreach (object childObject in ColorsMenuItem.Items)
        {
            if (childObject is MenuItem)
            {
                MenuItem childMenuItem = (MenuItem)childObject;

                if (childMenuItem.CommandParameter == null)
                    continue;

                childMenuItem.IsEnabled = ((ThemeColors)childMenuItem.CommandParameter != _noteCurrentTheme);
            }
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
        new NoteWindow(
            Left,
            Top,
            _noteCurrentTheme,
            _noteGravity
        ).Show();
    }

    private void TopButton_Click(object sender, RoutedEventArgs e)
    {
        _isPinned = TopButton.IsChecked ?? false;
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

    #region Colors

    private void ColorCycleMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.CycleColors = ColorCycleMenuItem.IsChecked;
        Properties.Settings.Default.Save();
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

    private void AllowMinimizeWhenPinnedMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AllowMinimizeWhenPinned = AllowMinimizeWhenPinnedMenuItem.IsChecked;
        Properties.Settings.Default.Save();
    }

    #endregion

    #endregion


    #endregion

    #region TextBox

    private void NoteTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        _noteSaved = false;
        if (!Properties.Settings.Default.NewLine || NoteTextBox.Text == "" || NoteTextBox.Text.EndsWith(Environment.NewLine))
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
        // Only takeover event if auto indentation is active and return was pressed
        if (e.Key != Key.Return || !Properties.Settings.Default.AutoIndent)
            return;

        e.Handled = true;
        TextBox textBox = (TextBox)sender;

        // If there is selected text remove it and set caret to correct position
        if (textBox.SelectionLength > 0)
        {
            int selectionStart = textBox.SelectionStart;
            textBox.Text = textBox.Text.Remove(selectionStart, textBox.SelectionLength);
            textBox.CaretIndex = selectionStart;
        }

        // Store caret position for positioning later
        int caretIndex = textBox.CaretIndex;

        // Get the current line of text, trimming any new lines
        string line = textBox.GetLineText(textBox.GetLineIndexFromCharacterIndex(caretIndex)).TrimEnd(Environment.NewLine.ToCharArray());

        // Get the whitespace from the beginning of the line and create our indent string
        string preceedingWhitespace = new(line.TakeWhile(char.IsWhiteSpace).ToArray());
        string indent = Environment.NewLine + preceedingWhitespace;

        // Add the indent and restore caret position
        textBox.Text = textBox.Text.Insert(caretIndex, indent);
        textBox.CaretIndex = caretIndex + indent.Length;
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
                Command = new RelayCommand(SelectAllCommandExecute),
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
        return text.Split(_wordSeparators, StringSplitOptions.RemoveEmptyEntries).Length;
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