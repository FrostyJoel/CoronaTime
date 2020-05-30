using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDurations : MonoBehaviour
{
    public float dur = 3f;
    public bool play = false;
    private ParticleSystem[] ps;

    void Start()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
    }
    private void Update()
    {
        if (play)
        {
            foreach (ParticleSystem party in ps)
            {
                party.Stop();
                ParticleSystem.MainModule nPS = party.main;
                nPS.duration = dur;
                party.Play();
            }
            play = false;
        }
    }
}
