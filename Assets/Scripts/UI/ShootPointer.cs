using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootPointer : MonoBehaviour
{
    private PoncherCharacter ownerPoncher;

    private Vector3 offset;

    //Aiming Properties
    private float AimAngle = 0.0f;
    private float x;
    private float y;

    // CANCEL ALL INPUT BELOW THIS FLOAT
    [SerializeField] private float R_analog_threshold = 0.20f;

    public Slider chargeArrow;
    public GameObject aimer;


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
        if (ownerPoncher == null)
            return;

        //Vector3 fixedPosition = transform.localPosition;
        //fixedPosition.x *= -x;
        //transform.localPosition = fixedPosition;
        RotateByInput();
    }


    public void SetPoncherOwner(PoncherCharacter owner)
    {
        if (owner != null)
        {
            ownerPoncher = owner;
            offset = transform.position;
        }
    }

    public void RotateByInput()
    {
        //Ask for control type enum

        //for mouse porpuses
        //print(Input.mousePosition);
        //Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        //Vector3 mousePos = Input.mousePosition;
        //x = mousePos.x - objectPos.x;
        //y = mousePos.y - objectPos.y;


        Vector2 axis = ownerPoncher.GetController().GetPoncherActions().PlayerGameplay.Movement.ReadValue<Vector2>();
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
