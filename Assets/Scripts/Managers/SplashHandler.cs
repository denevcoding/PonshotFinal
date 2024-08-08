using Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Events;

namespace Components
{
    public class SplashHandler : MonoBehaviour
    {
        public GameObject SplashPanel;

        public void HideSplash()
        {
            SplashPanel.SetActive(false);            
        }

    }
}

