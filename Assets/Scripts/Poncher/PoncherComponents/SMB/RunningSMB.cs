using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningSMB : BaseSMB
{ 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Running);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (poncherCharacter.isGrounded)
        {
            if (animator.GetFloat("VelocityX") < 0)
            {
                poncherCharacter.GetMovementComp().SetLocomotionFactor(0.75f);
            }
            else
            {
                poncherCharacter.GetMovementComp().RestoreLocomotionFactor();
            }
        }
        else
        {
            poncherCharacter.GetMovementComp().RestoreLocomotionFactor();
        }
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
