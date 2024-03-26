using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSMB : BaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //pkComponent = poncherCharacter.GetParkourComponent();
        //pkComponent.canFlip = false;

        poncherCharacter.SetState(PoncherState.Jumping);   
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("End Called");
        //poncherCharacter.GetComponent<JumpComponent>().EndJump();

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (poncherCharacter.GetRigidbody().velocity.y < -1)
            poncherCharacter.GetComponent<JumpComponent>().EndJump();
        //movementComponent.SetGroundAccel(0);
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
