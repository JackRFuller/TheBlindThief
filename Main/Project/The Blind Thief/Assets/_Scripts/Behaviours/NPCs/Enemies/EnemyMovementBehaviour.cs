using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovementBehaviour : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private EnemyAnimationController enemyAnim;
    [SerializeField] private Transform enemyMesh;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    private Vector3 startingPosition;
    private Vector3[] targets = new Vector3[2];  
    private Vector3 currentTarget;
    private Vector3 desiredVelocity;
    private int target;
    private float lastSqrMag;
    private bool isMoving;

    //Nodes
    private List<Vector3> nodePositions;

    //Components
    private Rigidbody rb;

    //Misc
    [SerializeField] private bool DebugMode;

    void Start()
    {
        GetNodeList();
        GetComponents();
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
        else
        {
            //Get Up Extreme

            //Get Down Extreme
        }

        //Choose which one to start towards
        target = Random.Range(0, 2);
        currentTarget = targets[target];

        //Move towards it
        StartMovement();
    }

    void StartMovement()
    {
        Vector3 directionalVector = (currentTarget - transform.position).normalized * movementSpeed;
        lastSqrMag = Mathf.Infinity;
        desiredVelocity = directionalVector;
        isMoving = true;

        enemyMesh.LookAt(GetLookAtTarget());

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
                isMoving = false;
                desiredVelocity = Vector3.zero;
                rb.velocity = desiredVelocity;
                enemyAnim.TurnOnAnimation("isIdle");
                //End Movement                
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

        if(enemyRotation == 0 || enemyRotation == 180)
        {
            lookAtDirection = new Vector3(currentTarget.x,
                                          enemyMesh.position.y,
                                          currentTarget.z);
        }

        return lookAtDirection;
    }

    Vector3 GetExtremeLeftX()
    {
        Vector3 extremeX = Vector3.zero;

        Vector3 nextNodeAlong = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);

        for (int i = 0; i < nodePositions.Count; i++)
        {
            nextNodeAlong.x -= (i + 1);

            if (nodePositions.Exists(d => d == nextNodeAlong))
            {
                extremeX = nextNodeAlong;
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
            nextNodeAlong.x += (i + 1);

            if (nodePositions.Exists(d => d == nextNodeAlong))
            {
                extremeX = nextNodeAlong;
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
	
}
