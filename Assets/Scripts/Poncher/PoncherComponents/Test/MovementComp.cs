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
        if (velocity.y < -31)
        {
            velocity.y = -31;
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

        //adjust movement values if we're in the air or on the ground
        currMovSpeed = (poncherCharacter.isGrounded) ? moveSpeed : airMoveSpeed;
        currAccel = (poncherCharacter.isGrounded) ? acceleration : airAccel;
        currDecel = (poncherCharacter.isGrounded) ? deceleration : airDecel;

        //Applying Multipliers
        currMovSpeed *= m_LocomotionMultiplier;

    }

    public void MovePoncher(Vector2 _inputDir)
    {
        if (!poncherCharacter.canMove)
            return;

        float targetSpeed = poncherCharacter.GetController().GetInputDirection().x * currMovSpeed;

        float speedDif = targetSpeed - poncherCharacter.GetRigidbody().velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? currAccel : currDecel;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        poncherCharacter.GetRigidbody().AddForce(movement * Vector2.right);



        if (poncherCharacter.IsGrounded() && Mathf.Abs(poncherCharacter.GetController().inputDirection.magnitude) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(poncherCharacter.GetRigidbody().velocity.x), Mathf.Abs(frictionAmount));

            amount *= Mathf.Sign(poncherCharacter.GetRigidbody().velocity.x);

            poncherCharacter.GetRigidbody().AddForce(Vector2.right * -amount, ForceMode.Impulse);
        }
    }


    public void RotateToDirection(Vector3 lookDir, bool ignoreY)
    {
        if (!poncherCharacter.canRotate)
            return;

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

        if (poncherCharacter.isStrafing)
        {
            lookDir *= -1;
            velDir = -1;
        }

        velFactor *= velDir;

        //If It is running wth input but is against a wall for example
        if (poncherCharacter.GetController().moveDirection.magnitude > 0 && velMag <= 0.05f)        
            velFactor = poncherCharacter.GetController().moveDirection.magnitude * velDir;

        poncherCharacter.GetAnimator().SetFloat("VelocityX", velFactor);


        float turnSpee = curRotateSpeed;
        Quaternion dirQ = Quaternion.LookRotation(lookDir);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, turnSpee * Time.deltaTime);
        poncherCharacter.GetRigidbody().MoveRotation(slerp);


       
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
}
