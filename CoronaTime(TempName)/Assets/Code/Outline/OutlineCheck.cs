using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class OutlineCheck : MonoBehaviourPunCallbacks
{
    public RaycastHit hit;
    public Outline lastOutline;
    private Controller controller;
    private CartStorage storage;
    private PlayerView pView;
    public Text text_ProductName;

    private void Start()
    {
        controller = GetComponent<Controller>();
        storage = GetComponent<CartStorage>();
        pView = GetComponent<PlayerView>();
        text_ProductName.text = "";
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine || pView.devView)
        {
            if (Physics.Raycast(controller.transform_Pov.position, controller.transform_Pov.forward, out hit, Mathf.Infinity, storage.mask))
            {
                Outline outlineObj = hit.transform.gameObject.GetComponent<Outline>();
                SpecialLookAtTxt slat = hit.transform.gameObject.GetComponent<SpecialLookAtTxt>();

                if (lastOutline != outlineObj && lastOutline != null)
                {
                    lastOutline.enabled = false;
                }
                DistanceCheck(outlineObj, slat);
            }
        }
    }

    private void DistanceCheck(Outline outlineObj, SpecialLookAtTxt slat)
    {
        text_ProductName.text = "";
        if (hit.distance < storage.interactRange) {
            if (outlineObj)
            {
                PowerUp pu = hit.transform.gameObject.GetComponent<PowerUp>();
                bool canShow = true;
                if (pu && pu.currentPlace != Interactable.Place.InShelve && !controller.useableProduct && !pu.interactable) {
                    canShow = false;
                }
                InteractableProduct ip = hit.transform.gameObject.GetComponent<InteractableProduct>();
                if (ip) {
                    text_ProductName.text = ip.scriptableProduct.productName;
                    if (!ip.interactable) {
                        canShow = false;
                    }
                }
                if (!outlineObj.enabled && canShow) {
                    lastOutline = outlineObj;
                    outlineObj.enabled = true;
                }
            }
            if (slat) {
                text_ProductName.text = slat.hoverText;
            }
            if (storage.productsNeededInCurrentList == storage.productsGotten) {
                InteractableCashRegister icr = hit.transform.GetComponent<InteractableCashRegister>();
                if (icr) {
                    text_ProductName.text = icr.hoverText;
                }
            }
        }
    }
}
