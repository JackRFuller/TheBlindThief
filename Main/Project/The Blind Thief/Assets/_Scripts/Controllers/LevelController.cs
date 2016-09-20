using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private LevelMode levelMode;
    
    //Level Elements
    [SerializeField]
    private LevelData currentLevel;
    public LevelData CurrentLevel  { get { return currentLevel; } }

    private int previousLevelIndex = 0;
    private GameObject currentLevelGeometry;

    private int numberOfHeldKeys;
    public int NumberOfHeldKeys { get { return numberOfHeldKeys; } }    

    //Enums
    private enum LevelMode
    {
        Building,
        Testing,
    }    

    void Start()
    {
        if (levelMode == LevelMode.Testing)
        {
            GetCurrentLevel();
            LoadInLevel();
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("NextLevel", IncrementLevel);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextLevel", IncrementLevel);
    }

    void GetCurrentLevel()
    {
        //Check it's not the first level
        if(GameController.Instance.LevelIndex > 0)
        {
            while (GameController.Instance.LevelIndex == previousLevelIndex)
                return;
        }
        
        currentLevel = GameController.Instance.Levels[GameController.Instance.LevelIndex];
        previousLevelIndex = GameController.Instance.LevelIndex;
    }

    void LoadInLevel()
    {
        currentLevelGeometry = Instantiate(currentLevel.levelGeometry);
        EventManager.TriggerEvent("NewLevel");
        StartCoroutine(NodeController.Instance.GetNodes());
    }

    public void AcquireKey()
    {
        numberOfHeldKeys++;
        EventManager.TriggerEvent("AcquiredKey");   
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
            QuitApplication();

        if (Input.GetKey(KeyCode.R))
            ResetGame();
    }

    public void IncrementLevel()
    {
        DestroyImmediate(currentLevelGeometry);

        GetCurrentLevel();
        LoadInLevel();
    }

    #region PlayerInput

    void QuitApplication()
    {
        Application.Quit();
    }

    void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    #endregion
}
