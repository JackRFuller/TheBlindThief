using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementBehaviour : Singleton<PlayerMovementBehaviour>
{
    private List<Vector3> nodePositions;

    //Components
    private Rigidbody rb;
    private PlayerAnimationController animController;
   
    [SerializeField] private Collider playerCollider;

    //Mesh
    private Transform mesh;    

    //Movement
    [SerializeField] private float sneakSpeed;
    [SerializeField] private float runSpeed;
    private bool isSprinting;
    public bool IsSprinting
    {
        get { return isSprinting; }
    }

    private Vector3 desiredVelocity;
    private float lastSqrMag;
    private Vector3 startingNode;
    private Vector3 targetPosition;

    //Debug
    [SerializeField] private bool DebugMode;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip legitPathSFX;

    public static event Action HasFoundLegitPath;

    private bool isDead;

    private bool isAlreadyMoving; //Used to check if the player is already moving and stop certain effects

    //Platform Relationship
    private bool isMovingOnPlatform; //Stops Registering Player Input if the Character is on a moving platform
    private Transform currentPlatform;
    private RotatingPlatformBehaviour rpbScript;
    private MovingPlatformBehaviour mpbScript;

	// Use this for initialization
	void Start ()
	{
	    GetComponents();

	    GetNodeList();

        SubscribeToEvents();
	}

    void GetComponents()
    {
        animController = PlayerSetupBehaviour.Instance.ActiveMesh.GetComponent<PlayerAnimationController>();
        mesh = PlayerSetupBehaviour.Instance.ActiveMesh.transform;

        rb = GetComponent<Rigidbody>();       
    }

    void SubscribeToEvents()
    {
        PathController.Instance.ReEvaluate += CheckIfPlayerIsMoving;

        //When player makes a valid input start movement process
        InputController.PlayerInput += InitiateMovement;
    }

    void GetNodeList()
    {
        nodePositions = NodeController.Instance.NodePositions;
    }

    /// <summary>
    /// Used to Revaluate CHaracter Paths
    /// </summary>
    void CheckIfPlayerIsMoving()
    {
        if(rb.velocity != Vector3.zero)
        {
            InitiateMovement();
        }
    }

    void InitiateMovement()
    {
        if (isMovingOnPlatform)
            return;

        startingNode = GetClosestNode();

        if(DebugMode)
            Debug.Log("Starting Node: " + startingNode);

        targetPosition = InputController.Instance.TargetPosition;

        if(!IsOnSamePlaneAsPlayer())
            return;

        float _playerRotation = GetPlayerRotation();

        if (_playerRotation == 0 || _playerRotation == 180)
        {
            if (IsThereAHorizontalPath())
            {
                Vector3 _targetPosition = new Vector3(targetPosition.x,transform.position.y,transform.position.z);
                StartMovement(_targetPosition);
            }
            else
            {
                if(DebugMode)
                    Debug.Log("No Valid Path");

                TurnOffMovement();
            }
        }
        else if (_playerRotation == 90 || _playerRotation == 270)
        {
            if (isThereAVerticalPath())
            {   
                Vector3 _targetPosition = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
                StartMovement(_targetPosition);
            }
            else
            {
                if (DebugMode)
                    Debug.Log("No Valid Path");

                TurnOffMovement();
            }
        }
    }

    void StartMovement(Vector3 _targetVector)
    {
        Vector3 _directionalVector = Vector3.zero;

        if (InputController.Instance.DoubleClicked)
        {
            _directionalVector = (_targetVector - transform.position).normalized*runSpeed;
            isSprinting = true;
        }

        else
        {
            _directionalVector = (_targetVector - transform.position).normalized * sneakSpeed;
            isSprinting = false;
        }          

        if (DebugMode)
            Debug.Log("Desired Speed: " + _directionalVector);

        //Event - Subscribed to by Waypoint
        if (HasFoundLegitPath != null)
            HasFoundLegitPath();

        //Calculate Mesh Look Direction
        Vector3 _targetLook = CalculateMeshLookAtVector(_targetVector);

        mesh.localRotation = Quaternion.Euler(_targetLook);

        lastSqrMag = Mathf.Infinity;

        desiredVelocity = _directionalVector;

        if (!isAlreadyMoving)
        {
            audioSource.clip = legitPathSFX;
            audioSource.Play();            

            if (InputController.Instance.DoubleClicked)
            {
                animController.TurnOnAnimation("isSprinting");
            }
            else
            {
                animController.TurnOnAnimation("isWalking");
            }

            isAlreadyMoving = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (isMovingOnPlatform)
                return;

            //Check Sqr Mag
            float sqrMag = (targetPosition - transform.position).sqrMagnitude;

            if (sqrMag > lastSqrMag)
            {
                TurnOffMovement();                    
            }


            lastSqrMag = sqrMag;
        }       
    }   

    void FixedUpdate()
    {
        if (isMovingOnPlatform)
            return;

        rb.velocity = desiredVelocity;
    }

    /// <summary>
    /// Used to Subscribe to Moving/Rotating Platforms - Triggered when the platform starts moving
    /// </summary>
    void PlayerIsOnMovingPlatfrom()
    {
        isMovingOnPlatform = true;
        playerCollider.enabled = false;
        TurnOffMovement();
    }

    void PlatformHasStoppedMoving()
    {
        isMovingOnPlatform = false;
        playerCollider.enabled = true;
    }

    void TurnOffMovement()
    {
        desiredVelocity = Vector3.zero;
        animController.TurnOnAnimation("isIdle");
        isAlreadyMoving = false;
    }

    void HitByEnemy()
    {
        desiredVelocity = Vector3.zero;
        isDead = true;
        animController.TurnOnAnimation("isDead");
    }

    bool isThereAVerticalPath()
    {
        //Get Initial Values
        int _playerY = Mathf.RoundToInt(transform.position.y);

        if(DebugMode)
            Debug.Log("Player Y: " + _playerY);


        int _targetY = Mathf.RoundToInt(targetPosition.y);

        if (DebugMode)
            Debug.Log("Target Y: " + _targetY);

        bool isGoingDown;

        //Check if the player is going Up or down
        if (_targetY > _playerY)
        {
            isGoingDown = false;
        }
        else
        {
            isGoingDown = true;
        }

        if (DebugMode)
            Debug.Log("Is Going Down: " + isGoingDown);

        int _difference = Mathf.Abs(CalculateDifferenceBetweenPlayerAndTarget(_playerY, _targetY));

        if (DebugMode)
            Debug.Log("Difference Between Player and Target: " + _difference);

        int _numberOfNodesBetween = 0;

        for (int i = 0; i < _difference; i++)
        {
            int _newY = _playerY;

            if (isGoingDown)
                _newY += (-i - 1);
            else _newY += (i + 1);

            if(DebugMode)
                Debug.Log("New Y Position: " +_newY);

            Vector3 _newNodePosition = new Vector3(startingNode.x,_newY,startingNode.z);

            if(DebugMode)
                Debug.Log("New Node Position: " + _newNodePosition);

            if (nodePositions.Exists(d => d == _newNodePosition))
            {
                _numberOfNodesBetween++;
            }
            else
            {
                if (DebugMode)
                    Debug.Log("Path Not Found");
            }

            //if (nodePositions.Contains(_newNodePosition))
            //{
            //    _numberOfNodesBetween++;
            //}
            //else
            //{
            //    if (DebugMode)
            //        Debug.Log("Path Not Found");
            //    return false;
            //}
        }

        if (DebugMode)
            Debug.Log("Number of Nodes Between: " + _numberOfNodesBetween);

        if (_numberOfNodesBetween == _difference)
        {
            if (DebugMode)
                Debug.Log("Path Found");
            return true;
        }
        else
        {
            if (DebugMode)
                Debug.Log("Path Not Found");
            return false;
        }

    }

    //Calculate Horizontal Path
    bool IsThereAHorizontalPath()
    {

        //Get InitialValues
        int _playerX = Mathf.RoundToInt(transform.position.x);

        if(DebugMode)
            Debug.Log("Player X: " + _playerX);

        int _targetX = Mathf.RoundToInt(targetPosition.x);

        if (DebugMode)
            Debug.Log("Target X: " + _targetX);

        if (DebugMode)
            Debug.Log("Got Initial Values");

        //Check if going left or right
        bool isGoingRight;

        if (_targetX > _playerX)
        {
            isGoingRight = true;

            if(DebugMode)
                Debug.Log("Going Right");
        }
        else
        {
            isGoingRight = false;
            if (DebugMode)
                Debug.Log("Going Left");
        }

        int _difference = Mathf.Abs(CalculateDifferenceBetweenPlayerAndTarget(_playerX, _targetX));

        if(DebugMode)
            Debug.Log("Difference Between Player and Target: " + _difference);

        int _numberOfNodesInbetween = 0;

        for (int i = 0; i < _difference; i++)
        {
            int _newX = _playerX;

            if (isGoingRight)
                _newX += (i + 1);
            else _newX += (-i - 1);

            if(DebugMode)
                Debug.Log("New X Position: " +_newX);

            Vector3 _newNodePosition = new Vector3(_newX,startingNode.y,startingNode.z);

            if (nodePositions.Exists(d => d == _newNodePosition))
            {
                _numberOfNodesInbetween++;
            }
            else
            {
                if(DebugMode)
                    Debug.Log("Path Not Found");
            }

            //if (nodePositions.Contains(_newNodePosition))
            //{
            //    _numberOfNodesInbetween++;
            //}
            //else
            //{
            //    if (DebugMode)
            //        Debug.Log("Path Not Found");
            //    return false;
            //}
        }

        if(DebugMode)
            Debug.Log("Number of Nodes Between: " + _numberOfNodesInbetween);

        if (_numberOfNodesInbetween == _difference)
        {
            if(DebugMode)
                Debug.Log("Path Found");
            return true;
        }
        else
        {
            if (DebugMode)
                Debug.Log("Path Not Found");
            return false;
        }
        
        
    }

    Vector3 CalculateMeshLookAtVector(Vector3 _targetPosition)
    {
       //Get Player Rotation - Used for 0/180 Rotation
        float _playerRot = GetPlayerRotation();

        Vector3 _newRotation = mesh.rotation.eulerAngles;

        //Used for 90/270 Rotation
        Quaternion rot = new Quaternion();
        rot = Quaternion.Euler(transform.eulerAngles);
        float _zAxis = rot.eulerAngles.z;

        if (_playerRot == 0)
        {
            if (_targetPosition.x > transform.position.x)
            {
                _newRotation = new Vector3(0, 90, 0);
            }
            else
            {
                _newRotation = new Vector3(0, 270, 0);
            }
        }
        else if (_playerRot == 180)
        {
            if (_targetPosition.x > transform.position.x)
            {
                _newRotation = new Vector3(0, 270, 0);
            }
            else
            {
                _newRotation = new Vector3(0, 90, 0);
            }
        }
        else if (_zAxis == 90)
        {
            if (_targetPosition.y > transform.position.y)
            {
                _newRotation = new Vector3(0, 90, 0);
            }
            else
            {
                _newRotation = new Vector3(0, 270, 0);
            }
        }
        else if (_zAxis == 270)
        {
            if (_targetPosition.y > transform.position.y)
            {
                _newRotation = new Vector3(0, 270, 0);
            }
            else
            {
                _newRotation = new Vector3(0, 90, 0);
            }
        }
        return _newRotation;
    }

    int CalculateDifferenceBetweenPlayerAndTarget(int _playerPos, int _targetPos)
    {
        //_playerPos = Mathf.Abs(_playerPos);
        //_targetPos = Mathf.Abs(_targetPos);

        int _difference =   _targetPos - _playerPos;

        _difference = Mathf.Abs(_difference);

        return _difference;
    }

    //Gets the Positive & Rounds the Player's Rotation
    float GetPlayerRotation()
    {
       float _playerRotation = Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.identity));
        _playerRotation = Mathf.Round(_playerRotation);

        if(DebugMode)
            Debug.Log("Player Rotation: " + _playerRotation);
        return _playerRotation;
    }

    //Identify Which is the Closest Node to the player
    Vector3 GetClosestNode()
    {
        Vector3 _initialNodePosition = nodePositions[0];
        float _dist = Vector3.Distance(transform.position, _initialNodePosition);

        for (int i = 0; i < nodePositions.Count; i++)
        {
            float _newDist = Vector3.Distance(transform.position, nodePositions[i]);

            if (_newDist < _dist)
            {
                _dist = _newDist;
                _initialNodePosition = nodePositions[i];
            }
        }

        return _initialNodePosition;
    }

    //Work Out If the Input Is On the Same Plane as the Player
    bool IsOnSamePlaneAsPlayer()
    {
        //Get Target Position
        Vector3 _targetPos = targetPosition;

        //Get Player Position
        Vector3 _playerPos = transform.position;

        //Work Out Player's Rotation
        float _playerRot = GetPlayerRotation();

        if (_playerRot == 0 || _playerRot == 180)
        {
            float _playerY = _playerPos.y;
            float _targetY = _targetPos.y;

            if (_playerY <= _targetY + 1.5 && _playerY >= _targetY - 1.5)
                return true;
            else
            {
                return false;
            }
        }
        else if(_playerRot == 90 || _playerRot == 270)
        {
            float _playerX = _playerPos.x;
            float _targetX = _targetPos.x;

            if (_playerX <= _targetX + 1.5f && _playerX >= _targetX - 1.5f)
                return true;
            else
            {
                return false;
            }
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Node")
        {
            transform.parent = other.transform.parent.parent;

            if(currentPlatform == null || currentPlatform != other.transform.parent.parent)
            {
                currentPlatform = other.transform.parent.parent;

                if(currentPlatform.GetComponent<RotatingPlatformBehaviour>() || currentPlatform.GetComponent<MovingPlatformBehaviour>())
                {
                    if (currentPlatform.GetComponent<RotatingPlatformBehaviour>())
                    {
                        UnSubscribeFromEvents();

                        rpbScript = currentPlatform.GetComponent<RotatingPlatformBehaviour>();

                        rpbScript.EndedRotating += PlatformHasStoppedMoving;
                        rpbScript.StartedRotating += PlayerIsOnMovingPlatfrom;
                    }

                    if (currentPlatform.GetComponent<MovingPlatformBehaviour>())
                    {
                        UnSubscribeFromEvents();

                        mpbScript = currentPlatform.GetComponent<MovingPlatformBehaviour>();

                        mpbScript.StartedMoving += PlayerIsOnMovingPlatfrom;
                        mpbScript.EndedMoving += PlatformHasStoppedMoving;
                    }
                }

            }
        }
    }

    void UnSubscribeFromEvents()
    {
        if (rpbScript != null)
        {
            rpbScript.EndedRotating -= PlatformHasStoppedMoving;
            rpbScript.StartedRotating -= PlayerIsOnMovingPlatfrom;
        }

        if(mpbScript != null)
        {
            mpbScript.StartedMoving -= PlayerIsOnMovingPlatfrom;
            mpbScript.EndedMoving -= PlatformHasStoppedMoving;
        }
    }
}
