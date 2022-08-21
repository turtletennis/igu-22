using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuController : MonoBehaviour
{
    SimpleCarController simpleCarController;
    public List<Transform> trackTargets;
    public int targetIndex;

    void Start()
    {
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
        turn *= accel; //if we're going backwards need to reverse the steering
        int layerMask = 1 << 6; // pickups layer
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit, float.MaxValue,layerMask))
        {
            turn = 0;
            
        }
        simpleCarController.SetInputDirection(new Vector2(turn,accel));
        Debug.Log($"dot={dot} angle={angle} accel = {accel} turn = {turn} hit {hit.collider?.gameObject?.name}");
    }

    public void PickupCollected(Transform location)
    {
        if(trackTargets[targetIndex].gameObject == location.gameObject)
        {
            targetIndex++;
            if(targetIndex >=trackTargets.Count) targetIndex = 0;
            Debug.Log("Pickup collected, matched. Next is " + trackTargets[targetIndex].gameObject.name);
            
        }
        else
        {
            Debug.Log($"Pickup collected, not matched {trackTargets[targetIndex].gameObject.name}!={location.gameObject.name}");
        }
    }
}
