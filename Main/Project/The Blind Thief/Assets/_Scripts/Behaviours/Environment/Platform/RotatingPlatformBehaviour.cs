using UnityEngine;
using System.Collections;

public class RotatingPlatformBehaviour : MonoBehaviour
{
    private bool isRotating;

    //Movement
    [SerializeField] private float rotationTarget;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private AnimationCurve movementCurve;
    private float timeStarted;
    private int rotationCount = 1;

    private Quaternion startingRotation;
    private Quaternion targetRotation;

    private Transform parentSwitch;

    void ActivateSwitchBehaviour(Transform _enabler)
    {
        InitiateRotation();

        if (parentSwitch == null)
            parentSwitch = _enabler;
    }

    void InitiateRotation()
    {
        timeStarted = Time.time;

        startingRotation.eulerAngles = transform.rotation.eulerAngles;
        targetRotation.eulerAngles = startingRotation * new Vector3(0, 0, rotationTarget * rotationCount);

        rotationCount++;

        if (rotationCount == 5)
        {
            rotationCount = 1;
        }

        isRotating = true;
    }

    void Update()
    {
        if(isRotating)
            RotatePlatform();
    }

    void RotatePlatform()
    {
        float _timeSinceStarted = Time.time - timeStarted;
        float _percentageComplete = _timeSinceStarted/rotationSpeed;

        transform.rotation  = Quaternion.Lerp(startingRotation,targetRotation, movementCurve.Evaluate(_percentageComplete));

        if (_percentageComplete >= 1)
        {
            isRotating = false;

            parentSwitch.SendMessage("EnableSwitch");

            //Trigger Recalculating of the Nodes
            NodeController.Instance.GetNodes();
        }
    }


	
}
