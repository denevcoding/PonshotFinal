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
    public class GameData
    {
        public AppState m_AppSstate;
        private static GameData instance;

        public Dictionary<int, PlayerInput> Players = new Dictionary<int, PlayerInput>();

        [Header("Ponchers Prefabs")]
        //Players Settings
        public GameObject[] CharacterPrefabs;

        public static GameData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameData();
                }
                return instance;
            }
        }


        public  void AddPlayer(int _index, PlayerInput _inputComp)
        {
            Players.TryAdd(_index, _inputComp);
            Debug.Log("Players: " + Players.Count);

            foreach (KeyValuePair<int, PlayerInput> player in Players)
            {
                Debug.Log("Player " + player.Key + ": " + player.Value.user.pairedDevices[0].displayName + " " + "Schema: " + player.Value.currentControlScheme);
            }
          
        }
    }
}


