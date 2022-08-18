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

    

    float speed;
    Vector2 inputDirection;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        //Debug.Log(rigidbody.velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move();
    }

    void Move()
    {
        //rigidbody.MoveRotation(Quaternion.Euler(0,turnPower * inputDirection.x * Time.deltaTime,0));
        if(inputDirection.x > steeringDeadZone || inputDirection.x < -steeringDeadZone)
        {
            transform.rotation *= Quaternion.AngleAxis(turnPower * inputDirection.x * Time.deltaTime,transform.up);
            
            var up = transform.up;
            rigidbody.velocity = Quaternion.Euler(up.x,up.y,up.z) * rigidbody.velocity;

        }
        
        Vector3 newV = transform.forward * acceleration * Time.deltaTime * inputDirection.y;
        rigidbody.velocity += newV;
        
    }

    void MoveKinematic()
    {
        
    }

    void MoveWithForces()
    {
        if(inputDirection.x > steeringDeadZone || inputDirection.x < -steeringDeadZone)
        {
            rigidbody.AddTorque(transform.up * turnPower * inputDirection.x);
        }
        //+ve acceleration and < max speed or -ve acceleration and > -maxSpeed
        if(rigidbody.velocity.magnitude * Mathf.Sign(inputDirection.y) < maxSpeed )
        {
            rigidbody.AddForce(transform.forward * acceleration * inputDirection.y);
        }
    }

    void OnMove(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
        //Debug.Log(inputDirection);
    }
}
