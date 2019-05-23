using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodBuilder {

    // Create a library from given names and functions
    public static Dictionary<string, Method> GenerateLibrary((string, Func<object[], object>)[] methods) {
        Dictionary<string, Method> library = new Dictionary<string, Method>(Library.builtIns);

        foreach ((string, Func<object[], object>) entry in methods) {
            Method method = Build(entry.Item2);
            library.Add(entry.Item1, method);
        }
        return library;
    }

    // create a SandSharp method from a function
    public static Method Build(Func<object[], object> callBack) {
        Method method = (methods, memory, name, paramStrings, subscript) => {
            object[] parameters = SandSharp.EvaluateParameters(methods, paramStrings, memory);

            object result = callBack(parameters);
            return (memory, result);
        };

        return method;
    }

}
