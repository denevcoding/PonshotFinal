using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball Info", menuName = "New Ball Info")]
public class BallInfoSB : ScriptableObject
{
    [Header("General Info")]
    public string BallName;
    [TextArea]
    public string BallDescription;


    [Header("Physic Settings")]
    public PhysicMaterial PhyxMat;
    [Range(0f, 1f)] public float LinearDrag;
    [Range(0f, 1f)] public float AngularDrag;
    [Tooltip("Speed in wich the ball can Punch a player Normally compared with the magniutude of vector velocity")]
    public float SpeedToPunch;
    

    [Header("Sound Settings")]
    public AudioClip[] wallBouncesClips;
    public AudioClip[] poncherHitsClips;

}
