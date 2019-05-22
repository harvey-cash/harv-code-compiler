using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library {

    // Defined methods all follow this structure
    public delegate (Dictionary<string,object>, object) Method(
        Dictionary<string, object> memory, string name, string[] paramStrings, string subscript);    

    public static Method Print = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);

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

    public static Method If = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);
        // if only takes one parameter, which equates to a bool
        if ((bool)parameters[0]) {
            return Command.RunSubscript(memory, subscript);
        }
        else {
            return (memory, null);
        }
    };

    public static Method Def = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);
        string[] restOfParameters = new string[parameters.Length-1];
        Array.Copy(parameters, 1, restOfParameters, 0, parameters.Length - 1);

        string methodName = (string)parameters[0];
        memory[methodName] = new UserMethod(restOfParameters, subscript);
        return (memory, null);
    };

    public static Method For = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);

        string variable = (string)parameters[0];
        int start = Mathf.RoundToInt((float)parameters[1]);
        int end = Mathf.RoundToInt((float)parameters[2]);

        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);

        for (int i = start; i <= end; i++) {
            subMemory[variable] = i;
            (memory, _) = Command.RunSubscript(memory, subMemory, subscript);
        }
        return (memory, null);
    };

    public static Method While = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);
        object result = null;
        while ((bool)parameters[0]) {
            (memory, result) = Command.RunSubscript(memory, subscript);
            parameters = Command.EvaluateParameters(paramStrings, memory); // re-evaluate!
        }
        return (memory, result);
    };

    public static Method Sin = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Sin(param));
    };
    public static Method Cos = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Cos(param));
    };
    public static Method Tan = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Tan(param));
    };

    // Harvey-defined methods
    public static Dictionary<string, Method> methods = new Dictionary<string, Method>() {
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