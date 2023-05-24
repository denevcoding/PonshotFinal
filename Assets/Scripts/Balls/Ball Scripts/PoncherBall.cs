﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BallState
{
    Free,
    Grabbed,
    Punching,
    Inactive
}

public class PoncherBall : MonoBehaviour, IPickeable
{
    public BallInfoSB BallData;
    public BallState ballState;

    public PoncherCharacter poncherOwner; // If some Poncher has this Ball

    public GameObject BounceVFX;

    private Rigidbody BallRB;
    private SphereCollider BallCollider;
    private BallAudioManager ballAudio;

    private ObjectPool objectPool;

    private float ballVelocitie; //Actual magnitud from velocity Vector
    private Transform socketOwner;  


    private void Awake()
    {
        BallRB = GetComponent<Rigidbody>();
        BallCollider = GetComponent<SphereCollider>();
        ballAudio = GetComponent<BallAudioManager>();        

        gameObject.name = BallData.BallName;

        //rigid Bodie Inits
        BallRB.drag = BallData.LinearDrag;
        BallRB.angularDrag = BallData.AngularDrag;
    }

    // Start is called before the first frame update
    void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballState == BallState.Grabbed)
        {
            this.transform.position = socketOwner.transform.position;
        }
        
    }

    public void FixedUpdate()
    {
        if (BallRB == null)
            return;

        ballVelocitie = BallRB.velocity.magnitude;

        if (ballVelocitie >= BallData.SpeedToPunch)
            ballState = BallState.Punching;
    }


    #region Interface Methods
    public bool CheckPreconditionsToPicked()
    {
        if (ballState == BallState.Grabbed)
            return false;
        return true;
    }

    public GameObject Picked(PoncherCharacter owner, Transform socket)
    {
        if (CheckPreconditionsToPicked() == false)
            return null;

        SwitchBall(true);
        poncherOwner = owner;
        socketOwner = socket;
        ballState = BallState.Grabbed;
        return this.gameObject;
    }

    public void Throwed(float force, float axis, Transform spawner)
    {
        ballState = BallState.Free;
        SwitchBall(false);
        this.transform.position = spawner.position;
               
        BallRB.AddForce(force * spawner.right, ForceMode.Impulse);

        float torque = 10;
        
        if (axis < 0)
            torque *= 1;
        else
            torque *= -1;

        BallRB.AddTorque(torque * spawner.forward, ForceMode.Impulse);
    }
    #endregion



    private void SwitchBall(bool kinematic)
    {
        if (kinematic)
        {
            BallCollider.enabled = false;

            BallRB.velocity = Vector3.zero;
            BallRB.useGravity = false;
            BallRB.isKinematic = true;
        }
        else
        {
            BallRB.velocity = Vector3.zero;
            BallRB.useGravity = true;
            BallRB.isKinematic = false;

            BallCollider.enabled = true;
        }
    
    }



    #region Physix Events
    private void OnCollisionEnter(Collision collision)
    {
        //Material detection to change the type of the sound
        float bounceForce = collision.relativeVelocity.magnitude / 30;

        //sonido de rebote normal a velocidad normal
        ballAudio.PlayBounce(bounceForce);

        //for VFX TODO: Implement Fixes
        if (collision.relativeVelocity.magnitude >= BallData.SpeedToPunch)
        {
            GameObject vfx = objectPool.GetPrefab();
            vfx.transform.position = collision.GetContact(0).point;
            
            IPoncheable poncheable = collision.gameObject.GetComponent<IPoncheable>();
            if (poncheable != null)
            {
                //BallRB.velocity = Vector3.zero;
                BallRB.AddForce(collision.GetContact(0).normal * -1 * 1.5f, ForceMode.Impulse);
                poncheable.Ponched(collision.GetContact(0).normal, collision.GetContact(0).point, collision.relativeVelocity.magnitude);
                //hitClip = BallData.poncherHitsClips[0];
                //reproducir sonido de golpe a jugador
            }           

            //Sonido de golpe duro

        }


    }
    #endregion


    public void HitPoncher() 
    {
        BallRB.velocity = Vector3.zero;
    }

    #region Getters Setters
    public Rigidbody GetRB()
    {
        return BallRB;
    }
    public Collider GetCollider()
    {
        return BallCollider;
    }
    #endregion
}