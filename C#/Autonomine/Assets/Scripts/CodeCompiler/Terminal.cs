using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public static Terminal terminal;

    public InputField input;
    public Text history;

    // Start is called before the first frame update
    void Start()
    {
        // temporary
        Dictionary<string, object> memory = new Dictionary<string, object>();

        terminal = this;
        
        input.onEndEdit.AddListener(delegate {
            Print("> " + input.text);

            string[] commands = ScriptParser.ParseCommandStrings(input.text);
            (memory, _) = Command.Run(memory, commands);

            input.text = "";
            //input.ActivateInputField();
        });
    }

    public void Print(string log) {
        history.text += log + "\n";
    }
}
