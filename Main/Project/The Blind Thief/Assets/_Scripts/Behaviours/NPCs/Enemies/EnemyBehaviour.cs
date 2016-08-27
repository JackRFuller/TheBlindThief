using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour
{
    private EnemyState enemyState;
    public EnemyState currentEnemyState
    {
        get { return enemyState; }
    }
    public enum EnemyState
    {
        stalking,
        aggro,
    }

    [Header("Detection Waves")]
    [SerializeField] private GameObject enemyDetectionWaves;
    [SerializeField] private int numberOfDetectionWavesToSpawn;
    private List<GameObject> detectionWaves;
    [SerializeField] private float detectionWaveMaxSize;
    [SerializeField] private float detectionWaveGrowthSpeed;
    [SerializeField] private float detectionWaveFrequency; //How many seconds between each wave
    private float detectionWaveTimer;    

    [Header("Sound Waves")]
    [SerializeField] private GameObject enemySoundWave;
    [SerializeField] private int numberToSpawn;
    private List<GameObject> soundWaves;

    void Start()
    {
        SpawnInDetectionWaves();
        SpawnInSoundWaves();
    }  
    
    void SpawnInDetectionWaves()
    {
        detectionWaves = new List<GameObject>();

        for(int i = 0; i < numberOfDetectionWavesToSpawn; i++)
        {
            GameObject detectionWave = (GameObject)Instantiate(enemyDetectionWaves);

            //Set Detection Wave Values
            EnemyDetectionWaveBehaviour edwbScript = detectionWave.GetComponent<EnemyDetectionWaveBehaviour>();
            edwbScript.MaxWaveSize = detectionWaveMaxSize;
            edwbScript.WaveGrowthSpeed = detectionWaveGrowthSpeed;
            edwbScript.OriginalParent = this.transform;

            detectionWaves.Add(detectionWave);
           
            detectionWave.SetActive(false);
        }
    }

    void Update()
    {
        DetectionWaveTimer();
    }

    void DetectionWaveTimer()
    {
        detectionWaveTimer += Time.smoothDeltaTime;
        if(detectionWaveTimer >= detectionWaveFrequency)
        {
            SendOutDetectionWave();
            detectionWaveTimer = 0;
        }
    }

    void SendOutDetectionWave()
    {
        for(int i = 0; i < detectionWaves.Count; i++)
        {
            if(!detectionWaves[i].activeInHierarchy)
            {
                detectionWaves[i].SetActive(true);
                break;
            }
        }
    }

    void SpawnInSoundWaves()
    {
        soundWaves = new List<GameObject>();

        for(int i = 0; i < numberToSpawn; i++)
        {
            GameObject _enemySoundWave = (GameObject)Instantiate(enemySoundWave);
            soundWaves.Add(_enemySoundWave);
            _enemySoundWave.transform.parent = this.transform;
            _enemySoundWave.SetActive(false);
        }
    }

    /// <summary>
    /// Trigger by Send Message from Sound Wave Behaviour
    /// </summary>
	void HitBySoundWave()
    {
        Debug.Log("Hit by Soundwave");
        if(enemyState == EnemyState.stalking)
        {
            InitiateCombat();
        }
    }
    
    /// <summary>
    /// Looks for sound wave to spawn in
    /// </summary>
    void InitiateCombat()
    {
        enemyState = EnemyState.aggro;

        for(int i = 0; i < soundWaves.Count; i++)
        {
            if (!soundWaves[i].activeInHierarchy)
            {
                soundWaves[i].SetActive(true);
                soundWaves[i].transform.position = transform.position;
                soundWaves[i].GetComponent<EnemySoundWaveBehaviour>().InitiateSoundWave();
                break;
            }
        }
    }
}
