using UnityEngine;
using System.Collections;

public class EnemyProjectileBehaviour : MonoBehaviour
{
    private Rigidbody rb;

    private float projectileSpeed;
    public float ProjectileSpeed { get { return projectileSpeed; } set { projectileSpeed = value; } }
    private float maxProjectileDistance; //How far the projectile can travel
    public float MaxProjectileDistance { get { return maxProjectileDistance; } set { maxProjectileDistance = value; } }

    private Vector3 targetDirection;
    private Vector3 playerPosition;
    public Vector3 PlayerPosition { get { return playerPosition; } set { playerPosition = value; } }
    private Vector3 originalPosition;
    private Transform originalParent;
    public Transform OriginalParent { get { return originalParent; } set { originalParent = value; } }

    private bool isMoving; 

    void Start()
    {
        Init();
    }

    void Init()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        InitiateProjectile();
    }

    void InitiateProjectile()
    {
        originalPosition = transform.position;
        targetDirection = playerPosition - originalPosition;

        isMoving = true;
    }

    void FixedUpdate()
    {
        if (isMoving)
            MoveProjectile();
    }

    void MoveProjectile()
    {
        rb.velocity = projectileSpeed * targetDirection * Time.deltaTime;
        float distanceTraveled = Vector3.Distance(originalPosition, transform.position);

        if(distanceTraveled >= maxProjectileDistance)
        {
            ResetProjectile();
        }
    }

    void ResetProjectile()
    {
        rb.velocity = Vector3.zero;

        transform.parent = originalParent;
        transform.localPosition = Vector3.zero;

        isMoving = false;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            Debug.Log("Projectile Hit Player");
        }
    }
	
}
