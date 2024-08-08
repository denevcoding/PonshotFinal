using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public enum PS_EventType
    {
        //Debug,
        App,           //Event throwed for Game
        Gameplay,
        //Information,
        //Statistic,
        Transition,
        Pooler,
        Player,
    }


    public class EventInfo
    {
        public PS_EventType Type;
        public string info;

        public EventInfo(PS_EventType type, string info)
        {
            Type = type;
            this.info = info;
        }
    }
}


