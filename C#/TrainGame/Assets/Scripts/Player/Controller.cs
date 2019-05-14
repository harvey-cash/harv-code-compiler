using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Rigidbody body;

    private void Start() {
        body = GetComponent<Rigidbody>();
    }

    public void MoveDirection(Vector2 dir) {
        body.AddForce(new Vector3(dir.x, 0, dir.y));
    }

    public void Turn(float theta) {
        body.AddTorque(new Vector3(0, theta, 0));
    }

}
