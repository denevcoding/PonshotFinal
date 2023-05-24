using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingSphere : MonoBehaviour
{
    PoncherInputActions poncherActions;
    [Header("Input Properties")]
    PlayerInput poncherInput;

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;


    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField]
    Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    Rigidbody body;

    Vector3 velocity;
    Vector3 desiredVelocity;

    bool desiredJump;


    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        poncherInput = GetComponent<PlayerInput>();

        poncherActions = new PoncherInputActions();
        poncherActions.PlayerGameplay.Enable();// Actiovating buttons for gameplay We can switch to UI or anything else
        poncherActions.PlayerGameplay.Jump.started += Jump;            
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerInput = poncherActions.PlayerGameplay.Movement.ReadValue<Vector2>();

        float h = playerInput.x;
        float v = playerInput.y;
        //playerInput.Normalize();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        

        //velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        //velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);


        body.velocity = velocity;

    }


    public void Jump(InputAction.CallbackContext context)
    {     
        velocity.y += 150f;
    }

    void MoveByTransform()
    {

    }
}
