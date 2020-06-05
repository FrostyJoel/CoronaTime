using Photon.Pun;
using UnityEngine;

public class Spawnpoints : MonoBehaviourPunCallbacks {
    public static Spawnpoints sp_Single;
    public Transform[] spawnpoints;

    private void Awake() {
        sp_Single = this;
    }

    public Vector3[] GetSpPositionAndRotation(int index) {
        Vector3[] par = new Vector3[] { spawnpoints[index].position, spawnpoints[index].rotation.eulerAngles };
        return par;
    }
}