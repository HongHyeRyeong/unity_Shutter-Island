using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCtrl : MonoBehaviour {
    float MoveSpeed = 4.5f;

    // 카메라
    private float x;
    float xSpeed = 100.0f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;

        transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(0, x, 0);
        transform.rotation = rotation;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player")
        {
           col.gameObject.GetComponent<PlayerCtrl>().DamageByBoss();
        }
    }
}
