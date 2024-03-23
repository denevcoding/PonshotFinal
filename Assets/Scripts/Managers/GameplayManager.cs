using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Components
{
    public class GameplayManager : SingletonTemplate<GameplayManager>
    {
        [SerializeField] private GameData GameData;

        [Header("Gameplay Objects")]
        [SerializeField] PoolManager m_PoolManager;


        [Header("GAMEPLAY MANAGER SETTINGS")]
        //Players Settings
        public GameObject[] CharacterPrefabs;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }





        #region Getters Setters
        public PoolManager GetPooler()
        {
            return m_PoolManager;
        }

        #endregion
    }
}

