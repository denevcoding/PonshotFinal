using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingSMB : BaseSMB
{
    public RollType rollType;
    public bool overrideRoll;
    float defaultAccel;
    public float acelerationAffect;
    public bool cancelJump;
    public bool canRotate;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        poncherCharacter.SetState(PoncherState.Landing);      

        if (cancelJump)
        {
            poncherCharacter.GetComponent<JumpComponent>().canJump = false;
        }
        else
        {
            poncherCharacter.GetComponent<JumpComponent>().canJump = true;
        }


        if (poncherCharacter.isStrafing && !overrideRoll)
        {
            poncherCharacter.GetAnimator().SetInteger("RollType", (int)RollType.backRoll);
        }
        else
        {
            poncherCharacter.GetRoll().SetRollType(rollType);
            poncherCharacter.GetAnimator().SetInteger("RollType", (int)rollType);
        }



        defaultAccel = poncherCharacter.GetComponent<MovementComp>().moveSpeed;
        poncherCharacter.GetComponent<MovementComp>().moveSpeed = acelerationAffect;

        poncherCharacter.canRotate = canRotate;

        
      

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!poncherCharacter.isStrafing)
            poncherCharacter.GetAnimator().SetInteger("RollType", (int)RollType.standRoll);

        poncherCharacter.GetComponent<MovementComp>().landingForce = 0f;
        poncherCharacter.GetComponent<MovementComp>().moveSpeed = defaultAccel;
        poncherCharacter.GetComponent<JumpComponent>().RestoreJump();
        poncherCharacter.canRotate = true;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
