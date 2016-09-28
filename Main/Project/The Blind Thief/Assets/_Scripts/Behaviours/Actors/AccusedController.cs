using UnityEngine;
using System.Collections;

public class AccusedController : MonoBehaviour {

    [Header("Components")]
    [SerializeField]
    private CharacterMovement movementBehaviour;

    public void OnEnable()
    {
        EventManager.StartListening("ActivateAccused", movementBehaviour.ActivateMovement);
        EventManager.StartListening("DisableAccused", movementBehaviour.DisableMovement);
    }

    public void OnDisable()
    {
        EventManager.StopListening("ActivateAccused", movementBehaviour.ActivateMovement);
        EventManager.StopListening("DisableAccused", movementBehaviour.DisableMovement);
    }
}
