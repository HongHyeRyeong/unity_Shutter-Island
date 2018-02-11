using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player")
        {
            GameObject.Find("Player").GetComponent<PlayerState>().DamageByBoss();

            //print("col");
        }
    }
}
