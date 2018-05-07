using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimCtrl : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject survivor = animator.gameObject.transform.parent.gameObject;

        if (stateInfo.IsName("AttackW") && survivor.GetComponent<SurvivorCtrl>().GetState() == 10)
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("AttackL") && survivor.GetComponent<SurvivorCtrl>().GetState() == 11)
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("ParryW") && survivor.GetComponent<SurvivorCtrl>().GetState() == 12)
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("ParryL") && survivor.GetComponent<SurvivorCtrl>().GetState() == 13)
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("Hit") && survivor.GetComponent<SurvivorCtrl>().GetState() == 3)
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("PickItem") && survivor.GetComponent<SurvivorCtrl>().GetState() == 4)
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("Die"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(-1);
        }
    }
}