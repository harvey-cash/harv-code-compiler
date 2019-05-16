using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library
{
    // Defined methods all follow this structure
    public delegate (Dictionary<string,object>, object) Method(
        Dictionary<string, object> memory, string name, object[] parameters, string subscript);    

    public static Method Print = (memory, name, parameters, subscript) => {
        string result = "";
        if (parameters.Length > 0) {
            for (int i = 0; i < parameters.Length - 1; i++) {
                result += parameters[i].ToString() + ",";
            }
            result += parameters[parameters.Length - 1].ToString();
        }

        Terminal.terminal.Print(result);

        return (memory, result);
    };

    public static Method If = (memory, name, parameters, subscript) => {
        // if only takes one parameter, which equates to a bool
        if ((bool)parameters[0]) {
            string[] commandStrings = ScriptParser.ParseCommandStrings(subscript);

            object result;
            Dictionary<string, object> subMemory;
            (subMemory, result) = Command.Run(memory, commandStrings);
            return (ResolveSubScope(memory, subMemory), result);
        }
        else {
            return (memory, null);
        }
    };

    // Harvey-defined methods
    public static Dictionary<string, Method> methods = new Dictionary<string, Method>() {
        { "print", Print },
        { "if", If }
    };

    // Modify what already existed, forget all else
    public static Dictionary<string, object> ResolveSubScope(Dictionary<string, object> memory, Dictionary<string, object> subMemory) {
        foreach (var entry in memory) {
            memory[entry.Key] = subMemory[entry.Key];
        }
        return memory;
    }

}