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
        
    }

    public override void Initcomponent()
    {
        BaseSMB[] bsmbs = m_poncherCharacter.GetAnimator().GetBehaviours<BaseSMB>();
        foreach (BaseSMB smb in bsmbs)
        {
            smb.poncherCharacter = m_poncherCharacter;
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
        tempPosition += m_poncherCharacter.GetAnimator().deltaPosition;
        tempPosition.z = 0;
        transform.position = tempPosition;
        //transform.forward = animator.deltaRotation * transform.forward;
    }


    public void ActivateRootMotion()
    {
        isRootMotion = true;
    }
    public void DeactivateRootmotion()
    {
        isRootMotion = false;
    }
}
