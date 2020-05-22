using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))][RequireComponent(typeof(CartStorage))]
public class Controller : MonoBehaviourPun {
    public PlayerView playerView;
    public Transform transform_Pov, transform_PovHolder, transform_Head;
    public Text text_Nickname;
    public MeshRenderer[] meshRenderersToDisableLocally;
    
    [Space]
    public GameObject localInGameHud;
    public bool hideCursorOnStart;

    [Header("Keyboard Movement")]
    public float defaultWalkSpeed;
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
    float xRotationAxisAngle, yRotationAxisAngle;

    [Header("HideInInspector")]
    [HideInInspector] public bool canMove;
    [HideInInspector] public Transform localPlayerTarget;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public CartStorage cartStorage;
    /*[HideInInspector]*/ public float currentWalkSpeed;
    /*HideInInspector]*/ public UseableProduct useableProduct;
    public List<PowerUp> powerups_AffectingMe = new List<PowerUp>();

    Camera[] cams;
    AudioListener audioListeners;
    float defaultFov, currentSprintValue, currentFovValue;

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
        if (photonView.IsMine || playerView.devView) {
            text_Nickname.gameObject.SetActive(false);
            if (meshRenderersToDisableLocally.Length > 0) {
                for (int i = 0; i < meshRenderersToDisableLocally.Length; i++) {
                    meshRenderersToDisableLocally[i].enabled = false;
                }
            }
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);

            if (photonView.Owner.IsMasterClient) {
                if (FindObjectOfType<ZoneControl>()) {
                    ZoneControl.zcSingle.photonView.RPC("RandomizeOrder", RpcTarget.MasterClient);
                }
            }
        }
        cartStorage = GetComponent<CartStorage>();
    }

    private void Start() {
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Init();
        rigid.centerOfMass = centerOfMass;
        canMove = true; Debug.LogWarning("(bool)canMove WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
    }

    private void Update() {
        CheckAndApplyBuffs();
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

    void CheckAndApplyBuffs() {
        currentWalkSpeed = defaultWalkSpeed;
        if (powerups_AffectingMe.Count > 0) {
            for (int i = 0; i < powerups_AffectingMe.Count; i++) {
                powerups_AffectingMe[i].Effect();
            }
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
            Vector3 headRot = new Vector3(0, transform_Pov.rotation.eulerAngles.y, 0);
            transform_Head.rotation = Quaternion.Euler(headRot);
            transform_PovHolder.Rotate(Vector3.up * mouseX);

            SprintCheck();
            float vertical = Input.GetAxis("Vertical") * currentWalkSpeed * currentSprintValue;

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
}