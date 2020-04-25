using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCheck : MonoBehaviour
{
    public RaycastHit hit;
    public Outline lastOutline;
    private Controller controller;
    private CartStorage storage;
    private void Start()
    {
        controller = GetComponent<Controller>();
        storage = GetComponent<CartStorage>();
    }
    private void Update()
    {
        if (Physics.Raycast(controller.pov.position, controller.pov.forward, out hit, storage.interactRange))
        {
            Outline outlineObj = hit.transform.gameObject.GetComponent<Outline>();
            if (lastOutline != outlineObj && lastOutline != null)
            {
                lastOutline.enabled = false;
            }
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
