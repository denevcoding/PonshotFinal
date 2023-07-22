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

public enum FlipType
{
    None = 5,
    FrontFlip = 0,
    SideFlip = 1,
    BackFlip = 2,
    TwistFlip = 3,
    BackStylish = 4,
}

public class RollComponent : PoncherComponentBase
{
    [Header("Roll Settings")]
    //Roll Settings
    public RollType rollType; //Handled from anim state SMB
    public bool canRoll;
    
    [Header("Flip Settings")]
    //Flip Settings
    public bool canFlip;
    private FlipType flipType;
    //[HideInInspector] public bool isflipping = false;

    // Start is called before the first frame update
    void Start()
    {
        canRoll = true;
        canFlip = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!poncherCharacter.isGrounded)
            canRoll = false; //Cant Roll in the air
        else
        {
            canRoll = true;
        }

        if (poncherCharacter.GetRigidbody().velocity.y < -2 && poncherCharacter.GetState() == PoncherState.Fliping)
        {
            poncherCharacter.GetAnimator().SetBool("Flipping", false);
        }

            
    }

    public void FlipRoll(InputAction.CallbackContext context)
    {
        if (CheckBasePreconditions() == false)
            return;

        if (poncherCharacter.IsGrounded())
        {
            //Perform RollPoncher is on the ground 
            if (poncherCharacter.GetState() == PoncherState.Rolling)
                return;

            if (poncherCharacter.coyoteTimeCounter < 0)
                return;

            if (!canRoll)
                return;


            if (poncherCharacter.isRotBlocked && poncherCharacter.GetState() != PoncherState.Landing)
                poncherCharacter.GetAnimator().SetInteger("RollType", (int)RollType.backRoll);
            //else        
            //    poncherCharacter.GetAnimator().SetInteger("RollType", (int)RollType.standRoll);

            poncherCharacter.GetAnimator().SetTrigger("Roll");
        }
        else
        {
            //Perform Flip - Poncehr is on the Air
            if (poncherCharacter.GetState() == PoncherState.Fliping)
                return;

            if (!canFlip)
                return;

            if (poncherCharacter.GetRigidbody().velocity.y < -2)
                return;
                
            poncherCharacter.GetAnimator().SetInteger("FlipType", (int)FlipType.FrontFlip);
            poncherCharacter.GetAnimator().SetBool("Flipping", true);
        }

  

      

    }


    #region Rolls
    public void SetRollType(RollType rollType)
    {
        poncherCharacter.GetAnimator().SetInteger("RollType", (int)this.rollType);
        this.rollType = rollType;
    }

    #endregion



    #region Flips
    public void SetFlipType(FlipType flipType)
    {
        poncherCharacter.GetAnimator().SetInteger("FlipType", (int)this.rollType);
        this.flipType = flipType;
    }

    public void EndFlip()
    {
        poncherCharacter.GetAnimator().SetBool("Flipping", false);
        poncherCharacter.canRotate = true;
        
    }

    #endregion
}
