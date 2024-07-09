using System.Text.Json;
using System.Windows.Controls;
using Pinny_Notes.Commands;

namespace Pinny_Notes.Tools;

public class JsonTool : BaseTool, ITool
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public JsonTool(TextBox noteTextBox) : base(noteTextBox)
    {
        MenuItem = new()
        {
            Header = "JSON",
        };
        MenuItem.Items.Add(
            new MenuItem()
            {
                Header = "Prettify",
                Command = new CustomCommand() { ExecuteMethod = JsonPrettifyAction }
            }
        );
    }

    private bool JsonPrettifyAction()
    {
        ApplyFunctionToNoteText(JsonPrettifyText);
        return true;
    }

    private string JsonPrettifyText(string text, string? additional = null)
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
