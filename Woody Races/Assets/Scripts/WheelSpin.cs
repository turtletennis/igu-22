using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    WheelCollider wheelCollider;
    public Transform wheelModel;

    private void Start() {
        wheelCollider = GetComponent<WheelCollider>();
    }
    Quaternion previousRotation;
    void Update()
    {
        Vector3 wheelPosition;
        Quaternion wheelRotation;
        wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        float angle;
        Vector3 axis;
        wheelRotation.ToAngleAxis(out angle, out axis);
        float wheelSpinAngle = Vector3.Dot(axis,transform.right);
        previousRotation = wheelRotation;
        //wheelModel.position = wheelPosition;
        //wheelModel.Rotate(transform.right,wheelSpinAngle);
        wheelModel.rotation =  wheelRotation;

    }
}
