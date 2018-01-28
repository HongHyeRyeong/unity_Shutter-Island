using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour {
    public float moveSpeed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h, 0, v) * moveSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
    }
}
