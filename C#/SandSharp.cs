using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SandSharp {

    // Run all commands in array of command strings
    public static (Dictionary<string, object>, object) Run(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, string[] commands) {

        if (!(commands != null && commands.Length > 0)) {
            return (memory, null);
        }

        object result = null;
        for (int i = 0; i < commands.Length; i++) {
            (memory, result) = Run(methods, memory, commands[i]);
        }
        return (memory, result);
        /*
        try {
            object result = null;
            for (int i = 0; i < commands.Length; i++) {
                (memory, result) = Run(memory, commands[i]);
            }
            return (memory, result);
        } catch (Exception e) {
            Debug.LogError(e);
            Terminal.terminal.Print("Script terminated.");
            return (memory, null);
        }
        */
    }

    // Run subscript on copy of memory, and remove changes to anything that wasn't defined in the outer scope
    public static (Dictionary<string, object>, object) RunSubscript(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, string subscript) {
        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);
        return RunSubscript(methods, memory, subMemory, subscript);
    }

    // Run subscript on subMemory, and remove changes to anything that wasn't defined in the outer scope's memory
    public static (Dictionary<string, object>, object) RunSubscript(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, Dictionary<string, object> subMemory, string subscript) {
        string[] subCommands = ScriptParser.ParseCommandStrings(subscript);

        // We ignore whatever the runtime equates to, 
        // and instead specifically look for the value of the "return" variable
        (subMemory, _) = Run(methods, subMemory, subCommands);
        object result = ParseReturn(subMemory);
        return (ResolveSubScope(memory, subMemory), result);
    }

    // Look in the given memory for the value of "return"
    public static object ParseReturn(Dictionary<string, object> memory) {
        bool defined = memory.TryGetValue("return", out object value);
        if (defined) {
            return value;
        }
        else {
            return null;
        }
    }

    // Modify what already existed, forget all else
    public static Dictionary<string, object> ResolveSubScope(Dictionary<string, object> memory, Dictionary<string, object> subMemory) {
        Dictionary<string, object> modifiedOuterScope = new Dictionary<string, object>();

        // Avoid modifying memory while enumerating over it
        foreach (var entry in memory) {
            modifiedOuterScope[entry.Key] = subMemory[entry.Key];
        }
        foreach (var entry in modifiedOuterScope) {
            memory[entry.Key] = modifiedOuterScope[entry.Key];
        }
        return memory;
    }

    // Run a command string
    public static (Dictionary<string, object>, object) Run(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, string command) {

        // if entirely in brackets, strip away and look inside
        if (ScriptParser.AllInBrackets(command)) {
            return Run(methods, memory, command.Substring(1, command.Length - 2));
        }

        // Base case, evaluates to literal
        if (ScriptParser.IsNumber(command)) {
            return (memory, float.Parse(command));
        }
        if (ScriptParser.IsStringLiteral(command)) {
            return (memory, command.Substring(1, command.Length - 2));
        }

        // Assign right hand side to memory space...
        if (ScriptParser.IsAssignment(command,
            out string name, out string value)) {

            (memory, memory[name]) = Run(methods, memory, value);
            return (memory, memory[name]);
        }

        // Check if its an equation, and run BODMAS over it!
        if (ScriptParser.ParseEquation(command,
            out string left, out string opstr, out string right)) {

            ScriptParser.IsOperator(opstr, out bool isBool);
            object leftObj, rightObj;
            (memory, leftObj) = Run(methods, memory, left);
            (memory, rightObj) = Run(methods, memory, right);

            float leftEval = (float)leftObj;
            float rightEval = (float)rightObj;

            if (isBool) {
                ScriptParser.BoolOperator op = ScriptParser.BoolOp(opstr);
                return (memory, op(leftEval, rightEval));
            }
            else {
                ScriptParser.FloatOperator op = ScriptParser.FloatOp(opstr);
                return (memory, op(leftEval, rightEval));
            }
        }

        // Name of something in memory, return it
        if (memory.ContainsKey(command)) {
            return (memory, memory[command]);
        }

        // Else, look for statement or method of some sort

        string buffer = "";
        for (int i = 0; i < command.Length; i++) {
            char c = command[i];

            // The start of a method?
            if (c == '(') {
                string methodName = buffer;
                // include open bracket for parsing parameters
                string[] paramNames = ScriptParser.SplitParameters(command.Substring(i));
                string subscript = ScriptParser.ParseSubscript(command.Substring(i + 1));

                return LookupAndRun(methods, out bool methodExists, memory, methodName, paramNames, subscript);
            }

            // Must just be some other letter or number!
            // Continue to add to the buffer
            buffer += c;
        }

        // We reached the end without calling anything interesting? Oh.
        // I guess we don't like that. Probably doesn't exist
        Terminal.terminal.Print("\"" + buffer + "\" is undefined.");
        throw new Exception();
    }

    // Look for a method to run. Error if it doesn't exist!
    private static (Dictionary<string, object>, object) LookupAndRun(Dictionary<string, Method> methods,
        out bool exists,
        Dictionary<string, object> memory, string name, string[] paramStrings, string subscript) {

        // Built-in method?
        bool builtIn = methods.TryGetValue(name, out Method method);        
        if (builtIn) {
            exists = true;
            return method(methods, memory, name, paramStrings, subscript);
        }

        // User-defined method?
        bool defined = memory.TryGetValue(name, out object userMethod);
        if (defined) {
            exists = true;
            return UserMethod.CallUserMethod(methods, memory, (UserMethod)userMethod, paramStrings);
        }

        // Else:
        Terminal.terminal.Print("\"" + name + "\" is undefined.");
        exists = false;
        return (memory, null);
    }

    // Don't split on ','s within brackets! (Methods as parameters...)
    public static object[] EvaluateParameters(Dictionary<string, Method> methods,
        string[] paramStrings, Dictionary<string,object> memory) {    
        
        object[] parameters = new object[paramStrings.Length];
        for (int p = 0; p < parameters.Length; p++) {
            (memory, parameters[p]) = Run(methods, memory, paramStrings[p]);
        }
        return parameters;
    }
}
