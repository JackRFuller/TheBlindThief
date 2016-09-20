using UnityEngine;
using System.Collections;

public class PlayerBreathingController : Singleton<PlayerBreathingController>, IReset
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

    [Header("Sneaking Settings")]
    [SerializeField]
    private float minSneakingBreathingRingSize;
    [SerializeField]
    private float maxSneakingBreathingRingSize;
    [SerializeField]
    private float sneakingBreathingRingSpeed;
  
    [Header("Running Settings")]
    [SerializeField]
    private float minRunningBreathingRingSize;
    [SerializeField]
    private float maxRunningBreathingRingSize;
    [SerializeField]
    private float runningBreathingRingSpeed;   

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

    [Header("Camera Effects - Vignette")]
    [SerializeField]
    private VignetteEffect breathInVignette;
    [SerializeField]
    private VignetteEffect breathOutVignette;
    [Header("Camera Effects - Zoom")]
    [SerializeField]
    private CameraZoomEffect breathInEffect;
    [SerializeField]
    private CameraZoomEffect breathOutEffect;

    private float timeStarted;
    private Vector3 startingRingSize;
    private Vector3 targetRingSize;
    private Vector3 minRingSize;
    private Vector3 maxRingSize;
    private float breathingSpeed;
    private AnimationCurve breathingCurve;
    private bool isBreathing = true;
    private int breathingCounter;    

    private BreathingState breathingState; 
    private enum BreathingState
    {
        In,
        Out,
    }


    void Start()
    {       
        SubscribeToPlayerMovement();
        ChangeBreathingState();
        InitialiseBreathingRing();
    }

    void OnEnable()
    {
        EventManager.StartListening("PlayerDeath", StopBreathingDueToDeath);
        EventManager.StartListening("Reset", Reset);
    }

    void OnDisable()
    {
        EventManager.StopListening("PlayerDeath", StopBreathingDueToDeath);
        EventManager.StopListening("Reset", Reset);
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
        breathingState = BreathingState.Out;
        ChangeBreathingState();
        SwitchBreathingTargets();      
    }

    void SwitchBreathingTargets()
    {
        if(breathingRingTransform)
            minRingSize = breathingRingTransform.localScale;

        if (isDebugMode)
            Debug.Log("Breathing Counter: " + breathingCounter);


        if (breathingCounter == 0)
        {
            maxRingSize = targetRingSize;
            breathingState = BreathingState.Out;
        }
        else
        {
            maxRingSize = startingRingSize;
            breathingState = BreathingState.In;
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

    void InitialiseBreathingRing()
    {
        breathingRingTransform.localScale = Vector3.zero;
    }

    void InterpolateBreathingRingSize()
    {
        float speed = breathingSpeed;
        speed *= Time.deltaTime;

        if (breathingState == BreathingState.In)
            speed = -speed;

        Vector3 newVector = new Vector3(breathingRingTransform.localScale.x + speed,
                                        breathingRingTransform.localScale.y + speed,
                                        breathingRingTransform.localScale.z + speed);

        breathingRingTransform.localScale = newVector;

        switch(breathingState)
        {
            case BreathingState.In:
                if (breathingRingTransform.localScale.x <= maxRingSize.x)
                    SwapBreathingStates();
                break;
            case BreathingState.Out:
                if (breathingRingTransform.localScale.x >= maxRingSize.x)
                    SwapBreathingStates();
                break;
        }        
    }

    void SwapBreathingStates()
    {
        breathingCounter++;

        if (breathingCounter > 1)
            breathingCounter = 0;

        isBreathing = false;

        SwitchBreathingTargets();
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

        //Camera Effects
        UnityStandardAssets.ImageEffects.CameraEffectsController.Instance.InitiateZoomEffect(breathInEffect);

        breathingRingStartSize = breathingRingTransform.localScale;
        breathingInTargetSize = Vector3.zero;

        timeStartedBreathingIn = Time.time;
        breathingTimer = 0;
        isBreathingIn = true;

        UnityStandardAssets.ImageEffects.CameraEffectsController.Instance.IntitateVignetteEffect(breathInVignette);
    }

    void BreathIn()
    {
        //HoldingBreath();

        breathingTimer += Time.deltaTime;
        if (breathingTimer >= holdingBreathTimer)
        {
            isBreathingIn = false;
            InitiateGasp();
        }

        float timeSinceStarted = Time.time - timeStartedBreathingIn;
        float percentageComplete = timeSinceStarted / breathingInSpeed;

        breathingRingTransform.localScale = Vector3.Lerp(breathingRingStartSize, breathingInTargetSize, holdingInBreathCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 0.5f)
        {
            //RunningOutOfBreath();
        }
    }

    void InitiateGasp()
    {
        //StartedReleasingBreath();

        gaspingStartingVector = breathingRingTransform.localScale;
        float gaspSize = CalcateSizeOfGasp();
        
        gaspingTargetVector = CreateVector(gaspSize);

        timeStartedGasping = Time.time;
        isGasping = true;
    }

    void Gasp()
    {
        //ReleasingBreath();
        //Camera Effects
        UnityStandardAssets.ImageEffects.CameraEffectsController.Instance.InitiateZoomEffect(breathOutEffect);

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

    void ChangeBreathingState()
    {
        switch(movementState)
        {
            case PlayerStates.MovementState.idle:
                startingRingSize = CreateVector(minIdleBreathingRingSize);
                targetRingSize = CreateVector(maxIdleBreathingRingSize);
                breathingSpeed = idleBreathingRingSpeed;                
                if (isDebugMode)
                    Debug.Log("Idle");
                break;
            case PlayerStates.MovementState.sneaking:
                startingRingSize = CreateVector(minSneakingBreathingRingSize);
                targetRingSize = CreateVector(maxSneakingBreathingRingSize);
                breathingSpeed = sneakingBreathingRingSpeed;                
                if (isDebugMode)
                    Debug.Log("Sneaking");
                break;
            case PlayerStates.MovementState.running:
                startingRingSize = CreateVector(minRunningBreathingRingSize);
                targetRingSize = CreateVector(maxRunningBreathingRingSize);
                breathingSpeed = runningBreathingRingSpeed;               
                if (isDebugMode)
                    Debug.Log("Running");
                break;
        }
    }

    void TurnOnBreathingRing()
    {
        breathingRingTransform.gameObject.SetActive(true);
    }

    void StopBreathingDueToDeath()
    {
        breathingRingTransform.gameObject.SetActive(false);
    }

    Vector3 CreateVector(float size)
    {
        Vector3 newVector = new Vector3(size, size, 1);
        return newVector;
    }  
    
    public void Reset()
    {
        InitialiseBreathingRing();
        TurnOnBreathingRing();
    }  	
}
