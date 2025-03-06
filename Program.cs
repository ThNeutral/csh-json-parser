using System.Text.Json;

var value = JSON.ParseAny("[false, true, \"null\\u0022\"]");

Console.WriteLine(JsonSerializer.Serialize(value));
