using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviourPun {
    public PlayerView playerView;
    public Transform transform_Pov, transform_PovHolder;
    public Text text_Nickname;
    public MeshRenderer[] meshRenderersToDisableLocally;
    public Transform localPlayerTarget;
    public GameObject localInGameHud;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    public bool hideCursorOnStart;
    [Header("Keyboard Movement")]
    public float walkSpeed = 0.1f;
    public float sprintMultiplier = 1.2f, sprintSpeedWindUpDivider, sprintSpeedWindDownDivider, sprintFov = 62,
        sprintFovWindUpDivider, sprintFovWindDownDivider, keyboardCartRotationSpeed = 1.5f;
    [Header("Mouse Movement")]
    public float mouseSensitivity = 1;
    public float camCartRotationSpeed = 1;
    [Range(0, 90)]
    public float maxVerticalViewAngle = 30, maxHorizontalViewAngle = 80;
    [Space] [Range(0, 90)]
    public float camInrangeForRotationDegree;
    public Vector3 centerOfMass;
    bool canMove;
    float xRotationAxisAngle, yRotationAxisAngle;

    Camera[] cams;
    AudioListener audioListeners;
    [Header("HideInInspector")]
    public float defaultFov, currentSprintValue, currentFovValue;

    private void Awake() {
        TurnCollidersOnOff(false);
        rigid = GetComponent<Rigidbody>();
        cams = GetComponentsInChildren<Camera>();
        defaultFov = cams[0].fieldOfView;
        for (int i = 0; i < cams.Length; i++) {
            cams[i].enabled = false;
        }
        SetCameraFOVS();
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;
        photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        if (photonView.IsMine || playerView.devView) {
            text_Nickname.gameObject.SetActive(false);
            if (meshRenderersToDisableLocally.Length > 0) {
                for (int i = 0; i < meshRenderersToDisableLocally.Length; i++) {
                    meshRenderersToDisableLocally[i].enabled = false;
                }
            }
        }
    }    

    private void Start() {
        if (!transform_Pov) {
            try {
                transform_Pov = GetComponentInChildren<Camera>().transform;
            } catch {
                transform_Pov = new GameObject("POV").transform;
                transform_Pov.gameObject.AddComponent<Camera>();
                transform_Pov.SetParent(transform);
                transform_Pov.localPosition = Vector3.zero;
            }
        }
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Init();
        rigid.centerOfMass = centerOfMass;
        canMove = true; Debug.LogWarning("(bool)canMove WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
    }

    [PunRPC]
    void RPC_SetMyNickname() {
        if (photonView.IsMine) {
            text_Nickname.text = PhotonLobbyCustomMatchMaking.lobbySingle.nickName;
        }
    }

    [PunRPC]
    void RPC_SetNicknameTargets() {
        if (photonView.IsMine) {
            Controller[] controllers = FindObjectsOfType<Controller>();
            for (int i = 0; i < controllers.Length; i++) {
                controllers[i].localPlayerTarget = transform_Pov;
                controllers[i].text_Nickname.text = PhotonRoomCustomMatchMaking.roomSingle.RemoveIdFromNickname(controllers[i].photonView.Owner.NickName);
            }
        }
    }

    public void Init() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (photonView.IsMine || playerView.devView) {
            for (int i = 0; i < cams.Length; i++) {
                cams[i].enabled = true;
            }
            audioListeners.enabled = true;
        }
        rigid.isKinematic = false;
        TurnCollidersOnOff(true);
    }

    void TurnCollidersOnOff(bool state) {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].enabled = state;
        }
    }

    private void FixedUpdate() {
        if ((canMove && photonView.IsMine) || playerView.devView) {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            float currentRotationSpeed = 0;

            xRotationAxisAngle += mouseY;
            yRotationAxisAngle += mouseX;

            if (xRotationAxisAngle > maxVerticalViewAngle) {
                xRotationAxisAngle = maxVerticalViewAngle;
                mouseY = 0f;
                ClampXRotationAxisToValue(transform_Pov, -maxVerticalViewAngle);
            } else if (xRotationAxisAngle < -maxVerticalViewAngle) {
                xRotationAxisAngle = -maxVerticalViewAngle;
                mouseY = 0f;
                ClampXRotationAxisToValue(transform_Pov, maxVerticalViewAngle);
            }

            if (yRotationAxisAngle > maxHorizontalViewAngle) {
                yRotationAxisAngle = maxHorizontalViewAngle;
                mouseX = 0f;
                ClampYRotationAxisToValue(transform_PovHolder, maxHorizontalViewAngle);
            } else if (yRotationAxisAngle < -maxHorizontalViewAngle) {
                yRotationAxisAngle = -maxHorizontalViewAngle;
                mouseX = 0f;
                ClampYRotationAxisToValue(transform_PovHolder, -maxHorizontalViewAngle);
            }

            if (yRotationAxisAngle > maxHorizontalViewAngle - camInrangeForRotationDegree) {
                currentRotationSpeed = camCartRotationSpeed;
            } else if (yRotationAxisAngle < -maxHorizontalViewAngle + camInrangeForRotationDegree) {
                currentRotationSpeed = -camCartRotationSpeed;
            }

            transform_Pov.Rotate(Vector3.left * mouseY);
            transform_PovHolder.Rotate(Vector3.up * mouseX);

            SprintCheck();
            float vertical = Input.GetAxis("Vertical") * walkSpeed * currentSprintValue;

            if (Input.GetAxis("Horizontal") != 0) {
                currentRotationSpeed = Input.GetAxis("Horizontal") * keyboardCartRotationSpeed;
            }

            transform.Translate(Vector3.forward * vertical);
            Vector3 newPos = transform.position + transform.forward * vertical;
            transform.Rotate(Vector3.up * currentRotationSpeed);
            Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles + transform.up * currentRotationSpeed);
        }
        if (localPlayerTarget) {
            text_Nickname.transform.LookAt(localPlayerTarget);
            text_Nickname.transform.Rotate(0, 180, 0);
        }
    }

    void SetCameraFOVS() {
        for (int i = 0; i < cams.Length; i++) {
            cams[i].fieldOfView = currentFovValue;
        }
    }

    void SprintCheck() {
        if (Input.GetButton("Sprint") && Input.GetAxis("Vertical") > 0) {
            //speed
            if(currentSprintValue < sprintMultiplier) {
                currentSprintValue = currentSprintValue + sprintMultiplier / sprintSpeedWindUpDivider;
            }
            if(currentSprintValue > sprintMultiplier) {
                currentSprintValue = sprintMultiplier;
            }

            //camera FOV
            if(currentFovValue != sprintFov) {
                if (currentFovValue < sprintFov) {
                    currentFovValue = currentFovValue + sprintFov / sprintFovWindUpDivider;
                } else {
                    currentFovValue = sprintFov;
                }
                SetCameraFOVS();
            }

        } else {
            //speed
            if(Input.GetAxis("Vertical") >= 0) {
                if (currentSprintValue > 1) {
                    currentSprintValue = currentSprintValue - sprintMultiplier / sprintSpeedWindDownDivider;
                }
                if (currentSprintValue < 1) {
                    currentSprintValue = 1;
                }
            } else {
                currentSprintValue = 1;
            }

            //camera FOV
            if(currentFovValue != defaultFov) {
                if (currentFovValue > defaultFov) {
                    currentFovValue = currentFovValue - sprintFov / sprintFovWindDownDivider;
                } else {
                    currentFovValue = defaultFov;
                }
                SetCameraFOVS();
            }
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
        transform_Pov.transform.localRotation = Quaternion.identity;
        transform_PovHolder.transform.localRotation = Quaternion.identity;
        xRotationAxisAngle = 0;
        yRotationAxisAngle = 0;
        rigid.velocity = Vector3.zero;
    }
}