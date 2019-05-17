using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMethod {

    // Call user method
    public static (Dictionary<string, object>, object) CallUserMethod(
        Dictionary<string, object> memory, UserMethod method, object[] parameters) {

        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);
        for (int i = 0; i < method.paramNames.Length; i++) {
            subMemory[method.paramNames[i]] = parameters[i];
        }

        Debug.Log(method.subscript);

        return Command.RunSubscript(memory, subMemory, method.subscript);
    }

    private string[] paramNames;
    private string subscript;
    public UserMethod(string[] paramNames, string subscript) {
        this.paramNames = paramNames;
        this.subscript = subscript;
    }
}
