using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollSMB : BaseSMB
{
    public RollType RollType;
    //public bool canMove;
    //public bool canRotate;
    //public bool useRootMotion;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Rolling);
        poncherCharacter.GetRoll().SetRollType(RollType);

        poncherCharacter.canMove = false;
        poncherCharacter.canRotate = false;
        poncherCharacter.GetAnimManager().ActivateRootMotion();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.canMove = true;
        poncherCharacter.canRotate = true;
        poncherCharacter.GetAnimManager().DeactivateRootmotion();
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
