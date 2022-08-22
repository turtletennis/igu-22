using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float acceleration = 5;
    [SerializeField] float maxSpeed = 25;
    [SerializeField] float turnPower = 50;
    [SerializeField] float steeringDeadZone = 0.25f;
    [SerializeField] float directionCheckDelay = 0.5f;
    SimpleCarController simpleCarController;
    float directionLastCheckedAt;
    float currentBearing;

    Vector3 trackCentre;

    float speed;
    Vector2 inputDirection;
    new Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        simpleCarController = GetComponent<SimpleCarController>();
        CheckpointManager checkpointManager = FindObjectOfType<CheckpointManager>();
        trackCentre = checkpointManager.transform.position;
        UpdateBearing();
    }

    private void UpdateBearing()
    {
        var deltaPos = transform.position - trackCentre;
        float newBearing = Mathf.Atan2(deltaPos.z,deltaPos.x) * 180f / Mathf.PI + 180;
        //Debug.Log(currentBearing);
        float diff = newBearing - currentBearing;
        if(diff < -180) diff+=360;
        if(diff < 0)
        {
            //Debug.Log("Going the wrong way!");
        }
        currentBearing = newBearing;
        directionLastCheckedAt = Time.time;
    }



    private void Update() {
        //Debug.Log(rigidbody.velocity);
        if(Time.time > directionLastCheckedAt + directionCheckDelay)
        {
            UpdateBearing();
        }
    }

    void OnMove(InputValue value)
    {
        simpleCarController.SetInputDirection(value.Get<Vector2>());
        //Debug.Log(inputDirection);
    }
}
