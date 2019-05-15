using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runtime
{
    
    public Dictionary<string, object> Execute(string[][] script,
        Dictionary<string, object> memory) {

        // End of script
        if (script.Length < 1) {
            return memory;
        }

        // Lines left to run
        string[] line = script[0];
        string[][] followingLines = new string[script.Length - 1][];
        Array.Copy(script, 1, followingLines, 0, script.Length - 1);

        // Call appropriate commands
        ExecuteLine(line, followingLines, memory);


        // Terminal.terminal.PrintToTerminal(ScriptParser.CodeToString(line));
        return Execute(followingLines, memory);
    }

    private void ExecuteLine(string[] line, string[][] followingLines,
        Dictionary<string, object> memory) {
        
        // Figure out the type of command from the class of the first two words!
        WordClass type = CodeBase.ClassifyWord(line[0], memory);

        // Check this structure against the library to find possible methods
        if (CodeBase.library.TryGetValue(type, out Dictionary<string, CodeBase.Method> methods)) {
            // Found a dictionary of methods!

            int wordIndex = CodeBase.MethodIDIndex(type);
            if (methods.TryGetValue(line[wordIndex], out CodeBase.Method method)) {
                // Found a method!

                method(line, followingLines, memory);
            }
            else {
                // No method found!
                Terminal.terminal.PrintToTerminal("\"" + line[wordIndex] + "\" is undefined.");
            }
        }
        else {
            Terminal.terminal.PrintToTerminal("No methods begin with: " + type.ToString());
            return;
        }
    }
    
}
