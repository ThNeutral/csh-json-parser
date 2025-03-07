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
            var type = current.GetType();
            if (type == typeof(bool)) {
                Boolean();
            } else if (type.IsArray) {
                Array();
            } else if (IsNumericType(type)) {
                Number();
            } else if (type == typeof(string)) {
                String();
            } else {
                Object();
            }
        }
    }
    private void Null() {
        result += "null";
    }
    private void Boolean() {
        if ((bool)current!) {
            result += "true";
        } else {
            result += "false";
        }
    }
    private void Array() {
        var array = (Array)current!;
        result += "[";
        for (var index = 0; index < array.Length; index++) {
            if (index != 0) result += ",";
            current = array.GetValue(index);
            Value();
        }
        result += "]";
    }
    private void Number() {
       result += current!.ToString();
    }
    private void Object() {

    }
    private void String() {
        result += current!.ToString();
    }
    private bool IsNumericType(Type type) {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }
}