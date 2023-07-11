using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RollType
{
    standRoll = 0,
    safetyRoll = 1,
    backRoll =2

}

public class RollComponent : PoncherComponentBase
{
    //Roll Settings
    public RollType rollType; //Handled from anim state SMB
    public bool canRoll;


    // Start is called before the first frame update
    void Start()
    {
        canRoll = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!poncherCharacter.isGrounded)
            canRoll = false; //Can Roll in the air
        else
        {
            canRoll = true;
        }
    }

    public void ParkourRoll(InputAction.CallbackContext context)
    {
        if (CheckBasePreconditions() == false)
            return;

        if (poncherCharacter.GetState() == PoncherState.Rolling)
            return;

        if (poncherCharacter.coyoteTimeCounter < 0)
            return;

        if (!canRoll)
            return;


        if (poncherCharacter.isRotBlocked && poncherCharacter.GetState() != PoncherState.Landing)        
            poncherCharacter.GetAnimator().SetInteger("RollType", (int)RollType.backRoll);
        else        
            poncherCharacter.GetAnimator().SetInteger("RollType", (int)RollType.standRoll);
        
        
        
        poncherCharacter.GetAnimator().SetTrigger("Roll");

    }



    public void SetRollType(RollType type)
    {
        poncherCharacter.GetAnimator().SetInteger("RollType", (int)rollType);
        rollType = type;
    }
}
