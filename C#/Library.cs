using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library {

    public static Method Print = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);

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

    public static Method If = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);
        // if only takes one parameter, which equates to a bool
        if ((bool)parameters[0]) {
            return SandSharp.RunSubscript(methods, memory, Command.SubCommand(line, subscript));
        }
        else {
            return (memory, null);
        }
    };

    public static Method Def = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);
        string[] restOfParameters = new string[parameters.Length-1];
        Array.Copy(parameters, 1, restOfParameters, 0, parameters.Length - 1);

        string methodName = (string)parameters[0];
        memory[methodName] = new UserMethod(restOfParameters, subscript);
        return (memory, null);
    };

    public static Method For = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);

        string variable = (string)parameters[0];
        int start = Mathf.RoundToInt((float)parameters[1]);
        int end = Mathf.RoundToInt((float)parameters[2]);

        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);

        for (int i = start; i <= end; i++) {
            subMemory[variable] = i;
            (memory, _) = SandSharp.RunSubscript(methods, memory, subMemory, Command.SubCommand(line, subscript));
        }
        return (memory, null);
    };

    public static Method While = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);
        object result = null;
        while ((bool)parameters[0]) {
            (memory, result) = SandSharp.RunSubscript(methods, memory, Command.SubCommand(line, subscript));
            parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory); // re-evaluate!
        }
        return (memory, result);
    };

    public static Method Sin = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Sin(param));
    };
    public static Method Cos = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Cos(param));
    };
    public static Method Tan = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);
        float param = (float)parameters[0];
        return (memory, Mathf.Tan(param));
    };

    public static Method Not = (methods, memory, name, paramStrings, line, subscript) => {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);

        bool param = (bool)parameters[0];
        return (memory, !param);
    };

    // Harvey-defined methods
    public static Dictionary<string, Method> builtIns = new Dictionary<string, Method>() {
        { "print", Print },
        { "if", If },
        { "def", Def },
        { "for", For },
        { "while", While },
        { "sin", Sin },
        { "cos", Cos },
        { "tan", Tan },
        { "!", Not }
    };
}