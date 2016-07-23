using UnityEngine;
using System.Collections;

public class SwitchBehaviour : InteractableObject
{
    [SerializeField] private GameObject[] targets;
    private bool isEnabled;

    public override void HitByRaycast()
    {
        if (!isEnabled)
        {
            base.HitByRaycast();

            TurnOnAnimation("Activate");

            TriggerTargetBehaviour();

            isEnabled = true;
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
