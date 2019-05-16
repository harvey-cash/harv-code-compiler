using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptWindow : MonoBehaviour
{
    public InputField input;
    public Button apply;

    // Start is called before the first frame update
    void Start() {
        // temporary
        Dictionary<string, object> memory = new Dictionary<string, object>();

        apply.onClick.AddListener(delegate {

            string[] commands = ScriptParser.ParseCommandStrings(input.text);
            for (int i = 0; i < commands.Length; i++) {
                object result;
                (memory, result) = Command.Evaluate(memory, commands[i]);
            }
            Debug.Log(ScriptParser.MemoryString(memory));

            input.ActivateInputField();
        });
    }
}
