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

    // Harvey-defined methods
    public static Dictionary<string, Method> methods = new Dictionary<string, Method>() {
        { "print", Print }
    };

}