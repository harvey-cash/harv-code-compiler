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

            string[] commandStrings = ScriptParser.ParseCommandStrings(input.text);
            for (int i = 0; i < commandStrings.Length; i++) {
                Debug.Log(commandStrings[i]);
            }

            input.text = "";
            //input.ActivateInputField();
        });
    }

    public void Print(string log) {
        history.text += log + "\n";
    }
}
