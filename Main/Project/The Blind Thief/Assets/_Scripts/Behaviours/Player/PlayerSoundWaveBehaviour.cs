using UnityEngine;
using System.Collections;

public class PlayerSoundWaveBehaviour : MonoBehaviour
{
    [Header("Sound Wave Sizes")]
    [SerializeField] private float sneakSize;	
    [SerializeField] private float runSize;

    [Header("Components")]
    [SerializeField] private SpriteRenderer soundWaveSprite;
    [SerializeField] private Collider soundWaveCollider;
    
    [Header("Lerping")]
    [SerializeField] private float sneakGrowTime;
    [SerializeField] private float runGrowTime;
    [SerializeField] private AnimationCurve soundWaveGrowCurve;
    private Vector3 soundWaveSizeMax = Vector3.zero;
    private bool isLerping;
    private bool isSneaking;
    private float timeStartedLerping;

    /// <summary>
    /// Triggered by external script - bool is true if character is sneaking
    /// </summary>
    /// <param name="isSneaking"></param>
    public void InitiateSoundWave(bool _isSneaking)
    {
        soundWaveCollider.enabled = true;

        isSneaking = _isSneaking;

        soundWaveSizeMax = Vector3.zero;

        if (isSneaking)
            soundWaveSizeMax = new Vector3(sneakSize, sneakSize, 1);

        if (!isSneaking)
            soundWaveSizeMax = new Vector3(runSize, runSize, 1);

        timeStartedLerping = Time.time;
        isLerping = true;
    }

    void Update()
    {
        if (isLerping)
        {
            GrowSoundWave();            
        }            
    } 

    void GrowSoundWave()
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = 0;

        if (isSneaking)
            percentageComplete = timeSinceStarted / sneakGrowTime;
        else
            percentageComplete = timeSinceStarted / runGrowTime;

        Color _newColor = soundWaveSprite.color;

        _newColor.a = Mathf.Lerp(_newColor.a, 0, soundWaveGrowCurve.Evaluate(percentageComplete));

        soundWaveSprite.color = _newColor;

        transform.localScale = Vector3.Lerp(transform.localScale, soundWaveSizeMax, soundWaveGrowCurve.Evaluate(percentageComplete));

        if(percentageComplete >= 1.0f)
        {
            ResetSoundWave();
        }

    }

    void ResetSoundWave()
    {
        soundWaveCollider.enabled = false;
        soundWaveSprite.color = Color.white;
        transform.localScale = new Vector3(0.001f, 0.001f, 1);
        gameObject.SetActive(false);
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyBehaviour enemyBehaviour = other.GetComponent<EnemyBehaviour>();

            if(enemyBehaviour.currentEnemyState == EnemyBehaviour.EnemyState.stalking)
                other.SendMessage("HitBySoundWave", SendMessageOptions.DontRequireReceiver);
        }
    }

}
