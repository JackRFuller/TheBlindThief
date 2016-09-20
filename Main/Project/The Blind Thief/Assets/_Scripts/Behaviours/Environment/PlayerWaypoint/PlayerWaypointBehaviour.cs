using UnityEngine;
using System.Collections;

public class PlayerWaypointBehaviour : AnimationController
{
    private Vector3 targetPosition;

    void Start()
    {
        PlayerMovementBehaviour.HasFoundLegitPath += MoveToTargetPosition;
        PlayerMovementBehaviour.HasReachedDestination += TurnOffAnimation;
    }

    void MoveToTargetPosition()
    {
        if (InputController.Instance == null)
            Debug.Log("Failure");

        targetPosition = InputController.Instance.TargetPosition;

        transform.position = new Vector3(targetPosition.x, targetPosition.y, -5);

        InitiateAnimation();
    }

    void InitiateAnimation()
    {        
        if (!PlayerMovementBehaviour.Instance.IsSprinting)
        {
            TurnOnAnimation("isSneaking");
        }

        if (PlayerMovementBehaviour.Instance.IsSprinting)
        {
            TurnOnAnimation("isSprinting");
        }
    }

    void TurnOffAnimation()
    {
        TurnOnAnimation("isIdle");
    }

  
}
