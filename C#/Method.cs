using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A method takes in memory and returns modified memory + a result
public delegate (Dictionary<string, object>, object) Method(Dictionary<string, Method> methods,
        Dictionary<string, object> memory, string name, string[] paramStrings, string subscript);
