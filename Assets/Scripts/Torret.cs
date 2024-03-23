using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Components
{
    public class Torret : PonshotEntity
    {
        public GameObject prefabToShoot;
        public float ShootForce;
        public Transform ShootPointer;



        // Start is called before the first frame update
        void Start()
        {   
            Invoke("LoadBall", 1f);   
        }


        void LoadBall()
        {
            GameObject objCopy = GameplayManager.singleinstance.GetPooler().GetFreeInstance(prefabToShoot.name);
            IPickeable pickeable = objCopy.GetComponent<IPickeable>();

            
            objCopy.SetActive(true);
            pickeable.Picked(this, ShootPointer);
            pickeable.Throwed(10f, 1, ShootPointer);

            Invoke("LoadBall", 1f);
        }

        //void Shoot()
        //{
        //    IPickeable pickeable = prefabToShoot.GetComponent<IPickeable>();
           

        //}
    }
}

