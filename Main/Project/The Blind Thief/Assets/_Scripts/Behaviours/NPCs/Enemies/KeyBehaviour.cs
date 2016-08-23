using UnityEngine;
using System.Collections;

public class KeyBehaviour : InteractableObject
{
    [SerializeField]
    private MeshRenderer mesh;
    [SerializeField]
    private Collider col;

    [Header("Rotation")]
    [SerializeField]
    private Transform meshTransform;
    [SerializeField]
    private Vector3 rotationVector;
    [SerializeField]
    private float rotationSpeed;


    private bool isActive = true;

    void Update()
    {
        if (isActive)
        {
            RotateMesh();
        }
    }

    void RotateMesh()
    {
        meshTransform.Rotate(rotationVector * rotationSpeed * Time.smoothDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
           PlayerAcquiresKey();
        }
    }

    void PlayerAcquiresKey()
    {
        mesh.enabled = false;
        col.enabled = false;
        isActive = false;

        //Increment The Number of Held Keys by 0
        LevelController.Instance.AcquireKey();
    }
}
