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

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ActivatePressurePlate();
        }
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
}
