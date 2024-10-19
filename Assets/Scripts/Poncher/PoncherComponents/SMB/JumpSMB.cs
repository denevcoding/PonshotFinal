using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSMB : BaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //pkComponent = poncherCharacter.GetParkourComponent();
        //pkComponent.canFlip = false;

        switch (animator.GetInteger("JumpType"))
        {
            case 0:
                poncherCharacter.SetState(PoncherState.Jumping);
                break;
            case 1:
                poncherCharacter.SetState(PoncherState.WallJumping);
                poncherCharacter.GetController().m_RotType = RotationType.ToVelocity;
                break;
            case 2:
                poncherCharacter.SetState(PoncherState.Jumping);
                break;
            default:
                break;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("End Called");
        //poncherCharacter.GetComponent<JumpComponent>().EndJump();

        //If wall jump restore the rotation settings        
        if (animator.GetInteger("JumpType") == 1)
        {
            poncherCharacter.GetController().m_RotType = RotationType.ToInputDir;

        }

        //poncherCharacter.GetJumpComp().EndJump();
   
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (poncherCharacter.GetRigidbody().velocity.y < -0.5)
        //    poncherCharacter.GetComponent<JumpComponent>().EndJump();

        if (animator.GetInteger("JumpType") == 1)
        {
            poncherCharacter.GetController().m_RotType = RotationType.ToVelocity;
        }
        //movementComponent.SetGroundAccel(0);
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
