using UnityEngine;
using System.Collections;

public class SwitchBehaviour : InteractableObject
{
    [SerializeField] private GameObject[] targets;

    public override void HitByRaycast()
    {
        base.HitByRaycast();

        TriggerTargetBehaviour();
    }

    void TriggerTargetBehaviour()
    {
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].SendMessage("ActivateSwitchBehaviour", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
