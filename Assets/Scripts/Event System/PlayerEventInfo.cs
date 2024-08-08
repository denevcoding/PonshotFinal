using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Events
{
    public enum PlayerInfoType
    {
        Added,
        Removed,
    }
    public class PlayerEventInfo : EventInfo
    {
        public PlayerInfoType pyInfoType;
        public PlayerData playerData;// player related with the launching of the event

        public PlayerEventInfo(PS_EventType _type, string _info, PlayerInfoType _infoType, PlayerData pyData)
        : base(_type, _info)
        {
            this.pyInfoType = _infoType;
            this.playerData = pyData;

        }

       
    }
}

