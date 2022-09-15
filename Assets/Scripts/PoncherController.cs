using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoncherController : MonoBehaviour
{
    Rigidbody rigidBodiePoncher;
    CapsuleCollider colliderPoncher;

    [Header("Ground Detection")]
    public float rayLenght;
    int groundedLayerMask;
    public bool isGrounded;

    [Header("Floating Properties")]
    public float RideHeight;
    public float RideSpringStrenght;
    public float RideSpringDamper;

    [Header("Estabilization Properties")]
    public float uprightForce;
    public float uprightSpringDamper;
    public bool isStabilizing = true;


    [Header("Locomotion")]
    public float MaxSpeed;
    public float Acceeleration;
    public AnimationCurve AcelerationFactorFromDot;
    public float MaxAccelForce;
    public AnimationCurve MaxAcelerationForceFactorFromDot;
    public Vector3 ForceScale;
    public float GravityScaleDrop;


    [Header("Inputs and Directions")]
    public Vector3 inputDirection;
    public Vector3 moveDirection;

    [Header("Movement")]
    public float accel;
    public float airAccel;
    public float decel;
    public float airDecel;


    public float maxSpeed;//maximum speed of movement in X/Z axis
    private float curAccel, curDecel, curRotateSpeed, slope;

    [Range(0f, 5f)]
    public float rotateSpeed, airRotateSpeed; //how fast to rotate on the ground, how fast to rotate in the air
    
    Vector3 movingObjSpeed = Vector3.zero;

    //Movement Vectors
    [HideInInspector] public Vector3 m_currentSpeed;
    [HideInInspector] public float m_DistanceToTarget;




    [Header("Jumping")]
    public Vector3 jumpForce;
    public float coyoteTime;
    public float coyoteTimeCounter;
    public float flipForce;

    public Vector3 currentAngVel;


    Vector3 screenMovementForward, screenMovementRight;
    private Quaternion screenMovementSpace = Quaternion.identity;


    [Space(20)]
    public Vector3 m_unitGoal = Vector3.zero;
    public Vector3 m_goalVel = Vector3.zero;

    //public float MaxSpeed;
    //public float Aceleration;
    //public float MaxAccelForce;
    //public float groundVel;
    //public AnimationCurve AcelerationFactorFromDot;


    private void Awake()
    {
        rigidBodiePoncher = GetComponent<Rigidbody>();
        colliderPoncher = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        groundedLayerMask = LayerMask.GetMask("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            //On the ground
            coyoteTimeCounter = coyoteTime;
            uprightForce = 50;
            uprightSpringDamper = 10;
            isStabilizing = true;
        }
        else
        {
            //Falling or in the air
            coyoteTimeCounter -= Time.deltaTime;
        }

        CalculateInputs();
        

        
    }


    void CalculateInputs()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //adjust movement values if we're in the air or on the ground
        curAccel = (isGrounded) ? accel : airAccel;
        curDecel = (isGrounded) ? decel : airDecel;
        curRotateSpeed = (isGrounded) ? rotateSpeed : airRotateSpeed;


        //get movement axis relative to camera
        screenMovementForward = screenMovementSpace * Camera.main.transform.forward;
        screenMovementRight = screenMovementSpace * Camera.main.transform.right;

        //only apply vertical input to movemement, if player is not sidescroller
        if (false)
            inputDirection = (screenMovementForward * v) + (screenMovementRight * h);
        else
            inputDirection = Vector3.right * h;


        if (inputDirection.magnitude > 1.0f)
            inputDirection.Normalize();

        moveDirection = transform.position + inputDirection;

        Debug.DrawRay(transform.position, moveDirection, Color.green);

        Debug.DrawRay(transform.position, inputDirection * 2f, Color.red);

        Debug.DrawRay(transform.position, rigidBodiePoncher.velocity);

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            coyoteTimeCounter = -2;
            Jump(jumpForce);
        }

        if (Input.GetMouseButton(0))
        {
            if (!isGrounded)
            {
                //isStabilizing = false;
                uprightForce = 0;
                uprightSpringDamper = 0;

                Vector3 direction = inputDirection;
                //direction.x = 0;
                //direction.y = 0;
                //direction.z = 0;
                float inertia = rigidBodiePoncher.inertiaTensor.z;
                float torque = inertia * rigidBodiePoncher.angularVelocity.magnitude;
                rigidBodiePoncher.AddRelativeTorque(Vector3.right * flipForce * inertia, ForceMode.VelocityChange);
            }
        }
        else
        {
            if (!isGrounded)
            {
                uprightForce = 2;
                uprightSpringDamper = 1.5f;
                //isStabilizing = true;
            }

        }
    }



    private void FixedUpdate()
    {
        currentAngVel = rigidBodiePoncher.angularVelocity;

        MoveTo(moveDirection, curAccel, 0.05f, true);

        if (rotateSpeed != 0 && inputDirection.magnitude != 0)        
            RotateToDirection(inputDirection,curRotateSpeed, true); 
        ManageSpeed(curDecel, maxSpeed + movingObjSpeed.magnitude, true);

        //Movement();
        UpdateUprightForce();
    }

    //Ground Cheking
    public bool IsGrounded()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y + rayLenght;

        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * (dist), Color.cyan, 0.05f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dist + 0.05f, groundedLayerMask))
        {
            //FloatingCapsule(hit);
            return true;
            
        }

        return false;
    }


    //Movement
    public bool MoveTo(Vector3 destination, float acceleration, float stopDistance, bool ignoreY)
    {
        //if (canMove == false)
        //    return false;

        Vector3 relativePos = (destination - transform.position);
        if (ignoreY)
            relativePos.y = 0;

        m_DistanceToTarget = relativePos.magnitude;
        if (m_DistanceToTarget <= stopDistance)
            return true;
        else
            rigidBodiePoncher.AddForce(relativePos * acceleration * Time.deltaTime, ForceMode.VelocityChange);

        return false; // Keep moving we havent arrive
    }
    public void ManageSpeed(float deceleration, float maxSpeed, bool ignoreY)
    {
        m_currentSpeed = rigidBodiePoncher.velocity;
        if (ignoreY)
            m_currentSpeed.y = 0;

        if (m_currentSpeed.magnitude > 0)
        {
            rigidBodiePoncher.AddForce((m_currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
            if (rigidBodiePoncher.velocity.magnitude > maxSpeed)
                rigidBodiePoncher.AddForce((m_currentSpeed * -1) * deceleration * Time.deltaTime, ForceMode.VelocityChange);
        }
    }



    //Rotation
    public void RotateVelocity(float turnSpeed, bool ignoreY)
    {
        Vector3 direction;
        if (ignoreY)
        {
            direction = new Vector3(rigidBodiePoncher.velocity.x, 0f, rigidBodiePoncher.velocity.z);            
        }
        else
        {
            direction = rigidBodiePoncher.velocity;
        }

        if (direction.magnitude > 0.1f)
        {
            Quaternion dirQ = Quaternion.LookRotation(direction);
            Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * turnSpeed * Time.deltaTime);
            rigidBodiePoncher.MoveRotation(slerp);
        }
    }
    public void RotateToDirection(Vector3 lookDir, float turnSpeed, bool ignoreY)
    {

        Vector3 characterPos = transform.position;
        if (ignoreY)
        {
            characterPos.y = 0;
            lookDir.y = 0;
        }
        //Un número mayor o menor a este hará que mire hacia el otro lado al voltear
        lookDir.z = 0;


        //inputDirection = lookDir - characterPos;
        //float direction = Vector3.Dot(inputDirection, transform.forward);

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

        turnSpeed *= 1.6f;
        Quaternion dirQ = Quaternion.LookRotation(inputDirection);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, turnSpeed * Time.deltaTime);
        rigidBodiePoncher.MoveRotation(slerp);
    }







    public void Jump(Vector3 force) {

       
            rigidBodiePoncher.AddRelativeForce(force, ForceMode.Impulse);
            
        
    }




    #region Floating Capsule Methods
    void Movement()
    {
        Vector3 groundVel = new Vector3 (0, 0, 0);

        float speedFactor = 5f;
        float maxAcelForceFactor = 5f;
        m_unitGoal = inputDirection;

        //Calculate new goal
        Vector3 unitVel = m_goalVel.normalized;
        float velDot = Vector3.Dot(m_unitGoal, unitVel);

        float accel = Acceeleration * AcelerationFactorFromDot.Evaluate(velDot);

        Vector3 goalVel = m_unitGoal * MaxSpeed * speedFactor;

        m_goalVel = Vector3.MoveTowards(m_goalVel, 
                                       (goalVel) + (groundVel)
                                       , accel * Time.fixedDeltaTime);

        //Actual Fore
        Vector3 neededAccel = (m_goalVel - rigidBodiePoncher.velocity) / Time.fixedDeltaTime;

        float maxAccel = MaxAccelForce * MaxAcelerationForceFactorFromDot.Evaluate(velDot) * maxAcelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        rigidBodiePoncher.AddForce(Vector3.Scale(neededAccel * rigidBodiePoncher.mass, ForceScale));


        //Vector3 goalVel = m_unitGoal * MaxSpeed * speedFactor;

        //m_goalVel = Vector3.MoveTowards(m_goalVel, (goalVel) + (groundVel), acc);
            
        //rigidBodiePoncher.AddForce(inputDirection * 5f * Time.deltaTime, ForceMode.VelocityChange);

    }
    void FloatingCapsule(RaycastHit hit)
    {
        Vector3 vel = rigidBodiePoncher.velocity;
        Vector3 rayDir = transform.TransformDirection(Vector3.down);

        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = hit.rigidbody;

        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }

        float rayDirVel = Vector3.Dot(rayDir, vel);
        float otherDirVel = Vector3.Dot(rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;

        float x = hit.distance - RideHeight;

        float springForce = (x * RideSpringStrenght) - (relVel * RideSpringDamper);

        rigidBodiePoncher.AddForce(rayDir * springForce);

        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
        }
    }
    void UpdateUprightForce()
    {
        if (!isStabilizing)
            return;
        //Quaternion upRightTargetRot = GetTargetRot(Vector3.up, Vector3.right);

        Quaternion characterCurrent = transform.rotation;

        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        Quaternion toGoal = MathUtils.ShortestRotation(q, characterCurrent);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        rigidBodiePoncher.AddTorque((rotAxis * (rotRadians * uprightForce)) - (rigidBodiePoncher.angularVelocity * uprightSpringDamper));
    }
    #endregion




}
