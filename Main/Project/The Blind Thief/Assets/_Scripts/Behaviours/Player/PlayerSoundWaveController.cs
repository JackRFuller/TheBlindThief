using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSoundWaveController : MonoBehaviour
{
    [Header("Sound Waves")]
    [SerializeField] private GameObject soundWave;
    [SerializeField] private int numberToSpawn;
    [SerializeField] private Transform spawnPoint;

    private Animator playerAnim;

    private List<GameObject> soundWaves;

    void Start()
    {
        SpawnInSoundWaves();
        GetAnimator();
    }

    void GetAnimator()
    {
        playerAnim = GetComponent<Animator>();
    }

    void SpawnInSoundWaves()
    {
        soundWaves = new List<GameObject>();

        GameObject soundWaveHolder = new GameObject();
        soundWaveHolder.name = "SoundWaveHolder";

        for(int i = 0; i < numberToSpawn; i++)
        {
            GameObject _soundWave = (GameObject)Instantiate(soundWave);
            soundWaves.Add(_soundWave);
            _soundWave.transform.parent = soundWaveHolder.transform;
            _soundWave.SetActive(false);
        }
    }

    //TODO: Remove Animation Events
    void TriggerSoundWave()
    {
        //bool isSneaking;

        //if (playerAnim.GetBool("isWalking"))
        //    isSneaking = true;
        //else isSneaking = false;     

        //for(int i = 0; i < soundWaves.Count; i++)
        //{
        //    if (!soundWaves[i].activeInHierarchy)
        //    {
        //        soundWaves[i].transform.position = spawnPoint.transform.position;
        //        soundWaves[i].SetActive(true);
        //        soundWaves[i].GetComponent<PlayerSoundWaveBehaviour>().InitiateSoundWave(isSneaking);
        //        break;
        //    }
        //}
    }
}
