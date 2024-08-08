using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AppState
{
    Splash,
    Menu,
    Gameplay    
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


    //[Serializable]
    //public class GameData
    //{
    //    private static GameData instance = null;
       


    //    public static GameData Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                instance = new GameData();
    //            }
    //            return instance;
    //        }
    //    }

        
    //}


   
}


