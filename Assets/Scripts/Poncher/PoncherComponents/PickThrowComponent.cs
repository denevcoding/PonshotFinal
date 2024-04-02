using System.Collections;
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

    private bool hasFired;
    private float maxChargeTime = 0.7f;
    private float chargeSpeed;

    private float timerPressed = 0;
    [SerializeField]private float CurrentLaunchForce;


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
        hasFired = false;

        chargeSpeed = (ShoothMaxForce - ShootMinForce) / maxChargeTime;
        //TODO:: init from poncher GUI with poncher character DATa scriptable Object

        if (poncherCharacter.GetponcherGUI())        
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


    public void PickDrop(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Charging)
            {
                Charging = false;
                ResetLaunch();
            }               

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


    private bool PreconditionsToThrow()
    {
        if (ObjectOnHand == null)
            return false;

        return true;
    }
    public void ThrowLaunch(InputAction.CallbackContext context)
    {
        if (CheckComponentPreconditions() == false)
            return;

        if (context.started)
        {
            Charging = true;
            hasFired = false;
           // CurrentLaunchForce = ShootMinForce;
            poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = ShootMinForce;            
   
        }

        if (context.canceled)
        {
            Charging = false;

            if (!hasFired)
            {
                Launch();
            }
           
        }
    }

 

    //Called on is preseed
    public void Charge() 
    {
        if (Charging && CurrentLaunchForce >= ShoothMaxForce && !hasFired)
        {
            CurrentLaunchForce = ShoothMaxForce;
            //Launch();
        }     
        else if (Charging && !hasFired)
        {
            CurrentLaunchForce += chargeSpeed * Time.deltaTime;
            poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = CurrentLaunchForce;
            //Launch();
        }
    }

    //Called on release
    public void Launch()
    {
        hasFired = true;

        if (ObjectOnHand)
        {
            IPickeable objectToLaunch = ObjectOnHand.GetComponent<IPickeable>();

            Transform shooterAim = poncherCharacter.GetponcherGUI().shooterPointer.moveAimer.transform;
            float axisX = poncherCharacter.GetponcherGUI().shooterPointer.GetComponent<ShootPointer>().GetXAxis();

            Debug.Log("Shoot force " + CurrentLaunchForce);
            objectToLaunch.Throwed(CurrentLaunchForce, axisX, shooterAim);
            ObjectOnHand = null;
        }

        ResetLaunch();
    }

    public void ResetLaunch() 
    {
        CurrentLaunchForce = ShootMinForce;
        poncherCharacter.GetponcherGUI().shooterPointer.chargeArrow.value = CurrentLaunchForce;
        //timerPressed = 0f;
        
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
