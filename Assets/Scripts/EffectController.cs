using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
     public ParticleSystem rootParticleSystem;
    public bool isPlaying = true;
      public void PlayParticles()
    {
        if (isPlaying == false) return;
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }
}
