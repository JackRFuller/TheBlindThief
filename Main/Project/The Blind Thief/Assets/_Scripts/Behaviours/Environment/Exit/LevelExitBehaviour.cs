using UnityEngine;
using System.Collections;

public class LevelExitBehaviour : Singleton<LevelExitBehaviour>
{
    [SerializeField] private GameObject[] keyHoles;
    private int numberOfKeys;

    void Start()
    {
        InitiateKeyHoles();
    }

    void InitiateKeyHoles()
    {
        numberOfKeys = LevelController.Instance.CurrentLevel.numberOfKeys;

        for (int i = 0; i < numberOfKeys; i++)
        {
            keyHoles[i].SetActive(true);
        }
    }

}
