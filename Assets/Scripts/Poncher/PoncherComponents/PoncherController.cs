using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RotationType
{
    ToInputDir,
    ToVelocity,
}

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
    public PlayerGUI playerGUI; //Aimer refernece
    public Vector3 movInputDirection;
    public Vector3 _lookDir;
    public Vector3 lookInputDirection;

    public Vector3 lastLookDirection;

    public RotationType m_RotType;
    public bool lockRotation; //lock the rotation to look the look input aimer
    public PoncherInputActions poncherActions;


    [Header("Input Properties")]
    PlayerInput poncherInput;
    //public delegate void AimBasedOnControlTypeFunc();
    //public AimBasedOnControlTypeFunc AimBasedOnControlType;
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
      
    }



    private void PlayerInputOnActionTriggered(InputAction.CallbackContext context)
    {
        //Debug.Log(context.action.name);
        
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
        //if (context.action.name == poncherActions.PlayerGameplay.Jump.name)
        //{
        //    if (context.started && !(poncherCharacter.coyoteTimeCounter < poncherCharacter.coyoteTime))
        //    {
        //        AddActionToBuffer(context);
        //    }
            
        //}
        ////Debug.Log(context);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_RotType = RotationType.ToInputDir;
        _lookDir = Vector3.right;
        lastLookDirection = Vector3.right;
    }

   

    // Update is called once per frame
    void Update()
    {
        //CalculateInputDirection();      

        //if (Mouse.current.leftButton.wasPressedThisFrame)
        //{
        //    //Get click operations
        //}

        ////Switch between Action maps
        //if (Keyboard.current.tKey.wasPressedThisFrame)
        //{
        //    //poncherInput.SwitchCurrentActionMap("UI"); // C# events
        //    poncherActions.PlayerGameplay.Enable();
        //    //Disable the other current map manually when we have a c# class
        //}

        //if (poncherCharacter.coyoteTimeCounter < poncherCharacter.coyoteTime)
        //{
        //    //Debug.Log(inputBuffer.Count);
        //    ExecuteInputBuffer();
        //}
    }



   



    private void FixedUpdate()
    {
        //UpdateUprightForce();

        CalculateInputDirection();
        poncherCharacter.GetMovementComp().RotateToDirection(lastLookDirection, true);
        poncherCharacter.GetMovementComp().MovePoncher(_lookDir);
        poncherCharacter.GetMovementComp().ManageSpeed();
    }





    #region Input Buffer
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
        float h = movInputDirection.x;
        float v = movInputDirection.y;


        //get movement axis relative to camera
        screenMovementForward = screenMovementSpace * Camera.main.transform.forward;
        screenMovementRight = screenMovementSpace * Camera.main.transform.right;

        //only apply vertical input to movemement, if player is not sidescroller
        if (!poncherCharacter.isSidescroller)
            _lookDir = (screenMovementForward * v) + (screenMovementRight * h);
        else
            _lookDir = Vector3.right * h;

      

        if (m_RotType == RotationType.ToVelocity)
        {
            Vector3 velDir = poncherCharacter.GetRigidbody().velocity;
            velDir.y = 0;

            if (velDir.magnitude > 0.45f)
            {
                float xDir = Mathf.Clamp(velDir.x, -1, 1);
                lastLookDirection = Vector3.right * xDir;
            }

            poncherCharacter.GetMovementComp().RotateVelocity(true);
        }
        
        if (m_RotType == RotationType.ToInputDir)
        {


            if (movInputDirection.magnitude > 0)
            {
                if (lockRotation)
                {
                    CalculateLastLookDir(movInputDirection *-1);
                }
                else
                {
                    CalculateLastLookDir(movInputDirection);
                }                
            }


            if (lookInputDirection.magnitude > 0)
            {
                CalculateLastLookDir(lookInputDirection);
            }
        

            if (poncherInput)
            {
                switch (poncherInput.currentControlScheme)
                {
                    case "Keyboard&Mouse":
                        if (lockRotation)
                        {
                            lookInputDirection = playerGUI.shooterPointer.AimWithMouse();
                            lastLookDirection = Vector3.right * lookInputDirection.x;
                        }
                        else
                            playerGUI.shooterPointer.RotateByInput(movInputDirection);

                        break;


                    case "Gamepad":
                        if (lookInputDirection.magnitude > 0f)
                            playerGUI.shooterPointer.RotateByInput(lookInputDirection);
                        else if(movInputDirection.magnitude > 0f)
                        {
                            if (lockRotation)                            
                                playerGUI.shooterPointer.RotateByInput(movInputDirection*-1);
                            else
                                playerGUI.shooterPointer.RotateByInput(movInputDirection);


                        }
                            
                       
                        
                        




                        //if (lockRotation)
                        //{
                        //    playerGUI.shooterPointer.RotateByInput(lookInputDirection);
                        //    lastLookDirection = Vector3.right * lookInputDirection.x;
                        //}
                        //else
                        //    playerGUI.shooterPointer.RotateByInput(movInputDirection);

                        break;
                }
            }



        }


        void CalculateLastLookDir(Vector2 _lookDir)
        {
            if (_lookDir.magnitude > 0)
            {
                if (_lookDir.x > 0)
                {
                    lastLookDirection = _lookDir.normalized;
                    lastLookDirection.x = 1;
                }
                else
                {
                    lastLookDirection = _lookDir.normalized;
                    lastLookDirection.x = -1;
                }
            }
        }


     



       // Debug.DrawRay(transform.position, moveDirection * 2f, Color.green);
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
    #endregion




    #region Input Actions Bindings
    public void BindInputActions(PlayerInput inputComp)
    {
        poncherInput = inputComp;

        Dictionary<string, Action<InputAction.CallbackContext>> ActionMap = new Dictionary<string, Action<InputAction.CallbackContext>>();
        
        //Navigation Actions
        ActionMap.Add("Movement", OnMoveInput);
        ActionMap.Add("Aim", OnLookInput);
        ActionMap.Add("Jump", poncherCharacter.GetJumpComp().JumpWithPressed);
        ActionMap.Add("Roll", poncherCharacter.GetRoll().FlipRoll);
        ActionMap.Add("L1", LockRotationbyKey);
        ActionMap.Add("DownFast", DownFast);
        ActionMap.Add("ThrowPlug", poncherCharacter.GetPickDropComp().ThrowLaunch);

        //Ball Actions
        ActionMap.Add("R1", poncherCharacter.GetPickDropComp().PickDrop);


        foreach (string keyAction in ActionMap.Keys)
        {
            poncherInput.actions[keyAction].started += ActionMap[keyAction];
            poncherInput.actions[keyAction].performed += ActionMap[keyAction];
            poncherInput.actions[keyAction].canceled += ActionMap[keyAction];
        }
        poncherInput.actions["Aim"].performed += OnLookInput; //Binding to right stick values
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movInputDirection = context.ReadValue<Vector2>();
        Debug.DrawRay(transform.position, movInputDirection * 2f, Color.green);
    }


    public void OnLookInput(InputAction.CallbackContext context)
    {
        lookInputDirection = context.ReadValue<Vector2>();
        poncherCharacter.GetAnimManager().SetAimDir((Vector2)lookInputDirection.normalized);
        Debug.DrawRay(transform.position, lookInputDirection * 2f, Color.red);


        if (context.started)
        {
            int holi = 0;
            //lockRotation = true;
        }
           

        if (context.canceled)
        {
            //poncherCharacter.GetAnimManager().SetUpperBodyLayerWeight(0f);
            //lockRotation = false;
        }
    }

    public void DownFast(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Started Fast Down");
            int holi = 0;
        }

        if (context.performed)
        {
            if (!poncherCharacter.isGrounded && poncherCharacter.GetState() != PoncherState.Jumping && !poncherCharacter.GetMovementComp().isFallingFast)
            {
                poncherCharacter.GetMovementComp().isFallingFast = true;
                Debug.Log("Passed the press Point");
                poncherCharacter.GetRigidbody().AddForce(5 * Vector2.down, ForceMode.Impulse);
            }
            else
            {//Down fast in the ground
                //if (poncherCharacter.slope)
                //{
                
                //}
            }            
        }

        if (context.canceled)
        {

        }
        
    }



    #endregion


    //Specially for keyboard by now since mouse is always aiming
    public void LockRotationbyKey(InputAction.CallbackContext context)
    {
        if (context.started)        
            lockRotation = true;  

        //Release
        if (context.canceled)        
            lockRotation = false;        

    }

    public void RightBumper(InputAction.CallbackContext context) 
    {
        if (context.started)
        {

        }
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

    public void SwitcBones()
    {
        //if (poncherCharacter.GetRagdollCtrl().IsRagdoll == true)
        //{
        //    RagdollController RG = poncherCharacter.GetRagdollCtrl();

        //    if (RG.HipGrounded() && RG.RootParent.gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
        //    {
        //        poncherCharacter.GetRagdollCtrl().SwitchBones(false);
        //    }
        //}
        if (poncherCharacter.GetRagdollCtrl().IsRagdoll == true)
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
        m_unitGoal = movInputDirection;

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
        return movInputDirection;
    }

    public PoncherInputActions GetPoncherActions()
    {
        return poncherActions;
    }

    public void SetLockRotation(bool _value)
    {
        if (lockRotation != _value)
            lockRotation = _value;


    }
    #endregion


}
