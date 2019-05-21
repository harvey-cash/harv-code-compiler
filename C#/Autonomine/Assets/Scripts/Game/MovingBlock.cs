using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public static MovingBlock block;
    public string[] commands;
    Dictionary<string, object> memory;

    private bool turnTaken = false;
    public static Library.Method MoveBlock = (memory, name, parameters, subscript) => {

        if (!block.turnTaken) {
            int x = Mathf.RoundToInt((float)parameters[0]);
            int z = Mathf.RoundToInt((float)parameters[1]);

            block.Move(new Vector2Int(x, z));
        }
        block.turnTaken = true;

        return (memory, null);
    };

    public void Move(Vector2Int dir) {
        transform.Translate(new Vector3(dir.x, 0, dir.y));
    }

    private void Awake() {
        block = this;
        Library.methods.Add("move", MoveBlock);
    }

    private void Start() {
        memory = new Dictionary<string, object>();
        InvokeRepeating("OnTurn", 1, 1);
    }

    private void OnTurn() {
        block.turnTaken = false;        
        memory["xPos"] = Mathf.Round(transform.position.x);
        memory["zPos"] = Mathf.Round(transform.position.z);
        (memory, _) = Command.Run(memory, commands);
    }
}
