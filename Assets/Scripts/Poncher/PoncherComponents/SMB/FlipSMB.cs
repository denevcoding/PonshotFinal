using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSMB : BaseSMB
{
    public FlipType flipType;
    //public bool canMove;
    //public bool canRotate;
    //public bool useRootMotion;
    RollComponent flipRollComponent;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        flipRollComponent = poncherCharacter.GetComponent<RollComponent>();
        poncherCharacter.SetState(PoncherState.Fliping);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        //movementComponent.SetGroundAccel(0);
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        flipRollComponent.EndFlip();

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
