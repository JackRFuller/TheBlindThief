using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour, IReset
{
    [Header("Components")]
    [SerializeField]
    protected Collider col;
    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected Animator animController;

    [Header("Movement")]
    [SerializeField]
    protected float sneakSpeed;
    [SerializeField]
    protected float sprintSpeed;
    [SerializeField]
    protected float fallingSpeed;

    protected bool isSprinting;
    protected Vector3 movementDirection;
    protected Vector3 fallingDirection;
    protected float distToGround;
    protected float characterRotation;
    public float CharacterRotation { get { return characterRotation; } }

    [Header("Grounded")]
    [SerializeField]
    protected float distToGroundModifier;
    protected bool isGrounded;
    protected float cooldownFromLanded; //Determines how long to wait until player can move again;
    protected bool canMove = true;
    protected bool hasFinishedRotating = true;   
    protected bool isFalling;
    protected Vector3 startFallingPosition;

    protected bool hasDied;
    protected bool hasStartedMoving;
    protected bool hasMoved;

    //Reset
    protected Vector3 spawnPoint;
    protected Vector3 spawnRotation;
    protected Transform originalParent;

    //Platform Components
    protected RotatePlatformBehaviour rotatePlatformScript;
    protected MovingPlatformBehaviour movingPlatformScript;

    [Header("Debug")]
    [SerializeField]
    protected bool DebugMode;

    protected void OnEnable()
    {
        EventManager.StartListening("Reset", Reset);
    }

    protected void OnDisable()
    {
        EventManager.StopListening("Reset", Reset);
    }

    protected virtual void Start()
    {
        GetStartingPoint();       
    }

    void GetStartingPoint()
    {
        distToGround = col.bounds.extents.y;

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

    public virtual bool CheckIfGrounded()
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

        RaycastHit hit;
        for (int i = 0; i < rays.Length; i++)
        {
            //Checks for Bridges & if Grounded
            if (Physics.Raycast(rays[i], out hit, distToGround + 0.1f))
            {
                numberOfGroundedRays++;
            }
        }

        if (numberOfGroundedRays == 0)
        {
            isGrounded = false;
            return false;
        }
        else
        {            
            isGrounded = true;
            return true;
        }
    }

    public virtual void DetermineFallingDirection()
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

    public virtual void MakeCharacterFall()
    {
        Vector3 movement = fallingDirection;
        rb.velocity = movement * Time.fixedDeltaTime;
    }

    public virtual void GetMovementDirection()
    {

#if UNITY_ANDROID
        movementDirection = ReturnMobileInputVectors();
#endif

#if UNITY_EDITOR
        movementDirection = ReturnInputVectors();
#endif       

    }

    public virtual Vector3 ReturnInputVectors()
    {
        Vector3 inputVector = Vector3.zero;

        float worldRot = ReturnWorldRotation();

        if (characterRotation == 0 || characterRotation == 180)
        {
            inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

            if (characterRotation == 180)
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

        if (characterRotation == 0)
            animController.SetFloat("Movement", inputVector.x);
        if (characterRotation == 180)
            animController.SetFloat("Movement", -inputVector.x);

        if (characterRotation == 90)
            animController.SetFloat("Movement", inputVector.y);
        if (characterRotation == 270)
            animController.SetFloat("Movement", -inputVector.y);      

        return inputVector;
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

    #region FreezingCharacter

    protected virtual void FreezeCharacter()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;
        hasFinishedRotating = false;
    }

    protected virtual void UnFreezeCharacter()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        //Determine New Player Rotation
        characterRotation = ReturnWorldRotation();
        Debug.Log(characterRotation);
        while (!CheckIfGrounded())
        {
            rb.isKinematic = true;
        }

        rb.isKinematic = false;
        hasFinishedRotating = true;

        EventManager.TriggerEvent("CharacterRotated");
    }

#endregion

    #region Subscription

    protected void SubscribeToRotationEvents()
    {
        rotatePlatformScript.StartedRotating += FreezeCharacter;
        rotatePlatformScript.EndedRotating += UnFreezeCharacter;
    }

    protected void SubscribeToMovingPlatformEvents()
    {
        movingPlatformScript.StartedMoving += FreezeCharacter;
        movingPlatformScript.EndedMoving += UnFreezeCharacter;
    }

    protected void UnSubscribeFromRotationEvents()
    {
        rotatePlatformScript.StartedRotating -= FreezeCharacter;
        rotatePlatformScript.EndedRotating -= UnFreezeCharacter;
    }

    protected void UnSubscribeFromMovingPlatformEvents()
    {
        movingPlatformScript.StartedMoving -= FreezeCharacter;
        movingPlatformScript.EndedMoving -= UnFreezeCharacter;
    }

    #endregion

    public virtual void Reset()
    {
        rb.velocity = Vector3.zero;
        transform.position = spawnPoint;
        transform.rotation = Quaternion.Euler(spawnRotation);
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
                if (rotatePlatformScript)
                {
                    UnSubscribeFromRotationEvents();
                    rotatePlatformScript = null;
                }
            }


            if (transform.parent.GetComponent<MovingPlatformBehaviour>())
            {
                if (movingPlatformScript != transform.parent.GetComponent<MovingPlatformBehaviour>())
                {
                    movingPlatformScript = transform.parent.GetComponent<MovingPlatformBehaviour>();
                    SubscribeToMovingPlatformEvents();
                }
            }
            else
            {
                if (movingPlatformScript)
                {
                    UnSubscribeFromMovingPlatformEvents();
                    movingPlatformScript = null;
                }
            }
        }
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
