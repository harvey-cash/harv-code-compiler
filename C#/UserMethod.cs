using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMethod {

    // Call user method
    public static (Dictionary<string, object>, object) CallUserMethod(
        Dictionary<string, Method> methods,
        Dictionary<string, object> memory, UserMethod method, int line, string[] paramStrings) {
        object[] parameters = SandSharp.EvaluateParameters(methods, Command.SubCommands(line, paramStrings), memory);

        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);
        for (int i = 0; i < method.paramNames.Length; i++) {
            subMemory[method.paramNames[i]] = parameters[i];
        }

        return SandSharp.RunSubscript(methods, memory, subMemory, Command.SubCommand(line, method.subscript));
    }

    private string[] paramNames;
    private string subscript;
    public UserMethod(string[] paramNames, string subscript) {
        this.paramNames = paramNames;
        this.subscript = subscript;
    }
}
