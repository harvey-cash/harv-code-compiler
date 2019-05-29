using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandSharpLineCount : MonoBehaviour
{
    public Text scriptText;
    private Text lineText;

    private void Start() {
        lineText = GetComponent<Text>();
    }

    public void OnScriptChange() {
        int lines = scriptText.text.Split(new char[] { '\n' }).Length;

        string lineCountText = "";
        for (int i = 0; i < lines; i++) {
            lineCountText += i.ToString() + '\n';
        }

        lineText.text = lineCountText;
    }
}
