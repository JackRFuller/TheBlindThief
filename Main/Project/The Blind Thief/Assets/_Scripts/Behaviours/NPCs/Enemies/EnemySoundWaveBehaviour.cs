using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EnemySoundWaveBehaviour : MonoBehaviour
{
    [SerializeField] private float maxSoundWaveSize;
    [SerializeField] private float soundWaveGrowthSpeed;
    [SerializeField] private AnimationCurve soundWaveGrowthCurve;
    private Vector3 soundWaveGrowTarget;
    private bool isGrowing;
    private bool foundPlayer;
    private float timeStartedGrowing;

    private AudioSource audioSource;

    private Transform originalParent;

    void OnEnable()
    {
        originalParent = transform.parent;
    }
    
    public void InitiateSoundWave()
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        soundWaveGrowTarget = new Vector3(maxSoundWaveSize, maxSoundWaveSize, 1);

        timeStartedGrowing = Time.time;
        isGrowing = true;
        audioSource.Play();

        transform.parent = null;
    }

    void Update()
    {
        if(isGrowing)
        {
            GrowSoundWave();           
        }
    }

    //void FixedUpdate()
    //{
    //    if(isGrowing)
    //    {
    //        if (!foundPlayer)
    //        {
    //            //CheckForPlayer();
    //        }
    //    }
            
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, transform.localScale.x * 0.5f);
    }

    /// <summary>
    /// Checks to see if the player is within the sound wave
    /// </summary>
    //void CheckForPlayer()
    //{
    //    Collider[]hitColliders = Physics.OverlapSphere(Vector3.zero, transform.localScale.x * 0.5f);

    //    for (int i = 0; i < hitColliders.Length; i++)
    //    {
    //        if(hitColliders[i].tag == "Player")
    //        {
    //            Debug.Log("SoundWave Hit Player");
    //            hitColliders[i].SendMessage("HitByEnemy", SendMessageOptions.DontRequireReceiver);
    //            foundPlayer = true;
    //            ResetSoundWave();

    //            break;
    //        }
            
    //    }
    //}

    void GrowSoundWave()
    {
        float timeSinceStarted = Time.time - timeStartedGrowing;
        float percentageComplete = timeSinceStarted / soundWaveGrowthSpeed;

        transform.localScale = Vector3.Lerp(transform.localScale, soundWaveGrowTarget, soundWaveGrowthCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 1.0f)
        {
            ResetSoundWave();
        }
    }

    void ResetSoundWave()
    {
        isGrowing = false;
        foundPlayer = false;
        gameObject.SetActive(false);
        audioSource.Stop();
        transform.parent = originalParent;
        transform.localPosition = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerHurtSpot")
        {
            other.transform.parent.SendMessage("HitByEnemy", SendMessageOptions.DontRequireReceiver);
            ResetSoundWave();
        }
    }
	
}
