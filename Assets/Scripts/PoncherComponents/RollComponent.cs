using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollComponent : PoncherComponentBase
{
    //Roll Settings
    public bool canRoll;

    // Start is called before the first frame update
    void Start()
    {
        canRoll = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ParkourRoll()
    {
        if (CheckPreconditions() == false)
            return;

        if (m_poncherCharacter.GetState() == PoncherState.Rolling)
            return;

        if (m_poncherCharacter.coyoteTimeCounter < 0)
            return;

        if (!canRoll)
            return;

        m_poncherCharacter.GetAnimator().SetTrigger("Roll");

    }
}
