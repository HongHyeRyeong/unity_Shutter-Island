using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SurvivorCamera : MonoBehaviour
{
    private Transform tr;
    Transform target;

    float dist = 5.0f;
    float height = 2.5f;

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

            Vector3 pos = target.position;
            pos.y += 1.5f;

            tr.position = Vector3.Lerp(tr.position,
                pos - (target.forward * dist) + (Vector3.up * height),
                Time.deltaTime * 20.0f);

            tr.LookAt(pos);
        }
        else
        {
            try
            {
                target = GameObject.Find("SurvivorCamPivot").transform;
            }
            catch (NullReferenceException ex)
            {
                print(ex);
            }
        }
    }
}
