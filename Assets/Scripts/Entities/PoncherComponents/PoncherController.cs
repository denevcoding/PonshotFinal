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
    public PoncherInputActions poncherActions;
    [Header("Input Properties")]
    PlayerInput poncherInput;
    public float flipForce;

    [Space(10)]
    public Queue<InputAction.CallbackContext> inputBuffer;
    public float jumpBufferTime;


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
        poncherInput.onActionTriggered += PlayerInputOnActionTriggered;

        inputBuffer = new Queue<InputAction.CallbackContext>();

        poncherActions = new PoncherInputActions();
        poncherActions.PlayerGameplay.Enable();// Actiovating buttons for gameplay We can switch to UI or anything else

        //Left Shoulder L1 Bindings
        poncherActions.PlayerGameplay.L1.performed += LeftShoulder;
        poncherActions.PlayerGameplay.L1.canceled += LeftShoulder;

        poncherActions.PlayerGameplay.Jump.started += GetComponent<JumpComponent>().JumpWithPressed;
        //poncherActions.PlayerGameplay.Jump.performed += GetComponent<JumpComponent>().JumpWithPressed;
        poncherActions.PlayerGameplay.Jump.canceled += GetComponent<JumpComponent>().JumpWithPressed;

        //poncherActions.PlayerGameplay.Roll.started += GetComponent<RollComponent>().ParkourRoll;

        // Y or Tringle Button
        poncherActions.PlayerGameplay.Ragdoll.started += SwitcBones;

        //poncherActions.PlayerGameplay.Movement.performed += CalculateInputs;
    }

    private void PlayerInputOnActionTriggered(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.name);
        //if (poncherActions.FindAction(context.action.name))
        //{

        //}
        //if (poncherActions.FindBinding(context))
        //{

        //}
        //InputAction a = poncherActions.FindAction(context.action.ToString());
        //int holi = 0;
        //Debug.Log(a.ToString());

        //poncherActions.Contains(context.action);
        //poncherActions.
        if (context.action.name == poncherActions.PlayerGameplay.Jump.name)
        {
            if (context.started && !poncherCharacter.IsGrounded())
            {
                AddActionToBuffer(context);
            }
            
        }
        ////Debug.Log(context);
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

        if (poncherCharacter.isGrounded)
        {
            Debug.Log(inputBuffer.Count);
            ExecuteInputBuffer();
        }
    }


   



    private void FixedUpdate()
    {
        //UpdateUprightForce();

        if (!CheckBasePreconditions())
            return;

        //poncherCharacter.GetMoveComponent().MoveTo(moveDirection, 0.05f, true);

        if (poncherCharacter.rotToVel == false)
        {
            if (poncherCharacter.GetMoveComponent().GetCurRotSpeed() != 0 && inputDirection.magnitude != 0)
            {
                poncherCharacter.GetMoveComponent().RotateToDirection(moveDirection, true);
            }
        }
        else
        {
            poncherCharacter.GetMoveComponent().RotateVelocity(poncherCharacter.GetMoveComponent().GetCurRotSpeed(), true);
        }

    }

    public void AddActionToBuffer(InputAction.CallbackContext action)
    {
        if (action.started)
        {
            inputBuffer.Enqueue(action);
            Invoke("CleanAction", jumpBufferTime);
        }
    }
    public void ExecuteInputBuffer()
    {
        if (inputBuffer.Count > 0)
        {
            if (inputBuffer.Peek().action.name == poncherActions.PlayerGameplay.Jump.name)
            {             
                GetComponent<JumpComponent>().Jump(/*inputBuffer.Peek()*/);
                inputBuffer.Dequeue();
            }
        }
    }

    public void CleanAction()
    {
        if (inputBuffer.Count > 0)
        {
            inputBuffer.Dequeue();
        }      
    }

    void CalculateInputDirection()
    {

        Vector3 inputVector = poncherActions.PlayerGameplay.Movement.ReadValue<Vector2>();

        float h = inputVector.x;
        float v = inputVector.y;

        ////adjust movement values if we're in the air or on the ground
        //curAccel = (poncherCharacter.isGrounded) ? accel : airAccel;
        //curDecel = (poncherCharacter.isGrounded) ? decel : airDecel;
        //curRotateSpeed = (poncherCharacter.isGrounded) ? rotateSpeed : airRotateSpeed;



        //get movement axis relative to camera
        screenMovementForward = screenMovementSpace * Camera.main.transform.forward;
        screenMovementRight = screenMovementSpace * Camera.main.transform.right;

        //only apply vertical input to movemement, if player is not sidescroller
        if (!poncherCharacter.isSidescroller)
            inputDirection = (screenMovementForward * v) + (screenMovementRight * h);
        else
            inputDirection = Vector3.right * h;


        if (inputDirection.magnitude > 1.0f)
            inputDirection.Normalize();

        moveDirection = transform.position + inputDirection;

        //Debug.DrawRay(transform.position, moveDirection, Color.green);
        Debug.DrawRay(transform.position, inputDirection * 2f, Color.red);



        //Flipping capsule
        //if (Input.GetMouseButton(0))
        //{
        //    if (!poncherCharacter.isGrounded)
        //    {
        //        //isStabilizing = false;
        //        uprightForce = 0;
        //        uprightSpringDamper = 0;

        //        Vector3 direction = inputDirection;
        //        //direction.x = 0;
        //        //direction.y = 0;
        //        //direction.z = 0;
        //        float inertia = poncherCharacter.GetRigidbody().inertiaTensor.z;
        //        float torque = inertia * poncherCharacter.GetRigidbody().angularVelocity.magnitude;
        //        poncherCharacter.GetRigidbody().AddRelativeTorque(Vector3.right * flipForce * inertia, ForceMode.VelocityChange);
        //    }
        //}
        //else
        //{
        //    if (! poncherCharacter.isGrounded)
        //    {
        //        uprightForce = 2;
        //        uprightForce = 2;
        //        uprightSpringDamper = 1.5f;
        //        //isStabilizing = true;
        //    }

        //}
    }


    public void LeftShoulder(InputAction.CallbackContext context)
    {
        //Pressed
        if (context.performed)        
            poncherCharacter.isRotBlocked = true;        

        //Release
        if (context.canceled)        
            poncherCharacter.isRotBlocked = false;        

    }


    public void SwitcBones(InputAction.CallbackContext context)
    {
        if (poncherCharacter.GetRagdollCtrl().IsRagdoll == true)
        {
            RagdollController RG = poncherCharacter.GetRagdollCtrl();

            if (RG.HipGrounded() && RG.RootParent.gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
            {
                poncherCharacter.GetRagdollCtrl().SwitchBones(false);
            }           
        }
        else
        {
            poncherCharacter.GetRagdollCtrl().SwitchBones(true);
        }

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
        Vector3 neededAccel = (m_goalVel - poncherCharacter.GetRigidbody().velocity) / Time.fixedDeltaTime;

        float maxAccel = MaxAccelForce * MaxAcelerationForceFactorFromDot.Evaluate(velDot) * maxAcelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        poncherCharacter.GetRigidbody().AddForce(Vector3.Scale(neededAccel * poncherCharacter.GetRigidbody().mass, ForceScale));


        //Vector3 goalVel = m_unitGoal * MaxSpeed * speedFactor;

        //m_goalVel = Vector3.MoveTowards(m_goalVel, (goalVel) + (groundVel), acc);
            
        //rigidBodiePoncher.AddForce(inputDirection * 5f * Time.deltaTime, ForceMode.VelocityChange);

    }
    public void FloatingCapsule(RaycastHit hit)
    {
        Vector3 vel = poncherCharacter.GetRigidbody().velocity;
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

        poncherCharacter.GetRigidbody().AddForce(rayDir * springForce);

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

        poncherCharacter.GetRigidbody().AddTorque((rotAxis * (rotRadians * uprightForce)) - (poncherCharacter.GetRigidbody().angularVelocity * uprightSpringDamper));
    }
    #endregion



    #region Getters Setters
    public Vector3 GetInputDirection()
    {
        return inputDirection;
    }
    #endregion


}
