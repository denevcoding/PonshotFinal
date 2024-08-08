using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Components
{
    public class MenuManager : SingletonTemplate<MenuManager>
    {
        public SplashHandler splashHandler;

        public CinemachineVirtualCameraBase menuCamera;

        public override void Awake()
        {
            base.Awake();
            EventManager.EM.RegisterListener(PS_EventType.Player, HandlePlayerEvent, this.GetType().Name);
        }

        public void HandlePlayerEvent(EventInfo _eventInfo)
        {
            if (_eventInfo.Type != PS_EventType.Player)
                return;

            PlayerEventInfo pyei = (PlayerEventInfo)_eventInfo;


            if (ApplicationManager.singleinstance.m_AppSstate == AppState.Splash)
            {
                if (pyei.pyInfoType == PlayerInfoType.Added)
                {
                    menuCamera.Follow = pyei.playerData.m_Poncher.transform;
                    menuCamera.LookAt = pyei.playerData.m_Poncher.transform;
                    //CameraManager.singleinstance.SetFollowTargetToActive(firstPlayer.transform);
                    CameraManager.singleinstance.SwitchCamera(menuCamera);
                    splashHandler.HideSplash();

                    //TODO:: Set the poncher Actions maps to use menu, and disable movement and other actions
                }              
            }
            else if (ApplicationManager.singleinstance.m_AppSstate == AppState.Menu)
            {
                switch (pyei.pyInfoType)
                {
                    case PlayerInfoType.Added:
                        break;
                    case PlayerInfoType.Removed:
                        break;
                    default:
                        break;
                }
            }

          
        }
    }
}

