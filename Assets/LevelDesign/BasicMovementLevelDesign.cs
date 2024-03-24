using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementLevelDesign : MonoBehaviour
{
    public float runSpeed;
    Rigidbody myRB;
    Animator myAnim;

    bool facingRight;


    bool grounded = false;
    Collider[] groundCollisions;
    float groundCheckRadius =0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float JumpHeight;
    
    // Start is called before the first frame update

    void Start()
    {
     myRB = GetComponent<Rigidbody>();
     myAnim = GetComponent<Animator>();
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {


        if (grounded && Input.GetAxis("Jump") > 0) {
            grounded = false;
            myAnim.SetBool("grounded", grounded);
            myRB.AddForce(new Vector3(0, JumpHeight, 0));
        }

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (groundCollisions.Length > 0)
        {

            grounded = true;

        }
        else { 
        grounded=false;

        }


        myAnim.SetBool("grounded", grounded);

        float move = Input.GetAxis("Horizontal");
        myAnim.SetFloat("speed", Mathf.Abs(move));

        myRB.velocity =  new Vector3(move* runSpeed, myRB.velocity.y, 0);

        if(move>0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();


    }

    public void Flip() { 
        facingRight=!facingRight;
        Vector3 theScale = transform.localScale;
        theScale.z *= -1;
        transform.localScale = theScale;
    
    }

}
