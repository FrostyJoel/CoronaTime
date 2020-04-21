using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCheck : MonoBehaviour
{
    public float playerCheckRad = 5f;
    public List<Outline> oLines = new List<Outline>();
    public void Update()
    {
        Collider[] objCol = Physics.OverlapSphere(transform.position, playerCheckRad);
        for (int i = 0; i < objCol.Length; i++)
        {
            if (objCol[i].tag == "Interact" && objCol[i].GetComponent<Outline>())
            {
                Renderer render = objCol[i].GetComponent<Renderer>();
                Outline oLine = objCol[i].GetComponent<Outline>();
                InteractableProduct iProduct = objCol[i].GetComponent<InteractableProduct>();

                if (iProduct)
                {
                    if(iProduct.currentPlace != InteractableProduct.Place.InShelve)
                    {
                        break;
                    }
                }
                if (!oLines.Contains(oLine))
                {
                    oLines.Add(oLine);
                }
                if (render.isVisible)
                {
                    oLine.enabled = true;
                }
                else
                {
                    if (oLine.enabled)
                    {
                        oLine.enabled = false;
                    }
                }
            }
        }
        if(oLines.Count > 0)
        {
            for (int i = 0; i < oLines.Count; i++)
            {
                float dis = Vector3.Distance(transform.position, oLines[i].transform.position);
                if (dis > playerCheckRad && oLines[i].enabled)
                {
                    oLines[i].enabled = false;
                    oLines.Remove(oLines[i]);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerCheckRad);
    }
}
