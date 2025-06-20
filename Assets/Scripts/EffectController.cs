using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
     public ParticleSystem rootParticleSystem;

      public void PlayParticles()
    {
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }
}
