using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library {

    // Create a library from given methods dictionary
    public static Dictionary<string, Command.Method> GenerateLibrary(Dictionary<string, Command.Method> methods) {
        Dictionary<string, Command.Method> library = new Dictionary<string, Command.Method>(builtIns);
        foreach (var entry in methods) {
            library[entry.Key] = methods[entry.Key];
        }
        return library;
    }

    public static Command.Method Print = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);

        string result = "";
        if (parameters.Length > 0) {
            for (int i = 0; i < parameters.Length - 1; i++) {
                result += parameters[i].ToString();
            }
            result += parameters[parameters.Length - 1].ToString();
        }

        Terminal.terminal.Print(result);

        return (memory, result);
    };

    public static Command.Method If = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);
        // if only takes one parameter, which equates to a bool
        if ((bool)parameters[0]) {
            return Command.RunSubscript(methods, memory, subscript);
        }
        else {
            return (memory, null);
        }
    };

    public static Command.Method Def = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);
        string[] restOfParameters = new string[parameters.Length-1];
        Array.Copy(parameters, 1, restOfParameters, 0, parameters.Length - 1);

        string methodName = (string)parameters[0];
        memory[methodName] = new UserMethod(restOfParameters, subscript);
        return (memory, null);
    };

    public static Command.Method For = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);

        string variable = (string)parameters[0];
        int start = Mathf.RoundToInt((float)parameters[1]);
        int end = Mathf.RoundToInt((float)parameters[2]);

        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);

        for (int i = start; i <= end; i++) {
            subMemory[variable] = i;
            (memory, _) = Command.RunSubscript(methods, memory, subMemory, subscript);
        }
        return (memory, null);
    };

    public static Command.Method While = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);
        object result = null;
        while ((bool)parameters[0]) {
            (memory, result) = Command.RunSubscript(methods, memory, subscript);
            parameters = Command.EvaluateParameters(methods, paramStrings, memory); // re-evaluate!
        }
        return (memory, result);
    };

    public static Command.Method Sin = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Sin(param));
    };
    public static Command.Method Cos = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Cos(param));
    };
    public static Command.Method Tan = (methods, memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(methods, paramStrings, memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Tan(param));
    };

    // Harvey-defined methods
    public static Dictionary<string, Command.Method> builtIns = new Dictionary<string, Command.Method>() {
        { "print", Print },
        { "if", If },
        { "def", Def },
        { "for", For },
        { "while", While },
        { "sin", Sin },
        { "cos", Cos },
        { "tan", Tan }
    };
}