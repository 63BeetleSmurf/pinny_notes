using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public partial class JsonTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
    public enum ToolActions
    {
        JsonPrettify
    }

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public MenuItem GetMenuItem()
    {
        MenuItem menuItem = new()
        {
            Header = "JSON",
        };

        menuItem.Items.Add(
            new MenuItem()
            {
                Header = "Prettify",
                Command = MenuActionCommand,
                CommandParameter = ToolActions.JsonPrettify
            }
        );

        return menuItem;
    }

    [RelayCommand]
    private void MenuAction(ToolActions action)
    {
        ApplyFunctionToNoteText(ModifyTextCallback, action);
    }

    private string ModifyTextCallback(string text, ToolActions action)
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
