using UnityEngine;
using System.Collections;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private LevelData currentLevel;
    public LevelData CurrentLevel
    {
        get { return currentLevel; }
    }

}
