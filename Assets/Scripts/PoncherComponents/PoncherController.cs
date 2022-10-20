using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoncherController : PoncherComponentBase
{
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

    [Space(20)]
    public Vector3 m_unitGoal = Vector3.zero;
    public Vector3 m_goalVel = Vector3.zero;


    [Header("Inputs and Directions")]
    public Vector3 inputDirection;
    public Vector3 moveDirection;
    PoncherInputActions poncherActions;
    [Header("Input Properties")]
    PlayerInput poncherInput;
    public float flipForce;




    //public Vector3 currentAngVel;

    Vector3 screenMovementForward, screenMovementRight;
    private Quaternion screenMovementSpace = Quaternion.identity;




    //public float MaxSpeed;
    //public float Aceleration;
    //public float MaxAccelForce;
    //public float groundVel;
    //public AnimationCurve AcelerationFactorFromDot;


    private void Awake()
    {
        poncherInput = GetComponent<PlayerInput>();
        poncherInput.onActionTriggered += PeayerInputOnActionTriggered;

        poncherActions = new PoncherInputActions();
        poncherActions.PlayerGameplay.Enable();// Actiovating buttons for gameplay We can switch to UI or anything else
        poncherActions.PlayerGameplay.Jump.started += GetComponent<JumpComponent>().Jump;
        poncherActions.PlayerGameplay.Roll.started += GetComponent<RollComponent>().ParkourRoll;
        //poncherActions.PlayerGameplay.Movement.performed += CalculateInputs;
    }

    private void PeayerInputOnActionTriggered(InputAction.CallbackContext context)
    {

        //Debug.Log(context);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateInputDirection();
      

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Get click operations
        }

        //Switch between Action maps
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            //poncherInput.SwitchCurrentActionMap("UI"); // C# events
            poncherActions.PlayerGameplay.Enable();
            //Disable the other current map manually when we have a c# class
        }
    }


    void CalculateInputDirection()
    {
       
        Vector3 inputVector = poncherActions.PlayerGameplay.Movement.ReadValue<Vector2>();

        float h = inputVector.x;
        float v = inputVector.y;

        ////adjust movement values if we're in the air or on the ground
        //curAccel = (m_poncherCharacter.isGrounded) ? accel : airAccel;
        //curDecel = (m_poncherCharacter.isGrounded) ? decel : airDecel;
        //curRotateSpeed = (m_poncherCharacter.isGrounded) ? rotateSpeed : airRotateSpeed;



        //get movement axis relative to camera
        screenMovementForward = screenMovementSpace * Camera.main.transform.forward;
        screenMovementRight = screenMovementSpace * Camera.main.transform.right;

        //only apply vertical input to movemement, if player is not sidescroller
        if (!m_poncherCharacter.isSidescroller)
            inputDirection = (screenMovementForward * v) + (screenMovementRight * h);
        else
            inputDirection = Vector3.right * h;


        if (inputDirection.magnitude > 1.0f)
            inputDirection.Normalize();

        moveDirection = transform.position + inputDirection;

        Debug.DrawRay(transform.position, moveDirection, Color.green);
        Debug.DrawRay(transform.position, inputDirection * 2f, Color.red);



        //Flipping capsule
        if (Input.GetMouseButton(0))
        {
            if (!m_poncherCharacter.isGrounded)
            {
                //isStabilizing = false;
                uprightForce = 0;
                uprightSpringDamper = 0;

                Vector3 direction = inputDirection;
                //direction.x = 0;
                //direction.y = 0;
                //direction.z = 0;
                float inertia = m_poncherCharacter.GetRigidbody().inertiaTensor.z;
                float torque = inertia * m_poncherCharacter.GetRigidbody().angularVelocity.magnitude;
                m_poncherCharacter.GetRigidbody().AddRelativeTorque(Vector3.right * flipForce * inertia, ForceMode.VelocityChange);
            }
        }
        else
        {
            if (! m_poncherCharacter.isGrounded)
            {
                uprightForce = 2;
                uprightForce = 2;
                uprightSpringDamper = 1.5f;
                //isStabilizing = true;
            }

        }
    }



    private void FixedUpdate()
    {
        UpdateUprightForce();

        if (!CheckBasePreconditions())
            return;

        m_poncherCharacter.GetMoveComponent().MoveTo(moveDirection, 0.05f, true);

        if (inputDirection.magnitude != 0)
            m_poncherCharacter.GetMoveComponent().RotateToDirection(inputDirection, true); 
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
        Vector3 neededAccel = (m_goalVel - m_poncherCharacter.GetRigidbody().velocity) / Time.fixedDeltaTime;

        float maxAccel = MaxAccelForce * MaxAcelerationForceFactorFromDot.Evaluate(velDot) * maxAcelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        m_poncherCharacter.GetRigidbody().AddForce(Vector3.Scale(neededAccel * m_poncherCharacter.GetRigidbody().mass, ForceScale));


        //Vector3 goalVel = m_unitGoal * MaxSpeed * speedFactor;

        //m_goalVel = Vector3.MoveTowards(m_goalVel, (goalVel) + (groundVel), acc);
            
        //rigidBodiePoncher.AddForce(inputDirection * 5f * Time.deltaTime, ForceMode.VelocityChange);

    }
    public void FloatingCapsule(RaycastHit hit)
    {
        Vector3 vel = m_poncherCharacter.GetRigidbody().velocity;
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

        m_poncherCharacter.GetRigidbody().AddForce(rayDir * springForce);

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

        m_poncherCharacter.GetRigidbody().AddTorque((rotAxis * (rotRadians * uprightForce)) - (m_poncherCharacter.GetRigidbody().angularVelocity * uprightSpringDamper));
    }
    #endregion




}
