﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public static Terminal terminal;
    public Runtime runtime;

    public InputField input;
    public Text history;

    // Start is called before the first frame update
    void Start()
    {
        // temporary
        Dictionary<string, object> memory = new Dictionary<string, object>();

        terminal = this;
        runtime = new Runtime();
        
        input.onEndEdit.AddListener(delegate {
            PrintToTerminal("> " + input.text);
            runtime.Execute(ScriptParser.ParseScript(input.text), memory);

            input.text = "";
            input.ActivateInputField();
        });
    }

    public void PrintToTerminal(string log) {
        history.text += log + "\n";
    }
}