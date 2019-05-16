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

            Command[] commands = ScriptParser.ParseCommands(input.text);
            for (int i = 0; i < commands.Length; i++) {
                Debug.Log(commands[i]);
            }

            input.ActivateInputField();
        });
    }
}
