using System.Text.Json;

var value = JSON.ParseAny("[false, true, null]");

Console.WriteLine(JsonSerializer.Serialize(value));
