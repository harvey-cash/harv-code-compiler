using System;
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
            return Command.RunSubscript(memory, subscript);
        }
        else {
            return (memory, null);
        }
    };

    public static Method Def = (memory, name, parameters, subscript) => {
        string[] restOfParameters = new string[parameters.Length-1];
        Array.Copy(parameters, 1, restOfParameters, 0, parameters.Length - 1);

        string methodName = (string)parameters[0];
        memory[methodName] = new UserMethod(restOfParameters, subscript);
        return (memory, null);
    };

    // Harvey-defined methods
    public static Dictionary<string, Method> methods = new Dictionary<string, Method>() {
        { "print", Print },
        { "if", If },
        { "def", Def }
    };

}