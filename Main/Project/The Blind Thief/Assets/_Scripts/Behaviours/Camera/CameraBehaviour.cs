using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player;
    public Collider target;
    public Collider cameraCollider;

    [Header("Camera Control")]
    [SerializeField] private float movementSpeed;
    private Vector3 originalPosition = Vector3.zero;
    private Vector3 movementVector;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;
    private Vector2 focusPosition;
    private bool hasMovedCamera = false;
    private bool isMovingCamera = false;

    //Lerping Controls
    [SerializeField] private float returnToPlayerPositionSpeed;
    [SerializeField] private AnimationCurve movementCurve;
    private float timeStartedMoving;
    private bool isLerping;
    private Vector3 startPosition;
    private Vector3 endPosition;

    [Header("Camera Focus Area")]
    [SerializeField] private Vector2 cameraFocusAreaSize;
    private CameraFocusArea cameraFocusArea;
    [SerializeField] private bool showCameraFocusBox; 

    [Header("Player Focus Area")]
    [SerializeField] private float verticalOffset;
    [SerializeField] private Vector2 playerFocusAreaSize;
    [SerializeField] private bool showPlayerBoundsBox;
    private PlayerFocusArea playerFocusArea;

    private bool followPlayer = true;
    private bool isPlayerOnMovingPlatform;

    void Start()
    {
        GetPlayer();
        SubscribeToEvents();
        
        cameraFocusArea = new CameraFocusArea(cameraCollider.bounds, cameraFocusAreaSize);
        //Vector2 focusPosition = cameraFocusArea.centre;

        playerFocusArea = new PlayerFocusArea(target.bounds, playerFocusAreaSize);        
    }

    void OnEnable()
    {
        EventManager.StartListening("NewLevel", GetPlayer);
    }

    void OnDisable()
    {
        EventManager.StopListening("NewLevel", GetPlayer);
    }

    void GetPlayer()
    {
        player = PlayerMovementBehaviour.Instance.gameObject.transform;
        target = player.GetComponent<Collider>();
    }

    void SubscribeToEvents()
    {
        PlayerMovementBehaviour.Instance.MovingOnPlatform += PlayerIsOnMovingPlatform;
        PlayerMovementBehaviour.Instance.StoppedMovingOnPlatform += InitiateReturnToPlayer;
    }

    struct PlayerFocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public PlayerFocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y - size.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }


    }

    struct CameraFocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        public float left, right;
        public float top, bottom;

        public CameraFocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.center.y - size.y / 2;
            top = targetBounds.center.y + size.y / 2;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }
        
       
    }

    void Update()
    {
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            MoveCamera();
        }
        else
        {
            if (hasMovedCamera)
            {
                if (!isLerping)
                {
                    InitiateReturnToPlayer();
                }
                else
                {    
                   ReturnToPlayerPosition();
                }
            }
            else
            {
                if(followPlayer)
                    FollowPlayer();
                else
                {
                    if(isLerping)
                        ReturnToPlayerPosition();
                }
            }      
           
        }
    }

    void FollowPlayer()
    {
        if(player)
            transform.position = new Vector3(player.transform.position.x,
                                            player.transform.position.y,
                                            -10);
    }
    

    //void LateUpdate()
    //{
    //    playerFocusArea.Update(target.bounds);

    //    focusPosition = playerFocusArea.centre + Vector2.up * verticalOffset;      

    //    currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

    //    focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
    //    focusPosition += Vector2.right * currentLookAheadX;

    //    if (!isMovingCamera && !isLerping)
    //        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        if (showCameraFocusBox)
        {            
            Gizmos.DrawCube(cameraFocusArea.centre, cameraFocusAreaSize);
        }

        if (showPlayerBoundsBox)
        {
            Gizmos.DrawCube(playerFocusArea.centre, playerFocusAreaSize);
        }
       
    }

    void MoveCamera()
    {        
        originalPosition = (Vector3)focusPosition + Vector3.forward * -10;
        movementVector = Vector3.zero;
        movementVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * movementSpeed;

        //Make sure it isn't outside of the bounds box
        if (transform.position.x < cameraFocusArea.left)
                movementVector.x = 0.01f;
        if (transform.position.x > cameraFocusArea.right)
            movementVector.x = 0.01f; 

        if (transform.position.y > cameraFocusArea.top)
            movementVector.y = 0.01f; 
        if (transform.position.y < cameraFocusArea.bottom)
            movementVector.y = 0.01f; 

        transform.Translate(movementVector * Time.smoothDeltaTime);

        hasMovedCamera = true;
        isMovingCamera = true;

        if (isLerping)
            isLerping = false;     
    }

    void InitiateReturnToPlayer()
    {
        startPosition = transform.position;
        endPosition = new Vector3(player.position.x,
                                  player.position.y,
                                  -10);

        timeStartedMoving = Time.time;
        isLerping = true;
        followPlayer = false;      
    }

    void PlayerIsOnMovingPlatform()
    {
        isPlayerOnMovingPlatform = true;
        followPlayer = false;
    }

    void ReturnToPlayerPosition()
    {
        float _timeSinceStarted = Time.time - timeStartedMoving;
        float _percentageComplete = _timeSinceStarted / returnToPlayerPositionSpeed;

        transform.position = Vector3.Lerp(startPosition, endPosition, movementCurve.Evaluate(_percentageComplete));

        if(_percentageComplete >= 1.0f)
        {
            isLerping = false;
            hasMovedCamera = false;
            isMovingCamera = false;
            followPlayer = true;
            isPlayerOnMovingPlatform = false;
        }
    }
}
