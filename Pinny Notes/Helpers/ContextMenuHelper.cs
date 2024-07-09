using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinny_Notes.Helpers;

public static class ContextMenuHelper
{
    public static MenuItem CreateMenuItem(string header, bool headerBold = false, bool enabled = true,
        RoutedEventHandler? clickEventHandler = null, List<object>? children = null,
        ICommand? command = null, object? commandParameter = null, IInputElement? commandTarget = null,
        string? inputGestureText = null
        )
    {
        MenuItem menuItem = new()
        {
            Header = header
        };
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
        if (commandParameter != null)
            menuItem.CommandParameter = commandParameter;
        if (commandTarget != null)
            menuItem.CommandTarget = commandTarget;

        if (inputGestureText != null)
            menuItem.InputGestureText = inputGestureText;

        return menuItem;
    }
}
