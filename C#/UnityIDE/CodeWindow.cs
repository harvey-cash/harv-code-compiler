using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CodeWindow : MonoBehaviour
{    
    public HorizontalLayoutGroup windowLayout;
    public Text lineCountText, scriptText;

    private RectTransform window, lineCount;
    private LayoutElement scriptTextLayout;

    private void OnGUI() {
        if (window == null) { window = GetComponent<RectTransform>(); }
        if (lineCount == null && lineCountText != null) { lineCount = lineCountText.rectTransform; }
        if (scriptTextLayout == null && scriptText != null) { scriptTextLayout = scriptText.GetComponent<LayoutElement>(); }

        if (lineCountText != null && scriptTextLayout != null && windowLayout != null) {
            scriptTextLayout.minWidth = window.rect.width - lineCount.rect.width
                - windowLayout.padding.left - windowLayout.padding.right - windowLayout.spacing * 2;
            scriptTextLayout.minHeight = window.rect.height
                - windowLayout.padding.top - windowLayout.padding.bottom;
        }
    }

    public void CountLines() {
        int lines = scriptText.text.Split(new char[] { '\n' }).Length;

        string buffer = "";
        for (int i = 0; i < lines; i++) {
            buffer += i.ToString() + '\n';
        }
        buffer += lines.ToString();

        lineCountText.text = buffer;
    }
}
