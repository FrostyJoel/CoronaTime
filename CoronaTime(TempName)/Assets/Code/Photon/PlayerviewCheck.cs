using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerviewCheck : Photon.MonoBehaviour
{
    public PhotonView photonview;
    public GameObject plCam;
    public int smoothness = 8;
    public bool devTesting = false;
    public Text plNameText;
    private Vector3 selfPos;
    private GameObject sceneCam;

    private void Awake()
    {
        if(!devTesting && photonview.isMine)
        {
            sceneCam = GameObject.Find("Main Camera");
            sceneCam.SetActive(false);
            plCam.SetActive(true);
        }
    }

    private void Update()
    {
        if (devTesting)
        {
            
            return;
        }
        if (photonview.isMine)
        {
            
        }
        else
        {
            SmoothNetMovement();
        }
    }

    private void SmoothNetMovement()
    {
        transform.position = Vector3.Lerp(transform.position, selfPos, Time.deltaTime * smoothness);
    }

    private void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }

}
