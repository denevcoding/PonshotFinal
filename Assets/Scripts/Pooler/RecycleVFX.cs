using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleVFX : MonoBehaviour
{
    private ObjectPool pooler;
    public float TimeToRecycle;

    private void OnEnable()
    {
        StartCoroutine(CheckIfAlive());
    }

    // Start is called before the first frame update
    void Start()
    {
        pooler = FindObjectOfType<ObjectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (pooler != null)
        {
            pooler.ReturnPrefab(this.gameObject);
        }
    }


    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeToRecycle);
            this.gameObject.SetActive(false);
            //if (!GetComponent<ParticleSystem>().IsAlive(true))
            //{
            //    this.gameObject.SetActive(false);
            //    break;
            //}
        }
    } 
}
