using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDurations : MonoBehaviour
{
    public float dur = 3f;
    public bool play = false;
    private ParticleSystem[] ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem party in ps)
        {
            ParticleSystem.MainModule nPS = party.main;
            nPS.duration = dur;   
        }
    }
    private void Update()
    {
        if (play)
        {
            foreach (ParticleSystem party in ps)
            {
                party.Stop();
                party.Play();
            }
            play = false;
        }
    }
}
