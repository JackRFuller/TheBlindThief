using UnityEngine;
using System.Collections;

public class SwitchBehaviour : InteractableObject
{
    [Header("Wire")] [SerializeField] private GameObject wireTarget;

    [Header("Meshes")]
    [SerializeField] private MeshRenderer[] meshes;
    [SerializeField] private float colorLerpTime;
    [SerializeField] private AnimationCurve colorLerpCurve;
    private bool isLerpingColour;
    private float timeStartedFadingColor;
    private Material newMaterial;
    private Color newColor;

    [Header("Targets")]
    [SerializeField] private GameObject[] targets;
    private bool isActivated;
    private bool isEnabled;

    public override void HitByRaycast()
    {
        if (isActivated)
        {
            if (!isEnabled)
            {
                base.HitByRaycast();

                TurnOnAnimation("Activate");

                TriggerTargetBehaviour();

                isEnabled = true;
            }
        }
    }

    //Turns on Switch
    void ActivateSwitch()
    {
        newMaterial = meshes[0].material;
        newColor = newMaterial.color;

        timeStartedFadingColor = Time.time;
        isLerpingColour = true;

    }

    void Update()
    {
        if(isLerpingColour)
            LerpColour();   
    }

    void LerpColour()
    {
        float _timeSinceStarted = Time.time - timeStartedFadingColor;
        float _percentageComplete = _timeSinceStarted/colorLerpTime;

        for (int i = 0; i < meshes.Length; i++)
        {
            newColor.a = Mathf.Lerp(newColor.a, 1, colorLerpCurve.Evaluate(_percentageComplete));
            newMaterial.color = newColor;

            meshes[i].material = newMaterial;
        }

        if (_percentageComplete >= 1.0f)
        {
            isLerpingColour = false;
            isActivated = true;
            wireTarget.SendMessage("ActivateWire",SendMessageOptions.DontRequireReceiver);
        }
    }

    void TriggerTargetBehaviour()
    {
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].SendMessage("ActivateSwitchBehaviour",this.transform, SendMessageOptions.DontRequireReceiver);
            }
        }

        StartCoroutine(WaitToTurnOffAnimation());
    }

    //:ets Player use switch again
    void EnableSwitch()
    {
        isEnabled = false;
    }

    IEnumerator WaitToTurnOffAnimation()
    {
        yield return new WaitForSeconds(0.75f);
        TurnOnAnimation("Deactivate");
    }
}
