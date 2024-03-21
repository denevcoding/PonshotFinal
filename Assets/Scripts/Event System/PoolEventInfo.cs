using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events 
{
    public enum PoolInfoType
    {
        RequestNewPool,
        Spawn,
        Recycle,
        WorldUIMessage,
    }

    public class PoolEventInfo : EventInfo
    {
        public PoolInfoType pInfoType;
        public string key;
        public object src;
        public object dst;
        public object property;
        public PoolEventInfo(PS_EventType _type, string _info, PoolInfoType _pType, string _key,
            object _src = null, object _dst = null, object _property = null) : base(_type, _info)
        {
            this.pInfoType = _pType;
            this.key = _key;
            this.src = _src;
            this.dst = _dst;
            this.property = _property;
        }
    }

}
