using UnityEngine;
using System.Collections;

public class InteractableObject : AnimationController
{
    [Header("Audio")]
    [SerializeField]
    protected AudioSource audioSource;

    public virtual void HitByRaycast()
    {
        
    }

    public virtual void PlayAudio()
    {
        audioSource.Play();
    }

    public virtual void StopAudio()
    {
        audioSource.Stop();
    }
}
