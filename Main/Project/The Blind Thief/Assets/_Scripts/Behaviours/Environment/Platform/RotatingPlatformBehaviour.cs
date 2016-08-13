using UnityEngine;
using System.Collections;

public class RotatingPlatformBehaviour : PlatformBehaviour
{
    private bool isRotating;

    //Movement
    [Header("Rotation")]
    [SerializeField] private float rotationTarget;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private AnimationCurve movementCurve;
    private float timeStarted;
    private int rotationCount = 1;

    private Quaternion startingRotation;
    private Quaternion targetRotation;

    private Transform parentSwitch;

    //Subscribed to by Enemies & Player
    public delegate void startedRotating();
    public startedRotating StartedRotating;
    public delegate void endedRotating();
    public endedRotating EndedRotating;
    

    public override void ActivateSwitchBehaviour(Transform _enabler)
    {
        InitiateRotation();

        if (parentSwitch == null)
            parentSwitch = _enabler;

        if(StartedRotating != null)
             StartedRotating();
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

    public override void Update()
    {
        base.Update();

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
            StartCoroutine(EndRotation());
        }
    }

    IEnumerator EndRotation()
    {
        isRotating = false;

        parentSwitch.SendMessage("EnableSwitch");

        //Trigger Recalculating of the Nodes

        yield return StartCoroutine(NodeController.Instance.GetNodes());

        if(EndedRotating != null)
            EndedRotating();
    }


	
}
