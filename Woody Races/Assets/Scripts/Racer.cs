using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    // Start is called before the first frame update
    CheckpointManager checkpointManager;
    public int racerIndex;
    int currentCheckpoint;
    void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
        Debug.Log($"Racer start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        var checkpoint = other.GetComponent<Checkpoint>();
        if(checkpoint != null)
        {
            Debug.Log("Checkpoint hit!");
            checkpointManager.CheckpointHit(racerIndex,checkpoint);
        }
        else
        {
            Debug.Log(other.gameObject.name);
        }
    }
}
