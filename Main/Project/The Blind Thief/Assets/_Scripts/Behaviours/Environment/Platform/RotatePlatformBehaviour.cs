using UnityEngine;
using System.Collections;

public class RotatePlatformBehaviour : PlatformBehaviour
{
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private AnimationCurve rotationCurve;

    
    private Vector3 targetRotation;
    private bool isRotating;

    private Quaternion startingRot;
    private Quaternion targetRot;
    private float timeStarted;

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

        if (StartedRotating != null)
            StartedRotating();

        
    }

    void InitiateRotation()
    {
        startingRot = transform.rotation;

        
        targetRotation = new Vector3(transform.localEulerAngles.x,
                                     transform.localEulerAngles.y,
                                     transform.localEulerAngles.z + 90);

        targetRot = Quaternion.Euler(targetRotation);
       
        isRotating = true;
        timeStarted = Time.time;

        TurnOffNodeColliders();
    }  

    public override void Update()
    {
        base.Update();

        if (isRotating)
            RotatePlatform();
    }

    void RotatePlatform()
    {
        float timeSinceStarted = Time.time - timeStarted;
        float percentageComplete = timeSinceStarted / rotationSpeed;

        transform.rotation = Quaternion.Lerp(startingRot, targetRot, rotationCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 1.0f)
        {
            EndRotation();
        }
    }

    void EndRotation()
    {
        TurnOnNodeColliders();

        audioSource.clip = stopMovement;
        PlayAudio();

        isRotating = false;
        transform.localEulerAngles = targetRotation;
        if (transform.localEulerAngles.z >= 360)
            transform.localEulerAngles = Vector3.zero;

        parentSwitch.SendMessage("EnableSwitch");

        //Trigger Recalculating of the Nodes
        StartCoroutine(PathController.Instance.RegisterMovementOfPlatforms());

        if (EndedRotating != null)
            EndedRotating();
    }


}
