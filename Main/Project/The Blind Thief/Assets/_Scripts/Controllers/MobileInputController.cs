using UnityEngine;
using System.Collections;

public class MobileInputController : Singleton<MobileInputController>, IEvent
{
    [Header("Inputs")]
    [SerializeField]
    private GameObject horizontalInputs;
    [SerializeField]
    private GameObject verticalInputs;

    private float horizontalDirection;    
    private float verticalDirection;

    private Vector3 movementVector;
    public Vector3 MovementVector { get { return movementVector; } }

    private int characterIndex = 0; //0 = Accused, 1 = Sage
    private float currentCharacterRotation;

    //Double Click
    private int numberOfTaps;
    private float tapTime;
    private int inputIndex; //1 = Right or Vertical, 0 = Left or Down

    private bool isSprinting;
    public bool IsSprinting { get { return isSprinting; } }

    void Start()
    {
        SetupCharacter();
    }

    public void OnEnable()
    {
        EventManager.StartListening("CharacterRotated", SetupCharacter);
    }

    public void OnDisable()
    {
        EventManager.StopListening("CharacterRotated", SetupCharacter);
    }

    void SetupCharacter()
    {
        GetCharacterOrientation();
        SetInputOrientation();
    }

    void GetCharacterOrientation()
    {
        if (characterIndex == 0)
            currentCharacterRotation = AccusedMovement.Instance.CharacterRotation;

        if (characterIndex == 1)
            currentCharacterRotation = SageMovement.Instance.CharacterRotation;
    }

    void SetInputOrientation()
    {
        if(currentCharacterRotation == 0 || currentCharacterRotation == 180)
        {
            horizontalInputs.SetActive(true);
            verticalInputs.SetActive(false);
        }
        if(currentCharacterRotation == 90 || currentCharacterRotation == 270)
        {
            verticalInputs.SetActive(true);
            horizontalInputs.SetActive(false);
        }
    }

    #region HorizontalInput

    public void MoveRight()
    {
        isSprinting = CheckDoubleTap();

        horizontalDirection = 1;
        CreateMovementVector();
    }

    public void MoveLeft()
    {
        isSprinting = CheckDoubleTap();

        horizontalDirection = -1;
        CreateMovementVector();        
    }

    public void ReleaseRight()
    {
        isSprinting = false;
        horizontalDirection = 0;

        if (movementVector.x != -1)
            CreateMovementVector();
    }

    public void ReleaseLeft()
    {
        isSprinting = false;
        horizontalDirection = 0;

        if (movementVector.x != 1)
            CreateMovementVector();
    }

    #endregion

    #region VerticalInput

    public void MoveUp()
    {
        isSprinting = CheckDoubleTap();

        verticalDirection = 1;
        CreateMovementVector();
    }

    public void MoveDown()
    {
        isSprinting = CheckDoubleTap();

        verticalDirection = -1;
        CreateMovementVector();
    }

    public void ReleaseUp()
    {
        verticalDirection = 0;
        if (verticalDirection != -1)
            CreateMovementVector();
    }

    public void ReleaseDown()
    {
        verticalDirection = 0;
        if (verticalDirection != 1)
            CreateMovementVector();
    }

    #endregion

    bool CheckDoubleTap()
    {
        numberOfTaps++;

        if (numberOfTaps == 1)
            tapTime = Time.time;

        if (numberOfTaps > 1 && Time.time - tapTime < 0.5f)
        {
            numberOfTaps = 0;
            tapTime = 0;
            Debug.Log("Double Tap");
            return true;           
        }
        else if (numberOfTaps > 2 || Time.time - tapTime > 1)
        {
            numberOfTaps = 0;
        }

        return false;
    }

    void CreateMovementVector()
    {
        movementVector = new Vector3(horizontalDirection, verticalDirection, 0);       
    }

    public void OnClickSwitchCharcaters()
    {
        EventManager.TriggerEvent("CharacterSwitch");

        characterIndex++;
        if (characterIndex > 1)
            characterIndex = 0;

        SetupCharacter();
    }


}
