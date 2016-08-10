using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private LevelMode levelMode;

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

    //Enums
    private enum LevelMode
    {
        Building,
        Testing,
    }


    public override void Awake()
    {
        base.Awake();

        if(levelMode == LevelMode.Testing)
            LoadInLevel();
    }

    void LoadInLevel()
    {
        Instantiate(currentLevel.levelGeometry);

        NodeController.Instance.GetNodes();
    }

    public void AcquireKey()
    {
        numberOfHeldKeys++;

        if (PlayerAcquiresKey != null)
            PlayerAcquiresKey();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
            QuitApplication();

        if (Input.GetKey(KeyCode.R))
            ResetGame();
    }

    void QuitApplication()
    {
        Application.Quit();
    }

    void ResetGame()
    {
        SceneManager.LoadScene(0);
    }
}
