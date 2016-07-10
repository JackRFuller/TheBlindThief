using UnityEngine;
using System.Collections;

public class PlayerWaypointBehaviour : AnimationController
{
    private Vector3 targetPosition;

    [SerializeField] private GameObject[] sprites;

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
        StartCoroutine(WaitToTurnOnGameObjects());

        if (!PlayerMovementBehaviour.Instance.IsSprinting)
        {
            TurnOnAnimation("isSneaking");
        }

        if (PlayerMovementBehaviour.Instance.IsSprinting)
        {
            TurnOnAnimation("isSprinting");
        }

        StartCoroutine(TurnOffWaypoint());
    }

    IEnumerator WaitToTurnOnGameObjects()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <sprites.Length; i++)
        {
            sprites[i].SetActive(true);
        }
    }

    IEnumerator TurnOffWaypoint()
    {
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].SetActive(false);
        }

        TurnOnAnimation("isIdle");
    }
}
