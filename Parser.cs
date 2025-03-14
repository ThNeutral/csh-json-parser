public class Parser(string source) {
    public class ParseException(string message) : Exception(message) {}
    private enum LiteralType {
        TRUE,
        FALSE,
        NULL,
        INVALID
    }
    private string source = source;
    private int current = 0;
    public object? ParseAny() {
        current = 0;
        SkipWhitespace();
        return Value();
    }
    private object? Value() {
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
    private Dictionary<string, object?> Object() {
        var dict = new Dictionary<string, object?>();

        Advance();
        SkipWhitespace();

        if (Peek() == '}') {
            Advance();
            return dict;
        }

        while (true) {
            if (IsAtEnd()) {
                throw new ParseException("Unexpected end of input.");
            }

            if (Peek() != '"') {
                throw new ParseException("Expected key of object to be string.");
            }

            var key = String();
            SkipWhitespace();

            if (Peek() != ':') {
                throw new ParseException("Expected ':' between key and value of object.");
            }

            Advance();
            SkipWhitespace();

            var value = Value();

            var isError = dict.TryAdd(key, value);
            if (!isError) {
                throw new ParseException($"Encountered duplicate key '{key}' in object.");
            }

            SkipWhitespace();

            if (Peek() == '}') {
                Advance();
                return dict;
            }

            if (Peek() != ',') {
                throw new ParseException("Expected ',' between object entries.");
            }
            Advance();

            SkipWhitespace();
        }
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
            if (IsAtEnd()) {
                throw new ParseException("Unexpected end of input.");
            }

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
        Advance();
        SkipWhitespace();

        var str = "";
        var isEscape = false;
        while (true) {
            if (IsAtEnd()) {
                throw new ParseException("Unexpected end of input.");
            }

            var letter = Peek();
            
            if (isEscape) {
                switch (letter) {
                    case '"':
                    case '\\':
                    case '/':
                    case 'b':
                    case 'f':
                    case 'n':
                    case 'r':
                    case 't': {
                        str += letter;
                        isEscape = false;
                        Advance();
                        continue;
                    }
                    case 'u': {
                        Advance();
                        str += Unicode();
                        isEscape = false;
                        continue;
                    }
                    default: {
                        throw new ParseException("Unexpected escape character: '" + letter +"'");
                    }
                }
            }

            if (letter == '\\') {
                isEscape = true;
                Advance();
                continue;    
            }


            if (letter == '"') {
                Advance();
                return str;
            }

            str += letter;
            Advance();
        }
    }
    private char Unicode() {
        var hex = "";
        for (int i = 0; i < 4; i++) {
            if (IsAtEnd()) {
                throw new ParseException("Unexpected end of input.");
            }

            var next = Peek();
            if (!char.IsAsciiHexDigit(next)) {
                throw new ParseException("Only hex digits are allowed in unicode.");
            }

            hex += Peek();
            Advance();
        }
        var unicode = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        return (char)unicode;
    }
    private double Number() {
        var numberString = "";
        var isZero = false;

        if (Peek() == '-') {
            numberString += Peek();
            Advance();
        }

        if (Peek() == '0') {
            isZero = true;
            numberString += Peek();
            Advance();
        }

        if (isZero && char.IsAsciiDigit(Peek())) {
            throw new ParseException($"Unexpected token '{Peek()}' after number starting with zero.");
        } 
        
        if (!isZero) {
            while (!IsAtEnd() && char.IsAsciiDigit(Peek())) {
                numberString += Peek();
                Advance();
            }
        }

        if (Peek() == '.') {
            numberString += Peek();
            Advance();

            if (!char.IsAsciiDigit(Peek())) {
                throw new ParseException($"Unexpected token '{Peek()}' after decimal point.");
            }
            
            while(!IsAtEnd() && char.IsAsciiDigit(Peek())) {
                numberString += Peek();
                Advance();
            }
        }

        if (Peek() == 'e' || Peek() == 'E') {
            numberString += Peek();
            Advance();
            
            if (Peek() == '+' || Peek() == '-') {
                numberString += Peek();
                Advance();
            }

            if (!char.IsAsciiDigit(Peek())) {
                throw new ParseException($"Unexpected token '{Peek()}' after exponent.");
            }

            while(!IsAtEnd() && char.IsAsciiDigit(Peek())) {
                numberString += Peek();
                Advance();
            }
        }

        if (!double.TryParse(numberString, out var result)) {
            throw new ParseException($"'{numberString}' is not a number.");
        }
        return result;   
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
        return current >= source.Length;
    }
    private char Peek() {
        return source[current];
    }
}