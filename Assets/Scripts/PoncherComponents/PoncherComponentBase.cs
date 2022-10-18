using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoncherComponentBase : MonoBehaviour
{

    public PoncherCharacter m_poncherCharacter; //Init from poncher character character

    public virtual bool CheckPreconditions()
    {
        if (m_poncherCharacter.GetState() == PoncherState.Ponched)
            return false;


        if (m_poncherCharacter.GetState() == PoncherState.Stumbled)
            return false;

        return true;
    }
}
