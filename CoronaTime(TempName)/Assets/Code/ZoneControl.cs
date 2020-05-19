using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneControl : MonoBehaviour {
    public ZoneType[] zoneTypes = new ZoneType[4];
}

[System.Serializable]
public class ZoneType {

    public GameObject zone1;
    public GameObject zone2;
    public GameObject zone3;
    public GameObject zone4;
}