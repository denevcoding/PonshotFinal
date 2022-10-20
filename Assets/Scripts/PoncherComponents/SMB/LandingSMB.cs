using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingSMB : BaseSMB
{
    public RollType rollType;

    float defaultAccel;
    public float acelerationAffect;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Landing);


        poncherCharacter.GetAnimator().SetInteger("RollType", (int)rollType);

        defaultAccel = poncherCharacter.GetMoveComponent().accel;
        poncherCharacter.GetMoveComponent().SetGroundAccel(acelerationAffect);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.GetMoveComponent().SetGroundAccel(defaultAccel);

        poncherCharacter.GetAnimator().SetInteger("RollType", (int) RollType.standRoll);
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
