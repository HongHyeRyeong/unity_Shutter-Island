using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorCamera : MonoBehaviour
{
    private Transform tr;
    public Transform target;

    float dist = 5.0f;
    float height = 5.0f;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        if (target)
        {
            dist -= 0.5f * Input.mouseScrollDelta.y;

            if (dist < 4) dist = 4;
            else if (dist >= 7) dist = 7;

            tr.position = Vector3.Lerp(tr.position,
                target.position - (target.forward * dist) + (Vector3.up * height),
                Time.deltaTime * 20.0f);

            tr.LookAt(target.position);
        }
    }
}
