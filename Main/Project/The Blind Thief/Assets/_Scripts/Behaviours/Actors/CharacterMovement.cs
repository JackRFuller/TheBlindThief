using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField]
    protected float sneakSpeed;
    [SerializeField]
    protected float sprintSpeed;
    [SerializeField]
    protected float fallingSpeed;
    protected Vector3 movementDirection;
    private float distToGround;

    [Header("Grounded")]
    [SerializeField]
    private float distToGroundModifier;

    [Header("Debug")]
    [SerializeField]
    protected bool DebugMode;

    private RotatePlatformBehaviour rotatePlatformScript;

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y * distToGroundModifier;
        Debug.Log(distToGround);
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
        while(!CheckIfGrounded())
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
        if(CheckIfGrounded())
        {
            GetMovementDirection();
        }
        else
        {
            DetermineFallingDirection();
        }
    }

    public virtual void GetMovementDirection()
    {
        movementDirection = ReturnInputVectors();
    }

    void DetermineFallingDirection()
    {
        float characterRotation = ReturnWorldRotation();

        if(characterRotation == 0)
            movementDirection = new Vector3(0,-fallingSpeed,0);

        if (characterRotation == 180)
            movementDirection = new Vector3(0, fallingSpeed, 0);

        if (characterRotation == 90)
            movementDirection = new Vector3(fallingSpeed, 0, 0);

        if (characterRotation == 270)
            movementDirection = new Vector3(-fallingSpeed, 0, 0);
    }

    public virtual Vector3 ReturnInputVectors()
    {
        Vector3 inputVector = Vector3.zero;

        float worldRot = ReturnWorldRotation();

        if (worldRot == 0 || worldRot == 180)
            inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        if (worldRot == 90 || worldRot == 270)
            inputVector = new Vector3(0, Input.GetAxis("Vertical"), 0);

        return inputVector;
    }

    public virtual void FixedUpdate()
    {
        MoveCharacter();
    }

    protected virtual void MoveCharacter()
    {
        Vector3 movement = movementDirection * sneakSpeed;
        rb.velocity = movement * Time.fixedDeltaTime;
    }
    
    void FreezeCharacter()
    {        
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }    

    void UnFreezeCharacter()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;       
    }

    bool CheckIfGrounded()
    {
        Vector3[] raycastPositions = new Vector3[3];
        for(int i = 0; i < raycastPositions.Length; i++)
        {
            raycastPositions[i] = transform.position;
        }
        
        Vector3 directionOfRayCast = Vector3.zero;

        float characterRotation = ReturnWorldRotation();

        if(characterRotation == 0)
        {
            directionOfRayCast = Vector3.down;
            raycastPositions[1] = new Vector3(transform.position.x - 0.25f, transform.position.y, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x + 0.25f, transform.position.y, transform.position.z);
        }
        if(characterRotation == 180)
        {
            directionOfRayCast = Vector3.up;
            raycastPositions[1] = new Vector3(transform.position.x - 0.25f, transform.position.y, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x + 0.25f, transform.position.y, transform.position.z);
        }
        if (characterRotation == 90)
        {
            directionOfRayCast = Vector3.right;
            raycastPositions[1] = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
            raycastPositions[2] = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        }
        if (characterRotation == 270)
        {
            directionOfRayCast = Vector3.left;
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
            if (Physics.Raycast(rays[i],out hit, distToGround))
            {
                if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Bridge"))
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

        if(DebugMode)
            Debug.Log(numberOfGroundedRays);

        if (numberOfGroundedRays == 0)
            return false;
        else return true;  
    }

    public virtual float ReturnWorldRotation()
    {
        float playerRotation = Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.identity));
        playerRotation = Mathf.Round(playerRotation);

        //if (DebugMode)
        //    Debug.Log("Player Rotation: " + playerRotation);

        return playerRotation;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Platform"))
        {
            transform.parent = other.transform.parent.parent;

            //Check for Rotating Platform Component
            if(transform.parent.GetComponent<RotatePlatformBehaviour>())
            {
                if(rotatePlatformScript != transform.parent.GetComponent<RotatePlatformBehaviour>())
                {
                    rotatePlatformScript = transform.parent.GetComponent<RotatePlatformBehaviour>();
                    SubscribeToRotationEvents();
                }
                    
            }
        }
    }

    void SubscribeToRotationEvents()
    {
        rotatePlatformScript.StartedRotating += FreezeCharacter;
        rotatePlatformScript.EndedRotating += UnFreezeCharacter;
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Platform"))
        {
            transform.parent = null;
            rotatePlatformScript = null;
        }
    }
}
