using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpComponent : PoncherComponentBase
{
    [Header("Jump Properties")]
    public Vector3 jumpForce;
    public bool canJump;
    //public bool Jumping;

    // Start is called before the first frame update
    void Start()
    {
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {      
        //Early Exit if I am not in this skill
        if (poncherCharacter.GetState() != PoncherState.Jumping)
            return;
        EndJump();
    }

    private void FixedUpdate()
    {
        //Early Exit if I am not in this skill
        if (poncherCharacter.GetState() != PoncherState.Jumping)
            return;
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (CheckBasePreconditions() == false)
            return;

        if (canJump == false)
            return;

        if (poncherCharacter.GetState() == PoncherState.Jumping)
            return;

        if (poncherCharacter.coyoteTimeCounter < 0)
            return;

        poncherCharacter.GetAnimator().SetBool("Jumping", true);
        //Debug.Log("salto");
        poncherCharacter.coyoteTimeCounter = 0;
        poncherCharacter.GetRigidbody().AddRelativeForce(jumpForce, ForceMode.Impulse);
        Gamepad gamepad = Gamepad.current;
        //gamepad.SetMotorSpeeds(0.5f, 0.8f);
    }

    public void EndJump()
    {
        if (poncherCharacter.GetRigidbody().velocity.y < -1 || poncherCharacter.isGrounded || poncherCharacter.isWalled)
        {
            poncherCharacter.GetAnimator().SetBool("Jumping", false);
        }
       
    }
}