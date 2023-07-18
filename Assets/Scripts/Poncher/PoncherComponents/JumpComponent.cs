using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpComponent : PoncherComponentBase
{
    [Header("Jump Properties")]
    public Vector3 jumpForce;
    public Vector3 doubleJumpForce;
    public Vector3 wallJumpForce;
    public float wallJumpXForce;
    public bool canJump;

    public bool canDoubleJump;

    public bool jumpPressed;

    public float maxJumpHighTime;
    public float timeMaxHigh;

    public float jumpCutMultiplier;

    [Header("Falling multipliers")]
    public float fallMultiplier;
    public float lowJumpMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        timeMaxHigh = 0;
        canJump = true;
        canDoubleJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Early Exit if I am not in this skill
        //if (poncherCharacter.GetState() != PoncherState.Jumping)
        //    return;


        //if (!canJump)
        //    return;


        //canJump = false;

        // jumpPressed = poncherCharacter.GetController().poncherActions.PlayerGameplay.Jump.ReadValue<float>() > 0;

        // Vector3 velocity = poncherCharacter.GetRigidbody().velocity;

        //if (jumpPressed /*timeMaxHigh < maxJumpHighTime && !poncherCharacter.upObstacle*/)   
        //{

        //    //timeMaxHigh += Time.deltaTime;   

        //    //velocity = new Vector3(velocity.x, jumpForce.y, velocity.z);
        //    //poncherCharacter.GetRigidbody().velocity = velocity;




        //    //poncherCharacter.GetRigidbody().AddRelativeForce(jumpForce, ForceMode.VelocityChange);
        //    Debug.Log("Jumping");
        //}
        //else
        //{

        //}

        //if (!jumpPressed || velocity.y < 0)
        //{

        //    velocity = new Vector3(velocity.x, Physics.gravity.y * -0.5f, velocity.z);
        //    poncherCharacter.GetRigidbody().velocity = velocity;
        //}


        //EndJump();

    }

    private void FixedUpdate()
    {
        if (poncherCharacter.GetRigidbody().velocity.y < 0)
        {
            GetComponent<MovementComp>().gravityScale = 1.0f * 1.5f;
        }

        //Early Exit if I am not in this skill
        if (poncherCharacter.GetState() != PoncherState.Jumping)
            return;

        //EndJump();
    }


    public void JumpWithPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canJump)
            {
                if (poncherCharacter.GetState() == PoncherState.Jumping && !poncherCharacter.checkIsWalled() && canDoubleJump)
                {
                    DoubleJump();
                }
                else
                {
                    Jump();
                }
                
            }       
            //jumpPressed = true;
            //poncherCharacter.GetController().AddActionToBuffer(context.action); 
        }

        //Release
        if (context.canceled)
        {
            if (poncherCharacter.GetController().inputBuffer.Count > 0)
            {
                poncherCharacter.GetController().inputBuffer.Dequeue();
            }

            if (poncherCharacter.GetRigidbody().velocity.y > 0 /*&& poncherCharacter.GetState() == PoncherState.Jumping*/)
            {
                poncherCharacter.GetRigidbody().AddForce(Vector2.down * poncherCharacter.GetRigidbody().velocity.y * (1 -jumpCutMultiplier), ForceMode.Impulse);
            }          

           
            //jumpPressed = false;

        }
            

    }


    public void Jump()
    {
        if (poncherCharacter.coyoteTimeCounter < poncherCharacter.coyoteTime)
        {
            if (poncherCharacter.GetState() == PoncherState.WallSliding)
            {
                poncherCharacter.GetRigidbody().AddForce(wallJumpForce.y * Vector2.up, ForceMode.Impulse);
                poncherCharacter.GetRigidbody().AddForce(wallJumpForce.x * poncherCharacter.wallNormal, ForceMode.Impulse);
                //poncherCharacter.GetRigidbody().AddForce((Vector2.right * -1) * poncherCharacter.GetRigidbody().velocity.x * (1 - 0.5f), ForceMode.Impulse);
                poncherCharacter.GetAnimator().SetInteger("JumpType", 1);
                poncherCharacter.GetAnimator().SetBool("Jumping", true);
            }
            else
            {
                //Normal Jump
                poncherCharacter.GetRigidbody().AddForce(jumpForce.y * Vector2.up, ForceMode.Impulse);
                poncherCharacter.GetRigidbody().AddForce((Vector2.right * -1) * poncherCharacter.GetRigidbody().velocity.x * (1 - 0.5f), ForceMode.Impulse);
                poncherCharacter.GetAnimator().SetInteger("JumpType", 0);
                poncherCharacter.GetAnimator().SetBool("Jumping", true);
            }
          
        }
       
    }


    public void DoubleJump()
    {
        canDoubleJump = false;
        poncherCharacter.GetRigidbody().AddForce(doubleJumpForce.y * Vector2.up, ForceMode.Impulse);
        poncherCharacter.GetRigidbody().AddForce((Vector2.right * -1) * poncherCharacter.GetRigidbody().velocity.x * (1 - 0.5f), ForceMode.Impulse);
        poncherCharacter.GetAnimator().SetInteger("JumpType", 2);
        poncherCharacter.GetAnimator().SetBool("Jumping", true);
    }

    public void RestoreJump()
    {
        canJump = true;
    }

    public void EndJump()
    {
        poncherCharacter.GetAnimator().SetBool("Jumping", false);
        //if (poncherCharacter.GetRigidbody().velocity.y > -4 || poncherCharacter.isGrounded || poncherCharacter.isWalled)
        //{
            
        //}

    }
}
