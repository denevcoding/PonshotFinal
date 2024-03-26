using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootPointer : MonoBehaviour
{
    private PoncherCharacter ownerPoncher;
    private PlayerGUI m_poncherGUI;

    private Vector3 offset;

    //Aiming Properties
    private float AimAngle = 0.0f;
    private float x;
    private float y;

    public Vector2 m_MoveInput;
    public Vector2 m_LookInput;
    [SerializeField] private bool isLooking;

    Vector3 mosPos;
    Vector3 mouseWorldPos;

  // CANCEL ALL INPUT BELOW THIS FLOAT
    [SerializeField] private float R_analog_threshold = 0.20f;

    public Slider chargeArrow;
    public GameObject aimer;

    public delegate void AimBasedOnControlTypeFunc();
    public AimBasedOnControlTypeFunc AimBasedOnControlType;


    private void Awake()
    {
        chargeArrow.value = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (ownerPoncher == null)
        //    return;

        //Vector3 fixedPosition = transform.localPosition;
        //fixedPosition.x *= -x;
        //transform.localPosition = fixedPosition;
        AimBasedOnControlType();       
    }


    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(mouseWorldPos, 0.5f);
    }

    public void InitAimer(PlayerGUI _guiOwner)
    {
        m_poncherGUI = _guiOwner;
        PlayerInput PI = m_poncherGUI.GetInputcomp();

        if (PI)
        {
            switch (PI.currentControlScheme)
            {
                case "Keyboard&Mouse":
                    AimBasedOnControlType = AimWithMouse;                    
                    break;

                case "Gamepad":
                    AimBasedOnControlType = AimwithGamepad;                    
                    break;
            }

            BindInputActions(PI);
        }
    }

    void BindInputActions(PlayerInput _inputComp)
    {
        Dictionary<string, Action<InputAction.CallbackContext>> ActionMap = new Dictionary<string, Action<InputAction.CallbackContext>>();
        ActionMap.Add("Movement", OnMoveInput);
        ActionMap.Add("Aim", OnLookInput);

        foreach (string keyAction in ActionMap.Keys)
        {
            _inputComp.actions[keyAction].started += ActionMap[keyAction];
            _inputComp.actions[keyAction].performed += ActionMap[keyAction];
            _inputComp.actions[keyAction].canceled += ActionMap[keyAction];
        }
        _inputComp.actions["Aim"].performed += OnLookInput; //Binding to right stick values
    }

    public void InitSlider(float minValue, float maxValue)
    {
        chargeArrow.minValue = minValue;
        chargeArrow.maxValue = maxValue;
    }


    public void SetPoncherOwner(PoncherCharacter owner)
    {
        if (owner != null)
        {
            ownerPoncher = owner;
            offset = transform.position;
        }
    }

    //If player is using a gamepad, this is the right stick to aim
    public void OnLookInput(InputAction.CallbackContext context) 
    {
        m_LookInput = context.ReadValue<Vector2>();

        if (context.started)        
            isLooking = true;        

        if (context.canceled)        
            isLooking = false;        
 
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
    }


    public void AimWithMouse()
    {       
        //Debug.Log($"Mouse: {x} : {y}");

        mosPos = Mouse.current.position.ReadValue();
        mosPos.z = 15;

        mouseWorldPos = Camera.main.ScreenToWorldPoint(mosPos);

        m_LookInput = (mouseWorldPos - transform.position);     

        RotateByInput(m_LookInput);
        Debug.DrawRay(transform.position, m_LookInput * 2f, Color.blue);
    }

    public void AimwithGamepad()
    {
        if (!isLooking)
        {
            RotateByInput(m_MoveInput);
        }
        else
        {
            RotateByInput(m_LookInput);
        }
    }

    public void RotateByInput(Vector2 axis)
    {
        //Ask for control type enum

        //for mouse porpuses
        //print(Input.mousePosition);
        //Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        //Vector3 mousePos = Input.mousePosition;
        //x = mousePos.x - objectPos.x;
        //y = mousePos.y - objectPos.y;


        //Vector2 axis = ownerPoncher.GetController().GetPoncherActions().PlayerGameplay.Movement.ReadValue<Vector2>();
        x = axis.x;
        y = axis.y;

        // USED TO CHECK OUTPUT
        //Debug.Log(" horz: " + x + "   vert: " + y);
        if (Mathf.Abs(x) < R_analog_threshold) { x = 0.0f; }
        if (Mathf.Abs(y) < R_analog_threshold) { y = 0.0f; }
        // CALCULATE ANGLE AND ROTATE
        if (x != 0.0f || y != 0.0f)
        {
            AimAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            //Debug.Log("Aim: " + AimAngle);
            // ANGLE AIM
            transform.rotation = Quaternion.AngleAxis(AimAngle, Vector3.forward);
        }
        //This is for animator controller aiming 
        //objFollow.GetComponent<Animator>().SetFloat("AimX", x);
        //objFollow.GetComponent<Animator>().SetFloat("AimY", y);

    }




    //Getters Setters
    public float GetXAxis()
    {
        return x;
    }

    public float GetYAxis()
    {
        return y;
    }

    
}
