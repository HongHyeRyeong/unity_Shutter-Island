using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float dist = 10.0f;
    public float height = 3.0f;
    public float smoothing = 20.0f;

    void LatedUpdate()
    {
        transform.position = Vector3.Lerp(target.position,
            target.position - (target.forward * dist) + (Vector3.up * height),
            Time.deltaTime * smoothing);

        transform.LookAt(target.position);
    }
}
