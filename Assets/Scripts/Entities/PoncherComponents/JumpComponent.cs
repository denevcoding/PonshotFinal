using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpComponent : PoncherComponentBase
{
    [Header("Jump Properties")]
    public Vector3 jumpForce;
    public bool canJump;

    public bool jumpPressed;

    public float maxJumpHighTime;
    public float timeMaxHigh;

    [Header("Falling multipliers")]
    public float fallMultiplier;
    public float lowJumpMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        timeMaxHigh = 0;
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Early Exit if I am not in this skill
        //if (poncherCharacter.GetState() != PoncherState.Jumping)
        //    return;

        if (poncherCharacter.isGrounded)
        {
            timeMaxHigh = 0;
        }

        jumpPressed = poncherCharacter.GetController().poncherActions.PlayerGameplay.Jump.ReadValue<float>() > 0;

        Vector3 velocity = poncherCharacter.GetRigidbody().velocity;
        if (jumpPressed && timeMaxHigh < maxJumpHighTime)
        {
            timeMaxHigh += Time.deltaTime;      
            velocity = new Vector3(velocity.x, jumpForce.y, velocity.z);
            poncherCharacter.GetRigidbody().velocity = velocity;

            //poncherCharacter.GetRigidbody().AddRelativeForce(jumpForce, ForceMode.VelocityChange);
            Debug.Log("Jumping");
        }

        //if (!jumpPressed || velocity.y < 0)
        //{

        //    velocity = new Vector3(velocity.x, Physics.gravity.y * -0.5f, velocity.z);
        //    poncherCharacter.GetRigidbody().velocity = velocity;
        //}


        //EndJump();
    }

    private void FixedUpdate()
    {
        //Early Exit if I am not in this skill
        if (poncherCharacter.GetState() != PoncherState.Jumping)
            return;
    }


    public void Jump(InputAction.CallbackContext context)
    {
        //if (context.performed)
        //{
        //    jumpPressed = true;
        //}
        //if (context.canceled)
        //{
        //    jumpPressed = false;
        //}
        
        //if (context.started)
        //{
        //    if (CheckBasePreconditions() == false)
        //        return;

        //    if (canJump == false)
        //        return;

        //    if (poncherCharacter.GetState() == PoncherState.Jumping)
        //        return;

        //    if (poncherCharacter.coyoteTimeCounter < 0)
        //        return;
        //}

       

        //button is being pressed
        //if (context.performed)
        //{
        //    Debug.Log("saltando....");
        //}


        //poncherCharacter.GetAnimator().SetBool("Jumping", true);
        ////Debug.Log("salto");
        //poncherCharacter.coyoteTimeCounter = 0;

        //poncherCharacter.GetRigidbody().AddRelativeForce(jumpForce, ForceMode.Impulse);
        //Gamepad gamepad = Gamepad.current;
        //poncherCharacter.GetAnimator().SetBool("Jumping", false);
        //gamepad.SetMotorSpeeds(0.5f, 0.8f);
    }

    public void EndJump()
    {
        if (poncherCharacter.GetRigidbody().velocity.y > -3 || poncherCharacter.isGrounded || poncherCharacter.isWalled)
        {
            poncherCharacter.GetAnimator().SetBool("Jumping", false);
        }

    }
}
