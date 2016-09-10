using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PressurePlateBehaviour : AnimationController
{
    [Header("Target")]
    [SerializeField] private GameObject target;

    [Header("Object Properties")]
    [SerializeField] private Collider col;
    private bool hasBeenActivated;

    private AudioSource audioSource;

    void Start()
    {
        GetAuidoComponent();
    }

    void GetAuidoComponent()
    {
        audioSource = GetComponent<AudioSource>();
    }

    

    void ActivatePressurePlate()
    {
        TurnOnAnimation("Activate");
        col.enabled = false;
        target.SendMessage("ActivateSwitch");

        PlayAudio();
    }

    void PlayAudio()
    {
        audioSource.Play();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") || other.tag.Equals("Enemy"))
        {
            ActivatePressurePlate();
        }
    }
}
