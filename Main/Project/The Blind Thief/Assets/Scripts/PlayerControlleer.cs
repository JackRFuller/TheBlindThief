using UnityEngine;
using System.Collections;

public class PlayerControlleer : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 6;

    private Rigidbody rb;
    private Camera mainCamera;

    Vector3 velocity;

	// Use this for initialization
	void Start ()
    {
        Init();
	}

    void Init()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
	}

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    void Move()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);

        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
    }
}
