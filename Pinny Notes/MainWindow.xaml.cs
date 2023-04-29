using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using System.Reflection;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls;
using System.Net;
using System.Windows.Documents;
using System.Globalization;
using System.Reflection.Emit;

namespace Pinny_Notes
{
    public class CustomCommand : ICommand
    {
        public Func<bool>? executeMethod { get; set; }

        public void Execute(object? parameter)
        {
            if (executeMethod != null)
                executeMethod();
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public event EventHandler? CanExecuteChanged;
    }

    public partial class MainWindow : Window
    {
        // Colour tuple = (Title Bar, Background, Border)
        Dictionary<string, (string, string, string)> NOTE_COLOURS = new Dictionary<string, (string, string, string)>{
            {"Yellow", ("#fef7b1", "#fffcdd", "#feea00")},
            {"Orange", ("#ffd179", "#fee8b9", "#ffab00")},
            {"Red",    ("#ff7c81", "#ffc4c6", "#e33036")},
            {"Pink",   ("#d986cc", "#ebbfe3", "#a72995")},
            {"Purple", ("#9d9add", "#d0cef3", "#625bb8")},
            {"Blue",   ("#7ac3e6", "#b3d9ec", "#1195dd")},
            {"Aqua",   ("#97cfc6", "#c0e2e1", "#16b098")},
            {"Green",  ("#c6d67d", "#e3ebc6", "#aacc04")}
        };

        string? NOTE_COLOUR = null;
        Tuple<bool, bool>? NOTE_GRAVITY = null;
        bool NOTE_SAVED = false;

        CustomCommand COPY_COMMAND = new();
        CustomCommand CUT_COMMAND = new();
        CustomCommand PASTE_COMMAND = new();

        #region MainWindow

        public MainWindow()
        {
            MainWindowInitialize();
            LoadSettings();
            PositionNote();
#pragma warning disable CS4014
            CheckForNewVersion();
#pragma warning restore CS4014
        }

        public MainWindow(double parentLeft, double parentTop, string? parentColour, Tuple<bool, bool>? parentGravity)
        {
            MainWindowInitialize();
            LoadSettings(parentColour, parentGravity);
            PositionNote(parentLeft, parentTop);
        }

        private void MainWindowInitialize()
        {
            InitializeComponent();
            NoteTextBox.ContextMenu = GetNoteTextBoxContextMenu();

            COPY_COMMAND.executeMethod = NoteTextBox_Copy;
            NoteTextBox.InputBindings.Add(new InputBinding(COPY_COMMAND, new KeyGesture(Key.C, ModifierKeys.Control)));
            CUT_COMMAND.executeMethod = NoteTextBox_Cut;
            NoteTextBox.InputBindings.Add(new InputBinding(CUT_COMMAND, new KeyGesture(Key.X, ModifierKeys.Control)));
            PASTE_COMMAND.executeMethod = NoteTextBox_Paste;
            NoteTextBox.InputBindings.Add(new InputBinding(PASTE_COMMAND, new KeyGesture(Key.V, ModifierKeys.Control)));
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
                NOTE_GRAVITY = new Tuple<bool, bool>(
                    gravityLeft,
                    gravityTop
                );
            }
        }

        #endregion

        #region MiscFunctions

        private void LoadSettings(string? parentColour = null, Tuple<bool, bool>? parentGravity = null)
        {
            AutoCopyMenuItem.IsChecked = Properties.Settings.Default.AutoCopy;
            TrimCopiedTextMenuItem.IsChecked = Properties.Settings.Default.TrimCopiedText;
            TrimPastedTextMenuItem.IsChecked = Properties.Settings.Default.TrimPastedText;
            SpellCheckMenuItem.IsChecked = Properties.Settings.Default.SpellCheck;
            NoteTextBox.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
            NewLineMenuItem.IsChecked = Properties.Settings.Default.NewLine;
            CheckForUpdatesMenuItem.IsChecked = Properties.Settings.Default.CheckForUpdates;
            ColourCycleMenuItem.IsChecked = Properties.Settings.Default.CycleColours;
            SetColour(parentColour: parentColour);
            if (parentGravity == null)
                NOTE_GRAVITY = new Tuple<bool, bool>(
                    Properties.Settings.Default.StartupPositionLeft,
                    Properties.Settings.Default.StartupPositionTop
                );
            else
                NOTE_GRAVITY = parentGravity;

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

        private void SetColour(string? colour = null, string? parentColour = null)
        {
            if (colour == null)
            {
                if (Properties.Settings.Default.CycleColours)
                {
                    // Get the next colour ensuring it is not the same as the parent notes colour.
                    string? nextColour = null;
                    int nextColourIndex = NOTE_COLOURS.Keys.ToList().IndexOf(Properties.Settings.Default.Colour) + 1;
                    while (nextColour == null)
                    {
                        nextColour = NOTE_COLOURS.Keys.ElementAtOrDefault(nextColourIndex);
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
                    colour = nextColour;
                }
                else
                {
                    colour = Properties.Settings.Default.Colour;
                }
            }

            BrushConverter brushConverter = new BrushConverter();
            object? titleBrush = brushConverter.ConvertFromString(NOTE_COLOURS[colour].Item1);
            object? bodyBrush = brushConverter.ConvertFromString(NOTE_COLOURS[colour].Item2);
            object? borderBrush = brushConverter.ConvertFromString(NOTE_COLOURS[colour].Item3);

#pragma warning disable CS8600
            TitleBarGrid.Background = (Brush)titleBrush;
            Background = (Brush)bodyBrush;
            BorderBrush = (Brush)borderBrush;
#pragma warning restore CS8600

            NOTE_COLOUR = colour;
            Properties.Settings.Default.Colour = colour;
            Properties.Settings.Default.Save();

            // Tick correct menu item for colour or note.
            foreach (object childObject in ColoursMenuItem.Items)
            {
                if (childObject.GetType().Name != "MenuItem")
                    continue;

                MenuItem childMenuItem = (MenuItem)childObject;
                if (childMenuItem.Header.ToString() != "Cycle" && childMenuItem.Header.ToString() != colour)
                    childMenuItem.IsChecked = false;
                else if (childMenuItem.Header.ToString() == colour)
                    childMenuItem.IsChecked = true;
            }
        }

        private void PositionNote(double? parentLeft = null, double? parentTop = null)
        {
            double positionTop = 0;
            double positionLeft = 0;
#pragma warning disable CS8602
            // If there is no parent, position relative to screen
            if (parentLeft == null || parentTop == null)
            {
                int screenMargin = 78;
                if (NOTE_GRAVITY.Item1) // Left
                    positionLeft = screenMargin;
                else // Right
                    positionLeft = (SystemParameters.PrimaryScreenWidth - screenMargin) - Width;

                if (NOTE_GRAVITY.Item2) // Top
                    positionTop = screenMargin;
                else // Bottom
                    positionTop = (SystemParameters.PrimaryScreenHeight - screenMargin) - Height;
            }
            // Position relative to parent
            else
            {
                if (NOTE_GRAVITY.Item1)
                    positionLeft = (double)parentLeft + 45;
                else
                    positionLeft = (double)parentLeft - 45;

                if (NOTE_GRAVITY.Item2)
                    positionTop = (double)parentTop + 45;
                else
                    positionTop = (double)parentTop - 45;
            }
#pragma warning restore CS8602

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

        private async Task CheckForNewVersion()
        {
            DateTime lastUpdateCheck = Properties.Settings.Default.LastUpdateCheck;
            if (
                Properties.Settings.Default.CheckForUpdates
                || DateTime.Now.Subtract(lastUpdateCheck).Days < 7
            )
                return;

            GitHubClient client = new GitHubClient(new ProductHeaderValue("pinny_notes"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("63BeetleSmurf", "pinny_notes");

            Version latestVersion = new Version(releases[0].TagName.Replace("v", "") + ".0");
            Version? localVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (localVersion is null)
                return;

            if (localVersion.CompareTo(latestVersion) < 0)
                MessageBox.Show(
                    "A new version of Pinny Notes is available.\n\nGet the latest release from;\nhttps://github.com/63BeetleSmurf/pinny_notes/releases",
                    "Update Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

            Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        private MessageBoxResult SaveNote()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Documents (*.txt)|*.txt|All Files|*";
            if (saveFileDialog.ShowDialog(this) == true)
            {
                File.WriteAllText(saveFileDialog.FileName, NoteTextBox.Text);
                NOTE_SAVED = true;
                return MessageBoxResult.OK;
            }
            return MessageBoxResult.Cancel;
        }

        private void ApplyFunctionToNoteText(Func<string, string?, string> function, string? additional = null)
        {
            if (NoteTextBox.SelectionLength > 0)
                NoteTextBox.SelectedText = function(NoteTextBox.SelectedText, additional);
            else
            {
                string noteText = NoteTextBox.Text;
                // Ignore trailing new line if it was automatically added
                if (Properties.Settings.Default.NewLine && NoteTextBox.Text.EndsWith(Environment.NewLine))
                    noteText = noteText.Remove(noteText.Length - Environment.NewLine.Length);
                NoteTextBox.Text = function(noteText, additional);
                if (NoteTextBox.Text.Length > 0)
                    NoteTextBox.CaretIndex = NoteTextBox.Text.Length - 1;
            }
        }

        private void ApplyFunctionToEachLine(Func<string, int, string?, string> function, string? additional = null)
        {
            string[] lines;

            if (NoteTextBox.SelectionLength > 0)
                lines = NoteTextBox.SelectedText.Split(Environment.NewLine);
            else
                lines = NoteTextBox.Text.Split(Environment.NewLine);

            int lineCount = lines.Length;
            // Ignore trailing new line if it was automatically added
            if (Properties.Settings.Default.NewLine && lines[lineCount - 1] == "")
                lineCount--;
            for (int i = 0; i < lineCount; i++)
                lines[i] = function(lines[i], i, additional);

            if (NoteTextBox.SelectionLength > 0)
                NoteTextBox.SelectedText = string.Join(Environment.NewLine, lines);
            else
            {
                NoteTextBox.Text = string.Join(Environment.NewLine, lines);
                if (NoteTextBox.Text.Length > 0)
                    NoteTextBox.CaretIndex = NoteTextBox.Text.Length - 1;
            }
        }

        #endregion

        #region TitleBar

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(
                Left,
                Top,
                NOTE_COLOUR,
                NOTE_GRAVITY
            ).Show();
        }

        private void TopButton_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            if (Topmost)
                TopButtonImage.Source = (ImageSource)Resources[(object)"PinImageSource"];
            else
                TopButtonImage.Source = (ImageSource)Resources[(object)"Pin45ImageSource"];
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NOTE_SAVED && NoteTextBox.Text != "")
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

        private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NoteTextBox.Clear();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveNote();
        }

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

            SetColour(menuItem.Header.ToString());
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

#pragma warning disable CS8602
            string[] position = menuItem.Header.ToString().Split(" ");
#pragma warning restore CS8602
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

        private void SpellCheckMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NoteTextBox.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
            Properties.Settings.Default.SpellCheck = SpellCheckMenuItem.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void NewLineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.NewLine = NewLineMenuItem.IsChecked;
            Properties.Settings.Default.Save();

            // Check for new line when this option is activated
            if (Properties.Settings.Default.NewLine)
                NoteTextBox_TextChanged(sender, e);
        }

        private void CheckForUpdatesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CheckForUpdates = CheckForUpdatesMenuItem.IsChecked;
            Properties.Settings.Default.Save();
        }

        #endregion

        #endregion


        #endregion

        #region TextBox

        private void NoteTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            NOTE_SAVED = false;
            if (Properties.Settings.Default.NewLine && NoteTextBox.Text != "" && !NoteTextBox.Text.EndsWith(Environment.NewLine))
            {
                // Preserving selection when adding new line
                int selectionStart = NoteTextBox.SelectionStart;
                int selectionLength = NoteTextBox.SelectionLength;

                NoteTextBox.Text += Environment.NewLine;

                NoteTextBox.SelectionStart = selectionStart;
                NoteTextBox.SelectionLength = selectionLength;
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
            NoteTextBox.Text = NoteTextBox.Text.Substring(0, selectionStart) + NoteTextBox.Text.Substring(selectionStart + NoteTextBox.SelectionLength);
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
                    NoteTextBox.Text = NoteTextBox.Text.Substring(0, caretIndex) + clipboardString + NoteTextBox.Text.Substring(caretIndex);
                    NoteTextBox.CaretIndex = caretIndex + clipboardString.Length;
                }
                else
                {
                    int selectionStart = NoteTextBox.SelectionStart;
                    NoteTextBox.Text = NoteTextBox.Text.Substring(0, selectionStart) + clipboardString + NoteTextBox.Text.Substring(selectionStart + NoteTextBox.SelectionLength);
                    NoteTextBox.CaretIndex = selectionStart + clipboardString.Length;
                }
            }
            return true;
        }

        #region ContextMenu

        private ContextMenu GetNoteTextBoxContextMenu()
        {
            ContextMenu menu = new ContextMenu();

            int caretIndex = NoteTextBox.CaretIndex;
            SpellingError spellingError = NoteTextBox.GetSpellingError(caretIndex);
            if (spellingError != null)
            {
                if (spellingError.Suggestions.Count() == 0)
                    menu.Items.Add(
                            CreateMenuItem(
                                header: "(no spelling suggestions)",
                                enabled: false
                            )
                        );
                else
                    foreach (string spellingSuggestion in spellingError.Suggestions)
                    {
                        menu.Items.Add(
                            CreateMenuItem(
                                header: spellingSuggestion,
                                headerBold: true,
                                command: EditingCommands.CorrectSpellingError,
                                commandParameter: spellingSuggestion,
                                commandTarget: NoteTextBox
                            )
                        );
                    }
                menu.Items.Add(new Separator());
            }

            menu.Items.Add(
                CreateMenuItem(
                    header: "Copy",
                    command: COPY_COMMAND,
                    inputGestureText: "Ctrl+C",
                    enabled: (NoteTextBox.SelectionLength > 0)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "Cut",
                    command: CUT_COMMAND,
                    inputGestureText: "Ctrl+X",
                    enabled: (NoteTextBox.SelectionLength > 0)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "Paste",
                    command: PASTE_COMMAND,
                    inputGestureText: "Ctrl+V",
                    enabled: (Clipboard.ContainsText())
                )
            );
            
            menu.Items.Add(new Separator());

            menu.Items.Add(
                CreateMenuItem(
                    header: "Select All",
                    clickEventHandler: new RoutedEventHandler(SelectAllMenuItem_Click),
                    enabled: (NoteTextBox.Text.Length > 0)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "Clear",
                    clickEventHandler: new RoutedEventHandler(ClearMenuItem_Click),
                    enabled: (NoteTextBox.Text.Length > 0)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "Save",
                    clickEventHandler: new RoutedEventHandler(SaveMenuItem_Click),
                    enabled: (NoteTextBox.Text.Length > 0)
                )
            );

            menu.Items.Add(new Separator());

            menu.Items.Add(
                CreateMenuItem(
                    header: "▸ Tools",
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Base64",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Encode",
                    clickEventHandler: new RoutedEventHandler(Base64EncodeMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Decode",
                    clickEventHandler: new RoutedEventHandler(Base64DecodeMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Case",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Lower",
                    clickEventHandler: new RoutedEventHandler(CaseLowerMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Upper",
                    clickEventHandler: new RoutedEventHandler(CaseUpperMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Proper",
                    clickEventHandler: new RoutedEventHandler(CaseProperMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Hash",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ SHA512",
                    clickEventHandler: new RoutedEventHandler(HashSHA512MenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ SHA384",
                    clickEventHandler: new RoutedEventHandler(HashSHA384MenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ SHA256",
                    clickEventHandler: new RoutedEventHandler(HashSHA256MenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ SHA1",
                    clickEventHandler: new RoutedEventHandler(HashSHA1MenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ MD5",
                    clickEventHandler: new RoutedEventHandler(HashMD5MenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ HTML Entity",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Encode",
                    clickEventHandler: new RoutedEventHandler(HTMLEntityEncodeMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Decode",
                    clickEventHandler: new RoutedEventHandler(HTMLEntityDecodeMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Indent",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ 2 Spaces",
                    clickEventHandler: new RoutedEventHandler(Indent2SpacesMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ 4 Spaces",
                    clickEventHandler: new RoutedEventHandler(Indent4SpacesMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Tab",
                    clickEventHandler: new RoutedEventHandler(IndentTabMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Join",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Comma",
                    clickEventHandler: new RoutedEventHandler(JoinCommaMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Space",
                    clickEventHandler: new RoutedEventHandler(JoinSpaceMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Tab",
                    clickEventHandler: new RoutedEventHandler(JoinTabMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ JSON",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Prettify",
                    clickEventHandler: new RoutedEventHandler(JSONPrettifyMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ List",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Enumerate",
                    clickEventHandler: new RoutedEventHandler(ListEnumerateMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Sort Asc.",
                    clickEventHandler: new RoutedEventHandler(ListSortAscMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Sort Des.",
                    clickEventHandler: new RoutedEventHandler(ListSortDecMenuItem_Click),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Split",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Comma",
                    clickEventHandler: new RoutedEventHandler(SplitCommaMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Space",
                    clickEventHandler: new RoutedEventHandler(SplitSpaceMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Tab",
                    clickEventHandler: new RoutedEventHandler(SplitTabMenuItem_Click),
                    visible: false
                )
            );
            //menu.Items.Add(new Separator());
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Selected",
                    clickEventHandler: new RoutedEventHandler(SplitSelectedMenuItem_Click),
                    enabled: (NoteTextBox.SelectionLength > 0),
                    visible: false
                )
            );

            menu.Items.Add(
                CreateMenuItem(
                    header: "└ ▸ Trim",
                    visible: false,
                    staysOpenOnClick: true,
                    clickEventHandler: new RoutedEventHandler(ParentMenuItem_Click)
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Start",
                    clickEventHandler: new RoutedEventHandler(TrimStartMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ End",
                    clickEventHandler: new RoutedEventHandler(TrimEndMenuItem_Click),
                    visible: false
                )
            );
            menu.Items.Add(
                CreateMenuItem(
                    header: "  └ Both",
                    clickEventHandler: new RoutedEventHandler(TrimBothMenuItem_Click),
                    visible: false
                )
            );

            return menu;
        }

        private MenuItem CreateMenuItem(string header, bool headerBold = false, bool enabled = true,
            RoutedEventHandler? clickEventHandler = null, List<object>? children = null,
            ICommand? command = null, object? commandParameter = null, IInputElement? commandTarget = null,
            string? inputGestureText = null, bool staysOpenOnClick = false, bool visible = true
            )
        {
            MenuItem menuItem = new();
            
            menuItem.Header = header;
            if (headerBold)
                menuItem.FontWeight = FontWeights.Bold;
            
            if (clickEventHandler != null)
                menuItem.Click += clickEventHandler;

            if (children != null)
                foreach (object child in children)
                    menuItem.Items.Add(child);

            if (!enabled)
                menuItem.IsEnabled = false;

            if (command != null)
                menuItem.Command = command;
            if (commandParameter!= null)
                menuItem.CommandParameter = commandParameter;
            if (commandTarget != null)
                menuItem.CommandTarget = commandTarget;

            if (inputGestureText != null)
                menuItem.InputGestureText = inputGestureText;

            menuItem.StaysOpenOnClick = staysOpenOnClick;

            if (!visible)
                menuItem.Visibility = Visibility.Collapsed;

            return menuItem;
        }

        private void NoteTextBox_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            NoteTextBox.ContextMenu = GetNoteTextBoxContextMenu();
        }

        private void ParentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ItemCollection contextMenuItems = NoteTextBox.ContextMenu.Items;
            int index = contextMenuItems.IndexOf(sender);

            MenuItem senderMenuItem = (MenuItem)sender;
            string header = senderMenuItem.Header.ToString();

            if (header.Contains("▸"))
            {
                int level = header.IndexOf("▸");

                senderMenuItem.Header = header.Replace("▸", "▾");
                index++;
                while (index < contextMenuItems.Count
                    && ((MenuItem)contextMenuItems[index]).Visibility == Visibility.Collapsed
                    && (
                        ((MenuItem)contextMenuItems[index]).Header.ToString()[level].ToString() == "└"
                        || ((MenuItem)contextMenuItems[index]).Header.ToString()[level].ToString() == " ")
                    )
                {
                    if (((MenuItem)contextMenuItems[index]).Header.ToString()[level].ToString() != " ")
                        ((MenuItem)contextMenuItems[index]).Visibility = Visibility.Visible;
                    index++;
                }
            }
            else if (header.Contains("▾"))
            {
                int level = header.IndexOf("▾");

                senderMenuItem.Header = header.Replace("▾", "▸");
                index++;
                while (
                    index < contextMenuItems.Count
                    && (
                        ((MenuItem)contextMenuItems[index]).Header.ToString()[level].ToString() == "└"
                        || ((MenuItem)contextMenuItems[index]).Header.ToString()[level].ToString() == " ")
                    )
                {
                    ((MenuItem)contextMenuItems[index]).Visibility = Visibility.Collapsed;
                    ((MenuItem)contextMenuItems[index]).Header = ((MenuItem)contextMenuItems[index]).Header.ToString().Replace("▾", "▸");
                    index++;
                }
            }
        }

        #region SelectAll Clear Save

        private void SelectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NoteTextBox.SelectionStart = 0;
            NoteTextBox.SelectionLength = NoteTextBox.Text.Length;
        }

        #endregion

        #region Base64

#pragma warning disable CS8622

        private void Base64EncodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(Base64EncodeText);
        }

        private void Base64DecodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(Base64DecodeText);
        }

#pragma warning restore CS8622

        private string Base64EncodeText(string text, string? additional = null)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }

        private string Base64DecodeText(string text, string? additional = null)
        {
            byte[] base64Bytes = System.Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }

        #endregion

        #region Case

#pragma warning disable CS8622
        private void CaseLowerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(SetTextCase, "l");
        }

        private void CaseUpperMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(SetTextCase, "u");
        }

        private void CaseProperMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(SetTextCase, "p");
        }
#pragma warning restore CS8622

        private string SetTextCase(string line, int index, string textCase)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            switch (textCase)
            {
                case "u":
                    return textInfo.ToUpper(line);
                case "p":
                    return textInfo.ToTitleCase(
                        textInfo.ToLower(line)
                    );
                default:
                    return textInfo.ToLower(line);
            }
        }

        #endregion

        #region Hash

#pragma warning disable CS8622

        private void HashSHA512MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HashText, "sha512");
        }

        private void HashSHA384MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HashText, "sha384");
        }

        private void HashSHA256MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HashText, "sha256");
        }

        private void HashSHA1MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HashText, "sha1");
        }

        private void HashMD5MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HashText, "md5");
        }

#pragma warning restore CS8622

        private string HashText(string text, string algorithm)
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
                hasher.ComputeHash(Encoding.UTF8.GetBytes(NoteTextBox.Text))
            ).Replace("-", "");
        }

        #endregion

        #region HTML Entity

#pragma warning disable CS8622

        private void HTMLEntityEncodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HTMLEntityEncodeText);
        }

        private void HTMLEntityDecodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(HTMLEntityDecodeText);
        }

#pragma warning restore CS8622

        private string HTMLEntityEncodeText(string text, string? additional = null)
        {
            return WebUtility.HtmlEncode(text);
        }
        private string HTMLEntityDecodeText(string text, string? additional = null)
        {
            return WebUtility.HtmlDecode(text);
        }

        #endregion

        #region Indent

#pragma warning disable CS8622
        private void Indent2SpacesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(IndentText, "  ");
        }

        private void Indent4SpacesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(IndentText, "    ");
        }

        private void IndentTabMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(IndentText, "\t");
        }
#pragma warning restore CS8622

        private string IndentText(string line, int index, string indentString)
        {
            return indentString + line;
        }

        #endregion

        #region Join

#pragma warning disable CS8622

        private void JoinCommaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(JoinText, ",");
        }

        private void JoinSpaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(JoinText, " ");
        }

        private void JoinTabMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(JoinText, "\t");
        }

#pragma warning restore CS8622

        private string JoinText(string text, string joinString)
        {
            return text.Replace(Environment.NewLine, joinString);
        }

        #endregion

        #region JSON

        private void JSONPrettifyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(JSONPrettifyText);
        }

        private string JSONPrettifyText(string text, string? additional = null)
        {
            try
            {
                return JsonConvert.SerializeObject(
                    JsonConvert.DeserializeObject(text),
                    Formatting.Indented
                );
            }
            catch
            {
                return text;
            }
        }

        #endregion

        #region List

        private void ListEnumerateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(EnumerateLine);
        }

        private string EnumerateLine(string line, int index, string? additional)
        {
            return (index + 1).ToString() + ". " + line;
        }

        private void ListSortAscMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(SortNoteText);
        }

        private void ListSortDecMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToNoteText(SortNoteText, "rev");
        }

        private string SortNoteText(string text, string? reverse = null)
        {
            string[] lines = text.Split(Environment.NewLine);
            Array.Sort(lines);
            if (reverse == "rev")
                Array.Reverse(lines);
            return string.Join(Environment.NewLine, lines);
        }

        #endregion

        #region Split

#pragma warning disable CS8622

        private void SplitCommaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(SplitText, ",");
        }

        private void SplitSpaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(SplitText, " ");
        }

        private void SplitTabMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(SplitText, "\t");
        }

        private void SplitSelectedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string splitString = NoteTextBox.SelectedText;
            NoteTextBox.SelectionLength = 0;
            ApplyFunctionToEachLine(SplitText, splitString);
        }

#pragma warning restore CS8622

        private string SplitText(string line, int index, string splitString)
        {
            return line.Replace(splitString, Environment.NewLine);
        }

        #endregion

        #region Trim

#pragma warning disable CS8622

        private void TrimStartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(TrimText, "Start");
        }

        private void TrimEndMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(TrimText, "End");
        }

        private void TrimBothMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyFunctionToEachLine(TrimText, "Both");
        }

#pragma warning restore CS8622

        private string TrimText(string line, int index, string trimType)
        {
            switch (trimType)
            {
                case "Start":
                    return line.TrimStart();
                case "End":
                    return line.TrimEnd();
                case "Both":
                    return line.Trim();
                default:
                    return line;

            }
        }

        #endregion

        #endregion

        #endregion

    }
}
