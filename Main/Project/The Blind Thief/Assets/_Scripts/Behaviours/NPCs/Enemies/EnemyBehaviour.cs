using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyAttackType enemyAttackType;

    private EnemyState enemyState;
    public EnemyState currentEnemyState
    {
        get { return enemyState; }
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

    [Header("Projectiles")]
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private int numberOfProjectilesToSpawn;
    private List<GameObject> projectiles;
    [SerializeField] private float projectileMovementSpeed;
    [SerializeField] private float projectileMaxDistance; // Determines how far a projectile can travel

    //Enums
    public enum EnemyState
    {
        stalking,
        aggro,
    }

    public enum EnemyAttackType
    {
        Projectile,
        AreaOfEffect,
    }

    void Start()
    {
        SpawnInDetectionWaves();
        switch(enemyAttackType)
        {
            case (EnemyAttackType.AreaOfEffect):
                SpawnInSoundWaves();
                break;
            case (EnemyAttackType.Projectile):
                SpawnInProjectiles();
                break;
        }
       
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

            detectionWave.transform.parent = this.transform;

            detectionWaves.Add(detectionWave);
           
            detectionWave.SetActive(false);
        }
    }

    void SpawnInProjectiles()
    {
        projectiles = new List<GameObject>();

        for (int i = 0; i < numberOfProjectilesToSpawn; i++)
        {
            GameObject projectile = (GameObject)Instantiate(enemyProjectile);

            //Set Projectile Values
            EnemyProjectileBehaviour epbScript = projectile.GetComponent<EnemyProjectileBehaviour>();
            epbScript.ProjectileSpeed = projectileMovementSpeed;
            epbScript.MaxProjectileDistance = projectileMaxDistance;
            epbScript.OriginalParent = this.transform;

            projectile.transform.parent = this.transform;
            projectile.transform.localPosition = Vector3.zero;

            projectiles.Add(projectile);
            projectile.SetActive(false);
        }
    }

    void SpawnInSoundWaves()
    {
        soundWaves = new List<GameObject>();

        for (int i = 0; i < numberToSpawn; i++)
        {
            GameObject _enemySoundWave = (GameObject)Instantiate(enemySoundWave);
            soundWaves.Add(_enemySoundWave);
            _enemySoundWave.transform.parent = this.transform;
            _enemySoundWave.SetActive(false);
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

    /// <summary>
    /// Trigger by Send Message from Sound Wave Behaviour
    /// </summary>
	void HitBySoundWave(Transform playerPosition)
    {
        if(enemyState == EnemyState.stalking)
        {
            switch (enemyAttackType)
            {
                case (EnemyAttackType.AreaOfEffect):
                    SendOutSoundWave();
                    break;
                case (EnemyAttackType.Projectile):
                    SendOutProjectile(playerPosition);
                    break;
            }
        }
    }

    void SendOutProjectile(Transform playerPos)
    {
        for (int i = 0; i < projectiles.Count; i++)
        {
            if(!projectiles[i].activeInHierarchy)
            {
                EnemyProjectileBehaviour projectileScript = projectiles[i].GetComponent<EnemyProjectileBehaviour>();
                projectileScript.PlayerPosition = playerPos.position;
                projectiles[i].SetActive(true);
                break;
            }
        }
    }
    
    /// <summary>
    /// Looks for sound wave to spawn in
    /// </summary>
    void SendOutSoundWave()
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
