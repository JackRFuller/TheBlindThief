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
        horizontalDirection = 1;
        CreateMovementVector();
    }

    public void MoveLeft()
    {
        horizontalDirection = -1;
        CreateMovementVector();        
    }

    public void ReleaseRight()
    {
        horizontalDirection = 0;

        if (movementVector.x != -1)
            CreateMovementVector();
    }

    public void ReleaseLeft()
    {
        horizontalDirection = 0;

        if (movementVector.x != 1)
            CreateMovementVector();
    }

    #endregion

    #region VerticalInput

    public void MoveUp()
    {
        verticalDirection = 1;
        CreateMovementVector();
    }

    public void MoveDown()
    {
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

    void CreateMovementVector()
    {
        movementVector = new Vector3(horizontalDirection, verticalDirection, 0);
        Debug.Log(movementVector);
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
