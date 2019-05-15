using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptParser
{
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
}
