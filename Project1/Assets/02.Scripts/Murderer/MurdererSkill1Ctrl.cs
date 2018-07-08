using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererSkill1Ctrl : MonoBehaviour
{
    public int SetNum = -1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Survivor")
        {
            this.gameObject.SetActive(false);

            GameCtrl.instance.Murderer.GetComponent<MurdererCtrl>();
        }
    }
}
