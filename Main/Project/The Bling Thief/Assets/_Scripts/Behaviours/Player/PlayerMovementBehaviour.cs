using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementBehaviour : MonoBehaviour
{
    private List<Vector3> nodePositions;

    //Components
    private Rigidbody rb;
    private PlayerAnimationController animController;

    //Mesh
    [SerializeField] private Transform mesh;

    //Movement
    [SerializeField] private float sneakSpeed;
    [SerializeField] private float runSpeed;

    private Vector3 desiredVelocity;
    private float lastSqrMag;
    private Vector3 startingNode;
    private Vector3 targetPosition;

    //Debug
    [SerializeField] private bool DebugMode;

	// Use this for initialization
	void Start ()
	{
	    GetComponents();

	    GetNodeList();

        SubscribeToEvents();
	}

    void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
        animController = GetComponent<PlayerAnimationController>();
    }

    void SubscribeToEvents()
    {
        //When player makes a valid input start movement process
        InputController.PlayerInput += InitiateMovement;
    }

    void GetNodeList()
    {
        nodePositions = NodeController.Instance.NodePositions;
    }
	
	

    void InitiateMovement()
    {
        startingNode = GetClosestNode();

        if(DebugMode)
            Debug.Log("Starting Node: " + startingNode);

        targetPosition = InputController.Instance.TargetPosition;

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
            }
        }
        else
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
            }
        }
    }

    void StartMovement(Vector3 _targetVector)
    {
        Vector3 _directionalVector = (_targetVector - transform.position).normalized*sneakSpeed;
        mesh.LookAt(_targetVector);

        lastSqrMag = Mathf.Infinity;

        desiredVelocity = _directionalVector;
        animController.TurnOnAnimation("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        //Check Sqr Mag
        float sqrMag = (targetPosition - transform.position).sqrMagnitude;

        if (sqrMag > lastSqrMag)
        {
            desiredVelocity = Vector3.zero;
            animController.TurnOnAnimation("isIdle");
        }
            

        lastSqrMag = sqrMag;
    }

    void FixedUpdate()
    {
        rb.velocity = desiredVelocity;
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

            if (nodePositions.Contains(_newNodePosition))
            {
                _numberOfNodesBetween++;
            }
            else
            {
                if (DebugMode)
                    Debug.Log("Path Not Found");
                return false;
            }
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

            if (nodePositions.Contains(_newNodePosition))
            {
                _numberOfNodesInbetween++;
            }
            else
            {
                if (DebugMode)
                    Debug.Log("Path Not Found");
                return false;
            }
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

    int CalculateDifferenceBetweenPlayerAndTarget(int _playerPos, int _targetPos)
    {
        //_playerPos = Mathf.Abs(_playerPos);
        //_targetPos = Mathf.Abs(_targetPos);

        int _difference =   _targetPos - _playerPos;

        _difference = Mathf.Abs(_difference);

        return _difference;
    }

    float GetPlayerRotation()
    {
       float _playerRotation = Mathf.Abs(Quaternion.Angle(transform.rotation, Quaternion.identity));
        _playerRotation = Mathf.Round(_playerRotation);
        return _playerRotation;
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Node")
        {
            transform.parent = other.transform.parent.parent;
        }
    }
}
