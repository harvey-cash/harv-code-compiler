using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public static MovingBlock block;
    public string[] commands;
    Dictionary<string, object> memory;

    private bool turnTaken = false;
    public static Library.Method MoveBlock = (memory, name, paramStrings, subscript) => {
        object[] parameters = Command.EvaluateParameters(paramStrings, memory);

        if (!block.turnTaken) {
            float x = (float)parameters[0];
            float z = (float)parameters[1];

            block.Move(new Vector2(x, z));
        }
        block.turnTaken = true;

        return (memory, null);
    };

    public void Move(Vector2 dir) {
        transform.Translate(new Vector3(dir.x, 0, dir.y));
    }

    private void Awake() {
        block = this;
        Library.methods.Add("move", MoveBlock);
    }

    private void Start() {
        memory = new Dictionary<string, object>();
        InvokeRepeating("OnTurn", 0.05f, 0.05f);
    }

    private void OnTurn() {
        block.turnTaken = false;        
        memory["xPos"] = transform.position.x;
        memory["zPos"] = transform.position.z;
        (memory, _) = Command.Run(memory, commands);
    }
}
