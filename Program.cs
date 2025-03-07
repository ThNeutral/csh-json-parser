using System.Text.Json;

var value = JSON.ParseAny("0.123e12");

Console.WriteLine(JsonSerializer.Serialize(value));
