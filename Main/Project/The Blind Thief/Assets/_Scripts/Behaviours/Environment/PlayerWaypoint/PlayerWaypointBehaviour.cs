using UnityEngine;
using System.Collections;

public class PlayerWaypointBehaviour : AnimationController
{
    private Vector3 targetPosition;

    [SerializeField] private SpriteRenderer[] sprites;

    void Start()
    {
        PlayerMovementBehaviour.HasFoundLegitPath += MoveToTargetPosition;
    }

    void MoveToTargetPosition()
    {
        targetPosition = InputController.Instance.TargetPosition;

        transform.position = new Vector3(targetPosition.x,targetPosition.y,-5);

        InitiateAnimation();
    }

    void InitiateAnimation()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = true;
        }

        TurnOnAnimation("isActive");

        StartCoroutine(TurnOffWaypoint());
    }

    IEnumerator TurnOffWaypoint()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }

        TurnOnAnimation("isIdle");
    }
}
