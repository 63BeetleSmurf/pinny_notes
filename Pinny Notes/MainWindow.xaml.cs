﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

namespace Pinny_Notes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AutoCopyMenuItem.IsChecked = Properties.Settings.Default.AutoCopy;
            SpellCheckMenuItem.IsChecked = Properties.Settings.Default.SpellCheck;
            NoteTextBox.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
        }

        public MainWindow(double left, double top)
        {
            InitializeComponent();
            
            Left = left;
            Top = top;
        }

        #region TitleBar
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            int gravityLeft;
            int gravityTop;

            if (this.Left > SystemParameters.PrimaryScreenWidth / 2)
                gravityLeft = -1;
            else
                gravityLeft = 1;

            if (this.Top > SystemParameters.PrimaryScreenHeight / 2)
                gravityTop = -1;
            else
                gravityTop = 5; // Leave extra room to keep title bar visible

            new MainWindow(
                this.Left + (10 * gravityLeft),
                this.Top + (10 * gravityTop)
            ).Show();
        }

        private void TopButton_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            if (Topmost)
            {
                PinImage.Visibility = Visibility.Visible;
                Pin45Image.Visibility = Visibility.Hidden;
            }
            else
            {
                PinImage.Visibility = Visibility.Hidden;
                Pin45Image.Visibility = Visibility.Visible;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (NoteTextBox.Text != "")
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "Do you want to save this note?",
                    "Pinny Notes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );
                if (
                    (messageBoxResult == MessageBoxResult.Yes && SaveNote() == MessageBoxResult.Cancel)
                    || messageBoxResult == MessageBoxResult.Cancel
                )
                    return;
            }
            Close();
        }
        #endregion

        #region ContectMenu
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveNote();
        }

        private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NoteTextBox.Clear();
        }

        #region Indent
        private void Indent2SpacesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IndentNoteText("  ");
        }
        private void Indent4SpacesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IndentNoteText("    ");
        }

        private void IndentTabMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IndentNoteText("\t");
        }

        private void IndentNoteText(string indentString)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = indentString + lines[i];
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
        }
        #endregion

        #region Trim
        private void TrimStartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].TrimStart(); 
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
        }
        private void TrimEndMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].TrimEnd();
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
        }
        private void TrimBothMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Trim();
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
        }
        #endregion

        #region List
        private void ListEnumerateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = NoteTextBox.Text.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
                lines[i] = (i + 1).ToString() + ". " + lines[i];
            NoteTextBox.Text = string.Join(Environment.NewLine, lines);
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
            string noteText = NoteTextBox.Text;
            try
            {
                NoteTextBox.Text = JsonConvert.SerializeObject(
                    JsonConvert.DeserializeObject(noteText),
                    Formatting.Indented
                );
            }
            catch
            {
                NoteTextBox.Text = noteText;
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
        #endregion

        private MessageBoxResult SaveNote()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Documents (*.txt)|*.txt|All Files|*";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, NoteTextBox.Text);
                return MessageBoxResult.OK;
            }
            return MessageBoxResult.Cancel;
        }

        private void NoteTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.AutoCopy && NoteTextBox.SelectionLength > 0)
                Clipboard.SetText(NoteTextBox.SelectedText.Trim());
        }
    }
}
