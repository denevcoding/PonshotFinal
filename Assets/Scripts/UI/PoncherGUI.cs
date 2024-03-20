using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoncherGUI : MonoBehaviour
{
    private PoncherCharacter ownerPoncher;
    private Vector3 offset;

    public ShootPointer shooterPointer;
    public GameObject indicatorPoncher;

    private PlayerInput m_inputComponent;


    private void Awake()
    {
        m_inputComponent = GetComponent<PlayerInput>();
        shooterPointer.InitAimer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowPoncher();
    }

    public void FollowPoncher()
    {
        if (ownerPoncher)
        {
            transform.position = ownerPoncher.transform.position + offset;
        }
    }

    //Handle intialization for All world space poncher GUI
    public void InitPoncheGUI(PoncherCharacter owner)
    {
        if (owner != null)
        {
            shooterPointer.SetPoncherOwner(owner);
            ownerPoncher = owner;
            offset = transform.position - ownerPoncher.GetPoncherCenteredPosition();
        }
    }


    #region Getters Setters
    public PlayerInput GetInputcomp()
    {
        return m_inputComponent;
    }
    #endregion
}
