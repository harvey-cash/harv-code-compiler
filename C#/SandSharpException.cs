using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandSharpException : Exception
{
    public int lineIndex { private set; get; }
    public string exception { private set; get; }

    public SandSharpException(int lineIndex, string exception) {
        this.lineIndex = lineIndex;
        this.exception = exception;
    }
}
