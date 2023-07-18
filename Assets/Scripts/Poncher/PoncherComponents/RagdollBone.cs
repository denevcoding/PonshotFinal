using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollBone : PoncherComponentBase, IPoncheable
{
    public Rigidbody boneRB;
    public Collider boneColldier;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //boneRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Ponched(Vector3 dir, Vector3 position, float collMagnitude)
    {
        RagdollController ragControl = poncherCharacter.GetRagdollCtrl();
        if (ragControl.GetIsRagdolled() == true)
            return;

        ragControl.SwitchBones(true);
        boneRB.AddForce(dir * collMagnitude * 3f * -1, ForceMode.Impulse);
        //boneRB.AddTorque(dir * collMagnitude * -1, ForceMode.Impulse);
        //Vector3 force = new Vector3(1f, 1f, 0f);
        ragControl.AddForceToBones(dir * -1, collMagnitude / 3f, position, ForceMode.Impulse);
    }

    public void FixedUpdate()
    {
        //if (poncherCharacter.GetRagdollCtrl().GetIsRagdolled())
        //{
        //    boneRB.AddForce(Physics.gravity * 5 * boneRB.mass);
        //}

    }


}
