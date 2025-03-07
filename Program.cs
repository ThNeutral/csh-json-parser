var value = JSON.Serialize(new object?[] {12, null, true, new object?[] {123, true, null, new object?[] {123, true, null }}});

Console.WriteLine(value);
