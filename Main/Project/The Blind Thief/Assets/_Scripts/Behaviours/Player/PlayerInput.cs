using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 movementVector;

    [Header("Movement")]
    [SerializeField]
    private float sneakSpeed;
    [SerializeField]
    private float sprintSpeed;

	// Use this for initialization
	void Start ()
    {
        GetComponents();
	}

    void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        movementVector = ReturnHorizontalVector();

        if (Input.GetKey(KeyCode.LeftShift))
            movementVector *= sprintSpeed;
        else
            movementVector *= sneakSpeed;
	}

    Vector3 ReturnHorizontalVector()
    {
        Vector3 horizontalMovement = new Vector3(Input.GetAxis("Horizontal"),0,0);
        return horizontalMovement;
    }

    void FixedUpdate()
    {
        rb.velocity = movementVector;
    }
}
