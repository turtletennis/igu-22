using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Checkpoint> checkpoints;
    [SerializeField] private int totalLaps;
    [SerializeField] private List<Racer> racers;
    private int racerCount;
    private List<int> racerLapNumbers;
    private List<int> racerCheckpoints = new List<int>();
    public int CheckpointCount{get{return checkpoints.Count;}}
    
    // Start is called before the first frame update
    void Start()
    {
        racerCount = racers.Count;
        checkpoints[0].FinishLine = true;
        for(int i = 0; i < checkpoints.Count; i++)
        {
            if(checkpoints[i] == null) Debug.Log($"Missing checkpoint {i}");
            checkpoints[i].Index = i;
        }
        racerLapNumbers = new List<int>();
        for(int i=0; i < racerCount; i++)
        {
            racerLapNumbers.Add(0);
            racerCheckpoints.Add(CheckpointCount - 1);
        }
    }

    public void CheckpointHit(int racerIndex,Checkpoint checkpoint)
    {
        if(checkpoint.FinishLine)
        {
            if(racerCheckpoints[racerIndex]==CheckpointCount - 1)
            {
                AddLap(racerIndex);
            }
            else
            {
                Debug.Log($"Racer {racerIndex} hit checkpoint {checkpoint.Index} but is currently at {racerCheckpoints[racerIndex]} failed to increment");
            }
            
        }
        else
        {
            if(racerCheckpoints[racerIndex]==checkpoint.Index - 1)
            {
                racerCheckpoints[racerIndex] ++;
                Debug.Log($"Racer {racerIndex} now at checkpoint {checkpoint.Index}");
            }
            else
            {
                Debug.Log($"Racer {racerIndex} hit checkpoint {checkpoint.Index} but is currently at {racerCheckpoints[racerIndex]} failed to increment");
            }
        }
    }

    public void AddLap(int playerIndex)
    {
        racerLapNumbers[playerIndex]++;
        racerCheckpoints[playerIndex] = 0;
        if(racerLapNumbers[playerIndex] > totalLaps)
        {
            Debug.Log("Finish!");
        }
        else
        {
            Debug.Log($"Lap {racerLapNumbers[playerIndex]}/{totalLaps}");
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
