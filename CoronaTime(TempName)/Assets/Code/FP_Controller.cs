using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FP_Controller : MonoBehaviour {

    public Transform pov;
    [HideInInspector] public Rigidbody rigid;

    public bool hideCursorOnStart;
    [Space]
    public float walkSpeed = 5;
    public float mouseSensitivity = 100;

    public float topLock = 270;
    public float bottomLock = 90;

    float xAxisClamp;

    public float jumpMultiplier;
    public int maxJumps; //0 = infinite jumps
    int currentJumps;

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
        if (Input.GetButtonDown("Jump")) {
            Jump();
        }
    }

    private void FixedUpdate() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontal, 0, vertical) * walkSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;
        
        transform.Rotate(Vector3.up * mouseX);

        if (pov) {
            if (xAxisClamp > bottomLock) {
                xAxisClamp = bottomLock;
                mouseY = 0f;
                ClampXAxisRotationToValue(topLock);
            } else if (xAxisClamp < -bottomLock) {
                xAxisClamp = -bottomLock;
                mouseY = 0f;
                ClampXAxisRotationToValue(bottomLock);
            }
            pov.Rotate(Vector3.left * mouseY);
        }
    }

    private void ClampXAxisRotationToValue(float value) {
        Vector3 eulerRotation = pov.localEulerAngles;
        eulerRotation.x = value;
        pov.localEulerAngles = eulerRotation;
    }

    void Jump() {
        if (currentJumps < maxJumps || maxJumps == 0) {
            rigid.velocity = transform.up * jumpMultiplier;
            currentJumps++;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        currentJumps = 0;
    }
}

