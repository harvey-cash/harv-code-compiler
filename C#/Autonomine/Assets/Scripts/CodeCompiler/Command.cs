using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    public static (Dictionary<string, object>, object) Evaluate(Dictionary<string, object> memory, string command) {

        // Base case, evaluates to literal
        if (ScriptParser.IsNumber(command)) {
            return (memory, float.Parse(command));
        }
        if (ScriptParser.IsStringLiteral(command)) {
            return (memory, command.Substring(1, command.Length - 2));
        }

        // Name of something in memory, evaluate and return it
        if (memory.ContainsKey(command)) {
            return Evaluate(memory, memory[command].ToString());
        }

        // Else, statement or method of some sort

        string buffer = "";
        for (int i = 0; i < command.Length; i++) {
            char c = command[i];
            
            string c2 = command[i + 1].ToString();

            // Next two form an operator
            if (ScriptParser.IsOperator(c + c2, out bool twoMakeBool)) {
                object p1, p2;
                (memory, p1) = Evaluate(memory, buffer);
                (memory, p2) = Evaluate(memory, command.Substring(i + 2));

                if (twoMakeBool) { ScriptParser.BoolOperator op = ScriptParser.BoolOp(c + c2); return (memory, op((float)p1, (float)p2)); }
                else { ScriptParser.FloatOperator op = ScriptParser.FloatOp(c + c2); return (memory, op((float)p1, (float)p2)); }
            }

            // Else, this one alone forms an operator
            if (ScriptParser.IsOperator(c, out bool isBool)) {
                object p1, p2;
                (memory, p1) = Evaluate(memory, buffer);
                (memory, p2) = Evaluate(memory, command.Substring(i + 1));
                
                if (isBool) { ScriptParser.BoolOperator op = ScriptParser.BoolOp(c); return (memory, op((float)p1, (float)p2)); }
                else { ScriptParser.FloatOperator op = ScriptParser.FloatOp(c); return (memory, op((float)p1, (float)p2)); }                
            }

            // Else, this is an assignment command
            if (c == '=' && ScriptParser.IsAlphaNumeric(c2)) {
                (memory, memory[buffer]) = Evaluate(memory, command.Substring(i + 1));
                return (memory, memory[buffer]);
            }

            // Else, if '=' this code is wrong
            if (c == '=') {
                throw new Exception();
            }

            // Else neither an assignment nor a simple statement
            // The start of a method?
            if (c == '(') {
                string methodName = buffer;
                object[] parameters = ParseParameters(command.Substring(i + 1), memory);
                string subscript = ParseSubscript(command.Substring(i + 1));

                return LookupAndRun(memory, methodName, parameters, subscript);
            }

            // Must just be some other letter or number!
            // Continue to add to the buffer
            buffer += c;
        }

        // We reached the end without calling anything interesting? Oh.
        // I guess we don't like that
        throw new Exception();
    }

    // Look in library for a method to run. Error if it doesn't exist!
    private static (Dictionary<string, object>, object) LookupAndRun(
        Dictionary<string, object> memory, string name, object[] parameters, string subscript) {

        bool defined = Library.methods.TryGetValue(name, out Library.Method Method);
        
        if (!defined) {
            Terminal.terminal.Print("\"" + name + "\" is undefined.");
            return (memory, null);
        } else {
            return Method(memory, name, parameters, subscript);
        }
    }

    private static string ParseSubscript(string restOfCommand) {
        // skip to after close bracket
        while (restOfCommand[0] != ')') {
            restOfCommand = restOfCommand.Substring(1);
        }

        // skip to first opening curly brace
        // if first thing after ) is not space then {, no subscript
        try {
            while (restOfCommand[0] == ' ') {
                restOfCommand = restOfCommand.Substring(1);
            }
            if (!(restOfCommand[0] == '{')) { throw new Exception(); }
        }
        // no subscript
        catch {
            return null;
        }

        restOfCommand = restOfCommand.Substring(1);
        // record inside brace until close brace of same level
        string buffer = "";
        int depth = 1;
        for (int i = 0; i < restOfCommand.Length; i++) {
            buffer += restOfCommand[i];

            if (restOfCommand[i] == '{') { depth++; }
            if (restOfCommand[i] == '}') {
                depth--;
                if (depth < 1) {
                    break;
                }
            }
        }
        // Ignore last outer brace
        return buffer.Substring(0, buffer.Length - 1);
    }

    private static object[] ParseParameters(string restOfCommand, Dictionary<string,object> memory) {
        string paramString = ParamsWithinBrackets(restOfCommand);
        string[] paramStrings = paramString.Split(new char[] { ',' });
        object[] parameters = new object[paramStrings.Length];
        for (int p = 0; p < parameters.Length; p++) {
            (memory, parameters[p]) = Evaluate(memory, paramStrings[p]);
        }
        return parameters;
    }

    // Will throw error if no ending bracket
    private static string ParamsWithinBrackets(string restOfCommand) {
        string paramBuffer = "";
        while (restOfCommand[0] != ')') {
            paramBuffer += restOfCommand[0];
            restOfCommand = restOfCommand.Substring(1);
        }
        return paramBuffer;
    }    
}
