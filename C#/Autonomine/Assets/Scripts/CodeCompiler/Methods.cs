
using System.Collections.Generic;

public static class Methods
{
    public static CodeBase.Method var = new CodeBase.Method(Var);

    public static object Var(string[] line, string[][] followingLines,
        Dictionary<string, object> memory) {
        
        string name = TryGet(line, 1);
        if (name == null) {
            Print("Must declare variable name");
            return null;
        }

        WordClass type = CodeBase.ClassifyWord(name, memory);

        if (type == WordClass.LITERAL || type == WordClass.OPERATOR) {
            Print("Invalid name (num/str/operator)");
            return null;
        }

        string valString = TryGet(line, 3);

        if (valString == null) {
            Print("Must assign a value");
            return null;
        }

        WordClass valType = CodeBase.ClassifyWord(valString, memory);

        if (valType == WordClass.OPERATOR || valType == WordClass.METHOD_NAME) {
            Print("Invalid value (operator/method)");
            return null;
        }

        // Variable name
        if (valType == WordClass.VARIABLE_NAME) {
            if (memory.TryGetValue(valString, out object value)) {
                memory.Add(name, value);
                return value;
            }
            else {
                Print("\"" + valString + "\" is undefined.");
                return null;
            }            
        }
        // Literal value
        else {
            // PARSE VALUE
            memory.Add(name, valString);
            return valString;
        }
    }

    // Return null if element of array doesn't exist
    private static string TryGet(string[] array, int index) {
        if (0 <= index && index < array.Length) {
            return array[index];
        }
        else {
            return null;
        }
    }

    private static void Print(string log) {
        Terminal.terminal.PrintToTerminal(log);
    }
}
