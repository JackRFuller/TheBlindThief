using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private StartingTarget startingTarget;

    [Header("Targets")]
    [SerializeField]
    private Transform accusedTransform;
    [SerializeField]
    private Transform sageTransform;
    private Transform currentTarget;
    private int targetIndex; //0 = Accused, 1 = Sage

    //Lerping Variables
    [Header("Lerping Attributes")]
    private Vector3 startingPoint;
    private Vector3 targetPoint;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private AnimationCurve movementCurve;
    private float timeStartedMoving;
    private bool isMovingBetweenTargets;

    private enum StartingTarget
    {
        Accused,
        Sage,
    }

    void Start()
    {
        DetermineStartingTarget();
    }

    void DetermineStartingTarget()
    {
        switch(startingTarget)
        {
            case StartingTarget.Accused:
                currentTarget = accusedTransform;
                EventManager.TriggerEvent("DisableSage");
                break;

            case StartingTarget.Sage:
                currentTarget = sageTransform;
                EventManager.TriggerEvent("DisableAccused");
                break;
        }
    }

  
    void Update()
    {
        DetectInput();
    }

    void DetectInput()
    {      
        if (isMovingBetweenTargets)
        {
            MoveToNewTarget();
        }
        else
        {
            FollowTarget();

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                SwitchTarget();
            }
        }
    }

    void FollowTarget()
    {
        Vector3 newPos = new Vector3(currentTarget.position.x, currentTarget.position.y, -10);
        transform.position = newPos;
    }

    void SwitchTarget()
    {
        //Determine Target To Switch To
        startingPoint = transform.position;

        if (targetIndex == 0)
        {
            targetPoint = ReturnNewTargetVector(sageTransform.position);
            currentTarget = sageTransform;           
            EventManager.TriggerEvent("DisableAccused");
        }
        if (targetIndex == 1)
        {
            targetPoint = ReturnNewTargetVector(accusedTransform.position);
            currentTarget = accusedTransform;           
            EventManager.TriggerEvent("DisableSage");
        }
            

        timeStartedMoving = Time.time;
        isMovingBetweenTargets = true;
    }

    void MoveToNewTarget()
    {
        float timeSinceStarted = Time.time - timeStartedMoving;
        float percentageComplete = timeSinceStarted / movementSpeed;

        transform.position = Vector3.Lerp(startingPoint, targetPoint, movementCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 1.0f)
        {
            isMovingBetweenTargets = false;

            //Determine Which Character to Enable
            if (targetIndex == 0)
            {
                EventManager.TriggerEvent("ActivateSage");                
            }
            if(targetIndex == 1)
            {
                EventManager.TriggerEvent("ActivateAccused");               
            }

            targetIndex++;
            if (targetIndex > 1)
                targetIndex = 0;
        }
    }

    Vector3 ReturnNewTargetVector(Vector3 target)
    {
        Vector3 targetVector = new Vector3(target.x, target.y, -10.0f);
        return targetVector;
    }
	
}
