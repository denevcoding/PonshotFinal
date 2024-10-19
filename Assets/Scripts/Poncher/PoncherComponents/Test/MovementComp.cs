using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementComp : PoncherComponentBase
{
    [Header("Rotation Properties")]
    [Range(0f, 5f)]
    public float curRotateSpeed;
    public float rotateSpeed, airRotateSpeed; //how fast to rotate on the ground, how fast to rotate in the air

    [Header("Movement Properties")]
    public Vector3 velocity;
    public float velMag;
    public float moveSpeed;
    public float acceleration;
    public float deceleration;

    [Space(5)]
    public float airMoveSpeed;
    public float airAccel;
    public float airDecel;

    [Header("Fast Fall")]
    public bool isFallingFast;
    public float curFallVel;
    public float NormalFallVel;
    public float FastFallVel;


    [Header("Landing")]
    public float landingForce = 0;

    float currMovSpeed;
    float currAccel;
    float currDecel;

    public float velPower;
    public float frictionAmount;

    [Header("Speed Multipliers")]
    public float m_LocomotionMultiplier;

    // Gravity Scale editable on the inspector
    // providing a gravity scale per object
    public float gravityScale = 1.0f;

    // Global Gravity doesn't appear in the inspector. Modify it here in the code
    // (or via scripting) to define a different default gravity for all objects.
    public static float globalGravity = -30;

    // Start is called before the first frame update
    void Start()
    {
        isFallingFast = false;
        NormalFallVel = -22;
        FastFallVel = -30;
        curFallVel = -22;
        poncherCharacter.GetRigidbody().useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSpeeds();  
    }

    public void FixedUpdate()
    {
        //poncherCharacter.GetRigidbody().WakeUp();
        ApplyGravity();


        velocity = poncherCharacter.GetRigidbody().velocity;
        velMag = poncherCharacter.GetRigidbody().velocity.magnitude;


        //Clamping maximum fall Y velocity
        if (velocity.y < curFallVel)
        {
            velocity.y = curFallVel;
            poncherCharacter.GetRigidbody().velocity = velocity;
        }

        //Calculating Landing force and setting animator properties
        if (!poncherCharacter.isGrounded && velocity.y < 0f)
        {
            landingForce = Mathf.Abs(Mathf.Round(GetComponent<Rigidbody>().velocity.y));
            poncherCharacter.GetAnimator().SetFloat("LandingForce", landingForce);          
            poncherCharacter.GetAnimator().SetFloat("VelocityY", poncherCharacter.GetRigidbody().velocity.y);
        }
        else
        {
            poncherCharacter.GetAnimator().SetFloat("VelocityY", 0f);
        }
        
    }

    public void CalculateSpeeds()
    {
        //Set rotation values
        curRotateSpeed = (poncherCharacter.isGrounded) ? rotateSpeed : airRotateSpeed;

        //Fall Speeds
        if (poncherCharacter.isGrounded || poncherCharacter.isWalled)
        {
            isFallingFast = false;
            curFallVel = NormalFallVel;
        }
      
        curFallVel = isFallingFast ? FastFallVel : NormalFallVel;
        

        //adjust movement values if we're in the air or on the ground
        currMovSpeed = (poncherCharacter.isGrounded) ? moveSpeed : airMoveSpeed;
        currAccel = (poncherCharacter.isGrounded) ? acceleration : airAccel;
        currDecel = (poncherCharacter.isGrounded) ? deceleration : airDecel;

        //Applying Multipliers
        currMovSpeed *= m_LocomotionMultiplier;

    }

    public void MovePoncher(Vector2 _inputDir)
    {
        //if (!poncherCharacter.canMove)
        //    return;
        poncherCharacter.GetAnimator().SetFloat("DistanceToTarget", Mathf.Abs(_inputDir.x));


        Vector3 velX = velocity;
        velX.z = 0f;
        velX.y = 0f;


        float targetSpeed = poncherCharacter.GetController().GetInputDirection().x * currMovSpeed;
        // Calculate desired velocity based on input
        Vector3 desiredVel = new Vector3(targetSpeed, 0f, 0f);    

        // Calculate direction from current velocity to desired velocity
        Vector3 direction = desiredVel - velX;
        Debug.DrawRay(transform.position, direction, Color.white);

        float speedDif = desiredVel.x - velocity.x;
        float accelRate = (Mathf.Abs(desiredVel.x) > 0.01f) ? currAccel : currDecel;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(direction.x);

      
        // Move current velocity towards desired velocity based on sliding speed
        poncherCharacter.GetRigidbody().AddForce(movement * Vector2.right);

     

        if (velX.magnitude > 0)
        {
            //Debug.Log("entrando este hpta");
            poncherCharacter.GetAnimator().SetFloat("CurrSpeed", velX.magnitude);

            // Calculate the dot product between the velocity and the desired movement direction
            float dotProduct = Vector3.Dot(velX.normalized, direction.normalized);


            // Check if the dot product is negative, indicating deceleration or change in direction
            if (dotProduct < 0f)
            {
                float changeDirection = Vector3.Dot(poncherCharacter.GetController().GetInputDirection(), direction);
                if (changeDirection > 0)
                {
                    poncherCharacter.GetAnimator().SetBool("ChangeDir", true);
                    poncherCharacter.GetAnimator().SetBool("IsSliding", false);
                }
                else
                {
                    poncherCharacter.GetAnimator().SetBool("ChangeDir", false);
                    // Check if the character's velocity is below a certain threshold
                    // and if the direction magnitude is above a certain threshold
                    if ((velX.magnitude < currMovSpeed && velX.magnitude > 5f) && direction.magnitude > 1)
                    {
                        // Set sliding animation
                        poncherCharacter.GetAnimator().SetFloat("Direction", direction.magnitude);
                        poncherCharacter.GetAnimator().SetBool("IsSliding", true);
                    }
                    else
                    {
                        // Not sliding when velocity or direction does not meet the thresholds
                        poncherCharacter.GetAnimator().SetBool("IsSliding", false);
                    }
                }

              
            }
            else
            {
                // Not sliding
                poncherCharacter.GetAnimator().SetBool("IsSliding", false);
                poncherCharacter.GetAnimator().SetBool("ChangeDir", false);
            }          
           
        }        
        else
        {
            poncherCharacter.GetAnimator().SetFloat("CurrSpeed", 0f);
            poncherCharacter.GetAnimator().SetBool("IsSliding", false);
        }

       
       
    }

    public void ManageSpeed()
    {
        //applyig friction Manually
        if (poncherCharacter.isGrounded && Mathf.Abs(poncherCharacter.GetController().movInputDirection.magnitude) < 0.01f && velocity.x > 0.05f)
        {
            float amount = Mathf.Min(Mathf.Abs(velocity.x), Mathf.Abs(frictionAmount));

            amount *= Mathf.Sign(velocity.x);

            poncherCharacter.GetRigidbody().AddForce(Vector2.right * -amount, ForceMode.Impulse);
        }
    }


    public void RotateToDirection(Vector3 lookDir, bool ignoreY)
    {
        //if (!poncherCharacter.canRotate)
        //    return;

        Vector3 characterPos = transform.position;
        if (ignoreY)
        {
            characterPos.y = 0;
            lookDir.y = 0;
        }
        //Un número mayor o menor a este hará que mire hacia el otro lado al voltear
        lookDir.z = 0;
        Vector3 towardDir = lookDir - characterPos;

        ////Handle turn 180 when change direction drastically
        // float direction = Vector3.Dot(towardDir, transform.forward);
        //if (poncherCharacter.canRotate && !poncherCharacter.isRotBlocked)
        //    poncherCharacter.GetAnimator().SetFloat("changeDirection", direction);
        //else
        //    poncherCharacter.GetAnimator().SetFloat("ChangeDirection", 0.01f);

        //direction = 0;


        float velFactor = (1 * velMag) / moveSpeed;
        float velDir = 1;

        if (poncherCharacter.GetController().lockRotation)
        {
            float alingment = Vector3.Dot(poncherCharacter.GetController().lastLookDirection, poncherCharacter.GetController().lookInputDirection);
            if (alingment > 0)
            {
                Debug.Log("Align");
           
            }
            else
            {
                Debug.Log("UnAlign");
                lookDir *= -1;
                velDir = -1;
            }

        }

        velFactor *= velDir;

        //If It is running wth input but is against a wall for example
        if (poncherCharacter.GetController()._lookDir.magnitude > 0 && velMag <= 0.1f)        
            velFactor = 0.2f * velDir;

        poncherCharacter.GetAnimator().SetFloat("VelocityX", velFactor);
        


        float turnSpee = curRotateSpeed;
        Quaternion dirQ = Quaternion.LookRotation(lookDir);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, turnSpee * Time.deltaTime);
        poncherCharacter.GetRigidbody().MoveRotation(slerp);
               
    }


    public void RotateVelocity(bool ignoreY)
    {
        Vector3 direction;
        if (ignoreY)
        {
            direction = new Vector3(poncherCharacter.GetRigidbody().velocity.x, 0f, poncherCharacter.GetRigidbody().velocity.z);
        }
        else
        {
            direction = poncherCharacter.GetRigidbody().velocity;
        }

        if (direction.magnitude > 0.20f)
        {
            Quaternion dirQ = Quaternion.LookRotation(direction);
            Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * curRotateSpeed * Time.deltaTime);
            poncherCharacter.GetRigidbody().MoveRotation(slerp);
        }
       
    }




    public void ApplyGravity()
    {
        //Applying Gravity
        Vector3 gravity = (/*Physics.gravity.y + */globalGravity) * gravityScale * Vector3.up;
        poncherCharacter.GetRigidbody().AddForce(gravity, ForceMode.Acceleration);
    }



    #region Multipliers
    public void SetLocomotionFactor(float factor)
    {
        m_LocomotionMultiplier = factor;
    }
    public void RestoreLocomotionFactor()
    {
        m_LocomotionMultiplier = 1;
    }
    #endregion


    public void SwitchFastFallVel(bool _isFastFall)
    {
       
    }
}
