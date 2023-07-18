using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComp : PoncherComponentBase
{
    [Header("Movement Properties")]
    public float moveSpeed;
    public float acceleration;
    public float deceleration;

    [Space(5)]
    public float airMoveSpeed;
    public float airAccel;
    public float airDecel;

    float currMovSpeed;
    float currAccel;
    float currDecel;

    public float velPower;
    public float frictionAmount;

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
        //adjust movement values if we're in the air or on the ground
        currMovSpeed = (poncherCharacter.isGrounded) ? moveSpeed : airMoveSpeed;
        currAccel = (poncherCharacter.isGrounded) ? acceleration : airAccel;
        currDecel = (poncherCharacter.isGrounded) ? deceleration : airDecel;
        
    }

    public void FixedUpdate()
    {
        poncherCharacter.GetAnimator().SetFloat("DistanceToTarget", Mathf.Abs(poncherCharacter.GetController().inputDirection.x));
        poncherCharacter.GetRigidbody().WakeUp();

        ApplyGravity();


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


    public void ApplyGravity()
    {
        //Applying Gravity
        Vector3 gravity = (/*Physics.gravity.y + */globalGravity) * gravityScale * Vector3.up;
        poncherCharacter.GetRigidbody().AddForce(gravity, ForceMode.Acceleration);
    }
}
