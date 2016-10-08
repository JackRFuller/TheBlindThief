using UnityEngine;
using System.Collections;

public class AccusedMovement : CharacterMovement
{
    private static AccusedMovement instance;
    public static AccusedMovement Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (CheckIfGrounded())
        {
            if (animController.GetBool("Falling"))
            {
                Falling();
                StartCoroutine(WaitForLandingCooldown());
            }
            if (canMove)
            {
                GetMovementDirection();
            }
        } 
        else
        {            
            if(!isFalling)
                if(hasFinishedRotating)
                    DetermineFallingDirection();
        }
    }       

    public override Vector3 ReturnMobileInputVectors()
    {
        Vector3 inputVector = MobileInputController.Instance.MovementVector;
        isSprinting = MobileInputController.Instance.IsSprinting;

        animController.SetBool("Sprinting", isSprinting);
        animController.SetBool("Walking", !isSprinting);

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

    void FixedUpdate()
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

    void Falling()
    {
        canMove = false;
        animController.SetBool("Falling", false);
        animController.SetBool("Landing", true);
    }      

    IEnumerator WaitForLandingCooldown()
    {
        yield return new WaitForSeconds(cooldownFromLanded);
        canMove = true;
        isFalling = false;
        animController.SetBool("Landed", true);
    }
}
