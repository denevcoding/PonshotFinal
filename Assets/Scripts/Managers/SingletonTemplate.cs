using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class SingletonTemplate<T> : MonoBehaviour where T : Component
    {
        private static T Instance;

        [Header("Manager General Settings")]
        public bool Persistant; //indicates if this manager persist all the application or just local kto the scene


        public static T singleinstance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = FindObjectOfType<T>();
                    if (Instance == null)
                    {
                        GameObject gameObj = new GameObject();
                        gameObj.name = typeof(T).Name;
                        Instance = gameObj.AddComponent<T>();
                    }
                }

                return Instance;
            }
        }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;

                if (Persistant)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
            }
            else if (Instance != this)
            {             
                Destroy(gameObject);
            }

        }
    }
}

