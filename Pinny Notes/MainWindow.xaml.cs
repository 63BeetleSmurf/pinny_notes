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

namespace Pinny_Notes
{
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

        #region MainWindow

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            PositionNote();
#pragma warning disable CS4014
            CheckForNewVersion();
#pragma warning restore CS4014
        }

        public MainWindow(double parentLeft, double parentTop, string? parentColour, Tuple<bool, bool>? parentGravity)
        {
            InitializeComponent();
            LoadSettings(parentColour, parentGravity);
            PositionNote(parentLeft, parentTop);
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
            SpellCheckMenuItem.IsChecked = Properties.Settings.Default.SpellCheck;
            NoteTextBox.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
            NewLineMenuItem.IsChecked = Properties.Settings.Default.NewLine;
            DisableUpdateCheckMenuItem.IsChecked = Properties.Settings.Default.DisableUpdateCheck;
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

            // Not a setting but will do here for now
            TitleTextBox.ContextMenu = TitleBarGrid.ContextMenu;
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
                Properties.Settings.Default.DisableUpdateCheck
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
            saveFileDialog.FileName = TitleTextBox.Text;
            if (saveFileDialog.ShowDialog(this) == true)
            {
                File.WriteAllText(saveFileDialog.FileName, NoteTextBox.Text);
                return MessageBoxResult.OK;
            }
            return MessageBoxResult.Cancel;
        }

        private void ApplyFunctionToEachLine(Func<string, int, string?, string> function, string? additional = null)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = function(lines[i], i, additional);
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
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

        #region Title

        private void TitleTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TitleTextBox.Cursor = Cursors.IBeam;
            TitleTextBox.Focusable = true;
            TitleTextBox.Focus();
            // Should restore original context menu here if possible
        }

        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TitleTextBox.Cursor = Cursors.Arrow;
            TitleTextBox.Focusable = false;
            TitleTextBox.ContextMenu = TitleBarGrid.ContextMenu;
        }

        #endregion

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (NoteTextBox.Text != "")
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

        #endregion

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
            SortNoteText();
        }

        private void ListSortDecMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SortNoteText(true);
        }

        private void SortNoteText(bool reverse = false)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            Array.Sort(lines);
            if (reverse)
                Array.Reverse(lines);
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
        }

        #endregion

        #region JSON

        private void JSONPrettifyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteTextBox.Text = JsonConvert.SerializeObject(
                    JsonConvert.DeserializeObject(NoteTextBox.Text),
                    Formatting.Indented
                );
            }
            catch
            {
                return;
            }
        }

        #endregion

        #region Hash

        private void HashSHA512MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                NoteTextBox.Text = BitConverter.ToString(
                    sha512.ComputeHash(Encoding.UTF8.GetBytes(NoteTextBox.Text))
                ).Replace("-", "");
            }
        }

        private void HashSHA384MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                NoteTextBox.Text = BitConverter.ToString(
                    sha384.ComputeHash(Encoding.UTF8.GetBytes(NoteTextBox.Text))
                ).Replace("-", "");
            }
        }

        private void HashSHA256MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                NoteTextBox.Text = BitConverter.ToString(
                    sha256.ComputeHash(Encoding.UTF8.GetBytes(NoteTextBox.Text))
                ).Replace("-", "");
            }
        }

        private void HashSHA1MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                NoteTextBox.Text = BitConverter.ToString(
                    sha1.ComputeHash(Encoding.UTF8.GetBytes(NoteTextBox.Text))
                ).Replace("-", "");
            }
        }

        private void HashMD5MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (MD5 md5 = MD5.Create())
            {
                NoteTextBox.Text = BitConverter.ToString(
                    md5.ComputeHash(Encoding.UTF8.GetBytes(NoteTextBox.Text))
                ).Replace("-", "");
            }
        }

        #endregion

        #region Base64

        private void Base64EncodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(NoteTextBox.Text);
            NoteTextBox.Text = System.Convert.ToBase64String(textBytes);
        }

        private void Base64DecodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            byte[] base64Bytes = System.Convert.FromBase64String(NoteTextBox.Text);
            NoteTextBox.Text = System.Text.Encoding.UTF8.GetString(base64Bytes);
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

        private void DisableUpdateCheckMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DisableUpdateCheck = DisableUpdateCheckMenuItem.IsChecked;
            Properties.Settings.Default.Save();
        }

        #endregion

        #endregion

        #region TextBox

        private void NoteTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
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
                Clipboard.SetText(NoteTextBox.SelectedText.Trim());
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
                string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                NoteTextBox.Text = File.ReadAllText(filePath);
                TitleTextBox.Text = Path.GetFileNameWithoutExtension(filePath);
            }
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                NoteTextBox.Text = (string)e.Data.GetData(DataFormats.StringFormat);
            }
        }

        #endregion

    }
}
