using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SandSharpText : MonoBehaviour
{
    public RectTransform scrollView;
    private LayoutElement layout;

    private void OnGUI() {
        if (scrollView == null) { return; }
        if (layout == null) { layout = GetComponent<LayoutElement>(); }

        layout.minWidth = scrollView.rect.width;
        layout.minHeight = scrollView.rect.height;
    }
}
