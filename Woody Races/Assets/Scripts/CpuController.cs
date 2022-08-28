using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuController : MonoBehaviour
{
    SimpleCarController simpleCarController;
    CpuTargetsManager cpuTargetsManager;
    public List<Transform> trackTargets;
    public int targetIndex;

    [Header("raycasting")]
    [SerializeField] float collisionAvoidDistance = 50f;
    [SerializeField] float collisionAvoidHorizontalDistance = 10f;
    [SerializeField] float reverseTimeForAvoiding = 3f;
    [SerializeField] float turnAvoidAngle = 75f;
    [SerializeField] Transform rayCastPosition;
    [Header("debugging")]
    [SerializeField] bool debugRay;
    [SerializeField] bool debugLog;
    public bool reversing;
    float trackAngleToNextTarget;
    Vector3 trackCentre;

    void Start()
    {
        if(rayCastPosition == null)
        {
            Debug.Log("Missing raycast position");
            rayCastPosition = transform;
        }
        cpuTargetsManager = FindObjectOfType<CpuTargetsManager>();
        trackTargets = cpuTargetsManager.TrackWayPoints;
        simpleCarController = GetComponent<SimpleCarController>();
        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
        trackCentre = checkpointManager.transform.position;
        SetTargetBearing();

    }

    void Log(string text, bool checkTime=true)
    {
        
        if(debugLog && (!checkTime || Time.time % 10 <1))
        {
            Debug.Log(text);
        }
    }

    void IncrementTargetIndexIfMissedIt()
    {
        
        var deltaPos = transform.position - trackCentre;
        float racerBearing = Mathf.Atan2(deltaPos.z,deltaPos.x) * 180f / Mathf.PI + 180;
        
        float diff = trackAngleToNextTarget - racerBearing;
        if(diff < -180) diff+=360;
        //Log($"racer track angle {racerBearing} target diff angle {diff} to {CurrentTarget.gameObject.name}");
        if(diff < 0)
        {
            Log("Missed target " + CurrentTarget.gameObject.name, false);
            IncrementTarget();
        }
    }

    private Transform CurrentTarget
    { get{ return trackTargets[targetIndex]; } }

    void SetTargetBearing()
    {
        Vector3 deltaPos = CurrentTarget.position - trackCentre;
        trackAngleToNextTarget = Mathf.Atan2(deltaPos.z,deltaPos.x) * 180f / Mathf.PI + 180;
    }

    void IncrementTarget()
    {
        targetIndex ++;
        if(targetIndex >= trackTargets.Count) targetIndex = 0;
        SetTargetBearing();
    }

    // returns angle, -angle or 0. Hit.Collider will be null and angle will be 0 if nothing to hit in front of us, Hit.Collider will not be null and angle will be 0 if we cannot avoid
    float GetCollisionAvoidanceAngle(Vector3 raycastDirection,Transform raycastOrigin, out RaycastHit hit, float raycastDistance, int layerMask,float avoidanceAngle)
    {
        if(Physics.Raycast(rayCastPosition.position,rayCastPosition.forward,out hit,collisionAvoidDistance, layerMask))
        {
            var deltaPos = CurrentTarget.position - transform.position;
            float angle = Vector3.SignedAngle(raycastDirection,deltaPos,raycastOrigin.up);
            if(angle > 0 && angle < avoidanceAngle)
            {
                //aiming angle deg away from forwards doesn't hit anything
                if(!Physics.Raycast(rayCastPosition.position,Quaternion.AngleAxis(avoidanceAngle,rayCastPosition.up) * raycastDirection,collisionAvoidDistance, layerMask))
                {
                    return avoidanceAngle;
                }
            }
            else if(angle < 0 && angle > -avoidanceAngle)
            {
                //aiming -angle deg away from forwards doesn't hit anything
                if(!Physics.Raycast(rayCastPosition.position,Quaternion.AngleAxis(-avoidanceAngle,rayCastPosition.up) * raycastDirection,collisionAvoidDistance, layerMask))
                {
                    return -avoidanceAngle;
                }
            }
        }
        
        return 0;
    }

    float GetTurnForNextTarget()
    {
        var deltaPos = CurrentTarget.position - transform.position;
        float dot = Vector3.Dot(transform.forward,deltaPos);
        float angle = Vector3.SignedAngle(transform.forward,deltaPos,Vector3.up);
        float turn = angle > 0 ? 1 : -1;
        Log($"Turn for the next target is {turn}");
        
        int layerMask = 1 << 6 | 1 << 9; //pickups and checkpoints
        RaycastHit hit;
        if(debugRay) Debug.DrawRay(rayCastPosition.position,rayCastPosition.forward*collisionAvoidDistance,Color.red,0.1f);
        if(Physics.Raycast(rayCastPosition.position,rayCastPosition.forward,out hit, float.MaxValue,layerMask)) // we're heading for the pickup, don't turn
        {
            //Log($"Already aiming at {hit.collider.gameObject.name}");
            turn = 0;
            
        }
        //Log($"turning direction {turn} for next target at {hit.point} called {CurrentTarget.gameObject.name}");
        return turn;
    }
    // Update is called once per frame
    bool dodge = false;
    void Update()
    {
        IncrementTargetIndexIfMissedIt();
        //simpleCarController.SetInputDirection(new Vector2(1,1));
        var deltaPos = CurrentTarget.position - transform.position;
        float dot = Vector3.Dot(transform.forward,deltaPos);
        float angle = Vector3.SignedAngle(transform.forward,deltaPos,Vector3.up);
        
        float accel = 1;
        float turn = GetTurnForNextTarget();
        
        RaycastHit hit;
        int layerMask = 1 << 7 | 1 << 8; //obstacles layer and racers layer
        
        //if(debugRay) Debug.DrawRay(rayCastPosition.position, rayCastPosition.forward * collisionAvoidDistance, Color.red,0.1f);
        if(Physics.Raycast(rayCastPosition.position,rayCastPosition.forward, out hit, collisionAvoidDistance, layerMask)) //if there's something to avoid in front of us
        {
            
            float angleToDodge = GetCollisionAvoidanceAngle(rayCastPosition.forward,rayCastPosition,out hit,collisionAvoidDistance,layerMask,turnAvoidAngle);
            if(hit.collider!=null && angleToDodge==0) // cannot dodge
            {
                if(!reversing)
                {
                    Log("reversing to avoid " + hit.transform.gameObject.name,false);
                    StartCoroutine(Reverse(reverseTimeForAvoiding));
                }
            }
            else if(dodge)
            {
                if(angleToDodge > float.Epsilon)
                {
                    turn = 1;
                    Log($"Dodging right to avoid " + hit.transform.gameObject.name);
                }
                else if(angleToDodge < -float.Epsilon)
                {
                    turn = -1;
                    Log($"Dodging left to avoid " + hit.transform.gameObject.name);
                }
            }
        }
        else if(dodge) // check to left and right for incoming racers
        {
            
            if(Physics.Raycast(rayCastPosition.position,rayCastPosition.right,out hit, collisionAvoidHorizontalDistance,layerMask)) // avoid things to the right
            {
                turn = -1;
                Log($"Dodging left to avoid racer " + hit.transform.gameObject.name);
            }
            else if(Physics.Raycast(rayCastPosition.position,rayCastPosition.right * -1,out hit, collisionAvoidHorizontalDistance,layerMask)) // avoid things to the left
            {
                turn = 1;
                Log($"Dodging right to avoid racer " + hit.transform.gameObject.name);
            }
        }

        if(reversing) 
        {
            accel = -1;
            
            Log($"dot={dot} angle={angle} accel = {accel} turn = {turn} hit {hit.collider?.gameObject?.name}");
            
        }
        turn *= accel; //if we're going backwards need to reverse the steering

        simpleCarController.SetInputDirection(new Vector2(turn,accel));
        if(Time.time % 10 <1)
        {
        //Log($"dot={dot} angle={angle} accel = {accel} turn = {turn} hit {hit.collider?.gameObject?.name}");
        }
    }

    IEnumerator Reverse(float time)
    {
        Log($"Reversing for {time} seconds",false);
        reversing = true;
        yield return new WaitForSeconds(time);
        reversing = false;

    }

    public void TargetReached(Transform location)
    {
        if(CurrentTarget.gameObject == location.gameObject)
        {
            IncrementTarget();
            Log("Target reached, matched. Next is " + CurrentTarget.gameObject.name,false);
            
        }
        else
        {
            Log($"Target reached, not matched {CurrentTarget.gameObject.name}!={location.gameObject.name}",false);
        }
    }
}
