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

    [Header("Sound Waves")]
    [SerializeField] private GameObject enemySoundWave;
    [SerializeField] private int numberToSpawn;
    private List<GameObject> soundWaves;

    void Start()
    {
        SpawnInSoundWaves();
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
