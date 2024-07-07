using System;
using System.Windows.Input;

namespace Pinny_Notes.Commands;

public class CustomCommand : ICommand
{
    public Func<bool>? ExecuteMethod { get; set; }

    public void Execute(object? parameter)
    {
        if (ExecuteMethod != null)
            _ = ExecuteMethod();
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public event EventHandler? CanExecuteChanged;
}
