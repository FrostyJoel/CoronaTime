using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //[RequireComponent(typeof(PlayerviewCheck))]
public class Controller : MonoBehaviour {

    public Transform pov, povHolder;
    [HideInInspector] public Rigidbody rigid;
    //[HideInInspector] public PlayerviewCheck playerviewCheck;
    public Vector3 startPosition;
    public Quaternion startRotation;
    public bool hideCursorOnStart, useTestController;
    [Space]
    public float walkSpeed = 5;
    public float mouseSensitivity = 100, keyboardCartRotationSpeed = 100, camCartRotationSpeed;

    public enum ClampType {
        Rectangular,
        Circular
    }
    [Space]
    public ClampType clampType;
    [Range(0, 90)]
    public float maxVerticalViewAngle = 30, maxHorizontalViewAngle = 80;
    [Space] [Range(0, 90)]
    public float camInrangeForRotationDegree;
    public Vector3 centerOfMass;

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
        //playerviewCheck = GetComponent<PlayerviewCheck>();
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        startPosition = transform.position;
        startRotation = transform.rotation;
    }


    //float Cos(float )

    //180 - 90 
    //A + B = C 
    private void Update() {
        rigid.centerOfMass = centerOfMass;
    }

    private void FixedUpdate() {
        //if (playerviewCheck.photonView.isMine || playerviewCheck.devTesting) {
            //camera rotation
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            float currentRotationSpeed = 0;

            if (clampType == ClampType.Rectangular) {
                xRotationAxisAngle += mouseY;
                yRotationAxisAngle += mouseX;

                if (xRotationAxisAngle > maxVerticalViewAngle) {
                    xRotationAxisAngle = maxVerticalViewAngle;
                    mouseY = 0f;
                    ClampXRotationAxisToValue(pov, -maxVerticalViewAngle);
                } else if (xRotationAxisAngle < -maxVerticalViewAngle) {
                    xRotationAxisAngle = -maxVerticalViewAngle;
                    mouseY = 0f;
                    ClampXRotationAxisToValue(pov, maxVerticalViewAngle);
                }

                if (yRotationAxisAngle > maxHorizontalViewAngle) {
                    yRotationAxisAngle = maxHorizontalViewAngle;
                    mouseX = 0f;
                    ClampYRotationAxisToValue(povHolder, maxHorizontalViewAngle);
                } else if (yRotationAxisAngle < -maxHorizontalViewAngle) {
                    yRotationAxisAngle = -maxHorizontalViewAngle;
                    mouseX = 0f;
                    ClampYRotationAxisToValue(povHolder, -maxHorizontalViewAngle);
                }

                if(yRotationAxisAngle > maxHorizontalViewAngle - camInrangeForRotationDegree) {
                    currentRotationSpeed = camCartRotationSpeed;
                } else if (yRotationAxisAngle < -maxHorizontalViewAngle + camInrangeForRotationDegree) {
                    currentRotationSpeed = -camCartRotationSpeed;
                }

                pov.Rotate(Vector3.left * mouseY);
                povHolder.Rotate(Vector3.up * mouseX);
            } else {

                xRotationAxisAngle += mouseY;
                yRotationAxisAngle += mouseX;

                //if (xRotationAxisAngle > maxVerticalViewAngle) {
                //    xRotationAxisAngle = maxVerticalViewAngle;
                //    mouseY = 0f;
                //    ClampXRotationAxisToValue(pov, -maxVerticalViewAngle);
                //} else if (xRotationAxisAngle < -maxVerticalViewAngle) {
                //    xRotationAxisAngle = -maxVerticalViewAngle;
                //    mouseY = 0f;
                //    ClampXRotationAxisToValue(pov, maxVerticalViewAngle);
                //}

                float alpha = Mathf.Atan(xRotationAxisAngle / yRotationAxisAngle) * Mathf.Rad2Deg;
                float test1 = maxVerticalViewAngle * Mathf.Cos(alpha) - maxVerticalViewAngle * (Mathf.Sin(alpha) * Mathf.Cos(alpha)) + maxHorizontalViewAngle * (Mathf.Sin(alpha) * Mathf.Cos(alpha));
                float test2 = maxVerticalViewAngle * Mathf.Sin(alpha) - maxVerticalViewAngle * (Mathf.Sin(alpha) * Mathf.Sin(alpha)) + maxHorizontalViewAngle * (Mathf.Sin(alpha) * Mathf.Sin(alpha));

                print(alpha + " " + test2);
                if (xRotationAxisAngle > test2) {
                    xRotationAxisAngle = test2;
                    mouseY = 0f;
                    ClampXRotationAxisToValue(pov, test2);
                } else if (xRotationAxisAngle < -test2) {
                    xRotationAxisAngle = -test2;
                    mouseY = 0f;
                    ClampXRotationAxisToValue(pov, -test2);
                }

                if (yRotationAxisAngle > maxHorizontalViewAngle) {
                    yRotationAxisAngle = maxHorizontalViewAngle;
                    mouseX = 0f;
                    ClampYRotationAxisToValue(povHolder, maxHorizontalViewAngle);
                } else if (yRotationAxisAngle < -maxHorizontalViewAngle) {
                    yRotationAxisAngle = -maxHorizontalViewAngle;
                    mouseX = 0f;
                    ClampYRotationAxisToValue(povHolder, -maxHorizontalViewAngle);
                }

                pov.Rotate(Vector3.left * mouseY);
                povHolder.Rotate(Vector3.up * mouseX);
            }

            //body rotation
            if (!useTestController) {
                float vertical = Input.GetAxis("Vertical") * walkSpeed;
                if (Input.GetAxis("Horizontal") != 0) {
                    currentRotationSpeed = Input.GetAxis("Horizontal") * keyboardCartRotationSpeed;
                }

                transform.Translate(Vector3.forward * vertical);
                Vector3 newPos = transform.position + transform.forward * vertical;
                //rigid.MovePosition(newPos);
                transform.Rotate(Vector3.up * currentRotationSpeed);
                Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles + transform.up * currentRotationSpeed);
                //rigid.MoveRotation(rot);
            }
        //}
    }

    private void ClampXRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.x = value;
        transform_.localEulerAngles = eulerRotation;
    }

    private void ClampYRotationAxisToValue(Transform transform_, float value) {
        Vector3 eulerRotation = transform_.localEulerAngles;
        eulerRotation.y = value;
        transform_.localEulerAngles = eulerRotation;
    }

    public void ResetAtStartPosition() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        pov.transform.localRotation = Quaternion.identity;
        povHolder.transform.localRotation = Quaternion.identity;
        xRotationAxisAngle = 0;
        yRotationAxisAngle = 0;
        rigid.velocity = Vector3.zero;
    }
}

