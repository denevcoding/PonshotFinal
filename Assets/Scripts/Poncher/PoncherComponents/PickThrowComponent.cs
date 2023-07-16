﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickThrowComponent : PoncherComponentBase
{
    private Vector3 offset;

    public bool canPick;
    public GameObject ObjectOnHand;

    [Header("Picking Settings")]
    public float DetectionRadius;
    public bool picking;

    [Header("Throwing settings")]
    public bool Charging;
    public float LaunchPressedTimeTreshhold;
    public float ShootMinForce;
    public float ShoothMaxForce;
    public float chargeMultiplierFactor;

    private float maxChargeTime = 0.75f;
    private float chargeSpeed;

    private float timerPressed = 0;
    private float CurrentLaunchForce;


    //Sockets
    public Transform LeftHandSocket;
    public Transform RighHandSocket;


    private void OnEnable()
    {
        //CurrentLaunchForce = min
    }

    private void Awake()
    {
 
    }

    // Start is called before the first frame update
    void Start()
    {
        canPick = true;

        chargeSpeed = (ShoothMaxForce - ShootMinForce) / maxChargeTime;
        //TODO:: init from poncher GUI with poncher character DATa scriptable Object
        poncherCharacter.GetponcherGUI().shooterPointer.InitSlider(ShootMinForce, ShoothMaxForce);

    }

    // Update is called once per frame
    void Update()
    {       
        Charge();        
    }

    private void FixedUpdate()
    {
        
    }


    public override bool CheckComponentPreconditions()
    {
        if (base.CheckBasePreconditions() == false)        
            return false;


        //if (m_poncherCharacter.GetState() == TraceurState.Hanging)
        //    return false;
        

        return true;
    }
    private bool PreconditionsToPick()
    {
        if (CheckComponentPreconditions() == false)
            return false;

        if (!canPick)
            return false;

        if (ObjectOnHand)
            return false;

        return true;
    }
    private bool PreconditionsToThrow()
    {
        if (CheckComponentPreconditions() == false)
            return false;

        if (ObjectOnHand == null)
            return false;

        return true;
    }
    



    public void PickDrop(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            picking = true;
            if (PreconditionsToPick() == false)
                return;

            Collider[] hitColliders = Physics.OverlapSphere(GetSpherePos(), DetectionRadius);

            foreach (var hitCollider in hitColliders)
            {
                IPickeable pickeable = hitCollider.gameObject.GetComponent<IPickeable>();

                if (pickeable != null)
                    ObjectOnHand = pickeable.Picked(this.poncherCharacter, LeftHandSocket);
            }
        }

        if (context.canceled)
        {
            picking = false;
        }       
    }

    public void ThrowLaunch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PreconditionsToThrow() == false)
                return;
            Charging = true;

            //Debug.Log("Stetting Launch");
            CurrentLaunchForce = ShootMinForce;
            poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = CurrentLaunchForce;
        }

        if (context.canceled)
        {
            Charging = false;

            if (ObjectOnHand)
            {
                //Launch();
                poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = ShootMinForce;
            }
        }
    }

    //Called on was presed
    //public void SetLaunchSettings()
    //{  
    //    if (PreconditionsToThrow() == false)
    //        return;
    //    Charging = true;

    //    //Debug.Log("Stetting Launch");
    //    CurrentLaunchForce = ShootMinForce;

    //    //Iniciar voz de carga
    //}


    //Called on is preseed
    public void Charge() 
    {
        //poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = ShootMinForce;

        if (CurrentLaunchForce >= ShoothMaxForce && ObjectOnHand != null && Charging)
        {
            //CurrentLaunchForce = ShoothMaxForce;
            //Launch();
        }
        
        if (Charging && ObjectOnHand != null)
        {
            CurrentLaunchForce += chargeSpeed * Time.deltaTime;
            poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = CurrentLaunchForce;
        }

    

        
        
        //timerPressed += Time.deltaTime;

        //if (timerPressed > LaunchPressedTimeTreshhold)
        ////{            
        //    float multiplier = chargeMultiplierFactor;
        //    CurrentLaunchForce += multiplier * Time.deltaTime;

        //if (CurrentLaunchForce > m_poncherCharacter.GetPoncherInfo().maxShootForce)
        //    CurrentLaunchForce = m_poncherCharacter.GetPoncherInfo().maxShootForce;

        //m_poncherCharacter.GetPoncherGUI().shooterPointer.chargeArrow.value = CurrentLaunchForce;
 

        //}
    }

    //Called on release
    public void Launch()
    {

        IPickeable objectToLaunch = ObjectOnHand.GetComponent<IPickeable>();

        //Transform shooterAim = m_poncherCharacter.GetPoncherGUI().shooterPointer.aimer.transform;
        //float axisX = m_poncherCharacter.GetPoncherGUI().shooterPointer.GetComponent<ShootPointer>().GetXAxis();

        //objectToLaunch.Throwed(CurrentLaunchForce, axisX, shooterAim);


        // m_poncherCharacter.GetPoncherGUI().shooterPointer.chargeArrow.value = 0;
        poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = 0;


        ResetLaunch();
    }

    public void ResetLaunch() 
    {
        CurrentLaunchForce = ShootMinForce;
        //timerPressed = 0f;
        ObjectOnHand = null;
    }



    private void OnDrawGizmos() 
    {
        if (picking)
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
            Gizmos.DrawWireSphere(GetSpherePos(), DetectionRadius);
        }
      
    }

    //Calculate the center of position for the poncher using the collider Extents and its transform
    private Vector3 GetSpherePos()
    {
        Vector3 spherePos = transform.position;
        Vector3 collSize = poncherCharacter.GetCollider().bounds.extents;        
        spherePos.y += (collSize.y);

        return spherePos;
    }

}
