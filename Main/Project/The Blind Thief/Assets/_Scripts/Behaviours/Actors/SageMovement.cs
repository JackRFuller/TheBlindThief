using UnityEngine;
using System.Collections;

public class SageMovement : CharacterMovement
{
    private static SageMovement instance;
    public static SageMovement Instance { get { return instance; } }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (CheckIfGrounded())
        {
            if (canMove)
            {
                GetMovementDirection();
            }
        }
        else
        {
            if (!isFalling)
                if (hasFinishedRotating)
                    DetermineFallingDirection();
        }
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            MoveCharacter();
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

        if (isSprinting)
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
