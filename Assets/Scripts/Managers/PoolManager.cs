using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Events;

namespace Components
{
    [Serializable]
    public class PoolKeyInfo
    {
        public GameObject prefab;
        public int size;
        public List<GameObject> references;
        public List<int> FreeQueue;
        public GameObject container;
    }

    public class PoolManager : MonoBehaviour
    {
        public Transform PoolsContainer;
        public List<PoolKeyInfo> poolLists;
        public Dictionary<string, List<GameObject>> PoolsDictionary = new Dictionary<string, List<GameObject>>();
        public Dictionary<string, Queue<int>> freeObjectsDict = new Dictionary<string, Queue<int>>();


        [Header("UI - Notifications - Messages")]
        public GameObject msgPrefab;


        private void Awake()
        {
            InitPooler();
            EventManager.EM.RegisterListener(PS_EventType.Pooler, OnPoolEvent, this.GetType().Name);
        }




        #region Init Methods
        void InitPooler()
        {
            FillPoolsdictionary();
            //testing
            //IncreasePool("SquarePooler", 5);
        }


        void FillPoolsdictionary()
        {
            foreach (PoolKeyInfo pki in poolLists)
            {
                CreatePool(pki);
            }
        }

        #endregion




        #region EVENTS
        public void OnPoolEvent(EventInfo ei)
        {
            PoolEventInfo pei = (PoolEventInfo)ei;

            if (ei.Type != PS_EventType.Pooler)
                return;

            switch (pei.pInfoType)
            {
                case PoolInfoType.RequestNewPool:
                    CreateNewPool(pei);
                    break;

                case PoolInfoType.Spawn:
                    //HandleSpawn(pei);
                    break;

                case PoolInfoType.Recycle:
                    RecycleInstance(pei);
                    break;

                case PoolInfoType.WorldUIMessage:
                    //HandleworldMessage(pei);
                    break;



                default:
                    break;
            }
        }

        void CreateNewPool(PoolEventInfo pei)
        {
            List<PoolKeyInfo> pkeyInfoList = poolLists.Where(x => x.prefab.name == pei.key).ToList();
            if (pkeyInfoList.Count > 0)
                return;

            PoolKeyInfo pki = new PoolKeyInfo();
            pki.prefab = (GameObject)pei.src; 
            pki.size = 1;
            poolLists.Add(pki);
            CreatePool(pki);
        }

        //void HandleSpawn(PoolEventInfo pei)
        //{
        //    Debug.Log($"[EV_PoolerManager] Spanw Request processing: {pei.info} infotype: {pei.pInfoType} key: {pei.key}");

        //    if (!PoolsDictionary.ContainsKey(pei.key))
        //        return;

        //    //checking if property is EV_SimpleObjectData
        //    //SRC is a vector for positions to spawn
        //    //Property is as EV_SimpleObjectData for initializate simpleobject
        //    //Debug.Log($"[EV_PoolerManager] Type: {pei.property.GetType()}");
        //    if (pei.property.GetType() == typeof(EV_SimpleObjectData))
        //    {
        //        //Debug.Log($"[EV_PoolerManager] Trye to spwan simpleobject int {pei.src}");
        //        GameObject go = GetFreeInstance(pei.key);
        //        EV_SimpleObject so = go.GetComponent<EV_SimpleObject>();
        //        so.Init((EV_SimpleObjectData)pei.property);
        //        Vector2 pos = (Vector2)pei.src;
        //        so.transform.position = new Vector3(pos.x, pos.y, 0f);
        //        so.gameObject.SetActive(true);
        //    }

        //}

        //private void HandleworldMessage(EV_PoolEventInfo pei)
        //{
        //    GameobjectData msgData = (GameobjectData)pei.property;

        //    GameObject MessagePrefab = GetFreeInstance(msgPrefab.name);

        //    EV_SingleMessage singleMsg = MessagePrefab.GetComponent<EV_SingleMessage>();

        //    singleMsg.ThrowMessage(msgData.position, msgData.message, msgData.color);

        //}

        private void RecycleInstance(PoolEventInfo pei)
        {
            int index = (int)pei.src;
            GameObject go = PoolsDictionary[pei.key][index];
            freeObjectsDict[pei.key].Enqueue(index);
            go.SetActive(false);
        }
        #endregion



        #region Pool Utilities
        public GameObject GetFreeInstance(string key)
        {
            GameObject go = null;
            if (!freeObjectsDict.ContainsKey(key))
                return null;


            if (freeObjectsDict[key].Count > 0)
            {//there are free objects
                int index = freeObjectsDict[key].Dequeue();
                go = PoolsDictionary[key][index];
            }
            else
            {
                //No free objects available
                IncreasePool(key, 5);
                int index = freeObjectsDict[key].Dequeue();
                go = PoolsDictionary[key][index];
            }

            return go;
        }

        public bool CreatePool(PoolKeyInfo pki)
        {
            bool success = false;
            string key = pki.prefab.name;

            Debug.Log($"[POOLER MANAGER] <color=yellow>Creating container and pooler name: {key}</color>");

            PoolsDictionary.Add(key, new List<GameObject>());
            pki.references = PoolsDictionary[key];
            GameObject poolContainer = Instantiate(new GameObject(key), PoolsContainer);
            poolContainer.name = key;
            pki.container = poolContainer;
            freeObjectsDict.Add(key, new Queue<int>());

            for (int i = 0; i < pki.size; i++)
            {
                GameObject newObj = Instantiate(pki.prefab, poolContainer.transform);
                newObj.SetActive(false);
                newObj.name = $"{key}_{string.Format("{0:0000}", i)}";
                PoolsDictionary[key].Add(newObj);
                freeObjectsDict[key].Enqueue(i);

                PoolInfo poolInfo = newObj.GetComponent<PoolInfo>();
                poolInfo.objectName = key;
                poolInfo.Id = i;
            }

            Debug.Log($"[POOLER MANAGER] <color=yellow>Free objects QUEUE size: {freeObjectsDict[key].Count}</color>");
            return success;
        }


      

        public bool IncreasePool(string key, int increaseAmount)
        {
            bool success = false;
            //find pool info
            List<PoolKeyInfo> lpki = poolLists.Where(x => x.prefab.name == key).ToList();
            PoolKeyInfo pki = null;
            if (lpki.Count > 0) pki = lpki[0];

            if (!PoolsDictionary.ContainsKey(key))
            {
                if (pki != null)
                {
                    CreatePool(pki);
                }
            }
            else
            {
                if (pki != null)
                {
                    ExpandPool(pki, increaseAmount);
                }
            }

            return success;
        }

        public bool ExpandPool(PoolKeyInfo pki, int increaseAmount)
        {
            bool success = false;
            int currentsize = pki.references.Count;
            string key = pki.prefab.name;

            Debug.Log($"[POOLER MANAGER] <color=yellow>Expanding pooler: {key} => amount: {increaseAmount}</color>");
            //find
            GameObject poolContainer = pki.container;
            for (int i = currentsize; i < currentsize + increaseAmount; i++)
            {
                GameObject newObj = Instantiate(pki.prefab, poolContainer.transform);
                newObj.SetActive(false);
                newObj.name = $"{key}_{string.Format("{0:0000}", i)}";
                PoolsDictionary[key].Add(newObj);
                freeObjectsDict[key].Enqueue(i);

                PoolInfo poolInfo = newObj.GetComponent<PoolInfo>();
                poolInfo.objectName = key;
                poolInfo.Id = i;
            }
            pki.size = pki.references.Count;
            return success;
        }
        #endregion






    }
}
