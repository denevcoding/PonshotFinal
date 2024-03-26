using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Poncher Info", menuName = "Ponshot Data/Poncher")]
public class PoncherDataSO : ScriptableObject
{
    public GameObject PoncherPrefab;

    [TextArea(1,5)]
    public string PoncherDescription;
}
