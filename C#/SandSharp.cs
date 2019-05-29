using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SandSharp {

    // Run all commands in array of command strings
    public static (Dictionary<string, object>, object) Run(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, Command command) {
        return Run(methods, memory, new Command[] { command });
    }
    public static (Dictionary<string, object>, object) Run(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, Command[] commands) {

        if (!(commands != null && commands.Length > 0)) {
            return (memory, null);
        }

        int i = -1;

        try {
            object result = null;            
            for (i = 0; i < commands.Length; i++) {
                (memory, result) = RunCommand(methods, memory, commands[i]);
            }
            return (memory, result);
        }
        catch (Exception e) {            
            Debug.LogError("Line " + i.ToString() + ": " + e.ToString());
            Terminal.terminal.Print("Line " + i.ToString() + ": " + e.ToString());
            Terminal.terminal.Print("Script stopped.");
            return (memory, null);
        }
    }

    // Run subscript on copy of memory, and remove changes to anything that wasn't defined in the outer scope
    public static (Dictionary<string, object>, object) RunSubscript(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, Command subscript) {
        Dictionary<string, object> subMemory = new Dictionary<string, object>(memory);
        return RunSubscript(methods, memory, subMemory, subscript);
    }

    // Run subscript on subMemory, and remove changes to anything that wasn't defined in the outer scope's memory
    public static (Dictionary<string, object>, object) RunSubscript(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, Dictionary<string, object> subMemory, Command subscript) {
        Command[] subCommands = ScriptParser.ParseCommandStrings(subscript.script);

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
    private static (Dictionary<string, object>, object) RunCommand(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, Command command) {

        // if entirely in brackets, strip away and look inside
        if (ScriptParser.AllInBrackets(command.script)) {
            string newString = command.script.Substring(1, command.script.Length - 2);
            return RunCommand(methods, memory, new Command(command.line, newString));
        }

        // Base case, evaluates to literal
        if (ScriptParser.IsNumber(command.script)) {
            return (memory, float.Parse(command.script));
        }
        if (ScriptParser.IsStringLiteral(command.script)) {
            string newString = command.script.Substring(1, command.script.Length - 2);
            Debug.Log(newString);
            return (memory, newString);
        }

        // Assign right hand side to memory space...
        if (ScriptParser.IsAssignment(command.script,
            out string name, out string value)) {

            (memory, memory[name]) = RunCommand(methods, memory, new Command(command.line, value));
            return (memory, memory[name]);
        }

        // Check if its an equation, and run BODMAS over it!
        if (ScriptParser.ParseEquation(command.script,
            out string left, out string opstr, out string right)) {

            Debug.Log(command);

            ScriptParser.IsOperator(opstr, out bool isBool);
            object leftObj, rightObj;
            (memory, leftObj) = RunCommand(methods, memory, new Command(command.line, left));
            (memory, rightObj) = RunCommand(methods, memory, new Command(command.line, right));

            float leftEval = float.Parse(leftObj.ToString());
            float rightEval = float.Parse(rightObj.ToString());

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
        if (memory.ContainsKey(command.script)) {
            return (memory, memory[command.script]);
        }

        // Else, look for statement or method of some sort

        string buffer = "";
        for (int i = 0; i < command.script.Length; i++) {
            char c = command.script[i];

            // The start of a method?
            if (c == '(') {
                string methodName = buffer;
                // include open bracket for parsing parameters
                string[] paramNames = ScriptParser.SplitParameters(command.script.Substring(i));
                string subscript = ScriptParser.ParseSubscript(command.script.Substring(i + 1));

                return LookupAndRun(methods, out bool methodExists, memory, methodName, 
                    paramNames, new Command(command.line, subscript));
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
    public static (Dictionary<string, object>, object) LookupAndRun(Dictionary<string, Method> methods,
        out bool exists,
        Dictionary<string, object> memory, string name, string[] paramStrings, Command subCommand) {

        // Built-in method?
        bool builtIn = methods.TryGetValue(name, out Method method);        
        if (builtIn) {
            exists = true;
            return method(methods, memory, name, paramStrings, subCommand.line, subCommand.script);
        }

        // User-defined method?
        bool defined = memory.TryGetValue(name, out object userMethod);
        if (defined) {
            exists = true;
            return UserMethod.CallUserMethod(methods, memory, (UserMethod)userMethod, subCommand.line, paramStrings);
        }

        // Else:
        // Terminal.terminal.Print("\"" + name + "\" is undefined.");
        exists = false;
        return (memory, null);
    }

    // Don't split on ','s within brackets! (Methods as parameters...)
    public static object[] EvaluateParameters(Dictionary<string, Method> methods,
        Command[] paramCommands, Dictionary<string,object> memory) {
        
        object[] parameters = new object[paramCommands.Length];
        for (int p = 0; p < parameters.Length; p++) {
            (memory, parameters[p]) = RunCommand(methods, memory, paramCommands[p]);
        }
        return parameters;
    }
}
