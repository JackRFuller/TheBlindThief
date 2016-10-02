using UnityEngine;
using System.Collections;

public class GameController : Singleton<GameController>
{
    //Level Data
    [SerializeField]
    private LevelData[] levels;
    public LevelData[] Levels { get { return levels; } }

    private int levelIndex;
    public int LevelIndex { get { return levelIndex; } }

    void Start()
    {
        Application.targetFrameRate = 30;
    }

    void OnEnable()
    {
        EventManager.StartListening("NextLevel", IncrementLevel);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextLevel", IncrementLevel);
    }

    public void IncrementLevel()
    {
        levelIndex++;
    }

    void Update()
    {

    }

    void Cheats()
    {
        if (Input.GetKeyUp(KeyCode.R))
            EventManager.TriggerEvent("Reset");
    }
}
