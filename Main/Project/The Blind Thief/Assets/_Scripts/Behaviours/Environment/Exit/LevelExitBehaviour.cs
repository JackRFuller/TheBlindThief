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

    void Start()
    {
        InitiateKeyHoles();        
    }

    void OnEnable()
    {
        EventManager.StartListening("AcquiredKey", PlayerHasAcquiredKey);
    }

    void OnDisable()
    {
        EventManager.StopListening("AcquiredKey", PlayerHasAcquiredKey);
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
        if (other.tag == "Player")
        {
            EventManager.TriggerEvent("EndOfLevel");
        }

    }
}
