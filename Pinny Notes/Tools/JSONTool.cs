using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows.Controls;

namespace Pinny_Notes.Tools;

public class JsonTool(TextBox noteTextBox) : BaseTool(noteTextBox), ITool
{
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
                Command = new RelayCommand(JsonPrettifyAction)
            }
        );

        return menuItem;
    }

    private void JsonPrettifyAction()
    {
        ApplyFunctionToNoteText<bool?>(JsonPrettifyText);
    }

    private string JsonPrettifyText(string text, bool? additional = null)
    {
        try
        {
            object? jsonObject = JsonSerializer.Deserialize<object>(text);
            if (jsonObject != null)
                return JsonSerializer.Serialize<object>(jsonObject, _jsonSerializerOptions);
        }
        catch
        {
        }
        return text;
    }
}
