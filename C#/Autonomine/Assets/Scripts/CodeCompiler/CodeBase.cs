using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class CodeBase
{
    // A command takes in parameters, memory and returns some sort of result
    public delegate object Method(string[] line, string[][] followingLines,
        Dictionary<string, object> memory);

    // "METHOD NAME" then parameters
    public static Dictionary<string, Method> callingMethods = new Dictionary<string, Method>() {
        {"var", Methods.Var }
    };
    // "VARIABLE NAME" then method name, then parameters
    public static Dictionary<string, Method> operatingMethods = new Dictionary<string, Method>() {

    };

    // The library is searched for line structure, then the caller referred to the
    // relevant dictionary of commands
    public static Dictionary<WordClass, Dictionary<string, Method>> library =
        new Dictionary<WordClass, Dictionary<string, Method>>() {
            { WordClass.METHOD_NAME, callingMethods },
            { WordClass.VARIABLE_NAME, operatingMethods }
        };

    // calling methods are looked up in first position
    // operating methods are looked up in second position
    public static int MethodIDIndex(WordClass type) {
        if (type == WordClass.METHOD_NAME)
            return 0;
        else
            return 1;
    }

    public static WordClass ClassifyWord(string word, Dictionary<string, object> memory) {

        // Memory lists an object under this name
        if (memory.ContainsKey(word)) {
            return WordClass.VARIABLE_NAME;
        }

        // Float-able or quoted string?
        if (IsNumber(word) || IsStringLiteral(word)) {
            return WordClass.LITERAL;
        }

        // Otherwise, if only letters and numbers, probably a method name
        if (IsAlphaNumeric(word)) {
            return WordClass.METHOD_NAME;
        }

        // Otherwise its some jumble of symbols, so probably an operator!
        else {
            return WordClass.OPERATOR;
        }
    }

    public static bool IsAlphaNumeric(string word) {
        Regex r = new Regex("^[a-zA-Z0-9]*$");
        return r.IsMatch(word);
    }

    public static bool IsNumber(string word) {
        return float.TryParse(word, out float val);
    }

    public static bool IsStringLiteral(string word) {
        return word[0] == '\"';
    }
}
