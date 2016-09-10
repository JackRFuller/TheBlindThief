using UnityEngine;
using System.Collections;

public class PlayerBreathingController : Singleton<PlayerBreathingController>
{ 
    [SerializeField]
    private PlayerStates.MovementState movementState;
    [SerializeField]
    private bool isDebugMode;

    [Header("Idle Settings")]
    [SerializeField]
    private float minIdleBreathingRingSize;
    [SerializeField]
    private float maxIdleBreathingRingSize;
    [SerializeField]
    private float idleBreathingRingSpeed;
    [SerializeField]
    private AnimationCurve idleBreathingRingCurve;

    [Header("Sneaking Settings")]
    [SerializeField]
    private float minSneakingBreathingRingSize;
    [SerializeField]
    private float maxSneakingBreathingRingSize;
    [SerializeField]
    private float sneakingBreathingRingSpeed;
    [SerializeField]
    private AnimationCurve sneakingBreathingRingCurve;

    [Header("Running Settings")]
    [SerializeField]
    private float minRunningBreathingRingSize;
    [SerializeField]
    private float maxRunningBreathingRingSize;
    [SerializeField]
    private float runningBreathingRingSpeed;
    [SerializeField]
    private AnimationCurve runningBreathingRingCurve;

    [Header("Breathing In Settings")]
    [SerializeField]
    private float gaspMultiplier; //Determines how much air the player takes in - increase the size of the ring
    [SerializeField]
    private float breathingInSpeed;
    [SerializeField]
    private float holdingBreathTimer;
    [SerializeField]
    private AnimationCurve holdingInBreathCurve;
    [SerializeField]
    private float holdingBreathCooldown;
   
    private bool isBreathingIn;
    private float breathingTimer;
    private Vector3 breathingRingStartSize;
    private Vector3 breathingInTargetSize;
    private float timeStartedBreathingIn;
    private bool canHoldBreath = true;

    [Header("Gasping")]
    [SerializeField]
    private float gaspMaxSize; //Determines how large the maximum gasp is after the player has held their
    [SerializeField]
    private float gaspSpeed;
    [SerializeField]
    private AnimationCurve gaspingCurve;

    private Vector3 gaspingStartingVector;
    private Vector3 gaspingTargetVector;
    private float timeStartedGasping;
    private bool isGasping;


    [Header("Mesh Attributes")]
    [SerializeField]
    private Transform breathingRingTransform;

    private float timeStarted;
    private Vector3 startingRingSize;
    private Vector3 targetRingSize;
    private Vector3 minRingSize;
    private Vector3 maxRingSize;
    private float breathingSpeed;
    private AnimationCurve breathingCurve;
    private bool isBreathing = true;
    private int breathingCounter;

    public delegate void startedHoldingBreath();
    public startedHoldingBreath StartedHoldingBreath;

    public delegate void holdingBreath();
    public holdingBreath HoldingBreath;

    public delegate void startedReleasingBreath();
    public startedReleasingBreath StartedReleasingBreath;

    public delegate void releasingBreath();
    public releasingBreath ReleasingBreath;


    void Start()
    {
        SubscribeToPlayerMovement();
        ChangeBreathingState();
        InitialiseBreathingRing();        
    }

    void SubscribeToPlayerMovement()
    {
        PlayerMovementBehaviour.ChangeInMovementState += ChangePlayerMovementState;
    }

    /// <summary>
    /// Triggered when the player moves - subscribed through delegate
    /// </summary>
    void ChangePlayerMovementState()
    {
        movementState = PlayerMovementBehaviour.Instance.MovementState;
        ChangeBreathingState();
        SwitchBreathingTargets();      
    }

    void InitialiseBreathingRing()
    {
        breathingRingTransform.localScale = Vector3.zero;        
    }

    void SwitchBreathingTargets()
    {
        minRingSize = breathingRingTransform.localScale;

        if (breathingCounter == 0)
        {
            maxRingSize = targetRingSize;
        }
        else
        {
            maxRingSize = startingRingSize;
        }

        if(isDebugMode)
        {
            Debug.Log("Min Ring Size " + minRingSize);
            Debug.Log("Max Ring Size " + maxRingSize);
        }

        
        timeStarted = Time.time;
        isBreathing = true;
        
           
    }

    void Update()
    {
        if (isBreathing)
            InterpolateBreathingRingSize();

        if (isBreathingIn)
            BreathIn();

        if (isGasping)
            Gasp();
    }

    void OnMouseDown()
    {
        if(canHoldBreath)
        {
            InitiateBreathingIn();
        }
       
    }

    void OnMouseUp()
    {
        if (isBreathingIn)
            InitiateGasp();
    }

    void InitiateBreathingIn()
    {
        isBreathing = false;

        breathingRingStartSize = breathingRingTransform.localScale;
        breathingInTargetSize = Vector3.zero;

        timeStartedBreathingIn = Time.time;
        breathingTimer = 0;
        isBreathingIn = true;

        StartedHoldingBreath();
    }

    void BreathIn()
    {
        HoldingBreath();

        breathingTimer += Time.deltaTime;
        if (breathingTimer >= holdingBreathTimer)
        {
            isBreathingIn = false;
            InitiateGasp();
        }

        float timeSinceStarted = Time.time - timeStartedBreathingIn;
        float percentageComplete = timeSinceStarted / breathingInSpeed;

        breathingRingTransform.localScale = Vector3.Lerp(breathingRingStartSize, breathingInTargetSize, holdingInBreathCurve.Evaluate(percentageComplete));

       
    }

    void InitiateGasp()
    {
        StartedReleasingBreath();

        gaspingStartingVector = breathingRingTransform.localScale;
        float gaspSize = CalcateSizeOfGasp();
        
        gaspingTargetVector = CreateVector(gaspSize);

        timeStartedGasping = Time.time;
        isGasping = true;
    }

    void Gasp()
    {
        ReleasingBreath();

        float timeSinceStarted = Time.time - timeStartedGasping;
        float percentageComplete = timeSinceStarted / gaspSpeed;

        breathingRingTransform.localScale = Vector3.Lerp(gaspingStartingVector, gaspingTargetVector, gaspingCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 1.0f)
        {
            EndOfHoldingBreath();
        }
    }

    float CalcateSizeOfGasp()
    {
        //Work out % of how long player held their breath
        float percentage = breathingTimer / holdingBreathTimer;
        float sizeOfGasp = gaspMaxSize * percentage;
        return sizeOfGasp;
    }

    IEnumerator HoldingBreathCooldown()
    {
        yield return new WaitForSeconds(holdingBreathCooldown);
        canHoldBreath = true;
    }

    void EndOfHoldingBreath()
    {
        breathingCounter = 1; //Go to the smallest size - breathing in
        isBreathingIn = false;
        isGasping = false;                
        canHoldBreath = false;        
        ChangePlayerMovementState();
        StartCoroutine(HoldingBreathCooldown());
        isBreathing = true;
    }

    void InterpolateBreathingRingSize()
    {
        float timeSinceStarted = Time.time - timeStarted;
        float percentageComplete = timeSinceStarted / breathingSpeed;

        breathingRingTransform.localScale = Vector3.Lerp(minRingSize, maxRingSize, breathingCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 1.0f)
        {
            breathingCounter++;

            if (breathingCounter > 1)
                breathingCounter = 0;

            isBreathing = false;

            SwitchBreathingTargets();
        }
    }

    void ChangeBreathingState()
    {
        switch(movementState)
        {
            case PlayerStates.MovementState.idle:
                startingRingSize = CreateVector(minIdleBreathingRingSize);
                targetRingSize = CreateVector(maxIdleBreathingRingSize);
                breathingSpeed = idleBreathingRingSpeed;
                breathingCurve = idleBreathingRingCurve;
                if (isDebugMode)
                    Debug.Log("Idle");
                break;
            case PlayerStates.MovementState.sneaking:
                startingRingSize = CreateVector(minSneakingBreathingRingSize);
                targetRingSize = CreateVector(maxSneakingBreathingRingSize);
                breathingSpeed = sneakingBreathingRingSpeed;
                breathingCurve = sneakingBreathingRingCurve;
                if (isDebugMode)
                    Debug.Log("Sneaking");
                break;
            case PlayerStates.MovementState.running:
                startingRingSize = CreateVector(minRunningBreathingRingSize);
                targetRingSize = CreateVector(maxRunningBreathingRingSize);
                breathingSpeed = idleBreathingRingSpeed;
                breathingCurve = idleBreathingRingCurve;
                if (isDebugMode)
                    Debug.Log("Running");
                break;
        }
    }

    Vector3 CreateVector(float size)
    {
        Vector3 newVector = new Vector3(size, size, 1);
        return newVector;
    }    	
}
