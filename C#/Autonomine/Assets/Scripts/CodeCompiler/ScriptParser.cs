using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptParser
{
    public static char[] ignoreChars = new char[] { '\t' };

    public static Command[] ParseCommands(string scriptString) {
        string[] commandStrings = ParseCommandStrings(scriptString);
        Command[] commands = new Command[commandStrings.Length];

        for (int i = 0; i < commandStrings.Length; i++) {
            commands[i] = new Command(commandStrings[i]);
        }

        return commands;
    }

    public static string[] ParseCommandStrings(string scriptString) {
        List<string> listCommands = new List<string>();

        bool withinSubscript = false;
        string bufferCommand = "";

        for (int i = 0; i < scriptString.Length; i++) {
            char c = scriptString[i];
            if (Array.Exists(ignoreChars, x => x == c)) {
                continue;
            }

            // Begin subscript
            if (c == '{' && !withinSubscript) {
                bufferCommand += c;
                withinSubscript = true;
                continue;
            }

            // End subscript
            if (c == '}' && withinSubscript) {
                bufferCommand += c;
                listCommands.Add(bufferCommand);
                bufferCommand = "";
                withinSubscript = false;
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

    public static string[][] ParseScript(string scriptString) {

        string[] script = scriptString.Split(new char[] { '\n' });
        List<string[]> lines = new List<string[]>();

        for (int l = 0; l < script.Length; l++) {
            string line = script[l];

            if (line == "" || line == "\n")
                continue;

            string[] lineWords = ParseLine(line);

            if (lineWords.Length > 0) {
                lines.Add(lineWords);
            }
        }

        return lines.ToArray();
    }

    public static string[] ParseLine(string line) {

        List<string> lineWords = new List<string>();
        bool withinString = false;
        string wordBuffer = "";

        for (int i = 0; i < line.Length; i++) {
            char c = line[i];

            if (c == '\"') {
                withinString = !withinString;

                wordBuffer += c;
                if (!withinString) {
                    lineWords.Add(wordBuffer);
                    wordBuffer = "";
                }
            }
            else if (c == ' ') {
                if (withinString) {
                    wordBuffer += c;
                }
                else if (wordBuffer.Length > 0) {
                    lineWords.Add(wordBuffer);
                    wordBuffer = "";
                }
            }
            else if (c != '\n' && c != '\t') {
                wordBuffer += c;
            }
        }

        if (wordBuffer.Length > 0) {
            lineWords.Add(wordBuffer);
        }
        return lineWords.ToArray();
    }

    public static string CodeToString(string[] line) {
        return CodeToString(new string[][] { line });
    }

    public static string CodeToString(string[][] commands) {
        string buffer = "[";
        for (int i = 0; i < commands.Length; i++) {
            buffer += "[";
            for (int j = 0; j < commands[i].Length - 1; j++) {
                buffer += commands[i][j];
                buffer += ",";
            }
            buffer += commands[i][commands[i].Length - 1] + "],";
        }
        buffer += "]";

        return buffer;
    }

    public delegate bool BoolOperator(float a, float b);
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


}
