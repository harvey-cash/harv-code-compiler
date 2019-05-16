using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Runtime {

    // Try running a script
    public static Dictionary<string, object> Run(string[][] script,
        Dictionary<string, object> memory) {

        try {
            return RunScript(script, memory);
        }
        catch (Exception e) {
            Terminal.terminal.Print("Script Execution Failed.");
            Debug.LogError(e);
            return memory; // return unchanged
        }
    }

    // Memory spaces created in sub-scopes are not stored following
    // completion of the scope
    public static Dictionary<string, object> RunSubScope(string[][] script,
        Dictionary<string, object> memory) {

        Dictionary<string, object> subScopeMemory = RunScript(script, memory);

        // We throw away any dictionary keys not found in memory outside
        // of this sub scope
        foreach (var item in memory) {
            memory[item.Key] = subScopeMemory[item.Key];
        }

        return memory;
    }

    // Run a script, based on the given memory
    public static Dictionary<string, object> RunScript(string[][] script,
        Dictionary<string, object> memory) {

        // End of script
        if (script.Length < 1) { return memory; }

        // Separate current line from the rest
        string[] line = script[0];
        string[][] followingLines = new string[script.Length - 1][];
        Array.Copy(script, 1, followingLines, 0, script.Length - 1);

        try {
            // Run this line, tail recurse on the rest
            //(followingLines, memory) = ExecuteLine(line, followingLines, memory);
            return Run(followingLines, memory);
        }
        catch (Exception e) {
            // Quit Execution if there's an error anywhere
            Terminal.terminal.Print("Error in: \"" + ScriptParser.CodeToString(line) + "\"");
            throw e;
        }
    }


    // EVALUATION return what this string represents
    public static object Evaluate(string name, Dictionary<string, object> memory) {
        // number
        if (IsNumber(name)) {
            float.TryParse(name, out float val);
            return val;
        }
        // string literal
        if (IsStringLiteral(name)) {
            return name.Substring(1, name.Length - 2);
        }
        // something in memory
        bool exists = memory.TryGetValue(name, out object value);
        if (exists) {
            // Check whether value is a method subscript
            // if it is, interpret and RunOperation on it
            return value;
        }
        // can't find it
        throw new Exception();
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

    public static string MemoryString(Dictionary<string, object> memory) {
        string buffer = "";
        foreach (KeyValuePair<string, object> kvp in memory) {
            buffer += kvp.ToString() + ",";
        }
        return buffer;
    }
}