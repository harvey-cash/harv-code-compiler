using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Machine
{
    public static GameObject New() {
        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newObj.AddComponent<Mover>();
        return newObj;
    }

    private void Awake() {
        if (!GetComponent<Rigidbody>()) {
            gameObject.AddComponent<Rigidbody>();
        }
        GetComponent<Rigidbody>().drag = 0;
    }

    private void Update() {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * Mathf.Sin(Time.time) * 5);
    }
}
