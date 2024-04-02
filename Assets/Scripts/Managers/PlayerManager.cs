using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Data;

public class PlayerManager : MonoBehaviour
{
    // We instantiate this GameObject to create a new player object.
    // Expected to have a PlayerInput component in its hierarchy.
    public GameObject PlayerPrefab;
    // We want to remove the event listener we install through InputSystem.onAnyButtonPress
    // after we're done so remember it here.
    private IDisposable m_EventListener;

    public InputActionAsset poncherActionAsset;

    [SerializeField] int playersAmount = 0;

    public Transform playerContainer;
    [SerializeField]private Transform[] playerSockets;

    private void OnEnable()
    {
        // Start listening.
        m_EventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
    }

    // When disabled, we remove our button press listener.
    void OnDisable()
    {
        m_EventListener.Dispose();
    }


    private void Awake()
    {
        InitPlayersSockets();              
    }

    // Start is called before the first frame update
    void Start()
    {
        //Response to the first button press. Calls our delegate
        // and then immediately stops listening.
        //InputSystem.onAnyButtonPress
        //    .Call(ctrl => 
        //    {
        //        InputDevice device = ctrl.device;

        //        Debug.Log("but");

           
        //    } );
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region Initialization
    void InitPlayersSockets()
    {
        playerSockets = new Transform[playerContainer.childCount];
        for (int i = 0; i < playerSockets.Length; i++)
        {
            playerSockets[i] = playerContainer.GetChild(i);
        }
    }
    #endregion


    //REgistered to input system callbacks on any button pressed
    void OnButtonPressed(InputControl button)
    {
        InputDevice device = button.device;   

        // Ignore presses on devices that are already used by a player.
        if (PlayerInput.FindFirstPairedToDevice(device) != null)
            return;

        Debug.Log("Device " + device.shortDisplayName);
        Debug.Log($"Button {button} was pressed");

        if (device is Mouse)        
            return;
        

        PlayerInput inputComp = null;
        // Create a new player.
        if (device is Keyboard )
        {
            inputComp = InstantiatePlayerInput(device, "Keyboard&Mouse", playersAmount);

        }
        else if (device is Gamepad)
        {
            inputComp = InstantiatePlayerInput(device, "Gamepad", playersAmount);
            
        }
        else if (device is Joystick)
        {
            Debug.LogWarning("Joystick controller not isntantiating player Yet");
        }


        //Creating and Adding 
        int RandomPoncher = UnityEngine.Random.Range(0, GameData.singleinstance.PonchersData.Length);
        PoncherDataSO poncher = GameData.singleinstance.PonchersData[RandomPoncher];
        GameObject poncherGO = Instantiate(poncher.PoncherPrefab, transform);
        poncherGO.transform.position = playerSockets[playersAmount].position;

        PoncherCharacter poncherChar = poncherGO.GetComponent<PoncherCharacter>();
        poncherChar.BindControllerToInputs(inputComp);

        PlayerGUI playerGUI = inputComp.GetComponent<PlayerGUI>();
        playerGUI.SetPoncher(poncherChar);

        poncherChar.GetController().playerGUI = playerGUI;


        PlayerData playerData = new PlayerData(playersAmount, inputComp, poncherChar, playerGUI);

        GameData.singleinstance.AddPlayerData(playerData);
        playersAmount += 1;
            

        // If the player did not end up with a valid input setup,
        // unjoin the player.
        if (inputComp.hasMissingRequiredDevices)
            Destroy(inputComp);

        // If we only want to join a single player, could uninstall our listener here
        // or use CallOnce() instead of Call() when we set it up.
    }




    PlayerInput InstantiatePlayerInput(InputDevice _device, string _scheme, int _pyIndex)
    {
        PlayerInput player = PlayerInput.Instantiate(PlayerPrefab, -1, _scheme, pairWithDevice: _device);
        //player.transform.parent = this.transform;
        player.transform.position = playerSockets[playersAmount].position;
        return player;
    }





    //Over head method
    public void InputWithEventPTR()
    {
        InputSystem.onEvent
        .Where(e => e.HasButtonPress())
        .Call(eventPtr =>
        {
            foreach (var button in eventPtr.GetAllButtonPresses())
            {
                InputDevice detectedDevice = InputSystem.GetDeviceById(eventPtr.deviceId);
                Debug.Log(detectedDevice.name);
                Debug.Log($"Button {button} was pressed");
            }
        });
    }

}
