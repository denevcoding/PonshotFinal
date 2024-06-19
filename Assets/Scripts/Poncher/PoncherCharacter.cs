using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Utilities;

public enum PoncherState
{
    Idle = 0,
    Running = 1,
    Jumping = 2,    
    Falling = 3,
    Landing = 4,
    Diving = 5,
    Rolling = 6,
    Fliping = 7,
    Ponched = 8,
    Stumbled = 9,
    Hanging = 10,
    WallJumping = 11,
    WallSliding = 12,
    GettingUp =13,
    Crouch =14,
    Sliding =15,

}


public class PoncherCharacter : PonshotEntity
{
    [SerializeField]private PoncherState poncherState;

    //Components
    private Rigidbody poncheRigidbodie;
    private CapsuleCollider poncherCollider;
    [SerializeField] private Animator animator;

    //Poncher Components
    private JumpComponent jumpComponent;
    private MoveComponent moveComponent;
    private MovementComp movementComponent;
    private PoncherController poncherController;
    private RollComponent rollComponent;
    private PickThrowComponent pickThrowComponent;
    private PoncherAnimManager animManager;
    private RagdollController ragdollController;

    [Header("Aimer Data")]
    public PlayerGUI poncherGUI;

    [Header("Grounded Settings")]
    public bool isGrounded;
    public float rayLenght;
    public float boxOffset;
    [Space(5)]
    public LayerMask groundedLayerMask;
    public float slopeAngle; //Currnet slope from the ground I am 
    public Vector3 slopeNormal;
    public float slopeLimit = 40; //maximum navigaton slope angle
    public float slideForceAmount = 35; //Force to down on slope not navigable
   
    [Space(5)]
    public float coyoteTime;
    public float coyoteTimeCounter;
 

    [Header("Walled Settings")]
    public bool isWalled;
    public bool isStiking;
    public float wallRayLenght;
    public float wallAngleTreshold;
    public float wallSlopeAngle;
    public Vector3 wallNormal = Vector3.zero;


    [Header("Movement - Locomotion")]
    public bool isSidescroller = true;
    [Space(10)]
    public bool isStrafing;
    public bool canMove;
    public bool canRotate;
    public bool rotToVel;

    [Header("Corner Correction")]
    public bool upObstacle;
    public float rayCornerLenght;
    public float rayOffset;
    public float fixForce;


    private void Awake()
    {
        InitPoncherComponents();
        //if (poncherGUI)        
        //    poncherGUI.InitPoncheGUI(this);        
     
    }

    // Start is called before the first frame update
    void Start()
    {
        //InitPoncherInGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        //Handle blocked rotation, for flips and running backwards
        //if (!isRotBlocked)
        //{
        //    animator.SetFloat("VelocityX", Mathf.Abs(poncheRigidbodie.velocity.x));
        //}
        //else
        //{
        //    if (poncherController.GetInputDirection().magnitude > 0)
        //    {
        //        float direction = Vector3.Dot(poncherController.GetInputDirection(), transform.forward);
        //        if (direction < 0)
        //        {
        //            animator.SetFloat("VelocityX", Math.Abs(GetComponent<Rigidbody>().velocity.x) * -1);
        //        }
        //        else
        //        {
        //            animator.SetFloat("VelocityX", Math.Abs(GetComponent<Rigidbody>().velocity.x));
        //        }
        //    }
        //    else
        //    {
        //        animator.SetFloat("VelocityX", 0f);
        //    }
        //}



        //if (IsGrounded() && !checkIsWalled())
        //{
        //    jumpComponent.canDoubleJump = true;
        //}

    }

    private void FixedUpdate()
    {

        isGrounded = IsGrounded();
        isWalled = IsWalled();

        animator.SetBool("Grounded", isGrounded);
        HandleCoyoteTime();

        //isWalled = checkIsWalled();



        //CorrectCorners();
        //LedgeDetection();


        //animator.SetBool("Walled", isWalled);

        //Debug.DrawRay(transform.position, poncheRigidbodie.velocity);
        //if (!isGrounded)
        //{
        //    landingForce = Mathf.Abs(Mathf.Round(GetComponent<Rigidbody>().velocity.y));
        //    GetAnimator().SetFloat("LandingForce", landingForce);
        //    animator.SetFloat("VelocityY", poncheRigidbodie.velocity.y);
        //}
        //else
        //{
        //    animator.SetFloat("VelocityY", 0f);
        //}
    }









    #region Initialization
    void InitPoncherComponents()
    {
        poncheRigidbodie = GetComponent<Rigidbody>();
        poncherCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        animManager = GetComponent<PoncherAnimManager>();

        ragdollController = GetComponent<RagdollController>();
        movementComponent = GetComponent<MovementComp>();
        jumpComponent = GetComponent<JumpComponent>();

        poncherController = GetComponent<PoncherController>();
  
        //moveComponent = GetComponent<MoveComponent>();

        //rollComponent = GetComponent<RollComponent>();
        pickThrowComponent = GetComponent<PickThrowComponent>(); 




        //Init PonshotComponents with the owner
        PoncherComponentBase[] poncherComponents = this.GetComponents<PoncherComponentBase>();
        foreach (PoncherComponentBase poncherComp in poncherComponents)
        {
            poncherComp.poncherCharacter = this;

            poncherComp.Initcomponent();
        }
    }


    public void BindControllerToInputs(PlayerInput _inputComp)
    {
        if (GetController())
            GetController().BindInputActions(_inputComp);
    }
   
    //void InitPoncherInGameplay() //possible inititialize from PlayerManager
    //{
    //    //Initialization for Gameplay Character porpuses
    //    poncherState = PoncherState.Idle;
    //    groundedLayerMask = LayerMask.GetMask("Obstacle");

    //    //Init Movement Properties
    //    canMove = true;
    //    canRotate = true;
    //    rotToVel = false;
    //    isRotBlocked = false;
    //}
    #endregion


    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //float dist = GetComponent<CapsuleCollider>().bounds.extents.y;
        //Vector2 boxPos = new Vector2(transform.position.x, transform.position.y - boxOffset);
        //Vector3 size = GetComponent<CapsuleCollider>().bounds.size;
        //size.y = 0.2f;
        //size.x = 0.2f;
        //size.z = 0.2f;

        //Gizmos.DrawWireCube(boxPos, size);
    }



    #region surface Detections Settings
    //Ground Cheking
    public bool IsGrounded()
    {
        //groundedLayerMask = LayerMask.NameToLayer("Obstacle");
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y;
        RaycastHit hitCenter;    
      
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 boxSize = new Vector3(0.2f, 0.1f, 0.25f);     

        DrawDebugCasts.DrawBoxCastBox(rayPos, boxSize, transform.rotation, Vector3.down, rayLenght, Color.cyan);
        bool hit = Physics.BoxCast(rayPos, boxSize, Vector3.down, out hitCenter, transform.rotation, rayLenght, groundedLayerMask.value);


        if (hit && GetState() !=  PoncherState.Jumping)
        {
            if (!hitCenter.transform.GetComponent<Collider>().isTrigger)
            {
                //slope control
                slopeAngle = Vector3.Angle(hitCenter.normal, Vector3.up);
                slopeNormal = hitCenter.normal;
                //Debug.Log("Slope is: " + slope);
                if (slopeAngle > slopeLimit /*&& hit.transform.tag != "Pushable"*/)
                {
                    //poncherController.m_RotType = RotationType.ToVelocity;
                    float slideForce = slopeAngle/3;
                    Vector3 slide = new Vector3(0f, -slideForce, 0f);
                    GetRigidbody().AddForce(slide, ForceMode.Force);
                }
                else
                {
                    poncherController.m_RotType = RotationType.ToInputDir;
                }

                float dot = Vector3.Dot(Vector3.down, poncherController.inputDirection);               
                if (dot > 0.5)
                {
                    GetAnimator().SetBool("Crouched", true);
                }
                else
                {
                    GetAnimator().SetBool("Crouched", false);
                }

                return true;
            }
        }
           
        
        //moveComponent.movingObjSpeed = Vector3.zero;
        return false;
    }

    public bool IsWalled()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<Collider>().bounds.extents.y;

        Vector3 boxSize = new Vector3(0.1f, 0.5f, 0.1f);

        RaycastHit wallRay;
        Vector3 centerPoncher = transform.position;
        //enterPoncher.y = centerPoncher.y + dist / (2 + 0.5f);


        Vector3 inputDir = poncherController.inputDirection;
        //Debug.DrawRay(centerPoncher, transform.forward * wallRayLenght, Color.magenta, 0f);
        DrawDebugCasts.DrawBoxCastBox(centerPoncher, boxSize, transform.rotation, transform.forward, wallRayLenght, Color.magenta);

        bool hit = Physics.BoxCast(centerPoncher, boxSize, transform.forward, out wallRay, transform.rotation, wallRayLenght, groundedLayerMask.value);


        if (hit)
        {
            if (!wallRay.transform.GetComponent<Collider>().isTrigger)
            {
                wallSlopeAngle = Vector3.Angle(wallRay.normal, Vector3.up);
                wallNormal = wallRay.normal;

                if (wallSlopeAngle >= wallAngleTreshold)
                {// In front of a wall                   

                    float dot = Vector3.Dot(inputDir, transform.forward);
                    //Debug.Log("DOT "+ dot);
                    if (dot > 0 && isGrounded == false && GetRigidbody().velocity.y < -1)
                    {//Stiking                       
                        if (GetState() != PoncherState.WallJumping)
                        {
                            isStiking = true;
                            SetState(PoncherState.WallSliding);
                            animator.SetBool("Walled", true);
                            //Up force to simulate stick
                            Vector3 newVel = GetRigidbody().velocity;
                            Vector3 slide = new Vector3(0f, newVel.y * -5f, 0f);
                            GetRigidbody().AddForce(slide, ForceMode.Acceleration);
                            return true;
                        }

                    }
                    else
                    {
                        //Not Stiking                       
                        isStiking = false;
                        animator.SetBool("Walled", false);
                    }

                    return true;
                }
            }
        }

        isStiking = false;
        animator.SetBool("Walled", false);
        wallNormal = Vector3.zero;
        wallSlopeAngle = -1;
        return false;
    }


    public void CorrectCorners()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y + rayCornerLenght;

        RaycastHit rayRight;
        RaycastHit rayLeft;
        bool right = Physics.Raycast(transform.position + new Vector3(0.5f, 0f, 0f), Vector3.up, out rayRight, dist, groundedLayerMask);
        bool left = Physics.Raycast(transform.position + new Vector3(-0.5f, 0f, 0f), Vector3.up, out rayLeft, dist, groundedLayerMask);

        Debug.DrawRay(transform.position + new Vector3(rayOffset, 0f, 0f), Vector3.up * (dist), Color.cyan, 0f);
        Debug.DrawRay(transform.position + new Vector3(-rayOffset, 0f, 0f), Vector3.up * (dist), Color.magenta, 0f);

          
        if (GetRigidbody().velocity.y > 0)
        {
            Vector2 currVelocity = Vector2.zero;
            
            //Right
            if (right && !left)
            {
               
                //currVelocity.x = 0f;
                //currVelocity.y = GetRigidbody().velocity.y;
                //GetRigidbody().velocity = currVelocity;
                //rayRight
                transform.position += new Vector3(-fixForce, 0);
                //GetRigidbody().AddForce(-Vector3.right * fixForce * Time.deltaTime, ForceMode.Force);
                //GetRigidbody().MovePosition(transform.position += new Vector3(-fixForce, 0f));

            }//Left 
            else if (left && !right)
            {                
                //currVelocity.x = 0f;
                //currVelocity.y = GetRigidbody().velocity.y;
                //GetRigidbody().velocity = currVelocity;
                transform.position += new Vector3(fixForce, 0);
                //GetRigidbody().AddForce(Vector3.right * fixForce * Time.deltaTime, ForceMode.Force);
                //GetRigidbody().MovePosition(transform.position += new Vector3(fixForce, 0f));
            }

        }

        if (left && right)
        {
            upObstacle = true;
        }
        else
        {
            upObstacle = false;
        }

    }


    public bool LedgeDetection()
    {
        if (poncherState == PoncherState.Falling)
        {
            float xOffset = 1.2f;
            float yOffset = 2.5f;

            float yExtent = GetComponent<Collider>().bounds.min.y;
            Vector2 heightRayrigin = new Vector2(transform.position.x + xOffset, yExtent); 

            Debug.DrawRay(heightRayrigin, Vector3.down * 5f, Color.green, 0f);
        }
        return true;
    }

   

    private void OnCollisionStay(Collision collision)
    {
        //wallAngleAmount = Vector3.Angle(collision.GetContact(0).normal, Vector3.up);
        //wallNormal = collision.GetContact(0).normal;

        //if (wallAngleAmount >= wallAngleTreshold)
        //{// In front of a wall                   

        //    isWalled = true;

        //    Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal * (2f), Color.cyan, 0f);

        //    float dot = Vector3.Dot(GetController().inputDirection, transform.forward);
        //    //Debug.Log("DOT "+ dot);
        //    if (dot > 0)
        //    {   //Stiking                  

        //        if (GetState() != PoncherState.WallJumping && GetRigidbody().velocity.y < -1)
        //        {
        //            //isStiking = true;
        //            //SetState(PoncherState.WallSliding);

        //            Vector3 newVel = GetRigidbody().velocity;
        //            Vector3 slide = new Vector3(0f, newVel.y * -5f, 0f);
        //            GetRigidbody().AddForce(slide, ForceMode.Acceleration);
        //            animator.SetBool("Walled", true);
        //        }
        //    }
        //    else
        //    {
        //        animator.SetBool("Walled", false);
        //    }
        //}
        //else
        //{
        //    isWalled = false;
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (isWalled == true)
        //{
        //    isWalled = false;
        //    //animator.SetBool("Walled", false);
        //}
    }






    void HandleCoyoteTime()
    {
        if (isGrounded || poncherState == PoncherState.WallSliding)
        {
            //On the ground
            coyoteTimeCounter = 0;
            //GetComponent<PoncherController>().uprightForce = 50;
            //GetComponent<PoncherController>().uprightSpringDamper = 10;
            //GetComponent<PoncherController>().isStabilizing = true;
        }
        else
        {
            //Falling or in the air
            coyoteTimeCounter += Time.deltaTime;
        }
    }

    #endregion





    #region Getters and Setters
    public PoncherState GetState()
    {
        return poncherState;
    }
    public void SetState(PoncherState state)
    {
        poncherState = state;
    }

    //public float GetLandingForce()
    //{
    //    return landingForce;
    //}

    public Vector3 GetPoncherCenteredPosition()
    {
        Vector3 FixedPosition = transform.position;
        float offsetY = GetCollider().bounds.size.y / 2;
        //FixedPosition.y -= offsetY;
        return FixedPosition;
    }



    public PlayerGUI GetponcherGUI()
    {
        return poncherGUI;
    }


    //Poncher Components
    public MoveComponent GetMoveComponent()
    {
        return moveComponent;
    }
    public MovementComp GetMovementComp()
    {
        return movementComponent;
    }

    public JumpComponent GetJumpComp()
    {
        return jumpComponent;
    }

    public PickThrowComponent GetPickDropComp()
    {
        return pickThrowComponent;
    }

    public PoncherAnimManager GetAnimManager()
    {
        return animManager;
    }
    public RollComponent GetRoll()
    {
        return rollComponent;
    }
    public RagdollController GetRagdollCtrl()
    {
        return ragdollController;
    }

    public PoncherController GetController()
    {
        return poncherController;
    }

    //Unity
    public CapsuleCollider GetCollider()
    {
        return poncherCollider;
    }
    public Rigidbody GetRigidbody()
    {
        return poncheRigidbodie;
    }
    public Animator GetAnimator()
    {
        return animator;
    }
    #endregion
}
