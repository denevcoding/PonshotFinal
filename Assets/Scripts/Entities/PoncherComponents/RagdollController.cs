using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : PoncherComponentBase
{
    public Transform RootParent;

    private List<GameObject> ragdollBones;
    private List<CharacterJoint> joints;

    public PhysicMaterial BouncePxMat;

    public bool IsRagdoll;
    public bool enableCollision;

      

    //Possible states of the ragdoll
    enum RagdollState
    {
        animated,    //Mecanim is fully in control
        ragdolled,   //Mecanim turned off, physics controls the ragdoll
        blendToAnim  //Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
    }

    //The current state
    RagdollState state = RagdollState.animated;

    //How long do we blend when transitioning from ragdolled to animated
    public float ragdollToMecanimBlendTime = 0.5f;
    float mecanimToGetUpTransitionTime = 0.05f;

    //A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
    float ragdollingEndTime = -100;



    //Declare a class that will hold useful information for each body part
    public class BodyPart
    {
        public Transform transform;
        public Vector3 storedPosition;
        public Quaternion storedRotation;
    }
    //Additional vectores for storing the pose the ragdoll ended up in.
    Vector3 ragdolledHipPosition, ragdolledHeadPosition, ragdolledFeetPosition;

    //Declare a list of body parts, initialized in Start()
    List<BodyPart> bodyParts = new List<BodyPart>();




    private void Awake()
    {
        ragdollBones = new List<GameObject>();
        joints = new List<CharacterJoint>();

        FindBones();
        FindJoints();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBones();

        //SetWeight();
        //SetPhysixMaterial(BouncePxMat);

        SwitchBones(IsRagdoll);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        //Clear the get up animation controls so that we don't end up repeating the animations indefinitely
        poncherCharacter.GetAnimator().SetBool("GetUpFromBelly", false);
        poncherCharacter.GetAnimator().SetBool("GetUpFromBack", false);

        if (state == RagdollState.blendToAnim)
        {
            if (Time.time <= ragdollingEndTime + mecanimToGetUpTransitionTime)
            {
                //If we are waiting for Mecanim to start playing the get up animations, update the root of the mecanim
                //character to the best match with the ragdoll
                Vector3 animatedToRagdolled = ragdolledHipPosition - poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Hips).position;
                Vector3 newRootPosition = transform.position + animatedToRagdolled;

                //Now cast a ray from the computed position downwards and find the highest hit that does not belong to the character 
                RaycastHit[] hits = Physics.RaycastAll(new Ray(newRootPosition, Vector3.down));
                newRootPosition.y = 0;
                foreach (RaycastHit hit in hits)
                {
                    if (!hit.transform.IsChildOf(transform))
                    {
                        newRootPosition.y = Mathf.Max(newRootPosition.y, hit.point.y);
                    }
                }
                transform.position = newRootPosition;

                //Get body orientation in ground plane for both the ragdolled pose and the animated get up pose
                Vector3 ragdolledDirection = ragdolledHeadPosition - ragdolledFeetPosition;
                ragdolledDirection.y = 0;

                Vector3 meanFeetPosition = 0.5f * (poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.LeftFoot).position + poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.RightFoot).position);
                Vector3 animatedDirection = poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Head).position - meanFeetPosition;
                animatedDirection.y = 0;

                //Try to match the rotations. Note that we can only rotate around Y axis, as the animated characted must stay upright,
                //hence setting the y components of the vectors to zero. 
                transform.rotation *= Quaternion.FromToRotation(animatedDirection.normalized, ragdolledDirection.normalized);
            }

            //compute the ragdoll blend amount in the range 0...1
            float ragdollBlendAmount = 1.0f - (Time.time - ragdollingEndTime - mecanimToGetUpTransitionTime) / ragdollToMecanimBlendTime;
            ragdollBlendAmount = Mathf.Clamp01(ragdollBlendAmount);

            //In LateUpdate(), Mecanim has already updated the body pose according to the animations. 
            //To enable smooth transitioning from a ragdoll to animation, we lerp the position of the hips 
            //and slerp all the rotations towards the ones stored when ending the ragdolling
            foreach (BodyPart b in bodyParts)
            {
                if (b.transform != transform)
                { //this if is to prevent us from modifying the root of the character, only the actual body parts
                  //position is only interpolated for the hips
                    if (b.transform == poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Hips))
                        b.transform.position = UnityEngine.Vector3.Lerp(b.transform.position, b.storedPosition, ragdollBlendAmount);
                    //rotation is interpolated for all body parts
                    b.transform.rotation = Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);
                }
            }

            //if the ragdoll blend amount has decreased to zero, move to animated state
            if (ragdollBlendAmount == 0)
            {
                Vector3 poncherPos = poncherCharacter.transform.position;
                poncherPos.z = 0;
                poncherCharacter.transform.position = poncherPos;
                state = RagdollState.animated;

                return;
            }

        }

    }

    void FindBones()
    {
        Rigidbody[] bones = RootParent.GetComponentsInChildren<Rigidbody>();

        if (bones.Length > 0)
        {
            foreach (Rigidbody bone in bones)
            {
                if (bone != null)
                {
                    bone.gameObject.tag = "RagdollBone";

                    ragdollBones.Add(bone.gameObject);
                    
                    //fill body parts for animation porpuses
                    BodyPart bodyPart = new BodyPart();
                    bodyPart.transform = bone.transform;
                    bodyParts.Add(bodyPart);

                }
                   
            }

        }
    }

    void FindJoints()
    {
        CharacterJoint[] jointsBackup = RootParent.GetComponentsInChildren<CharacterJoint>();
        if (jointsBackup.Length > 0)
        {
            foreach (CharacterJoint chJoint in jointsBackup)
            {
                if (chJoint != null)
                {
                    joints.Add(chJoint);
                    chJoint.enableProjection = true;
                    //chJoint.enableCollision = false;
                }
                
        
            }
        }
    }
    void InitBones()
    {
        foreach (GameObject bone in ragdollBones)
        {
            RagdollBone rgBone = bone.gameObject.AddComponent<RagdollBone>();
            rgBone.poncherCharacter = poncherCharacter;
            bone.gameObject.layer = 11;
            rgBone.boneRB = rgBone.gameObject.GetComponent<Rigidbody>();
        }
    }

    public void SwitchBones(bool isRagdoll)
    {
        IsRagdoll = isRagdoll;

        if (IsRagdoll)
        {
            ActivateRagdoll();
            state = RagdollState.ragdolled;

            Rigidbody hipBodie = poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
            hipBodie.constraints = RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            DeactivateRagdoll();
            ragdollingEndTime = Time.time; //store the state change time
            state = RagdollState.blendToAnim;

            //Store the ragdolled position for blending
            foreach (BodyPart b in bodyParts)
            {
                b.storedRotation = b.transform.rotation;
                b.storedPosition = b.transform.position;
            }

            //Remember some key positions
            ragdolledFeetPosition = 0.5f * (poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.LeftToes).position + poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.RightToes).position);
            ragdolledHeadPosition = poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Head).position;
            ragdolledHipPosition = poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Hips).position;

            //Initiate the get up animation
            if (poncherCharacter.GetAnimator().GetBoneTransform(HumanBodyBones.Hips).forward.y > 0) //hip hips forward vector pointing upwards, initiate the get up from back animation
            {
                poncherCharacter.GetAnimator().SetBool("GetUpFromBack", true);
            }
            else
            {
                poncherCharacter.GetAnimator().SetBool("GetUpFromBelly", true);
            }
        }
    }


    public void ActivateRagdoll()
    {
        //Handles the phisic oproperties for the poncehr root character, not for the bones
        poncherCharacter.GetAnimator().enabled = false;  //Deactivate character colldiers and rigidbodie

        poncherCharacter.GetRigidbody().isKinematic = true;
        poncherCharacter.GetComponent<Collider>().enabled = false;

        //poncherCharacter.GetComponent<MoveComponent>().enabled = !value;
        //Vector3 velDir = poncherCharacter.GetComponent<Rigidbody>().velocity;
        //velDir *= 0.5f;

        foreach (GameObject bone in ragdollBones)
        {
            bone.GetComponent<Rigidbody>().velocity = Vector3.zero;
            bone.GetComponent<Rigidbody>().isKinematic = false;
            bone.GetComponent<Collider>().enabled = true;
            //bone.GetComponent<Rigidbody>().velocity = velDir;
        }

        int randomBone = Random.Range(0, ragdollBones.Count);
        ragdollBones[randomBone].GetComponent<Rigidbody>().AddForce(Vector3.up * 800f, ForceMode.Impulse);

        ragdollBones[randomBone].GetComponent<Rigidbody>().AddForce(Vector3.right * 150f, ForceMode.Impulse);



    }

    public void DeactivateRagdoll()
    {


        //Activate player components for player controller
        poncherCharacter.GetRigidbody().isKinematic = false;
        poncherCharacter.GetComponent<Collider>().enabled = true;

        //poncherCharacter.GetComponent<MoveComponent>().enabled = !value;
        //Vector3 velDir = poncherCharacter.GetComponent<Rigidbody>().velocity;
        //velDir *= 0.5f;

        foreach (GameObject bone in ragdollBones)
        {
            bone.GetComponent<Rigidbody>().isKinematic = true;
            bone.GetComponent<Collider>().enabled= false;
            //bone.GetComponent<Rigidbody>().velocity = velDir;
        }

        //Handles the phisic oproperties for the poncehr root character, not for the bones
        poncherCharacter.GetAnimator().enabled = true;
    }

  

    
    //Function fir debug  mass
    public void SetWeight()
    {
        //TODO:: Depending the type of the bone 
        foreach (GameObject bone in ragdollBones)
        {
            Rigidbody bodie = bone.GetComponent<Rigidbody>();
            //bodie.mass *= 4f;
            bodie.interpolation = RigidbodyInterpolation.None;
            bodie.collisionDetectionMode = CollisionDetectionMode.Continuous;

            //bodie.angularDrag = 0f;
            //bodie.angularDrag = 0f;
            //Debug.Log(bodie.gameObject.name + " " + bodie.mass);           
        }

        foreach (CharacterJoint joint in joints)
        {
            //joint.enableProjection = false;
            //joint.enableCollision = false;
        }
    }
    public void SetPhysixMaterial(PhysicMaterial pxMat)
    {
        foreach (GameObject bone in ragdollBones)
        {
            Collider coll = bone.GetComponent<Collider>();
            coll.material = pxMat;
            //Debug.Log(bodie.gameObject.name + " " + bodie.mass);
        }
    }


    public void AddForceToBones(Vector3 force, float forceMultiplier, Vector3 position, ForceMode forceMode) 
    {
        foreach (GameObject bone in ragdollBones)
        {
            Rigidbody boneRB = bone.GetComponent<Rigidbody>();          
            boneRB.AddForce(force * forceMultiplier, forceMode);
            //boneRB.AddForceAtPosition(force * forceMultiplier, position, ForceMode.VelocityChange);
            //boneRB.AddExplosionForce(forceMultiplier, position, 5f, 0.5f, forceMode);
        }
    }

    //Getters Stters
    public bool GetIsRagdolled()
    {
        return state != RagdollState.animated;
    }
    public void SetIsRagdolled(bool ragdolled) { }

}
