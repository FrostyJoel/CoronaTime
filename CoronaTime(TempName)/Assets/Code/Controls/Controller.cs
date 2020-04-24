using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviourPun {

    public PlayerView playerView;
    public Transform pov, povHolder;
    [HideInInspector] public Rigidbody rigid;
    public Vector3 startPosition;
    public Quaternion startRotation;
    public bool hideCursorOnStart;
    [Space]
    public float walkSpeed = 5;
    public float mouseSensitivity = 100, keyboardCartRotationSpeed = 100, camCartRotationSpeed;
    ColorPicker colorPicker;

    [Range(0, 90)]
    public float maxVerticalViewAngle = 30, maxHorizontalViewAngle = 80;
    [Space] [Range(0, 90)]
    public float camInrangeForRotationDegree;
    public Vector3 centerOfMass;

    public float xRotationAxisAngle, yRotationAxisAngle;

    Camera[] cams;
    AudioListener audioListeners;

    private void Awake() {
        cams = GetComponentsInChildren<Camera>();
        for (int i = 0; i < cams.Length; i++) {
            cams[i].enabled = false;
        }
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;
        colorPicker = GetComponent<ColorPicker>();
        if (playerView.devView) {
            FindObjectOfType<PlayerSpawner>().enabled = false;
        }
    }    

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
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (photonView.IsMine || playerView.devView) {
            for (int i = 0; i < cams.Length; i++) {
                cams[i].enabled = true;
            }
            audioListeners.enabled = true;
        }
    }

    private void Update() {
        rigid.centerOfMass = centerOfMass;
    }

    private void FixedUpdate() {
        if (colorPicker.pickedAColor && (photonView.IsMine || playerView.devView)) {
            //camera rotation
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            float currentRotationSpeed = 0;

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

            if (yRotationAxisAngle > maxHorizontalViewAngle - camInrangeForRotationDegree) {
                currentRotationSpeed = camCartRotationSpeed;
            } else if (yRotationAxisAngle < -maxHorizontalViewAngle + camInrangeForRotationDegree) {
                currentRotationSpeed = -camCartRotationSpeed;
            }

            pov.Rotate(Vector3.left * mouseY);
            povHolder.Rotate(Vector3.up * mouseX);

            //body rotation
            float vertical = Input.GetAxis("Vertical") * walkSpeed;
            if (Input.GetAxis("Horizontal") != 0) {
                currentRotationSpeed = Input.GetAxis("Horizontal") * keyboardCartRotationSpeed;
            }

            transform.Translate(Vector3.forward * vertical);
            Vector3 newPos = transform.position + transform.forward * vertical;
            transform.Rotate(Vector3.up * currentRotationSpeed);
            Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles + transform.up * currentRotationSpeed);
        }
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