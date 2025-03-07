public static class JSON {
    public static object? ParseAny(string source) {
        if (source == null) return null;
        return new Parser(source).ParseAny();
    }
    public static string Serialize(object? source) {
        return new Serializer(source).Serialize();
    }
}