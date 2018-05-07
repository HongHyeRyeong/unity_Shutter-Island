using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererAnimCtrl : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject Murderer = animator.gameObject;

        if (stateInfo.IsName("AttackW") && Murderer.GetComponent<MurdererCtrl>().GetState() == 10)
        {
            Murderer.GetComponent<MurdererCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("AttackL") && Murderer.GetComponent<MurdererCtrl>().GetState() == 11)
        {
            Murderer.GetComponent<MurdererCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("Parry") && Murderer.GetComponent<MurdererCtrl>().GetState() == 2)
        {
            Murderer.GetComponent<MurdererCtrl>().SetState(0);
        }
        else if (stateInfo.IsName("Die"))
        {
            Murderer.GetComponent<MurdererCtrl>().SetState(-1);
        }
    }
}
