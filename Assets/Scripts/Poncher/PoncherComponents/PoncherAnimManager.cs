using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoncherAnimManager : PoncherComponentBase
{
    Animator animator;   
    [HideInInspector] public Animation currentAnimation;
    public bool isRootMotion = false; //Turns root motion just for actions that needed


    //Masks

    //Layers -Base - Additives - Overrides 
    //Pesos de las additivas
    //- Facial Layer Additive - Flips Acrobascys? Roll?

    //State nmachine states acces

    //RootMotion

    //IK


    //Set animation Values


    //Paramters
    [HideInInspector]
    public int VelocityHash;

    //Triggers


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //Validate preconditions

        //if (!poncherCharacter.isGrounded && isRootMotion)
        //{
        //    SwitchMovement(1);
        //}
    }

    public override void Initcomponent()
    {
        BaseSMB[] bsmbs = poncherCharacter.GetAnimator().GetBehaviours<BaseSMB>();
        foreach (BaseSMB smb in bsmbs)
        {
            smb.poncherCharacter = poncherCharacter;
        }
        //Debug.Log("estados:" + bsmbs.Length);

        //init parameters
        VelocityHash = Animator.StringToHash("Velocity");
    }



    //Handeling Root motion
    private void OnAnimatorMove()
    {
        if (!isRootMotion)
            return;

        Vector3 tempPosition = transform.position;

        if (!poncherCharacter.isWalled)
            tempPosition.x += poncherCharacter.GetAnimator().deltaPosition.x;
        else
        {
            DeactivateRootmotion();
        }

        if (!poncherCharacter.IsGrounded())
        {
            tempPosition.y += -5f * Time.deltaTime;
        }


        tempPosition.z = 0;
        poncherCharacter.GetRigidbody().MovePosition(tempPosition);
        transform.forward = animator.deltaRotation * transform.forward;
    }

    


    public void ActivateRootMotion()
    {
        //poncherCharacter.GetRigidbody().interpolation = RigidbodyInterpolation.None;
        isRootMotion = true;     
        //poncherCharacter.canMove = false;
    }
    public void DeactivateRootmotion()
    {
       //poncherCharacter.GetRigidbody().interpolation = RigidbodyInterpolation.Interpolate;
        isRootMotion = false;
        //poncherCharacter.canMove = true;
    }



    //Called from anim event
    public void CancelRoll()
    {
        //poncherCharacter.GetRoll().canRoll = false;
    }


    public void SwitchMovement(int restore)
    {
        if (restore == 0)
        {
            poncherCharacter.canRotate = false;
            poncherCharacter.canMove = false;
        }
        else
        {
            DeactivateRootmotion();
            poncherCharacter.canRotate = true;
            poncherCharacter.canMove = true;
        }

    }


}
