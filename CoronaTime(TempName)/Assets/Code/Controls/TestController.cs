using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum Axel {
    Front,
    Rear,
    FourWheelDrive
}

[Serializable]
public struct Wheel {
    public WheelCollider wheelCollider;
    public GameObject model;
    public Axel axel;
}

public class TestController : MonoBehaviour {

    [SerializeField]
    private float maxAcceleration = 50, turnSensitivity = 1, maxSteeringAngle = 20, brakeForce = 50;
    public List<Wheel> wheels = new List<Wheel>();
    Rigidbody rb;
    public bool inverseTurn;
    public float divide;
    public Axel poweredAxel, steeringAxel, breakAxel;
    public Controller controller;
    float inputX, inputY;

    [Header("Speed")]
    public float maxSpeed;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        inputY = Input.GetAxis("Vertical");
        inputX = Input.GetAxis("Horizontal");
        float turn = inputX * maxSteeringAngle;
        for (int i = 0; i < wheels.Count; i++) {
            if (wheels[i].axel == poweredAxel) {
                wheels[i].wheelCollider.motorTorque = inputY * maxAcceleration * 500 * Time.deltaTime;
            }

            if (wheels[i].axel == steeringAxel) {
                Steering(wheels[i], turn);
            } else if (inverseTurn) {
                Steering(wheels[i], -turn * divide);
            }
        }
    }

    private void LateUpdate() {
        for (int i = 0; i < wheels.Count; i++) {
            Vector3 position;
            Quaternion rotation;
            wheels[i].wheelCollider.GetWorldPose(out position, out rotation);
            wheels[i].model.transform.position = position;
            wheels[i].model.transform.rotation = rotation;
        }
    }

    void Steering(Wheel wheel, float turn) {
        wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, turn, turnSensitivity);
    }
}