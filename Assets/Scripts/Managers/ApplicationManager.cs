using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Components;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Components
{
    public class ApplicationManager : SingletonTemplate<ApplicationManager>
    {
        //[SerializeField] public GameData gameData;
        public AppState m_AppSstate;

        //public override void Awake()
        //{
        //    base.Awake();
        //    //gameData = GameData.Instance;
        //}

        //Scene Management
        public void LoadScene(string level, LoadSceneMode loadMode)
        {
            SceneManager.LoadScene(level, loadMode);
        }

        public void SetAppState(AppState appState)
        {
            m_AppSstate = appState;
        }


    }
}

