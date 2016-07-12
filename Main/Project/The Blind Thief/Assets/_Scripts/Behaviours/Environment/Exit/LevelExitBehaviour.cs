using UnityEngine;
using System.Collections;

public class LevelExitBehaviour : Singleton<LevelExitBehaviour>
{
    [SerializeField]
    private Collider doorCol;

    [Header("Key Holes")]
    [SerializeField]
    private GameObject[] keyHoles;
    [SerializeField]
    private SpriteRenderer[] filledKeyHoles;
    private int maxNumberOfKeys;
    private int currentNumberOfKeys;

    void Start()
    {
        InitiateKeyHoles();

        SubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        LevelController.Instance.PlayerAcquiresKey += PlayerHasAcquiredKey;
    }

    void InitiateKeyHoles()
    {
        maxNumberOfKeys = LevelController.Instance.CurrentLevel.numberOfKeys;

        for (int i = 0; i < maxNumberOfKeys; i++)
        {
            keyHoles[i].SetActive(true);
        }
    }

    void PlayerHasAcquiredKey()
    {
        filledKeyHoles[currentNumberOfKeys].enabled = true;
        currentNumberOfKeys++;

        if (currentNumberOfKeys == maxNumberOfKeys)
        {
            OpenDoor();
        }

    }

    void OpenDoor()
    {
        doorCol.enabled = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Level Complete");
        }
        
    }
}
