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

    [SerializeField] GameData m_GameData;
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
        m_GameData = GameData.Instance;

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


        PlayerInput player = null;
        // Create a new player.
        if (device is Keyboard || device is Mouse)
        {
            player = InstantiatePlayer(device, "Keyboard&Mouse", playersAmount);

        }
        else if (device is Gamepad)
        {
            player = InstantiatePlayer(device, "Gamepad", playersAmount);
            
        }
        else if (device is Joystick)
        {
            Debug.LogWarning("Joystick controller not isntantiating player Yet");
        }
        playersAmount += 1;


        m_GameData.AddPlayer(playersAmount, player);

        // If the player did not end up with a valid input setup,
        // unjoin the player.
        if (player.hasMissingRequiredDevices)
            Destroy(player);

        // If we only want to join a single player, could uninstall our listener here
        // or use CallOnce() instead of Call() when we set it up.
    }


    PlayerInput InstantiatePlayer(InputDevice _device, string _scheme, int _pyIndex)
    {
        PlayerInput player = PlayerInput.Instantiate(PlayerPrefab, -1, _scheme, pairWithDevice: _device);
        player.transform.parent = this.transform;
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