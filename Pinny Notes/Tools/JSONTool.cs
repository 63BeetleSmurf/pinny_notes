using CommunityToolkit.Mvvm.Input;
using System;
using System.Text.Json;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class JsonTool : BaseTool, ITool
{
    public enum ToolActions
    {
        JsonPrettify
    }

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public JsonTool(TextBox noteTextBox) : base(noteTextBox)
    {
        _name = "JSON";
        _menuActions.Add(new("Prettify", MenuActionCommand, ToolActions.JsonPrettify));
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, Enum action)
    {
        try
        {
            switch (action)
            {
                case ToolActions.JsonPrettify:
                    object? jsonObject = JsonSerializer.Deserialize<object>(text);
                    if (jsonObject != null)
                        return JsonSerializer.Serialize<object>(jsonObject, _jsonSerializerOptions);
                    break;
            }
        }
        catch
        {
        }

        return text;
    }
}
