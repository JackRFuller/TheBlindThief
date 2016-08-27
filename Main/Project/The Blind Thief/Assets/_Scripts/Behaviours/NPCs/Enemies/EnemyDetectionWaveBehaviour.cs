using UnityEngine;
using System.Collections;

public class EnemyDetectionWaveBehaviour : MonoBehaviour
{
    private Vector3 originalSize;
    private Transform originalParent;
    public Transform OriginalParent { get { return originalParent; } set { originalParent = value; } }

    private float waveGrowthSpeed; //Defines how quickly the wave grows
    public float WaveGrowthSpeed { get { return waveGrowthSpeed; } set { waveGrowthSpeed = value; } }
    private float maxWaveSize; // Defines the maximum size a detection wave can get
    public float MaxWaveSize { get { return maxWaveSize; } set { maxWaveSize = value; } }

    private Vector3 maxSize;

    private bool isGrowing;

    void Start()
    {
        Init();
    }

    void Init()
    {
        originalSize = transform.localScale;
    }

	void OnEnable()
    {
        InitiateGrowth();
    }

    void InitiateGrowth()
    {
        transform.parent = null;
        maxSize = new Vector3(maxWaveSize, maxWaveSize, maxWaveSize);
        isGrowing = true;
    }

    void FixedUpdate()
    {
        if (isGrowing)
            GrowWave();
    }   

    void GrowWave()
    {
        Vector3 currentSize = transform.localScale;
        float speed = waveGrowthSpeed * Time.deltaTime;
        Vector3 newSize = new Vector3(currentSize.x + speed, currentSize.y + speed, currentSize.z + speed);
        transform.localScale = newSize;

        if(newSize.x >= maxWaveSize)
        {
            ResetWave();
        }
    }

    void ResetWave()
    {
        isGrowing = false;

        transform.localScale = originalSize;
        transform.parent = originalParent;
        transform.localPosition = Vector3.zero;

        gameObject.SetActive(false);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            Debug.Log("Detected Player");
            originalParent.SendMessage("HitBySoundWave",other.transform, SendMessageOptions.DontRequireReceiver);
        }
    }
}
