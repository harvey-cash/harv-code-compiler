using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ScriptParser {
    
    public static char[] ignoreChars = new char[] { '\t' };

    // We don't like spaces anywhere other than within string literals
    public static string[] ParseCommandStrings(string scriptString) {
        List<string> listCommands = new List<string>();

        int depthSubscript = 0;
        bool withinSubscript = false;
        bool withinStringLiteral = false;

        string bufferCommand = "";

        for (int i = 0; i < scriptString.Length; i++) {
            char c = scriptString[i];
            if (Array.Exists(ignoreChars, x => x == c)) {
                continue;
            }

            if (c == '"') {
                withinStringLiteral = !withinStringLiteral;
            }
            // Ignore spaces everywhere except within strings!
            if (c == ' ' && !withinStringLiteral) {
                continue;
            }

            // Begin subscript
            if (c == '{' && !withinSubscript) {
                bufferCommand += c;
                depthSubscript++;
                withinSubscript = true;
                continue;
            }

            // Within subscript, increase depth
            if (c == '{' && withinSubscript) {
                bufferCommand += c;
                depthSubscript++;
                continue;
            }

            // End subscript
            if (c == '}' && withinSubscript) {
                bufferCommand += c;
                depthSubscript--;

                // Broke the surface
                if (depthSubscript < 1) {
                    listCommands.Add(bufferCommand);
                    bufferCommand = "";
                    withinSubscript = false;
                }

                continue;
            }

            // Split on line breaks
            if (c == '\n') {
                if (!withinSubscript && bufferCommand.Length > 0) {
                    listCommands.Add(bufferCommand);
                    bufferCommand = "";
                }
                continue;
            }

            // Otherwise, simply add to buffer
            bufferCommand += c;
        }

        // Final command
        if (bufferCommand.Length > 0) {
            listCommands.Add(bufferCommand);
        }

        return listCommands.ToArray();
    }


    public static bool IsOperator(char c, out bool isBool) { return IsOperator(c.ToString(), out isBool); }
    public static bool IsOperator(string opstr, out bool isBool) {
        try {
            BoolOp(opstr);
            isBool = true;
            return true;
        }
        catch {
            try {
                FloatOp(opstr);
                isBool = false;
                return true;
            }
            catch {
                isBool = false;
                return false;
            }
        }
    }

    public delegate bool BoolOperator(float a, float b);
    public static BoolOperator BoolOp(char c) { return BoolOp(c.ToString()); }
    public static BoolOperator BoolOp(string opstr) {
        if (opstr.Equals("=="))
            return (a, b) => a == b;
        if (opstr.Equals("!="))
            return (a, b) => a != b;
        if (opstr.Equals(">"))
            return (a, b) => a > b;
        if (opstr.Equals(">="))
            return (a, b) => a >= b;
        if (opstr.Equals("<"))
            return (a, b) => a < b;
        if (opstr.Equals("<="))
            return (a, b) => a <= b;

        throw new Exception();
    }

    public delegate float FloatOperator(float a, float b);
    public static FloatOperator FloatOp(char c) { return FloatOp(c.ToString()); }
    public static FloatOperator FloatOp(string opstr) {
        if (opstr.Equals("+"))
            return (a, b) => a + b;
        if (opstr.Equals("-"))
            return (a, b) => a - b;
        if (opstr.Equals("*"))
            return (a, b) => a * b;
        if (opstr.Equals("/"))
            return (a, b) => a / b;
        if (opstr.Equals("%"))
            return (a, b) => a % b;
        if (opstr.Equals("^"))
            return (a, b) => (float)Math.Pow(a, b);

        throw new Exception();
    }

    public static bool IsAlphaNumeric(char c) { return IsAlphaNumeric(c.ToString()); }
    public static bool IsAlphaNumeric(string word) {
        Regex r = new Regex("^[a-zA-Z0-9]*$");
        return r.IsMatch(word);
    }

    public static bool IsNumber(string word) {
        return float.TryParse(word, out float val);
    }

    public static bool IsStringLiteral(string word) {
        return word[0] == '\"';
    }

    // look through characters until an operator is reached
    // buffer the operator, check it actually is one
    // return either side
    // beware of strings with operations inside them...
    public static bool IsOperationStatement(string statement, 
        out string left, out string opstr, out string right) {

        int depth = 0;
        bool withinOp = false;
        string opBuffer = "";

        for (int i = 0; i < statement.Length; i++) {
            char c = statement[i];

            // only concern ourselves with operators
            // outside of parameters
            if (c == '(') { depth++; continue; }
            if (c == ')') { depth--; continue; }
            if (depth != 0) {
                continue;
            }

            // Start of something non-alphanumeric
            if (!withinOp && !IsAlphaNumeric(c)) {
                opBuffer += c;
                withinOp = true;
                continue;
            }

            // Continuation of something...
            if (withinOp) {
                if (!IsAlphaNumeric(c)) {
                    opBuffer += c;
                    continue;
                }
                else {
                    if (IsOperator(opBuffer, out _)) {
                        opstr = opBuffer;
                        left = statement.Substring(0, i - opstr.Length);
                        right = statement.Substring(i);
                        return true;
                    }
                    else {
                        left = null;
                        right = null;
                        opstr = null;
                        return false;
                    }
                }
            }
        }

        left = null;
        right = null;
        opstr = null;
        return false;
    }

    public static string MemoryString(Dictionary<string, object> memory) {
        string buffer = "";
        foreach (KeyValuePair<string, object> kvp in memory) {
            buffer += kvp.ToString() + ",";
        }
        return buffer;
    }

    // Step through parameters from first bracket (which must be there!) until final
    public static string[] SplitParameters(string restOfCommand) {
        while (restOfCommand[0] != '(') {
            restOfCommand = restOfCommand.Substring(1);
        }
        string paramString = restOfCommand.Substring(1);

        List<string> paramsList = new List<string>();
        string paramBuffer = "";
        int depth = 0;
        for (int i = 0; i < paramString.Length; i++) {
            char c = paramString[i];

            if (c == '(') { depth++; }
            if (c == ')') {
                depth--;
                if (depth < 0) { break; } // parameters ended
            }

            // outside of any sub-brackets
            if (c == ',' && depth == 0) {
                paramsList.Add(paramBuffer);
                paramBuffer = "";
                continue;
            }

            paramBuffer += c;
        }
        if (paramBuffer.Length > 0) {
            paramsList.Add(paramBuffer);
        }
        return paramsList.ToArray();
    }

    public static string ArrayToString(string[] array) {
        string buffer = "";
        if (array.Length > 0) {            
            for (int i = 0; i < array.Length - 1; i++) {
                buffer += array[i] + ",";
            }
            buffer += array[array.Length-1];
        }
        return buffer;        
    }
}
