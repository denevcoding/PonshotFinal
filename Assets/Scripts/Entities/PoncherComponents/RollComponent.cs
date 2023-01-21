using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RollType
{
    standRoll = 0,
    safetyRoll = 1
}

public class RollComponent : PoncherComponentBase
{
    //Roll Settings
    public RollType rollType;
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

        poncherCharacter.GetAnimator().SetTrigger("Roll");

    }
}
