using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OutlineCheck : MonoBehaviourPunCallbacks
{
    public RaycastHit hit;
    public Outline lastOutline;
    private Controller controller;
    private CartStorage storage;
    private PlayerView pView;
    private void Start()
    {
        controller = GetComponent<Controller>();
        storage = GetComponent<CartStorage>();
        pView = GetComponent<PlayerView>();
    }

    private void Update()
    {
        if (photonView.IsMine || pView.devView)
        {
            if (Physics.Raycast(controller.transform_Pov.position, controller.transform_Pov.forward, out hit, Mathf.Infinity))
            {
                Outline outlineObj = hit.transform.gameObject.GetComponent<Outline>();

                if (lastOutline != outlineObj && lastOutline != null)
                {
                    lastOutline.enabled = false;
                }
                DistanceCheck(outlineObj);
            }
        }
    }

    private void DistanceCheck(Outline outlineObj)
    {
        if (hit.distance < storage.interactRange) {
            if (outlineObj)
            {
                PowerUp pu = hit.transform.gameObject.GetComponent<PowerUp>();
                bool canShow = true;
                if (pu && pu.currentPlace != Interactable.Place.InShelve) {
                    canShow = false;
                }
                if (!outlineObj.enabled && canShow) {
                    lastOutline = outlineObj;
                    outlineObj.enabled = true;
                }
            }
        }
    }
}
