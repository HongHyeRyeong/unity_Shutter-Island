using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSurvivorCtrl : MonoBehaviour
{
    private Animator Ani;

    bool Prison = true;
    bool PrisonTP = false;

    void Start()
    {
        Ani = this.gameObject.transform.Find("SurvivorModel").GetComponent<Animator>();
    }

    void Update()
    {
        if (Prison)
            PrisonTrue();
    }
    void PrisonTrue()
    {
        if (PrisonTP == false)
        {
            PrisonTP = true;

            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Prison");
            GameObject minObject = null;
            float minDist = 10000f;

            foreach (GameObject respawn in respawns)
            {
                if (respawn.GetComponent<PrisonCtrl>().GetOpen() == false)
                {
                    float dist = Vector3.Distance(transform.position, respawn.transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        minObject = respawn;
                    }
                }
            }

            if (minObject != null)
            {
                minObject.GetComponent<PrisonCtrl>().SurvivorEnter(this.gameObject);
                transform.position = minObject.transform.position;
            }
        }
    }

    public void PrisonFalse()
    {
        Prison = false;
        PrisonTP = false;
    }
}
