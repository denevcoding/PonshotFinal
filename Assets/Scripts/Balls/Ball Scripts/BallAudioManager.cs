using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAudioManager : MonoBehaviour
{
    private AudioSource BallSource;
    private PoncherBall ballComponent;

    private void Awake()
    {
        ballComponent = GetComponent<PoncherBall>();

        BallSource = GetComponent<AudioSource>();
        BallSource.volume = 1;
        BallSource.loop = false;
    }


    public void PlayBounce(float bounceForce) 
    {        
        int randomClip = (int)Random.Range(0, ballComponent.BallData.wallBouncesClips.Length);
        AudioClip bounce = ballComponent.BallData.wallBouncesClips[0];
        BallSource.PlayOneShot(bounce, bounceForce);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
