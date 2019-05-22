using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMethod {

    // Call user method
    public static (Dictionary<string, object>, object) CallUserMethod(
        Dictionary<string, object> memory, UserMethod method, string[] paramStrings) {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);

        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);
        for (int i = 0; i < method.paramNames.Length; i++) {
            subMemory[method.paramNames[i]] = parameters[i];
        }

        return Command.RunSubscript(memory, subMemory, method.subscript);
    }

    private string[] paramNames;
    private string subscript;
    public UserMethod(string[] paramNames, string subscript) {
        this.paramNames = paramNames;
        this.subscript = subscript;
    }
}
