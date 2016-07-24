using UnityEngine;
using UnityEngine.EventSystems;
using System;
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

    //Events
    public event Action EndOfLevel; //Used when the player enters the door

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
        doorCol.isTrigger = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");

        if (other.tag == "Player")
        {
            Debug.Log("Level Complete");

            if (EndOfLevel != null)
                EndOfLevel();
        }

    }
}
