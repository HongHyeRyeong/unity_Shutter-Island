using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimCtrl : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject survivor = animator.gameObject.transform.parent.gameObject;

        if (stateInfo.IsName("AttackW"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("AttackL"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("ParryW"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("ParryL"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("Hit"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("PickItem"))
        {
            survivor.GetComponent<SurvivorCtrl>().SetState(0);
        }
    }
}