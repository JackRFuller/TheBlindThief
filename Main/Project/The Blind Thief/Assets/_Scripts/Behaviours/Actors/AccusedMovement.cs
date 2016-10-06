using UnityEngine;
using System.Collections;

public class AccusedMovement : Singleton<AccusedMovement>, IReset
{
    [Header("Components")]
    [SerializeField]
    private Collider col;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Animator animController;

    [Header("Movement")]
    [SerializeField]
    protected float sneakSpeed;
    [SerializeField]
    protected float sprintSpeed;
    [SerializeField]
    protected float fallingSpeed;

    private bool isSprinting;
    protected Vector3 movementDirection;
    protected Vector3 fallingDirection;
    private float distToGround;
    private float characterRotation;
    public float CharacterRotation { get { return characterRotation; } }

    [Header("Grounded")]
    [SerializeField]
    private LayerMask groundedLayer;
    [SerializeField]
    private float distToGroundModifier;    
    private bool isGrounded;
    private float cooldownFromLanded; //Determines how long to wait until player can move again;
    private bool canMove = true;
    private bool hasFinishedRotating = true;
    [SerializeField]
    private float maxFallingHeight;
    private bool isFalling;
    private Vector3 startFallingPosition;

    private bool hasDied;
    private bool hasStartedMoving;
    private bool hasMoved;

    [Header("Debug")]
    [SerializeField]
    protected bool DebugMode;

    //Reset
    private Vector3 spawnPoint;
    private Vector3 spawnRotation;
    private Transform originalParent;

    void OnEnable()
    {
        EventManager.StartListening("Reset", Reset);
    }

    void OnDisable()
    {
        EventManager.StopListening("Reset", Reset);
    }

    private RotatePlatformBehaviour rotatePlatformScript;
    private MovingPlatformBehaviour movingPlatformScript;

    void Start()
    {
        col = GetComponent<Collider>();
        distToGround = col.bounds.extents.y;
        Debug.Log(distToGround);
    }

    void GetStartingPoint()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.eulerAngles;
        originalParent = transform.parent;
    }

    public virtual void ActivateMovement()
    {
        this.enabled = true;
    }

    public virtual void DisableMovement()
    {
        StartCoroutine(StopMovement());
    }

    IEnumerator StopMovement()
    {
        while (!CheckIfGrounded())
        {
            yield return null;
        }
        rb.velocity = Vector3.zero;
        movementDirection = Vector3.zero;
        this.enabled = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (CheckIfGrounded())
        {
            if(canMove)
            {
                GetMovementDirection();
            }
        } 
        else
        {            
            if(!isFalling)
                if(hasFinishedRotating)
                    DetermineFallingDirection();

            //if (isFalling)
            //    CheckDistanceFalling();
        }
    }

    public virtual void GetMovementDirection()
    {
        movementDirection = ReturnMobileInputVectors();
        //movementDirection = ReturnInputVectors();        
    }

    void DetermineFallingDirection()
    {
        if (characterRotation == 0)
            fallingDirection = new Vector3(0, -fallingSpeed, 0);

        if (characterRotation == 180)
            fallingDirection = new Vector3(0, fallingSpeed, 0);

        if (characterRotation == 90)
            fallingDirection = new Vector3(fallingSpeed, 0, 0);

        if (characterRotation == 270)
            fallingDirection = new Vector3(-fallingSpeed, 0, 0);

        startFallingPosition = transform.position;

        isFalling = true;
        animController.SetBool("Falling", true);
        animController.SetBool("Landed", false);
    }

    /// <summary>
    /// Runs while the character is falling
    /// </summary>
    void CheckDistanceFalling()
    {
        Vector3 currentFallingPosition = transform.position;

        float distanceFallen = 0;

        if(characterRotation == 0 || characterRotation == 180)
        {
            distanceFallen = Mathf.Abs(startFallingPosition.y - currentFallingPosition.y);            
        }
        else if(characterRotation == 90 || characterRotation == 270)
        {
            distanceFallen = Mathf.Abs(startFallingPosition.x - currentFallingPosition.x);            
        }
        
        if(distanceFallen >= maxFallingHeight)
        {
            Debug.Log("Dead MOFO!!");
            hasDied = true;
        }
    }

    public virtual Vector3 ReturnInputVectors()
    {
        Vector3 inputVector = Vector3.zero;

        float worldRot = ReturnWorldRotation();

        if (characterRotation == 0 || characterRotation == 180)
        {
            inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            
            if(characterRotation == 180)
                animController.SetFloat("Movement", -inputVector.x);
            else
                animController.SetFloat("Movement", inputVector.x);
        }

        if (characterRotation == 90 || characterRotation == 270)
        {
            inputVector = new Vector3(0, Input.GetAxis("Vertical"), 0);

            if (characterRotation == 270)
                animController.SetFloat("Movement", -inputVector.y);
            else
                animController.SetFloat("Movement", inputVector.y);
        }
            

        return inputVector;
    }

    public virtual Vector3 ReturnMobileInputVectors()
    {
        Vector3 inputVector = MobileInputController.Instance.MovementVector;
        isSprinting = MobileInputController.Instance.IsSprinting;

        

        if(characterRotation == 0)
            animController.SetFloat("Movement", inputVector.x);
        if(characterRotation == 180)
            animController.SetFloat("Movement", -inputVector.x);

        if (characterRotation == 90)
            animController.SetFloat("Movement", inputVector.y);
        if (characterRotation == 270)
            animController.SetFloat("Movement", -inputVector.y);

        animController.SetBool("Sprinting", isSprinting);
        animController.SetBool("Walking", !isSprinting);

        return inputVector;
    }

    public virtual void FixedUpdate()
    {
        if(isGrounded)
        {
            MoveCharacter();            
        }
        else
        {
            MakeCharacterFall();
        }       
    }

    //Used for Left/Right or Up & Down
    protected virtual void MoveCharacter()
    {
        if (movementDirection == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Vector3 movement = Vector3.zero;
        movement = movementDirection;

        if(isSprinting)
            movement *= sprintSpeed;
        else
        {
            movement *= sneakSpeed;
        }
        
        rb.velocity = movement * Time.fixedDeltaTime;
        hasMoved = true;
        hasStartedMoving = true;     
    }

    void MakeCharacterFall()
    {
        Vector3 movement = fallingDirection;
        rb.velocity = movement * Time.fixedDeltaTime;        
    }

    void FreezeCharacter()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;
        hasFinishedRotating = false;
    }

    void UnFreezeCharacter()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        //Determine New Player Rotation
        characterRotation = ReturnWorldRotation();
        Debug.Log(characterRotation);
        while(!CheckIfGrounded())
        {
            rb.isKinematic = true;
        }

        rb.isKinematic = false;
        hasFinishedRotating = true;

        EventManager.TriggerEvent("CharacterRotated");
    }
   
    bool CheckIfGrounded()
    {
        Vector3[] raycastPositions = new Vector3[3];
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            raycastPositions[i] = transform.position;
        }

        Vector3 directionOfRayCast = Vector3.zero;
       
        if (characterRotation == 0)
        {
            directionOfRayCast = Vector3.down;
            raycastPositions[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            raycastPositions[1] = new Vector3(transform.position.x - 0.25f, transform.position.y, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x + 0.25f, transform.position.y, transform.position.z);
        }
        if (characterRotation == 180)
        {
            directionOfRayCast = Vector3.up;
            raycastPositions[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            raycastPositions[1] = new Vector3(transform.position.x - 0.25f, transform.position.y, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x + 0.25f, transform.position.y, transform.position.z);
        }
        if (characterRotation == 90)
        {
            directionOfRayCast = Vector3.right;
            raycastPositions[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            raycastPositions[1] = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        }
        if (characterRotation == 270)
        {
            directionOfRayCast = Vector3.left;
            raycastPositions[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            raycastPositions[1] = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        }


        Ray ray1 = new Ray(raycastPositions[0], directionOfRayCast);
        Ray ray2 = new Ray(raycastPositions[1], directionOfRayCast);
        Ray ray3 = new Ray(raycastPositions[2], directionOfRayCast);

        if (DebugMode)
        {
            Debug.DrawRay(raycastPositions[0], directionOfRayCast, Color.red);
            Debug.DrawRay(raycastPositions[1], directionOfRayCast, Color.red);
            Debug.DrawRay(raycastPositions[2], directionOfRayCast, Color.red);
        }

        Ray[] rays = new Ray[3];
        rays[0] = ray1;
        rays[1] = ray2;
        rays[2] = ray3;

        int numberOfGroundedRays = 0;

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit hit;

            //Checks for Bridges & if Grounded
            if (Physics.Raycast(rays[i], out hit, distToGround + 0.1f))
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Bridge"))
                {
                    numberOfGroundedRays++;
                }
                else
                {
                    if (gameObject.layer != LayerMask.NameToLayer("Accused"))
                    {
                        numberOfGroundedRays++;
                    }
                }
            }
        }

        if (numberOfGroundedRays == 0)
        {
            isGrounded = false;
            return false;
        }
        else
        {
            if(animController.GetBool("Falling"))
            {
                if(hasDied)
                {
                    animController.SetBool("Died", true);
                }
                else
                {
                    canMove = false;
                    animController.SetBool("Falling", false);
                    animController.SetBool("Landing", true);
                    StartCoroutine(WaitForLandingCooldown());
                }
                
            }

            isFalling = false;
            isGrounded = true;
            return true;
        }
           
    }

    IEnumerator WaitForLandingCooldown()
    {
        yield return new WaitForSeconds(cooldownFromLanded);
        canMove = true;
        animController.SetBool("Landed", true);
    }

    public virtual float ReturnWorldRotation()
    {
        float playerRotation = transform.eulerAngles.z;        
        playerRotation = Mathf.Abs(playerRotation);
        playerRotation = Mathf.Round(playerRotation);
        if (playerRotation >= 358)
            playerRotation = 0;

        //if (DebugMode)
        //    Debug.Log("Player Rotation: " + playerRotation);

        return playerRotation;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Platform"))
        {
            transform.parent = other.transform.parent.parent;

            //Check for Rotating Platform Component
            if (transform.parent.GetComponent<RotatePlatformBehaviour>())
            {
                if (rotatePlatformScript != transform.parent.GetComponent<RotatePlatformBehaviour>())
                {
                    rotatePlatformScript = transform.parent.GetComponent<RotatePlatformBehaviour>();
                    SubscribeToRotationEvents();
                }
            }
            else
            {
                if(rotatePlatformScript)
                {
                    UnSubscribeFromRotationEvents();
                    rotatePlatformScript = null;
                }
            }


            if(transform.parent.GetComponent<MovingPlatformBehaviour>())
            {
                if(movingPlatformScript != transform.parent.GetComponent<MovingPlatformBehaviour>())
                {
                    movingPlatformScript = transform.parent.GetComponent<MovingPlatformBehaviour>();
                    SubscribeToMovingPlatformEvents();
                }
            }
            else
            {
                if(movingPlatformScript)
                {
                    UnSubscribeFromMovingPlatformEvents();
                    movingPlatformScript = null;
                }
            }
        }
    }

    void SubscribeToRotationEvents()
    {
        rotatePlatformScript.StartedRotating += FreezeCharacter;
        rotatePlatformScript.EndedRotating += UnFreezeCharacter;
    }

    void SubscribeToMovingPlatformEvents()
    {
        movingPlatformScript.StartedMoving += FreezeCharacter;
        movingPlatformScript.EndedMoving += UnFreezeCharacter;
    }

    void UnSubscribeFromRotationEvents()
    {
        rotatePlatformScript.StartedRotating -= FreezeCharacter;
        rotatePlatformScript.EndedRotating -= UnFreezeCharacter;
    }

    void UnSubscribeFromMovingPlatformEvents()
    {
        movingPlatformScript.StartedMoving -= FreezeCharacter;
        movingPlatformScript.EndedMoving -= UnFreezeCharacter;
    }

    public void Reset()
    {
        rb.velocity = Vector3.zero;
        transform.position = spawnPoint;
        transform.rotation = Quaternion.Euler(spawnRotation);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Platform"))
        {
            transform.parent = originalParent;

            if (movingPlatformScript)
            {
                UnSubscribeFromMovingPlatformEvents();
                movingPlatformScript = null;
            }
            if (rotatePlatformScript)
            {
                UnSubscribeFromRotationEvents();
                rotatePlatformScript = null;
            }

        }
    }
}
