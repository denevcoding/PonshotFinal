using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public enum AppInfoType
    {
        //---CONTROL---
        AppStateChange,
        //---INFO-----
        AppStateInform
    }

    public enum AppActionType
    {
        Load,
        Unload,
        ExitRequest,
    }


    public class AppEventInfo : EventInfo
    {
        public AppInfoType appInfoType;
        public AppActionType appActionType;
        public AppState appState;


        public AppEventInfo(PS_EventType _type, string _info, AppInfoType _infoType, AppActionType _actType, AppState _appState)
        : base(_type, _info)
        {
            this.appInfoType = _infoType;
            this.appActionType = _actType;
            this.appState = _appState;
        }

        public AppEventInfo(PS_EventType _type, string _info, AppState _appState)
        : base(_type, _info)
        {
            this.appState = _appState;
        }
    }
}

