using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviourPun {
    public PlayerView playerView;
    public Transform pov, povHolder;
    public Text text_Nickname;
    public Transform localPlayerTarget;
    public GameObject localInGameHud;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    public bool hideCursorOnStart;
    [Space]
    public float walkSpeed = 5;
    public float mouseSensitivity = 100, keyboardCartRotationSpeed = 100, camCartRotationSpeed;

    [Range(0, 90)]
    public float maxVerticalViewAngle = 30, maxHorizontalViewAngle = 80;
    [Space] [Range(0, 90)]
    public float camInrangeForRotationDegree;
    public Vector3 centerOfMass;
    bool canMove;
    public float xRotationAxisAngle, yRotationAxisAngle;

    Camera[] cams;
    AudioListener audioListeners;

    private void Awake() {
        TurnCollidersOnOff(false);
        rigid = GetComponent<Rigidbody>();
        cams = GetComponentsInChildren<Camera>();
        for (int i = 0; i < cams.Length; i++) {
            cams[i].enabled = false;
        }
        audioListeners = GetComponentInChildren<AudioListener>();
        audioListeners.enabled = false;
        photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        if (photonView.IsMine) {
            text_Nickname.gameObject.SetActive(false);
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
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Init();
        canMove = true; Debug.LogWarning("(bool)canMove WAS ACCESSED BY A DEV FUNCTION, TAKE OUT FOR RELEASE");
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
                controllers[i].localPlayerTarget = pov;
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

    private void Update() {
        rigid.centerOfMass = centerOfMass;
    }

    private void FixedUpdate() {
        if ((canMove && photonView.IsMine) || playerView.devView) {
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
        if (localPlayerTarget) {
            text_Nickname.transform.LookAt(localPlayerTarget);
            text_Nickname.transform.Rotate(0, 180, 0);
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