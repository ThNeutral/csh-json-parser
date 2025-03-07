var value = JSON.Serialize(new Class());

Console.WriteLine(value);

class Class {
    public int Id = 42;
    public string Name = "Hello";   
}