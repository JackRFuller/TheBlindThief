using UnityEngine;
using System.Collections;

public class PressurePlateBehaviour : AnimationController
{
    [Header("Target")]
    [SerializeField] private GameObject target;

    [Header("Object Properties")]
    [SerializeField] private Collider col;
    private bool hasBeenActivated;

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
    }
}
