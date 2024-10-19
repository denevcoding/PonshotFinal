using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingSMB : BaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Charging);

        //poncherCharacter.canRotate = canRotate;
        //poncherCharacter.canMove = canRotate;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //poncherCharacter.canRotate = true;
        //poncherCharacter.canMove = true;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //poncherCharacter.SetState(PoncherState.Charging);
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
