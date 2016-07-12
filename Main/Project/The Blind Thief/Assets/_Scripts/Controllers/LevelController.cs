using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private LevelData currentLevel;
    public LevelData CurrentLevel
    {
        get { return currentLevel; }
    }

    private int numberOfHeldKeys;
    public int NumberOfHeldKeys
    {
        get { return numberOfHeldKeys; }
    }

    public event Action PlayerAcquiresKey; //Subscribed to by the Doors

    public void AcquireKey()
    {
        numberOfHeldKeys++;

        if (PlayerAcquiresKey != null)
            PlayerAcquiresKey();

        
    }
}
