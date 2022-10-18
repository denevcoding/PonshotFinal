using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingSMB : BaseSMB
{
    float defaultAccel;
    public float acelerationAffect;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Landing);

        defaultAccel = poncherCharacter.GetMoveComponent().accel;
        poncherCharacter.GetMoveComponent().SetGroundAccel(acelerationAffect);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.GetMoveComponent().SetGroundAccel(defaultAccel);
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
