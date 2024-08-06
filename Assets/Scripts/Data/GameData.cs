using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AppState
{
    Menu,
    Gameplay,
}

public enum GameplayState
{
    Running,
    Paused,
    Finished
}




namespace Data
{
    [Serializable]
    public class PlayerData
    {
        public int m_PlayerIndex;
        public PlayerInput m_InputComp;
        public PoncherCharacter m_Poncher;
        public PlayerGUI PlayerInstance; //Instance with UI and player input represent player in the scene

        public PlayerData(int _index, PlayerInput _inputComp, PoncherCharacter _poncher, PlayerGUI _player)
        {
            m_PlayerIndex = _index;
            m_InputComp = _inputComp;
            m_Poncher = _poncher;
            PlayerInstance = _player;
        }
    }


    [Serializable]
    public class GameData : SingletonTemplate<GameData>
    {
        public AppState m_AppSstate;
        private static GameData instance;

        public Dictionary<int, PlayerInput> Players = new Dictionary<int, PlayerInput>();
        public List<PlayerData> playersData = new List<PlayerData>();

        [Header("Ponchers Data")]
        //Players Settings
        public PoncherDataSO[] PonchersData;


        [Header("Player Info")]
        public int PlayersAmount;

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


   
}


