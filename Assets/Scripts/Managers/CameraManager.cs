using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Components 
{
    public class CameraManager : SingletonTemplate<CameraManager>
    {
        public CinemachineBrain brain;
        public CinemachineVirtualCamera currentCamera;



        public override void Awake()
        {
            base.Awake();
            brain = Camera.main.GetComponent<CinemachineBrain>();
        }

        public void SwitchCamera(CinemachineVirtualCameraBase newCamera)
        {
          
            newCamera.Priority = brain.ActiveVirtualCamera.Priority;
            brain.ActiveVirtualCamera.Priority = newCamera.Priority-10;
        }

        public void SetFollowTargetToActive(Transform fTarget)
        {
            brain.ActiveVirtualCamera.Follow = fTarget;
        }

    }
}


