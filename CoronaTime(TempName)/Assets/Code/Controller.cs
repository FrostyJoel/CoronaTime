using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {

    public Transform pov;
    [HideInInspector] public Rigidbody rigid;

    public bool hideCursorOnStart;
    [Space]
    public float walkSpeed = 5;
    public float mouseSensitivity = 100, cartRotationSpeed;

    public float leftLock;
    public float rightLock;
    [Range(0, 90)]
    public float viewAngle = 30;

    float xRotationAxisClamp, yRotationAxisClamp;

    private void Start() {
        if (!pov) {
            try {
                pov = GetComponentInChildren<Camera>().transform;
            } catch {
                pov = new GameObject("POV").transform;
                pov.gameObject.AddComponent<Camera>();
                pov.SetParent(transform);
                pov.localPosition = Vector3.zero;
            }
        }
        rigid = GetComponent<Rigidbody>();
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void FixedUpdate() {
        //body rotation
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(0, 0, vertical) * walkSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontal * cartRotationSpeed * Time.deltaTime);

        //camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotationAxisClamp += mouseY;
        yRotationAxisClamp += mouseX;

        if (xRotationAxisClamp > viewAngle) {
            xRotationAxisClamp = viewAngle;
            mouseY = 0f;
            ClampXRotationAxisRotationToValue(-viewAngle);
        } else if (xRotationAxisClamp < -viewAngle) {
            xRotationAxisClamp = -viewAngle;
            mouseY = 0f;
            ClampXRotationAxisRotationToValue(viewAngle);
        }

        //if (xRotationAxisClamp > -topLock) {
        //    xRotationAxisClamp = -topLock;
        //    mouseY = 0f;
        //    ClampXRotationAxisRotationToValue(topLock);
        //} else if (xRotationAxisClamp < -bottomLock) {
        //    xRotationAxisClamp = -bottomLock;
        //    mouseY = 0f;
        //    ClampXRotationAxisRotationToValue(bottomLock);
        //}
        print(Vector3.left);
        pov.Rotate(Vector3.left * mouseY);
        //pov.Rotate(Vector3.up * mouseX);
    }

    private void ClampXRotationAxisRotationToValue(float value) {
        Vector3 eulerRotation = pov.localEulerAngles;
        eulerRotation.x = value;
        pov.localEulerAngles = eulerRotation;
    }
    
    private void ClampYRotationAxisRotationToValue(float value) {
        Vector3 eulerRotation = pov.localEulerAngles;
        eulerRotation.y = value;
        pov.localEulerAngles = eulerRotation;
    }
}

