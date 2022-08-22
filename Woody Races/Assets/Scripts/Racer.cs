using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    // Start is called before the first frame update
    CheckpointManager checkpointManager;
    SimpleCarController carController;
    CpuController cpuController;
    public int racerIndex;
    int currentCheckpoint;
    
    void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
        carController = GetComponent<SimpleCarController>();
        cpuController = GetComponent<CpuController>();
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
            var pickup = other.GetComponent<FuelPickup>();
            if(pickup!=null)
            {
                carController.AddFuel(pickup.Value);
                StartCoroutine(pickup.Collect());
                //cpuController?.TargetReached(other.transform);
            }
            else
            {
                Debug.Log(other.gameObject.name);
            }
        }
        if(other.gameObject.tag=="cpuTarget")
        {
            cpuController?.TargetReached(other.transform);
        }
    }

    
}
