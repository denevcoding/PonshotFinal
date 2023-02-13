using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : PoncherComponentBase
{
    [Header("Movement")]
    public float accel;
    public float decel;
    [Space(5)]
    public float airAccel;
    public float airDecel;

    public float maxSpeed;//maximum speed of movement in X/Z axis
    private float curAccel, curDecel, curRotateSpeed, slope;

    [Range(0f, 5f)]
    public float rotateSpeed, airRotateSpeed; //how fast to rotate on the ground, how fast to rotate in the air

    public Vector3 movingObjSpeed = Vector3.zero;

    //Movement Vectors
    [HideInInspector] public Vector3 m_currentSpeed;
    [HideInInspector] public float m_DistanceToTarget;

    public Vector3 currentAngVel;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //adjust movement values if we're in the air or on the ground
        curAccel = (poncherCharacter.isGrounded) ? accel : airAccel;
        curDecel = (poncherCharacter.isGrounded) ? decel : airDecel;
        curRotateSpeed = (poncherCharacter.isGrounded) ? rotateSpeed : airRotateSpeed;
    }

    private void FixedUpdate()
    {
        ManageSpeed(curDecel, + movingObjSpeed.magnitude, true);
        currentAngVel = poncherCharacter.GetRigidbody().angularVelocity;
    }


    //Movement
    public bool MoveTo(Vector3 destination, float stopDistance, bool ignoreY)
    {
        if (!poncherCharacter.canMove)
            return false;

        poncherCharacter.GetRigidbody().WakeUp();
       

        Vector3 relativePos = (destination - transform.position);
        if (ignoreY)
            relativePos.y = 0;

        m_DistanceToTarget = relativePos.magnitude;
        if (m_DistanceToTarget <= stopDistance)
            return true;
        else
            poncherCharacter.GetRigidbody().AddForce(relativePos * curAccel * Time.deltaTime, ForceMode.VelocityChange);

        return false; // Keep moving we havent arrive
    }
    public void ManageSpeed(float deceleration, float maxSpeed, bool ignoreY)
    {
        m_currentSpeed = poncherCharacter.GetRigidbody().velocity;
        if (ignoreY)
            m_currentSpeed.y = 0;

        if (m_currentSpeed.magnitude > 0)
        {
            poncherCharacter.GetRigidbody().AddForce((m_currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
            if (poncherCharacter.GetRigidbody().velocity.magnitude > maxSpeed)
                poncherCharacter.GetRigidbody().AddForce((m_currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
        }

        //m_poncherCharacter.GetAnimator().SetFloat("xVe");
    }



    //Rotation
    public void RotateVelocity(float turnSpeed, bool ignoreY)
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

        if (direction.magnitude > 0.1f)
        {
            Quaternion dirQ = Quaternion.LookRotation(direction);
            Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * turnSpeed * Time.deltaTime);
            poncherCharacter.GetRigidbody().MoveRotation(slerp);
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

        
        //Handle turn 180 when change direction drastically
        float direction = Vector3.Dot(towardDir, transform.forward);
        if (poncherCharacter.canRotate && !poncherCharacter.isRotBlocked)
            poncherCharacter.GetAnimator().SetFloat("changeDirection", direction);
        else
            poncherCharacter.GetAnimator().SetFloat("ChangeDirection", 0.01f);

        direction = 0;

        if (poncherCharacter.isRotBlocked)
            towardDir *= -1;
        


        //if (canRotate)
        //{
        //    animatorPoncher.SetFloat("ChangeDirection", direction);
        //}
        //else
        //{
        //    animatorPoncher.SetFloat("ChangeDirection", 0.01f);
        //}

        //direction = 0;


        ////Debug.DrawRay(transform.position, lookDir, Color.black);
        //Debug.DrawRay(transform.position, inputDirection, Color.red);
        //Debug.DrawRay(transform.position, transform.forward * 3f, Color.blue);


        //if (canRotate == false)
        //    return;

        float turnSpee = curRotateSpeed *= 1.6f;
        Quaternion dirQ = Quaternion.LookRotation(towardDir);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, turnSpee * Time.deltaTime);
        poncherCharacter.GetRigidbody().MoveRotation(slerp);
    }







    public void SetGroundAccel(float newAccel)
    {
        accel = newAccel;
    }
    public void ResetGroundAccel()
    {
        //accel = m_poncherCharacter.m_poncherInfo.accel;
    }





    #region Getter Setters
    public float GetCurRotSpeed()
    {
        return curRotateSpeed;
    }
    #endregion

}
