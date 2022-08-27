using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioSource constantNoise;
    [SerializeField] AudioSource revNoise;
    bool revving;
    float decayPerSecond=0.2f;
    float engineRev;
    void Start()
    {
        //set random position for different racers so they don't sound identical
        constantNoise.time = Random.Range(0,constantNoise.clip.length);
        //constantNoise.pitch = Random.Range(0.95f,1.05f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!revving && revNoise.isPlaying)
        {
            engineRev -= decayPerSecond * Time.deltaTime;
            if(engineRev < 0)
            {
                revNoise.Stop();
                Debug.Log("Stopped rev noise");
            }
        }
    }

    public void SetAcceleration(float magnitude)
    {
        if(magnitude>float.Epsilon)
        {
            revving = true;
            engineRev = 1;
            if(!revNoise.isPlaying)
            {
                revNoise.Play();
                Debug.Log("Playing sound");
            }
        }
        else if(magnitude <= float.Epsilon && revNoise.isPlaying)
        {
            revving = false;
            //revNoise.Stop();
            //Debug.Log("Stopping sound");
        }
    }

    
}
