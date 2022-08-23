using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Checkpoint> checkpoints;
    [SerializeField] private int totalLaps;
    [SerializeField] private List<Racer> racers;
    private int racerCount;
    private List<int> racerLapNumbers;
    private List<int> racerCheckpoints = new List<int>();
    private List<int> finishers = new List<int>();
    public int CheckpointCount{get{return checkpoints.Count;}}
    public HudManager hudManager;
    
    // Start is called before the first frame update
    void Start()
    {
        hudManager = FindObjectOfType<HudManager>();
        hudManager.UpdateLapText(1,totalLaps);
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
            racers[i].racerIndex=i;
            racerLapNumbers.Add(0);
            racerCheckpoints.Add(CheckpointCount - 1);
        }
    }

    public void CheckpointHit(int racerIndex,Checkpoint checkpoint,bool isPlayer = false)
    {
        //if(isPlayer) Debug.Log("player hit checkpoint "+checkpoint.Index);
        if(checkpoint.FinishLine)
        {
            if(racerCheckpoints[racerIndex]==CheckpointCount - 1)
            {
                
                AddLap(racerIndex,isPlayer);
            }
            else
            {
                //Debug.Log($"Racer {racerIndex} hit checkpoint {checkpoint.Index} but is currently at {racerCheckpoints[racerIndex]} failed to increment");
            }
            
        }
        else
        {
            if(racerCheckpoints[racerIndex]==checkpoint.Index - 1)
            {
                racerCheckpoints[racerIndex] ++;
                //Debug.Log($"Racer {racerIndex} now at checkpoint {checkpoint.Index}");
            }
            else
            {
                //Debug.Log($"Racer {racerIndex} hit checkpoint {checkpoint.Index} but is currently at {racerCheckpoints[racerIndex]} failed to increment");
            }
        }
    }

    public void AddLap(int racerIndex,bool isPlayer)
    {
        racerLapNumbers[racerIndex]++;
        racerCheckpoints[racerIndex] = 0;
        if(racerLapNumbers[racerIndex] == totalLaps+1)
        {
            finishers.Add(racerIndex);
            Debug.Log("Finish!");
        }
        else
        {
                Debug.Log($"{racerIndex}{isPlayer} Lap {racerLapNumbers[racerIndex]}/{totalLaps}");
        }
        if(isPlayer)
        {
            hudManager.UpdateLapText(racerLapNumbers[racerIndex],totalLaps,GetFinalPosition(racerIndex));
            //hudManager.UpdateLapText(racerLapNumbers[racerIndex],totalLaps);
        }
    }

    private int GetFinalPosition(int racerIndex)
    {
        Debug.Log(string.Join(",",finishers));
        for(int pos=1; pos < finishers.Count; pos++)
        {
            if(finishers[pos]==racerIndex)  return pos;
        }
        return -1;
    }

    private int GetPosition(int racerIndex)
    {
        List<int> positions = new List<int>();
        //[4,3,2,1] [3,4,1,2] [1,0,0,0]
        
        var tempLaps = new List<int>();
        var checkedRacers = new List<bool>();
        foreach(int l in racerLapNumbers)
        {
            tempLaps.Add(l);
            checkedRacers.Add(false);
        }

        while(checkedRacers.Any(r=> r==false))
        {
            int maxLap=-1;
            int maxLapIndex=-1;
            for(int i=0; i<tempLaps.Count; i++)
            {
                //current racer index is on lap > current max lap, haven't already added racer into positions
                if(tempLaps[i]>maxLap && !checkedRacers[i])
                {
                    maxLap=tempLaps[i];
                    maxLapIndex = i;
                }
            }
            positions.Add(maxLapIndex);
            Debug.Log(maxLapIndex);
            checkedRacers[maxLapIndex] = true;
        }
        Debug.Log(string.Join(",",positions));

        for( int p=1; p<=positions.Count; p++)
        {
            if(positions[p-1]==racerIndex) return p;
        }
        throw new UnityException("Could not calculate position of racer " + racerIndex);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
