using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PoncherAnimManager : PoncherComponentBase
{


    [Header("ANIMATION RIGGING")]
    public RigBuilder PoncherRigBuilder;
    public Rig AimTargetRig;
    public MultiAimConstraint headAimConstraint;
    public Transform AimTarget;

    [Space(20)]
    public float aimWeight;
    public float aimWeightSpeed = 3.5f;

    [Header("IK SETTINGS")]
    public Transform RightArmIK;
    public Transform LeftArmIK;

    Animator animator;   
    [HideInInspector] public Animation currentAnimation;
    public bool isRootMotion = false; //Turns root motion just for actions that needed

    //Hashes
    int AimXHash;
    int AimYHash;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitHashes();

        AimTarget = poncherCharacter.GetController().playerGUI.shooterPointer.GetAimTarget();

        //Init Target Source
        foreach (MultiAimConstraint multiAim in AimTargetRig.GetComponentsInChildren<MultiAimConstraint>())
        {
            WeightedTransformArray sources = new WeightedTransformArray();
            sources.Add(new WeightedTransform(AimTarget, 1f));
            multiAim.data.sourceObjects = sources;


            animator.enabled = false;
            PoncherRigBuilder.Build();
            animator.enabled = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //AimTargetRig.weight = Mathf.Lerp(AimTargetRig.weight, aimWeight, Time.deltaTime * 5f);
        //animator.SetLayerWeight(1, AimTargetRig.weight);


        //if (AimTarget)
        //{
        //    AimTarget.transform.position = poncherCharacter.GetController().playerGUI.
        //}

        //Validate preconditions

        //if (!poncherCharacter.isGrounded && isRootMotion)
        //{
        //    SwitchMovement(1);
        //}
    }

    private void FixedUpdate()
    {
        //AimTargetRig.weight = Mathf.Lerp(AimTargetRig.weight, aimWeight, Time.deltaTime * aimWeightSpeed);
      
        //animator.SetLayerWeight(1, AimTargetRig.weight);
    }


    #region Initialization
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

    public void InitHashes()
    {
        //Rotation Hashesh
        AimXHash = Animator.StringToHash("AimX");
        AimYHash = Animator.StringToHash("AimY");
    }
    #endregion

    #region Settters
    public void SetAimDir(Vector2 lookDir)
    {
        animator.SetFloat(AimXHash, lookDir.x);
        animator.SetFloat(AimYHash, lookDir.y);
    }

    #endregion
    public void SetUpperBodyLayerWeight(float weight, bool _instantly)
    {
       // aimWeight = weight;

        animator.SetLayerWeight(1, weight);

        //if (_instantly)        
        //    AimTargetRig.weight = weight;        
        //else        
            //aimWeight = weight;
        
          
    }
    public void SetUpperBodyRigWeight(float weight)
    {
        AimTargetRig.weight = weight;
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
