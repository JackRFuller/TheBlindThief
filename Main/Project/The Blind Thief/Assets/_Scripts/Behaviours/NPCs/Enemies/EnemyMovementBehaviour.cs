using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovementBehaviour : MonoBehaviour, IReset
{
    //Setup
    private Vector3 spawnPosition;
    private Quaternion startingMeshRotation;

    [SerializeField] private EnemyBehaviour enemyBehaviour;
    [SerializeField] private FieldOfView fieldOfView;

    [Header("Animation")]
    [SerializeField] private EnemyAnimationController enemyAnim;
    [SerializeField] private Transform enemyMesh;

    [Header("Movement")]
    [SerializeField] private Collider characterCollider;
    [SerializeField] private float movementSpeed;
    private Vector3 startingPosition;
    private Vector3[] targets = new Vector3[2];  
    private Vector3 currentTarget;
    private Vector3 desiredVelocity;
    private int target;
    private float lastSqrMag;
    private bool isMoving;

    //Subscribing to Dynamic Platforms
    private MovingPlatformBehaviour mpbScript;
    private RotatePlatformBehaviour rpbScript;
    private string platformName;
    private Transform currentPlatform;

    //Nodes
    private List<Vector3> nodePositions;

    //Components
    private Rigidbody rb;

    //Misc
    [SerializeField] private bool DebugMode;

    void Start()
    {
        GetStartingPoint();
        SubscribeToEvents();
        GetComponents();
        InitiateMovement();
    }    

    void OnEnable()
    {
        EventManager.StartListening("Reset", Reset);
    }

    void OnDisable()
    {
        EventManager.StopListening("Reset", Reset);
    }

    void GetStartingPoint()
    {
        spawnPosition = transform.position;
        startingMeshRotation = enemyMesh.rotation;
    }

    void SubscribeToEvents()
    {
        PathController.Instance.ReEvaluate += InitiateMovement;
        enemyBehaviour.Attacking += PlayAttackAnimation;
        fieldOfView.FinishedFOV += StartMovement;        
    }

    void InitiateMovement()
    {       
        //while(NodeController.Instance.IsGettingNodes)
        //{
        //    yield return new WaitForSeconds(1.0f);
        //}

        GetNodeList();
        characterCollider.enabled = true;             
        FindTargetPaths();
    }

    /// <summary>
    /// Gets Node List off Node Controller
    /// </summary>
    void GetNodeList()
    {
        nodePositions = NodeController.Instance.NodePositions;
    }

    void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Finds the Two Extremes on the Path
    /// </summary>
    void FindTargetPaths()
    {
        //Get Starting Position
        startingPosition = GetClosestNode();

        if (DebugMode)
            Debug.Log("<color=yellow>Starting Node: </color>" + startingPosition);

        //Work out Rotation
        float enemyRotation = GetEnemyRotation();

        //Work out two extremes - left/right or up/down
        if(enemyRotation == 0 || enemyRotation == 180)
        {
            //Get Left Extreme
            targets[0] = GetExtremeLeftX();

            if (DebugMode)
                Debug.Log("<color=yellow>Extreme Left: </color>" + targets[0]);

            //Get Right Extreme
            targets[1] = GetExtremeRightX();

            if (DebugMode)
                Debug.Log("<color=yellow>Extreme Right: </color>" + targets[1]);

            //Take Height Into Account
            targets[0].y = transform.position.y;
            targets[1].y = transform.position.y;

        }
        else if(enemyRotation == 90 || enemyRotation == 270)
        {
            //Get Up Extreme
            targets[0] = GetExtremeUpY();

            if (DebugMode)
                Debug.Log("<color=yellow>Extreme Up: </color>" + targets[0]);

            //Get Down Extreme
            targets[1] = GetExtremeDownY();

            if (DebugMode)
                Debug.Log("<color=yellow>Extreme Down: </color>" + targets[1]);

            //Take X Into Account
            targets[0].x = transform.position.x;
            targets[1].x = transform.position.x;
        }

        //Check that the enemy isn't already moving
        if(rb.velocity == Vector3.zero)
        {
            //Choose which one to start towards
            target = Random.Range(0, 2);
            currentTarget = targets[target];
        }
        else
        {
            currentTarget = targets[target];
        }

        //Move towards it
        StartMovement();
    }

    void StartMovement()
    {
        characterCollider.enabled = true;
        Vector3 directionalVector = (currentTarget - transform.position).normalized * movementSpeed;
        lastSqrMag = Mathf.Infinity;
        desiredVelocity = directionalVector;
        isMoving = true;

        //enemyMesh.LookAt(GetLookAtTarget());
        enemyMesh.localPosition = Vector3.zero;
        enemyMesh.localRotation = Quaternion.Euler(GetLookAtTarget());

        //Trigger Animation For Walking
        enemyAnim.TurnOnAnimation("isWalking");
    }

    void Update()
    {
        if (isMoving)
        {
            float sqrMag = (currentTarget - transform.position).sqrMagnitude;

            if (sqrMag > lastSqrMag)
            {
                //End Movement    
                EndMovement();            
                StartCoroutine(StopMovement());
            }

            lastSqrMag = sqrMag;
        }
    }

    void FixedUpdate()
    {
        if(isMoving)
            rb.velocity = desiredVelocity;
    }

    void PlayAttackAnimation()
    {
        isMoving = false;
        desiredVelocity = Vector3.zero;
        rb.velocity = desiredVelocity;        
    }

    void EndMovement()
    {
        characterCollider.enabled = false;
        isMoving = false;
        desiredVelocity = Vector3.zero;
        rb.velocity = desiredVelocity;
        enemyAnim.TurnOnAnimation("isIdle");
    }

    /// <summary>
    /// Waits a certain amount of time before restarting movement
    /// </summary>
    /// <returns></returns>
    IEnumerator StopMovement()
    {              
        yield return new WaitForSeconds(1.0f);
        SwitchTargets();
    }

    /// <summary>
    /// Switches Path Targets
    /// </summary>
    void SwitchTargets()
    { 
        target++;
        if (target > 1)
            target = 0;

        currentTarget = targets[target];

        if (DebugMode)
            Debug.Log("<color=yellow>Current Target: </color>" + currentTarget);

        StartMovement();
    }

    Vector3 GetLookAtTarget()
    {
        Vector3 lookAtDirection = Vector3.zero;

        float enemyRotation = GetEnemyRotation();

        //Horizontal
        if (enemyRotation == 0)
        {
            if (currentTarget.x > transform.position.x)
                lookAtDirection = new Vector3(0, 90, 0);
            else
                lookAtDirection = new Vector3(0, -90, 0);
        }
        else if (enemyRotation == 180)
        {
            if (currentTarget.x > transform.position.x)
                lookAtDirection = new Vector3(0, -90, 0);
            else
                lookAtDirection = new Vector3(0, 90, 0);
        }
        if(enemyRotation == 90)
        {
            if (currentTarget.y > transform.position.y)
                lookAtDirection = new Vector3(0, 90, 0);
            else
                lookAtDirection = new Vector3(0, -90, 0);
        }
        if (enemyRotation == 270)
        {
            if (currentTarget.y > transform.position.y)
                lookAtDirection = new Vector3(0, 90, 0);
            else
                lookAtDirection = new Vector3(0, -90, 0);
        }



        //if(enemyRotation == 0 || enemyRotation == 180)
        //{
        //    lookAtDirection = new Vector3(currentTarget.x,
        //                                  enemyMesh.position.y,
        //                                  currentTarget.z);
        //}

        //if(enemyRotation == 90 || enemyRotation == 270)
        //{
        //    lookAtDirection = currentTarget;
        //    lookAtDirection.x = enemyMesh.position.x;
        //    lookAtDirection.z = enemyMesh.position.z;                                          
        //}

        return lookAtDirection;
    }

    Vector3 GetExtremeDownY()
    {
        Vector3 extremeY = Vector3.zero;

        Vector3 nextNodeAlong = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);

        for (int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 nextNode = nextNodeAlong;
            nextNode.y -= i;

            if (nodePositions.Exists(d => d == nextNode))
            {
                extremeY = nextNode;
            }
            else
            {
                return extremeY;
            }
        }

        Debug.Log("End of the Line");
        return extremeY;
    }

    Vector3 GetExtremeUpY()
    {
        Vector3 extremeY = Vector3.zero;

        Vector3 nextNodeAlong = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);
        
        for(int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 nextNode = nextNodeAlong;
            nextNode.y += i;           

            if (nodePositions.Exists(d => d == nextNode))
            {
                extremeY = nextNode;
            }
            else
            {
                return extremeY;
            }
        }

        return extremeY;
    }

    Vector3 GetExtremeLeftX()
    {
        Vector3 extremeX = Vector3.zero;

        Vector3 nextNodeAlong = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);

        for (int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 nextNode = nextNodeAlong;
            nextNode.x -= i;            

            if (nodePositions.Exists(d => d == nextNode))
            {
                extremeX = nextNode;
            }
            else
            {
                return extremeX;
            }
        }

        return extremeX;
    }

    Vector3 GetExtremeRightX()
    {
        Vector3 extremeX = Vector3.zero;

        Vector3 nextNodeAlong = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);
        
               
        for (int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 nextNode = nextNodeAlong;
            nextNode.x += i;  

            if (nodePositions.Exists(d => d == nextNode))
            {
                extremeX = nextNode;
            }
            else
            {
                return extremeX;
            }
        }

        return extremeX;
    }

    /// <summary>
    /// Returns a Position & Rounded Version of the Enemy's Rotation
    /// </summary>
    /// <returns></returns>
    float GetEnemyRotation()
    {
        float enemyRotation = Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.identity));
        enemyRotation = Mathf.Round(enemyRotation);

        return enemyRotation;
    }

    /// <summary>
    /// Gets Closest Node - Used for Starting Position
    /// </summary>
    /// <returns></returns>
    Vector3 GetClosestNode()
    {
        Vector3 initialNodePosition = nodePositions[0];

        float dist = Vector3.Distance(transform.position, initialNodePosition);

        for(int i = 0; i < nodePositions.Count; i++)
        {
            float newDist = Vector3.Distance(transform.position, nodePositions[i]);

            if(newDist < dist)
            {
                dist = newDist;
                initialNodePosition = nodePositions[i];
            }
        }

        return initialNodePosition;

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Node")
        {
            transform.parent = other.transform.parent.parent;

            if(currentPlatform == null || currentPlatform != other.transform.parent.parent)
            {
                currentPlatform = other.transform.parent.parent;        
                
                if(currentPlatform.GetComponent<RotatePlatformBehaviour>() || currentPlatform.GetComponent<MovingPlatformBehaviour>())
                {
                    //Check if it's rotating
                    if (currentPlatform.GetComponent<RotatePlatformBehaviour>())
                    {
                        //Unsubscribe from previous rotating platform
                        UnSubscribeFromEvents();

                        rpbScript = currentPlatform.GetComponent<RotatePlatformBehaviour>();

                        rpbScript.EndedRotating += InitiateMovement;
                        rpbScript.StartedRotating += EndMovement;

                        if (DebugMode)
                            Debug.Log("<color=yellow>Got Rotating Platform Component</color>");

                    } //or if it is moving
                    if (currentPlatform.GetComponent<MovingPlatformBehaviour>())
                    {
                        // Unsubscribe from previous moving platform
                        UnSubscribeFromEvents();

                        mpbScript = currentPlatform.GetComponent<MovingPlatformBehaviour>();

                        mpbScript.StartedMoving += EndMovement;
                        mpbScript.EndedMoving += InitiateMovement;

                        if (DebugMode)
                            Debug.Log("<color=yellow>Got Moving Platform Component</color>");
                    }
                }
                else
                {
                    UnSubscribeFromEvents();
                }
            }
        }
    }    

    void UnSubscribeFromEvents()
    {
        //Rotating Platform
        if (rpbScript != null)
        {
            rpbScript.EndedRotating -= InitiateMovement;
            rpbScript.StartedRotating -= EndMovement;
        }

        //Moving Platform
        if(mpbScript != null)
        {
            mpbScript.StartedMoving -= EndMovement;
            mpbScript.EndedMoving -= InitiateMovement;
        }
    }

    public void Reset()
    {
        EndMovement();
        transform.position = spawnPosition;
        enemyMesh.rotation = startingMeshRotation;
        InitiateMovement();
    }



}
