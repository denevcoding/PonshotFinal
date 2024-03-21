using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{    public class EventManager
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
            nEvents = UnityEngine.EventType.GetNames(typeof(PS_EventType)).Length;
            EventListeners = new EventListener[nEvents];
            Datalisteners = new List<DataListener>[nEvents];

            for (int i = 0; i < nEvents; i++)
            {
                //EventListeners[i] = _ => { };
                Datalisteners[i] = new List<DataListener>();
            }
            if (debug) Debug.Log("[EventManager] Creating array of event type with lenght:" + nEvents);
        }



        public void RegisterListener(PS_EventType type, EventListener listener, string name = "", int value = 0)
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

        public void UnregisterListener(PS_EventType type, EventListener listener, string name = "")
        {
            if (name == "" || name == string.Empty)
            {
                Debug.LogError($"[EventManager] You must be assign name for: {listener.Method.Name}");
                return;
            }
            int found = -1;
            for (int i = 0; i < EM.Datalisteners[(int)type].Count; i++)
            {
                if (listener == EM.Datalisteners[(int)type][i].listener)
                {
                    EM.EventListeners[(int)type] -= listener;
                    found = i;
                }
            }
            if (found >= 0)
            {
                EM.Datalisteners[(int)type].RemoveAt(found);
            }
            else
            {
                Debug.LogError($"[EventManager] Listener: {listener.Method.Name} Not found");
            }
        }


        public void DispatchEvent(EventInfo eventInfo)
        {
            if (eventInfo.Type >= 0 && (int)eventInfo.Type < nEvents)
            {
                if (debug)
                    if (EM.EventListeners[(int)eventInfo.Type] != null)
                    {
                        System.Delegate[] dl = EM.EventListeners[(int)eventInfo.Type].GetInvocationList();
                        Debug.Log("[EventManager] --------->Dispatching Event:" + eventInfo.Type + " info:" + eventInfo.info + " TotalListeners: " + dl.Length);
                        List<DataListener> ll = Datalisteners[(int)eventInfo.Type];
                        for (int i = 0; i < ll.Count; i++)
                        {
                            DataListener d = ll[i];
                            Debug.Log("[EventManager] Executing: " + eventInfo.Type + " info:" + eventInfo.info + " [" + d.name + "] Event: " + d.listener.Method.Name + " prop:" + d.value);
                            //d.listener.DynamicInvoke(eventInfo);
                        }
                        int n = 0;
                        foreach (var d in dl)
                        {
                            Debug.Log("[EventManager] Executing-->" + eventInfo.Type + " info:" + eventInfo.info + " [" + n + "] Event: " + ((EventListener)d).Method.Name);
                            //d.DynamicInvoke(eventInfo);
                            n++;
                        }

                    }
                EM.EventListeners[(int)eventInfo.Type]?.Invoke(eventInfo);
            }
        }

        public static void ResetEventManager()
        {
            for (int i = 0; i < EM.EventListeners.Length; i++)
            {
                foreach (var d in EM.EventListeners[i].GetInvocationList())
                {
                    EM.EventListeners[i] -= (EventListener)d;
                }

            }
        }


    }
}

