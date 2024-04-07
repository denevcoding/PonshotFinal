using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchSMB : BaseSMB
{
    float defaultAccel;
    public float acelerationAffect;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {     
        poncherCharacter.SetState(PoncherState.Crouch);


        defaultAccel = poncherCharacter.GetComponent<MovementComp>().moveSpeed;
    
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.GetComponent<MovementComp>().moveSpeed = defaultAccel;
        //poncherCharacter.GetComponent<JumpComponent>().RestoreJump();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.GetComponent<MovementComp>().moveSpeed = acelerationAffect;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
