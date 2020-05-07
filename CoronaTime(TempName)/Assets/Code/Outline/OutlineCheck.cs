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
            if (Physics.Raycast(controller.pov.position, controller.pov.forward, out hit, Mathf.Infinity))
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
        if (hit.distance < storage.interactRange)
        {
            if (outlineObj)
            {
                if (!outlineObj.enabled)
                {
                    lastOutline = outlineObj;
                    outlineObj.enabled = true;
                }
            }
        }
    }
}
