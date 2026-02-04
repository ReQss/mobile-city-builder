using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{
    public AudioClip bowShotSound;
    public AudioClip swordSlashSound;
    public List<AudioClip> footStepSounds;
    int footStepIndex = 0;
    public AudioSource audioSource;
    public AudioSource movementAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayBowShotSound()
    {
        audioSource.PlayOneShot(bowShotSound);
    }
    public void PlaySwordSlashSound()
    {
        audioSource.PlayOneShot(swordSlashSound);
    }
    public void PlayFootStepSound()
    {
        if (footStepSounds.Count == 0) return;

        AudioClip footStepSound = footStepSounds[footStepIndex];
        movementAudioSource.PlayOneShot(footStepSound);

        footStepIndex = (footStepIndex + 1) % footStepSounds.Count;
    }

}
