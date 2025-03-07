public class Serializer(object? source) {
    private object? source = source;
    private object? current = source;
    private string result = "";
    public string Serialize() {
        result = "";
        current = source;
        Value();
        return result;
    }
    private void Value() {
        if (current == null) {
            Null();
        } else {
            switch(current.GetType()) {

            }
        }
    }
    private void Null() {
        result += "null";
    }
}