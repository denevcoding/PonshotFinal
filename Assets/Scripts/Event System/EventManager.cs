using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class EventManager
    {
        static private EventManager _em;
        private int nEvents;
        static public bool debug = false;

        public struct DataListener
        {
            public string name;
            public int value;
            public EventListener listener;
            public DataListener(string _name, int _value, EventListener _listener)
            {
                this.value = _value;
                this.listener = _listener;
                this.name = _name;
            }
        }



        static public EventManager EM
        {
            get
            {
                if (_em == null)
                {
                    _em = new EventManager();
                }
                return _em;
            }
        }

        //Listener properties
        public delegate void EventListener(EventInfo ei);
        public EventListener[] EventListeners;
        public List<DataListener>[] Datalisteners;

        public EventManager()
        {
            nEvents = EventType.GetNames(typeof(EV_EventType)).Length;
            EventListeners = new EventListener[nEvents];
            Datalisteners = new List<DataListener>[nEvents];

            for (int i = 0; i < nEvents; i++)
            {
                //EventListeners[i] = _ => { };
                Datalisteners[i] = new List<DataListener>();
            }
            if (debug) Debug.Log("[EventManager] Creating array of event type with lenght:" + nEvents);
        }



        public void RegisterListener(EV_EventType type, EventListener listener, string name = "", int value = 0)
        {
            if (name == "" || name == string.Empty)
            {
                Debug.LogError($"[EventManager] You must be assign name for: {listener.Method.Name}");
                return;
            }
            DataListener d = new DataListener(name, value, listener);
            EM.Datalisteners[(int)type].Add(d);
            EM.EventListeners[(int)type] += listener;
        }


    }
}

