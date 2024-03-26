using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Components
{
    public class GameplayManager : SingletonTemplate<GameplayManager>
    {
        [Header("Gameplay Objects")]
        [SerializeField] PoolManager m_PoolManager;


        public override void Awake()
        {
            //base.Awake();
            //m_GameData = GameData.Instance;
        }


        // Start is called before the first frame update
        void Start()
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

