using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoncherController : MonoBehaviour
{
    Rigidbody rigidBodiePoncher;
    CapsuleCollider colliderPoncher;

    public float rayLenght;

    int groundedLayerMask;


    public float RideHeight;
    public float RideSpringStrenght;
    public float RideSpringDamper;

    [Space(20)]
    public float uprightForce;
    public float uprightSpringDamper;

    public Vector3 inputDirection;

    Vector3 screenMovementForward, screenMovementRight;
    private Quaternion screenMovementSpace = Quaternion.identity;


    //[Space(20)]
    //public Vector3 unitGoal = Vector3.zero;
    //public Vector3 goalVel = Vector3.zero;

    //public float MaxSpeed;
    //public float Aceleration;
    //public float MaxAccelForce;
    //public float groundVel;
    //public AnimationCurve AcelerationFactorFromDot;


    private void Awake()
    {
        rigidBodiePoncher = GetComponent<Rigidbody>();
        colliderPoncher = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        groundedLayerMask = LayerMask.GetMask("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {

        CalculateInputs();
        

        
    }


    void CalculateInputs()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        //get movement axis relative to camera
        screenMovementForward = screenMovementSpace * Camera.main.transform.forward;
        screenMovementRight = screenMovementSpace * Camera.main.transform.right;

        Vector3 move = Vector3.zero;
        //only apply vertical input to movemement, if player is not sidescroller
        if (true)
            inputDirection = (screenMovementForward * v) + (screenMovementRight * h);
        else
            inputDirection = Vector3.right * h;


        if (move.magnitude > 1.0f)

            move.Normalize();


        Debug.DrawRay(transform.position, inputDirection * 2f, Color.red);

        Debug.DrawRay(transform.position, rigidBodiePoncher.velocity.normalized * 3f);
    }



    private void FixedUpdate()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = GetComponent<CapsuleCollider>().bounds.extents.y + rayLenght;

        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * (dist), Color.cyan, 0.05f);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dist + 0.05f, groundedLayerMask))
        {
            FloatingCapsule(hit);
        }

        Movement();

        UpdateUprightForce();


        rigidBodiePoncher.AddForce(inputDirection * 5f * Time.deltaTime, ForceMode.VelocityChange);
    }




    void Movement()
    {

        //unitGoal = move;
        //Calculate new Goal
        //Vector3 unitVel = goalVel.normalized;

        //float velDot = Vector3.Dot(unitGoal, unitVel);

        //float accel = Aceleration * AcelerationFactorFromDot.Evaluate(velDot);


        //float speedFactor = 0.5f;
        //Vector3 newGoalVel = unitGoal * MaxSpeed * speedFactor;
        //Vector3 groundVel = this.transform.position;

        //goalVel = Vector3.MoveTowards(goalVel, (newGoalVel) + (groundVel), accel * Time.deltaTime);

        //Vector3 neededAccel = (goalVel - rigidBodiePoncher.velocity ) / Time.deltaTime; 

        //float maxAcel = MaxAccelForce * maxAce

    }


    void FloatingCapsule(RaycastHit hit)
    {
        Vector3 vel = rigidBodiePoncher.velocity;
        Vector3 rayDir = transform.TransformDirection(Vector3.down);

        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = hit.rigidbody;

        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }

        float rayDirVel = Vector3.Dot(rayDir, vel);
        float otherDirVel = Vector3.Dot(rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;

        float x = hit.distance - RideHeight;

        float springForce = (x * RideSpringStrenght) - (relVel * RideSpringDamper);

        rigidBodiePoncher.AddForce(rayDir * springForce);

        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
        }
    }
    void UpdateUprightForce()
    {
        //Quaternion upRightTargetRot = GetTargetRot(Vector3.up, Vector3.right);

        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = MathUtils.ShortestRotation(Quaternion.identity, characterCurrent);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        rigidBodiePoncher.AddTorque((rotAxis * (rotRadians * uprightForce)) - (rigidBodiePoncher.angularVelocity * uprightSpringDamper));
    }





}
