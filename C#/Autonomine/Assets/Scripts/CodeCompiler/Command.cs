using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    private enum WordOrder {
        Destination = 0,
        Method = 1,
        Params = 2,
        Ignore = 3,
        Subscript = 4,
        Finished = 5
    }

    public string destination = null, method = null, value = null;
    public string[] parameters = null;
    public string subscript = null;

    public override string ToString() {
        string buffer = "";
        if (destination != null) { buffer += "dest: " + destination + ","; }
        if (method != null) { buffer += "method: " + method + ","; }
        if (value != null) { buffer += "value: " + value + ","; }
        if (parameters != null) { buffer += "params: " + parameters.ToString() + ","; }
        if (subscript != null) { buffer += "subscript: " + subscript; }

        return buffer;
    }

    public Command(string commandString) {
        string parameterBody = null;

        WordOrder state = WordOrder.Destination;
        int subscriptDepth = 0;
        string bufferWord = "";

        for (int i = 0; i < commandString.Length; i++) {
            char c = commandString[i];

            // assignment
            if (c == '=' && state == WordOrder.Destination) {                
                destination = bufferWord;
                bufferWord = "";
                state = WordOrder.Method;
                continue;
            }

            // start params
            if (c == '(' && (state == WordOrder.Method || destination == null)) {
                method = bufferWord;
                bufferWord = "";
                state = WordOrder.Params;
                continue;
            }

            // end params
            if (c == ')' && state == WordOrder.Params) {
                parameterBody = bufferWord;
                bufferWord = "";
                state = WordOrder.Ignore;
                continue;
            }

            // start subscript
            if (c == '{' && state == WordOrder.Ignore) {
                bufferWord = "";
                state = WordOrder.Subscript;
                continue;
            }

            // deeper subscripts
            if (c == '{' && state == WordOrder.Subscript) {
                bufferWord += c;
                subscriptDepth++;
            }

            // shallower / end subscript
            if (c == '}' && state == WordOrder.Subscript) {
                // shallower
                if (subscriptDepth > 0) {
                    bufferWord += c;
                    subscriptDepth--;
                }
                // finish
                else {
                    subscript = bufferWord;
                    bufferWord = "";
                    state = WordOrder.Finished;
                }
                
                continue;
            }

            bufferWord += c;
        }

        // end of command and still awaiting end of method declaration
        // must therefore be a value!
        if (state == WordOrder.Method) {
            value = bufferWord;
        }

        parameters = SeparateString(parameterBody);
    }

    private static string[] SeparateString(string body) {
        if (body == null) {
            return null;
        }

        List<string> listParams = new List<string>();
        string bufferWord = "";

        for (int i = 0; i < body.Length; i++) {
            char c = body[i];

            if (c == ',') {
                listParams.Add(bufferWord);
                bufferWord = "";
                continue;
            }

            bufferWord += c;
        }

        if (bufferWord.Length > 0) {
            listParams.Add(bufferWord);
        }

        return listParams.ToArray();
    }
    
}
