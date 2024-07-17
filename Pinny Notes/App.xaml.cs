using Pinny_Notes.Views;
using System.Windows;

namespace Pinny_Notes;

public partial class App : Application
{
    private SettingsWindow? _settingsWindow;

    public void ShowSettingsWindow(Window owner)
    {
        if (_settingsWindow == null || !_settingsWindow.IsLoaded)
            _settingsWindow = new SettingsWindow();

        _settingsWindow.Owner = owner;

        if (_settingsWindow.IsVisible)
            _settingsWindow.Activate();
        else
            _settingsWindow.Show();
    }
}
