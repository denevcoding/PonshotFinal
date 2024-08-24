using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Data;
using Components;
using Events;

public class PlayerManager : SingletonTemplate<PlayerManager>
{
    //[SerializeField] private GameData gameData;
    // We instantiate this GameObject to create a new player object.
    // Expected to have a PlayerInput component in its hierarchy.

    [Space(20)]
    public Transform playerContainer;
    [SerializeField] private Transform[] playerSockets;
    public GameObject PlayerPrefab;
    public InputActionAsset poncherActionAsset;

    // We want to remove the event listener we install through InputSystem.onAnyButtonPress
    // after we're done so remember it here.
    private IDisposable m_EventListener;

    [SerializeField] int playersAmount = 0;


    [Header("Players Data")]
    public Dictionary<int, PlayerInput> Players = new Dictionary<int, PlayerInput>();
    public List<PlayerData> playersData = new List<PlayerData>();

    [Header("Ponchers Data")]
    //Players Settings
    public PoncherDataSO[] PonchersData;





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


    public override void Awake()
    {
        base.Awake();
        //gameData = GameData.Instance; 
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


    //Registered to input system callbacks on any button pressed
    void OnButtonPressed(InputControl inputControl)
    {
        InputDevice device = inputControl.device;

        // Ignore presses on devices that are already used by a player.
        if (PlayerInput.FindFirstPairedToDevice(device) != null)
            return;

        //Debug.Log("Alliases " + inputControl.aliases[0]);
        Debug.Log("Device " + device.shortDisplayName);
        Debug.Log($"Button {inputControl} was pressed");
        // Debug.Log($"Descriptioon {device.description.manufacturer} {device.description.product} xD");


        if (ApplicationManager.singleinstance.m_AppSstate == AppState.Splash || ApplicationManager.singleinstance.m_AppSstate == AppState.Menu || ApplicationManager.singleinstance.m_AppSstate == AppState.Gameplay)
        {
            //InputControl control = ;
            HandleJoinPlayers(device);

            //if (InputControlPath.TryFindControl(inputControl, "start") != null || InputControlPath.TryFindControl(inputControl, "enter") != null)
            //{         
            //    HandleJoinPlayers(device);
            //}          
       
        }
        //else if (ApplicationManager.singleinstance.m_AppSstate == AppState.Menu)
        //{
        //    if (InputControlPath.TryFindControl(inputControl, "start") != null || InputControlPath.TryFindControl(inputControl, "enter") != null)            
        //        HandleJoinPlayers(device);                  
        //}
     

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



    GameObject HandleJoinPlayers(InputControl button)
    {
        InputDevice device = button.device;

        if (device is Mouse)
            return null;

        PlayerInput inputComp = null;
        // Create a new player.
        if (device is Keyboard)
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
        int RandomPoncher = UnityEngine.Random.Range(0, PonchersData.Length);
        PoncherDataSO poncher = PonchersData[RandomPoncher];
        GameObject poncherGO = Instantiate(poncher.PoncherPrefab);
        poncherGO.transform.position = playerSockets[playersAmount].position;

        PoncherCharacter poncherChar = poncherGO.GetComponent<PoncherCharacter>();
        poncherChar.BindControllerToInputs(inputComp);

        PlayerGUI playerGUI = inputComp.GetComponent<PlayerGUI>();
        playerGUI.SetPoncher(poncherChar);

        poncherChar.poncherGUI = playerGUI;
        poncherChar.GetController().playerGUI = playerGUI;


        PlayerData playerData = new PlayerData(playersAmount, inputComp, poncherChar, playerGUI);

        AddPlayerData(playerData);
        playersAmount += 1;


        // If the player did not end up with a valid input setup,
        // unjoin the player.
        if (inputComp.hasMissingRequiredDevices) {
            Destroy(inputComp);
            return null;
        }

        //Launching event when new player is added to the Game. We should 
        PlayerEventInfo pyei = new PlayerEventInfo(PS_EventType.Player, "Recycling Ball", PlayerInfoType.Added, playerData);
        EventManager.EM.DispatchEvent(pyei);


        return poncherGO;
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




    //Players Management
    public void AddPlayer(int _index, PlayerInput _inputComp)
    {
        Players.TryAdd(Players.Count, _inputComp);
        Debug.Log("Players: " + Players.Count);

        foreach (KeyValuePair<int, PlayerInput> player in Players)
        {
            Debug.Log("Player " + player.Key + ": " + player.Value.user.pairedDevices[0].displayName + " " + "Schema: " + player.Value.currentControlScheme);
        }
    }

    public void AddPlayerData(PlayerData _player)
    {
        if (_player != null)
        {
            playersData.Add(_player);
            Debug.Log("Player " + _player.m_PlayerIndex + ": " + _player.m_InputComp.user.pairedDevices[0].displayName + " " + "Schema: " + _player.m_InputComp.currentControlScheme);
        }

    }








}
