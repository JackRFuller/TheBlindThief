using UnityEngine;
using System.Collections;

public class SageController : MonoBehaviour, IEvent
{
    [Header("Components")]
    [SerializeField]
    private SageMovement movementBehaviour;

    public void OnEnable()
    {
        EventManager.StartListening("ActivateSage", movementBehaviour.ActivateMovement);
        EventManager.StartListening("DisableSage", movementBehaviour.DisableMovement);
    }

    public void OnDisable()
    {
        EventManager.StopListening("ActivateSage", movementBehaviour.ActivateMovement);
        EventManager.StopListening("DisableSage", movementBehaviour.DisableMovement);
    }
}
