using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour {

    public Transform pov, povHolder;
    [HideInInspector] public Rigidbody rigid;

    public bool hideCursorOnStart, useTestController;
    [Space]
    public float walkSpeed = 5;
    public float mouseSensitivity = 100, cartRotationSpeed;

    public enum ClampType {
        Rectangular,
        Circular
    }
    [Space]
    public ClampType clampType;
    [Range(0, 90)]
    public float maxVerticalViewAngle = 30, maxHorizontalViewAngle = 80;
    public Vector3 center;

    public float xRotationAxisAngle, yRotationAxisAngle;

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

    private void Update() {
        rigid.centerOfMass = center;
    }

    private void FixedUpdate() {
        //body rotation
        if (!useTestController) {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            transform.Translate(new Vector3(0, 0, vertical) * walkSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up * horizontal * cartRotationSpeed * Time.deltaTime);
        }

        //camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (clampType == ClampType.Rectangular) {
            xRotationAxisAngle += mouseY;
            yRotationAxisAngle += mouseX;

            if (xRotationAxisAngle > maxVerticalViewAngle) {
                xRotationAxisAngle = maxVerticalViewAngle;
                mouseY = 0f;
                ClampRotationAxisRotationToValue(pov, -maxVerticalViewAngle);
            } else if (xRotationAxisAngle < -maxVerticalViewAngle) {
                xRotationAxisAngle = -maxVerticalViewAngle;
                mouseY = 0f;
                ClampRotationAxisRotationToValue(pov, maxVerticalViewAngle);
            }

            if (yRotationAxisAngle > maxHorizontalViewAngle) {
                yRotationAxisAngle = maxHorizontalViewAngle;
                mouseX = 0f;
                ClampRotationAxisRotationToValue(povHolder, maxHorizontalViewAngle);
            } else if (yRotationAxisAngle < -maxHorizontalViewAngle) {
                yRotationAxisAngle = -maxHorizontalViewAngle;
                mouseX = 0f;
                ClampRotationAxisRotationToValue(povHolder, -maxHorizontalViewAngle);
            }
            pov.Rotate(Vector3.left * mouseY);
            povHolder.Rotate(Vector3.up * mouseX);
        }
    }

    private void ClampRotationAxisRotationToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.x = value;
        transform_.localEulerAngles = eulerRotation;
    }
}

