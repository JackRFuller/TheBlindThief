using UnityEngine;
using System.Collections;

public class LevelUIController : Singleton<LevelUIController>
{
    [Header("End of Level")]
    [SerializeField] private Animator endOfLevelScreen;

    void Start()
    {
        SubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        LevelExitBehaviour.Instance.EndOfLevel += EndOfLevel;
    }

    void EndOfLevel()
    {
        endOfLevelScreen.SetBool("isFadeIn",true);
    }

    public void NextLevel()
    {
        endOfLevelScreen.SetBool("isFadeIn", false);
        endOfLevelScreen.SetBool("isFadeOut", true);
    }
	
}
