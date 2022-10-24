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

    [Header("Grounded Settings")]
    public bool isGrounded;
    public float rayLenght;
    public int groundedLayerMask;
    [Space(5)]
    public float coyoteTime;
    public float coyoteTimeCounter;


    [Header("Movement - Locomotion")]
    public bool isSidescroller = true;
    [Space(10)]
    public bool canMove;
    public bool canRotate;

    [Header("Corner Correction")]
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
        HandleCoyoteTime();

        CheckCorners();

        animator.SetBool("Grounded", isGrounded);

        animator.SetFloat("DistanceToTarget", moveComponent.m_DistanceToTarget);
    
        animator.SetFloat("VelocityX", Mathf.Abs(poncheRigidbodie.velocity.x));

        if (!isGrounded)
            animator.SetFloat("VelocityY", Mathf.Abs(poncheRigidbodie.velocity.y));


        //if (isGrounded)
        //{
        //    if (poncheRigidbodie.velocity.y < -1)
        //    {
        //        poncherState = PoncherState.Landing;
        //    }

        //    if (poncherState == PoncherState.Jumping)
        //        return;

        //    if (poncherController.m_DistanceToTarget < 0.05f)
        //    {
        //        SetState(PoncherState.Idle);
        //    }

        //    if (poncherController.m_DistanceToTarget > 0.05f)
        //    {
        //        SetState(PoncherState.Running);
        //    }



        //}
        //else
        //{
        //    if (poncheRigidbodie.velocity.y < 0)
        //    {                
        //        poncherState = PoncherState.Falling;
        //    }
        //}


    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, poncheRigidbodie.velocity);
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
        rollComponent = GetComponent<RollComponent>();
        animManager = GetComponent<PoncherAnimManager>();

        //Init PonshotComponents with the owner
        PoncherComponentBase[] poncherComponents = this.GetComponents<PoncherComponentBase>();
        foreach (PoncherComponentBase poncherComp in poncherComponents)
        {
            poncherComp.m_poncherCharacter = this;

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
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y;

        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * (dist), Color.cyan, 0f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dist + rayLenght, groundedLayerMask))
        {
            poncherController.FloatingCapsule(hit);
            return true;

        }

        return false;
    }



    public void CheckCorners()
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

      
            //Right
            if (right && !left)
            {
                //rayRight
                transform.position += new Vector3(-fixForce, 0.1f);
                //GetRigidbody().AddForce(-Vector3.right * fixForce * Time.deltaTime, ForceMode.Acceleration);
                //GetRigidbody().MovePosition(transform.position += new Vector3(-fixForce, 0f));

            }//Left 
            else if (left && !right)
            {
                transform.position += new Vector3(fixForce, 0.1f);
                //GetRigidbody().AddForce(Vector3.right * fixForce * Time.deltaTime, ForceMode.Acceleration);
                //GetRigidbody().MovePosition(transform.position += new Vector3(fixForce, 0f));
            }

        }


    }




    void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            //On the ground
            coyoteTimeCounter = coyoteTime;
            GetComponent<PoncherController>().uprightForce = 50;
            GetComponent<PoncherController>().uprightSpringDamper = 10;
            GetComponent<PoncherController>().isStabilizing = true;
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

    //Unity
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
