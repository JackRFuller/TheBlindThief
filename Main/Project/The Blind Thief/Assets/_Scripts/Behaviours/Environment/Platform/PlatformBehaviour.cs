using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
    [Header("Controlling Switch")]
    [SerializeField] private SwitchBehaviour swScript;

    [Header("Layers")]    
    [SerializeField]
    private int disabledLayerIndex;
    [SerializeField]
    private int enabledLayerIndex;

    [Header("Colors")]
    [SerializeField]
    private MeshRenderer[] quads;
    [SerializeField]
    private Color startingColor;
    [SerializeField]
    private Color targetingColor;
    [SerializeField]
    private float speed;
    [SerializeField]
    private AnimationCurve lerpCurve;
    private bool isLerpingColor;
    private float timeStartedLerpingColor;
    private PlatformState platformState;
    private Color startColor; //Used for the lerp
    private Color targetColor; //Used for the lerp
    private Material newPlatformMaterial;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] private AudioClip platformActivated;
    [SerializeField] protected AudioClip stopMovement;

    private enum PlatformState
    {
        disabled,
        enabled,
    }

    public virtual void Start()
    {
        SetStartingColor();
        SetStartingLayer();
    }

    void SetStartingColor()
    {
        platformState = PlatformState.disabled;

        newPlatformMaterial = quads[0].material;
        newPlatformMaterial.color = startingColor;

        for (int i = 0; i < quads.Length; i++)
        {
            quads[i].material = newPlatformMaterial;            
        }
    }

    void SetStartingLayer()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            quads[i].gameObject.layer = disabledLayerIndex;
        }
    }

    public virtual void ActivateSwitchBehaviour(Transform _enablerer)
    {
        
    }

    public virtual void ActivatePlatform(bool isActivatedByEnemy)
    {
        if (isActivatedByEnemy)
            Debug.Log("Hit by Enemy");

        if (isActivatedByEnemy)
        {
            if (platformState == PlatformState.disabled)
                return;
        }
        

        InitiateColorChange();
        ChangePlatformState();
        audioSource.clip = platformActivated;
        PlayAudio();
    }

    protected void PlayAudio()
    {
        audioSource.Play();
    }

    protected void TurnOffNodeColliders()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform node in child)
            {
                if (node.GetComponent<Collider>())
                    node.GetComponent<Collider>().enabled = false;
            }
        }
    }

    protected void TurnOnNodeColliders()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform node in child)
            {
                if (node.GetComponent<Collider>())
                    node.GetComponent<Collider>().enabled = true;
            }
        }
    }
   
    void InitiateColorChange()
    {
        switch(platformState)
        {
            case PlatformState.disabled:
                startColor = startingColor;
                targetColor = targetingColor;
                break;
            case PlatformState.enabled:
                startColor = targetingColor;
                targetColor = startingColor;
                break;
        }
        
        timeStartedLerpingColor = Time.time;
        isLerpingColor = true;
    }

    public virtual void Update()
    {
        if (isLerpingColor)
            FadeInColor();
    }

    void FadeInColor()
    {
        float timeSinceStarted = Time.time - timeStartedLerpingColor;
        float percentageComplete = timeSinceStarted / speed;

        for (int i = 0; i < quads.Length; i++)
        {
            newPlatformMaterial.color = Color.Lerp(startColor, targetColor, lerpCurve.Evaluate(percentageComplete));
        }

        if (percentageComplete >= 1.0f)
        {
            isLerpingColor = false;

            if (swScript != null)
                swScript.IsActivated = true;
        }
    }

    void ChangePlatformState()
    {
        switch (platformState)
        {
            case PlatformState.disabled:
                platformState = PlatformState.enabled;
                for (int i = 0; i < quads.Length; i++)
                {
                    quads[i].gameObject.layer = LayerMask.NameToLayer("Platforms");
                }
                break;
            case PlatformState.enabled:
                platformState = PlatformState.disabled;
                for (int i = 0; i < quads.Length; i++)
                {
                    quads[i].gameObject.layer = LayerMask.NameToLayer("Default");
                }
                break;
        }
    }
}
