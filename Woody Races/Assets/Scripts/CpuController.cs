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
    [SerializeField] float collisionAvoidDistance = 5f;
    [SerializeField] float reverseTimeForAvoiding = 1f;
    
    [SerializeField] Transform rayCastPosition;
    [Header("debugging")]
    [SerializeField] bool debugRay;
    [SerializeField] bool debugLog;
    public bool reversing;

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

    }

    // Update is called once per frame
    void Update()
    {
        
        //simpleCarController.SetInputDirection(new Vector2(1,1));
        var deltaPos = trackTargets[targetIndex].position - transform.position;
        float dot = Vector3.Dot(transform.forward,deltaPos);
        float angle = Vector3.SignedAngle(transform.forward,deltaPos,Vector3.up);
        
        float accel = dot > 0 ? 1 : -1;
        float turn = angle > 0 ? 1 : -1;
        
        int layerMask = 1 << 6; // pickups layer
        RaycastHit hit;
        if(Physics.Raycast(rayCastPosition.position,rayCastPosition.forward,out hit, float.MaxValue,layerMask))
        {
            turn = 0;
            
        }

        layerMask = 1 << 7; //obstacles layer
        if(debugRay) Debug.DrawRay(rayCastPosition.position,rayCastPosition.forward*5,Color.red,0.1f);
        if(Physics.Raycast(rayCastPosition.position,rayCastPosition.forward,out hit, collisionAvoidDistance,layerMask))
        {
            if(!reversing)
            {
                if(debugLog) Debug.Log("reversing to avoid obstacle");
                StartCoroutine(Reverse(reverseTimeForAvoiding));
            }
        }
        else
        {
            layerMask = 1 << 6;
            layerMask = ~ layerMask; //not pickups layer
            if(Physics.Raycast(rayCastPosition.position,rayCastPosition.right,out hit, collisionAvoidDistance,layerMask)) // avoid things to the right
            {
                turn = -1;
            }
            else if(Physics.Raycast(rayCastPosition.position,rayCastPosition.right * -1,out hit, collisionAvoidDistance,layerMask)) // avoid things to the left
            {
                turn = 1;
            }
            else if(Physics.Raycast(rayCastPosition.position,rayCastPosition.right * -1,out hit, collisionAvoidDistance,layerMask)) // avoid things in front of us
            {
                deltaPos = hit.point - rayCastPosition.position;
                angle = Vector3.SignedAngle(transform.forward,deltaPos,Vector3.up);
                turn = angle < 0 ? 1 : -1;    // turn away from the hit point
            }
        }

        if(reversing) 
        {
            accel = -1;
            if(Time.time % 10 <1)
            {
            if(debugLog) Debug.Log($"dot={dot} angle={angle} accel = {accel} turn = {turn} hit {hit.collider?.gameObject?.name}");
            }
        }
        turn *= accel; //if we're going backwards need to reverse the steering

        simpleCarController.SetInputDirection(new Vector2(turn,accel));
        if(Time.time % 10 <1)
        {
        if(debugLog) Debug.Log($"dot={dot} angle={angle} accel = {accel} turn = {turn} hit {hit.collider?.gameObject?.name}");
        }
    }

    IEnumerator Reverse(float time)
    {
        if(debugLog) Debug.Log("Reversing");
        reversing = true;
        yield return new WaitForSeconds(time);
        reversing = false;

    }

    public void TargetReached(Transform location)
    {
        if(trackTargets[targetIndex].gameObject == location.gameObject)
        {
            targetIndex++;
            if(targetIndex >=trackTargets.Count) targetIndex = 0;
            if(debugLog) Debug.Log("Target reached, matched. Next is " + trackTargets[targetIndex].gameObject.name);
            
        }
        else
        {
            if(debugLog) Debug.Log($"Target reached, not matched {trackTargets[targetIndex].gameObject.name}!={location.gameObject.name}");
        }
    }
}
