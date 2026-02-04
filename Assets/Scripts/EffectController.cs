using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
     public ParticleSystem rootParticleSystem;
     public GameObject healingObject;
    public bool isPlaying = true;
    public bool isHealing = false;
      public void PlayParticles()
    {
        if (isPlaying == false) return;
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }
        public void PlayHealingParticles()
        {
            if (isHealing == true) return;
            healingObject.SetActive(true);
            isHealing = true;
            foreach (var ps in healingObject.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }
            isHealing = false;
        }
    
    public void HealPlayer()
    {
        PlayerMovement.playerMovementInstance.HealPlayer(50);
        PlayHealingParticles();
    }
}