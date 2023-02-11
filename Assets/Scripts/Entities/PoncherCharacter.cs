using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    WallSliding = 12

}

public class PoncherCharacter : MonoBehaviour
{
    [SerializeField]private PoncherState poncherState;

    //Components
    private Rigidbody poncheRigidbodie;
    private CapsuleCollider poncherCollider;
    [SerializeField] private Animator animator;

    //Poncher Components
    private JumpComponent jumpComponent;
    private MoveComponent moveComponent;
    private PoncherController poncherController;
    private RollComponent rollComponent;
    private PoncherAnimManager animManager;

    private RagdollController ragdollController;

    [Header("Grounded Settings")]
    public bool isGrounded;
    public float rayLenght;
    [Space(5)]
    public LayerMask groundedLayerMask;
    public float slope; //Currnet slope from the ground I am 
    public Vector3 slopeNormal;
    public float slopeLimit = 40; //maximum navigaton slope angle
    public float slideAmount = 35; //Force to down on slope not navigable
    [Space(5)]
    public float coyoteTime;
    public float coyoteTimeCounter;

    [Header("Walled Settings")]
    public bool isWalled;
    public bool isStiking;
    public float wallRayLenght;
    public float wallAngleTreshold;
    public Vector3 wallNormal = Vector3.zero;


    [Header("Movement - Locomotion")]
    public bool isSidescroller = true;
    [Space(10)]
    public bool canMove;
    public bool canRotate;

    [Header("Corner Correction")]
    public bool upObstacle;
    public float rayCornerLenght;
    public float rayOffset;
    public float fixForce;


    private void Awake()
    {
        InitPoncherComponents();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitPoncher();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGrounded();
        isWalled = checkIsWalled();
        HandleCoyoteTime();

        CorrectCorners();

        LedgeDetection();


        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("Walled", isWalled);
        animator.SetFloat("DistanceToTarget", moveComponent.m_DistanceToTarget);
        animator.SetFloat("VelocityX", Mathf.Abs(poncheRigidbodie.velocity.x));

        //if (!isGrounded)
        //    animator.SetFloat("VelocityY", poncheRigidbodie.velocity.y));
        //else        
            animator.SetFloat("VelocityY", poncheRigidbodie.velocity.y);

        if (!isGrounded)
            animator.SetFloat("LandingForce", Mathf.Abs(poncheRigidbodie.velocity.y));



        if (isGrounded)
        {
            if (poncheRigidbodie.velocity.y < -1)
            {
                poncherState = PoncherState.Landing;
            }

            if (poncherState == PoncherState.Jumping)
                return;

            if (moveComponent.m_DistanceToTarget < 0.05f)
            {
                SetState(PoncherState.Idle);
            }

            if (moveComponent.m_DistanceToTarget > 0.05f)
            {
                SetState(PoncherState.Running);
            }



        }
        else
        {
            //if (poncheRigidbodie.velocity.y < 0)
            //{
            //    poncherState = PoncherState.Falling;
            //}
        }


    }

    private void FixedUpdate()
    {
        //Debug.DrawRay(transform.position, poncheRigidbodie.velocity);
    }









    #region Initialization
    void InitPoncherComponents()
    {
        poncheRigidbodie = GetComponent<Rigidbody>();
        poncherCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        jumpComponent = GetComponent<JumpComponent>();
        moveComponent = GetComponent<MoveComponent>();
        poncherController = GetComponent<PoncherController>();
        //rollComponent = GetComponent<RollComponent>();
        animManager = GetComponent<PoncherAnimManager>();

        ragdollController = GetComponent<RagdollController>();

        //Init PonshotComponents with the owner
        PoncherComponentBase[] poncherComponents = this.GetComponents<PoncherComponentBase>();
        foreach (PoncherComponentBase poncherComp in poncherComponents)
        {
            poncherComp.poncherCharacter = this;

            poncherComp.Initcomponent();
        }     
    }

   
    void InitPoncher() //possible inititialize from PlayerManager
    {
        //Initialization for Gameplay Character porpuses
        poncherState = PoncherState.Idle;
        groundedLayerMask = LayerMask.GetMask("Obstacle");

        canMove = true;
        canRotate = true;
    }
    #endregion





    #region surface Detections Settings
    //Ground Cheking
    public bool IsGrounded()
    {
        //groundedLayerMask = LayerMask.NameToLayer("Obstacle");
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y;
        RaycastHit hitCenter;

        for (int i = -1; i < 2; i++)
        {
            float point = 0f;
            point = GetComponent<CapsuleCollider>().bounds.size.x;
            point /= 3;
            point *= i;
            Vector2 rayPos = new Vector2((transform.position.x + point), transform.position.y);          

            Debug.DrawRay(rayPos, Vector3.down * rayLenght, Color.cyan, 0f);//Center          
            bool hit = Physics.Raycast(rayPos, Vector3.down, out hitCenter, rayLenght , groundedLayerMask.value);

            if (hit)
            {
                if (!hitCenter.transform.GetComponent<Collider>().isTrigger)
                {
                    //slope control
                    slope = Vector3.Angle(hitCenter.normal, Vector3.up);
                    slopeNormal = hitCenter.normal;
                    Debug.Log("Slope is: " + slope);
                    if (slope > slopeLimit /*&& hit.transform.tag != "Pushable"*/)
                    {
                        float slideForce = slope;
                        Vector3 slide = new Vector3(0f, -slideForce, 0f);
                        GetRigidbody().AddForce(slide, ForceMode.Force);
                    }
                    return true;
                }
            }
           
        }
        moveComponent.movingObjSpeed = Vector3.zero;
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

    public bool checkIsWalled()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<Collider>().bounds.extents.y;

        RaycastHit wallRay;
        Vector3 centerPoncher = transform.position;
        centerPoncher.y = centerPoncher.y + dist / 2;

        Vector3 inputDir = poncherController.inputDirection;
        Debug.DrawRay(centerPoncher, inputDir * wallRayLenght, Color.magenta, 0f);

        if (Physics.Raycast(centerPoncher, transform.forward, out wallRay, wallRayLenght, groundedLayerMask))
        {           
            if (!wallRay.transform.GetComponent<Collider>().isTrigger)
            {
                float angle = Vector3.Angle(wallRay.normal, Vector3.up);
                wallNormal = wallRay.normal;

                if (angle > wallAngleTreshold)
                {// In front of a wall                   

                    float dot = Vector3.Dot(inputDir, transform.forward);
                    //Debug.Log("DOT "+ dot);
                    if (dot > 0)
                    {   //Stiking                  

                        if (isGrounded == false && GetRigidbody().velocity.y < 0)
                        {
                            if (GetState() != PoncherState.WallJumping)
                            {
                                //isStiking = true;
                                //SetState(PoncherState.WallSliding);

                                Vector3 newVel = GetRigidbody().velocity;
                                Vector3 slide = new Vector3(0f, newVel.y * -5f, 0f);
                                GetRigidbody().AddForce(slide, ForceMode.Acceleration);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        //Not Stiking                       
                        return false;
                    }

                    return false;
                }
            }
        }

   
        return false;
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal * 2f, Color.white, 0f);
    //        if (contact.normal.y < 0.1f)
    //        {
    //            isWalled = true;
    //        }
    //    }
    //    //Debug.DrawRay(collision., Vector3.up * (dist), Color.cyan, 0f);
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (isWalled == true)
    //    {
    //        isWalled = false;
    //    }
    //}






    void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            //On the ground
            coyoteTimeCounter = coyoteTime;
            //GetComponent<PoncherController>().uprightForce = 50;
            //GetComponent<PoncherController>().uprightSpringDamper = 10;
            //GetComponent<PoncherController>().isStabilizing = true;
        }
        else
        {
            //Falling or in the air
            coyoteTimeCounter -= Time.deltaTime;
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



    //Poncher Components
    public MoveComponent GetMoveComponent()
    {
        return moveComponent;
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
