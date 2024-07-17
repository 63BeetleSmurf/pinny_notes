using System.Windows;
using Pinny_Notes.ViewModels;

namespace Pinny_Notes.Views;

public partial class SettingsWindow : Window
{
    private Window _lastOwner;

    public SettingsWindow()
    {
        DataContext = new SettingsViewModel();
        InitializeComponent();
        _lastOwner = Owner;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Activated(object sender, System.EventArgs e)
    {
        if (Owner == _lastOwner)
            return;

        _lastOwner = Owner;

        double centerOwnerX = Owner.Left + Owner.Width / 2;
        double centerOwnerY = Owner.Top + Owner.Height / 2;

        Left = centerOwnerX - Width / 2;
        Top = centerOwnerY - Height / 2;
    }
}
