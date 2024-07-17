using System;
using System.Windows.Input;

namespace Pinny_Notes.Tools;

public class ToolMenuAction(string name, ICommand? command = null, Enum? action = null)
{
    public string Name { get; set; } = name;
    public ICommand? Command { get; set; } = command;
    public Enum? Action { get; set; } = action;
}
