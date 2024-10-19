using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchSMB : BaseSMB
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Launching);
        //poncherCharacter.GetRoll().SetRollType(RollType);

        //poncherCharacter.GetRigidbody().velocity = Vector3.zero;
        poncherCharacter.canMove = false;
        poncherCharacter.canRotate = false;
       // poncherCharacter.GetAnimManager().ActivateRootMotion();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Enu for launch Type
        poncherCharacter.GetController().lockRotation = false;
        poncherCharacter.canMove = true;
        poncherCharacter.canRotate = true;
        // poncherCharacter.GetAnimManager().DeactivateRootmotion();
        poncherCharacter.GetAnimManager().SetUpperBodyLayerWeight(0f, true);
        poncherCharacter.GetAnimManager().SetUpperBodyRigWeight(0f);
        animator.SetBool("Launching", false);
        
        
   
        //poncherCharacter.GetAnimator().SetBool("Rolling", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //movementComponent.SetGroundAccel(0);
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
