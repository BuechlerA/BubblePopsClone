using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiCannonBehaviour : MonoBehaviour
{
    public ParticleSystem confettiCannonLeft, confettiCannonRight, confettiTop;
    public AudioClip confettiCannonSound, applauseSound;

    private AudioSource confettiAudioSource;

    private void Start()
    {
        confettiAudioSource = GetComponent<AudioSource>();
    }

    [ContextMenu("FireConfettiCannons")]
    public void FireConfettiCannons()
    {
        if (!confettiCannonLeft.isPlaying)
        {
            confettiAudioSource.PlayOneShot(confettiCannonSound);
            confettiCannonLeft.Play();
            confettiCannonRight.Play();
        }
    }
    [ContextMenu("ConfettiApplause")]
    public void ConfettiApplause()
    {
        if (!confettiTop.isPlaying)
        {
            confettiAudioSource.PlayOneShot(applauseSound);
            confettiTop.Play();
        }
    }
}
