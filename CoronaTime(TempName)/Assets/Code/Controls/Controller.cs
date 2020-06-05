using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CartStorage))]
public class Controller : MonoBehaviourPun {
    public PlayerView playerView;
    public Transform transform_Pov, transform_PovHolder, transform_Head, transform_ThrowFromPoint;
    public Text text_Nickname;
    public MeshRenderer[] meshRenderersToDisableLocally;
    
    [Space]
    public GameObject localInGameHud;
    public bool hideCursorOnStart, keepLocalMeshesEnabled;

    [Header("Keyboard Movement")]
    public float defaultWalkSpeed;
    public float sprintMultiplier = 1.2f, sprintSpeedWindUpDivider, sprintSpeedWindDownDivider, sprintFov = 62,
        sprintFovWindUpDivider, sprintFovWindDownDivider, keyboardCartRotationSpeed = 1.5f;

    [Header("Mouse Movement")]
    public float mouseSensitivity = 1;
    public float camCartRotationSpeed = 1;

    [Range(0, 90)]
    public float maxVerticalViewAngle = 30, maxHorizontalViewAngle = 80;

    [Space][Range(0, 90)]
    public float camInrangeForRotationDegree;

    [Header("Particles")]
    public VisualFX[] particles;

    [HideInInspector] public bool canMove;
    [HideInInspector] public float currentWalkSpeed;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Quaternion startRotation;
    [HideInInspector] public Outline myOutline;
    [HideInInspector] public PowerUp useableProduct;
    [HideInInspector] public CartStorage cartStorage;
    [HideInInspector] public Transform localPlayerTarget;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public List<PowerUp> powerups_AffectingMe = new List<PowerUp>();
    Camera[] cams;
    AudioListener audioListeners;
    float defaultFov, currentSprintValue, currentFovValue, xRotationAxisAngle, yRotationAxisAngle;

    private void Awake() {
        TurnCollidersOnOff(false);
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
            if (meshRenderersToDisableLocally.Length > 0 && !keepLocalMeshesEnabled) {
                for (int i = 0; i < meshRenderersToDisableLocally.Length; i++) {
                    meshRenderersToDisableLocally[i].enabled = false;
                }
            }
            if (GameObject.Find("GameManager")) {
                gameObject.layer = Manager.staticInformation.int_LocalPlayerLayer;
            }
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("RPC_SetNicknameTargets", RpcTarget.All);
        }
        cartStorage = GetComponent<CartStorage>();
        myOutline = GetComponentInChildren<Outline>();
        myOutline.enabled = false;
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
    }

    private void Start() {
        if (photonView.IsMine && Spawnpoints.sp_Single && PhotonRoomCustomMatchMaking.roomSingle) {
            Vector3[] posAndRot = Spawnpoints.sp_Single.GetSpPositionAndRotation(PhotonRoomCustomMatchMaking.roomSingle.myNumberInRoom -1);
            transform.position = posAndRot[0];
            transform.rotation = Quaternion.Euler(posAndRot[1]);
        }
        TurnCollidersOnOff(true);
        if (hideCursorOnStart) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Init();
        canMove = true; 
        Debug.LogWarning("(bool)canMove WAS ACCESSED BY A DEV FUNCTION, CHANGE TO ALTERNATIVE WHEN READY");
    }

    private void Update() {
        if (photonView.IsMine || playerView.devView) {
            CheckAndApplyPowerUpFX();
            if (Input.GetButtonDown("UsePowerUp")) {
                if (useableProduct) {
                    useableProduct.Use();
                }
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
            Quaternion headRot = Quaternion.Euler(new Vector3(0, transform_Pov.rotation.eulerAngles.y, 0));
            transform_Head.rotation = headRot;
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

        if (Input.GetButtonDown("UsePowerUp")) {
            if (useableProduct) {
                useableProduct.Use();
            }
        }
    }

    void TurnCollidersOnOff(bool state) {
        colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].enabled = state;
        }
    }

    public void SetAffectingFX(PowerUp pu) {
        int blockIndex = CheckForBlockFX();
        if (blockIndex < 0 || pu.GetType() == typeof(BlockFX)) {
            if (powerups_AffectingMe.Count > 0) {
                Debug.LogWarning(">");
                if (!ContainsPuAt(pu)) {
                    powerups_AffectingMe.Add(pu);
                    Debug.LogWarning("add pu");
                }
            } else {
                Debug.LogWarning("else add pu");
                powerups_AffectingMe.Add(pu);
            }
        } else if (blockIndex >= 0) {
            ProductInteractions.pi_Single.DestroyUseAbleProduct(pu.index, 0, RpcTarget.All);
        }
    }

    bool ContainsPuAt(PowerUp pu) {
        bool contains = false;
        for (int i = 0; i < powerups_AffectingMe.Count; i++) {
            if(powerups_AffectingMe[i].GetType() == pu.GetType()) {
                powerups_AffectingMe[i].durationSpentInSeconds = 0f;
                ProductInteractions.pi_Single.DestroyUseAbleProduct(pu.index, 0, RpcTarget.All);
                contains = true;
                break;
            }
        }
        return contains;
    }

    int CheckForBlockFX() {
        int blockAt = -1;
        for (int i = 0; i < powerups_AffectingMe.Count; i++) {
            if(powerups_AffectingMe[i].GetType() == typeof(BlockFX)) {
                powerups_AffectingMe[i].StopUsing();
                blockAt = i;
                break;
            }
        }
        return blockAt;
    }

    void CheckAndApplyPowerUpFX() {
        currentWalkSpeed = defaultWalkSpeed;
        if (powerups_AffectingMe.Count > 0) {
            for (int i = 0; i < powerups_AffectingMe.Count; i++) {
                powerups_AffectingMe[i].UseEffect();
            }
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