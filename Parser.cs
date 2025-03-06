public class Parser(string source) {
    private class ParseException(string message) : Exception(message) {}
    private enum LiteralType {
        TRUE,
        FALSE,
        NULL,
        INVALID
    }
    private string source = source;
    private int current = 0;
    public dynamic? ParseAny() {
        SkipWhitespace();
        return Value();
    }
    private dynamic? Value() {
        var next = Peek();
        switch(next) {
            case '{': return Object();
            case '[': return Array();
            case '"': return String();
            default: {
                if (char.IsAsciiDigit(next) || next == '-') {
                    return Number();
                }

                return Literal() switch
                {
                    LiteralType.TRUE => true,
                    LiteralType.FALSE => false,
                    LiteralType.NULL => null,
                    _ => throw new ParseException("Encountered unexpected token: '" + next + "'."),
                };
            }
        }
    }
    private dynamic Object() {
        throw new NotImplementedException();
    }
    private List<object?> Array() {
        var array = new List<object?>();

        Advance();
        SkipWhitespace();

        if (Peek() == ']') {
            Advance();
            return array;
        }
        
        while (true) {
            array.Add(Value());
            SkipWhitespace();

            if (Peek() == ']') {
                Advance();
                break;
            }

            if (Peek() != ',')  {
                throw new ParseException("Expected ',' between array elements");
            }
        
            Advance();
            SkipWhitespace();
        }
        
        return array;
    }
    private string String() {
        throw new NotImplementedException();
    }
    private float Number() {
        throw new NotImplementedException();    
    }
    private LiteralType Literal() {
        if (MatchWord("true")) return LiteralType.TRUE;
        if (MatchWord("false")) return LiteralType.FALSE;
        if (MatchWord("null")) return LiteralType.NULL;
        return LiteralType.INVALID;
    }
    private void SkipWhitespace() {
        while(Match(' ', '\n', '\r', '\t') && !IsAtEnd()) Advance(); 
    }
    private bool Match(params char[] targets) {
        return targets.Contains(Peek());
    }
    private bool MatchWord(string word) {
        var start = current;
        foreach (var letter in word) {
            if (Peek() != letter) {
                current = start;
                return false;
            }
            Advance();
        }
        return true;
    }
    private void Advance() {
        if (IsAtEnd()) return;
        current += 1;
    }
    private bool IsAtEnd() {
        return current >= source.Length - 1;
    }
    private char Peek() {
        return source[current];
    }
}