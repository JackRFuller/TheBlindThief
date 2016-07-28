using UnityEngine;
using System.Collections;

public class PlayerAnimationController : AnimationController
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] sneakingFootsteps;
    [SerializeField] private AudioClip[] runningFootsteps;

    //Audio
	public void PlaySneakingFootsteps()
    {
        int _randomClip = Random.Range(0, sneakingFootsteps.Length);
        audioSource.clip = sneakingFootsteps[_randomClip];
        audioSource.Play();
    }

    public void PlayRunningFootsteps()
    {
        int _randomClip = Random.Range(0, runningFootsteps.Length);
        audioSource.clip = runningFootsteps[_randomClip];
        audioSource.Play();
    }
}
