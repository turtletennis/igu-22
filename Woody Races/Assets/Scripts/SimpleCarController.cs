using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour {
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    Vector2 inputDirection;
    [SerializeField] float startingFuel = 10f;
    [SerializeField] float maxFuel = 10;
    [SerializeField] float fuelWarningLevel = 2;
    [SerializeField] float fuelDrainRate = 0.5f;
    [SerializeField] float minimumSpeedFactor = 0.05f;
    public float fuelRemaining;
    HudManager hudManager;
        
    private void Start() {
        fuelRemaining = startingFuel;
        hudManager = FindObjectOfType<HudManager>();

    }

    public void SetInputDirection(Vector2 input)
    {
        inputDirection = input;
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * inputDirection.y;
        if(fuelRemaining < fuelWarningLevel)
        {
            motor *= fuelRemaining / fuelWarningLevel;
            if(motor < minimumSpeedFactor)
            {
                motor = minimumSpeedFactor;
            }
        }
        fuelRemaining -= Mathf.Abs(motor) * fuelDrainRate / maxMotorTorque;
        UpdateHud();
        float steering = maxSteeringAngle * inputDirection.x;
            
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }
    // void OnMove(InputValue value)
    // {
    //     inputDirection = value.Get<Vector2>();
    //     //Debug.Log(inputDirection);
    // }

    public void AddFuel(float value)
    {
        fuelRemaining += value;
        if(fuelRemaining > maxFuel)
        {
            fuelRemaining = maxFuel;
        }
        UpdateHud();
    }

    void UpdateHud()
    {
        hudManager.UpdateFuelRemaining(fuelRemaining/maxFuel);
    }

}


    

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}