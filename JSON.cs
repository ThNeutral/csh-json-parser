public static class JSON {
    public static dynamic? ParseAny(string source) {
        if (source == null) return null;
        return new Parser(source).ParseAny();
    }
}